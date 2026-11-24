using System;
using System.Collections.Generic;
using Scolita.Core.Content;
using UnityEngine;

namespace Scolita.Game.Lessons
{
    /// <summary>
    /// Each activity kind's view prefab (AGENTS.md, seam 3), the one asset
    /// (<c>Prefabs/Lesson/ActivityRegistry.asset</c>) through which <see cref="LessonRunner"/> finds
    /// a step's view by its kind; and the kinds not built yet (the human, 2026-11-24), whose steps are
    /// skipped with a warning until their brief takes them off the list. The content validator's
    /// registry test checks that every kind a lesson uses is in one or the other.
    /// </summary>
    [CreateAssetMenu(fileName = "ActivityRegistry", menuName = "Scolita/Activity registry")]
    public sealed class ActivityRegistry : ScriptableObject
    {
        [SerializeField] private Entry[] views = new Entry[0];
        [SerializeField] private ActivityKind[] notBuiltYet = new ActivityKind[0];

        /// <summary>The kinds whose steps are skipped until their brief builds them.</summary>
        public IReadOnlyList<ActivityKind> NotBuiltYet => notBuiltYet;

        /// <summary>
        /// The kinds that have a view, in the registry's order, as a new list. Called by
        /// <see cref="LessonRunner"/> when a lesson is built, and the registry's validator test.
        /// </summary>
        public IReadOnlyList<ActivityKind> KindsWithViews()
        {
            List<ActivityKind> kinds = new List<ActivityKind>(views.Length);
            foreach (Entry entry in views)
            {
                if (entry.View != null)
                {
                    kinds.Add(entry.Kind);
                }
            }

            return kinds;
        }

        /// <summary>
        /// The view prefab of <paramref name="kind"/>; false when it has none. Called by
        /// <see cref="LessonRunner"/> when a lesson is built, and the registry's validator test.
        /// </summary>
        public bool TryGetView(ActivityKind kind, out ActivityView view)
        {
            foreach (Entry entry in views)
            {
                if (entry.Kind == kind && entry.View != null)
                {
                    view = entry.View;
                    return true;
                }
            }

            view = null;
            return false;
        }

        /// <summary>One kind and its view prefab.</summary>
        [Serializable]
        private struct Entry
        {
            [SerializeField] private ActivityKind kind;
            [SerializeField] private ActivityView view;

            /// <summary>The kind.</summary>
            public ActivityKind Kind => kind;

            /// <summary>Its view prefab's root component.</summary>
            public ActivityView View => view;
        }
    }
}
