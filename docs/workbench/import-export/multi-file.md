# Multi-File Projects

Multi-file projects let you import a whole folder of files as a single Supervertaler project.

## Import a folder

1. Go to **File → Import → Folder (Multiple Files)...**
2. Choose a folder containing supported files (DOCX/TXT/MD)
3. Select which files to include
4. Choose the **source** and **target** languages

## How it behaves

- All segments live in one grid, but each segment is associated with a source file.
- You can jump between files and track progress per file.
- Each DOCX file is imported via the **Okapi sidecar** (the same engine that handles single-file DOCX import) – you get SRX segmentation, faithful round-trip, and hyperlinks/structural tags preserved per file.
- TXT and MD files use the simple per-line import.
- Original files are backed up to a `_source_files/` folder inside the project folder so the export can reconstruct each document faithfully.

## Export

When you export a multi-file project:

- Each DOCX file is reconstructed via the Okapi sidecar's `/merge` endpoint, using the original from `_source_files/` as the template. Layout, formatting, hyperlinks, and tables round-trip identically.
- TXT/MD files are written with the same per-line structure as the source.
- Output files land in the destination folder you choose, named `<original>_translated.<ext>`.

## Tips

- Use this when you receive a set of related files (e.g. claim documents, manual chapters split into separate files, UI strings split across documents).
- Export is done in one operation – pick the destination folder and Supervertaler writes all the translated files at once.

## Requirements

- The Okapi sidecar must be running before you import any folder containing DOCX files. Supervertaler checks this up-front and shows an "Okapi sidecar required" dialogue if it can't reach the sidecar – better than failing halfway through importing twenty files.
