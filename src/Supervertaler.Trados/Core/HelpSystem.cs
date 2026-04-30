using System.Diagnostics;

namespace Supervertaler.Trados.Core
{
    /// <summary>
    /// Centralized context-sensitive help system.
    /// Maps UI elements to GitBook documentation pages and opens them in the browser.
    /// </summary>
    public static class HelpSystem
    {
        /// <summary>
        /// Base URL for the unified Supervertaler GitBook documentation site.
        /// Trados Plugin pages live under <c>/trados/</c>; Workbench pages live
        /// under <c>/workbench/</c>. The site previously used a <c>/trados</c>
        /// slug; that slug has been renamed to root so both products coexist
        /// at the same level.
        /// </summary>
        private const string DocsBaseUrl = "https://supervertaler.gitbook.io";

        /// <summary>
        /// Help topic identifiers. Each maps to a documentation page path
        /// rooted at the site base. Trados pages are prefixed with
        /// <c>trados/</c> to match the file-path structure in
        /// <c>docs/trados/</c>.
        /// </summary>
        public static class Topics
        {
            public const string Overview            = "trados/";
            public const string Installation        = "trados/installation";
            public const string GettingStarted      = "trados/getting-started";
            public const string TermLensPanel       = "trados/termlens";
            public const string AddTermDialog       = "trados/termlens/adding-terms";
            public const string TermLensPopup       = "trados/termlens/termlens-popup";
            public const string TermPickerDialog    = "trados/termlens/term-picker";
            public const string AiAssistantChat     = "trados/ai-assistant";
            public const string StudioTools         = "trados/ai-assistant/studio-tools";
            public const string QuickLauncher       = "trados/quicklauncher";
            public const string BatchOperations     = "trados/batch-operations";
            public const string BatchTranslate      = "trados/batch-translate";
            public const string AiProofreader       = "trados/ai-proofreader";
            public const string AiProofreaderReports = "trados/ai-proofreader#reports-tab";
            public const string PromptLogging       = "trados/settings/ai-settings#prompt-logging";
            public const string AiCostGuide         = "trados/ai-cost-guide";
            public const string ClipboardMode       = "trados/clipboard-mode";
            public const string GeneratePrompt      = "trados/generate-prompt";
            public const string MultiTermSupport    = "trados/multiterm-support";
            public const string TermbaseEditor      = "trados/termbase-management";
            public const string SettingsGeneral     = "trados/settings/usage-statistics";
            public const string SettingsTermLens    = "trados/settings/termlens";
            public const string SettingsAi          = "trados/settings/ai-settings";
            public const string SettingsPrompts     = "trados/settings/prompts";
            public const string SettingsBackup      = "trados/settings/backup";
            public const string SettingsUsageStats  = "trados/settings/usage-statistics";
            public const string Licensing           = "trados/licensing";
            public const string ProjectSettings     = "trados/settings/project-settings";
            public const string KeyboardShortcuts   = "trados/keyboard-shortcuts";
            public const string Troubleshooting     = "trados/troubleshooting";
            // Memory banks (formerly "SuperMemory") — now nested under the Supervertaler
            // Assistant section in the GitBook SUMMARY. C# identifier names kept as
            // SuperMemory* for backwards-compat with existing call sites; rename when the
            // Trados UI strings are updated to match the new memory bank terminology.
            public const string SuperMemory         = "trados/ai-assistant/super-memory";
            public const string SuperMemoryQuickAdd = "trados/ai-assistant/super-memory/quick-add";
            public const string SuperMemoryInbox    = "trados/ai-assistant/super-memory/process-inbox";
            public const string SuperMemoryHealth   = "trados/ai-assistant/super-memory/health-check";
            public const string SuperMemoryDistill  = "trados/ai-assistant/super-memory/distill";
            public const string SuperMemoryObsidian = "trados/ai-assistant/super-memory/obsidian-setup";
            public const string SuperSearch         = "trados/supersearch";
        }

        /// <summary>
        /// Opens the help page for the given topic identifier.
        /// Falls back to the Trados section root if topic is null/empty.
        /// </summary>
        public static void OpenHelp(string topic = null)
        {
            string url = string.IsNullOrEmpty(topic)
                ? DocsBaseUrl + "/trados/"
                : DocsBaseUrl + "/" + topic.TrimStart('/');

            OpenUrl(url);
        }

        /// <summary>
        /// Opens the docs site root (the product chooser landing page).
        /// </summary>
        public static void OpenDocsHome()
        {
            OpenUrl(DocsBaseUrl);
        }

        private static void OpenUrl(string url)
        {
            try
            {
                Process.Start(new ProcessStartInfo(url) { UseShellExecute = true });
            }
            catch
            {
                // No default browser configured — silently ignore
            }
        }
    }
}
