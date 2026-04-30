# Supervertaler Sidekick

The **Supervertaler Sidekick** is a floating AI assistant window that gives you instant access to an AI chat, instant machine translation (QuickTrans), text conversions, snippets, and your custom prompts -- from anywhere on your computer.

## Opening the Assistant

### Inside Supervertaler

| Method | Shortcut |
|--------|----------|
| Supervertaler Sidekick | **Ctrl+Q** |

### From any application (system-wide)

| Method | Shortcut |
|--------|----------|
| Supervertaler Sidekick (Chat) | **Alt+K** |
| QuickTrans (instant translation) | **Ctrl+Alt+Q** |

Select text in any application (Word, memoQ, Trados, browser, etc.), then press the shortcut. The assistant opens with the selected text ready to use.

- **Alt+K** opens the **Chat** tab for AI conversation
- **Ctrl+Alt+Q** opens the **QuickTrans** tab and immediately fetches translations from all enabled providers

::: info
If a shortcut doesn't work, another application may have claimed it. You can change shortcuts in **Settings > Keyboard Shortcuts > Global**.
:::

## Window Layout

The assistant has two panels separated by a draggable splitter:

- **Left panel** -- two tabs: **Chat** and **QuickTrans**
- **Right panel** -- expandable action menu with tools, prompts, snippets, and text conversions

The window size, position, and splitter proportions are remembered across sessions.

## Chat Tab

The Chat tab is a full AI assistant with the same capabilities as the chat in the AI tab:

- Type messages and press **Enter** to send (Shift+Enter for new line)
- **Ctrl+V** to paste images from the clipboard (works with vision-capable models)
- Click the **model indicator** next to Send to switch AI provider/model on the fly
- **Context chips** (Document, TMs, Termbases, Files) toggle what data is included in the AI context. Right-click a chip for details or to attach files

### Shared Conversation

All chat views share the same conversation: the grid-side AI Assistant panel, the AI tab's Assistant sub-tab, and the floating assistant window all show the same messages and stay in sync.

## QuickTrans Tab

The QuickTrans tab provides instant machine translation from multiple providers simultaneously, similar to [GT4T](https://gt4t.cn/).

### How It Works

1. **From an external app:** Select text, press **Ctrl+Alt+Q** -- translations appear immediately
2. **From the tab:** Type or paste text in the input field at the bottom, press Enter or click **Translate**

### Translation Results

Results arrive as they complete from each provider and are displayed in a compact list with provider icons:

- **MT engines** -- Google Translate, DeepL, Microsoft, Amazon, ModernMT, MyMemory
- **AI models** -- Claude, OpenAI, Gemini, Mistral, Ollama, and any custom provider

### Selecting a Translation

| Method | Action |
|--------|--------|
| **Press 1--9** | Instantly insert the numbered translation |
| **Arrow keys + Enter** | Navigate and select |
| **Click** | Copy the translation to clipboard |

When launched from an external app, selecting a translation hides the assistant, returns focus to the source application, and pastes the result over your selection.

### Configuring Providers

Enable or disable individual MT and AI providers in **Settings > QuickTrans**. Each provider can be toggled independently.

## Action Menu

The right panel has expandable categories. Click a category heading to expand or collapse it. Use **arrow keys** to navigate and **Enter** to activate.

### Workbench Tools

- **QuickTrans** -- switches to the QuickTrans tab and translates the input text
- **Superlookup** -- concordance search across TMs, glossaries, and web resources

### Prompts

Your custom prompts from the Prompt Manager, grouped by folder. Clicking a prompt runs it against the input text and shows the AI response in the chat.

### Special Characters

Quick-insert symbols, arrows, primes, dashes and quotes, currency signs, legal symbols, maths operators, and bullet characters. Clicking an item copies it to the clipboard and (if launched from an external app) pastes it over your selection.

### Personal Snippets

Frequently used text snippets (e.g. phone numbers, addresses, boilerplate text). Clicking an item copies it to the clipboard and pastes it back.

### Text Conversions

Transform selected text:

| Conversion | Result |
|-----------|--------|
| Uppercase | SELECTED TEXT |
| Lowercase | selected text |
| Title Case | Selected Text |
| Sentence case | Selected text |
| Single curly quotes | 'Selected text' |
| Double curly quotes | \u201CSelected text\u201D |
| Round brackets | (Selected text) |
| Square brackets | [Selected text] |
| Remove soft hyphens | Strips invisible U+00AD characters |
| Double to single quotes | Replaces \u201C with \u2018 |
| Make bold | Wraps in `<b>...</b>` HTML tags |

**Direct action flow:** When launched from an external app, text conversions and snippets are *direct actions* -- the assistant hides immediately, returns focus to the source application, and pastes the result over your selection.

## Keyboard Navigation

| Key | Action |
|-----|--------|
| **Arrow keys** | Navigate menu items or QuickTrans results |
| **1--9** | Select QuickTrans result by number |
| **Enter** | Activate selected item |
| **Tab** | Switch focus between the action menu and the input |
| **Escape** | Close the assistant |

## Context Chips

The context chips row appears above the chat input:

| Chip | Left-click | Right-click |
|------|-----------|-------------|
| Document | Toggle document context | View project/language info |
| TMs | Toggle TM data inclusion | Browse available TMs |
| Termbases | Toggle termbase inclusion | Browse available termbases |
| Files | Toggle file context | Attach files or view attached files |

## Customising Hotkeys

You can change the default keyboard shortcuts in **Settings > Keyboard Shortcuts**:

| Default shortcut | Action |
|------------------|--------|
| **Ctrl+Q** | Open Supervertaler Sidekick (in-app) |
| **Alt+K** | Open Supervertaler Sidekick (global, from any app) |
| **Ctrl+Alt+Q** | Open QuickTrans (global, from any app) |
| **Ctrl+M** | QuickTrans (in-app, direct) |

## Tips

- **Ctrl+Alt+Q is the fastest way to translate** -- select text anywhere, press the shortcut, results appear instantly
- **Press Alt+K without selecting text** to open the assistant for general AI chat
- **Use text conversions** for quick formatting changes without leaving your current app
- **Right-click context chips** to browse and select specific TMs or termbases
- **Paste images** (Ctrl+V) to ask the AI about screenshots or diagrams
- The assistant works without a project open -- useful for general-purpose AI lookups
