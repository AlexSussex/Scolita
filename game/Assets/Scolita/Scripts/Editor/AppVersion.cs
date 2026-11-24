using System;
using System.Globalization;
using System.IO;
using System.Text.RegularExpressions;

namespace Scolita.Editor
{
    /// <summary>
    /// The app's version, <c>MAJOR.MINOR.PATCH</c> with each part 0 to 99 and no leading zeros,
    /// as written in the repository's <c>VERSION</c> file. Read by <see cref="BuildScript"/>.
    /// </summary>
    public readonly struct AppVersion
    {
        private const int PartLimit = 100;
        private const int LowestVersionCode = 1;

        private static readonly Regex Pattern = new Regex(
            @"^(0|[1-9][0-9]?)\.(0|[1-9][0-9]?)\.(0|[1-9][0-9]?)\z", RegexOptions.CultureInvariant);

        private AppVersion(int major, int minor, int patch)
        {
            Major = major;
            Minor = minor;
            Patch = patch;
        }

        /// <summary>The first part: 1 from the 1.0.0 release on.</summary>
        public int Major { get; }

        /// <summary>The second part, raised by each finished feature.</summary>
        public int Minor { get; }

        /// <summary>The third part, raised by each fix or chore.</summary>
        public int Patch { get; }

        /// <summary>
        /// The Android version code: <c>major * 10000 + minor * 100 + patch</c>, so 0.4.2 is 402;
        /// 1 for 0.0.0 (the version before the first release), as Android needs a positive code.
        /// So 0.0.0 and 0.0.1 share code 1; neither is ever released.
        /// </summary>
        public int VersionCode => Math.Max(LowestVersionCode, (Major * PartLimit + Minor) * PartLimit + Patch);

        /// <summary>
        /// Reads a version from the text of the <c>VERSION</c> file, ignoring the spaces and line
        /// ending around it. Throws a <see cref="FormatException"/> that says what is wrong for
        /// anything else. Called by <see cref="BuildScript"/> and by <c>AppVersionTests</c>.
        /// </summary>
        public static AppVersion Parse(string text)
        {
            return ParseText(text);
        }

        /// <summary>
        /// Reads the version from the <c>VERSION</c> file at <paramref name="path"/> (a byte order
        /// mark, spaces and the line ending around it are ignored), as <see cref="Parse"/> does.
        /// Throws a <see cref="FileNotFoundException"/> when the file is missing. Called by
        /// <see cref="BuildScript"/> and by <c>AppVersionTests</c>.
        /// </summary>
        public static AppVersion ReadFile(string path)
        {
            if (!File.Exists(path))
            {
                throw new FileNotFoundException("The VERSION file was not found at " + path + ".", path);
            }

            return ParseText(File.ReadAllText(path));
        }

        /// <summary>The version as <c>MAJOR.MINOR.PATCH</c>; the app's <c>bundleVersion</c>.</summary>
        public override string ToString()
        {
            return Major.ToString(CultureInfo.InvariantCulture) + "."
                + Minor.ToString(CultureInfo.InvariantCulture) + "."
                + Patch.ToString(CultureInfo.InvariantCulture);
        }

        private static AppVersion ParseText(string text)
        {
            string trimmed = text?.Trim() ?? string.Empty;
            Match match = Pattern.Match(trimmed);
            if (!match.Success)
            {
                throw new FormatException("VERSION must be MAJOR.MINOR.PATCH, each part 0 to 99 without "
                    + "leading zeros (for example 0.4.2), but it is '" + trimmed + "'.");
            }

            return new AppVersion(PartOf(match, 1), PartOf(match, 2), PartOf(match, 3));
        }

        private static int PartOf(Match match, int group)
        {
            return int.Parse(match.Groups[group].Value, NumberStyles.None, CultureInfo.InvariantCulture);
        }
    }
}
