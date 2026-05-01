{% hint style="info" %}
You are viewing help for **Supervertaler for Trados** – the Trados Studio plugin. Looking for help with the standalone app? Visit [Supervertaler Workbench help](https://supervertaler.gitbook.io/help/workbench/).
{% endhint %}

The **Sidekick Bridge** is a small localhost-only HTTP service that the Supervertaler for Trados plugin runs in the background. Its only job is to let the floating Sidekick Chat in **Supervertaler Workbench** read the active Trados project context — so you can ask Workbench's chat questions about the segment you're currently editing, without leaving Trados Studio.

## What it does

When Supervertaler Workbench's Sidekick Chat is asked a question (with the **🔗 Trados** chip on), it asks the bridge for a snapshot of the current project state. The snapshot contains:

- The active source segment and your current target draft
- A few surrounding segments (default 5 before and 5 after)
- TM matches Trados has found for the active segment
- Termbase hits from your enabled termbases
- Project name, file name, source and target language

This is the same context the in-Trados Supervertaler Assistant chat already uses for its own answers — the bridge just exposes it to the Workbench client.

The bridge also accepts an "insert translation" command from the Workbench client, so a future build of Workbench Sidekick will be able to drop AI-suggested translations directly into your active Trados target cell.

## When it runs

The bridge is started automatically by the Supervertaler Assistant panel when **all** of these are true:

1. You have **Assistant access** — a paid subscription or an active trial. Users without Assistant access never start the bridge.
2. The hidden setting `AiSettings.SidekickBridgeEnabled` in `settings.json` is `true` (the default).
3. You have **opened the Supervertaler Assistant panel** at least once in this Trados session. The panel is lazy – Trados doesn't initialise it until you activate it.

The bridge is stopped automatically when Trados Studio exits. If Trados crashes or is force-killed, the next start of Trados detects the stale handshake file and replaces it with a fresh one.

## Privacy and security

The bridge is designed to be safe for everyday use:

- **Loopback-only.** The HTTP listener binds exclusively to `127.0.0.1`. Other devices on your network — even on the same Wi-Fi — can never reach it. There is a defence-in-depth check that rejects any non-loopback `RemoteEndPoint` even if the binding ever drifts.
- **Per-session authentication token.** A fresh GUID is generated every time the bridge starts. Clients must present it as a `Bearer` token. Stale tokens from previous sessions are useless.
- **Random high port.** The bridge picks a random port in the 49152–65535 range to avoid collisions with other local services.
- **No external network access.** The bridge only listens; it never reaches out to any external service.

## How to disable

The bridge has no UI checkbox – it's a hidden setting. To disable it entirely:

1. Close Trados Studio.
2. Open `~/Supervertaler/trados/settings/settings.json` in a text editor.
3. Find or add the field `"sidekickBridgeEnabled": false` inside the `aiSettings` object.
4. Save and restart Trados.

With this setting off, no bridge listener is started, no handshake file is written, and Workbench Sidekick will not detect a Trados plugin to talk to.

## Troubleshooting

The bridge writes a diagnostic log to two locations on every start:

- `~/Supervertaler/trados/runtime/bridge.log` — under your Supervertaler user-data folder
- `%TEMP%\Supervertaler-bridge.log` — guaranteed-writable fallback

The log is truncated on every plugin start, so it always reflects the current Trados session. The first lines record the resolved data-folder path so you can see exactly where the plugin is looking.

Useful entries to look for:

| Log line | Meaning |
|----------|---------|
| `Initialize: HasAssistantAccess=false` | Your licence isn't picked up as paid or trial. The bridge is correctly skipped in this case. |
| `guard: AiSettings.SidekickBridgeEnabled=false` | You've explicitly disabled the bridge in settings.json. |
| `port NNNNN bind failed: HttpListenerException code=5` | Windows is refusing to let the plugin bind to a localhost port. Rare; usually means a strict group-policy environment. |
| `Start() complete. Bridge live on http://127.0.0.1:NNNNN/` | All good – the bridge is running and the handshake file should be at `~/Supervertaler/trados/runtime/bridge.json`. |

If `bridge.json` exists and contains `port`, `token`, `pid`, and `startedAt`, the bridge is healthy. Workbench Sidekick should detect it within ~3 seconds of opening the chat tab.

## Endpoint reference (advanced)

For developers who want to integrate other tools with the bridge, here's the wire protocol. **The URL prefix is versioned** so future schema changes can ship without breaking older clients.

### `GET /v1/active-context`

Returns a JSON snapshot of the current Trados project state. Authentication via `Authorization: Bearer <token>` from the handshake file.

```json
{
  "available": true,
  "project": {
    "name": "BRANTS-CARG-001",
    "fileName": "20260331 CARG-003-BE-EP Application as filed.docx",
    "sourceLang": "nl-BE",
    "targetLang": "en-US"
  },
  "activeSegment": { "source": "...", "target": "..." },
  "surroundingSegments": [
    { "source": "...", "target": "..." }
  ],
  "tmMatches": [
    { "score": 95, "source": "...", "target": "...", "tmName": "..." }
  ],
  "termbaseHits": [
    { "source": "...", "target": "...", "termbaseName": "...",
      "definition": "...", "domain": "...", "notes": "..." }
  ]
}
```

When no document is active, returns `{"available": false}` with HTTP 200.

### `POST /v1/insert-translation`

Inserts text into the active Trados target segment via the same code path as the in-Chat Apply-To-Target button.

Request body:

```json
{ "text": "The translation to insert" }
```

Response on success:

```json
{ "ok": true }
```

Response on failure (e.g. no active segment):

```json
{ "ok": false, "error": "no active document" }
```

---

## Related pages

* [Supervertaler Assistant](../ai-assistant.md) – the in-Trados chat that uses the same context fields
* [Sidekick Chat – Trados-aware mode (Workbench)](https://supervertaler.gitbook.io/help/workbench/sidekick/trados-aware-chat) – the primary consumer of this bridge
* [User Data Folder](../data-folder.md) – where the handshake file lives
