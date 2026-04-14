# RWS App Store Manager — v4.19.5.0

**Version number:** `4.19.5.0`
**Minimum studio version:** `18.0`
**Maximum studio version:** `18.9`

---

## Changelog (since v4.18.49.0)

This is a significant release. The headline feature is **SuperMemory multi-bank support** – you can now keep several self-contained knowledge bases side by side (one per client, per domain, per language pair) and switch between them in a single click. The release also rolls up all improvements from v4.18.50 through v4.19.5.

### Added – SuperMemory multi-bank support

- **Memory Bank dropdown** in the Supervertaler Assistant toolbar. Lists every bank under your user-data folder with the active one pre-selected. Switching is immediate: the next chat turn, batch translation and Process Inbox run all read from the new bank, and your chat history is preserved across the switch. The active bank is persisted in settings and survives Trados restarts.
- **Create new banks from the toolbar** – pick `+ New memory bank…` at the bottom of the dropdown, enter a short name with a live preview of the final folder name, and the bank is created on disk with the full seven-folder skeleton and activated in one click.
- **Bundled template files** – every new bank ships with the canonical templates in `06_TEMPLATES/`, so Process Inbox and Health Check work against a fresh bank out of the box.
- **Heal-on-activation prompt** – if you activate an older bank missing its canonical template files, the plugin offers to restore them from the built-in defaults. Existing template files are never overwritten.
- **Shared with the Python Supervertaler** – memory banks live in the shared Supervertaler user-data folder with byte-identical layout, so a bank created in Trados works unchanged in the standalone app and vice versa.
- **Legacy single-bank migration** – the first time you open this release against an older single-bank installation, the plugin offers to move your existing folder into the new multi-bank layout automatically.

### Added – Batch Translate

- **Crash-recovery TMX backup.** Batch Translate now writes every translated segment to a TMX file as it arrives from the AI. If Trados crashes mid-run, the backup contains everything received so far – import it into any TM to recover. Files are saved to `Supervertaler\trados\batch_backups\` and are standard TMX 1.4.
- **Clipboard Mode now respects the Limit spinner.** Previously "Copy to Clipboard" ignored the Limit value and always copied every matching segment; now it applies the same limit as the API batch path.

### Added – Models

- **Gemma 4 models** – Google's Gemma 4 31B and Gemma 4 26B MoE added to both the Gemini provider and OpenRouter routes. 256K context window.

### Changed

- **Term Picker shortcut – Ctrl tap (memoQ-style).** Pressing and releasing Ctrl alone now opens the Term Picker dialogue, matching memoQ's behaviour. Maximum hold duration is 400 ms to prevent accidental triggers. The previous shortcut Ctrl+Alt+G is kept as a fallback.
- **Shorter panel names** – docking tabs and ribbon buttons show "TermLens" and "SuperSearch" instead of "Supervertaler TermLens" and "Supervertaler SuperSearch".
- **SuperMemory is the brand name; memory banks are the containers.** The two-level terminology is now reflected consistently across the UI, docs, and help system.
- **SuperMemory is off by default for new installations.** Most translators should start with the simpler workflow (TermLens glossaries + AI context awareness) and opt into SuperMemory once they have a populated bank. Existing users are unaffected.
- **Quick Add dialog redesigned (Ctrl+Alt+M).** Field labels are now language-aware ("Source term (Dutch):" and "Target term (English):"). New "Save as raw note" checkbox lets you dump ambiguous knowledge into `00_INBOX/` for the AI to compile via Process Inbox.

### Improved – SuperMemory workflow

- **Distill now archives source files** dropped in the inbox after a successful distill.
- **Process Inbox now recognises non-Markdown files** in the inbox and shows a helpful message pointing you at Distill for binary files.
- **Health Check shows progress instantly** – the scan now runs on a background thread with an immediate progress bubble.
- **Next-steps messages** in Distill and Process Inbox summaries guide you through the recommended workflow.

### Improved – SuperSearch

- **Resizable preview pane** with a draggable splitter bar.
- **Visible source/target divider** for better visual separation.
- **Highlight rendering fix** – no more "documentsare" word-collision artefacts.
- **Preview pane click reliability** – preview now also updates on cell click, not just selection change.
- **Header label clipping fix** – descenders (g, y, p) are no longer cut off.
- **Match truncation fix** – matches no longer show "Da..." instead of "Dawn".

### Fixed – AI Assistant chat

- **Memory bank dropdown empty after licence reactivation.** Deactivating and reactivating a licence mid-session left the memory bank dropdown empty and all AI Assistant event handlers unwired. The full initialisation now runs automatically when the licence state changes to active, without requiring a Trados restart.
- **Long AI responses are now truncated in the chat.** Messages longer than 1,500 characters are truncated in the chat bubble with a note; full text is always available via right-click → Copy.
- **Chat scroll rewrite.** Fixed the long-standing "messages disappear into ghost white space" bug on long chat histories. Users no longer need to click Clear before every chat message.
- **"Thinking…" bubble no longer bounces the chat.** The animation timer no longer yanks the user back to the bottom every 2 seconds.
- **User scroll is respected during long operations.** Scroll up to read history while Health Check or Distill is running; the chat stays where you put it.
- **User-initiated actions re-engage auto-scroll.** Clicking Send, Health Check, Process Inbox, Distill, or switching memory banks now resets the "user scrolled up" flag so results land in view.
- **Health Check always shows a completion summary** instead of leaving the user guessing whether the operation finished.
- **Process Inbox button correctly disabled when the inbox is empty.**
- **Duplicate "Thinking…" bubbles** no longer appear.
- **Reports tab now shows entries newest-first.**
- **SuperMemory toolbar** – Health Check button no longer stays greyed out after operations complete.

### Fixed – Terminology

- **Termbase AI filtering – word-boundary matching.** A term like "claim" no longer incorrectly matches "Disclaimer" or "Proclaim".
- **Termbase AI filtering – initialisation flag.** Per-project termbase exclusions are now preserved correctly across settings saves.

### Fixed – Proofreading

- **Proofreading false positives for inline tags.** Source and target now use the same plain-text extraction so tag markup never reaches the AI proofreader.

---

For the full changelog, see: https://github.com/Supervertaler/Supervertaler-for-Trados/releases
