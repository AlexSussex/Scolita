using System;
using UnityEngine;

namespace Scolita.Editor.Voice
{
    /// <summary>
    /// One numbered item of a recording session's script: a spoken segment of a voice line (or the
    /// level test, number 0), the clip it becomes, and how long it is expected to last (for matching
    /// the take). Kept in the session's manifest as JSON.
    /// </summary>
    [Serializable]
    public sealed class RecordingItem
    {
        [SerializeField] private int number;
        [SerializeField] private string line;
        [SerializeField] private int segment;
        [SerializeField] private int segments;
        [SerializeField] private string clip;
        [SerializeField] private string kind;
        [SerializeField] private string text;
        [SerializeField] private string continuesWith;
        [SerializeField] private float expectedSeconds;

        private RecordingItem()
        {
        }

        /// <summary>Creates an item; called by <see cref="RecordingSessionPlanner"/> and tests.</summary>
        public RecordingItem(int number, string line, int segment, int segments, string clip, string kind, string text,
            string continuesWith, float expectedSeconds)
        {
            this.number = number;
            this.line = line;
            this.segment = segment;
            this.segments = segments;
            this.clip = clip;
            this.kind = kind;
            this.text = text;
            this.continuesWith = continuesWith;
            this.expectedSeconds = expectedSeconds;
        }

        /// <summary>Its number in the script (0 for the level test).</summary>
        public int Number => number;

        /// <summary>The voice line's id; empty for the level test.</summary>
        public string Line => line;

        /// <summary>Which spoken segment of the line it is, from 0.</summary>
        public int Segment => segment;

        /// <summary>How many spoken segments the line has.</summary>
        public int Segments => segments;

        /// <summary>The clip's file name in <c>content/voice/</c>; empty for the level test.</summary>
        public string Clip => clip;

        /// <summary>The line's kind code (<c>sound</c>, <c>word</c>, <c>line</c> …).</summary>
        public string Kind => kind;

        /// <summary>What to say.</summary>
        public string Text => text;

        /// <summary>What the app adds after it (the letter, the word), shown on the script; empty for none.</summary>
        public string ContinuesWith => continuesWith ?? string.Empty;

        /// <summary>How long it is expected to last, in seconds.</summary>
        public float ExpectedSeconds => expectedSeconds;

        /// <summary>Whether it is the level test, which is not kept.</summary>
        public bool IsLevelTest => string.IsNullOrEmpty(line);
    }
}
