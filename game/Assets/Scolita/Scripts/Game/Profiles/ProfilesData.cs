using System;
using System.Collections.Generic;
using System.Linq;
using Scolita.Core.Content;
using Scolita.Core.Profiles;
using Scolita.Core.Timing;
using Scolita.Game.Saving;
using UnityEngine;

namespace Scolita.Game.Profiles
{
    /// <summary>
    /// The profile list, saved as <c>profiles.json</c> in the tablet's own folder (beside <c>settings.json</c>, through
    /// <see cref="SaveStore.ForTablet(string)"/>), version 1: the profiles in their order (<see cref="ProfileData"/>), the
    /// child who played last, whose saves are open now, and the next child's number. Everything else about a profile
    /// is in its own folder. Read and written by <see cref="ProfileStore"/>.
    /// </summary>
    [Serializable]
    public sealed class ProfilesData : ISaveData<ProfilesData>
    {
        /// <summary>The save file's name, without its extension.</summary>
        public const string FileName = "profiles";

        private const int LatestVersion = 1;

        [SerializeField] private int version;
        [SerializeField] private ProfileData[] profiles;
        [SerializeField] private string lastChild;
        [SerializeField] private string playing;
        [SerializeField] private int nextChild;

        /// <summary>Keeps <paramref name="list"/> in the current shape; called by <see cref="ProfileStore"/> and the tests.</summary>
        public ProfilesData(ProfileList list)
        {
            if (list == null)
            {
                throw new ArgumentNullException(nameof(list));
            }

            version = LatestVersion;
            profiles = list.All.Select(profile => new ProfileData(profile)).ToArray();
            lastChild = list.LastChild.ToString();
            playing = list.Playing.ToString();
            nextChild = list.NextChildNumber;
        }

        private ProfilesData()
        {
            version = LatestVersion;
            profiles = new ProfileData[0];
            lastChild = string.Empty;
            playing = string.Empty;
            nextChild = 1;
        }

        /// <inheritdoc/>
        public int Version => version;

        /// <inheritdoc/>
        public int CurrentVersion => LatestVersion;

        /// <summary>The profiles as saved, in their order.</summary>
        public IReadOnlyList<ProfileData> Profiles => profiles ?? new ProfileData[0];

        /// <summary>An empty list (no profile yet), the defaults of a tablet without the file; called by <see cref="ProfileStore"/> and the tests.</summary>
        public static ProfilesData CreateDefaults()
        {
            return new ProfilesData();
        }

        /// <summary>
        /// Core's list from this data, or null when no child's entry can be read (the caller makes the list from the
        /// profile folders then); <paramref name="leftOut"/> is how many entries were left out (the caller keeps a dated
        /// copy of the file before saving over it). An entry whose id isn't a profile id of its kind, an id already read or
        /// an unknown kind is left out (with a warning); a name that can't be a <see cref="ProfileName"/> (read without
        /// today's length limit) reads as none; an
        /// avatar id that can't be an id reads as the first of <paramref name="avatars"/> (an id the catalog doesn't know
        /// is kept); a day that can't be read is <paramref name="today"/>. Called by <see cref="ProfileStore"/> and the tests.
        /// </summary>
        public ProfileList ToList(ProfileRules rules, IReadOnlyList<ContentId> avatars, CalendarDay today, out int leftOut)
        {
            leftOut = 0;
            if (avatars == null || avatars.Count == 0)
            {
                throw new ArgumentException("There is at least one avatar.", nameof(avatars));
            }

            List<Profile> read = new List<Profile>();
            foreach (ProfileData entry in Profiles.Where(entry => entry != null))
            {
                Profile profile = Read(entry, avatars[0], today);
                if (profile == null || read.Any(other => other.Id == profile.Id)
                    || (profile.Kind == ProfileKind.Tester && read.Any(other => other.Kind == ProfileKind.Tester)))
                {
                    Debug.LogWarning("Scolita: a profile in " + FileName + ".json can't be used and is left out (id '"
                        + entry.Id + "', kind '" + entry.Kind + "').");
                    leftOut++;
                    continue;
                }

                read.Add(profile);
            }

            if (!read.Any(profile => profile.IsChild))
            {
                return null;
            }

            ProfileId.TryCreate(lastChild, out ProfileId last);
            ProfileId.TryCreate(playing, out ProfileId opened);
            return new ProfileList(read, last, opened, nextChild, rules);
        }

        /// <summary>
        /// Returns this data in the current shape (a missing list read as empty). A later version adds its migration step
        /// here, by <see cref="Version"/>. Called by <see cref="SaveStore.Load{T}"/>.
        /// </summary>
        public ProfilesData Upgraded()
        {
            profiles = profiles ?? new ProfileData[0];
            return this;
        }

        private static Profile Read(ProfileData entry, ContentId firstAvatar, CalendarDay today)
        {
            if (!ProfileId.TryCreate(entry.Id, out ProfileId id) || !TryKind(entry.Kind, out ProfileKind kind)
                || !ProfileList.IsIdOfKind(id, kind))
            {
                return null;
            }

            if (ProfileName.ReadSaved(entry.Name, out ProfileName name) != ProfileNameProblem.None)
            {
                Debug.LogWarning("Scolita: the name of the profile " + id + " can't be read; it has none now.");
            }

            ContentId avatar = ContentId.TryCreate(entry.Avatar, out ContentId saved) ? saved : firstAvatar;
            CalendarDay created = CalendarDay.TryParse(entry.Created, out CalendarDay day) ? day : today;
            return new Profile(id, name, avatar, kind, created);
        }

        private static bool TryKind(string code, out ProfileKind kind)
        {
            kind = code == ProfileData.TesterKind ? ProfileKind.Tester : ProfileKind.Child;
            return code == ProfileData.ChildKind || code == ProfileData.TesterKind;
        }
    }
}
