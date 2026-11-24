using System.IO;
using UnityEngine;

namespace Scolita.Editor.Voice
{
    /// <summary>
    /// The recording kit's folders, in the human's own <c>local/</c> (never committed):
    /// <c>local/voice-inbox/scripts/</c> for the printable pages and their manifests, and
    /// <c>local/voice-inbox/&lt;session&gt;/</c> for each session's take, its clips and its report.
    /// </summary>
    public sealed class VoiceInbox
    {
        private const string ScriptsFolder = "scripts";
        private const string ClipsFolder = "clips";
        private const string WorkFolder = "work";

        /// <summary>Creates the inbox at the full path <paramref name="root"/>; called by the kit and its tests.</summary>
        public VoiceInbox(string root)
        {
            Root = root;
        }

        /// <summary>The inbox's full path.</summary>
        public string Root { get; }

        /// <summary>The folder of the scripts and manifests.</summary>
        public string Scripts => Path.Combine(Root, ScriptsFolder);

        /// <summary>The repository's inbox, <c>local/voice-inbox/</c> beside <c>game/</c>; called by the kit's menu items.</summary>
        public static VoiceInbox OfRepository()
        {
            string repositoryRoot = Directory.GetParent(Application.dataPath).Parent.FullName;
            return new VoiceInbox(Path.Combine(repositoryRoot, "local", "voice-inbox"));
        }

        /// <summary>The printable page of session <paramref name="id"/>.</summary>
        public string ScriptPage(string id)
        {
            return Path.Combine(Scripts, id + ".html");
        }

        /// <summary>The manifest of session <paramref name="id"/>.</summary>
        public string Manifest(string id)
        {
            return Path.Combine(Scripts, id + ".json");
        }

        /// <summary>The folder of session <paramref name="id"/>, where its take goes.</summary>
        public string Session(string id)
        {
            return Path.Combine(Root, id);
        }

        /// <summary>The folder of session <paramref name="id"/>'s clips, made by the import.</summary>
        public string Clips(string id)
        {
            return Path.Combine(Session(id), ClipsFolder);
        }

        /// <summary>The folder for session <paramref name="id"/>'s decoded take.</summary>
        public string Work(string id)
        {
            return Path.Combine(Session(id), WorkFolder);
        }

        /// <summary>The import's report page of session <paramref name="id"/>.</summary>
        public string Report(string id)
        {
            return Path.Combine(Session(id), "report.html");
        }

        /// <summary>The import's result (which clips are good) of session <paramref name="id"/>.</summary>
        public string Result(string id)
        {
            return Path.Combine(Session(id), "result.json");
        }
    }
}
