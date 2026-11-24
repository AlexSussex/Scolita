# Școlița

A learning game for Android tablets that teaches a young child to read and
write in **Romanian and English at the same time**. A rocket flies from
planet to planet; each planet holds a few letters, and every lesson is a
short game: hear the sound, find the letter, trace it with a finger, build
a word, read it. Space, rainbows, unicorns, animals, music and drawing are
the world it lives in. Fully offline. Other subjects (numbers, music and
instruments, space and animals) come later on the same engine.

## Where things are

| Path | What |
| --- | --- |
| [`AGENTS.md`](AGENTS.md) | How the project is built: stack, seams, workflow, rules. Read first. |
| [`docs/prd.md`](docs/prd.md) | Product requirements: what the app does, every feature |
| [`docs/curriculum.md`](docs/curriculum.md) | How she learns: the approach, the letter order, the activities, the words |
| [`docs/mvp-build-plan.md`](docs/mvp-build-plan.md) | Build order, stack reasons, open decisions |
| [`docs/manual-testing.md`](docs/manual-testing.md) | What to test by hand on the tablet in each release |
| [`design/README.md`](design/README.md) | The look: the purple space world, the mascot, the language badges, the palette |
| [`design/screens.md`](design/screens.md) | Each designed screen, described for building it; the canvas's source in `design/canvas/` |
| [`tasks/`](tasks/README.md) | One brief per feature, with progress and completion records |
| [`CHANGELOG.md`](CHANGELOG.md) | What changed in each release; the current version is in `VERSION` (from `project-skeleton`) |
| `content/` | The lessons' source: letters, words, lessons and voice lines as CSV, with their art and audio (from `content-model`) |
| `game/` | The Unity project (Unity 6.3 LTS, C#, URP 2D). Open this folder in Unity Hub (from `project-skeleton`) |

## Status

Planning: the rules, the spec, the curriculum and the briefs are written;
no code yet. The first briefs are `project-skeleton` and
`curriculum-research`, which can run side by side. See
[`tasks/README.md`](tasks/README.md) → "Where we are".

## Set up the PC

Already done for SeliSnake on this PC (`C:\Projects\SeliSnake\README.md` →
"Set up the PC"): Unity Hub with a Unity Personal licence, **Unity
`6000.3.29f1`** with Android Build Support (OpenJDK, Android SDK & NDK
Tools), and Git LFS. Școlița uses the same Unity version, so nothing new
to install.

## Open the app in Unity

Once `project-skeleton` is done: Unity Hub → Projects → **Add** → choose
`C:\Projects\Scolita\game`, then open it. The first import builds
`game/Library/` and takes a few minutes. Wait until the import progress bar
has finished, open `Assets/Scolita/Scenes/Boot.unity` and press Play.

## Run the tests

In the Editor: Window → General → **Test Runner** → EditMode → Run All, then
PlayMode → Run All.

From a terminal (the Editor must be closed for this project):

```bash
UNITY="C:/Program Files/Unity/Hub/Editor/6000.3.29f1/Editor/Unity.exe"
"$UNITY" -batchmode -nographics -projectPath game -runTests -testPlatform EditMode -testResults editmode.xml
"$UNITY" -batchmode -projectPath game -runTests -testPlatform PlayMode -testResults playmode.xml
```

## Edit the content

The lessons are data, not code (from `content-model`):

1. Edit the CSV files in `content/` (letters, words, lessons, voice lines)
   in a text editor or a spreadsheet, saved as UTF-8 (Excel: "CSV UTF-8").
   Romanian ș and ț must be the comma-below letters; the validator says
   where a cedilla slipped in.
2. In Unity: **Școlița → Import content**. It rebuilds the assets under
   `Assets/Scolita/Content` and lists any problem (a word using a letter
   not taught yet, a missing picture or clip).
3. Run the EditMode tests; the content validator must pass.

## Record voice lines

`content/voice-lines.csv` lists everything the app says and whether its
clip is recorded. The guide is `local/todo/04-record-voices.md` (how to
record, the file names, the loudness). Lines without a clip use the
tablet's own voice until recorded.

## Build a debug APK

Once `project-skeleton` is done:

```bash
"$UNITY" -batchmode -nographics -quit -projectPath game \
  -executeMethod Scolita.Editor.BuildScript.BuildAndroidDebug -logFile build.log
```

The APK is written to `game/Builds/Android/Scolita-<version>-debug.apk`
(ignored by git). Every build checks the APK and fails if it asks for
`android.permission.INTERNET`.

## Build a release APK

From the tag of the release, with the Școlița release key (made once, see
`local/todo/02-release-key.md`; a key of its own, not SeliSnake's), its
passwords given as environment variables for this one command (never saved
in the repo):

```bash
git checkout v<version>
SCOLITA_KEYSTORE_PATH="C:/Projects/Scolita-secrets/scolita-release.keystore" \
SCOLITA_KEYSTORE_PASS="<from your password manager>" \
SCOLITA_KEY_ALIAS="scolita" \
SCOLITA_KEY_PASS="<from your password manager>" \
"$UNITY" -batchmode -nographics -quit -projectPath game \
  -executeMethod Scolita.Editor.BuildScript.BuildAndroidRelease -logFile build.log
```

The APK is written to `game/Builds/Android/Scolita-<version>.apk`. The
signing keeps the passwords out of `build.gradle` and Gradle's caches from
the start (SeliSnake's `release-signing` follow-up, built in
`project-skeleton` here).

## Install on the tablet

The Galaxy Tab A7 already has USB debugging on (set up for SeliSnake).
Connect it by USB, then:

```bash
adb devices
adb -s <the tablet's serial> install -r game/Builds/Android/Scolita-<version>.apk
```

Or copy the APK to the tablet and open it there. Every APK is signed with
the same release key, so a new version installs over the old one and keeps
her progress.
