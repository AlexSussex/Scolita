# Task: feature/project-skeleton

> **Status: In progress (skeleton)** — Unity 6000.3.29f1 (as SeliSnake); the Galaxy Tab A7,
> minimum API 26; landscape left and right.

Read `AGENTS.md` first. This is the first brief; it builds the foundation
every later feature sits on. No lessons yet. **Read SeliSnake's
`tasks/done/project-skeleton.md` completion record**
(`C:\Projects\SeliSnake\tasks\done\project-skeleton.md`, read only): the
same project was built there on the same PC, and its conventions,
pitfalls and fixes apply here (the launcher manifest without `INTERNET`,
ARM64 only, `-force-gles` on the emulators, the release build's caches).

**Feature:** `feature/project-skeleton`
**Implements:** PRD §4 (technical architecture), `AGENTS.md` → "Stack",
"Repo layout", "The five seams", "Versioning and releases".
**Depends on:** nothing.

## Scope

1. **The Unity project in `game/`**, from the **Universal 2D** template,
   with SeliSnake's project settings, adapted:
   - Editor: **Asset Serialization: Force Text**; **Visible Meta Files**.
   - Player: Company `softshack`, Product `Școlița` (check the launcher
     label shows the Ș and ț on the emulator), package
     `com.softshack.scolita`; **landscape left and right** only; IL2CPP;
     **ARM64** only (as SeliSnake, whose completion record says why);
     minimum API 26; target API the highest Unity 6.3 installs; Input
     System Package (New) only; no Unity splash; services off; target 60
     fps; the screen kept awake during a lesson only (not on the map, so
     the tablet can sleep when she walks away).
   - Graphics APIs: the same as SeliSnake ends up with after its Tab A7
     check (ask the human, or read SeliSnake's `tasks/README.md`).
2. **Packages:** URP (from the template), Input System, TextMeshPro (its
   essential resources), Unity Localization, Unity Test Framework, 2D
   Sprite; at the versions SeliSnake's `game/Packages/manifest.json` uses.
   Nothing else.
3. **Folders and assembly definitions** as in `AGENTS.md` → "Repo layout":
   - `Scolita.Core` (`Scripts/Core`) — **`noEngineReferences: true`**, no
     references;
   - `Scolita.Content` → Core;
   - `Scolita.Game` → Core, Content, Unity.InputSystem, Unity.TextMeshPro;
   - `Scolita.UI` → Core, Content, Game, Unity.TextMeshPro,
     Unity.Localization;
   - `Scolita.Editor` (Editor only) → all of the above;
   - `Scolita.Tests.EditMode` (Editor only) → Core, Content, Editor (and
     Game where a test needs it, as SeliSnake found);
   - `Scolita.Tests.PlayMode` → Core, Content, Game, UI.
4. **Core's foundation** (small, real, tested):
   - `IRandom` and `SeededRandom` (xorshift, as SeliSnake);
   - `IClock` (the current local date and the elapsed session time, both
     injectable) and `SystemClock` on the Unity side; Core counts days
     from the clock's local date, so a review "after 1 day" means the next
     calendar day on the tablet;
   - `Language` (`Romanian`, `English`) and `LanguageMix` (`Both`,
     `RomanianOnly`, `EnglishOnly`) enums;
   - `ProfileId` (a `readonly struct` around a validated `[a-z0-9-]+`
     string).
5. **The Unity side's foundation:**
   - **Scenes:** `Boot` (sets the frame rate, loads the profile, loads
     Map), `Map` (a placeholder with a big "Go" button), `Lesson` (a
     placeholder with a home button); loaded by an enum through a
     `SceneNavigator`, never by a name in text (SeliSnake's convention).
   - `SaveStore`: versioned JSON per profile under
     `Application.persistentDataPath/profiles/<profileId>/`, with
     SeliSnake's rules (`ISaveData<T>`, `Upgraded()`, clamping, the
     broken-file rule, `[a-z0-9_-]+` names). One `settings` file to prove
     it (music, sounds, voice volume, vibration, the app's language, the
     language mix). The MVP's single profile is `child-1`, created at first
     start.
6. **Fonts:** Andika (reading, OFL) and Baloo 2 (the app's words, OFL) as
   TextMeshPro font assets whose atlases include ă â î ș ț and their
   capitals (Ă Â Î Ș Ț), with their licences in `ThirdParty/`. An EditMode
   test checks both atlases contain those ten characters, and that the
   comma-below ș ț (U+0219, U+021B) are there, not only the cedilla ones.
   If Andika lacks any of them, stop and tell the human (Progress →
   "Blocked").
7. **`Scolita.Editor.BuildScript`** with `BuildAndroidDebug` and
   `BuildAndroidRelease`, as SeliSnake's (`VERSION` read and checked,
   version code at least 1, the `INTERNET` check with `aapt`, exit codes),
   **plus signing that never writes the passwords into `build.gradle`**:
   a Gradle signing template (`Plugins/Android/mainTemplate.gradle` or the
   launcher's) that reads `SCOLITA_KEYSTORE_PASS` and `SCOLITA_KEY_PASS`
   from the environment at Gradle time, so they never reach Unity's or
   Gradle's caches. This is SeliSnake's open `release-signing` follow-up;
   write what worked back to `local/for-human-next-steps.md` so the human
   can tell SeliSnake's lead.
8. **`VERSION`** at the repo root, `0.1.0`; the version shown small on the
   Map placeholder (bottom corner) until the parent area exists.
9. **The content validator** as an EditMode test over every
   ScriptableObject under `Content/` (`IValidatableContent`), passing with
   none yet; and a **diacritics check** over every string table and, later,
   every content CSV: fails on ş ţ Ş Ţ (cedilla) anywhere.
10. **`.gitignore` and `.gitattributes`** as SeliSnake's, plus Git LFS for
    `content/**/*.ogg`, `*.wav`, `*.png`, `*.psd`, `*.ttf`.

## Out of scope — do not build

- Any lesson, activity, map, mascot or content beyond the placeholders.
- A profile screen (one profile, made at first start).
- Art beyond plain text.

## Acceptance criteria

- Opening `game/` compiles with **zero warnings**; Play from `Boot` goes
  Boot → Map; "Go" goes to Lesson; the home button returns to Map.
- `Scolita.Core` has no UnityEngine reference (asmdef and a test on the
  compiled assembly).
- The batch-mode EditMode and PlayMode runs pass; the debug APK installs
  and starts on the agent's emulator in landscape, with the launcher label
  "Școlița" (comma-below Ș, ț) and the version from `VERSION`.
- A malformed `VERSION` fails the build with a clear message.
- The release build refuses to run without its four variables; with a
  throwaway key, it succeeds and **no password appears in the build log,
  under `game/`, in `Library/` or in `%USERPROFILE%\.gradle\caches`**
  (search them).
- The APK has no `INTERNET` permission and no Unity splash.
- A corrupt `settings.json` is set aside and the app starts with the
  defaults.
- The font test passes: the ten Romanian letters are in both atlases.

## Tests required

- EditMode: `SeededRandom`, `ProfileId` validation, the VERSION parser and
  version-code formula, `SaveStore` (round trip, missing, broken, per
  profile folder), the Core-has-no-engine test, the font atlas test, the
  cedilla check.
- PlayMode: Boot loads Map; Go loads Lesson; home loads Map.
- Emulator: install and start the debug APK; screenshots at 2000 × 1200
  and 1280 × 800.

## Touches

Everything (it creates the project). **No other code brief can run in
parallel with this one** (`curriculum-research` can: it is docs only and
touches `docs/curriculum.md` and `design/research/`).

## Screens

Boot, the Map and Lesson placeholders (2000 × 1200 and 1280 × 800).

## Voice lines

None.

## Before closing out

- Add the human's steps "create the release key" and "open the project in
  Unity Hub" to `local/for-human-next-steps.md`.
- Update README with the exact package and font versions used.
- Release 0.1.0.

## Progress

_Kept by `skeleton` while the brief is in flight._

### Status

- **Claimed** in `75d614b`.
- **Pushed** (rebased on `cb2e467`): `de1ce39` build (the project,
  packages, settings, TextMeshPro resources, the launcher manifest and
  Gradle template), `68f68ff` core (random, clock, languages, profile id,
  the cedilla rule, their tests), `453ff68` content (the validator and the
  cedilla check), `9f3080f` game (saving, settings, boot, scenes by enum,
  the clock), `0e68bac` ui (the placeholder controllers), `cc72fce` build
  (the build script with Gradle-time signing, the font builder, their
  tests), `77308b7` content (Andika, Baloo 2, the atlases, the font test),
  `a0dd323` ui (the prefabs), and the `game:` commit with the scenes, the
  PlayMode tests and this Progress. Reviewed before the push (see
  "Fresh-eyes review"), tested at the last of them (see "Test results").
- **Next:** the close-out (release 0.1.0).

### Design note (medium change)

- **Where the logic lives.**
  - Core (`Scolita.Core`, `noEngineReferences: true`, no references):
    `IRandom` + `SeededRandom` (`Core/Randomness`); `IClock` + `CalendarDay`
    (`Core/Timing`); `Language`, `LanguageMix`, `LanguageCodes` (the stable
    `ro` / `en` / `both` codes), `RomanianSpelling` + `CedillaFinding` (the
    cedilla rule, for the validator now and the content importer later)
    (`Core/Languages`); `ProfileId` (`Core/Profiles`).
  - Content (`Scolita.Content`): `IValidatableContent`.
  - Game (`Scolita.Game`): `Boot/BootLoader`, `Scenes/AppScene` +
    `SceneNavigator`, `Saving/` (`SaveStore` with `ForTablet` and
    `ForProfile`, `ISaveData<T>`, `SaveLoadResult<T>`, `SaveLoadOutcome`),
    `Settings/` (`TabletSettingsData`, `LearningSettingsData`),
    `Timing/SystemClock`, `Profiles/BuiltInProfiles` (`child-1`),
    `Lessons/LessonRunner` (the Lesson scene's root; keeps the screen awake
    while the Lesson is open, and only then).
  - UI (`Scolita.UI`): `Map/MapScreen` (Go), `Map/VersionLabel`,
    `Lesson/LessonHud` (home), `Layout/SafeAreaFitter`.
  - Editor (`Scolita.Editor`): `BuildScript`, `AppVersion`,
    `ReleaseSigning`, `GradleSigningFile` + `GradleSigningContents`,
    `ApkPermissions`, `ApkSigner`, `AndroidSettingsSnapshot`,
    `AndroidBuildKind`; `Fonts/FontAssetBuilder` (+ `FontAssetSpec`,
    `FontCharacterSet`).
- **Time.** Core never reads a clock: `IClock.Today` is a `CalendarDay`
  (a day number, no time, no time zone), so "after 1 day" is the next
  calendar day on the tablet whatever the hour; `IClock.SessionElapsed`
  is the real time since the clock was made. `SystemClock` (Unity side)
  takes `DateTime.Now`'s date and `Time.realtimeSinceStartupAsDouble`.
  Tests give Core a fake clock with the day they want.
- **`SeededRandom` is SplitMix64,** the xorshift-multiply generator
  SeliSnake uses (the brief's "xorshift, as SeliSnake"): every seed valid,
  the same sequence everywhere; a test pins seed 42's sequence (the same
  numbers as SeliSnake's). Only `NextInt` for now (picking and shuffling
  cards); a float comes when a rule needs one.
- **Saving.** SeliSnake's `SaveStore` rules (versioned JSON,
  `ISaveData<T>.Upgraded()` migrates and clamps, `<name>.broken.json` and
  the defaults for a file that can't be read, `Unreadable` and no save over
  it when even that fails, writes through a flushed `.tmp` and
  `File.Replace`, names `[a-z0-9_-]+`), plus where the files live:
  `SaveStore.ForTablet(root)` for the tablet's own files and
  `SaveStore.ForProfile(root, profileId)` for `profiles/<profileId>/`.
  `ProfileId` (`[a-z0-9-]+`) keeps a profile's folder name safe.
- **How it serves another domain:** nothing here is about letters: the
  random, the clock, the language codes, the profile folders and the
  validator serve numbers or music as they are.
- **Tests:** EditMode for every Core type, the saving (round trip,
  missing, broken, empty, unknown version, older broken file, clamping,
  unusable folder, invalid names, per-profile folders that never touch
  each other), the settings data, `SystemClock`, the build pieces
  (`AppVersion`, `ReleaseSigning`, `GradleSigningFile`, `ApkPermissions`,
  `ApkSigner`, the snapshot, the launcher manifest and Gradle template,
  the Player Settings), the content validator, the cedilla check, the font
  atlases and Core's compiled references; PlayMode for Boot → Map → Lesson
  → Map.

### Release signing without passwords in Gradle (SeliSnake's follow-up)

- **What Unity does by itself:** with a keystore in Player Settings, Unity
  writes both passwords into the generated `launcher/build.gradle` and its
  own build caches (`Library/Bee`), and Gradle compiles that script into
  its shared cache (`%USERPROFILE%\.gradle\caches`). SeliSnake found all
  three.
- **What Școlița does instead:** Unity is never given the key
  (`useCustomKeystore` stays off; `BuildScript` turns it off for a build
  if someone turned it on). For a release build, `BuildScript` reads the
  four `SCOLITA_*` variables, removes them from its own process at once
  (Unity prints its whole environment to the log when Gradle fails), and
  writes the key's path, alias and passwords to a JSON file in a folder of
  its own in the user's temporary folder (`%TEMP%\scolita-signing\`),
  naming it in one variable, `SCOLITA_SIGNING_FILE` (a path, no secret).
  The launcher's Gradle template
  (`Assets/Plugins/Android/launcherTemplate.gradle`, Unity's own plus a few
  lines at the end) reads that file when Gradle configures the build and
  sets the release signing config from it, so `build.gradle` holds only
  `System.getenv('SCOLITA_SIGNING_FILE')`. The folder is deleted after the
  build (in a `finally`) and, if a build was killed, at the start of the
  next one. After a release build `BuildScript` checks the APK's signer
  with `apksigner` and refuses (and deletes) an APK signed with the debug
  key, which would not install over her copy.
- **Why a file and not the two variables the brief names:** Gradle runs
  as Unity's child process, so a variable Gradle can read is one Unity
  holds, and Unity prints every variable it holds when Gradle fails: a
  wrong alias would put both passwords in `build.log`. The file is named
  by a variable that holds no secret.

### Decisions and deviations

- **Settings per tablet and per profile (AGENTS.md's seam 5 over the
  brief's one file).** The brief asks for one `settings` file under
  `profiles/<profileId>/` holding the volumes, vibration, the app's
  language and the language mix. `AGENTS.md` ("The five seams", 5), the
  PRD (§4 "Saving", "Profiles") and the `profiles` brief all put volumes,
  vibration and the app's language per tablet, and only the learning
  settings per profile. So there are two files: `settings.json` at the
  save root (`TabletSettingsData`: music on, sounds on, the voice volume
  0–1, vibration on, the app's language) and
  `profiles/child-1/learning-settings.json` (`LearningSettingsData`: the
  language mix). Both follow the saving rules; `profiles` adds the
  profile list beside `settings.json`.
- **Defaults:** the app's language Romanian (D1, the anchor), the language
  mix both, everything on, the voice at full volume.
- **Graphics APIs: automatic** (Vulkan, then OpenGL ES 3), as SeliSnake
  still has it: its Tab A7 check hasn't been recorded yet, so whether
  Vulkan is dropped is still open in SeliSnake's `tasks/README.md`. The
  emulator runs the app with `-e unity -force-gles`.
- **Template:** Universal 2D, the `com.unity.template.2d-cross-platform-2d`
  6.1.6 shipped with 6000.3.29f1 (SeliSnake's "universal-2d 6.1.6"),
  cloned in batch mode with `-createProject game -cloneFromTemplate`.
- **Packages** (`game/Packages/manifest.json`), SeliSnake's versions: URP
  17.3.0, Input System 1.20.1, uGUI 2.0.0 (which holds TextMeshPro),
  Localization 1.5.8 (it brings Addressables), Test Framework 1.6.0, 2D
  Sprite 1.0.0, and the IDE packages Visual Studio 2.0.26 and Rider 3.1.0
  (editor only, as SeliSnake: they write the C# project files for the
  human's code editor and never reach the APK). 2D SpriteShape (in
  SeliSnake from its template) is left out, as the brief lists only 2D
  Sprite. Removed from the template as SeliSnake did: collab-proxy,
  multiplayer.center, 2d.animation, 2d.aseprite, 2d.psdimporter,
  2d.tilemap(.extras), 2d.tooling, timeline, visualscripting, and the same
  built-in modules (SeliSnake's module list exactly).
- **Settings as SeliSnake's** (`ProjectSettings.asset` differs from
  SeliSnake's only in the names, the package id, the custom launcher
  Gradle template switch and Unity's own bookkeeping): Force Text, Visible
  Meta Files, IL2CPP, ARM64 only, minimum API 26, target API automatic,
  Input System only, no splash, services and hardware statistics off,
  Engine Diagnostics off, the custom launcher manifest without `INTERNET`.
- **Render pipeline and input settings** moved from the template's
  `Assets/Settings` to `Assets/Scolita/Settings` (GUIDs kept), as
  AGENTS.md's layout says. TextMeshPro's essential resources are in
  `Assets/TextMesh Pro`, unpacked from the package's `.unitypackage` with
  their GUIDs (byte for byte SeliSnake's, line endings aside).
- **Fonts:** Andika 7.000 Regular (SIL) and Baloo 2 1.700 (the variable
  font from Google Fonts' repository, Regular by default), unchanged, in
  `Assets/ThirdParty/Andika` and `Assets/ThirdParty/Baloo2`, each with
  `LICENSE.txt` (OFL 1.1) and `SOURCE.md` (where from, checksums). Both
  have all ten letters, with the comma below. The TextMeshPro font assets
  (`Assets/Scolita/Fonts/Andika-Regular SDF.asset`, `Baloo2 SDF.asset`)
  are static SDF atlases (72 pt sampling, 8 px padding, 1024 × 1024)
  holding printable ASCII, ă â î ș ț Ă Â Î Ș Ț and „ ” “ ‘ ’ – — … • ·,
  built by Școlița → Rebuild font assets (`FontAssetBuilder`, which
  rebuilds in place, keeping the GUIDs). The cedilla ş ţ are deliberately
  not in the atlases, so one that slipped past the validator shows as
  missing. A text that needs another character adds it to
  `FontCharacterSet` and rebuilds.
- **Placeholders:** the Map is the purple sky with "Școlița" in Baloo 2, a
  yellow "Hai!" button and the version bottom right; the Lesson is a cream
  card with "ă â î ș ț / Ă Â Î Ș Ț" in Andika (to see the reading font on
  the tablet) and a cream "Acasă" button top left. Plain shapes and text
  only (no art, as the brief says); nothing is spoken yet (the brief has
  no voice lines).
- **Assembly references beyond the brief's list** (as SeliSnake):
  `Scolita.UI` and `Scolita.Tests.PlayMode` also reference `UnityEngine.UI`
  (for `Button`); `Scolita.Editor` references `Unity.TextMeshPro` (the font
  builder); `Scolita.Tests.EditMode` references `Scolita.Game`,
  `Unity.TextMeshPro` and `Unity.Localization` (saving, fonts, string
  tables).
- **`VERSION` is `0.0.0`** until the close-out (version code 1, as
  SeliSnake), so the Editor's Play mode shows `0.0.0`.
- **Debug APK = development build** (as SeliSnake).
- **Device Simulator not available:** the agent runs Unity in batch mode,
  without the Editor's window, so the screen sizes were checked on the
  emulator (`adb shell wm size`), as SeliSnake did.

### Usability of the placeholder screens

- The Go button is 420 × 140 and the home button 112 × 112 reference
  pixels (canvas 1280 × 800, match 0.5), well over 64 dp at every size
  checked; the home button is top left, inside the safe area, 24 px from
  the edges; leaving the Lesson never asks. Text sizes: the title 120,
  "Hai!" 72, the letters 140, "Acasă" 28, the version 24 (reference
  pixels), white or dark ink on purple or cream. Nothing is spoken yet;
  the real screens come with `galaxy-map` and `lesson-engine`.

### Fresh-eyes review (2026-11-24)

A second agent read the saving and the build/signing code against
AGENTS.md before the push. Verdict: no blockers (no path found where a
password reaches Player Settings, `build.gradle`, the log or Gradle's
caches). Findings and what was done:

1. **Builds in other worktrees could delete a running release's signing
   file** (one shared `%TEMP%\scolita-signing`). Now each release build
   has its own folder, `scolita-signing\<pid>-<random>`, deleted in its
   `finally`; only a release build sweeps leftovers, and only folders
   whose Unity process is gone; a debug build touches nothing there
   (`GradleSigningFile.NewBuildFolder`, `DeleteLeftovers`, tests).
2. **Cleanup order:** the signing folder is deleted before the settings
   are restored, a failure to delete it is logged and doesn't hide the
   build's own error, and the leftover sweep runs before the variables
   are read (so a missing-variable failure still cleans up).
3. **The release build type is pinned** (`EditorUserBuildSettings.androidBuildType`
   = Release, Development for debug; restored afterwards), and a release
   APK that is debuggable is refused and deleted (`ApkBadging`).
4. **A file that can't be opened right now is kept, not set aside**
   (`Unreadable`), so a briefly locked good save is never replaced by the
   defaults (`TestLoad_LockedFileIsKeptAndNotSavedOver`).
5. **A file from a newer version of the app is kept** (`Unreadable`), not
   treated as damaged (`TestLoad_NewerVersionIsKeptAndNotSavedOver`);
   version 0 is still set aside.
6. **The store refuses to save over a file it loaded as `Unreadable`**
   (logs an error, returns false), instead of relying on callers.
7. **Temporary files are unique per save** (`<name>.<random>.json.tmp`,
   deleted if a write fails), so two test runs sharing the Editor's save
   folder can't collide. Giving `BootLoader` its own save root for the
   PlayMode test is a follow-up (see "Follow-ups").
8. **The keystore path is made full** before Gradle sees it.
9. Information: the variables are in Unity's environment before the build
   script clears them (README sets them for the one command only); the
   password searches also covered `%LOCALAPPDATA%\Unity` and
   `%TEMP%\Unity` (see "Test results").
10. **Gradle's configuration cache:** a test checks no custom Gradle
    properties template is used and the template never mentions the
    configuration cache (it would store the signing config).
11. A second broken file replaces the first `.broken.json` (as AGENTS.md
    says); keeping several is a question for the human (follow-up).
12. **Unknown language codes are kept** when a settings file is saved
    again, and read as the default (`TestUpgraded_KeepsUnknownCodes`).
13. XML docs on `AndroidBuildKind`'s values; the release checks are keyed
    on the build kind; the manifest and template tests renamed.

### Test results (2026-11-24, at the commits pushed for the slices)

- **EditMode** 154/154, **PlayMode** 3/3, in batch mode; no `warning CS`
  or `error CS` in any log.
- **`BuildAndroidDebug`:** exit 0, `Scolita-0.0.0-debug.apk` (47 MB):
  `com.softshack.scolita`, version code 1, `application-label:'Școlița'`,
  min SDK 26, target SDK 36, `arm64-v8a` only, landscape
  (`screenOrientation` 11), no `INTERNET`. `ProjectSettings.asset`
  unchanged after the build.
- **Malformed `VERSION`** (`1.2`): exit 1, "VERSION must be
  MAJOR.MINOR.PATCH, …, but it is '1.2'." **Release without variables:**
  exit 1, naming all four.
- **Release with a throwaway key** (random password, scratch only):
  exit 0; signer `CN=Throwaway Test`; not debuggable; no `INTERNET`;
  the generated `launcher/build.gradle` holds only
  `System.getenv('SCOLITA_SIGNING_FILE')`; the password found **nowhere**:
  0 hits in the build log, under `game/` (with `Library/`), in
  `%USERPROFILE%\.gradle\caches`, `%LOCALAPPDATA%\Unity` and
  `%TEMP%\Unity`; `%TEMP%\scolita-signing` empty afterwards.
- **Release with a wrong alias:** exit 1 (Gradle: "Failed to read key
  wrongalias from store"), 0 hits for the password and for `SCOLITA_KEY`
  in the log (Unity's environment dump held only `SCOLITA_SIGNING_FILE`),
  none under `game/` or in Gradle's caches, the signing folder empty. An
  earlier release build cut short when another process stopped the shared
  Gradle daemon left nothing either.
- **Emulator** (`emulator-5600`, `Scolita_Tablet_skeleton`, Pixel Tablet,
  Android 16 x86_64 running the ARM64 APK by translation, `-force-gles`):
  Boot → Map → Lesson → Map at 2560 × 1600 (native), 2000 × 1200 (the Tab
  A7) and 1280 × 800 (`wm size`); "Școlița", "Hai!", "Acasă" and the ten
  letters drawn with the comma below; `0.0.0` bottom right; the screen
  wake lock taken on the Lesson and released on the Map (`dumpsys power`);
  held in portrait (user rotation 1 and 3) the app stays landscape,
  letterboxed; Home and back resumes the same process on the Lesson; the
  first start writes `settings.json` (`appLanguage` `ro`) and
  `profiles/child-1/learning-settings.json` (`languageMix` `both`); a
  corrupt one of each is set aside as `.broken.json` with the defaults
  written and the warning logged; no `.tmp` file left. `logcat` shows
  only SeliSnake's three expected development-build socket errors.
- **Screen sizes:** 2000 × 1200 and 1280 × 800 as the brief asks (plus the
  emulator's native 2560 × 1600); the placeholders are centred, scaled
  layouts, so the other sizes add nothing until real screens exist.
  Screenshots in the agent's scratch folder.
- **Not run:** the Editor's Play mode and Device Simulator (batch mode
  only, as SeliSnake); listening (no voice lines in this brief).

### Follow-ups

- **`BootLoader` with its own save root for tests:** the PlayMode test
  writes to the Editor's `persistentDataPath` (shared by every worktree
  and the human's Play mode); an injectable root would also allow an
  end-to-end "corrupt settings at boot" PlayMode test (now checked on the
  emulator).
- **Several broken copies?** A second damaged file replaces the first
  `.broken.json` (AGENTS.md's rule). Whether to keep each (for progress
  files later) is a question for the human, before `progress-review`.
- **Graphics APIs:** follow SeliSnake's Tab A7 result (manual test
  0.1.0-2 here checks it too); drop Vulkan if it fails on the tablet.
- **`AGENTS.md` repo layout:** add `Assets/Scolita/Settings` (render
  pipeline and input settings), `Assets/Plugins/Android` (the launcher
  manifest and Gradle template) and `Assets/ThirdParty/<pack>/SOURCE.md`.
- **The app's words in both languages:** "Hai!" and "Acasă" are plain
  Romanian text until the string tables come (`galaxy-map`,
  `lesson-engine`).

## How to test locally

Commands are for Git Bash from the worktree (or checkout) root, with
`UNITY="C:/Program Files/Unity/Hub/Editor/6000.3.29f1/Editor/Unity.exe"`,
`ADB="$LOCALAPPDATA/Android/Sdk/platform-tools/adb"`, `EMU=emulator-5600`
(your own emulator), `S=<your scratch folder>`, and
`export MSYS_NO_PATHCONV=1` (so Git Bash leaves device paths alone). Close
your Editor on this project before any batch-mode run. On the emulator,
start the app with
`"$ADB" -s $EMU shell am start -n com.softshack.scolita/com.unity3d.player.UnityPlayerGameActivity -e unity -force-gles`
(the x86_64 emulator crashes under Vulkan; real tablets don't need the
flag).

### Build slice

1. **The project opens clean.** Open `game/` in Unity Hub with 6000.3.29f1,
   or run any batch command below: the log has no `error CS` and no
   `warning CS`. Expected: Project Settings → Player shows company
   `softshack`, product `Școlița`, package `com.softshack.scolita`,
   landscape left and right only, IL2CPP, ARM64, minimum API 26, target API
   automatic, Active Input Handling "Input System Package (New)", no
   splash, Publishing Settings → Custom Launcher Manifest and Custom
   Launcher Gradle Template on, no custom keystore; Editor → Asset
   Serialization "Force Text", Version Control "Visible Meta Files".
   `PlayerSettingsTests` checks these.
2. **Debug APK:**
   `"$UNITY" -batchmode -nographics -quit -projectPath game -executeMethod Scolita.Editor.BuildScript.BuildAndroidDebug -logFile $S/build.log; echo $?`
   Expected: exit 0, `Scolita 0.0.0 built: ...\game\Builds/Android\Scolita-0.0.0-debug.apk`
   in the log (the first build takes about 10 minutes), no `warning CS`,
   and `git status` shows no change under `game/ProjectSettings`.
3. **APK contents:** with
   `AAPT="C:/Program Files/Unity/Hub/Editor/6000.3.29f1/Editor/Data/PlaybackEngines/AndroidPlayer/SDK/build-tools/36.0.0/aapt.exe"`,
   `"$AAPT" dump badging <apk>`. Expected: `package: name='com.softshack.scolita' versionCode='1' versionName='0.0.0'`,
   `sdkVersion:'26'`, `targetSdkVersion:'36'`, `application-label:'Școlița'`,
   `native-code: 'arm64-v8a'`, and `launchable-activity` with
   `screenOrientation` landscape-sensor (`'6'` or `'11'`).
   `"$AAPT" dump permissions <apk>` lists only
   `com.softshack.scolita.DYNAMIC_RECEIVER_NOT_EXPORTED_PERMISSION`: no
   `INTERNET` (the build itself checks this and fails if it is there).
4. **A malformed VERSION stops the build:** write `1.2` into `VERSION`, run
   step 2, then `git checkout VERSION`. Expected: exit code 1, and the log
   says `Scolita build failed: VERSION must be MAJOR.MINOR.PATCH, each part
   0 to 99 without leading zeros (for example 0.4.2), but it is '1.2'.`
5. **The release build refuses without its variables:** run step 2 with
   `BuildAndroidRelease` and no `SCOLITA_*` variables. Expected: exit code
   1 and `Scolita build failed: The release build needs the release key's
   environment variables (README → "Build a release APK"); missing:
   SCOLITA_KEYSTORE_PATH, SCOLITA_KEYSTORE_PASS, SCOLITA_KEY_ALIAS,
   SCOLITA_KEY_PASS.`
6. **Release build with a throwaway key** (never the human's key): make
   one in `$S` with the Unity OpenJDK's
   `keytool -genkeypair -keystore $S/dummykey/throwaway.keystore -storepass <pw> -alias throwaway -keyalg RSA -keysize 2048 -validity 2 -dname "CN=Throwaway Test"`
   (a PKCS12 keystore: the key password is the store password), then run
   the release build with the four variables set for that one command:
   `SCOLITA_KEYSTORE_PATH=$S/dummykey/throwaway.keystore SCOLITA_KEYSTORE_PASS=<pw> SCOLITA_KEY_ALIAS=throwaway SCOLITA_KEY_PASS=<pw> "$UNITY" ... BuildAndroidRelease -logFile $S/build-release.log`.
   Expected: exit 0, `Scolita-0.0.0.apk`; `apksigner verify --print-certs`
   (`java -jar <build-tools>/lib/apksigner.jar`) shows
   `Signer #1 certificate DN: CN=Throwaway Test`; `aapt dump badging` has
   no `application-debuggable`; no `INTERNET` (the build checks all three
   and fails, deleting the APK, otherwise); `%TEMP%\scolita-signing` is
   empty after the build; and the password is nowhere:
   `grep -c <pw> $S/build-release.log` is 0, and `grep -rl -a <pw>` finds
   nothing in `game` (`game/Library` included), `~/.gradle/caches`,
   `$LOCALAPPDATA/Unity` or `$TEMP/Unity`. `git status` shows no change
   under `game/ProjectSettings`. The generated
   `game/Library/Bee/Android/Prj/IL2CPP/Gradle/launcher/build.gradle` has
   `System.getenv('SCOLITA_SIGNING_FILE')` and no password. Delete the
   APK afterwards.
7. **A failing release build leaks no password:** run step 6 with a wrong
   `SCOLITA_KEY_ALIAS`. Expected: exit 1, Gradle fails reading the key
   from the store, `grep -c <pw>` and `grep -c SCOLITA_KEY` on the log are
   both 0 (Unity's environment dump holds only `SCOLITA_SIGNING_FILE`, a
   path), and `%TEMP%\scolita-signing` is empty.
8. **A killed release build:** start step 6 and kill that Unity process
   (only yours: find it by its `-logFile` argument) once the log shows
   `IL2CPP`. Expected: `%TEMP%\scolita-signing\<pid>-<random>\release-signing.json`
   is left (it holds the passwords) and `git status` is clean. The next
   release build deletes it at its start (its Unity process is gone);
   or delete the folder by hand. A debug build never touches the folder.

### Core slice

1. **EditMode tests:**
   `"$UNITY" -batchmode -nographics -projectPath game -runTests -testPlatform EditMode -testResults $S/editmode.xml -logFile $S/editmode.log`
   Expected: exit 0; in `editmode.xml`, `result="Passed"` with every test
   passed (`SeededRandomTests`, `CalendarDayTests`, `ProfileIdTests`,
   `LanguageCodesTests`, `RomanianSpellingTests`, `CoreAssemblyTests` among
   them). The only warnings in the log are the `SaveStore` tests' expected
   "could not be read" lines.
2. **Core has no engine reference:** `CoreAssemblyTests` passes, and
   `Scripts/Core/Scolita.Core.asmdef` has `"noEngineReferences": true` and
   no references.

### Content slice

1. **The validator** (in the EditMode run above): `ContentValidatorTests`
   passes with no content (zero ScriptableObjects under
   `Assets/Scolita/Content`); `CedillaCheckTests` passes with no string
   table and no `content/*.csv` yet.
2. **The cedilla check bites:** write a file `content/check.csv` holding
   `id,text` and `w1,şase` (U+015F), run the EditMode tests, then delete
   the file. Expected: `TestFindCedillas_NoneInTheContentCsvs` fails with
   `...content\check.csv, line 2, column 4: U+015F ş must be U+0219 ș`.
3. **The fonts:** `FontAtlasTests` passes: both atlases hold ă â î ș ț Ă Â
   Î Ș Ț (comma below), each with a glyph, and no cedilla ş ţ Ş Ţ. In the
   Editor, Școlița → Rebuild font assets rebuilds both with no change
   under `git status` beyond the atlases' own bytes.

### Game slice

1. **PlayMode tests:**
   `"$UNITY" -batchmode -projectPath game -runTests -testPlatform PlayMode -testResults $S/playmode.xml -logFile $S/playmode.log`
   Expected: exit 0, 3 tests passed (`SceneFlowTests`: Boot opens the Map
   and lets the screen sleep; Go opens the Lesson, which keeps the screen
   awake; home returns to the Map).
2. **In the Editor** (the human; agents run without the Editor's window):
   open `Assets/Scolita/Scenes/Boot.unity`, press Play. Expected: the Map
   (purple, "Școlița", a big yellow "Hai!", `0.0.0` bottom right); Hai! →
   the Lesson placeholder (a cream card with "ă â î ș ț / Ă Â Î Ș Ț", a
   cream "Acasă" button top left); Acasă → the Map. The Console stays
   empty. Device Simulator: 2000 × 1200 and 1280 × 800.
3. **On the emulator:**
   `"$ADB" -s $EMU install -r game/Builds/Android/Scolita-0.0.0-debug.apk`,
   then start it (see the top). If Android shows "Viewing full screen", tap
   "Got it". Expected: the launcher shows the icon labelled "Școlița"
   (comma-below Ș and ț); a plain dark screen for a few seconds (no Unity
   logo), then the Map in landscape with `0.0.0` bottom right and
   "Development Build" in the corner. Tap Hai! (the screen's centre, a
   little below the middle): the Lesson. Tap Acasă (top left): the Map.
   `logcat -s Unity` has the three errors SeliSnake documented at each
   start of a development build without `INTERNET` ("Socket: Failed to set
   blocking mode" twice, "Unable to setup multicast socket for player
   connection"), nothing else. `exec-out screencap -p > $S/<name>.png` for
   each screen.
4. **The Tab A7's 2000 × 1200 and 1280 × 800:** `"$ADB" -s $EMU shell wm size 2000x1200`
   and `wm density 240` (then `1280x800` and `wm density 160`), force-stop
   and start the app again, repeat step 3; then `wm size reset` and
   `wm density reset`.
5. **Landscape only:** `settings put system accelerometer_rotation 0`,
   `settings put system user_rotation 0` then `1`, `2`, `3`. Expected: the
   app stays landscape (left or right), never portrait. Put back with
   `user_rotation 0` and `accelerometer_rotation 1`.
6. **Pause and resume:** on the Lesson, press Home
   (`input keyevent KEYCODE_HOME`), start the app again. Expected: the same
   process (`pidof com.softshack.scolita` unchanged) resumes on the Lesson.
7. **The settings files and corrupt ones:** after the first start,
   `"$ADB" -s $EMU shell cat /storage/emulated/0/Android/data/com.softshack.scolita/files/settings.json`
   shows version 1, music, sounds and vibration on, `voiceVolume` 1,
   `appLanguage` `ro`; and
   `.../files/profiles/child-1/learning-settings.json` shows version 1,
   `languageMix` `both`. Then force-stop,
   `shell "echo '{{{' > /storage/emulated/0/Android/data/com.softshack.scolita/files/settings.json"`
   (and the same for `profiles/child-1/learning-settings.json`), and start
   again. Expected: the Map as usual; each folder now holds its
   `.broken.json` (`{{{`) and a fresh file with the defaults;
   `logcat -s Unity` has "settings.json could not be read ... set aside as
   settings.broken.json and the defaults are used." (and the same for
   `learning-settings.json`).

### Cleanup

- `"$ADB" -s $EMU shell pm clear com.softshack.scolita` clears the save
  files; `"$ADB" -s $EMU uninstall com.softshack.scolita` removes the app.
- `game/Builds/` is ignored by git; delete it to free space. Delete the
  throwaway keystore with your scratch folder.
