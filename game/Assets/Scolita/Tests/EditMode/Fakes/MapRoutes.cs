using System.Linq;
using Scolita.Core.Activities;
using Scolita.Core.Content;
using Scolita.Core.Languages;
using Scolita.Core.Lessons;
using Scolita.Core.Map;

namespace Scolita.Tests.EditMode.Fakes
{
    /// <summary>
    /// Galaxy maps of the real content for the map tests: every kind with rules can be played, and
    /// the lessons named are done. Used by <c>MapRouteTests</c>, <c>RocketMoveTests</c> and <c>MapLinesTests</c>.
    /// </summary>
    public static class MapRoutes
    {
        /// <summary>The map of <paramref name="curriculum"/> for <paramref name="mix"/> with <paramref name="doneLessons"/> done; used by the map tests.</summary>
        public static MapRoute Of(Curriculum curriculum, LanguageMix mix, params string[] doneLessons)
        {
            return Of(curriculum, mix, Progress(null, doneLessons));
        }

        /// <summary>The map of <paramref name="curriculum"/> for <paramref name="mix"/> with <paramref name="progress"/>; used by the map tests.</summary>
        public static MapRoute Of(Curriculum curriculum, LanguageMix mix, IDoneLessons progress)
        {
            Domain domain = curriculum.Domains[0];
            LessonOrder order = new LessonOrder(domain, mix, new PlayableSteps(ActivityRulesFactory.Kinds, curriculum));
            return new MapRoute(curriculum, domain, order, progress);
        }

        /// <summary>
        /// The map of <paramref name="curriculum"/> in both languages with every lesson done up to (not
        /// including) <paramref name="nextLesson"/>; used by the map tests.
        /// </summary>
        public static MapRoute Before(Curriculum curriculum, string nextLesson)
        {
            MapRoute fresh = Of(curriculum, LanguageMix.Both);
            string[] before = fresh.Stops
                .Select(stop => stop.Lesson.Id.ToString())
                .TakeWhile(id => id != nextLesson)
                .ToArray();
            return Of(curriculum, LanguageMix.Both, before);
        }

        /// <summary>The map of <paramref name="curriculum"/> in both languages with every lesson done; used by the map tests.</summary>
        public static MapRoute AllDone(Curriculum curriculum)
        {
            MapRoute fresh = Of(curriculum, LanguageMix.Both);
            return Of(curriculum, LanguageMix.Both, fresh.Stops.Select(stop => stop.Lesson.Id.ToString()).ToArray());
        }

        /// <summary>Progress with <paramref name="doneLessons"/> done and the rocket last shown at <paramref name="rocketStop"/> (null for never); used by the map tests.</summary>
        public static MapProgress Progress(string rocketStop, params string[] doneLessons)
        {
            return new MapProgress(doneLessons.Select(ContentId.Create),
                rocketStop == null ? default : ContentId.Create(rocketStop));
        }
    }
}
