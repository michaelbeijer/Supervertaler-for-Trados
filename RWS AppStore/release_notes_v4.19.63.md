# RWS App Store Manager – v4.19.63.0

**Version number:** `4.19.63.0`
**Minimum studio version:** `18.0`
**Maximum studio version:** `18.9`
**Checksum:** `fa9ceea6ef0843653903800e4a04abad5b0deed7686eb6bed130b77957c06b35`

---

## Changelog

### Added
- **When `finish_reason == "tool_calls"` but the response contains no tool-call objects, fall through to the text content** instead of throwing. Some providers (notably DeepSeek through OpenRouter, and some Custom-OpenAI gateways) occasionally set the finish reason but omit the `tool_calls` array – the model intended to answer in plain text after all. With this fix, those turns succeed with the model's text reply rather than blowing up the whole conversation. If neither tool calls nor text is present, the error message remains but now uses the dynamic provider label.

### Fixed
- **Error messages now name the actual provider you selected**, not "OpenAI". OpenRouter / DeepSeek / Mistral / Grok / Custom-OpenAI users were seeing things like "OpenAI indicated tool_calls but no calls found in response" because the provider-label helper at [LlmClient.cs:393](src/Supervertaler.Trados/Core/LlmClient.cs#L393) didn't have an OpenRouter case (fell through to the "OpenAI" default), and one tool-use error string was hardcoded to "OpenAI" rather than going through the helper. Now `OpenAiProviderLabel()` includes OpenRouter, and the tool-use error formats with the dynamic label.
- Reported by a user who saw "OpenAI" in the error while running DeepSeek-V4-Pro through OpenRouter.

For the full changelog, see: https://github.com/Supervertaler/Supervertaler-for-Trados/releases