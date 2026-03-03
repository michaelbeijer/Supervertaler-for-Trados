using System;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using System.Text;

namespace Termview.Settings
{
    /// <summary>
    /// Persisted settings for the Termview plugin.
    /// Stored at %LocalAppData%\Termview\settings.json.
    /// </summary>
    [DataContract]
    public class TermviewSettings
    {
        private static readonly string SettingsDir = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
            "Termview");

        private static readonly string SettingsFile = Path.Combine(SettingsDir, "settings.json");

        [DataMember(Name = "termbasePath")]
        public string TermbasePath { get; set; } = "";

        [DataMember(Name = "autoLoadOnStartup")]
        public bool AutoLoadOnStartup { get; set; } = true;

        /// <summary>
        /// Loads settings from disk. Returns default settings if the file doesn't exist or can't be read.
        /// </summary>
        public static TermviewSettings Load()
        {
            try
            {
                if (!File.Exists(SettingsFile))
                    return new TermviewSettings();

                var json = File.ReadAllText(SettingsFile, Encoding.UTF8);
                using (var stream = new MemoryStream(Encoding.UTF8.GetBytes(json)))
                {
                    var serializer = new DataContractJsonSerializer(typeof(TermviewSettings));
                    return (TermviewSettings)serializer.ReadObject(stream);
                }
            }
            catch
            {
                return new TermviewSettings();
            }
        }

        /// <summary>
        /// Saves settings to disk.
        /// </summary>
        public void Save()
        {
            try
            {
                Directory.CreateDirectory(SettingsDir);

                using (var stream = new MemoryStream())
                {
                    var settings = new DataContractJsonSerializerSettings
                    {
                        UseSimpleDictionaryFormat = true
                    };
                    var serializer = new DataContractJsonSerializer(typeof(TermviewSettings), settings);
                    serializer.WriteObject(stream, this);

                    // Pretty-print by re-parsing (DataContractJsonSerializer writes compact JSON)
                    var json = Encoding.UTF8.GetString(stream.ToArray());
                    File.WriteAllText(SettingsFile, json, Encoding.UTF8);
                }
            }
            catch
            {
                // Silently ignore save failures
            }
        }
    }
}
