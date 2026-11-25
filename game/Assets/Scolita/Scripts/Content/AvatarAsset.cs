using System.Collections.Generic;
using Scolita.Core.Profiles;
using UnityEngine;

namespace Scolita.Content
{
    /// <summary>
    /// A <see cref="ProfileAvatar"/> as an asset (<c>Content/Avatars/&lt;id&gt;.asset</c>) with its picture,
    /// <c>Art/Avatars/avatar-&lt;id&gt;</c>; written by the content importer and never edited by hand.
    /// </summary>
    public sealed class AvatarAsset : ScriptableObject, IValidatableContent
    {
        [SerializeField] private string id;
        [SerializeField] private bool retired;
        [SerializeField] private Sprite sprite;

        /// <summary>The avatar's id.</summary>
        public string Id => id;

        /// <summary>Whether it is no longer offered (a profile that has it keeps it).</summary>
        public bool Retired => retired;

        /// <summary>Its picture.</summary>
        public Sprite Sprite => sprite;

        /// <summary>Writes <paramref name="avatar"/> and its <paramref name="picture"/> into this asset; called by the content importer.</summary>
        public void Fill(ProfileAvatar avatar, Sprite picture)
        {
            id = avatar.Id.Value;
            retired = avatar.Retired;
            sprite = picture;
        }

        /// <inheritdoc/>
        public IReadOnlyList<string> Validate()
        {
            AssetFields fields = new AssetFields(name);
            fields.OwnId(id);
            if (sprite == null)
            {
                fields.Problem("It has no picture (run Școlița → Import content).");
            }

            return fields.Problems;
        }
    }
}
