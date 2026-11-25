using Scolita.Core.Profiles;
using Scolita.Game.Lessons;
using Scolita.Game.Profiles;
using Scolita.Game.Saving;
using Scolita.Game.Scenes;
using Scolita.Game.Settings;
using UnityEngine;

namespace Scolita.Game.Boot
{
    /// <summary>
    /// The Boot scene's controller: sets the frame rate, reads the tablet's settings and the
    /// profile's learning settings, selects the app's language for the string tables (a missing or set-aside file is replaced by the defaults, which
    /// makes the <c>child-1</c> profile at the first start), removes the damaged copies whose file a
    /// newer version has read and saved fine on enough days (<see cref="SaveStore.RemoveSettledCopies"/>,
    /// the number in the learning rules asset), in the tablet's folder and each profile's, then opens
    /// the Map, whatever happened to the files.
    /// </summary>
    public sealed class BootLoader : MonoBehaviour
    {
        [SerializeField] private int targetFrameRate = 60;
        [SerializeField] private LearningRulesAsset learningRules;

        private void Start()
        {
            Application.targetFrameRate = targetFrameRate;
            Screen.sleepTimeout = SleepTimeout.SystemSetting;
            try
            {
                string saveRoot = Application.persistentDataPath;
                SaveStore tabletFiles = SaveStore.ForTablet(saveRoot);
                EnsureFile(tabletFiles, TabletSettingsData.FileName, TabletSettingsData.CreateDefaults());
                AppLocale.Select(tabletFiles.Load(TabletSettingsData.FileName, TabletSettingsData.CreateDefaults()).Data.AppLanguage);
                ProfileId profile = BuiltInProfiles.FirstChild;
                EnsureFile(SaveStore.ForProfile(saveRoot, profile), LearningSettingsData.FileName,
                    LearningSettingsData.CreateDefaults());
                RemoveSettledCopies(saveRoot);
            }
            finally
            {
                SceneNavigator.Load(AppScene.Map);
            }
        }

        private void RemoveSettledCopies(string saveRoot)
        {
            int fineDays = learningRules.ToProgressRules().DamagedCopyFineDays;
            SaveStore.ForTablet(saveRoot).RemoveSettledCopies(fineDays);
            foreach (ProfileId profile in SaveStore.ProfilesIn(saveRoot))
            {
                SaveStore.ForProfile(saveRoot, profile).RemoveSettledCopies(fineDays);
            }
        }

        private static void EnsureFile<T>(SaveStore store, string name, T defaults) where T : class, ISaveData<T>
        {
            SaveLoadResult<T> result = store.Load(name, defaults);
            if (result.Outcome != SaveLoadOutcome.Unreadable)
            {
                store.Save(name, result.Data);
            }
        }
    }
}
