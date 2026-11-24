using UnityEngine;
using UnityEngine.UI;

namespace Scolita.UI.Lesson
{
    /// <summary>
    /// A five-pointed star drawn as a mesh (crisp at any size, no sprite needed), lit from the top left
    /// as every shape of the look (design/README.md): its centre and its upper left points lighter, its
    /// lower right points a little darker. <c>LetterHunt</c>'s stars, their outline and their glow. Its
    /// mesh is made again only when its size or colour changes, never while it moves.
    /// </summary>
    public sealed class StarGraphic : MaskableGraphic
    {
        private const int Points = 5;

        [SerializeField, Range(0.2f, 0.8f)] private float innerShare = 0.5f;
        [SerializeField, Range(0f, 1f)] private float lightShare = 0.35f;

        /// <summary>Builds the star's mesh; called by Unity's UI when the star changes.</summary>
        protected override void OnPopulateMesh(VertexHelper mesh)
        {
            mesh.Clear();
            Rect rect = GetPixelAdjustedRect();
            Vector2 centre = rect.center;
            float outer = Mathf.Min(rect.width, rect.height) * 0.5f;
            float inner = outer * innerShare;
            Color32 lit = Color.Lerp(color, Color.white, lightShare);
            mesh.AddVert(new Vector3(centre.x, centre.y), lit, Vector4.zero);
            for (int corner = 0; corner < Points * 2; corner++)
            {
                float angle = Mathf.PI * 0.5f + corner * Mathf.PI / Points;
                float radius = corner % 2 == 0 ? outer : inner;
                Vector2 direction = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle));
                float towardsLight = Vector2.Dot(direction, new Vector2(-0.7071f, 0.7071f));
                Color32 shade = towardsLight > 0f
                    ? Color.Lerp(color, Color.white, lightShare * towardsLight)
                    : Color.Lerp(color, Color.black, lightShare * 0.4f * -towardsLight);
                mesh.AddVert(new Vector3(centre.x + direction.x * radius, centre.y + direction.y * radius), shade, Vector4.zero);
            }

            for (int corner = 0; corner < Points * 2; corner++)
            {
                mesh.AddTriangle(0, 1 + corner, 1 + (corner + 1) % (Points * 2));
            }
        }
    }
}
