{% hint style="info" %}
You are viewing help for **Supervertaler for Trados** – the Trados Studio plugin. Looking for help with the standalone app? Visit [Supervertaler Workbench help](https://supervertaler.gitbook.io/help/workbench/).
{% endhint %}

The **Batch Operations** tab in the Supervertaler Assistant panel provides two AI-powered modes for processing multiple segments at once:

| Mode | Description |
|------|-------------|
| **[Batch Translate](batch-translate.md)** | Translate segments using AI with customisable prompts |
| **[AI Proofreader](ai-proofreader.md)** | Check translations for errors, inconsistencies, and style issues |

Switch between modes using the **Mode** dropdown at the top of the Batch Operations tab.

Both modes share the same prompt selector, provider/model configuration, and scope options. Prompts are filtered by mode – Translate prompts appear in Translate mode, Proofread prompts appear in Proofread mode. You can click the **provider/model label** to quickly switch AI models via a flyout menu – the same menu available in the Chat tab.

### Clipboard Mode

Both Translate and Proofread modes support **[Clipboard Mode](clipboard-mode.md)** – an alternative workflow that lets you use any web-based AI (ChatGPT, Claude, Gemini, etc.) without an API key. Tick the **Clipboard Mode** checkbox to switch from API-based processing to a manual copy/paste workflow. See [Clipboard Mode](clipboard-mode.md) for full details.

### Preview prompt

Next to the action button, the **👁 Preview prompt** link opens a read-only dialog showing **exactly what would be sent to the AI** for the current configuration: the assembled system prompt (including the active custom prompt, termbase entries, language-specific checks, and the full bilingual document context for proofread), followed by the numbered segment list. No LLM call is made.

This is useful for:

* **Sanity-checking before an expensive call** – see what the model will actually receive (including how many tokens of context, whether your termbase is being included, whether the right segments are in scope) before clicking Translate / Proofread.
* **Debugging unexpected output** – if the AI produces an odd suggestion, the preview shows you the exact prompt the model was answering, so you can see whether the issue is in your custom prompt, the termbase, the document context, or the segment list.
* **Manually pasting into a web LLM** – the dialog has its own *Copy to clipboard* button, so you can use it as a one-shot "send this to ChatGPT/Claude/Gemini" path without toggling Clipboard Mode.

The preview works in both **API mode** and **Clipboard Mode** without switching, and is available for both Translate and Proofread.

### AutoPrompt

The Batch Operations tab also includes an **[AutoPrompt](generate-prompt.md)** link that uses AI to create a comprehensive, domain-specific translation prompt based on your project's content, terminology, and TM data.

## See Also

* [Clipboard Mode](clipboard-mode.md)
* [AutoPrompt](generate-prompt.md)
* [Prompts](settings/prompts.md)
* [AI Settings](settings/ai-settings.md)
* [Keyboard Shortcuts](keyboard-shortcuts.md)
