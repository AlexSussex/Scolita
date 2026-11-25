using Scolita.Core.Content;

namespace Scolita.Core.Profiles
{
    /// <summary>
    /// A picture a child chooses for her profile (design screen 11): one row of <c>avatars.csv</c>, read with the
    /// rewards' files (<c>RewardTables</c>). Its picture is <c>Art/Avatars/avatar-&lt;id&gt;</c>, by name, so a redrawn
    /// picture needs no change here. A retired avatar is no longer offered, but a profile that has it keeps it.
    /// </summary>
    public sealed class ProfileAvatar : IContentItem
    {
        /// <summary>Creates an avatar; called by <c>RewardTables</c>, the avatar asset and tests.</summary>
        public ProfileAvatar(ContentId id, bool retired, ContentSource source)
        {
            Id = id;
            Retired = retired;
            Source = source;
        }

        /// <inheritdoc/>
        public ContentId Id { get; }

        /// <inheritdoc/>
        public bool Retired { get; }

        /// <inheritdoc/>
        public ContentSource Source { get; }
    }
}
