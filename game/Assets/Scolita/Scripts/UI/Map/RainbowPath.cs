using UnityEngine;
using UnityEngine.Sprites;
using UnityEngine.UI;

namespace Scolita.UI.Map
{
    /// <summary>
    /// The galaxy map's rainbow path (design/README.md → "A rainbow path"): one strip along a centre line,
    /// textured across its width with <c>look</c>'s six-band sprite (pink on top, purple at the bottom), so it
    /// is drawn from the Map atlas with the planets. Its mesh is made when the line is set, never per frame.
    /// </summary>
    public sealed class RainbowPath : MaskableGraphic
    {
        [SerializeField] private Sprite bands;
        [SerializeField] private float width = 60f;

        private Vector2[] line = new Vector2[0];

        /// <inheritdoc/>
        public override Texture mainTexture => bands != null ? bands.texture : s_WhiteTexture;

        /// <summary>Draws the path along <paramref name="centreLine"/> (left to right); called by <see cref="MapScreen.Show"/>.</summary>
        public void SetLine(Vector2[] centreLine)
        {
            line = centreLine;
            SetVerticesDirty();
        }

        /// <summary>Builds the strip's mesh; called by Unity's UI when the line changes.</summary>
        protected override void OnPopulateMesh(VertexHelper mesh)
        {
            mesh.Clear();
            if (line.Length < 2)
            {
                return;
            }

            Vector4 uv = bands != null ? DataUtility.GetOuterUV(bands) : new Vector4(0f, 0f, 1f, 1f);
            float middleU = (uv.x + uv.z) * 0.5f;
            Color32 tint = color;
            float half = width * 0.5f;
            for (int index = 0; index < line.Length; index++)
            {
                Vector2 before = line[Mathf.Max(0, index - 1)];
                Vector2 after = line[Mathf.Min(line.Length - 1, index + 1)];
                Vector2 along = (after - before).normalized;
                Vector2 up = new Vector2(-along.y, along.x);
                mesh.AddVert(line[index] + up * half, tint, new Vector4(middleU, uv.w));
                mesh.AddVert(line[index] - up * half, tint, new Vector4(middleU, uv.y));
                if (index > 0)
                {
                    int top = index * 2;
                    mesh.AddTriangle(top - 2, top, top - 1);
                    mesh.AddTriangle(top - 1, top, top + 1);
                }
            }
        }
    }
}
