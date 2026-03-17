{% hint style="info" %}
You are viewing help for **Supervertaler for Trados** — the Trados Studio plugin. Looking for help with the standalone app? Visit [Supervertaler Workbench help](https://help.supervertaler.com).
{% endhint %}

# Prompts

Prompts control how the AI translates your content. The Prompt Manager lets you browse built-in domain prompts and create your own custom prompts.

## Accessing the Prompt Manager

Open the plugin **Settings** dialog and switch to the **Prompts** tab.

## Built-in prompts

The plugin ships with built-in prompts organised into two categories:

| Category | Prompts | Used in |
|----------|---------|---------|
| **Translate** | Medical, Legal, Patent, Financial, Technical, Marketing, IT, Dutch Style Guide, English Style Guide, French Style Guide, German Style Guide, Spanish Style Guide, General Translation, Creative / Transcreation | Batch Translate mode |
| **Proofread** | General Proofreading | Batch Proofread mode |

Each prompt is tuned for its purpose, including instructions for tone, terminology handling, and formatting conventions.

### Prompt category filtering

Prompts are filtered by mode in the **Batch Operations** tab:

* In **Translate** mode, only prompts with the **Translate** category appear in the dropdown
* In **Proofread** mode, only prompts with the **Proofread** category appear

This keeps the prompt list focused and relevant to the task at hand.

{% hint style="info" %}
Built-in prompts are **read-only**. If you want to customise one, create a new prompt and copy the content as a starting point.
{% endhint %}

## Creating custom prompts

1. Click **New**
2. Fill in the fields:

| Field | Description |
|-------|-------------|
| **Name** | A short label for this prompt (shown in the selection list) |
| **Description** | Optional summary of what this prompt is for |
| **Category** | Group the prompt under a category for easier browsing |
| **Content** | The full prompt text sent to the AI model |

3. Click **Save**

## Prompt variables

You can use the following variables in your prompt content. They are replaced automatically at translation time:

| Variable | Replaced with |
|----------|---------------|
| `{{SOURCE_LANGUAGE}}` | The source language of the current project |
| `{{TARGET_LANGUAGE}}` | The target language of the current project |

**Example prompt content:**

```
You are a professional medical translator. Translate the following text
from {{SOURCE_LANGUAGE}} to {{TARGET_LANGUAGE}}. Use formal, clinical
language. Preserve all formatting tags exactly as they appear.
```

## Editing prompts

1. Select a prompt in the list
2. Click **Edit**
3. Modify the fields as needed
4. Click **Save**

{% hint style="warning" %}
Built-in prompts cannot be edited. To modify a built-in prompt, create a new custom prompt based on it.
{% endhint %}

## Deleting custom prompts

1. Select a custom prompt in the list
2. Click **Delete**
3. Confirm the deletion

Built-in prompts cannot be deleted.

## Prompt file format

Prompts are stored as `.svprompt` files with YAML frontmatter. This is the same format used by Supervertaler desktop, so prompts are interchangeable between both applications.

```yaml
---
name: Medical Translation Specialist
description: Clinical and pharmaceutical content
category: Translate
---

You are a professional medical translator...
```

| Field | Description |
|-------|-------------|
| `name` | Display name shown in the prompt selector |
| `description` | Optional summary |
| `category` | **Translate** or **Proofread** — controls which mode the prompt appears in |

{% hint style="info" %}
Older prompts using `.md` extension or the `domain` YAML key are still loaded for backward compatibility. They will be saved as `.svprompt` with the `category` key when edited.
{% endhint %}

## Using prompts

The currently selected prompt is used when running **Batch Translate** or **Batch Proofread**. Before starting a batch, make sure you have the correct prompt selected for the domain and style you need.

---

## See Also

- [AI Settings](ai-settings.md)
- [AI Proofreader](../ai-proofreader.md)
- [Batch Translation (Workbench)](https://supervertaler.gitbook.io/supervertaler/ai-translation/batch-translation)
- [Creating Prompts (Workbench)](https://supervertaler.gitbook.io/supervertaler/ai-translation/prompts)
