First GitHub release since v4.19.23 — about forty internal versions' worth of changes have shipped via the RWS AppStore in between. This release is **notes-only**: distribution moved to AppStore-only in v4.19.24 (no more `.sdlplugin` attached to GitHub releases). For the full per-version detail, see [CHANGELOG.md](https://github.com/Supervertaler/Supervertaler-for-Trados/blob/main/CHANGELOG.md).

## What's in v4.19.63

### Fixed — Provider name in error messages

Error messages now name the actual provider you selected, not "OpenAI". OpenRouter / DeepSeek / Mistral / Grok / Custom-OpenAI users were seeing things like *"OpenAI indicated tool_calls but no calls found in response"* because:

1. The provider-label helper at `LlmClient.cs:393` didn't have an OpenRouter case (fell through to the "OpenAI" default).
2. One tool-use error string was hardcoded to "OpenAI" rather than going through the helper.

Now `OpenAiProviderLabel()` includes OpenRouter, and the tool-use error formats with the dynamic label. Reported by a user who saw "OpenAI" in the error while running DeepSeek-V4-Pro through OpenRouter.

### Added — Defensive fallback when tool_calls is empty

When `finish_reason == "tool_calls"` but the response contains no tool-call objects, the AI Assistant now falls through to the text content instead of throwing. Some providers (notably DeepSeek through OpenRouter, and some Custom-OpenAI gateways) occasionally set the finish reason but omit the `tool_calls` array – the model intended to answer in plain text after all. With this fix, those turns succeed with the model's text reply rather than blowing up the whole conversation. If neither tool calls nor text is present, the error message remains but now uses the dynamic provider label.

## Install

Distribution is **AppStore-only** since v4.19.24:

- **Recommended:** install via the [RWS AppStore listing](https://appstore.rws.com/Plugin/) for a fully code-signed build with no "unsigned plugin" warning during install.

This GitHub release is for changelog visibility and source code only.

## Highlights since v4.19.23

A few of the bigger items shipped in the intervening forty internal versions, in case they're news:

- **Termbase Editor: Created column** (v4.19.61). New sortable "Created" column surfaces the `termbase_terms.created_date` value the schema has stored for every term. Useful for finding and removing recently-added entries.
- **TSV import/export preserves multi-line fields** (v4.19.62). Newlines, tabs, and backslashes in termbase fields (notes, projects, etc.) now escape properly for round-trip.
- **Termbase direction safety** (v4.19.56–60). Inversion logic no longer mis-handles unrelated language pairs; a confirm dialog catches accidentally-flagging an unrelated termbase as Write or Project.
- **Sidekick bridge for Workbench** (v4.19.52). The Trados plugin now exposes a localhost bridge so the Workbench Sidekick chat can read active project context (segment, surrounding segments, TM matches, termbase hits, project metadata).
- **TermLens popup keyboard-only flow** (v4.19.55). Ctrl+Shift+L opens a borderless TermLens popup at the cursor; arrow keys cycle, Enter inserts, Escape closes — mouse-free term insertion.
- **Termbase migration to v3 schema** with idempotent migrations and richer per-term metadata.
- **Lots of AI-Assistant polish** — file attachments, image input, prompt-library cleanup, several edge-case fixes for long-running conversations.

For everything else, [`CHANGELOG.md`](https://github.com/Supervertaler/Supervertaler-for-Trados/blob/main/CHANGELOG.md) has the per-version detail.
