# RWS App Store Manager – v4.19.38.0

**Version number:** `4.19.38.0`
**Minimum studio version:** `18.0`
**Maximum studio version:** `18.9`
**Checksum:** `1ab5f63966ac103650131ff7f0da29556e1287dfa7f238376212225b5f419a73`

This release covers everything since v4.19.25.0. Headline item: a new floating TermLens popup for keyboard-only term insertion, ideal for laptop / small-screen workflows where keeping the docked TermLens panel always visible costs too much vertical space. Also: a fix for the Quick-Add merge prompt silently missing partial-duplicate matches in reverse-direction termbases, a root-cause fix for a TermLens handler leak that could cause repeated term insertions across document close/reopen cycles, and a small shortcut reshuffle to avoid Trados Studio collisions.

---

## Changelog

### Added

- **New floating TermLens popup for keyboard-only term insertion.** A borderless popup that mirrors the docked TermLens panel for the active segment. Aimed at small-screen / laptop workflows and at translators who want to insert terms without leaving the keyboard. The popup reuses the docked panel's already-loaded matcher and termbase index (no reload), and renders the same chips with the same colour scheme.
  - **Open / close:** Ctrl-tap toggles the popup; Ctrl+Alt+G is the explicit-key fallback.
  - **Cycle highlighted match:** Right / Down / Tab (next), Left / Up / Shift+Tab (previous). One match is highlighted as "current" with an amber ring around the source word.
  - **Insert:** Enter inserts the highlighted match into the target segment and closes the popup; clicking a chip also inserts. Focus returns to the target cell after insert so typing continues uninterrupted.
  - **Edit term:** E opens the term-entry editor for the highlighted match (same multi-entry / multi-termbase flow as the docked panel's right-click "Edit Term…" menu). On a MultiTerm (green) match, E flashes a brief read-only hint pointing to Trados → Termbase Viewer instead, since MultiTerm entries are read-only.
  - **Close without inserting:** Esc, second Ctrl-tap, or second Ctrl+Alt+G.
  - The popup positions itself near the cursor and clamps to the working area; width and height scale with the screen so long patent-style segments aren't truncated.

### Changed

- **Term Picker dialogue shortcut is now Ctrl+Shift+P** (previously Ctrl+Shift+L). Ctrl+Shift+L collides with Trados Studio's own termbase-entry-listing shortcut, so the user-facing combo had to move; Ctrl+Shift+P avoids all known Trados and Supervertaler bindings and follows the VS Code-style "command palette" convention (P for Picker). Plugin-internal action ID is unchanged (`TermLens_TermPicker`) so any user-customised shortcut remappings survive the rename.

### Fixed

- **Quick-Add merge prompt now fires correctly for partial duplicates in reverse-direction termbases.** Regression introduced in v4.19.13. For any termbase declared in the inverse direction of the project (e.g. an EN→NL termbase used in an NL→EN project), the merge-candidate search was comparing the wrong DB columns and silently missing every potential match – a near-duplicate entry was created instead of offering merge. The merge-candidate search and the Merge Prompt dialogue now both apply the per-termbase column swap consistently. Reported by a user Quick-Adding *"even more preferably between"* against an EN→NL termbase that already held *"still more preferably between"* for the same Dutch source.
- **TermLens handler leak across document close/reopen cycles.** Root-cause fix for a class of bugs where a single chip click in the docked TermLens panel could fire two, three, or N target-cell replacements depending on how many times the view-part had been recreated in the session (document close/reopen, project switch, layout change). `TermLensEditorViewPart.Initialize` was subscribing event handlers to static singletons but `Dispose` only unsubscribed the per-document handlers, so the dead instances stayed reachable through the singletons' invocation lists and continued to dispatch. All seven handlers are now correctly unsubscribed on dispose. This was also the underlying cause of the floating popup's double-insert bug, which a separate popup-side guard had been working around.

For the full changelog, see: https://github.com/Supervertaler/Supervertaler-for-Trados/blob/main/CHANGELOG.md
