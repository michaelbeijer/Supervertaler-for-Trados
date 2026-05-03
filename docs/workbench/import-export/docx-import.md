# Importing DOCX Files

Use DOCX import for normal Word documents (not CAT tool bilingual formats).

DOCX import is handled by the bundled **Okapi sidecar** – an industry-standard localisation library that runs as a small background service. This gives you SRX-based segmentation, proper paragraph and table detection, and a faithful round-trip on export.

## Import steps

1. Go to **File → Import → DOCX...**
2. Select your `.docx` file
3. Choose the **source** and **target** languages when prompted
4. The document is segmented into rows in the translation grid – formatting tags (`<b>`, `<i>`, `<u>`, hyperlinks, runs) appear inline so you can preserve them in the translation

A progress dialogue shows extraction progress for large documents – on a 2,500-segment file expect a few seconds.

## What is preserved on round-trip

When you later export back to DOCX, Supervertaler reconstructs the original document via the same Okapi sidecar:

- **Layout**: paragraphs, tables, headers, footers, page breaks
- **Inline formatting**: bold, italic, underline, sub/superscript, colour
- **Hyperlinks**: anchor text and links round-trip identically – broken or working
- **Images and other non-translatable content**: passed through untouched

## Tips

- If you are working with memoQ/Trados/Phrase/CafeTran, prefer the specific CAT workflow import instead of generic DOCX.
- Keep formatting tags balanced when editing translations (e.g. `<b>text</b>`, not `<b>text`).
- Placeholder tags like `<hyperlink1>` and `<tags2/>` represent structural elements (hyperlinks, run boundaries). Leave them in the same position as the source so the export reconstructs the document correctly.

## When DOCX import is unavailable

The Okapi sidecar requires Java – Supervertaler ships a bundled JRE, so this is normally invisible. If the sidecar fails to start (Java missing, port 8090 blocked by another process, …) you'll see an "Okapi sidecar required" dialogue with troubleshooting steps. DOCX import won't fall back silently to a degraded engine.

{% hint style="info" %}
Need OCR? Use [PDF Rescue (OCR)](../tools/pdf-rescue.md) to turn scanned PDFs into editable DOCX before importing.
{% endhint %}
