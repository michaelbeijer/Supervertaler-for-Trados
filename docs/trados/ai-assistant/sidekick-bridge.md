{% hint style="info" %}
You are viewing help for **Supervertaler for Trados** – the Trados Studio plugin. Looking for help with the standalone app? Visit [Supervertaler Workbench help](https://supervertaler.gitbook.io/help/workbench/).
{% endhint %}

The **Sidekick Bridge** is a pair of small localhost-only HTTP services – one in each product – that let Supervertaler for Trados and Supervertaler Workbench cooperate while you translate. Each bridge runs in the background, exposes a few endpoints on `127.0.0.1` only, and is gated behind a per-session bearer token.

The two directions are independent and either side can be used without the other:

* **Trados → Workbench (read context).** The Trados plugin exposes the active project's segment, TM matches, termbase hits, and surrounding context so Workbench's floating Sidekick Chat can answer questions about your real Trados work.
* **Workbench → Trados (route QuickLauncher).** Workbench exposes a single endpoint that lets the Trados plugin push a [QuickLauncher](../quicklauncher.md) prompt into Sidekick Chat – the response then appears in Sidekick instead of in the Trados Assistant.

The rest of this page covers the Trados-side bridge first (the read direction, which has been around longer), then the Workbench-side bridge (the QuickLauncher-routing direction).

## Trados-side bridge: read project context

### What it does

When Supervertaler Workbench's Sidekick Chat is asked a question (with the **🔗 Trados** chip on), it asks the bridge for a snapshot of the current project state. The snapshot contains:

- The active source segment and your current target draft
- A few surrounding segments (default 5 before and 5 after)
- TM matches Trados has found for the active segment
- Termbase hits from your enabled termbases
- Project name, file name, source and target language

This is the same context the in-Trados Supervertaler Assistant chat already uses for its own answers — the bridge just exposes it to the Workbench client.

The bridge also accepts an "insert translation" command from the Workbench client, so a future build of Workbench Sidekick will be able to drop AI-suggested translations directly into your active Trados target cell.

### When it runs

The bridge is started automatically by the Supervertaler Assistant panel when **all** of these are true:

1. You have **Assistant access** — a paid subscription or an active trial. Users without Assistant access never start the bridge.
2. The hidden setting `AiSettings.SidekickBridgeEnabled` in `settings.json` is `true` (the default).
3. You have **opened the Supervertaler Assistant panel** at least once in this Trados session. The panel is lazy – Trados doesn't initialise it until you activate it.

The bridge is stopped automatically when Trados Studio exits. If Trados crashes or is force-killed, the next start of Trados detects the stale handshake file and replaces it with a fresh one.

### Privacy and security

The bridge is designed to be safe for everyday use:

- **Loopback-only.** The HTTP listener binds exclusively to `127.0.0.1`. Other devices on your network — even on the same Wi-Fi — can never reach it. There is a defence-in-depth check that rejects any non-loopback `RemoteEndPoint` even if the binding ever drifts.
- **Per-session authentication token.** A fresh GUID is generated every time the bridge starts. Clients must present it as a `Bearer` token. Stale tokens from previous sessions are useless.
- **Random high port.** The bridge picks a random port in the 49152–65535 range to avoid collisions with other local services.
- **No external network access.** The bridge only listens; it never reaches out to any external service.

### How to disable

The bridge has no UI checkbox – it's a hidden setting. To disable it entirely:

1. Close Trados Studio.
2. Open `~/Supervertaler/trados/settings/settings.json` in a text editor.
3. Find or add the field `"sidekickBridgeEnabled": false` inside the `aiSettings` object.
4. Save and restart Trados.

With this setting off, no bridge listener is started, no handshake file is written, and Workbench Sidekick will not detect a Trados plugin to talk to.

### Troubleshooting

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

### Endpoint reference (advanced)

For developers who want to integrate other tools with the bridge, here's the wire protocol. **The URL prefix is versioned** so future schema changes can ship without breaking older clients.

#### `GET /v1/active-context`

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

#### `POST /v1/insert-translation`

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

## Workbench-side bridge: route QuickLauncher to Sidekick

The Workbench-side bridge is the inverse of the one above: instead of exposing Trados context for Workbench to read, it lets the Trados plugin **push a QuickLauncher prompt into Workbench's Sidekick Chat**. The response then renders in Sidekick instead of in the in-Trados Assistant.

This is the bridge that makes the **QuickLauncher prompts go to: Workbench Sidekick** option in [AI Settings](../settings/ai-settings.md#quicklauncher-prompts-go-to) actually do something.

### When it runs

The Workbench-side bridge is started automatically by Workbench when its **Sidekick** is initialised (i.e. as soon as Workbench is running with Sidekick available). It writes its own handshake file at `~/Supervertaler/workbench/runtime/sidekick-bridge.json` and is stopped when Workbench exits.

The Trados plugin discovers this handshake the same way Workbench discovers the Trados handshake: it reads the file, validates the PID is alive, and posts a Bearer-authenticated HTTP request.

### What happens on a QuickLauncher click

When you press Ctrl+Q in Trados, pick a prompt, and have **Workbench Sidekick** selected as the target:

1. The Trados plugin expands the prompt's variables (selection, surrounding segments, TM matches, project text, etc.) exactly as it would for the in-Trados Assistant.
2. It calls Windows' `AllowSetForegroundWindow` with the Workbench PID so Sidekick is allowed to come to the front, then POSTs the expanded prompt to the Workbench bridge.
3. Workbench's Sidekick window pops forward, maximises to the screen it's on, switches to the Chat tab, echoes the display version of the prompt as a "user" message, and sends the full expansion through Sidekick's normal LLM pipeline.
4. The response renders in Sidekick. The window stays on top throughout (Sidekick uses a topmost-flip + AttachThreadInput trick to defeat the Windows foreground lock when the response arrives).

### Fallback when Workbench isn't running

If Workbench isn't running, the handshake file is missing, the PID is stale, or any HTTP error fires, Trados silently falls back to the in-Trados Assistant. Your prompt is never lost – it just lands where it always used to.

### Endpoint reference (advanced)

#### `POST /v1/run-prompt`

Runs a QuickLauncher prompt in Sidekick Chat. Authentication via `Authorization: Bearer <token>` from the Workbench-side handshake file.

Request body:

```json
{
  "prompt": "<fully expanded prompt sent to the LLM>",
  "displayPrompt": "<redacted version shown in chat as the user message>",
  "promptName": "Explain selection (in general)"
}
```

`displayPrompt` is what the user sees as their own message in Sidekick. It's typically a shortened form of `prompt` (e.g. `[source document — 173 segments]` instead of the full project text) so the chat doesn't get spammed with the kilobytes of context the LLM needs.

Response on success:

```json
{ "ok": true }
```

Response on failure:

```json
{ "ok": false, "error": "<reason>" }
```

#### `GET /v1/ping`

Cheap health check. No authentication required because the response carries no privileged information.

```json
{ "ok": true }
```

### Troubleshooting

The Workbench-side bridge writes its own diagnostic log at `~/Supervertaler/workbench/runtime/sidekick-bridge.log` – truncated on every Workbench start. Each accepted prompt logs a line like:

```
[2026-05-02 22:29:44.634588] POST /v1/run-prompt accepted (name='Explain selection (in general)', prompt=86 chars, displayPrompt=86 chars)
```

If you've set the QuickLauncher target to Workbench Sidekick but nothing's appearing in Sidekick:

1. Check that Workbench is actually running.
2. Check that `~/Supervertaler/workbench/runtime/sidekick-bridge.json` exists and contains a `port`, `token`, and a non-zero `pid`.
3. Trigger a QuickLauncher prompt, then check the bridge log for an `accepted` line. If the line is there, the prompt was delivered – the issue is on the Sidekick render side. If it isn't, the Trados-side request never arrived (most likely cause: Workbench was started after Trados, so Trados is using a stale handshake; restart Trados).

---

## Related pages

* [QuickLauncher](../quicklauncher.md) – the action that drives the Workbench-side bridge
* [Supervertaler Assistant](../ai-assistant.md) – the in-Trados chat that uses the same context fields the Trados-side bridge exposes
* [Sidekick Chat – Trados-aware mode (Workbench)](https://supervertaler.gitbook.io/help/workbench/sidekick/trados-aware-chat) – the primary consumer of the Trados-side bridge
* [User Data Folder](../data-folder.md) – where both handshake files live
