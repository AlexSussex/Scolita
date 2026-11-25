using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace Scolita.Editor.Art
{
    /// <summary>
    /// The 9-slice borders of the stretchable pieces (buttons, cards, the board), read from
    /// <see cref="ArtPaths.BordersFile"/>, which <c>design/art/build.mjs</c> writes from the drawings'
    /// own table, so the borders are defined in one place. Used by <see cref="ArtTextureImporter"/>.
    /// </summary>
    public sealed class SpriteBorderTable
    {
        private readonly IReadOnlyDictionary<string, Vector4> bordersByName;

        private SpriteBorderTable(IReadOnlyDictionary<string, Vector4> bordersByName)
        {
            this.bordersByName = bordersByName;
        }

        /// <summary>
        /// Reads the table from disk (an empty table if the file isn't there yet); throws an
        /// <see cref="InvalidDataException"/> if the file can't be read as the table, or lists a
        /// path twice or a border with no path. Called from
        /// <see cref="ArtTextureImporter"/>.
        /// </summary>
        public static SpriteBorderTable Load(string path)
        {
            Dictionary<string, Vector4> borders = new Dictionary<string, Vector4>(StringComparer.Ordinal);
            if (!File.Exists(path))
            {
                return new SpriteBorderTable(borders);
            }

            BorderFile file = JsonUtility.FromJson<BorderFile>(File.ReadAllText(path));
            if (file == null || file.Borders == null)
            {
                throw new InvalidDataException(path + " holds no \"borders\" list.");
            }

            foreach (BorderEntry entry in file.Borders)
            {
                if (string.IsNullOrEmpty(entry.Path))
                {
                    throw new InvalidDataException(path + " has a border with no \"path\".");
                }

                if (!borders.TryAdd(entry.Path, entry.Border))
                {
                    throw new InvalidDataException(path + " lists " + entry.Path + " twice.");
                }
            }

            return new SpriteBorderTable(borders);
        }

        /// <summary>
        /// The border (left, bottom, right, top, in pixels) for an art name such as "UI/button-yellow",
        /// or zero for a sprite that doesn't stretch. Called from <see cref="ArtTextureImporter"/>.
        /// </summary>
        public Vector4 BorderFor(string artName)
        {
            return bordersByName.TryGetValue(artName, out Vector4 border) ? border : Vector4.zero;
        }

        /// <summary>The art names that have a border. Called from the art tests.</summary>
        public IEnumerable<string> Names => bordersByName.Keys;

        [Serializable]
        private sealed class BorderFile
        {
            [SerializeField] private BorderEntry[] borders = Array.Empty<BorderEntry>();

            public BorderEntry[] Borders => borders;
        }

        [Serializable]
        private sealed class BorderEntry
        {
            [SerializeField] private string path = string.Empty;
            [SerializeField] private int left = 0;
            [SerializeField] private int bottom = 0;
            [SerializeField] private int right = 0;
            [SerializeField] private int top = 0;

            public string Path => path;

            public Vector4 Border => new Vector4(left, bottom, right, top);
        }
    }
}
