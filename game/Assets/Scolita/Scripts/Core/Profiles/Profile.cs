using System;
using Scolita.Core.Content;
using Scolita.Core.Timing;

namespace Scolita.Core.Profiles
{
    /// <summary>
    /// One learner on the tablet (PRD §3.8): a stable id (its save folder, <c>profiles/&lt;id&gt;/</c>), the name the parent
    /// typed (or none), the avatar a child chose, its kind and the day it was made. Nothing else about the child is kept:
    /// no photo, no birthday, no age, no surname. What the tester adds is asked of the profile
    /// (<see cref="OpensEveryLesson"/>, <see cref="CanSkipSteps"/>, <see cref="ShowsTestInfo"/>), so a child's profile can
    /// never turn it on. Immutable: <see cref="ProfileList"/> makes the changed copies.
    /// </summary>
    public sealed class Profile
    {
        /// <summary>
        /// Creates a profile; throws an <see cref="ArgumentException"/> for an id or an avatar that isn't set. Called by
        /// <see cref="ProfileList"/>, the profile list's loading and the tests.
        /// </summary>
        public Profile(ProfileId id, ProfileName name, ContentId avatar, ProfileKind kind, CalendarDay created)
        {
            if (!id.IsSet)
            {
                throw new ArgumentException("A profile has an id.", nameof(id));
            }

            if (!avatar.IsSet)
            {
                throw new ArgumentException("A profile has an avatar.", nameof(avatar));
            }

            Id = id;
            Name = name;
            Avatar = avatar;
            Kind = kind;
            Created = created;
        }

        /// <summary>The stable id, never shown and never reused.</summary>
        public ProfileId Id { get; }

        /// <summary>The name the parent typed, or <see cref="ProfileName.None"/>.</summary>
        public ProfileName Name { get; }

        /// <summary>The avatar's id in the avatar catalog.</summary>
        public ContentId Avatar { get; }

        /// <summary>A child, or the tester.</summary>
        public ProfileKind Kind { get; }

        /// <summary>The day the profile was made.</summary>
        public CalendarDay Created { get; }

        /// <summary>Whether this is a child's profile.</summary>
        public bool IsChild => Kind == ProfileKind.Child;

        /// <summary>Whether every planet's every lesson opens on the map (the tester only).</summary>
        public bool OpensEveryLesson => Kind == ProfileKind.Tester;

        /// <summary>Whether a lesson shows a button that skips its step (the tester only).</summary>
        public bool CanSkipSteps => Kind == ProfileKind.Tester;

        /// <summary>Whether the screens show "TEST" and the test information: the version, the ids, the voice (the tester only).</summary>
        public bool ShowsTestInfo => Kind == ProfileKind.Tester;

        /// <summary>This profile with <paramref name="name"/> and <paramref name="avatar"/>; called by <see cref="ProfileList.Change"/>.</summary>
        public Profile With(ProfileName name, ContentId avatar)
        {
            return new Profile(Id, name, avatar, Kind, Created);
        }
    }
}
