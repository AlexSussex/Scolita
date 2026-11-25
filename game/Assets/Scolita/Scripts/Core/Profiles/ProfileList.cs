using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Scolita.Core.Content;
using Scolita.Core.Timing;

namespace Scolita.Core.Profiles
{
    /// <summary>
    /// The learners on the tablet (PRD §3.8; the one owner of these rules): the profiles in the order they were made,
    /// the child who played last (<see cref="LastChild"/>: ringed on "Who's playing?"), whose saves are open now
    /// (<see cref="Playing"/>: a child or the tester; nobody while "Who's playing?" waits), and the number the next
    /// child's id takes. A child's id is <c>child-&lt;n&gt;</c> from <see cref="NextChildNumber"/>, which only grows, so a
    /// deleted child's id (and its folder's name) is never reused; the tester's is <c>tester</c>. There is always at
    /// least one child; each child has an avatar no other child has, so a child who can't read finds her own.
    /// </summary>
    public sealed class ProfileList
    {
        private const string ChildIdPrefix = "child-";

        private readonly List<Profile> profiles;
        private readonly ProfileRules rules;

        /// <summary>
        /// The list of <paramref name="profiles"/> in their order. <paramref name="lastChild"/> and
        /// <paramref name="playing"/> that name no profile of the list (or, for the last child, the tester) read as
        /// nobody; <paramref name="nextChildNumber"/> is raised above every child's number. Throws an
        /// <see cref="ArgumentException"/> for an id used twice, an id that doesn't fit its kind (<see cref="IsIdOfKind"/>: so
        /// one tester at most), or no child (the loading leaves out the
        /// entries that can't be used before). Called by <see cref="FromFolders"/>, the profile list's loading and the
        /// tests.
        /// </summary>
        public ProfileList(IEnumerable<Profile> profiles, ProfileId lastChild, ProfileId playing, int nextChildNumber,
            ProfileRules rules)
        {
            if (profiles == null)
            {
                throw new ArgumentNullException(nameof(profiles));
            }

            this.rules = rules ?? throw new ArgumentNullException(nameof(rules));
            this.profiles = profiles.ToList();
            if (this.profiles.Select(profile => profile.Id).Distinct().Count() != this.profiles.Count)
            {
                throw new ArgumentException("A profile id is used once.", nameof(profiles));
            }

            if (this.profiles.Any(profile => !IdFits(profile.Id, profile.Kind)))
            {
                throw new ArgumentException("A child's id is child-<n> and the tester's is tester.", nameof(profiles));
            }

            if (!this.profiles.Any(profile => profile.IsChild))
            {
                throw new ArgumentException("There is always at least one child.", nameof(profiles));
            }

            LastChild = Find(lastChild)?.IsChild == true ? lastChild : default;
            Playing = Find(playing) != null ? playing : default;
            NextChildNumber = Math.Max(Math.Max(1, nextChildNumber), HighestChildNumber(this.profiles) + 1);
        }

        /// <summary>The id of the tester's profile.</summary>
        public static ProfileId TesterId { get; } = ProfileId.Create("tester");

        /// <summary>The id of the first child, <c>child-1</c>: the profile every release before profiles had.</summary>
        public static ProfileId FirstChildId { get; } = ChildId(1);

        /// <summary>Every profile, in the order they were made.</summary>
        public IReadOnlyList<Profile> All => profiles;

        /// <summary>The children's profiles, in the order they were made.</summary>
        public IReadOnlyList<Profile> Children => profiles.Where(profile => profile.IsChild).ToArray();

        /// <summary>The tester's profile, or null when there is none.</summary>
        public Profile Tester => profiles.FirstOrDefault(profile => profile.Kind == ProfileKind.Tester);

        /// <summary>The child who played last, or nobody (not set).</summary>
        public ProfileId LastChild { get; private set; }

        /// <summary>Whose saves are open now, or nobody (not set) while "Who's playing?" waits.</summary>
        public ProfileId Playing { get; private set; }

        /// <summary>The number of the next child's id (<c>child-&lt;n&gt;</c>); it only grows.</summary>
        public int NextChildNumber { get; private set; }

        /// <summary>Whether the app starts on "Who's playing?": there are two or more children.</summary>
        public bool StartsWithPicker => ChildCount >= 2;

        /// <summary>Whether another child can be added (fewer than <see cref="ProfileRules.MostChildren"/>).</summary>
        public bool CanAddChild => ChildCount < rules.MostChildren;

        /// <summary>
        /// Whose map, lessons and saves open: the one playing, else the last child, else the first child; never
        /// nobody, never a deleted profile. Called by the Map, Lesson and Parent scenes and the tests.
        /// </summary>
        public Profile ProfileToOpen
        {
            get
            {
                return Find(Playing) ?? Find(LastChild) ?? profiles.First(profile => profile.IsChild);
            }
        }

