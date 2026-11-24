using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using Scolita.Core.Activities;
using Scolita.Core.Content;
using Scolita.Game.Lessons;
using Scolita.Tests.EditMode.Fakes;
using UnityEditor;

namespace Scolita.Tests.EditMode.Content
{
    /// <summary>
    /// The content validator's check of <see cref="ActivityRegistry"/> (<c>Prefabs/Lesson/ActivityRegistry.asset</c>)
    /// against the real content: every kind a lesson in use plays has a view and rules, or waits on the
    /// registry's "not built yet" list (the human, 2026-11-24), which is printed as a warning.
    /// </summary>
    public sealed class ActivityRegistryTests
    {
        /// <summary>
        /// Tests that <see cref="ActivityRegistry.TryGetView"/> finds a view, and <see cref="ActivityRulesFactory.Has"/>
        /// rules, for every kind the content uses, unless the kind is not built yet.
        /// </summary>
        [Test]
        public void TestTryGetView_EveryKindTheContentUses()
        {
            ActivityRegistry registry = AssetDatabase.LoadAssetAtPath<ActivityRegistry>(RegistryPath);
            Curriculum curriculum = RealContent.Read();
            ActivityKind[] used = curriculum.Domains
                .SelectMany(domain => domain.Units)
                .Where(unit => !unit.Retired)
                .SelectMany(unit => unit.Lessons)
                .Where(lesson => !lesson.Retired)
                .SelectMany(lesson => lesson.Steps)
                .Select(step => step.Kind)
                .Distinct()
                .ToArray();

            Assert.That(registry, Is.Not.Null, "no registry at " + RegistryPath);
            List<string> missing = new List<string>();
            foreach (ActivityKind kind in used.Where(kind => !registry.NotBuiltYet.Contains(kind)))
            {
                if (!registry.TryGetView(kind, out ActivityView _) || !ActivityRulesFactory.Has(kind))
                {
                    missing.Add(kind.ToString());
                }
            }

            Assert.That(missing, Is.Empty, "kinds used by the content with no view or no rules");
            foreach (ActivityKind kind in used.Where(kind => registry.NotBuiltYet.Contains(kind)))
            {
                TestContext.Out.WriteLine("Warning: " + kind + " is not built yet; its steps are skipped.");
            }
        }

        /// <summary>
        /// Tests that the registry's lists don't contradict each other: a kind with a view isn't also "not
        /// built yet", every kind with a view has rules, and each view is listed once.
        /// </summary>
        [Test]
        public void TestKindsWithViews_AgreeWithTheRules()
        {
            ActivityRegistry registry = AssetDatabase.LoadAssetAtPath<ActivityRegistry>(RegistryPath);

            IReadOnlyList<ActivityKind> withViews = registry.KindsWithViews();

            Assert.That(withViews, Is.Unique);
            Assert.That(withViews.Intersect(registry.NotBuiltYet), Is.Empty);
            Assert.That(withViews.All(ActivityRulesFactory.Has), Is.True);
            Assert.That(withViews, Is.EquivalentTo(new[]
            {
                ActivityKind.MeetLetter, ActivityKind.ListenAndPick, ActivityKind.LetterHunt, ActivityKind.FirstSound,
                ActivityKind.TwoSounds
            }));
        }

        private static string RegistryPath => "Assets/Scolita/Prefabs/Lesson/ActivityRegistry.asset";
    }
}
