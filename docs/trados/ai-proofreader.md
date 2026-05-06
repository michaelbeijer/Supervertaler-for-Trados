{% hint style="info" %}
You are viewing help for **Supervertaler for Trados** – the Trados Studio plugin. Looking for help with the standalone app? Visit [Supervertaler Workbench help](https://supervertaler.gitbook.io/help/workbench/).
{% endhint %}

The AI Proofreader checks your translated segments for errors using AI. It identifies issues such as mistranslations, omissions, grammar problems, and inconsistencies, and presents the results as clickable issue cards in the **Reports** tab.

## Starting a Proofreading Run

1. Open the **Supervertaler Assistant** panel (View > Supervertaler Assistant)
2. Switch to the **Batch Operations** tab
3. Select **Proofread** (instead of Translate)
4. Choose a **scope** from the dropdown
5. Optionally select a proofreading **prompt** from the prompt selector
6. Click **Proofread**

## Scope

The scope dropdown controls which segments are checked:

| Scope                              | Description                                                                     |
| ---------------------------------- | ------------------------------------------------------------------------------- |
| **Translated only**                | Checks only segments with Translated status                                     |
| **Translated + approved/signed-off** | Checks segments with Translated, Approved, or Signed-off status              |
| **All segments**                   | Checks every segment that has target text                                       |
| **Filtered segments**              | Checks only segments visible after applying a Trados display filter             |
| **Filtered (translated only)**     | Checks translated segments within the current filter                            |

## Prompt Selection

When in Proofread mode, the prompt dropdown shows only prompts with the **Proofread** category. This keeps the list focused – translation prompts are hidden.

If no prompt is selected, the AI uses a default proofreading instruction that checks for accuracy, completeness, grammar, and consistency.

{% hint style="info" %}
You can create custom proofreading prompts in the [Prompt Manager](settings/prompts.md). Set the category to **Proofread** so they appear in the dropdown when proofreading.
{% endhint %}

### Default Proofreading Prompt

The shipped **Default Proofreading Prompt** is deliberately slim. The hardcoded base of every Batch Proofread already includes the persona, the five quality categories (accuracy / completeness / terminology / grammar / number formatting), the output format, the "no full corrected translations" rule, and language-specific checks for Dutch, German, and French – so the prompt itself only needs to add what's missing from the base.

What the default prompt *does* add:

* **Default to OK.** Raise an ISSUE only when a specific, demonstrable problem can be pointed to in the translation – never speculative or hypothetical concerns.
* **Citation discipline.** When flagging a terminology consistency issue, the AI must cite specific source segment numbers in the **Evidence:** field – e.g. *"`'trekriem'` rendered as `'pull belt'` in `[SEGMENT 0031]`, `'draw strap'` in `[SEGMENT 0084]`"*. Inconsistency claims without concrete citations are not allowed.
* **Source query distinction.** If the source itself contains an error (typo, duplication, missing word, internal inconsistency), the AI prefixes the Issue line with **"Source query:"** and notes whether the translation handled it correctly. A faithful rendering of a flawed source is not a translation error.
* **Explicit boundaries.** The AI does not re-engineer the source, propose alternative terminology without a citation, flag stylistic preferences as errors, or flag empty target lines (those mean the segment hasn't been translated yet).

{% hint style="info" %}
**If you want to customise:** clone the default in the [Prompt Manager](settings/prompts.md) and edit your copy. The default itself is read-only and gets refreshed when the plugin updates. Your clone keeps all your changes.
{% endhint %}

## Reports Tab

Proofreading results appear in the **Reports** tab of the Supervertaler Assistant panel. Each issue is shown as a clickable card containing:

* **Segment number** – the actual per-file segment number as shown in the Trados editor grid
* **Issue description** – what the AI found wrong
* **Evidence** *(when applicable)* – specific source segment numbers the AI cites to back up the claim, shown in italic grey between the issue and the suggestion. Required for terminology consistency claims (see *Default Proofreading Prompt* below) so you can verify the inconsistency yourself by jumping to the cited segments.
* **Suggestion** – the AI's recommended fix (if available)

Right-click any card to copy the issue, the evidence, the suggestion, or the whole card to the clipboard.

### Navigating to Issues

Click any issue card to navigate directly to that segment in the Trados editor. This works correctly in multi-file projects – the plugin uses the segment's internal identifiers to find the exact segment.

### Dismissing Issues

Each issue card has a checkbox. Tick it to dismiss the issue and remove it from the list. This lets you work through the results one by one, keeping track of which issues you have already addressed. When all issues have been dismissed, the Reports tab shows "All issues addressed – well done!"

### Clearing Results

Click the **Clear** button at the top of the Reports tab to remove all results and start fresh.

### Run Summary

After a proofreading run, the Reports tab shows:

* Total number of issues found and segments checked
* Run timestamp and duration in the footer

## Adding Issues as Trados Comments

Check the **"Also add issues as Trados comments"** checkbox in the Batch Operations tab (visible only in Proofread mode) before starting the run. When enabled, each issue found by the proofreader is also inserted as a Trados segment comment, so you can see the issues directly in the editor without switching to the Reports tab.

## AI Context in Proofreading

Batch Proofread builds a richer context than Batch Translate – it has to, because verifying whether a term is rendered consistently across the document is exactly the kind of question the AI needs the whole document to answer.

* **Full bilingual document context** – when **Include document context** is enabled in [AI Settings](settings/ai-settings.md), every segment in the document is included with both source AND target text, with no truncation. This is what makes target-side consistency verifiable: the AI can check "this term is rendered as X in [SEGMENT 0031] and Y in [SEGMENT 0084]" against the actual document, not against a guess. Segment numbers in the document context match the `[SEGMENT XXXX]` numbers the AI sees in the batch it's reviewing, so citations cross-reference both ways.
* **Termbase terms** – terminology from enabled termbases is checked against the translations, including term definitions and domains when that option is enabled. Forbidden terms are flagged with a `⚠️ DO NOT USE` marker so the AI knows to flag them as issues if it sees them in the translation.
* **Language-specific checks** – Dutch, German, and French targets get auto-included quality checks (compound spelling, dt-errors, de/het articles for Dutch; capitalisation and case system for German; accents and punctuation spacing for French). These come from the hardcoded base and don't need to be in your custom prompt.
* **Custom prompts** – the selected proofreading prompt provides domain-specific quality checks on top of all of the above.

TM matches and surrounding segments are **not** included in proofreading – these are Chat & QuickLauncher features only. See the [AI Settings](settings/ai-settings.md) page for a full comparison table.

{% hint style="warning" %}
**Token cost:** Sending the full bilingual document roughly doubles the context size compared to source-only. For typical patent / legal / technical jobs (under ~500 segments) this is a minor cost increase – usually a few extra cents per batch on Sonnet-class models. For very long documents the cost scales linearly; if you proofread a 5,000-segment book you may want to disable Include document context and rely on per-batch context only.
{% endhint %}

## Clipboard Mode

If you prefer to use a web-based AI (ChatGPT, Claude, Gemini, etc.) instead of an API, tick the **Clipboard Mode** checkbox. Supervertaler builds a complete proofreading prompt with both source and target text for each segment and copies it to your clipboard. See [Clipboard Mode](clipboard-mode.md) for full details.

## Tips

### Start with Confirmed Segments

Use the **Confirmed Only** scope to check segments you consider finished. This avoids noise from segments that are still being worked on.

### Use Domain-Specific Proofreading Prompts

Create custom proofreading prompts tailored to your domain. For example, a medical proofreading prompt can check for correct use of clinical terminology, while a legal proofreading prompt can verify that defined terms are used consistently.

### Review After AI Translation

The AI Proofreader pairs well with [Batch Translate](batch-translate.md). After translating a batch of segments with AI, run the proofreader to catch any issues before final review.

### Combine with Display Filters

Use Trados display filters to isolate specific segments (e.g., segments containing a certain term), then proofread only those filtered segments for targeted quality checks.

---

## See Also

* [Clipboard Mode](clipboard-mode.md)
* [Batch Translate](batch-translate.md)
* [Prompts](settings/prompts.md)
* [Supervertaler Assistant](ai-assistant.md)
* [Keyboard Shortcuts](keyboard-shortcuts.md)
