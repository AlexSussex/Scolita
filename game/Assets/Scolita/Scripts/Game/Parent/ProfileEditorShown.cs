using System.Collections.Generic;
using Scolita.Core.Languages;
using Scolita.Core.Profiles;

namespace Scolita.Game.Parent
{
    /// <summary>
    /// What the profile's editor shows (design screen 10, the right card): a child being changed, or a new child; the
    /// name as typed, the avatars, the language mix, and whether it can be deleted.
    /// </summary>
    public sealed class ProfileEditorShown
    {
        /// <summary>Creates what the editor shows; called by <see cref="ParentRunner"/> and the tests.</summary>
        public ProfileEditorShown(bool isNew, bool isTester, string name, IReadOnlyList<AvatarChoice> avatars, LanguageMix mix,
            bool canDelete, ProfileNameProblem nameProblem, int nameMostCharacters)
        {
            NameMostCharacters = nameMostCharacters;
            IsNew = isNew;
            IsTester = isTester;
            Name = name ?? string.Empty;
            Avatars = avatars;
            Mix = mix;
            CanDelete = canDelete;
            NameProblem = nameProblem;
        }

        /// <summary>Whether it is a new child ("Un copil nou", saved with "Adaugă"), else a profile being changed.</summary>
        public bool IsNew { get; }

        /// <summary>Whether it is the tester's profile (no name, no language mix here: only its avatar).</summary>
        public bool IsTester { get; }

        /// <summary>The name as typed.</summary>
        public string Name { get; }

        /// <summary>The avatars offered, in the catalog's order.</summary>
        public IReadOnlyList<AvatarChoice> Avatars { get; }

        /// <summary>The language mix chosen.</summary>
        public LanguageMix Mix { get; }

        /// <summary>Whether "Șterge profilul…" is offered (a child, not the only one).</summary>
        public bool CanDelete { get; }

        /// <summary>The most characters in a name (the rules' number, shown beside the field).</summary>
        public int NameMostCharacters { get; }

        /// <summary>Why the name typed can't be saved, or none.</summary>
        public ProfileNameProblem NameProblem { get; }
    }
}
