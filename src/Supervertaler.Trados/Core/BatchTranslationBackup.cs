using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Supervertaler.Trados.Core
{
    /// <summary>
    /// Writes translated segments to a TMX backup file as they arrive during a batch
    /// translate run. The file is flushed (overwritten in full) every
    /// <see cref="FlushInterval"/> segments so that a Trados crash never loses more
    /// than one flush interval worth of translations.
    ///
    /// The backup TMX can be imported into any TM to recover translations.
    ///
    /// Thread-safety: <see cref="AddSegment"/> and <see cref="Flush"/> are protected
    /// by a lock so they can be called from any thread.
    /// </summary>
    internal class BatchTranslationBackup
    {
        /// <summary>
        /// Number of segments between automatic flushes. A full rewrite of the TMX
        /// is triggered every time this many new segments have been added since the
        /// last flush.
        /// </summary>
        private const int FlushInterval = 10;

        private readonly string _filePath;
        private readonly string _sourceLang;
        private readonly string _targetLang;
        private readonly string _pluginVersion;
        private readonly List<(string Source, string Target)> _entries =
            new List<(string, string)>();
        private readonly object _lock = new object();
        private int _lastFlushedCount = 0;

        /// <summary>
        /// Path of the TMX file being written. Exposed so callers can log or display it.
        /// </summary>
        public string FilePath => _filePath;

        /// <summary>Number of segments written so far (including unflushed ones).</summary>
        public int Count { get { lock (_lock) return _entries.Count; } }

        public BatchTranslationBackup(
            string filePath,
            string sourceLang,
            string targetLang,
            string pluginVersion = "")
        {
            _filePath = filePath;
            _sourceLang = sourceLang ?? "EN-US";
            _targetLang = targetLang ?? "NL-NL";
            _pluginVersion = pluginVersion ?? "";
        }

        /// <summary>
        /// Records a translated segment and triggers a flush if the interval is reached.
        /// Safe to call from any thread.
        /// </summary>
        public void AddSegment(string sourceText, string translatedText)
        {
            if (string.IsNullOrEmpty(sourceText) || string.IsNullOrEmpty(translatedText))
                return;

            lock (_lock)
            {
                _entries.Add((sourceText, translatedText));
                if (_entries.Count - _lastFlushedCount >= FlushInterval)
                    WriteTmxInternal();
            }
        }

        /// <summary>
        /// Forces an immediate write of all accumulated segments to disk.
        /// Call at the end of the batch translate run (completed or cancelled).
        /// Safe to call from any thread.
        /// </summary>
        public void Flush()
        {
            lock (_lock)
            {
                if (_entries.Count > _lastFlushedCount)
                    WriteTmxInternal();
            }
        }

        // ─── Private ─────────────────────────────────────────────────

        /// <summary>
        /// Writes the full TMX to a temp file then renames it over the target path.
        /// The rename makes the update atomic on Windows — a reader always sees a
        /// complete, valid TMX file (either the old one or the new one, never a partial).
        /// Must be called with <see cref="_lock"/> held.
        /// </summary>
        private void WriteTmxInternal()
        {
            try
            {
                Directory.CreateDirectory(Path.GetDirectoryName(_filePath));

                var tempPath = _filePath + ".tmp";

                using (var writer = new StreamWriter(tempPath, false, new UTF8Encoding(true)))
                {
                    writer.WriteLine("<?xml version=\"1.0\" encoding=\"utf-8\"?>");
                    writer.WriteLine("<tmx version=\"1.4\">");
                    writer.WriteLine(
                        $"  <header creationtool=\"Supervertaler for Trados\"" +
                        $" creationtoolversion=\"{EscapeXml(_pluginVersion)}\"" +
                        $" datatype=\"plaintext\" segtype=\"sentence\"" +
                        $" adminlang=\"en-US\" srclang=\"{EscapeXml(_sourceLang)}\"/>");
                    writer.WriteLine("  <body>");

                    foreach (var (src, tgt) in _entries)
                    {
                        writer.WriteLine("    <tu>");
                        writer.WriteLine($"      <tuv xml:lang=\"{EscapeXml(_sourceLang)}\">");
                        writer.WriteLine($"        <seg>{EscapeXml(src)}</seg>");
                        writer.WriteLine("      </tuv>");
                        writer.WriteLine($"      <tuv xml:lang=\"{EscapeXml(_targetLang)}\">");
                        writer.WriteLine($"        <seg>{EscapeXml(tgt)}</seg>");
                        writer.WriteLine("      </tuv>");
                        writer.WriteLine("    </tu>");
                    }

                    writer.WriteLine("  </body>");
                    writer.WriteLine("</tmx>");
                }

                // Atomic swap
                if (File.Exists(_filePath)) File.Delete(_filePath);
                File.Move(tempPath, _filePath);

                _lastFlushedCount = _entries.Count;
            }
            catch
            {
                // Never let a backup failure interrupt or crash the translation run.
                // Silently swallow — the translation itself continues regardless.
            }
        }

        private static string EscapeXml(string text)
        {
            if (string.IsNullOrEmpty(text)) return string.Empty;
            return text
                .Replace("&", "&amp;")
                .Replace("<", "&lt;")
                .Replace(">", "&gt;")
                .Replace("\"", "&quot;")
                .Replace("'", "&apos;");
        }
    }
}
