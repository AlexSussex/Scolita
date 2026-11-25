using System;
using System.Collections;
using System.IO;
using System.Linq;
using System.Runtime.ExceptionServices;
using NUnit.Framework;
using Scolita.Core.Content;
using Scolita.Core.Profiles;
using Scolita.Core.Timing;
using Scolita.Game.Lessons;
using Scolita.Game.Profiles;
using Scolita.Game.Saving;
using Scolita.Game.Scenes;
using Scolita.Game.Timing;
using Scolita.Tests.PlayMode.Map;
using Scolita.Tests.PlayMode.Parent;
using Scolita.UI.Profiles;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Scolita.Tests.PlayMode.Profiles
{
    /// <summary>
    /// Helpers the profile PlayMode tests share: keeping the Editor's own save root (the profile list, every profile's
    /// folder, the tablet's settings) and putting it back, writing a profile list, and opening and finding the scenes.
    /// Used by <see cref="ProfilesFlowTests"/> and <see cref="ParentProfilesTests"/>.
    /// </summary>
    public static class ProfilesScene
    {
        private static readonly string[] RootFiles = { ProfilesData.FileName + ".json", "settings.json" };

        /// <summary>The Editor's save root.</summary>
        public static string Root => SaveRoot.Path;

        /// <summary>
        /// Copies the Editor's profile list, profile folders and tablet settings aside, then clears them, so a test starts
        /// from a tablet with nothing; returns where they were copied. Used by the profile tests.
        /// </summary>
        public static string Keep()
        {
            string kept = Path.Combine(Path.GetTempPath(), "scolita-kept-" + Guid.NewGuid().ToString("N"));
            Directory.CreateDirectory(kept);
            string profiles = Path.Combine(Root, SaveStore.ProfilesFolderName);
            if (Directory.Exists(profiles))
            {
                CopyFolder(profiles, Path.Combine(kept, SaveStore.ProfilesFolderName));
            }

            foreach (string name in RootFiles.Where(name => File.Exists(Path.Combine(Root, name))))
            {
                File.Copy(Path.Combine(Root, name), Path.Combine(kept, name));
            }

            Clear();
            return kept;
        }

        /// <summary>
        /// Runs <paramref name="body"/> on an empty save root: keeps the Editor's own saves first and, whether the body
        /// passes or fails, leaves its open scene (so nothing it opened saves after) and puts the saves back, then
        /// rethrows a failure. Used by every profile test.
        /// </summary>
        public static IEnumerator Isolated(Func<IEnumerator> body)
        {
            string kept = Keep();
            ExceptionDispatchInfo failure = null;
            IEnumerator steps = body();
            while (true)
            {
                object current;
                try
                {
                    if (!steps.MoveNext())
                    {
                        break;
                    }

                    current = steps.Current;
                }
                catch (Exception exception)
                {
                    failure = ExceptionDispatchInfo.Capture(exception);
                    break;
                }

                yield return current;
            }

            yield return ParentScene.LeaveTheOpenScene();
            Restore(kept);
            failure?.Throw();
        }

        /// <summary>Puts back what <see cref="Keep"/> copied aside, removing what the test wrote; used by the profile tests.</summary>
        public static void Restore(string kept)
        {
            Clear();
            string profiles = Path.Combine(kept, SaveStore.ProfilesFolderName);
            if (Directory.Exists(profiles))
            {
                CopyFolder(profiles, Path.Combine(Root, SaveStore.ProfilesFolderName));
            }

            foreach (string name in RootFiles.Where(name => File.Exists(Path.Combine(kept, name))))
            {
                File.Copy(Path.Combine(kept, name), Path.Combine(Root, name), true);
            }

            Directory.Delete(kept, true);
        }

        /// <summary>
        /// Writes a profile list of children named <paramref name="names"/> (child-1, child-2, …; an empty name for none),
        /// each with a different avatar, and a tester when <paramref name="withTester"/>; nobody playing. Used by the
        /// profile tests.
        /// </summary>
        public static ProfileList SaveChildren(bool withTester, params string[] names)
        {
            ProfileStore store = Store();
            ProfileList list = store.Load();
            CalendarDay today = CalendarDay.FromDateTime(DateTime.Now);
            for (int index = 0; index < names.Length; index++)
            {
                ProfileName.Read(names[index], new ProfileRules(6, 20), out ProfileName name);
                ContentId avatar = ContentId.Create(Avatars()[index]);
                if (index == 0)
                {
                    list.Change(ProfileList.FirstChildId, name, avatar);
                }
                else
                {
                    list.AddChild(name, avatar, today);
                }
            }

            if (withTester)
            {
                list.AddTester(ContentId.Create("rocket"), today);
            }

            list.StopPlaying();
            Assert.That(store.Save(list), Is.True);
            return list;
        }

        /// <summary>Opens the saves of <paramref name="profile"/> (as "Who's playing?" does); used by the profile tests.</summary>
        public static void Play(string profile)
        {
            ProfileStore store = Store();
            ProfileList list = store.Load();
            list.Play(ProfileId.Create(profile));
            Assert.That(store.Save(list), Is.True);
        }

        /// <summary>The profile list as saved now; used by the profile tests.</summary>
        public static ProfileList Saved()
        {
            return Store().Load();
        }

        /// <summary>The store of the profile <paramref name="profile"/>'s folder; used by the profile tests.</summary>
        public static SaveStore FilesOf(string profile)
        {
            return SaveStore.ForProfile(Root, ProfileId.Create(profile));
        }

        /// <summary>Whether the folder of the profile <paramref name="profile"/> exists; used by the profile tests.</summary>
        public static bool FolderExists(string profile)
        {
            return Directory.Exists(Path.Combine(Root, SaveStore.ProfilesFolderName, profile));
        }

        /// <summary>
        /// Saves <paramref name="lessonId"/> as the lesson to open at its first step in <paramref name="profile"/>'s folder;
        /// used by the profile tests.
        /// </summary>
        public static void SaveLessonToOpen(string profile, string lessonId)
        {
            Assert.That(FilesOf(profile).Save(LessonRunData.FileName, new LessonRunData(ContentId.Create(lessonId), 1, string.Empty)),
                Is.True);
        }

        /// <summary>Opens the Boot scene and waits for the scene it opens; used by the profile tests.</summary>
        public static IEnumerator Boot(AppScene expected)
        {
            SceneManager.LoadScene(AppScene.Boot.ToString());
            yield return SceneSearch.WaitForScene(expected);
        }

        /// <summary>Opens "Who's playing?" and waits for its cards; used by the profile tests.</summary>
        public static IEnumerator OpenPicker()
        {
            SceneManager.LoadScene(AppScene.Profiles.ToString());
            yield return WaitForPicker();
        }

        /// <summary>Waits for "Who's playing?" and its cards; used by the profile tests.</summary>
        public static IEnumerator WaitForPicker()
        {
            yield return SceneSearch.WaitForScene(AppScene.Profiles);
            yield return MapScene.WaitFor(() => Runner() != null && Runner().PickerVoice != null);
        }

        /// <summary>The Profiles scene's composition root; used by the profile tests.</summary>
        public static ProfilesRunner Runner()
        {
            return SceneSearch.FindInActiveScene<ProfilesRunner>();
        }

        /// <summary>"Who's playing?"'s screen; used by the profile tests.</summary>
        public static ProfilesScreen Screen()
        {
            return SceneSearch.FindInActiveScene<ProfilesScreen>();
        }

        /// <summary>The tester's strip of the open scene; used by the profile tests.</summary>
        public static TesterStrip Strip()
        {
            return SceneSearch.FindInActiveScene<TesterStrip>();
        }

        /// <summary>The words in the mascot's bubble on "Who's playing?"; used by the profile tests.</summary>
        public static string BubbleWords()
        {
            return Screen().Bubble.Words;
        }

        private static string[] Avatars()
        {
            return new[] { "kitty", "cat", "unicorn", "whale", "fox", "owl" };
        }

        private static ProfileStore Store()
        {
            ContentId[] avatars = new[] { "kitty", "cat", "unicorn", "rocket", "star", "moon", "planet", "whale", "guitar",
                "bicycle", "rainbow", "fox", "owl", "dog" }.Select(ContentId.Create).ToArray();
            return new ProfileStore(Root, SaveStore.ForTablet(Root), avatars, new ProfileRules(6, 20), new SystemClock());
        }

        private static void Clear()
        {
            string profiles = Path.Combine(Root, SaveStore.ProfilesFolderName);
            if (Directory.Exists(profiles))
            {
                Directory.Delete(profiles, true);
            }

            foreach (string name in RootFiles)
            {
                File.Delete(Path.Combine(Root, name));
            }
        }

        private static void CopyFolder(string from, string to)
        {
            Directory.CreateDirectory(to);
            foreach (string file in Directory.GetFiles(from))
            {
                File.Copy(file, Path.Combine(to, Path.GetFileName(file)), true);
            }

            foreach (string folder in Directory.GetDirectories(from))
            {
                CopyFolder(folder, Path.Combine(to, Path.GetFileName(folder)));
            }
        }
    }
}
