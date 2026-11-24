using System.Collections.Generic;
using Scolita.Content;
using Scolita.Core.Content;
using UnityEngine;

namespace Scolita.Game.Lessons
{
    /// <summary>
    /// The pictures' sprites by picture id, from the picture catalog, for the activity views. A
    /// picture with no sprite falls back to a placeholder and logs a warning once: never an impossible
    /// state on a tablet (AGENTS.md → "Quality"; the validator makes it impossible in a release).
    /// </summary>
    public sealed class PictureSprites
    {
        private readonly Dictionary<ContentId, Sprite> sprites = new Dictionary<ContentId, Sprite>();
        private readonly HashSet<ContentId> warned = new HashSet<ContentId>();
        private readonly Sprite missing;

        /// <summary>
        /// Reads <paramref name="catalog"/>, with <paramref name="missingPicture"/> for a picture without
        /// a sprite; called by <see cref="LessonRunner"/> when a lesson is built.
        /// </summary>
        public PictureSprites(PictureCatalog catalog, Sprite missingPicture)
        {
            missing = missingPicture;
            foreach (PictureAsset picture in catalog.Items)
            {
                if (picture != null && ContentId.TryCreate(picture.Id, out ContentId id) && !sprites.ContainsKey(id))
                {
                    sprites.Add(id, picture.Sprite);
                }
            }
        }

        /// <summary>
        /// The sprite of <paramref name="picture"/>; the placeholder (with a warning, once) when it has
        /// none. Called by the activity views when a step begins.
        /// </summary>
        public Sprite Of(ContentId picture)
        {
            if (sprites.TryGetValue(picture, out Sprite sprite) && sprite != null)
            {
                return sprite;
            }

            if (warned.Add(picture))
            {
                Debug.LogWarning("Lesson: the picture " + picture + " has no sprite; a placeholder is shown.");
            }

            return missing;
        }
    }
}
