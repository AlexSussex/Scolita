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
