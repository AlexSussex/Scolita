using System;
using System.Collections.Generic;
using UnityEngine;

namespace Scolita.Editor.Voice
{
    /// <summary>What importing a session made: how the take matched its script, and each item's clip and flags (<c>result.json</c>).</summary>
    [Serializable]
    public sealed class RecordingResult
    {
        [SerializeField] private string session;
        [SerializeField] private string summary;
        [SerializeField] private List<RecordingResultItem> items = new List<RecordingResultItem>();

        private RecordingResult()
        {
        }

        /// <summary>Creates a result; called by <see cref="RecordingSessionImport"/>.</summary>
        public RecordingResult(string session, string summary, IEnumerable<RecordingResultItem> items)
        {
            this.session = session;
            this.summary = summary;
            this.items = new List<RecordingResultItem>(items);
        }

        /// <summary>The session's id.</summary>
        public string Session => session;

        /// <summary>How the take matched its script, in words.</summary>
        public string Summary => summary;

        /// <summary>Each item, the level test first.</summary>
        public IReadOnlyList<RecordingResultItem> Items => items;

        /// <summary>The result as JSON; called by the session import.</summary>
        public string ToJson()
        {
            return JsonUtility.ToJson(this, true);
        }

        /// <summary>Reads a result; throws an <see cref="ArgumentException"/> for one that isn't. Called by Accept recording session.</summary>
        public static RecordingResult FromJson(string json)
        {
            RecordingResult result = JsonUtility.FromJson<RecordingResult>(json);
            if (result == null || string.IsNullOrEmpty(result.session) || result.items == null)
            {
                throw new ArgumentException("The file is not a recording session's result.", nameof(json));
            }

            return result;
        }
    }
}
