# Supervertaler for Trados – Claude Context

## What this project is
Supervertaler for Trados is a Trados Studio 2024 (v18) plugin that brings key Supervertaler features into the Trados ecosystem. It uses a **tabbed ViewPart** with separate tabs for each feature:

- **TermLens** — live inline terminology display (glossary panel) — fully implemented
- **AI Assistant** — project-aware chat interface — tab exists as placeholder, implementation next
- **Batch Translate** — AI-powered segment translation — fully implemented (OpenAI/Anthropic/Google)
- **Prompt Library** — domain-specific and custom prompt management — fully implemented (14 built-in prompts)

### Tech stack
- **Language**: C# / .NET Framework 4.8, SDK-style .csproj
- **Namespace**: `Supervertaler.Trados` (sub-namespaces: `.Controls`, `.Core`, `.Models`, `.Settings`)
- **Build**: `bash build.sh` from repo root (dotnet build → package_plugin.py → deploy)
- **Deploy target**: `%LOCALAPPDATA%\Trados\Trados Studio\18\Plugins\Packages\Supervertaler.Trados.sdlplugin`
- **Strong-name key**: `src/Supervertaler.Trados/Supervertaler.Trados.snk` — PublicKeyToken: `6afde1272ae2306a`
  (Trados's `DefaultPluginTypeLoader` refuses unsigned assemblies — this is non-negotiable)

---

## UI architecture

The ViewPart ("Supervertaler for Trados") uses a three-layer structure:

```
TermLensEditorViewPart (AbstractViewPartController)
  └── MainPanelControl (UserControl, IUIControl) — tabbed container
        ├── Tab "TermLens" → TermLensControl (glossary panel with header, flow panel)
        ├── Tab "AI Assistant" → launcher panel (activates the dockable AI Assistant panel)
        └── Tab "Batch Translate" → BatchTranslateControl (scope/prompt/provider, translate button, log)

AiAssistantViewPart (AbstractViewPartController) — planned, separate dockable panel
  └── AiAssistantControl — chat UI (message history, input, context)
```

- `TermLensEditorViewPart` owns the lifecycle, settings, and event routing
- `MainPanelControl` is a thin wrapper holding the `TabControl`
- `TermLensControl` is the existing glossary panel (header with A+/A−/gear buttons, FlowLayoutPanel with TermBlock/WordLabel controls)
- Both `_control` (TermLensControl) and `_mainPanel` (MainPanelControl) are lazy singletons; all existing `_control.Value` references work unchanged

---

## SQLite library: Microsoft.Data.Sqlite (not System.Data.SQLite)

We use **`Microsoft.Data.Sqlite`** + SQLitePCLRaw (native DLL: `e_sqlite3.dll`).

**Do NOT switch to `System.Data.SQLite`** — it uses `SQLite.Interop.dll` with a version-fingerprint
hash scheme (`SI04b638e115f7beb4` etc.) that causes `EntryPointNotFoundException` inside Trados
Studio's plugin environment. The root cause: other apps (memoQ, Glossary Converter) ship their
own `SQLite.Interop.dll` with different hashes, and Windows's DLL loader picks the wrong one.
Microsoft.Data.Sqlite uses standard SQLite3 C entry points — no version-hash conflicts.

`AppInitializer.cs` pre-loads `e_sqlite3.dll` by full path and handles `AssemblyResolve` for all
managed DLLs we ship (Microsoft.Data.Sqlite, SQLitePCLRaw, System.Memory, etc.) because Trados
ships older versions of several .NET Standard polyfills.

---

## Key files

| File | Purpose |
|------|---------|
| `src/Supervertaler.Trados/TermLensEditorViewPart.cs` | Main ViewPart controller — Initialize(), segment events, settings, Alt+digit chords |
| `src/Supervertaler.Trados/Controls/MainPanelControl.cs` | Tabbed container (IUIControl) — hosts TermLens tab and future AI tabs |
| `src/Supervertaler.Trados/Controls/TermLensControl.cs` | TermLens glossary panel — header bar, FlowLayoutPanel with term blocks |
| `src/Supervertaler.Trados/Controls/TermBlock.cs` | Individual term chip (custom-painted) + WordLabel for unmatched words |
| `src/Supervertaler.Trados/AppInitializer.cs` | Runs at Trados startup; pre-loads `e_sqlite3.dll`, registers `AssemblyResolve` |
| `src/Supervertaler.Trados/Core/TermbaseReader.cs` | SQLite reader — Open(), LoadAllTerms(), InsertTerm(), InsertTermBatch(), UpdateTerm() |
| `src/Supervertaler.Trados/Core/TermMatcher.cs` | In-memory term matching + incremental AddEntry()/RemoveEntry() |
| `src/Supervertaler.Trados/Settings/TermLensSettings.cs` | JSON settings at `%LocalAppData%\Supervertaler.Trados\settings.json` |
| `src/Supervertaler.Trados/Settings/TermLensSettingsForm.cs` | Settings dialog — termbase picker, glossary management, import/export |
| `src/Supervertaler.Trados/Supervertaler.Trados.plugin.xml` | Extension manifest (UTF-16 LE — edit via Python to preserve encoding) |
| `build.sh` | Build → package → deploy script; aborts if Trados is running |
| `package_plugin.py` | Creates OPC-format `.sdlplugin` (NOT plain ZIP — needs `[Content_Types].xml`, `_rels/`) |

---

## Build / deploy rules

- **Trados must be fully closed** before running `bash build.sh` — it locks plugin files and skips re-extraction if `Unpacked/Supervertaler.Trados/` is non-empty. `build.sh` detects this via `tasklist.exe` and aborts.
- `build.sh` wipes `%LOCALAPPDATA%\Trados\...\Plugins\Unpacked\Supervertaler.Trados\` before deploying so Trados re-extracts cleanly on next start.
- `.sdlplugin` is OPC (Open Packaging Convention), like `.docx`. Requires `[Content_Types].xml` and `_rels/` entries — plain ZIP will silently fail to load.

---

## Naming conventions

- **Plugin name**: "Supervertaler for Trados" (visible in Trados docking header and plugin manager)
- **Glossary panel name**: "TermLens" (tab label inside the ViewPart — kept as the feature name)
- **Action IDs**: Prefixed with `TermLens_` for glossary-related actions (e.g. `TermLens_AddTerm`, `TermLens_TermPicker`); do NOT rename these — users may have custom shortcut overrides
- **Class names**: TermLens-prefixed classes (`TermLensEditorViewPart`, `TermLensControl`, etc.) are the glossary feature; future AI classes will use different naming
- **Settings auto-migrate** from old `%LocalAppData%\TermLens\` to `%LocalAppData%\Supervertaler.Trados\` on first run

---

## SQLite / WAL notes

- `supervertaler.db` uses WAL mode (Write-Ahead Log). Leftover `.db-wal` / `.db-shm` files after non-clean Supervertaler shutdown are harmless — SQLite replays the WAL on next open.
- Connection string uses `SqliteConnectionStringBuilder` with `Mode = SqliteOpenMode.ReadOnly` — safe for concurrent access while Supervertaler has the DB open.

---

## Term add/edit/delete: incremental index updates

The quick-add actions (Alt+Down, Alt+Up) and right-click edit/delete use **incremental in-memory index updates** instead of reloading the entire database:

- **`TermMatcher.AddEntry(TermEntry)`** — inserts one entry into `_termIndex` under both the lowercase key and stripped-punctuation variant. O(1).
- **`TermMatcher.RemoveEntry(long termId)`** — removes entries by ID from all keys.
- **`TermbaseReader.InsertTermBatch()`** — inserts into multiple write termbases in a single SQLite connection + transaction, instead of one connection per termbase.
- **`NotifyTermInserted(List<TermEntry>)`** — adds entries to the index and refreshes the UI. No settings reload, no DB reload.
- **`NotifyTermDeleted(long termId)`** — removes from index and refreshes.
- **`NotifyTermAdded()`** — the old full-reload path. Still used by the settings dialog when the user toggles glossaries.

The edit handler (right-click → Edit) does a remove + add of the updated entry.

On app startup or settings change, `LoadTermbase(forceReload: true)` still does a full DB load to ensure consistency.

---

## Non-translatable terms

Terms can be marked as **non-translatable** (brand names, product codes, abbreviations that stay the same across languages). These are stored with `is_nontranslatable = 1` in the `termbase_terms` table and `TargetTerm = SourceTerm`.

- **Visual**: Non-translatable chips render with a **light yellow background** (`#FFF3D0`). Color precedence: non-translatable (yellow) > project (pink) > regular (blue).
- **Keyboard shortcut**: `Ctrl+Alt+N` — quick-adds the selected source text as non-translatable to all Write termbases (target is set to source automatically). Only requires source text selected.
- **Right-click menu**: "Mark as Non-Translatable" / "Mark as Translatable" toggle on any term chip. Uses `TermbaseReader.SetNonTranslatable()` for a lightweight DB update.
- **Add Term dialog**: "Non-translatable" checkbox auto-fills target = source and makes target read-only when checked. Pre-populates from `TermEntry.IsNonTranslatable` in edit mode.
- **Alt+digit insertion**: Works unchanged — inserts `TargetTerm` which equals `SourceTerm` for non-translatables.
- **Term Picker** (Ctrl+Shift+G): Shows yellow background for non-translatable matches.
- **Glossary Editor**: "NT" checkbox column for toggling per-term.
- **DB migration**: `MigrateSchema()` uses `PRAGMA table_info` to detect the column and `ALTER TABLE ADD COLUMN` if missing. Called from `Open()` (via `HasColumn`) and all static write methods. Idempotent and backward-compatible with older Supervertaler databases.
- **Action ID**: `TermLens_QuickAddNonTranslatable` — do NOT rename (users may have custom shortcut overrides).

---

## License

Source-available license (not MIT). Source code viewable/forkable for personal use, but binary redistribution (.sdlplugin) restricted to copyright holder. Pre-built binaries available at supervertaler.com.

---

## Monetization

- Source code is open on GitHub (source-available license)
- Pre-built .sdlplugin binaries sold via monthly/annual subscription
- Technical support included with subscription
- Payment platform: TBD (Lemon Squeezy or similar — handles EU VAT)
- License key validation planned: key entered in plugin settings, validated against payment platform API
- Free tier: TermLens glossary features. Paid tier: AI features (Batch Translate, AI Assistant)

---

## Planned features

### AI Chat Assistant (next up)

The "AI Assistant" tab in the main ViewPart is currently a placeholder. Design decisions:

- **Separate dockable ViewPart** — the chat UI needs space that a tab in the existing narrow panel can't provide. Register a second `AbstractViewPartController` in `plugin.xml` so it becomes a fully native Trados dockable panel. Users can dock it right, bottom, floating, or on a second monitor. Position/size persists across sessions automatically (Trados handles this). Many RWS AppStore plugins use multiple dockable ViewParts.
- **The "AI Assistant" tab becomes a launcher** — shows an "Open AI Assistant" button that activates the dockable panel, plus a brief description. Alternatively, the tab could be removed entirely — the panel would just appear in Trados's View menu like any other dockable panel.
- **Implementation**: New `AiAssistantViewPart` (extends `AbstractViewPartController`) + `AiAssistantControl` (the chat UI). Register in `Supervertaler.Trados.plugin.xml` as a separate ViewPart.
- **Same LLM providers as Batch Translate** — OpenAI, Anthropic, Google (reuse `AiSettings` for API keys, provider/model selection).
- **Project-aware context** — the assistant should have access to: current segment (source + target), current file info, TM matches, glossary terms from TermLens. This makes it useful for asking "why was this translated this way?" or "suggest a better translation for this term in context."
- **Conversation history** — TBD whether to persist across sessions or keep ephemeral.
- **Ability to apply suggestions** — the assistant should be able to insert its suggestion directly into the target segment (with user confirmation).
- **Layout tip for laptop users** — consider showing a first-run tip suggesting users dock the Supervertaler panel to the right of the editor grid (instead of above/below) for better screen real estate usage on smaller screens.

### Other planned features

- **TBX support** — to be added simultaneously in both Supervertaler and this plugin
