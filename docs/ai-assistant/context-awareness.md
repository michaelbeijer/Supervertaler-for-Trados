# Context Awareness

{% hint style="info" %}
You are viewing help for **Supervertaler for Trados** -- the Trados Studio plugin. Looking for help with the standalone app? Visit [Supervertaler Workbench help](https://help.supervertaler.com).
{% endhint %}

The Supervertaler Assistant is deeply integrated with your Trados project. Every time you send a message, the assistant automatically receives a rich snapshot of your current work so it can give you informed, project-specific answers. This context is assembled fresh on each message, so the AI always sees the latest state.

## Project and File Information

The assistant knows which project and file you are working in, the language pair (e.g. Dutch --> English), and your current position in the document (e.g. "Segment 42 of 318").

## Full Document Content

When enabled, all source segments in the current document are included in the AI prompt. This allows the assistant to analyse the document and determine its type -- legal, medical, technical, marketing, financial, scientific, etc. -- and use that assessment to inform its advice on terminology, style, and translation choices.

For very large documents, the content is automatically truncated to the configured maximum (default: 500 segments). The truncation preserves the first 80% and the last 20% so the AI still sees both the beginning and the end of the document.

## Current Segment

The source text you are translating and any target translation you have already entered.

## Surrounding Segments

Two segments before and two segments after your current position are included, with their translations where available. This gives the AI local context for cohesion and consistency.

## Translation Memory Matches

TM fuzzy matches for the current segment are included, showing the match percentage, source text, and target text. This gives the AI reference material from your previous translations.

## Terminology

Matched terms from your active termbases are included with their approved translations and synonyms. Optionally, term definitions, domains, and usage notes are also included, giving the AI deeper understanding of your terminology requirements.

Terms marked as non-translatable or forbidden are flagged so the AI can respect those constraints.

## Controlling the Context

{% hint style="info" %}
You can control exactly what context the assistant receives. In the settings dialogue on the **AI Settings** tab, you can toggle document content, TM matches, term metadata, and select which termbases contribute to the AI prompt.
{% endhint %}

{% hint style="success" %}
**Tip:** For the best results, keep document content and term metadata enabled. The more context the AI has, the more accurate and consistent its suggestions will be. The document type analysis is especially valuable -- it helps the AI understand that "consideration" means something different in a legal contract than in a marketing brochure.
{% endhint %}

## See Also

* [Supervertaler Assistant](../ai-assistant.md) -- Overview
* [AI Settings](../settings/ai-settings.md) -- Configure context options
* [Memory banks – AI Integration](memory-banks/ai-integration.md) -- Knowledge base context
