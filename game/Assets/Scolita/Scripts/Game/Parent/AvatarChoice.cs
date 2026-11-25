using Scolita.Core.Content;
using UnityEngine;

namespace Scolita.Game.Parent
{
    /// <summary>An avatar offered in the profile's editor: its picture, whether it can be chosen, whether it is chosen.</summary>
    public readonly struct AvatarChoice
    {
        /// <summary>Creates a choice; called by <see cref="ParentRunner"/> and the tests.</summary>
        public AvatarChoice(ContentId id, Sprite picture, bool isFree, bool isChosen)
        {
            Id = id;
            Picture = picture;
            IsFree = isFree;
            IsChosen = isChosen;
        }

        /// <summary>The avatar's id.</summary>
        public ContentId Id { get; }

        /// <summary>Its picture.</summary>
        public Sprite Picture { get; }

        /// <summary>Whether it can be chosen (no other child has it); dimmed otherwise.</summary>
        public bool IsFree { get; }

        /// <summary>Whether it is the one chosen now (outlined).</summary>
        public bool IsChosen { get; }
    }
}
