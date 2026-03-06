namespace Supervertaler.Trados.Models
{
    /// <summary>
    /// Represents a prompt template loaded from the prompt library.
    /// Stored as Markdown files with YAML frontmatter in
    /// %LocalAppData%\Supervertaler.Trados\prompts\.
    /// </summary>
    public class PromptTemplate
    {
        /// <summary>Display name (from YAML 'name:' field or filename).</summary>
        public string Name { get; set; } = "";

        /// <summary>One-line description (from YAML 'description:' field).</summary>
        public string Description { get; set; } = "";

        /// <summary>Domain or category, e.g. "Medical/Healthcare" (from YAML 'domain:' or folder name).</summary>
        public string Domain { get; set; } = "";

        /// <summary>The actual prompt text (everything after the YAML frontmatter).</summary>
        public string Content { get; set; } = "";

        /// <summary>Full filesystem path to the .md file.</summary>
        public string FilePath { get; set; } = "";

        /// <summary>
        /// Relative path from the prompts root directory (e.g. "Domain Expertise/Medical Translation Specialist.md").
        /// Used as the stable identifier for settings persistence.
        /// </summary>
        public string RelativePath { get; set; } = "";

        /// <summary>True if this prompt was shipped with the plugin (can be restored if deleted).</summary>
        public bool IsBuiltIn { get; set; }

        /// <summary>True if this prompt is from the Supervertaler desktop app (shared, not editable from here).</summary>
        public bool IsReadOnly { get; set; }

        public override string ToString() => Name;
    }
}
