# Supervertaler for Trados – Codebase Health Scan Checklist

A systematic checklist for auditing an AI-assisted ("vibe coded") C#/.NET Trados plugin codebase, informed by the lessons from David Linthicum's "The AI coding hangover" (InfoWorld, March 2026).

---

## 1. Consistency and coherence

AI-generated code across many sessions tends to drift – solving similar problems in different ways, using different naming conventions, or structuring similar classes inconsistently.

- [ ] **Naming conventions**: Are classes, methods, properties, and variables consistently named? (e.g. camelCase vs PascalCase applied uniformly, no mixing of `Get`/`Fetch`/`Retrieve` for the same concept)
- [ ] **Error handling patterns**: Is there one consistent approach? (e.g. always try/catch at the boundary layer, or always return Result objects – not a mix of both)
- [ ] **Logging**: Is there a single logging approach throughout, or do some modules use `Console.WriteLine`, others `Debug.WriteLine`, others a proper logger?
- [ ] **Configuration access**: Is config/settings accessed the same way everywhere? No hardcoded values in some places and config lookups in others?
- [ ] **Async patterns**: Are `async`/`await` used consistently? No mixing of `.Result` or `.Wait()` blocking calls alongside proper async usage?
- [ ] **String handling**: Consistent use of string interpolation vs concatenation vs `string.Format`?
- [ ] **Null handling**: Consistent approach – nullable reference types, null checks, or null-conditional operators applied uniformly?

## 2. Over-abstraction and unnecessary complexity

LLMs love to create abstraction layers, interfaces, and design patterns even when they're not needed – especially for a solo-developer plugin.

- [ ] **Interfaces with single implementations**: Any `IFoo` that only has one `Foo` implementing it and no realistic prospect of a second implementation?
- [ ] **Factory/strategy/builder patterns**: Used where a simple constructor or method would do?
- [ ] **Wrapper classes**: Classes that just delegate to another class without adding meaningful logic?
- [ ] **Deep inheritance hierarchies**: More than 2 levels deep without clear justification?
- [ ] **Over-parameterised methods**: Methods with many parameters that are always called with the same values?
- [ ] **Dead code**: Unused methods, classes, or branches that were generated but never called?

## 3. Duplicated logic

Different AI sessions may produce similar solutions to similar problems without awareness of what already exists.

- [ ] **Duplicate utility methods**: String manipulation, file path handling, XML processing done in multiple places?
- [ ] **Repeated API call patterns**: HTTP client setup, request building, response parsing duplicated across modules?
- [ ] **Copy-pasted error handling**: Same try/catch/log/rethrow blocks repeated rather than centralised?
- [ ] **Similar data classes**: Multiple DTOs or models representing the same concept with slight variations?
- [ ] **Duplicate validation logic**: Input validation for the same data done in multiple locations?

## 4. Dependency hygiene

- [ ] **NuGet audit**: Run `dotnet list package --vulnerable` – are there any packages with known CVEs?
- [ ] **Unused packages**: Any NuGet references that are installed but never actually used in the code?
- [ ] **Transitive dependency bloat**: Any packages pulling in heavy transitive dependencies for minimal functionality?
- [ ] **Pinned versions**: Are package versions pinned to specific versions or using floating ranges that could break on update?
- [ ] **Framework compatibility**: All packages compatible with the target .NET Framework / .NET version required by the Trados SDK?

## 5. Security – especially critical for a commercial plugin

- [ ] **API key handling**: Are LLM API keys (OpenAI, Anthropic, Google) stored securely? Not hardcoded, not in plain text config files, not logged?
- [ ] **Secrets in source control**: Any API keys, tokens, or credentials committed to the repo (check git history too)?
- [ ] **HTTPS enforcement**: All API calls use HTTPS? Certificate validation not disabled?
- [ ] **Input sanitisation**: User input from Trados UI properly sanitised before use in API calls or file operations?
- [ ] **Localhost security (Okapi sidecar)**: If the plugin communicates with a localhost REST API, is it bound only to 127.0.0.1? Is there any auth/token mechanism to prevent other local processes from using it?
- [ ] **Sensitive data in logs**: Are translation segments, API responses, or user content logged in a way that could expose confidential client material?
- [ ] **Temporary files**: Are temp files containing translation data cleaned up properly? Especially on failure paths?

## 6. Performance – plugin responsiveness in Trados

