using UnityEngine;

namespace Scolita.UI.Layout
{
    /// <summary>
    /// Fits its RectTransform to the screen's safe area (<see cref="Screen.safeArea"/>), so the
    /// controls inside it stay clear of a display's cut-outs and rounded corners. Re-fits when the
    /// safe area or the screen size changes. On the panel of each screen that holds its buttons and labels.
    /// </summary>
    [RequireComponent(typeof(RectTransform))]
    public sealed class SafeAreaFitter : MonoBehaviour
    {
        private RectTransform area;
        private Rect fittedSafeArea;
        private Vector2Int fittedScreen;

        private void Awake()
        {
            area = GetComponent<RectTransform>();
            Fit();
        }

        private void Update()
        {
            if (Screen.safeArea != fittedSafeArea || Screen.width != fittedScreen.x || Screen.height != fittedScreen.y)
            {
                Fit();
            }
        }

        private void Fit()
        {
            fittedSafeArea = Screen.safeArea;
            fittedScreen = new Vector2Int(Screen.width, Screen.height);
            if (fittedScreen.x <= 0 || fittedScreen.y <= 0)
            {
                return;
            }

            area.anchorMin = new Vector2(fittedSafeArea.xMin / fittedScreen.x, fittedSafeArea.yMin / fittedScreen.y);
            area.anchorMax = new Vector2(fittedSafeArea.xMax / fittedScreen.x, fittedSafeArea.yMax / fittedScreen.y);
            area.offsetMin = Vector2.zero;
            area.offsetMax = Vector2.zero;
        }
    }
}
