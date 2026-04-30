# RWS App Store Manager – v4.19.22.0

**Version number:** `4.19.22.0`
**Minimum studio version:** `18.0`
**Maximum studio version:** `18.9`

This release covers everything since v4.19.13.0. Headline items: architectural overhaul of termbase direction handling that fixes a long-standing class of bug where reverse-direction termbase entries silently failed to match; new bulk "Reverse source/target" right-click action in the Termbase Editor; Claude Opus 4.7 support; and live-refresh of the "Set as active prompt for this project" action in the Batch Translate dropdown.

---

## Changelog

### Fixed (termbase direction handling – architectural overhaul)

- **Term matching now uses the termbase's declared direction, not per-entry language metadata.** Legacy write-path bugs (pre-v4.19.13) left many termbases with a mix of entries whose per-entry `source_lang` / `target_lang` tags didn't agree with the termbase's declaration – and TermLens trusted those per-entry tags when deciding whether to invert for reverse-direction projects. The result was entries silently not matching even though their text was correct. Direction decisions now pull source/target language from the canonical `termbases` table. Resilient to corrupted per-entry tags, and root-cause fix for the "my termbase entry exists but TermLens says no match" class of bug.
- **The term entry editor now always shows fields in termbase-declared direction.** Previously the edit dialog inverted field layout when the project direction differed from the termbase's, and did so inconsistently depending on which entry point opened it (the Termbase Editor grid right-click didn't invert, but the TermLens chip right-click did). Fields, labels, and save flow are now always in termbase order, matching the Termbase Editor grid.

### Fixed (Prompt Manager ↔ Batch Translate sync)

- **"Set as active prompt for this project" now takes effect immediately in the Batch Translate dropdown and works for any prompt.** Right-clicking a prompt in the Prompt Manager used to have no visible effect on the Batch dropdown until the Settings dialog was closed – and even then, prompts whose `Category` was not `Translate` (or `Proofread`, in proofread mode) were silently filtered out, so the checkmark had nowhere to land. The dropdown now live-refreshes the moment the active prompt is toggled, without closing the Settings dialog, and the active prompt always appears regardless of its folder or category. Works from all entry points (AI Assistant gear, TermLens gear, About dialog). Cancelling the Settings dialog still reverts the change; clicking OK persists it.
- **New prompts created at the Prompt Manager tree root are now visible in the Batch Translate dropdown.** Previously, creating a new prompt without first selecting a folder left its `Category` empty and the Batch dropdown's category filter silently excluded it. New prompts now default to the `Translate` category when no folder is selected.
- **Corrected stale pricing for Claude Opus 4.6 and Haiku 4.5.** The internal pricing table had Opus 4.6 at the pre-4.6 rate of $15 / $75 per MTok – Anthropic dropped Opus pricing to $5 / $25 with the 4.6 release. Haiku 4.5 was listed at $0.80 / $4.00, corrected to the current $1.00 / $5.00. Cost estimates shown in the AI Assistant and Batch Translate were over-stating Opus usage and under-stating Haiku usage – now accurate.

### Added

- **"Reverse source/target" right-click action in the Termbase Editor.** Fixes one or many reversed-direction entries at once. Menu label dynamically shows the count when multiple rows are selected. Swaps `source_term` ↔ `target_term`, language tags, abbreviations, and flips every linked synonym's language tag, all in one transaction. Right-click on the grid now also preserves multi-row selection when the clicked row was already part of the selection (matches Windows list conventions).
- **Claude Opus 4.7 support.** Anthropic's new flagship model (released 2026-04-16) is now selectable in AI Settings under the Claude provider and via the OpenRouter gateway (`anthropic/claude-opus-4.7`). Opus 4.7 has a 1M-token context window, 128k max output, and is Anthropic's most capable generally available model. Pricing is $5 / input MTok, $25 / output MTok – the same as Opus 4.6. Sonnet 4.6 remains the recommended default for most translation work; reach for Opus 4.7 when you need top-tier reasoning or long-context jobs.

### Note on Opus 4.7 tokenizer

- Claude Opus 4.7 uses a new tokenizer that can use **~1.0×–1.35× more tokens** for the same text compared to earlier models. Pre-send cost estimates (the `chars / 4` heuristic) will under-estimate Opus 4.7 costs by a similar margin. Actual billing is based on Anthropic's token counts.

For the full changelog, see: https://github.com/Supervertaler/Supervertaler-for-Trados/blob/main/CHANGELOG.md