        /// <summary>
        /// The list made from the profile folders on the tablet, for a tablet without a list (every release before
        /// profiles: her <c>child-1</c> folder becomes the first child) or with a damaged one: the <c>child-&lt;n&gt;</c>
        /// folders by number (<c>child-1</c> alone when there are none), then the tester's; other folders are left as
        /// they are. Names none, avatars from <paramref name="avatars"/> in order (each child a different one, while
        /// there are enough), <paramref name="day"/> as their day; nobody playing, the first child as the last one.
        /// Called by the profile list's loading and the tests.
        /// </summary>
        public static ProfileList FromFolders(IEnumerable<ProfileId> folders, IReadOnlyList<ContentId> avatars,
            CalendarDay day, ProfileRules rules)
        {
            if (avatars == null || avatars.Count == 0)
            {
                throw new ArgumentException("There is at least one avatar to give.", nameof(avatars));
            }

            ProfileId[] found = (folders ?? Enumerable.Empty<ProfileId>()).Distinct().ToArray();
            List<ProfileId> children = found
                .Where(id => ChildNumberOf(id) > 0)
                .OrderBy(ChildNumberOf)
                .ToList();
            if (children.Count == 0)
            {
                children.Add(FirstChildId);
            }

            List<Profile> made = new List<Profile>();
            for (int index = 0; index < children.Count; index++)
            {
                made.Add(new Profile(children[index], ProfileName.None, avatars[index % avatars.Count], ProfileKind.Child, day));
            }

            if (found.Contains(TesterId))
            {
                made.Add(new Profile(TesterId, ProfileName.None, FirstFree(made, avatars), ProfileKind.Tester, day));
            }

            return new ProfileList(made, children[0], default, 0, rules);
        }

        /// <summary>
        /// Whether <paramref name="id"/> can be the id of a profile of <paramref name="kind"/>: <c>child-&lt;n&gt;</c> (n from 1,
        /// without leading zeros) for a child, <c>tester</c> for the tester. Called by the profile list's loading and the
        /// tests.
        /// </summary>
        public static bool IsIdOfKind(ProfileId id, ProfileKind kind)
        {
            return IdFits(id, kind);
        }

        /// <summary>Finds the profile <paramref name="id"/>; false when the list has none (or the id isn't set).</summary>
        public bool TryGet(ProfileId id, out Profile profile)
        {
            profile = Find(id);
            return profile != null;
        }

        /// <summary>
        /// Whether <paramref name="avatar"/> can be <paramref name="forProfile"/>'s: no other child has it (the tester may
        /// share one). Called by the parent area's avatar grid and the tests.
        /// </summary>
        public bool IsAvatarFree(ContentId avatar, ProfileId forProfile)
        {
            return AvatarFree(avatar, forProfile);
        }

        /// <summary>
        /// The first of <paramref name="offered"/> that no child has (for a new child), else the first offered. Called by
        /// the parent area and the tests.
        /// </summary>
        public ContentId FirstFreeAvatar(IReadOnlyList<ContentId> offered)
        {
            if (offered == null || offered.Count == 0)
            {
                throw new ArgumentException("There is at least one avatar to offer.", nameof(offered));
            }

            return FirstFree(profiles, offered);
        }

        /// <summary>
        /// Adds a child named <paramref name="name"/> (or none) with <paramref name="avatar"/>, made on
        /// <paramref name="day"/>, as <c>child-&lt;n&gt;</c>, and returns it. Throws an <see cref="InvalidOperationException"/>
        /// when the tablet has the most children already or another child has the avatar. Called by the parent area and
        /// the tests.
        /// </summary>
        public Profile AddChild(ProfileName name, ContentId avatar, CalendarDay day)
        {
            if (ChildCount >= rules.MostChildren)
            {
                throw new InvalidOperationException("The tablet has " + rules.MostChildren + " children's profiles already.");
            }

            RequireFree(avatar, default);
            Profile child = new Profile(ChildId(NextChildNumber), name, avatar, ProfileKind.Child, day);
            NextChildNumber++;
            profiles.Add(child);
            return child;
        }

        /// <summary>
        /// Adds the tester's profile with <paramref name="avatar"/>, made on <paramref name="day"/>, and returns it. Throws
        /// an <see cref="InvalidOperationException"/> when there is a tester already. Called by the parent area and the tests.
        /// </summary>
        public Profile AddTester(ContentId avatar, CalendarDay day)
        {
            if (profiles.Any(profile => profile.Kind == ProfileKind.Tester))
            {
                throw new InvalidOperationException("There is a tester's profile already.");
            }

            Profile tester = new Profile(TesterId, ProfileName.None, avatar, ProfileKind.Tester, day);
            profiles.Add(tester);
            return tester;
        }

