using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Scolita.Editor.Voice
{
    /// <summary>
    /// One item of an imported session: its clip (as WAV in the session's <c>clips/</c>), its
    /// measurement and its flags. Kept in the session's <c>result.json</c> for Accept recording session.
    /// </summary>
    [Serializable]
    public sealed class RecordingResultItem
    {
        [SerializeField] private int number;
        [SerializeField] private string line;
        [SerializeField] private string clip;
        [SerializeField] private string text;
        [SerializeField] private string wav;
        [SerializeField] private string measurement;
        [SerializeField] private List<string> flags = new List<string>();

        private RecordingResultItem()
        {
        }

        /// <summary>Creates an item; called by <see cref="RecordingSessionImport"/> and tests.</summary>
        public RecordingResultItem(RecordingItem item, string wav, string measurement, IEnumerable<ClipFlag> flags)
        {
            number = item.Number;
            line = item.Line;
            clip = item.Clip;
            text = item.Text;
            this.wav = wav ?? string.Empty;
            this.measurement = measurement ?? string.Empty;
            this.flags = flags.Select(flag => flag.ToString()).ToList();
        }

        /// <summary>Its number in the script.</summary>
        public int Number => number;

        /// <summary>The voice line's id; empty for the level test.</summary>
        public string Line => line;

        /// <summary>The clip's file name in <c>content/voice/</c>.</summary>
        public string Clip => clip;

        /// <summary>What was to be said.</summary>
        public string Text => text;

        /// <summary>Its WAV's file name in the session's <c>clips/</c>; empty when the line seems missed.</summary>
        public string Wav => wav;

        /// <summary>Its numbers, in words.</summary>
        public string Measurement => measurement;

        /// <summary>What is wrong (the <see cref="ClipFlag"/> names); empty when it is good.</summary>
        public IReadOnlyList<string> Flags => flags;

        /// <summary>Whether it is good to accept: a line's clip, there, with no flag.</summary>
        public bool IsGood => !string.IsNullOrEmpty(line) && wav.Length > 0 && flags.Count == 0;
    }
}
