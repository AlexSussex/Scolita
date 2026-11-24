using Scolita.Game.Scenes;
using UnityEngine;
using UnityEngine.UI;

namespace Scolita.UI.Lesson
{
    /// <summary>
    /// The Lesson placeholder's HUD: its home button, top left, returns to the Map at once, never
    /// asking to confirm.
    /// </summary>
    public sealed class LessonHud : MonoBehaviour
    {
        [SerializeField] private Button homeButton;

        private void OnEnable()
        {
            homeButton.onClick.AddListener(OnHomeClicked);
        }

        private void OnDisable()
        {
            homeButton.onClick.RemoveListener(OnHomeClicked);
        }

        private void OnHomeClicked()
        {
            SceneNavigator.Load(AppScene.Map);
        }
    }
}
