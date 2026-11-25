using System;
using Scolita.Core.Parent;
using Scolita.Core.Timing;
using Scolita.Game.Saving;

namespace Scolita.Game.Parent
{
    /// <summary>
    /// Counts the time a learner plays (the map or a lesson on screen, the app in front) into her
    /// <c>play-time.json</c> (<see cref="PlayTimeData"/>), for the daily limit. Ticked every frame by the scene's
    /// composition root, without allocating; saved by it when a step ends, when the scene closes and when the app
    /// goes to the background. Today is read when the clock is made and at each save, not every frame.
    /// </summary>
    public sealed class PlayClock
    {
        private const float LongestFrameCountedSeconds = 1f;

        private readonly SaveStore profileFiles;
        private readonly IClock clock;
        private CalendarDay today;
        private DailyPlay play;

        /// <summary>
        /// Reads the time played today from <paramref name="profileFiles"/>, the day from <paramref name="clock"/>.
        /// Called by the scenes' composition roots and the tests.
        /// </summary>
        public PlayClock(SaveStore profileFiles, IClock clock)
        {
            this.profileFiles = profileFiles ?? throw new ArgumentNullException(nameof(profileFiles));
            this.clock = clock ?? throw new ArgumentNullException(nameof(clock));
            today = clock.Today;
            play = profileFiles.Load(PlayTimeData.FileName, PlayTimeData.CreateDefaults()).Data.Play;
        }

        /// <summary>The time played today.</summary>
        public TimeSpan PlayedToday => play.PlayedOn(today);

        /// <summary>
        /// Counts <paramref name="seconds"/> more (this frame's time; a longer frame, after the app was away, counts as
        /// a second). Called every frame by the scenes' composition roots, and the tests.
        /// </summary>
        public void Tick(float seconds)
        {
            if (!(seconds > 0f))
            {
                return;
            }

            long ticks = (long)(Math.Min(seconds, LongestFrameCountedSeconds) * TimeSpan.TicksPerSecond);
            play = play.Plus(today, TimeSpan.FromTicks(ticks));
        }

        /// <summary>
        /// Whether today's play has reached <paramref name="limit"/> (never when it is off). Called by the map, and the
        /// tests.
        /// </summary>
        public bool HasReached(SessionLimit limit)
        {
            return limit.IsReachedBy(play, today);
        }

        /// <summary>
        /// Writes the time played (moving to the clock's day first, so a session that runs past midnight starts the
        /// new day there). Called by the scenes' composition roots and the tests.
        /// </summary>
        public bool Save()
        {
            today = clock.Today;
            play = play.Plus(today, TimeSpan.Zero);
            return profileFiles.Save(PlayTimeData.FileName, new PlayTimeData(play));
        }

        /// <summary>
        /// Wakes the rocket: nothing played yet today, saved at once. Called by the parent area when it opens (only the
        /// parent gate leads there), and the tests.
        /// </summary>
        public bool Wake()
        {
            today = clock.Today;
            play = DailyPlay.Woken(today);
            return profileFiles.Save(PlayTimeData.FileName, new PlayTimeData(play));
        }
    }
}
