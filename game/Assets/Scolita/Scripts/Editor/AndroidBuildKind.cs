namespace Scolita.Editor
{
    /// <summary>The two APKs <see cref="BuildScript"/> makes.</summary>
    public enum AndroidBuildKind
    {
        /// <summary>A development build signed with the debug key, for agents and the emulator.</summary>
        Debug,

        /// <summary>A build signed with the release key, for the tablet.</summary>
        Release
    }
}
