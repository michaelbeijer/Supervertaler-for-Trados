AutoFingers is Supervertaler's voice command and dictation engine. It lets you control any application on your computer – Trados, memoQ, Word, or anything else in the foreground – using your voice, while Supervertaler Workbench stays running in the background.

Open it via **Sidekick → AutoFingers tab** (the microphone icon), or press **Ctrl+Alt+A** to toggle Always-On listening from anywhere.

![](<../../.gitbook/assets/Supervertaler-Workbench-Sidekick-AutoFingers.png>)

---

## Two modes

### Always-On listening

Always-On runs a continuous microphone stream in the background. When you speak, AutoFingers detects speech, transcribes it, and checks it against your command list. If it matches a command the action fires; if not (and "commands only" is off), the transcribed text is typed into whichever window is in the foreground.

**To start:** click **▶ Start Always-On** in the AutoFingers tab, or press **Ctrl+Alt+A** from any application. A red mic icon appears in the system tray while Always-On is active.

**To stop:** click **⏹ Stop Always-On** or press **Ctrl+Alt+A** again.

**Focus matters:** AutoFingers sends keystrokes and text to whichever window is currently focused. After starting Always-On, click into Trados, Word, or your browser before you speak.

### Push-to-Talk (F9 / Ctrl+Alt+D)

Press **F9** (inside the Workbench editor) or **Ctrl+Alt+D** (globally, from any application) to record a single utterance. Recording stops when you release the key or after silence is detected. The transcribed text is inserted at the cursor position.

**F9 modes** (configurable in the Push-to-Talk settings):
- **Toggle** (default) – press once to start, press again to stop
- **Hold-to-talk** – hold the key to record, release to stop

---

## Voice commands

Voice commands execute specific actions when you speak a trigger phrase. They can type text, press keyboard shortcuts, run AutoHotkey scripts, or call internal Workbench functions.

### The commands table

The commands table (right side of the AutoFingers tab) lists all your configured commands.

| Column | Description |
|--------|-------------|
| ☑ | Enable/disable checkbox – uncheck to silence a command without deleting it |
| Phrase | The primary trigger word or phrase |
| Aliases | Alternative phrases that also trigger the command |
| Type | Command / Keystroke / AHK Script / AHK Inline |
| Action | What happens when the phrase is recognised |
| Category | Organisational label (Navigation, Editing, etc.) |

### Enabling and disabling commands

- **Single command** – click the checkbox in the first column
- **All commands** – click the checkbox column header to toggle all at once (enables any disabled, or disables all if all are already active)
- **Multiple commands** – select rows with **Shift+Click** or **Ctrl+Click**, then right-click and choose **✅ Activate** or **⬜ Deactivate**

Disabled commands are greyed out and are skipped during recognition. Their settings are preserved.

### Editing a command

Double-click any row to open the Edit Voice Command dialog. You can change the phrase, aliases, action type, and action value.

You can also use the **Edit** button in the toolbar above the table.

### Adding a command

Click **+ Add** above the table. Choose a command type:

- **Command** – calls an internal Workbench action (confirm segment, next segment, etc.)
- **Keystroke** – sends a key combination to the active window (e.g. `ctrl+enter`)
- **AHK Script** – runs an AutoHotkey v2 script file
- **AHK Inline** – runs a short AutoHotkey v2 snippet directly

{% hint style="info" %}
Keystroke commands use `SendInput` under the hood, which works correctly with WPF applications like Trados Studio. Most standard key names are supported: `ctrl`, `alt`, `shift`, `enter`, `tab`, `up`, `down`, `left`, `right`, `f1`–`f12`, etc.
{% endhint %}

### Removing a command

Select the row and click **Remove**, or select multiple rows and remove them together.

---

## Settings

### Speech recognition engine

| Engine | Speed | Accuracy | Internet required |
|--------|-------|----------|-------------------|
| Local Whisper (offline) | Slower | Good | No |
| OpenAI Whisper API | Fast | Excellent | Yes |

The OpenAI API option is recommended for Always-On use because it responds significantly faster and handles accents well. It requires an OpenAI API key in **Settings → AI Settings**.

### Whisper model (local engine only)

Larger models are more accurate but slower and require more RAM.

| Model | Download size | Notes |
|-------|--------------|-------|
| tiny | ~75 MB | Very fast, lowest accuracy |
| base | ~142 MB | Good balance (recommended) |
| small | ~466 MB | Noticeably better accuracy |
| medium | ~1.5 GB | High accuracy |
| large | ~2.9 GB | Best accuracy, slow on CPU |

### Mic sensitivity

Controls the amplitude threshold used to detect speech onset.

- **Low (noisy)** – raises the threshold; ignores quiet background sounds but may miss soft speech
- **Medium (normal)** – default; works well in a typical home office
- **High (quiet)** – lowers the threshold; captures quiet voices but may trigger on background noise

### Listen for commands only

When checked, Always-On fires voice commands but discards any speech that doesn't match a command – it is not typed. Use this if you only want voice control, not dictation.

When unchecked, unmatched speech is transcribed and typed at the cursor position.

### Maximum recording duration

Sets the upper limit (in seconds) for a single voice clip. Speech that exceeds this length is cut and transcribed up to the limit. Useful to prevent long silences from being held open indefinitely.

### Language

- **Auto** – uses the project's target language as the transcription hint
- Explicit language – forces Whisper to transcribe in the selected language, which improves accuracy when the target language differs from the source

---

## AutoHotkey integration

AutoHotkey v2 must be installed for AHK-type commands to work. Supervertaler checks for it automatically and shows the path in the AutoHotkey section of the AutoFingers settings panel.

To verify: the status line shows either the AHK path (green) or "AutoHotkey v2 not found" (orange).

Click **Open scripts folder** to open the folder where standalone AHK script files are stored.

---

## Using AutoFingers with Trados Studio

AutoFingers sends input at the Win32 hardware-input level (equivalent to physical keystrokes), which is fully compatible with Trados Studio's WPF editor. Useful commands to add:

| Phrase | Type | Action |
|--------|------|--------|
| "confirm segment" | Keystroke | `ctrl+enter` |
| "next segment" | Keystroke | `alt+down` |
| "previous segment" | Keystroke | `alt+up` |
| "go to top" | Keystroke | `ctrl+home` |
| "undo" | Keystroke | `ctrl+z` |

After creating a command, start Always-On, click into Trados Studio, and speak the phrase.

---

## Global hotkeys

| Shortcut | Action |
|----------|--------|
| **Ctrl+Alt+A** | Toggle Always-On listening |
| **Ctrl+Alt+D** | Push-to-talk (one utterance) |
| **F9** | Push-to-talk (inside Workbench editor) |

Hotkeys can be customised in **Settings → Keyboard Shortcuts → Global**.

---

## Related pages

- [Sidekick Overview](overview.md)
- [Keyboard Shortcuts](../settings/shortcuts.md)
- [AI Settings](../settings/general.md)
