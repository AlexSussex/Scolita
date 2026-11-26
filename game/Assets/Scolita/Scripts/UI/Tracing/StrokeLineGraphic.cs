using UnityEngine;
using UnityEngine.Sprites;
using UnityEngine.UI;

namespace Scolita.UI.Tracing
{
    /// <summary>
    /// Lines drawn as one mesh on the writing board (design/README.md: the dotted guide, the faint shadow,
    /// her 26 px rainbow trail, the yellow arrow): a strip along each line with round ends, broken with a
    /// round corner where the line turns back on itself (m's stem), or round dots along it. Its points are
    /// kept in buffers made once, so adding a point and rebuilding the mesh allocate nothing; the mesh is
    /// rebuilt only when its points change.
    /// </summary>
    public sealed class StrokeLineGraphic : MaskableGraphic
    {
        private const int CapCorners = 10;
        private const float TurnsBack = -0.2f;

        [SerializeField] private Sprite brush;
        [SerializeField] private StrokeLineLook look = StrokeLineLook.Solid;
        [SerializeField, Min(1f)] private float width = 26f;
        [SerializeField, Min(1f)] private float dotSpacing = 24f;
        [SerializeField, Min(1f)] private float rainbowBand = 70f;
        [SerializeField, Min(16)] private int capacity = 2048;
        [SerializeField, Min(1)] private int mostLines = 8;
        [SerializeField] private Color[] rainbow =
        {
            new Color(0.608f, 0.424f, 1f), new Color(0.231f, 0.655f, 0.941f), new Color(0.298f, 0.851f, 0.482f),
            new Color(1f, 0.851f, 0.239f), new Color(1f, 0.624f, 0.263f), new Color(1f, 0.361f, 0.541f)
        };

        private Vector2[] points;
        private int[] lineEnds;
        private int count;
        private int lines;
        private bool full;

        /// <summary>How many points it holds.</summary>
        public int Count => count;

        /// <summary>
        /// Point <paramref name="index"/> it holds, in its local space; read by the PlayMode tests (a handwritten letter
        /// drawn exactly from its stroke data).
        /// </summary>
        public Vector2 PointAt(int index)
        {
            return points[index];
        }

        /// <summary>The line width, in the board's pixels.</summary>
        public float Width => width;

        /// <summary>How many separate lines it holds; read by the PlayMode tests (strokes never joined to each other).</summary>
        public int LineCount => lines;

        /// <inheritdoc/>
        public override Texture mainTexture => brush != null ? brush.texture : base.mainTexture;

        /// <summary>Forgets every line; called by the tracing view.</summary>
        public void Clear()
        {
            EnsureBuffers();
            count = 0;
            lines = 0;
            full = false;
            SetVerticesDirty();
        }

        /// <summary>
        /// Sets the line width to <paramref name="pixels"/> (at least 1), in the board's pixels; called by the tracing view
        /// to draw the guide as wide as what is accepted.
        /// </summary>
        public void SetWidth(float pixels)
        {
            float wanted = Mathf.Max(1f, pixels);
            if (!Mathf.Approximately(wanted, width))
            {
                width = wanted;
                SetVerticesDirty();
            }
        }

        /// <summary>
        /// Starts a new line (the next points no longer join the last one, so two strokes are never drawn joined); a line
        /// begun while the last one is still empty is that one. Once it holds as many lines as it can, the next lines'
        /// points are dropped until it is cleared. Called by the tracing view.
        /// </summary>
        public void BeginLine()
        {
            EnsureBuffers();
            int lastStart = lines > 1 ? lineEnds[lines - 2] : 0;
            if (lines > 0 && lineEnds[lines - 1] == lastStart)
            {
                return;
            }

            if (lines == lineEnds.Length)
            {
                full = true;
                return;
            }

            lines++;
            lineEnds[lines - 1] = count;
        }

        /// <summary>
        /// Adds <paramref name="point"/> (in the graphic's local pixels) to the line begun last; dropped once it is
        /// full. Called by the tracing view.
        /// </summary>
        public void Add(Vector2 point)
        {
            EnsureBuffers();
            if (lines == 0 || full || count == points.Length)
            {
                return;
            }

            points[count] = point;
            count++;
            lineEnds[lines - 1] = count;
            SetVerticesDirty();
        }

        /// <summary>Builds the mesh of its lines; called by Unity's UI when its points change.</summary>
        protected override void OnPopulateMesh(VertexHelper mesh)
        {
            mesh.Clear();
            if (points == null)
            {
                return;
            }

            Vector4 uv = brush != null ? DataUtility.GetOuterUV(brush) : new Vector4(0f, 0f, 1f, 1f);
            int start = 0;
            for (int line = 0; line < lines; line++)
            {
                int end = lineEnds[line];
                if (end > start)
                {
                    if (look == StrokeLineLook.Dotted)
                    {
                        AddDots(mesh, start, end, uv);
                    }
                    else
                    {
                        AddStrip(mesh, start, end, uv);
                    }
                }

                start = end;
            }
        }