- [ ] **Synchronous blocking on UI thread**: Any API calls or file I/O happening on the main thread that could freeze Trados?
- [ ] **HTTP client lifecycle**: Is `HttpClient` instantiated once and reused (recommended) or created per-request (causes socket exhaustion)?
- [ ] **Unnecessary allocations**: Large string concatenations in loops? Objects created inside hot paths that could be pooled or reused?
- [ ] **Batch processing**: When processing multiple segments, are they batched into fewer API calls where possible, or does each segment trigger a separate round-trip?
- [ ] **Timeout handling**: Reasonable timeouts on API calls? Cancellation token support so users can cancel long-running operations?
- [ ] **Memory leaks**: Event handler subscriptions not unsubscribed? Large objects held in memory after use?

## 7. Maintainability and comprehensibility

The article's key point – "debt without authorship." Could you (or someone else) understand and modify each module six months from now?

- [ ] **File/class organisation**: Does the project structure make sense? Can you find things logically?
- [ ] **Method length**: Any methods over ~50 lines that should be broken up?
- [ ] **Class responsibilities**: Each class does one coherent thing? No "god classes" doing everything?
- [ ] **Comments on non-obvious code**: Complex algorithms or workarounds explained? (Not trivial comments on obvious code)
- [ ] **Magic numbers/strings**: Unexplained numeric or string constants scattered through the code?
- [ ] **TODO/FIXME/HACK markers**: Outstanding items that need resolution before commercial release?

## 8. Trados SDK integration quality

- [ ] **SDK version alignment**: Plugin targets the correct Trados Studio SDK version?
- [ ] **Plugin lifecycle**: Proper initialisation and cleanup in plugin activation/deactivation?
- [ ] **SDLXLIFF handling**: Correct handling of segment statuses, confirmation levels, comments, and tracked changes?
- [ ] **Translation provider interface**: If implementing `ITranslationProvider`, are all required interface members properly implemented?
- [ ] **Settings persistence**: Plugin settings saved/loaded correctly across Trados sessions?
- [ ] **Error reporting to user**: Errors surfaced through Trados' own messaging/notification system rather than silent failures?

## 9. Build and deployment readiness

- [ ] **Clean build**: Does the solution build with zero warnings? (Treat warnings as potential issues)
- [ ] **Code analysis**: Run `dotnet build /p:EnforceCodeStyleInBuild=true` – any style violations?
- [ ] **Release configuration**: Debug-only code (e.g. `#if DEBUG` blocks) properly guarded?
- [ ] **Versioning**: Assembly version, file version, and any displayed version string consistent and following a scheme?
- [ ] **Installer/packaging**: Plugin package (.sdlplugin file) correctly structured for RWS AppStore submission?
- [ ] **Signing**: Assembly signed if required by the Trados plugin loading mechanism?

## 10. Testing

- [ ] **Unit tests exist**: At least for core logic (API communication, segment processing, TM lookup)?
- [ ] **Edge cases covered**: Empty segments, very long segments, segments with special characters, RTL text, inline tags?
- [ ] **Integration test**: Can the plugin be installed in a clean Trados instance and perform a basic workflow end-to-end?
- [ ] **Error path testing**: What happens when the API is unreachable? When the API key is invalid? When the Okapi sidecar is not running?
- [ ] **Performance test**: Process a large project (1000+ segments) – does performance degrade? Does memory usage grow unboundedly?

---

## How to use this checklist

1. Work through it section by section – don't try to do everything at once
2. For each item, note: ✅ OK / ⚠️ Needs attention / ❌ Problem found
3. Prioritise security (section 5) and performance (section 6) for a commercial release
4. Re-run periodically – especially after intensive AI-assisted coding sessions where you've generated a lot of new code

## Automated tooling to support the scan

Run these alongside the manual checklist:

```bash
# .NET dependency vulnerability scan
dotnet list package --vulnerable

# Build with code analysis
dotnet build /p:EnforceCodeStyleInBuild=true /warnaserror

# Find TODOs and FIXMEs
grep -rn "TODO\|FIXME\|HACK\|XXX" --include="*.cs" .

# Find potential hardcoded secrets
grep -rn "api[_-]key\|apikey\|secret\|password\|token" --include="*.cs" -i .

# Check for .Result or .Wait() async anti-patterns
grep -rn "\.Result\b\|\.Wait()" --include="*.cs" .

# Find large methods (rough heuristic – files with long unbroken code blocks)
# Better done with a proper tool like NDepend or SonarQube, but grep gives a start
```
