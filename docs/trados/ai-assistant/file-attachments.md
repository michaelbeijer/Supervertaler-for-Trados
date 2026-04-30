# File Attachments

{% hint style="info" %}
You are viewing help for **Supervertaler for Trados** -- the Trados Studio plugin. Looking for help with the standalone app? Visit [Supervertaler Workbench help](https://supervertaler.gitbook.io/help/workbench/).
{% endhint %}

The Supervertaler Assistant supports attaching both images and documents to your messages. Use the **paperclip button** next to the chat input, or drag and drop files directly onto the chat area.

## Images

Attach images for visual context -- for example, a screenshot of the source document layout, a reference image, or a table that is hard to describe in text. Images are sent to the AI using each provider's native vision API.

| Method        | How                                                 |
| ------------- | --------------------------------------------------- |
| Paste         | Press **Ctrl+V** with an image on the clipboard     |
| Drag and drop | Drag an image file into the chat input area         |
| Browse        | Click the paperclip button and select an image file |

Supported image formats: PNG, JPEG, GIF, WebP, BMP. Up to **5 images** per message, **10 MB** maximum per image.

## Documents

Attach documents to provide the AI with additional reference material -- for example, a client style guide, a glossary in spreadsheet form, a reference PDF, or a translation memory export. The text content is automatically extracted from the document and included in your message as context.

| Method        | How                                                    |
| ------------- | ------------------------------------------------------ |
| Drag and drop | Drag a document file into the chat input area          |
| Browse        | Click the paperclip button and select a document file  |

The chat bubble shows a compact summary (file name and size) instead of the full extracted text, keeping the conversation readable.

### Supported Document Formats

| Category           | Formats                            |
| ------------------ | ---------------------------------- |
| Documents          | DOCX, DOC, PDF, RTF                |
| Presentations      | PPTX, PPT                         |
| Spreadsheets       | XLSX, XLS, CSV, TSV                |
| Translation files  | TMX, SDLXLIFF, XLIFF/XLF, TBX     |
| Text and markup    | TXT, Markdown, HTML, JSON, XML     |

{% hint style="info" %}
Up to **5 documents** per message, **20 MB** maximum per file. Very large documents are automatically truncated to avoid exceeding AI context limits. Legacy binary formats (DOC, XLS, PPT) use best-effort text extraction -- for best results, save as the modern format (DOCX, XLSX, PPTX) first.
{% endhint %}

{% hint style="success" %}
**Tip:** Attaching a client style guide or reference document alongside your translation question gives the AI much better context for providing accurate, style-consistent suggestions.
{% endhint %}

## See Also

* [Supervertaler Assistant](../ai-assistant.md) -- Overview
* [Context Awareness](context-awareness.md) -- What context is sent automatically