        private void EnsureBuffers()
        {
            if (points == null)
            {
                points = new Vector2[capacity];
                lineEnds = new int[mostLines];
            }
        }

        private void AddStrip(VertexHelper mesh, int start, int end, Vector4 uv)
        {
            float half = width * 0.5f;
            float along = 0f;
            AddCap(mesh, points[start], half, ColourAt(along), uv);
            int stripFirst = -1;
            for (int index = start; index < end; index++)
            {
                if (index > start)
                {
                    along += (points[index] - points[index - 1]).magnitude;
                }

                Vector2 before = index > start ? points[index] - points[index - 1] : Vector2.zero;
                Vector2 after = index + 1 < end ? points[index + 1] - points[index] : Vector2.zero;
                Vector2 direction = (before.normalized + after.normalized).normalized;
                if (direction == Vector2.zero)
                {
                    direction = after != Vector2.zero ? after.normalized : before.normalized;
                }

                bool turnsBack = before != Vector2.zero && after != Vector2.zero
                    && Vector2.Dot(before.normalized, after.normalized) < TurnsBack;
                Color32 colour = ColourAt(along);
                if (turnsBack)
                {
                    AddEdge(mesh, points[index], before.normalized, half, colour, uv, ref stripFirst);
                    AddCap(mesh, points[index], half, colour, uv);
                    stripFirst = -1;
                    direction = after.normalized;
                }

                AddEdge(mesh, points[index], direction, half, colour, uv, ref stripFirst);
            }

            AddCap(mesh, points[end - 1], half, ColourAt(along), uv);
        }

        private static void AddEdge(VertexHelper mesh, Vector2 point, Vector2 direction, float half, Color32 colour, Vector4 uv,
            ref int stripFirst)
        {
            Vector2 normal = new Vector2(-direction.y, direction.x) * half;
            float middle = (uv.x + uv.z) * 0.5f;
            int left = mesh.currentVertCount;
            mesh.AddVert(point + normal, colour, new Vector4(middle, uv.w));
            mesh.AddVert(point - normal, colour, new Vector4(middle, uv.y));
            if (stripFirst >= 0)
            {
                mesh.AddTriangle(stripFirst, left, left + 1);
                mesh.AddTriangle(stripFirst, left + 1, stripFirst + 1);
            }

            stripFirst = left;
        }

        private static void AddCap(VertexHelper mesh, Vector2 centre, float half, Color32 colour, Vector4 uv)
        {
            Vector2 uvCentre = new Vector2((uv.x + uv.z) * 0.5f, (uv.y + uv.w) * 0.5f);
            Vector2 uvHalf = new Vector2((uv.z - uv.x) * 0.5f, (uv.w - uv.y) * 0.5f);
            int first = mesh.currentVertCount;
            mesh.AddVert(centre, colour, uvCentre);
            for (int corner = 0; corner < CapCorners; corner++)
            {
                float angle = corner * Mathf.PI * 2f / CapCorners;
                Vector2 round = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle));
                mesh.AddVert(centre + round * half, colour, uvCentre + Vector2.Scale(round, uvHalf));
            }

            for (int corner = 0; corner < CapCorners; corner++)
            {
                mesh.AddTriangle(first, first + 1 + corner, first + 1 + (corner + 1) % CapCorners);
            }
        }

        private void AddDots(VertexHelper mesh, int start, int end, Vector4 uv)
        {
            float half = width * 0.5f;
            Color32 colour = color;
            float untilNext = 0f;
            AddCap(mesh, points[start], half, colour, uv);
            for (int index = start + 1; index < end; index++)
            {
                Vector2 from = points[index - 1];
                Vector2 step = points[index] - from;
                float length = step.magnitude;
                float walked = 0f;
                while (length - walked >= dotSpacing - untilNext)
                {
                    walked += dotSpacing - untilNext;
                    untilNext = 0f;
                    AddCap(mesh, from + step * (walked / length), half, colour, uv);
                }

                untilNext += length - walked;
            }
        }

        private Color32 ColourAt(float along)
        {
            if (look != StrokeLineLook.Rainbow || rainbow.Length == 0)
            {
                return color;
            }

            float band = along / rainbowBand;
            int first = (int)band % rainbow.Length;
            int second = (first + 1) % rainbow.Length;
            Color mixed = Color.Lerp(rainbow[first], rainbow[second], band - Mathf.Floor(band));
            mixed.a *= color.a;
            return mixed;
        }
    }
}
