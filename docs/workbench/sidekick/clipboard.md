The Clipboard Manager in Supervertaler Sidekick captures everything you copy to the clipboard and keeps a persistent history that survives application restarts. Click any item to paste it into the active window.

Open it via **Sidekick → Clipboard tab** (the clipboard icon).

---

## Two columns

The tab is split into two side-by-side panels:

- **Text snippets (left)** – plain text, rich text, and any other text copied from any application
- **Images (right)** – raster images (screenshots, copied graphics, etc.)

Each column has its own count in its header. A draggable splitter lets you resize the two panels.

---

## How clips are captured

The Clipboard Manager monitors the system clipboard in the background. Every time you copy something in any application – a word in Trados, a URL, a code snippet, a screenshot – it is added to the top of the relevant list automatically.

Duplicate copies of identical content are deduplicated (the existing item moves to the top instead of a new entry appearing).

**Capacity limits:**

| Kind | Maximum items |
|------|--------------|
| Text snippets | 200 |
| Images | 50 |

When a list is full, the oldest item is removed to make room.

---

## Pasting a clip

Click any item in the list to paste it. What happens:

1. The item is placed on the system clipboard.
2. Sidekick hides itself.
3. `Ctrl+V` is sent to whichever window was active before Sidekick opened.

After pasting, the item is marked as used and appears greyed out. This makes it easy to track which clips you have already inserted in a session.

## Deleting clips

**Single item** – right-click any entry and choose **🗑 Delete**, or select it and press the **Delete** key.

**All clips** – click **Clear all** in the top-right corner of the Clipboard tab, or right-click any entry and choose **Clear all**. This removes the entire history from both columns and cannot be undone.

---

## Keyboard shortcuts

| Key | Action |
|-----|--------|
| **Up / Down** | Move through items in the focused column |
| **Right** | Move focus from the text column to the image column |
| **Left** | Move focus from the image column back to the text column |
| **Enter** | Paste the selected item |
| **Delete** | Remove the selected item from history |

Use **Tab** and **Shift+Tab** to move focus between the Clipboard panel and the Sidekick right-pane Menu.

---

## Empty state

When a column contains no clips, a centred placeholder message is shown:

- Text column: *No text snippets yet – copy any text to start*
- Image column: *No images yet – copy any image to start*

---

## Persistence

The full clip history is stored in your [user data folder](../reference/faq.md) in a shared SQLite database. Items are available the next time you open Supervertaler Workbench.

---

## Related pages

- [Sidekick Overview](overview.md)
- [AutoFingers Voice Commands](autofingers.md)
