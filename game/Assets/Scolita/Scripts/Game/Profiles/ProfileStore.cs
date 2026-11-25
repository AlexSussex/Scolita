using System;
using System.Collections.Generic;
using System.Linq;
using Scolita.Content;
using Scolita.Core.Content;
using Scolita.Core.Profiles;
using Scolita.Core.Timing;
using Scolita.Game.Lessons;
using Scolita.Game.Saving;
using Scolita.Game.Timing;
using UnityEngine;

namespace Scolita.Game.Profiles
{
    /// <summary>
    /// Reads and writes the profile list (<c>profiles.json</c>, <see cref="ProfilesData"/>) in the tablet's folder, opens a
    /// profile's own folder (<see cref="FilesOf"/>), and deletes a deleted profile's folder. A tablet without the list
    /// (every release before profiles) or with a damaged one gets the list made from its profile folders
    /// (<see cref="ProfileList.FromFolders"/>: her <c>child-1</c> stays the first child, nothing lost), saved at once.
    /// Made by each scene's composition root and by the tests.
    /// </summary>
    public sealed class ProfileStore
    {
        private readonly string saveRoot;
        private readonly SaveStore tabletFiles;
        private readonly IReadOnlyList<ContentId> avatars;
        private readonly ProfileRules rules;
        private readonly IClock clock;

        /// <summary>
        /// A store for the profiles under <paramref name="saveRoot"/>, offering <paramref name="avatars"/> (in order) to the
        /// profiles it makes, with <paramref name="rules"/>' numbers and <paramref name="clock"/>'s days. Called by
        /// <see cref="ForApp"/> and the tests.
        /// </summary>
        public ProfileStore(string saveRoot, SaveStore tabletFiles, IReadOnlyList<ContentId> avatars, ProfileRules rules,
            IClock clock)
        {
            if (avatars == null || avatars.Count == 0)
            {
                throw new ArgumentException("There is at least one avatar.", nameof(avatars));
            }

            this.saveRoot = saveRoot;
            this.tabletFiles = tabletFiles ?? throw new ArgumentNullException(nameof(tabletFiles));
            this.avatars = avatars.ToArray();
            this.rules = rules ?? throw new ArgumentNullException(nameof(rules));
            this.clock = clock ?? throw new ArgumentNullException(nameof(clock));
        }

        /// <summary>The avatars offered, in order (the catalog's, without the retired ones).</summary>
        public IReadOnlyList<ContentId> Avatars => avatars;

        /// <summary>The profiles' numbers.</summary>
        public ProfileRules Rules => rules;

        /// <summary>
        /// The app's store, in <see cref="Application.persistentDataPath"/>, offering <paramref name="avatarCatalog"/>'s
        /// avatars that aren't retired, with <paramref name="learningRules"/>' numbers. Called by the scenes' composition
        /// roots.
        /// </summary>
        public static ProfileStore ForApp(AvatarCatalog avatarCatalog, LearningRulesAsset learningRules)
        {
            string root = Application.persistentDataPath;
            return new ProfileStore(root, SaveStore.ForTablet(root), avatarCatalog.OfferedIds(), learningRules.ToProfileRules(),
                new SystemClock());
        }

        /// <summary>
        /// The profile list: as saved, or (none saved, a damaged file, or no child in it) made from the profile folders and
        /// saved. A list that can't be read now (kept as it is by the store) is made from the folders without saving over
        /// it. Called by the scenes' composition roots and the tests.
        /// </summary>
        public ProfileList Load()
        {
            SaveLoadResult<ProfilesData> read = tabletFiles.Load(ProfilesData.FileName, ProfilesData.CreateDefaults());
            ProfileList list = read.Outcome == SaveLoadOutcome.Loaded ? read.Data.ToList(rules, avatars, clock.Today) : null;
            if (list != null)
            {
                return list;
            }

            list = ProfileList.FromFolders(SaveStore.ProfilesIn(saveRoot), avatars, clock.Today, rules);
            if (read.Outcome != SaveLoadOutcome.Unreadable)
            {
                Debug.Log("Scolita: the profile list was made from the profile folders (" + string.Join(", ",
                    list.All.Select(profile => profile.Id.Value)) + ").");
                tabletFiles.Save(ProfilesData.FileName, new ProfilesData(list));
            }

            return list;
        }

        /// <summary>Saves <paramref name="list"/>; false (logged by the store) when it could not be. Called by the scenes and the tests.</summary>
        public bool Save(ProfileList list)
        {
            return tabletFiles.Save(ProfilesData.FileName, new ProfilesData(list));
        }

        /// <summary>The store of <paramref name="profile"/>'s own folder, <c>profiles/&lt;id&gt;/</c>; called by the scenes and the tests.</summary>
        public SaveStore FilesOf(Profile profile)
        {
            return SaveStore.ForProfile(saveRoot, profile.Id);
        }

        /// <summary>
        /// Deletes the folder of the profile <paramref name="id"/>, which <paramref name="saved"/> (the list just saved
        /// without it) must no longer have: a profile still listed is never deleted. True when the folder is gone.
        /// Called by the parent area after a profile's deletion is saved, and the tests.
        /// </summary>
        public bool DeleteFolder(ProfileList saved, ProfileId id)
        {
            if (saved == null || saved.TryGet(id, out Profile _))
            {
                Debug.LogError("Scolita: the folder of the profile " + id + " was not deleted: the profile is still listed.");
                return false;
            }

            return SaveStore.DeleteProfileFolder(saveRoot, id);
        }
    }
}