        /// <summary>
        /// Gives the profile <paramref name="id"/> <paramref name="name"/> and <paramref name="avatar"/>, and returns it.
        /// Throws an <see cref="InvalidOperationException"/> for an unknown profile or an avatar another child has. Called
        /// by the parent area and the tests.
        /// </summary>
        public Profile Change(ProfileId id, ProfileName name, ContentId avatar)
        {
            int index = IndexOf(id);
            RequireFree(avatar, id);
            Profile changed = profiles[index].With(name, avatar);
            profiles[index] = changed;
            return changed;
        }

        /// <summary>
        /// Whether the profile <paramref name="id"/> can be deleted: a child, and not the only one (the tester is never
        /// deleted: its progress is reset instead). Called by the parent area and the tests.
        /// </summary>
        public bool CanDelete(ProfileId id)
        {
            return Deletable(id);
        }

        /// <summary>
        /// Takes the profile <paramref name="id"/> off the list (its folder is deleted by the Unity side once the list is
        /// saved); the last child becomes the first child when it was this one, and nobody plays when this one did.
        /// Throws an <see cref="InvalidOperationException"/> when it can't be deleted (<see cref="CanDelete"/>). Called by
        /// the parent area and the tests.
        /// </summary>
        public void Delete(ProfileId id)
        {
            if (!Deletable(id))
            {
                throw new InvalidOperationException("The profile " + id + " can't be deleted: only a child, never the last one.");
            }

            profiles.RemoveAt(IndexOf(id));
            if (LastChild == id)
            {
                LastChild = profiles.First(profile => profile.IsChild).Id;
            }

            if (Playing == id)
            {
                Playing = default;
            }
        }

        /// <summary>
        /// Opens the profile <paramref name="id"/>'s saves (a child also becomes the last child). Throws an
        /// <see cref="InvalidOperationException"/> for an unknown profile. Called by Boot, "Who's playing?", the parent
        /// area's "Joacă ca tester" and the tests.
        /// </summary>
        public void Play(ProfileId id)
        {
            Profile profile = profiles[IndexOf(id)];
            Playing = id;
            if (profile.IsChild)
            {
                LastChild = id;
            }
        }

        /// <summary>Nobody plays (while "Who's playing?" waits); called by Boot and the tests.</summary>
        public void StopPlaying()
        {
            Playing = default;
        }

        private static ProfileId ChildId(int number)
        {
            return ProfileId.Create(ChildIdPrefix + number.ToString(CultureInfo.InvariantCulture));
        }

        private static int ChildNumberOf(ProfileId id)
        {
            string text = id.ToString();
            if (!text.StartsWith(ChildIdPrefix, StringComparison.Ordinal))
            {
                return 0;
            }

            string digits = text.Substring(ChildIdPrefix.Length);
            bool canonical = digits.Length > 0 && digits.Length <= 6 && digits[0] != '0' && digits.All(char.IsDigit);
            return canonical ? int.Parse(digits, NumberStyles.None, CultureInfo.InvariantCulture) : 0;
        }

        private static bool IdFits(ProfileId id, ProfileKind kind)
        {
            return kind == ProfileKind.Tester ? id == TesterId : ChildNumberOf(id) > 0;
        }

        private static int HighestChildNumber(IEnumerable<Profile> profiles)
        {
            return profiles.Select(profile => ChildNumberOf(profile.Id)).DefaultIfEmpty(0).Max();
        }

        private static ContentId FirstFree(IEnumerable<Profile> profiles, IReadOnlyList<ContentId> offered)
        {
            HashSet<ContentId> taken = new HashSet<ContentId>(profiles.Where(profile => profile.IsChild).Select(profile => profile.Avatar));
            ContentId free = offered.FirstOrDefault(avatar => !taken.Contains(avatar));
            return free.IsSet ? free : offered[0];
        }

        private int ChildCount => profiles.Count(profile => profile.IsChild);

        private Profile Find(ProfileId id)
        {
            return id.IsSet ? profiles.FirstOrDefault(candidate => candidate.Id == id) : null;
        }

        private bool AvatarFree(ContentId avatar, ProfileId forProfile)
        {
            if (Find(forProfile)?.IsChild == false)
            {
                return true;
            }

            return !profiles.Any(other => other.IsChild && other.Id != forProfile && other.Avatar == avatar);
        }

        private bool Deletable(ProfileId id)
        {
            return Find(id)?.IsChild == true && ChildCount > 1;
        }

        private int IndexOf(ProfileId id)
        {
            int index = id.IsSet ? profiles.FindIndex(profile => profile.Id == id) : -1;
            if (index < 0)
            {
                throw new InvalidOperationException("There is no profile " + id + ".");
            }

            return index;
        }

        private void RequireFree(ContentId avatar, ProfileId forProfile)
        {
            if (!AvatarFree(avatar, forProfile))
            {
                throw new InvalidOperationException("Another child has the avatar " + avatar + ".");
            }
        }
    }
}
