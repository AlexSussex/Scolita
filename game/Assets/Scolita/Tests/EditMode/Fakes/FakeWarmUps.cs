using System.Collections.Generic;
using Scolita.Core.Content;
using Scolita.Core.Lessons;

namespace Scolita.Tests.EditMode.Fakes
{
    /// <summary>An <see cref="IWarmUps"/> that gives the same steps before any lesson and notes which lessons asked. Used by the lesson tests.</summary>
    public sealed class FakeWarmUps : IWarmUps
    {
        private readonly Step[] steps;
        private readonly List<Lesson> askedFor = new List<Lesson>();

        /// <summary>Gives <paramref name="warmUps"/>; called by the tests.</summary>
        public FakeWarmUps(params Step[] warmUps)
        {
            steps = warmUps;
        }

        /// <summary>The lessons it was asked for, in order.</summary>
        public IReadOnlyList<Lesson> AskedFor => askedFor;

        /// <inheritdoc/>
        public IReadOnlyList<Step> For(Lesson lesson)
        {
            askedFor.Add(lesson);
            return steps;
        }
    }
}
