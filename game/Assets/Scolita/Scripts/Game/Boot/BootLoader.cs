using Scolita.Core.Profiles;
using Scolita.Game.Profiles;
using Scolita.Game.Saving;
using Scolita.Game.Scenes;
using Scolita.Game.Settings;
using UnityEngine;

namespace Scolita.Game.Boot
{
    /// <summary>
    /// The Boot scene's controller: sets the frame rate, reads the tablet's settings and the
    /// profile's learning settings (a missing or set-aside file is replaced by the defaults, which
    /// makes the <c>child-1</c> profile at the first start), then opens the Map, whatever happened
    /// to the files.
    /// </summary>
    public sealed class BootLoader : MonoBehaviour
    {
        [SerializeField] private int targetFrameRate = 60;

        private void Start()
        {
            Application.targetFrameRate = targetFrameRate;
            Screen.sleepTimeout = SleepTimeout.SystemSetting;
            try
            {
                string saveRoot = Application.persistentDataPath;
                EnsureFile(SaveStore.ForTablet(saveRoot), TabletSettingsData.FileName, TabletSettingsData.CreateDefaults());
                ProfileId profile = BuiltInProfiles.FirstChild;
                EnsureFile(SaveStore.ForProfile(saveRoot, profile), LearningSettingsData.FileName,
                    LearningSettingsData.CreateDefaults());
            }
            finally
            {
                SceneNavigator.Load(AppScene.Map);
            }
        }

        private static void EnsureFile<T>(SaveStore store, string name, T defaults) where T : class, ISaveData<T>
        {
            SaveLoadResult<T> result = store.Load(name, defaults);
            if (result.Outcome == SaveLoadOutcome.Missing || result.Outcome == SaveLoadOutcome.SetAside)
            {
                store.Save(name, result.Data);
            }
        }
    }
}
