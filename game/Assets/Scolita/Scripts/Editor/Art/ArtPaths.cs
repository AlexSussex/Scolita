using System;
using System.Collections.Generic;

namespace Scolita.Editor.Art
{
    /// <summary>
    /// Where the app's art lives: the PNGs drawn by <c>design/art/build.mjs</c> under
    /// <see cref="Root"/>, one folder per sprite atlas (<see cref="AtlasFolders"/>), and the launcher
    /// icons in <see cref="IconFolder"/> (plain textures, in no atlas). Used by
    /// <see cref="ArtTextureImporter"/>, <see cref="ArtAtlases"/> and the art tests.
    /// </summary>
    public static class ArtPaths
    {
        /// <summary>The art's root folder, with its trailing slash.</summary>
        public const string Root = "Assets/Scolita/Art/";

        /// <summary>The launcher icons' folder: textures for Player Settings, not sprites.</summary>
        public const string IconFolder = Root + "Icon/";

        /// <summary>The folder holding the sprite atlases.</summary>
        public const string AtlasFolder = Root + "Atlases";

        /// <summary>The 9-slice borders written by <c>design/art/build.mjs</c>.</summary>
        public const string BordersFile = Root + "sprite-borders.json";

        /// <summary>The sprite folders, each packed into its own atlas of the same name.</summary>
        public static readonly IReadOnlyList<string> AtlasFolders = new[]
        {
            "Sky", "Map", "Effects", "Guides", "Badges", "Board", "UI", "Stickers", "Constellations", "Avatars"
        };

        /// <summary>
        /// True for a PNG under <see cref="Root"/>. Called from <see cref="ArtTextureImporter"/>
        /// and the art tests.
        /// </summary>
        public static bool IsArt(string assetPath)
        {
            return assetPath.StartsWith(Root, StringComparison.Ordinal)
                && assetPath.EndsWith(".png", StringComparison.OrdinalIgnoreCase);
        }

        /// <summary>True for a launcher icon texture. Called from <see cref="ArtTextureImporter"/> and the art tests.</summary>
        public static bool IsIcon(string assetPath)
        {
            return assetPath.StartsWith(IconFolder, StringComparison.Ordinal);
        }

        /// <summary>
        /// The art path as <c>design/art</c> names it: "UI/button-yellow" for
        /// "Assets/Scolita/Art/UI/button-yellow.png". Called from <see cref="ArtTextureImporter"/>.
        /// </summary>
        public static string ArtName(string assetPath)
        {
            string relative = assetPath.Substring(Root.Length);
            return relative.Substring(0, relative.Length - ".png".Length);
        }

        /// <summary>The atlas asset's path for a sprite folder. Called from <see cref="ArtAtlases"/> and the art tests.</summary>
        public static string AtlasPath(string folder)
        {
            return AtlasFolder + "/" + folder + ".spriteatlasv2";
        }
    }
}
