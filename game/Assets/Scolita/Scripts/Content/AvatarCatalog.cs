using System.Collections.Generic;
using System.Linq;
using Scolita.Core.Content;
using UnityEngine;

namespace Scolita.Content
{
    /// <summary>
    /// Every avatar asset, in the order of <c>avatars.csv</c> (<c>Content/AvatarCatalog.asset</c>): the pictures a child
    /// chooses for her profile, and each profile's picture by its id.
    /// </summary>
    public sealed class AvatarCatalog : ContentCatalog<AvatarAsset>
    {
        /// <summary>
        /// The ids of the avatars offered (not retired), in order; the first is the one the app gives its first profile.
        /// Called by the profile store and the tests.
        /// </summary>
        public IReadOnlyList<ContentId> OfferedIds()
        {
            return Items
                .Where(avatar => !avatar.Retired)
                .Select(avatar => ContentId.Create(avatar.Id))
                .ToArray();
        }

        /// <summary>
        /// The picture of the avatar <paramref name="id"/>; the first avatar's picture for an id the catalog doesn't have
        /// (a later version's), so a profile is always drawn. Called by the screens' composition roots and the tests.
        /// </summary>
        public Sprite SpriteOf(ContentId id)
        {
            AvatarAsset found = Items.FirstOrDefault(avatar => id.IsSet && avatar.Id == id.Value);
            if (found == null)
            {
                Debug.LogWarning("Scolita: there is no avatar " + id + "; the first one is drawn instead.");
                found = Items[0];
            }

            return found.Sprite;
        }

        /// <inheritdoc/>
        protected override string IdOf(AvatarAsset asset)
        {
            return asset.Id;
        }
    }
}
