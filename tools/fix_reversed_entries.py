#!/usr/bin/env python3
"""
Fix reversed term entries in a Supervertaler termbase.

When terms were added from a project whose direction is the inverse of the
termbase's declared direction (e.g. NL->EN project adding to an EN->NL termbase),
older versions of Supervertaler stored source_term and target_term in the wrong
columns. This script detects and fixes those entries.

Usage:
    python fix_reversed_entries.py <db_path> <termbase_id> [--dry-run]

    --dry-run   Show what would be swapped without modifying the database.

Example:
    python fix_reversed_entries.py "path/to/supervertaler.db" 13 --dry-run
"""

import sqlite3
import sys
import os

# Handle Windows console encoding issues
sys.stdout.reconfigure(errors="replace")
sys.stderr.reconfigure(errors="replace")


def main():
    if len(sys.argv) < 3:
        print(__doc__)
        sys.exit(1)

    db_path = sys.argv[1]
    termbase_id = int(sys.argv[2])
    dry_run = "--dry-run" in sys.argv

    if not os.path.exists(db_path):
        print(f"Error: Database not found: {db_path}")
        sys.exit(1)

    conn = sqlite3.connect(db_path)
    conn.row_factory = sqlite3.Row

    # Get termbase info
    tb = conn.execute(
        "SELECT id, name, source_lang, target_lang FROM termbases WHERE id = ?",
        (termbase_id,)
    ).fetchone()

    if not tb:
        print(f"Error: Termbase with ID {termbase_id} not found.")
        conn.close()
        sys.exit(1)

    print(f"Termbase: {tb['name']} (ID {tb['id']})")
    print(f"Declared direction: {tb['source_lang']} -> {tb['target_lang']}")
    print()

    # Load all entries
    entries = conn.execute(
        "SELECT id, source_term, target_term, source_lang, target_lang FROM termbase_terms WHERE termbase_id = ?",
        (termbase_id,)
    ).fetchall()

    print(f"Total entries: {len(entries)}")
    print()

    # Identify entries where source_lang doesn't match the termbase's declared source_lang
    # These are the reversed entries (stored in project direction instead of termbase direction)
    reversed_entries = []
    for entry in entries:
        entry_src = (entry["source_lang"] or "").strip().lower()
        tb_src = (tb["source_lang"] or "").strip().lower()
        # If the entry's source_lang doesn't match the termbase's source_lang, it's reversed
        if entry_src and tb_src and entry_src != tb_src:
            reversed_entries.append(entry)

    if not reversed_entries:
        print("No reversed entries found (all entries' source_lang matches the termbase's declared source_lang).")
        print()
        print("If entries still appear wrong, they may have the same source_lang code.")
        print("In that case, manual review is needed.")
        conn.close()
        return

    print(f"Found {len(reversed_entries)} reversed entries (source_lang != termbase source_lang):")
    print()

    for entry in reversed_entries:
        print(f"  ID {entry['id']:>6}: {entry['source_term'][:50]:50s} | {entry['target_term'][:50]:50s}  ({entry['source_lang']} -> {entry['target_lang']})")

    print()

    if dry_run:
        print("[DRY RUN] No changes made. Run without --dry-run to fix these entries.")
        conn.close()
        return

    # Confirm
    answer = input(f"Swap source_term and target_term for {len(reversed_entries)} entries? (yes/no): ")
    if answer.strip().lower() != "yes":
        print("Aborted.")
        conn.close()
        return

    # Swap source_term <-> target_term AND source_lang <-> target_lang for reversed entries
    cursor = conn.cursor()
    for entry in reversed_entries:
        cursor.execute("""
            UPDATE termbase_terms
            SET source_term = ?, target_term = ?,
                source_lang = ?, target_lang = ?
            WHERE id = ?
        """, (
            entry["target_term"], entry["source_term"],
            entry["target_lang"], entry["source_lang"],
            entry["id"]
        ))

    # Also swap abbreviations if present
    try:
        cursor.execute("SELECT 1 FROM termbase_terms LIMIT 1")
        cols = [desc[0] for desc in cursor.description]
        if "source_abbreviation" in cols:
            for entry_row in reversed_entries:
                eid = entry_row["id"]
                abbr = conn.execute(
                    "SELECT source_abbreviation, target_abbreviation FROM termbase_terms WHERE id = ?",
                    (eid,)
                ).fetchone()
                if abbr:
                    cursor.execute("""
                        UPDATE termbase_terms
                        SET source_abbreviation = ?, target_abbreviation = ?
                        WHERE id = ?
                    """, (abbr["target_abbreviation"], abbr["source_abbreviation"], eid))
    except Exception:
        pass  # abbreviation columns may not exist in older schemas

    # Swap synonyms language labels
    try:
        for entry_row in reversed_entries:
            eid = entry_row["id"]
            # Swap 'source' <-> 'target' in language column of termbase_synonyms
            cursor.execute("""
                UPDATE termbase_synonyms
                SET language = CASE
                    WHEN language = 'source' THEN 'target'
                    WHEN language = 'target' THEN 'source'
                    ELSE language
                END
                WHERE term_id = ?
            """, (eid,))
    except Exception:
        pass  # synonyms table may not exist

    conn.commit()
    print(f"Fixed {len(reversed_entries)} entries.")
    conn.close()


if __name__ == "__main__":
    main()
