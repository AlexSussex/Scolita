using UnityEngine;
using UnityEngine.UI;

namespace Scolita.UI.Lesson
{
    /// <summary>
    /// A round shape drawn as a mesh (crisp at any size, no sprite needed): a disc, or a ring when
    /// <c>ringShare</c> is above 0, filling its rect (an oval in a rect that isn't square), lit from the top left
    /// as every shape of the look (design/README.md). The word activities' round parts: <c>Syllables</c>' beats and
    /// drum head, <c>BuildWord</c>'s windows and the rocket's nose and flame. Its mesh is made again only when its
    /// size or colour changes.
    /// </summary>
    public sealed class DiscGraphic : MaskableGraphic
    {
        private const int Segments = 48;

        [SerializeField, Range(0f, 0.95f)] private float ringShare;
        [SerializeField, Range(0f, 1f)] private float lightShare = 0.25f;

        /// <summary>Builds the disc's mesh; called by Unity's UI when the disc changes.</summary>
        protected override void OnPopulateMesh(VertexHelper mesh)
        {
            mesh.Clear();
            Rect rect = GetPixelAdjustedRect();
            Vector2 centre = rect.center;
            Vector2 outer = new Vector2(rect.width * 0.5f, rect.height * 0.5f);
            Vector2 inner = outer * ringShare;
            for (int step = 0; step < Segments; step++)
            {
                float angle = step * 2f * Mathf.PI / Segments;
                Vector2 direction = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle));
                float towardsLight = Vector2.Dot(direction, new Vector2(-0.7071f, 0.7071f));
                Color32 shade = towardsLight > 0f
                    ? Color.Lerp(color, Color.white, lightShare * towardsLight)
                    : Color.Lerp(color, Color.black, lightShare * 0.4f * -towardsLight);
                mesh.AddVert(new Vector3(centre.x + direction.x * outer.x, centre.y + direction.y * outer.y), shade, Vector4.zero);
                mesh.AddVert(new Vector3(centre.x + direction.x * inner.x, centre.y + direction.y * inner.y),
                    ringShare > 0f ? shade : Color.Lerp(color, Color.white, lightShare * 0.5f), Vector4.zero);
            }

            for (int step = 0; step < Segments; step++)
            {
                int outerHere = step * 2;
                int innerHere = outerHere + 1;
                int outerNext = (step + 1) % Segments * 2;
                int innerNext = outerNext + 1;
                mesh.AddTriangle(outerHere, outerNext, innerHere);
                mesh.AddTriangle(innerHere, outerNext, innerNext);
            }
        }
    }
}
