When you have **Supervertaler for Trados** running with the AI Assistant panel active, the floating Sidekick Chat tab automatically picks up the project context from Trados — the active segment, surrounding segments, TM matches, termbase hits, and project metadata. You can ask the chat questions about your real translation work without switching away from Trados Studio.

This is especially useful on small laptop screens where there's not enough room to keep the in-Trados Assistant panel visible alongside the editor. **Alt+K** summons Sidekick over Trados; **Esc** dismisses it; **Ctrl+Tab** cycles between Chat, SuperLookup, Clipboard, and AutoFingers.

## How it works

Supervertaler for Trados runs a tiny localhost-only HTTP service called the **Sidekick Bridge** while the AI Assistant panel is active. Sidekick Chat detects the bridge automatically and uses it to fetch the current Trados project state on every message you send.

Nothing leaves your computer — the bridge listens only on `127.0.0.1`, requires a per-session authentication token, and is never reachable from outside the machine.

## The 🔗 Trados chip

Above the chat input you'll see a row of context chips (Document, TMs, Termbases, Files). When the Trados plugin is detected, a fifth chip appears:

* **Hidden** until the bridge is detected for the first time. Users without the Trados plugin never see this chip.
* **Lit green** — bridge is reachable; Trados context is included in chat messages.
* **Greyed** — the bridge was previously available but is now unreachable (e.g. you closed Trados mid-session). The chip recovers automatically when Trados restarts.

Click the chip to toggle it off if you want to ask a non-Trados-related question without the project context being included. The chip pref is shared across all chat views — toggling it in one place affects every send path.

## What the chat sees

When the chip is on, every message you send to Sidekick Chat is preceded by a context block that contains:

| Field | What it is |
|-------|------------|
| **Project name and file name** | So the AI knows what document you're working on |
| **Source and target language** | The language pair from Trados, used as a sanity-check against the chat's reasoning |
| **Active segment** | The source segment you're currently editing in Trados, plus your current target draft (if any) |
| **Surrounding segments** | A few segments before and after the active one — gives the AI enough context to know what kind of document this is (legal, technical, marketing, patent…) |
| **TM matches** | Fuzzy and exact matches Trados has found for the active segment, with their match percentages and TM names |
| **Termbase hits** | Term entries from your enabled termbases that match the active segment, including definitions, domains, and notes |

This is the same context the in-Trados Supervertaler Assistant chat already uses, so answer quality is comparable.

## Example questions

The Trados-aware chat shines when you ask questions that depend on what you're actually translating right now:

> *Which termbase entries apply to this segment?*

The chat will list the termbase hits and explain how to apply them.

> *What's a more natural English translation for the segment I'm currently editing?*

The chat will read the source, your draft target (if any), and any surrounding segments, and propose a polished translation.

> *Are there any TM matches I should reuse for this segment?*

The chat will summarise the TM matches and tell you which ones are close enough to confirm as-is.

> *What domain is this document about?*

The chat will infer the domain from the surrounding segments and reply.

## Privacy

* The bridge listens **only on `127.0.0.1`** (loopback). Other devices on your network can never reach it.
* Each Trados session gets a **fresh authentication token** — stale tokens from old sessions are useless.
* The bridge **only starts when you have Assistant access** (paid subscription or trial). Without Assistant access, no bridge is started.
* You can disable the bridge entirely on the Trados side by editing the plugin's `settings.json` and setting `"sidekickBridgeEnabled": false`. See [Sidekick Bridge](../../trados/ai-assistant/sidekick-bridge.md) for details.

## Troubleshooting

**The 🔗 Trados chip never appears.**
The bridge isn't being detected. Check:

1. Trados Studio is running with **Supervertaler for Trados v4.19.52 or later**
2. You have a paid subscription or active trial (the bridge is gated behind Assistant access)
3. You've **opened the Supervertaler Assistant panel** at least once in this Trados session — the bridge starts lazily when the panel is first activated
4. The handshake file `~/Supervertaler/trados/runtime/bridge.json` exists. If it doesn't, check the bridge log at `%TEMP%\Supervertaler-bridge.log` for diagnostic information.

**The chip appears but the answers don't seem to use the project context.**

Send a question that explicitly references your segment, like *"Which termbase entries apply to this segment?"*. If the answer cites specific terms from your termbases, the bridge is working. If the answer is generic, the chip may have been toggled off, or the bridge may have lost reachability — check the chip's tooltip.

**The chip is greyed out.**

The bridge was reachable earlier but is no longer responding. Make sure Trados Studio is still running and the Supervertaler Assistant panel hasn't been closed. The chip recovers automatically (within ~3 seconds) when Trados is reachable again.

---

## Related pages

* [Sidekick Overview](overview.md)
* [Sidekick Bridge (Trados side)](../../trados/ai-assistant/sidekick-bridge.md)
* [Supervertaler Assistant (Trados)](../../trados/ai-assistant.md)
