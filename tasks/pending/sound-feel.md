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
  `Boing`; the answer shown → `Glow`; a tap that isn't an answer → `Tap`, except on `Syllables`' drum, which is the
  `Drum`. So the activity views stay as they are: the lesson's composition root plays what Core says.
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
  writes mono 44.1 kHz WAVs to `Assets/Scolita/Audio/` (Git LFS) and the bank. The import (`SoundImporter`):
  effects decompressed on load, music streamed (Vorbis). Levels against the voice's −16 LUFS: effects about
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
  sets it to a new temporary folder when the run starts and deletes it when it ends (a test-run callback), so a
  test never touches the shared root. An agent can set the variable before opening its Editor for Play mode.
- **Effects polish:** the planets on the map breathe softly (awake and beaming; the sleepy ones bob slower), the
  rocket wobbles before the take-off; the stardust and the trail stay as `tracing` made them. Effects stay "one
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

### Pushed

- `core:` the sound rules: `SoundCue`, `MusicPiece`, `Haptic`, `FeedbackEffects`, `MusicDucking`, `SoundLevels`,
  `SoundRules`, `FrameStats`, their tests (EditMode 830/830).

### Still to test

- Nothing yet beyond EditMode (the core slice is used by nothing until the game slice).
