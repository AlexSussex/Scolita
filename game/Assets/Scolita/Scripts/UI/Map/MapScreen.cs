using Scolita.Game.Scenes;
using UnityEngine;
using UnityEngine.UI;

namespace Scolita.UI.Map
{
    /// <summary>
    /// The Map placeholder's controller: its big Go button ("Hai!") opens the Lesson scene. The
    /// galaxy map replaces it (<c>galaxy-map</c>).
    /// </summary>
    public sealed class MapScreen : MonoBehaviour
    {
        [SerializeField] private Button goButton;

        private void OnEnable()
        {
            goButton.onClick.AddListener(OnGoClicked);
        }

        private void OnDisable()
        {
            goButton.onClick.RemoveListener(OnGoClicked);
        }

        private void OnGoClicked()
        {
            SceneNavigator.Load(AppScene.Lesson);
        }
    }
}
