{% hint style="info" %}
You are viewing help for **Supervertaler for Trados** – the Trados Studio plugin. Looking for help with the standalone app? Visit [Supervertaler Workbench help](https://supervertaler.gitbook.io/help/workbench/).
{% endhint %}

Configure how TermLens loads and displays terminology in Trados Studio.

## Accessing TermLens settings

Click the **gear icon** in the TermLens panel, or open the plugin **Settings** dialogue and switch to the **TermLens** tab.

## Database path

The path to your Supervertaler termbase `.db` file. Click **Browse** to select a database, or **Create New** to start with an empty one.

{% hint style="info" %}
**Auto-detect:** If Supervertaler Workbench is installed on the same machine, the plugin can automatically detect its default database location. Click **Auto-detect** to find and use it.
{% endhint %}

## Termbase toggles

Each Supervertaler termbase in the database has three toggles. See [Termbase Management](../termbase-management.md) for full details.

| Toggle | Purpose |
|--------|---------|
| **Read** | Load terms for matching –only termbases with Read enabled appear in TermLens |
| **Write** | Receive new terms added via the [quick-add shortcuts](../termlens/adding-terms.md) |
| **Project** | Mark as the project termbase (shown in pink, prioritised) |

### Confirm dialog for non-matching termbases

When you tick **Write** or **Project** on a termbase whose declared language pair does not match the active project (for example, ticking an EN→NL termbase as Write while you have a DE→FR project open), a confirmation dialog appears:

> *"&lt;termbase&gt;" is a EN → NL termbase, but the active project's source language is German. Setting it as a Write termbase means new terms added during this project will be written into a termbase whose language pair doesn't match.*
>
> *This is occasionally intentional (multilingual or global termbases, bootstrapping a new direction) – tick "Yes" to continue. The plugin will remember this choice for this termbase and won't ask again until you untick the box.*

Click **Yes** to keep the tick, or **No** to revert it.

The plugin remembers each "Yes" answer per termbase, so once you have explicitly confirmed a non-matching termbase you are not asked again on subsequent ticks. **Unticking the box clears that confirmation** – a future re-tick will re-ask. This is intentional: an untick is taken as a clear signal that you are reconsidering, so the next tick deserves a fresh look.

The header **tick-all** on the Write column also respects this guard – non-matching termbases that haven't been individually confirmed are skipped during a bulk tick, so a quick "tick everything" can't accidentally enable unrelated termbases for write.

The **Read** column is intentionally exempt from the dialog – there is no harm in *reading* a non-matching termbase (its terms simply won't match anything in your segments), only in writing into it.

{% hint style="info" %}
**No project loaded.** If you open the Settings dialog without an active project (for example by clicking the gear from the QuickLauncher header), the plugin has no source language to compare against, so the confirmation is suppressed and ticks behave normally.
{% endhint %}

## MultiTerm termbases

If your Trados project has MultiTerm termbases (`.sdltb` files) attached, they appear at the bottom of the termbase list with a **[MultiTerm]** label and a light green row background. The **Read** toggle controls visibility in TermLens; **Write** and **Project** are always disabled because MultiTerm termbases are read-only.

To add or remove MultiTerm termbases, use Trados Studio's **Project Settings > Language Pairs > Termbases**. See [MultiTerm Support](../multiterm-support.md) for full details.

## Auto-load on startup

When enabled, the plugin automatically loads the termbase database when Trados Studio opens. This means terms are available immediately when you start translating, without needing to open the settings first.

If disabled, the termbase loads the first time you open the TermLens settings or click the TermLens panel.

## Case-sensitive matching

By default, TermLens matches terms regardless of letter case – "polymer", "Polymer", and "POLYMER" all match the same term entry. Enable **"Enable case-sensitive matching globally"** to require exact case matching across all termbases.

You can also control case sensitivity per termbase using the **CS** checkbox in the termbase grid. When the CS checkbox is ticked for a termbase, that termbase always matches case-sensitively; when unticked, it matches case-insensitively.

{% hint style="info" %}
**Tip:** The CS checkbox is useful when you have one termbase with abbreviations that must match exactly (e.g., "GC" should not match "gc") while other termbases should remain case-insensitive.
{% endhint %}

## Panel font size

Adjust the font size used in the TermLens display panel. Valid range: **7 pt** to **16 pt**.

Increase the font size if TermLens text is hard to read; decrease it to fit more terms on screen.

## Term shortcuts

Choose how Alt+digit shortcuts work when a segment has more than 9 matched terms:

* **Sequential** (default) – type the term number digit by digit. Alt+45 inserts term 45. Badges show clean sequential numbers (10, 11, 12, ...). There is a brief delay after each digit while the system waits for a possible next digit.
* **Repeated digit** – press the same digit key multiple times. Alt+55 inserts term 14 (the 5th term in the second tier). Badges show repeated digits (11, 22, 333, ...). No delay, but the badges are less intuitive.

Both modes behave identically when a segment has 9 or fewer matches – pressing Alt+N inserts immediately with no delay.

## Shortcut delay

Controls how long the system waits for the next digit in **Sequential** mode (in milliseconds). Default: **1100 ms**. Valid range: **300 ms** to **3000 ms**.

Increase the delay if you need more time between keystrokes. Decrease it if you find the pause too long when inserting single-digit terms in segments with 10+ matches. This setting has no effect in Repeated digit mode.

See [Keyboard Shortcuts](../keyboard-shortcuts.md) for the full reference.

---

## See Also

- [Termbase Management](../termbase-management.md)
- [MultiTerm Support](../multiterm-support.md)
- [AI Settings](ai-settings.md)
- [TermLens (Workbench)](https://supervertaler.gitbook.io/supervertaler/glossaries/termlens)
