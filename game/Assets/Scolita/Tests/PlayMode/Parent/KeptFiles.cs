namespace Scolita.Tests.PlayMode.Parent
{
    /// <summary>The Editor's own files a parent test keeps: the profile's (as <c>MapScene.KeepFiles</c>) and the tablet's settings.</summary>
    public sealed class KeptFiles
    {
        /// <summary>Keeps <paramref name="profile"/> and <paramref name="tablet"/>; called by <see cref="ParentScene.Keep"/>.</summary>
        public KeptFiles(string[] profile, string tablet)
        {
            Profile = profile;
            Tablet = tablet;
        }

        /// <summary>The profile's files, as <c>MapScene.KeepFiles</c> kept them.</summary>
        public string[] Profile { get; }

        /// <summary>The tablet's settings file, or null when there was none.</summary>
        public string Tablet { get; }
    }
}
