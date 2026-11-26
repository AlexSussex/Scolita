# Task: feature/sound-feel

> **Status: In progress (sound-feel).** The last MVP brief.

Read `AGENTS.md` first (especially "Audio", "Performance"), and the
completion records in `tasks/done/`.

**Feature:** `feature/sound-feel`
**Implements:** PRD §3.1–§3.6 (sound and effects); `AGENTS.md` →
"Performance" (the budgets).
**Depends on:** `rewards`, `profiles`, `content-letters-1`, `look`.

## Scope

1. **Music:** a calm space theme on the map and a lighter loop in lessons
   (CC0 or made for the app, licences in `ThirdParty/`), streamed; the
   music ducks under the voice (`IVoice`'s events).
2. **Effects:** taps, pops, the boing of a miss, the sparkle of a right
   answer, the rocket's take-off, stickers, the drum; decompressed on load;
   all through one `SoundPlayer` with the sounds volume.
3. **Haptics:** a light tick on a right answer and a sticker (vibration
   switch respected).
4. **Effects polish:** stardust, the trail, the planet faces' small
   animations, within the "one effect at a time" rule.
5. **The performance pass** on the Tab A7 (with the human: a development
   APK and what to note, as SeliSnake's stress test did): 60 fps on the
   map and in every activity, a lesson ready within 2 seconds, the app
   ready within 5 seconds, the APK size; fixes for whatever misses a
   budget.

## Out of scope — do not build

- Music lessons (`domain-music`).

## Acceptance criteria

- The voice is never covered by music or effects (listening check of a
  whole lesson).
- With sound off in the parent area, nothing plays and every activity
  still works from its pictures.
- The budgets hold on the Tab A7 (the human's numbers recorded in
  Progress).

## Tests required

- EditMode: the ducking rule, `SoundPlayer`'s volume routing.
- PlayMode: a lesson with sound off runs to its end.
- Emulator and the Tab A7: the performance pass.

## Touches

`Scripts/Game/Audio`, `Assets/Scolita/Audio`, prefabs that play sounds
(listed when claimed), `ThirdParty/`.

## Screens

None new.

## Voice lines

None new.

## Before closing out

- Release (the next MINOR): **the MVP is complete.** Update "Where we are"
  with the after-MVP order agreed with the human.

## Progress

_Agent `sound-feel`, worktree `C:\Projects\scolita-sound`, emulator `emulator-5600`._

### Design note (a medium change: a new system, a save format change, a new permission)

**Where the logic lives.** The rules are in Core (`Scripts/Core/Audio`, no UnityEngine); the Unity side
(`Scripts/Game/Audio`) plays clips, sets volumes and vibrates.

- **`SoundCue`** names every effect (`Tap`, `Sparkle`, `StepDone`, `Boing`, `Glow`, `Drum`, `NotYet`, `TakeOff`,
  `Whoosh`, `Landing`, `StarLands`, `GiftOpens`, `StickerPlaced`, `CardTapped`, `BookOpens`); **`MusicPiece`** names
  the music (`Space` on "Who's playing?", the map and the parent area; `Lesson` in a lesson); **`Haptic`** the
  vibrations (`Tick`).
- **`FeedbackEffects`** (one owner): what an answer sounds and feels like, from its kind and
  `FeedbackOutcome`: right → `Sparkle` and a `Tick`; the step done → `StepDone` and a `Tick`; a miss → a soft
  `Boing`; the answer shown → `Glow`; a tap that isn't an answer → nothing (the tapped letter or word is heard, and no
  effect may cover its pure sound), except a tap on `Syllables`' drum, which is the `Drum`. So the activity views stay as they are: the lesson's composition root plays what Core says.
- **`MusicDucking`** (the ducking rule): while the voice speaks (`IVoice.Started` / `Finished`) the music goes
  down to `DuckedLevel` in `DuckSeconds`, waits `HoldSeconds` after the voice ends (so it doesn't pump between two
  lines), then comes back in `RecoverSeconds`. **`SoundLevels`** (the volume routing): the music's volume is
  the music slider × the ducking; an effect's is the sounds slider, × `EffectsUnderVoice` while the voice speaks;
  a slider at 0 plays nothing at all (the source isn't started). The voice keeps its own slider. The numbers
  are in the learning rules asset (`LearningRulesAsset.ToSoundRules`), as every tuning number.
- **The Unity side:** `SoundPlayer` (one per scene that plays sound, as `VoiceHost`; not a singleton): a music
  source (streamed clips) and a small pool of effect sources (no `Instantiate`, no allocation when an effect
  plays), `Play(cue)`, `Vibrate(haptic)`, and `PlayAcrossScenes(cue)` for the take-off (its source outlives the
  map, so the sound isn't cut when the lesson loads). `SoundBank` (a ScriptableObject) maps each cue and piece to
  its clips. `AndroidHaptics` vibrates through Android's `Vibrator` (`VibrationEffect.EFFECT_TICK` on API 29+,
  else a 15 ms one-shot), with the JNI method and argument cached (no allocation per tick); the Editor doesn't
  vibrate. The runners (Map, Lesson, Profiles, Parent) begin it with the tablet's settings and the scene's voice;
  `RewardsSession` plays the rewards' sounds (a star landing and the gift opening are told by the lesson's end
  screen through `IRewardsInput`, the sticker placed by the session itself).
- **The sounds are made for the app** (no third-party pack, so no licence beyond the repo's): **Școlița → Make
  sounds** (`Scripts/Editor/Sounds`) synthesises every effect and the two music loops from a fixed seed (sine
  and bell tones, filtered noise, a small reverb), normalises them with the recording kit's `Loudness` and
  writes mono 44.1 kHz WAVs to `Assets/Scolita/Audio/` (Git LFS) and the bank. The import (set by the maker):
  effects decompressed on load and preloaded, music streamed (Vorbis, mono). Levels against the voice's −16 LUFS: effects about
  −22 LUFS (peaks under −3 dBFS), music about −30 LUFS, so the voice stands above both before any ducking.
  Calm and kind: the miss is a soft low "boing", never a buzzer.
- **The tablet's `settings.json` version 2:** `musicVolume` and `soundsVolume` (0–1) replace `musicOn` and
  `soundsOn` as what is used. A version 1 file is migrated: on → 1, off → 0. `musicOn` and `soundsOn` stay in
  the file, written as "volume above 0" (`JsonUtility` reads only declared fields, and an older version reading
  the file refuses it as newer anyway). The parent area's settings card gets **Muzica** and **Sunetele** sliders
  (beside the voice's) and the **Vibrații** switch; moving a slider is heard at once (the music plays under the
  area; the sounds slider plays a tap when let go), the switch gives a tick when turned on.
- **The VIBRATE permission** (a normal permission, granted at install, no prompt): the brief's haptics need it;
  it is added to `Plugins/Android/LauncherManifest.xml` with the reason, and the changelog says so. INTERNET
  stays removed (BuildScript checks it).
- **The PlayMode tests' save root:** they wrote into the Editor's own save root
  (`%USERPROFILE%\AppData\LocalLow\softshack\Școlița`, shared by every worktree and the human's Editor), and the
  profiles' tests moved its files aside and back. Now every save goes through `SaveRoot.Path`, which is
  `Application.persistentDataPath` except in the Editor when `SCOLITA_SAVE_ROOT` is set; the PlayMode assembly
  sets it to a new temporary folder when the run starts (a test-run callback) and deletes it once the Editor is
  back in Edit mode (`TemporarySaveRootCleanup`, after the last scene has closed and saved), so a test never touches
  the shared root. An agent can set the variable before opening its Editor for Play mode.
- **Effects polish:** the awake and beaming planets on the map breathe softly (the sleepy ones keep still); the
  stardust and the trail stay as `tracing` made them. Effects stay "one
  at a time" and after an answer, never while she listens.
- **The performance pass:** the tester's strip shows the frames per second, the slowest frame and the GC
  allocated per frame (`FrameStats` in Core: averaged over a second, written without allocating), so the human
  can read them on the Tab A7 with no tools; the lesson logs how long it took to be ready.

**How it is tested.** EditMode: `MusicDuckingTests` (the ramp down, the hold, the return, a new line during the
hold), `SoundLevelsTests` (the routing; a slider at 0 plays nothing), `FeedbackEffectsTests` (every outcome of
every kind), `FrameStatsTests`, `TabletSettingsData` version 2 and the migration from version 1, `SoundPlayerTests`
(the volume routing on the real component and its sources), `SoundBankTests` (every cue and piece has its clip,
the import settings, the levels measured). PlayMode: a lesson with sound off runs to its end and nothing plays; a
lesson with sound on plays its music and ducks it while the voice speaks. Emulator and the Tab A7: the
performance pass.

**Another domain.** Music lessons (`domain-music`) reuse `SoundPlayer` for an instrument's sound (a cue or a clip
from the content, never `IVoice`), and `FeedbackEffects` serves any kind; numbers need nothing new.

### Built

- **Core** (`Scripts/Core/Audio`): `SoundCue`, `MusicPiece`, `Haptic`, `FeedbackEffect`, `FeedbackEffects`, `SoundRules`,
  `MusicDucking`, `SoundLevels`; `Scripts/Core/Timing/FrameStats`.
- **Game** (`Scripts/Game/Audio`): `SoundPlayer`, `ISoundEffects`, `SoundBank`, `SoundBankEffect`, `SoundBankMusic`,
  `AndroidHaptics`; `Prefabs/Audio/SoundPlayer.prefab` (a music source, four effect sources) in `Map`, `Lesson`,
  `Profiles` and `Parent.unity`, each runner's `sounds` field. `Saving/SaveRoot` (every save's root; the tests' own).
  `TabletSettingsData` version 2. `LearningRulesAsset.ToSoundRules` (six numbers in `Settings/LearningRules.asset`).
  The hooks: `LessonRunner` (the lesson loop, each answer's sound and tick, a "ready" log line), `MapRunner` (the space
  theme, a tap, "not yet", the whoosh, the landing, the take-off across scenes, an "opens" log line), `ProfilesRunner`
  (the space theme, the card's chime), `ParentRunner` (the space theme, the sliders and the switch heard at once),
  `RewardsSession` (a book opening, a star landing, the gift, a sticker placed with a tick, a trail chosen);
  `IRewardsInput.StarLanded` / `GiftOpened`; `IParentAreaInput.SetMusicVolume` / `SetSoundsVolume` / `SetVibration`;
  `ParentSettingsShown`'s volumes and switch.
- **UI:** `ParentAreaScreen` (**Muzica** and **Sunetele** side by side under **Vocea**; **Vibrații** beside **Meniurile
  în engleză**: the card's rows paired so it still fits 1280 × 800; the two new labels 92 units wide so the sliders
  get room); `LessonEndScreen` tells a star landing and the
  gift opening; `TesterStrip` shows `60 fps · max 17 ms · GC 0 B` once a second (the `Frames` label); `PlanetView`
  breathes. Three keys in the `App` table (`music-volume`, `sounds-volume`, `vibration`), both languages.
- **Build and content:** `Scripts/Editor/Sounds` (`SoundMaker`, Școlița → Make sounds; `Synth`, `EffectRecipes`,
  `MusicRecipes`, `MadeSound`, `SoundStandard`); `Assets/Scolita/Audio/Effects/*.wav` (17 takes of 15 cues),
  `Audio/Music/space.wav` (60 s) and `lesson.wav` (40 s), `Audio/SoundBank.asset`; VIBRATE in
  `Plugins/Android/LauncherManifest.xml`.
- **Tests:** EditMode `Core/Audio/*` (`MusicDuckingTests`, `SoundLevelsTests`, `FeedbackEffectsTests`, `SoundRulesTests`),
  `Core/FrameStatsTests`, `Game/SoundPlayerTests`, `Game/SoundBankTests` (every cue's clips, the import, the levels, the
  files fresh), `Saving/SettingsDataTests` (version 2, the migration); PlayMode `Lessons/SoundFlowTests` (sound off runs
  to the end silently; the music ducks and comes back; the tests' own save root), `TemporarySaveRoot`.

### The sounds: made for the app

No third-party sound: every effect and both loops are synthesised by `SoundMaker` from fixed seeds (sines, bells of a
few partials, a soft marimba, filtered noise, a small Schroeder reverb), so `ThirdParty/` gets nothing and there is no
licence beyond the repo's. Levels, measured by the recording kit's `Loudness` (BS.1770) and checked by `SoundBankTests`:

| Sound | Target | Beside the voice (−16 LUFS) |
| --- | --- | --- |
| Effects (sparkle, step done, boing, drum, landing, stars, gift, sticker, card) | −22 LUFS, peaks ≤ −3 dBFS | 6 dB under it; 10 dB while the voice speaks (× 0.6) |
| Soft effects (tap, not yet, glow, book) | −26 LUFS | 10 dB under it |
| Take-off, whoosh | −24 LUFS | 8 dB under it |
| Music (space, lesson) | −30 LUFS, peaks ≤ −6 dBFS | 14 dB under it at full volume; 24 dB while she hears the voice (ducked to 0.3) |

### How each screen meets "Usability"

- **The parent area's settings** (for adults): the new sliders are 64 px tall like the voice's, the switch's box 56 px
  in a 64 px row; text 20 px; each change is heard (or felt) at once, so a parent knows what it does. At 0 a slider
  turns its sound off entirely.
- **Sound off:** every activity still works from its pictures and the bubble (the PlayMode test plays a whole lesson
  with music and sounds at 0); no sound is the only feedback of anything (each sound goes with the wobble, the
  sparkle, the glow or the bounce already there).
- **Kind:** the miss is a soft low "boing", "not yet" two gentle notes; nothing buzzes or scolds.

### Fresh-eyes review (a second agent; the settings' format, the save root, the sound system)

Findings and what was done:

1. *Blocker: the tests' temporary root came back after it was deleted* (a scene still open saved its play time as it
   closed, into the folder its store had opened), *and closing the scenes inside the run's end crashed the run.*
   Fixed: the folder is deleted once the Editor is back in Edit mode (`TemporarySaveRootCleanup`, Editor only), when
   every scene has closed; an EditMode run gives its folder up at once.
2. *The runs had not passed in full:* one assertion still expected settings version 1, and two lesson tests assumed
   `profiles/child-1/` exists (it always did in the shared root). Fixed (`LessonScene.RestoreFile` makes the folder);
   the full run is below.
3. *A run cut short left stale variables:* fixed: the folder made for the tests has its own variable, a later run
   deletes a leftover one first, and only that folder (in the system's temporary folder, with its mark) is ever
   deleted; a failed delete is a warning.
4. *The callback also runs for EditMode runs:* fine now (it gives the folder up at once there).
5. *`ProjectSettings.asset` changed:* the Test Framework's "run in background" and the build's version, left by a
   crashed run; reverted, never committed (the build rewrites the version on every build).
6. *The In flight row's Touches:* completed.
7. *`AndroidHaptics` could call a missing method:* fixed: the vibrator is kept only once the method and the effect are
   found; everything made is disposed on failure; the JNI exception's reference is deleted.
8. *No test of the parent area's new settings:* added `ParentAreaTests.TestMusicSoundsAndVibration_SavedAndUsedAtOnce`.
9. *The breathing planets re-batch the map's canvas each frame:* measured on the emulator: the map holds 55–60 fps after
   its first second (see below); the scale is reset when a planet is shown again.
10. *The Tab A7 may have no vibration motor:* then nothing ticks there (the switch does nothing); the human's step asks.
11. *Neutral taps played a tap over the letter's own sound:* fixed: a tap that isn't an answer is silent, except
    `Syllables`' drum (now decided by the answer, so the tick tapped before any beat is silent too).
12. *"GC 0 B" where the player can't count:* shows "GC -" then.
13. *Test names; the settings' doc:* fixed.
14. *Romanian:* "Muzica" and "Sunetele" are definite like "Vocea", "Vibrații" isn't ("Vibrațiile" would match): left as
    the parent-area follow-up named it, for the human.
15. *The Test Framework writes `TestResults.xml` into the shared Editor root on every PlayMode batch run* (before this
    brief too): not a save; mine removed after the runs.

Sound, as the review found: the settings' migration (version 1's on and off by the `version` field, the rest kept, a
later version refused and never saved over); no save path left on `Application.persistentDataPath`; the tablet can't be
redirected; Core's audio pure and allocation-free; the per-answer path allocation-free; the JNI calls for Android 8–14;
the sound maker deterministic; the conventions.

### Pushed

- `294f494` `core:` the sound rules: `SoundCue`, `MusicPiece`, `Haptic`, `FeedbackEffects`, `MusicDucking`, `SoundLevels`,
  `SoundRules`, `FrameStats`, their tests (EditMode 830/830).
- `d105e6c` `core:` taps that aren't answers silent, but the drum (the review);
- `4a27621` `game:` `SoundPlayer`, `SoundBank`, `AndroidHaptics`, the runners' hooks, `settings.json` version 2,
  `SaveRoot` and the tests' own save root;
- `2e1860f` `build:` the sound maker (Școlița → Make sounds), VIBRATE, `SoundBankTests`;
- `5b65ed0` `content:` the 17 effects, the 2 loops, the bank;
- `4507831` `game:` the sound player in Map, Lesson, Profiles and Parent; `SoundFlowTests`;
- `058f001` `ui:` the area's music, sounds and vibration, the tester's frame rate, the end screen's sounds, the planets
  breathing; three string keys; then this `docs:` commit.

**Blocked: waiting for the human's Tab A7 numbers** (step 0 in `local/for-human-next-steps.md`: the frames per second on
the map and in each activity from the tester's strip, the app and a lesson timed, one lesson heard, sound off, whether
it ticks). The try APK is `C:\Projects\Scolita\local\apks\Scolita-0.14.1-sound-try-debug.apk` (built on 0.14.1, `969337c`, so steps 0 and 0b share one install; the earlier 0.14.0 sound-try APK is superseded). Next: fix whatever
misses a budget, record the numbers here, then the close-out as the MVP release (the next MINOR from `origin/main`,
the after-MVP order in "Where we are" as the human agrees it).

### Test results

- **The full run** (after the review's fixes, on `294f494` plus this work): EditMode 842/842, PlayMode 63/63,
  `BuildAndroidDebug` fine; no `warning CS` and no error in any log. The APK asks for VIBRATE (granted at install)
  and not INTERNET. Every PlayMode run left no `%TEMP%\scolita-saves-*` folder, and nothing in the Editor's own save
  root changed but the Test Framework's `TestResults.xml` (removed).
- **Emulator** (`emulator-5600`, Pixel Tablet, Android 16, `-feature -Vulkan`, 2000 × 1200):
  - the map, a lesson, the parent area and the tester's strip, with the sound player in each; no error or warning in
    logcat; `dumpsys vibrator_manager` lists a `Prebaked=TICK` from the app for each right answer;
  - **the parent area** at 2000 × 1200: **Muzica** and **Sunetele** side by side, **Vibrații** beside **Meniurile în
    engleză**, the reset button inside the card (screenshots in the agent's scratch folder); the labels narrowed to
    92 units after the first look (the sliders were too short), checked again at 2000 × 1200 and at 2400 × 1080 (the
    phone: every row inside the card, the headings closer together, as before this brief);
  - **the performance numbers** (the tester's strip; the emulator is slower than the tablet: function, not smoothness):
    the map **60 fps, slowest frame 17 ms, GC 0 B** once settled (47 fps, 50 ms in its first second, while it loads);
    a lesson (`MeetLetter`) **60 fps, 17 ms, 0 B**, and **57 fps, 33 ms, 0 B** in the second a right answer played its
    sparkle and tick;
  - **a lesson ready** 1.07 s after **Hai!** the first time (logcat `Map: ro-a opens` → `Lesson: ro-a is ready`),
    0.24 s the second time (budget 2 s);
  - **the app ready** in about 10 s from a cold start on the emulator (the first frame at 1.6 s; Unity's engine start
    4.6 s, Boot 1 s, the map's scene 2 s, its start 0.7 s); the sound adds almost nothing to it (17 small effects
    decompressed, the music streamed). The Tab A7's own number is the human's step;
  - **the APK** 58 MB of content (59.5 MB on disk; 0.14.0 was 60.3 MB with the same content but for the sounds: the
    sounds add about 0.6 MB). An incremental build can leave holes in the APK (85 MB on disk once): the try APK is
    packaged clean.
- **Listening by measuring** (the emulator has no tablet voice): the levels in the table above, checked by
  `SoundBankTests` on every run, and the ducking measured in PlayMode (the music at 0.3 of its volume while the
  voice speaks, back to full after). Nobody has heard them yet: the check by ear is the human's step.
- **Not run:** the Editor's Device Simulator and the Profiler window (batch mode and the emulator only; the tester's
  strip gives the same numbers, from `ProfilerRecorder`); the 1280 × 800, 2560 × 1600 and 2048 × 1536 sizes of the
  parent area (the card is on the 1280 × 800 canvas, its preferred height, 652 of about 668 units, was
  measured in the prefab, and the phone, the tightest, was seen); a display cut-out; the drum, the stars and the gift
  and a sticker placed on the emulator (covered by `FeedbackEffectsTests`, `SoundPlayerTests` and the end screen's
  calls; heard in the human's step); the migration from an installed 0.14.0 (covered by `SettingsDataTests` and
  `SaveStoreTests`).

### Still to test

- **After 0.14.1** (`tablet-feedback-1`: tracing tolerances, the pen hop, the voice waiting for Android's TTS), which
  landed on top of this work: the rebase was a fast-forward (no conflict; `LearningRules.asset` keeps both the new
  `traceFingerAllowance` and the six sound numbers; `VoiceRouter` still raises `Started` and `Finished`, which the
  ducking follows). Rerun: EditMode 848/848, PlayMode 64/64, `BuildAndroidDebug` fine, no `warning CS`, no temporary
  save root left. **Not rerun on the emulator:** the ducking while the tablet's voice waits to get ready (Android's TTS,
  absent on the emulator), and the sound, frame-rate and timing checks above: they go with the human's step 0 and the
  full run before the close-out.

- **The Tab A7** (the human's step in `local/for-human-next-steps.md`): 60 fps on the map and in each activity, a lesson
  ready within 2 s, the app ready within 5 s, any stutter; one whole lesson heard (the voice never covered); the sound
  off; whether it ticks. Then fixes for whatever misses a budget, and the close-out.


## How to test locally

### Core, game, build, content and UI slices (the sound rules, the sound player in every scene, the sounds, the settings, the tests' own save root)

**Prerequisites.** Unity 6000.3.29f1; the worktree's `game/` folder; an emulator (here `emulator-5600`, AVD
`Scolita_Tablet_tracing`, Pixel Tablet, Android 16, x86_64, started with `-feature -Vulkan`, `wm size` 2000x1200). In
Git Bash (Windows paths for Unity's arguments; `MSYS_NO_PATHCONV=1` passes them as they are):

```bash
UNITY="/c/Program Files/Unity/Hub/Editor/6000.3.29f1/Editor/Unity.exe"
export MSYS_NO_PATHCONV=1 ADB_LOCAL_TRANSPORT_MAX_PORT=5699
ADB="$LOCALAPPDATA/Android/Sdk/platform-tools/adb.exe"; EMU=emulator-5600; S=C:/<your scratch folder>
"$UNITY" -batchmode -nographics -quit -projectPath game -executeMethod Scolita.Editor.BuildScript.BuildAndroidDebug -logFile $S/build.log
"$ADB" -s $EMU install -r game/Builds/Android/Scolita-0.14.0-debug.apk
"$ADB" -s $EMU shell am start -S -n com.softshack.scolita/com.unity3d.player.UnityPlayerGameActivity -e unity -force-gles
```

**Remaking the sounds.** Școlița → Make sounds (or `-executeMethod Scolita.Editor.Sounds.SoundMaker.MakeFromCommandLine`)
writes the same 17 effects and 2 loops again from their recipes (`SoundBankTests.TestMake_TheFilesAreFresh` fails if a
recipe and its file differ).

**Automated tests.** EditMode (`-testPlatform EditMode`) and PlayMode (`-testPlatform PlayMode`, about six minutes),
results in `$S/editmode.xml` and `$S/playmode.xml`. The sound ones: EditMode `Core/Audio/*`, `Core/FrameStatsTests`,
`Game/SoundPlayerTests`, `Game/SoundBankTests` (the clips, the import, the levels, the files fresh),
`Saving/SettingsDataTests` (version 2 and the migration); PlayMode `Lessons/SoundFlowTests` (sound off: a lesson to its
end and nothing plays; sound on: the music ducks to 0.3 under the voice and comes back; the tests' own save root) and
`Parent/ParentAreaTests.TestMusicSoundsAndVibration_SavedAndUsedAtOnce`. Every PlayMode run saves into its own
`%TEMP%\scolita-saves-<id>`, deleted when the Editor is back in Edit mode; the Editor's own save root is never touched
(the Test Framework itself still writes `TestResults.xml` there).

**Steps** (on the emulator, which has no tablet voice: every line shows in the bubble; its sound is the PC's).

1. **The map.** The space theme fades in over 2 s; the planets that are awake breathe slowly. Tap the awake planet: a
   soft tick and the greeting; tap a sleeping planet or a faint stop: two soft notes ("not yet") and its wobble.
2. **A lesson.** Tap **Hai!**: the take-off sound (it carries on as the lesson opens); logcat has
   `Map: ro-a opens, … ms` and `Lesson: ro-a is ready, … ms` (1.07 s apart cold, 0.24 s warm, on this emulator). The
   lighter lesson loop plays, lower while the bubble's line is said. Tap the letter card: a sparkle; a wrong letter in
   the next game: a soft "boing" and its wobble; the second miss: a gentle bell and the glow.
   `"$ADB" -s $EMU shell dumpsys vibrator_manager | grep scolita` lists a `Prebaked=TICK` per right answer.
3. **The drum** (`ro-words-1`, the drum game, as the tester): each tap on the drum is a drum beat; the green tick tapped
   before any beat makes no sound.
4. **The end of a lesson:** each star chimes as it lands (three steps up), the gift box twinkles as it opens; in the
   sticker book a sticker placed pops and ticks.
5. **The parent area** (hold the lock 3 s, tap the number in digits): **Setări** has **Vocea**, then **Muzica** and
   **Sunetele** side by side, then **Meniurile în engleză** and **Vibrații** side by side, and the reset button, all
   inside the card. Move **Muzica** to 0 and let go: the music stops; back up: it fades in. Move **Sunetele** and let go:
   a tick at the new volume. Turn **Vibrații** off and on: a tick when on. `"$ADB" -s $EMU shell cat
   /storage/emulated/0/Android/data/com.softshack.scolita/files/settings.json` shows `"version": 2` with `musicVolume`
   and `soundsVolume`.
6. **Sound off.** Music and sounds at 0: a whole lesson plays to its end from its pictures and the bubble, and nothing
   plays.
7. **The tester's numbers.** Profiluri → **Fă un profil de test** → **Joacă ca tester**: the strip shows `60 fps · max
   17 ms · GC 0 B` once a second (on this emulator: the map 60 fps after its first second, a lesson 57–60 fps, GC 0 B).
8. **An older tablet.** Install 0.14.0 (`C:\Projects\Scolita\local\apks\Scolita-0.14.0-debug.apk`), open the parent area
   once (its `settings.json` is version 1), then this build over it: the settings read as before (music and sounds
   full), and the file becomes version 2 when a setting changes.

**Cleanup:** `"$ADB" -s $EMU shell pm clear com.softshack.scolita`; `"$ADB" -s $EMU shell wm size reset`.
