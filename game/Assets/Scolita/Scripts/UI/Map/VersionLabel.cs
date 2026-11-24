using TMPro;
using UnityEngine;

namespace Scolita.UI.Map
{
    /// <summary>
    /// Shows the app's version (<see cref="Application.version"/>, set from the repository's
    /// <c>VERSION</c> file by the build script) in its text; on the Map placeholder until the
    /// parent area shows it.
    /// </summary>
    [RequireComponent(typeof(TMP_Text))]
    public sealed class VersionLabel : MonoBehaviour
    {
        private void Awake()
        {
            GetComponent<TMP_Text>().text = Application.version;
        }
    }
}
