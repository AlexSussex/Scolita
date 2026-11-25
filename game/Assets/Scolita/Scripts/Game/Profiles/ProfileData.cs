using System;
using Scolita.Core.Profiles;
using UnityEngine;

namespace Scolita.Game.Profiles
{
    /// <summary>
    /// One profile as <c>profiles.json</c> keeps it (<see cref="ProfilesData"/>): its id, the name typed (empty for none),
    /// its avatar's id, its kind (<c>child</c> or <c>tester</c>) and its day (<c>2026-11-25</c>).
    /// </summary>
    [Serializable]
    public sealed class ProfileData
    {
        /// <summary>The kind's code of a child.</summary>
        public const string ChildKind = "child";

        /// <summary>The kind's code of the tester.</summary>
        public const string TesterKind = "tester";

        [SerializeField] private string id;
        [SerializeField] private string name;
        [SerializeField] private string avatar;
        [SerializeField] private string kind;
        [SerializeField] private string created;

        /// <summary>An empty entry, for Unity's serialization.</summary>
        public ProfileData()
        {
        }

        /// <summary>Keeps <paramref name="profile"/>; called by <see cref="ProfilesData"/>.</summary>
        public ProfileData(Profile profile)
        {
            id = profile.Id.Value;
            name = profile.Name.Text;
            avatar = profile.Avatar.Value;
            kind = profile.Kind == ProfileKind.Tester ? TesterKind : ChildKind;
            created = profile.Created.ToString();
        }

        /// <summary>The id as saved.</summary>
        public string Id => id;

        /// <summary>The name as saved.</summary>
        public string Name => name;

        /// <summary>The avatar's id as saved.</summary>
        public string Avatar => avatar;

        /// <summary>The kind's code as saved.</summary>
        public string Kind => kind;

        /// <summary>The day as saved.</summary>
        public string Created => created;
    }
}
