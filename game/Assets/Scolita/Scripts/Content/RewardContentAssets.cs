using System;
using System.Linq;
using Scolita.Core.Content;
using Scolita.Core.Rewards;

namespace Scolita.Content
{
    /// <summary>
    /// Builds Core's <see cref="RewardContent"/> from the three reward catalogs at run time. The content was
    /// validated when it was imported, so a problem here is an impossible state: it throws.
    /// </summary>
    public static class RewardContentAssets
    {
        /// <summary>
        /// The rewards of the catalogs; throws an <see cref="InvalidOperationException"/> naming every problem. Called
        /// by the scenes' composition roots (<c>LessonRunner</c>, <c>MapRunner</c>) and the tests.
        /// </summary>
        public static RewardContent Build(StickerCatalog stickers, ConstellationCatalog constellations, TrailCatalog trails)
        {
            ContentProblems problems = new ContentProblems();
            RewardContent content = RewardContent.Build(
                stickers.Items.Select(sticker => sticker.ToSticker()).ToArray(),
                constellations.Items.Select(constellation => constellation.ToConstellation()).ToArray(),
                trails.Items.Select(trail => trail.ToTrail()).ToArray(),
                problems);
            if (problems.HasErrors)
            {
                throw new InvalidOperationException("The reward assets don't make the rewards (run "
                    + "Școlița → Import content): "
                    + string.Join(" ", problems.All.Select(problem => problem.ToString())));
            }

            return content;
        }
    }
}
