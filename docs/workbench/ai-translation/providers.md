Supervertaler supports multiple AI providers so you can choose what fits your workflow and budget. You only need one to get started.

## Cloud providers

### OpenAI

Models: **GPT-5.4**, **GPT-5.4 Mini**

Get an API key at [platform.openai.com/api-keys](https://platform.openai.com/api-keys). GPT-5.4 Mini is a good starting point – fast, affordable, and high quality for most translation tasks.

### Anthropic (Claude)

Models: **Claude Sonnet 4.6**, **Claude Haiku 4.5**, **Claude Opus 4.7**, **Claude Opus 4.6**

Get an API key at [console.anthropic.com](https://console.anthropic.com). Claude Sonnet 4.6 is the recommended default. Haiku 4.5 is the fastest and cheapest option; Opus 4.7 is the most capable.

### Google (Gemini)

Models: **Gemini 2.5 Flash**, **Gemini 2.5 Pro**, **Gemini 3.1 Pro (Preview)**, **Gemma 4 31B**, **Gemma 4 26B MoE**

Get an API key at [aistudio.google.com/apikey](https://aistudio.google.com/apikey). Gemini 2.5 Flash offers a generous free tier and is a strong choice for high-volume work.

### Grok (xAI)

Models: **Grok 4.20**, **Grok 4.1 Fast**, **Grok 4.20 (Reasoning)**

Get an API key at [console.x.ai](https://console.x.ai). Grok 4.20 supports a 2M-token context window.

### Mistral AI

Models: **Mistral Large**, **Mistral Small**, **Mistral Nemo**

Get an API key at [console.mistral.ai](https://console.mistral.ai). Particularly strong on European languages.

### DeepSeek

Models: **DeepSeek V4 Pro**, **DeepSeek V4 Flash**

Get an API key at [platform.deepseek.com](https://platform.deepseek.com). DeepSeek offers competitive pricing and strong multilingual quality. V4 Flash is the fast, cost-effective option for high-volume work.

## Gateway providers

### OpenRouter

[OpenRouter](https://openrouter.ai) is an API gateway that gives you access to 200+ models from OpenAI, Anthropic, Google, DeepSeek, Mistral, Meta, and many others – all through a **single API key**.

The model dropdown includes a curated selection for translation, and you can also type any OpenRouter model ID directly. Browse all models at [openrouter.ai/models](https://openrouter.ai/models).

Get an API key at [openrouter.ai/keys](https://openrouter.ai/keys).

{% hint style="info" %}
OpenRouter adds a 5.5% platform fee on top of the underlying provider's price. For most translation jobs this adds only a few cents.
{% endhint %}

## Local providers

### Ollama

Run models entirely on your own machine – no API key and no internet required.

See [Ollama Setup](ollama.md) for download and configuration instructions.

## Custom (OpenAI-compatible)

For any provider that exposes an OpenAI-compatible API (Azure OpenAI, together.ai, local inference servers, etc.), select **Custom (OpenAI-compatible)** and enter the endpoint URL, model name, and API key.

---

## Related pages

- [Setting Up API Keys](../get-started/api-keys.md)
- [Ollama Setup](ollama.md)
- [AI Translation Overview](overview.md)
