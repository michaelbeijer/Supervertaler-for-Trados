Supervertaler has two kinds of keyboard shortcuts: **editor shortcuts** (active inside the translation grid) and **global hotkeys** (active system-wide, from any application).

## Managing shortcuts

Open **Settings → Keyboard Shortcuts**. The panel is split into two tabs:

- **Editor** – shortcuts for navigating and editing segments
- **Global** – OS-level hotkeys registered via the global hotkey manager

Click any shortcut row to edit it. Press the new key combination, then save.

{% hint style="info" %}
Global hotkey changes apply immediately — no restart needed.
{% endhint %}

## Default global hotkeys

| Shortcut | Action |
|----------|--------|
| **Alt+K** | Show / hide Sidekick |
| **Ctrl+Shift+C** | Open Sidekick directly to the Clipboard tab |
| **Ctrl+K** | Open Sidekick to SuperLookup (searches selected text; editor only) |
| **Ctrl+Alt+L** | Open SuperLookup from any application (searches selected text) |
| **Ctrl+Alt+D** | Push-to-talk dictation (one utterance) |
| **Ctrl+Alt+A** | Toggle AutoFingers Always-On listening |

{% hint style="warning" %}
**macOS: global hotkeys are currently disabled.** On macOS 26 (Sequoia), the underlying `pynput` library crashes the app when a global hotkey fires – the Text Services Manager hard-asserts that input-source calls happen on the main thread, but `pynput`'s keyboard listener runs on a background thread and triggers a process-wide abort the first time any registered hotkey is pressed. As a temporary fix in v1.9.419, all global hotkeys are disabled on macOS to prevent the crash. **Mac users summon Sidekick / SuperLookup / QuickTrans via the menu-bar tray icon** instead. The in-app shortcuts (Cmd+K etc.) still work when Supervertaler itself is the focused application – the limitation is only that they don't fire when another app has focus.

A proper Cocoa-native global-hotkey backend is tracked in [Workbench issue #188](https://github.com/Supervertaler/Supervertaler-Workbench/issues/188). Note that even with the future fix in place, Cmd+K conflicts with Finder's "Connect to Server…" binding, so Mac users may want to remap to Ctrl+Shift+K, Opt+Shift+K, or Cmd+Shift+K via Settings → Keyboard Shortcuts.
{% endhint %}

{% hint style="info" %}
On macOS, the **Alt** modifier in the table above is automatically translated to **Cmd** at registration time (e.g. **Alt+K** → **Cmd+K**). Once the Cocoa-native backend lands, this mapping will continue to apply unless you explicitly override the shortcut in Settings.
{% endhint %}

## Default editor shortcuts

See the full list on [Editor Keyboard Shortcuts](../editor/keyboard-shortcuts.md).

## Sidekick keyboard navigation

| Shortcut | Action |
|----------|--------|
| **Ctrl+Tab** | Next Sidekick tab |
| **Ctrl+Shift+Tab** | Previous Sidekick tab |
| **Tab** | Move focus to Sidekick Menu |
| **Left** | Return focus to last active Sidekick panel |
| **Esc** | Hide Sidekick |

## Related pages

- [Editor Keyboard Shortcuts](../editor/keyboard-shortcuts.md)
- [AutoFingers Voice Commands](../sidekick/autofingers.md)
- [Sidekick Overview](../sidekick/overview.md)
