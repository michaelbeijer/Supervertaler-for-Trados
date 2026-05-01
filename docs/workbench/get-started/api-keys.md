To use AI translation you need an API key from at least one provider. Enter your key in **Settings → AI Settings**.

## Supported providers

| Provider | Where to get a key |
|----------|--------------------|
| **OpenAI** | [platform.openai.com/api-keys](https://platform.openai.com/api-keys) |
| **Anthropic (Claude)** | [console.anthropic.com](https://console.anthropic.com) |
| **Google (Gemini)** | [aistudio.google.com/apikey](https://aistudio.google.com/apikey) |
| **Grok (xAI)** | [console.x.ai](https://console.x.ai) |
| **Mistral AI** | [console.mistral.ai](https://console.mistral.ai) |
| **DeepSeek** | [platform.deepseek.com](https://platform.deepseek.com) |
| **OpenRouter** (200+ models, one key) | [openrouter.ai/keys](https://openrouter.ai/keys) |
| **Ollama** | No key needed – runs locally |

## Entering a key

1. Open **Settings → AI Settings**
2. Select your provider from the **Provider** dropdown
3. Paste your API key into the **API Key** field
4. Click **Test Connection** to verify
5. Save settings

Keys are stored locally and are only sent to the provider's own API endpoint.

## Switching providers

You can configure keys for multiple providers in the same settings panel. Switch between them without re-entering credentials – the key for each provider is remembered independently.

## Using Ollama (no key required)

Ollama runs models entirely on your machine. No API key or internet connection is needed.

See [Ollama Setup](../ai-translation/ollama.md) for download and configuration instructions.

## Using OpenRouter (one key for everything)

If you prefer not to manage multiple accounts, OpenRouter lets you access 200+ models from all major providers with a single API key. Create an account at [openrouter.ai](https://openrouter.ai) and paste your key into the **OpenRouter** provider slot.

## Troubleshooting

| Problem | Solution |
|---------|----------|
| "Invalid API key" | Double-check the key; ensure no leading or trailing spaces |
| "Rate limit exceeded" | Wait a moment, or upgrade your API plan |
| "Model not found" | Check the model name in settings; it may have been updated |
| No response | Check your internet connection and that the provider's service is up |

---

## Next steps

- [Create your first project](first-project.md)
- [Supported LLM Providers](../ai-translation/providers.md)
- [AI Translation Overview](../ai-translation/overview.md)
