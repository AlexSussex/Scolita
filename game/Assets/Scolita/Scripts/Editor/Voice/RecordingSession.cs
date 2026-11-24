using System;
using System.Collections.Generic;
using UnityEngine;

namespace Scolita.Editor.Voice
{
    /// <summary>
    /// One recording session: one language, one planet (or the lines used everywhere), at most
    /// <see cref="RecordingSessionPlanner.MostItems"/> items after the level test. Its manifest is
    /// <c>local/voice-inbox/scripts/&lt;id&gt;.json</c>, beside its printable page.
    /// </summary>
    [Serializable]
    public sealed class RecordingSession
    {
        [SerializeField] private string id;
        [SerializeField] private string language;
        [SerializeField] private string place;
        [SerializeField] private List<RecordingItem> items = new List<RecordingItem>();

        private RecordingSession()
        {
        }

        /// <summary>Creates a session; called by <see cref="RecordingSessionPlanner"/> and tests.</summary>
        public RecordingSession(string id, string language, string place, IEnumerable<RecordingItem> items)
        {
            this.id = id;
            this.language = language;
            this.place = place;
            this.items = new List<RecordingItem>(items);
        }

        /// <summary>Its id, also its folder's name in the inbox (<c>ro-planet-1-1</c>).</summary>
        public string Id => id;

        /// <summary>Its language code.</summary>
        public string Language => language;

        /// <summary>The planet (unit id) whose lines it holds, or <c>everywhere</c>.</summary>
        public string Place => place;

        /// <summary>Its items, the level test first.</summary>
        public IReadOnlyList<RecordingItem> Items => items;

        /// <summary>The session as JSON, for its manifest; called by Make recording scripts.</summary>
        public string ToJson()
        {
            return JsonUtility.ToJson(this, true);
        }

        /// <summary>
        /// Reads a manifest; throws an <see cref="ArgumentException"/> for one that isn't a session.
        /// Called by the session import and tests.
        /// </summary>
        public static RecordingSession FromJson(string json)
        {
            RecordingSession session = JsonUtility.FromJson<RecordingSession>(json);
            if (session == null || string.IsNullOrEmpty(session.id) || session.items == null || session.items.Count == 0)
            {
                throw new ArgumentException("The manifest is not a recording session.", nameof(json));
            }

            return session;
        }
    }
}
