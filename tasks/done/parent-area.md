# Task: feature/parent-area

> **Status: Done 2026-11-25, released as 0.13.0.**

Read `AGENTS.md` first, then `design/README.md` and `design/screens.md` (build the screens as designed there) (especially "Security and privacy" → the parent
gate, "Fresh-eyes review"), and the completion records in `tasks/done/`.

**Feature:** `feature/parent-area`
**Implements:** PRD §3.7; `docs/curriculum.md` §1 (little and often).
**Depends on:** `progress-review`.

## Scope

1. **`ParentGate`:** press and hold the parent button for 3 seconds (a
   ring fills), then the app says a number from 11 to 19 and shows three
   numbers written as words in the app's language; the right one opens
   the area, a wrong one closes the gate quietly. Numbers in the rules
   asset. A fresh-eyes review checks a child can't open it by tapping
   around.
2. **What she has learned:** a grid of the track's letters by language
   (new: grey; learning: half-filled; learned: full, with its star or
   circle badge), the days played in the last two weeks, the next lessons.
3. **Settings:** the language mix (D1), the app's own language, letter
   case first (D2), the session limit (D8: off, 10, 15, 20 minutes), music,
   sounds and voice volume, vibration, the version.
4. **The session limit:** when reached, the current lesson finishes, then
   the mascot yawns ("Racheta se odihnește" / "The rocket needs a rest")
   and the map sleeps until the next calendar day or until a parent opens
   it again through the gate.
5. **Voice status:** recorded lines per language, out of the total.
6. **Reset progress:** asks twice, saying what will be lost; keeps the
   settings.
7. **Fill in "The basics"** in `docs/manual-testing.md`: a whole lesson
   from the map and back, the gate, a setting changed.
8. Android Back does what the on-screen Back does on every screen.

## Out of scope — do not build

- Managing profiles (`profiles`, next: it adds the profile list, "Who's
  playing?" and the tester profile to this area), exporting progress,
  notifications. Build this area so the settings marked "per profile" in
  PRD §3.8 (the language mix, letter case first, the session limit) are
  read from the current profile's folder, and the rest from the tablet's
  settings.

## Acceptance criteria

- The gate opens only with the hold and the right number; random taps for
  a minute never open it (a PlayMode monkey test).
- Changing the mix to English only hides the Romanian lessons at once.
- The session limit never ends a lesson in the middle.
- Every screen meets "Usability": 64 dp targets, the parent area's text at
  least 18 sp (it is for adults), a way back everywhere.

## Tests required

- EditMode: the gate's rules, the session limit, the grid's states.
- PlayMode: the gate (right, wrong, the monkey test), each setting takes
  effect, reset.
- Emulator: every screen at every size in the table, in both app
  languages.

## Touches

`Scripts/Core/Parent`, `Scripts/UI/Parent`, the map's parent button, new
prefabs, the `settings` save file (new fields, migrated), the string
tables.

## Screens

The gate, the progress grid, the settings, reset.

## Voice lines

The gate's numbers, the session's end, both languages.

## Before closing out

- Release (the next MINOR). The first APK for her comes with `profiles`. Add to
  `local/for-human-next-steps.md`: install it, run "The basics" (on a
  test profile once `profiles` exists).

## Progress

_Agent `parent-area`, worktree `C:\Projects\scolita-parent`, emulator `emulator-5600`._

### Design note (a medium change: a new system, four screens, a new save file, a save migrated, the first string tables)

**Where the logic lives.** Every rule is in Core (`Scripts/Core/Parent`, no UnityEngine); the
Unity side draws it, forwards the taps and saves.

- **`ParentGate`** (`ParentGateRules`: the hold's seconds, the lowest and highest number, how many
  choices; all in `LearningRules.asset`: 3 s, 11–19, 3): the hold fills while the button is held
  (`Hold(seconds)`, the frame's time from the Unity side) and empties when it is let go; when full,
  the gate asks: a number from the range (a seeded `IRandom`) among that many different numbers in
  a shuffled order. `Choose(number)` opens it (right) or closes it (wrong); a closed or opened gate
  must be held again. No tap can open it: only a hold of the whole time, then the right word.
- **`DailyPlay`** and **`SessionLimit`**: the time played today (per profile, on the tablet's
  calendar day), counted while the map or a lesson is on screen and the app is in front; the
  limit's choices (off, 10, 15, 20 minutes; the minutes in the rules asset, off by default, D8).
  It is reached when the limit is on and today's time reaches it; a new day starts from zero; a
  parent who passes the gate wakes the rocket (today starts again from zero). A lesson never asks:
  it always plays to its end. The map asks when it opens and while it is shown (so a limit
  reached on the map puts it to sleep at once, one reached in a lesson after "Mai departe").
- **`ParentReport`**: what the grid shows, from `LearnerProgress` and the curriculum: per
  language, its track's letters in teaching order with their `ItemState` (new, learning,
  learned); the last 14 days, played or not (`DaysPlayed`); the next 3 lessons of her order not
  done (their planet and new letter, or "practice"); and the voice status per language: the
  lines recorded out of the lines to record (a line said by its sounds needs no recording).
- **`ProgressReset`** (Game): deletes the profile's `progress.json`, `map-progress.json`,
  `lesson-run.json` and, once `rewards` is on `main`, its rewards file, together (or the
  progress is made again from the older ones); keeps the learning settings, the play time and
  every damaged copy (they are for rescuing her progress by hand). `SaveStore` gets `Delete(name)`.

**The saves.**

- `learning-settings.json` **version 2** adds `sessionLimitMinutes` (0 is off). A version 1 file
  reads as off (D8); a number that is not one of the rules' choices is kept in the file and read
  as off, so a later version can use it. The language mix stays.
- **`play-time.json`** (new, per profile, version 1): `day` (`2026-11-25`) and the `seconds`
  played that day. A damaged file is set aside, dated, and read as nothing played (so the rocket
  is awake: never put to sleep by a damaged file). Saved when a step ends, when the map or a
  lesson closes and when the app goes to the background, never per frame.
- The tablet's `settings.json` keeps its shape: the app's language and the voice's volume are
  already in it.

**The screens** (for adults: text at least 20 px on the 1280 × 800 canvas, every target 64 px
or more, a way back everywhere; design screen 7):

- **The parent button** (a small lock in the map's `ParentPlace`, top right): press and hold,
  a ring fills in 3 s. Let go early and the ring empties; nothing else happens.
- **The gate** (a panel over the map): "Pentru părinți", "Atinge numărul pe care îl auzi.", a
  listen button and three numbers written as words in the app's language. The number is said in
  the app's language through `IVoice` and is **never shown in the mascot's bubble** (it would give
  the answer away; the panel covers the map). If no voice can say it (no clip, and no tablet
  voice for the language), the panel shows the number in digits instead, so a parent is never
  locked out. Wrong: the panel closes, without a sound. Its close button and Android Back close
  it.
- **The parent area** (its own scene, `Parent.unity`, with `ParentRunner` as its composition
  root and `Prefabs/Parent/ParentArea.prefab`): **Ce a învățat** (the legend, a row per language
  with the track's letters, the days played, the next lessons, the voice status) and **Setări**
  (the language mix, the daily limit, the voice's volume, the app's language, the version).
  Every change is saved at once; the map reads it when it opens again (an English only mix hides
  the Romanian lessons at once).
- **Reset:** "Șterge progresul…" asks twice, saying what will be lost (the letters learned, the
  lessons done, the place on the map, the stars and stickers) and that the settings stay.
- **The sleeping map:** when the limit is reached, the mascot yawns (its sleepy pose, `look`'s
  art) and says „Racheta se odihnește. Ne vedem mâine!” / "The rocket needs a rest. See you
  tomorrow!"; Go and the stops don't open lessons (a tap says the line again); the parent button
  still works.
- **Android Back** does what the screen's Back does: the parent area goes back to the map, a
  reset question says no, the gate closes, a lesson goes home; the map has no Back, so Back does
  nothing there.

**The string tables** (the first ones; the Localization package came with `project-skeleton`,
no new package): the locales `ro` and `en-GB` and one table collection, `App`, under
`Assets/Scolita/Localization/` (the parent area's words, the gate's number words, and "Hai!" /
"Go!", moved from `MapScreen`'s two strings). The app's language (`settings.json`) selects the
locale at boot and when the parent changes it. Localization keeps its tables in Addressables,
which it brought: local groups only, built with the player, no remote catalog (still offline;
the launcher manifest already removes the INTERNET permission their networking code brings, and
`BuildScript` checks every APK). The cedilla test reads every table; a new test checks every key
has both languages.

**Voice lines** (new rows at the end of `voice-lines.csv`, no clips yet): `<lang>-line-gate-11`
to `-19` („Atinge numărul unsprezece.” / "Tap the number eleven.", …) and
`<lang>-line-rocket-rests`. A test checks each gate line says its table's number word.

**Not built, and why** (deviations from the brief, for the human):

- **"Literele mici întâi" (letter case first, D2):** nothing in the content can play capitals
  first (the capitals are their own lesson after each planet's letters, as the curriculum
  says); a switch would do nothing, and capitals first would change the curriculum's order.
  Asked in `local/for-human-next-steps.md` (keep lower case first only, or what capitals first
  should do).
- **The music and sounds sliders and the vibration switch:** nothing plays music or effects or
  vibrates yet; `sound-feel` adds them with what they control (`settings.json`'s `musicOn` and
  `soundsOn` become volumes then). The voice slider works now (the clips and the tablet's voice).
- **An English only mix never meets the capitals lessons** (they are Romanian;
  `content-letters-1`'s note): noted, not fixed here.

**Waits for `rewards`** (it holds `MapScreen.prefab`, `Map.unity`, `MapRunner`, `LessonRunner`):
the parent button and the gate on the map, the sleeping map, counting play time in the runners,
Go's word from the table, the voice's volume on the map and in a lesson, the rewards file in the
reset, and Android Back in a lesson. Until then the parent area is on `main` but unreachable, so
no button leads nowhere.

**Tests.** EditMode: `ParentGateTests` (hold, let go, the numbers in range and different, right,
wrong, held again), `DailyPlayTests` and `SessionLimitTests` (off, reached, a new day, woken,
an unknown choice), `ParentReportTests` (the grid's states on the real content, the 14 days, the
next lessons, the voice status), `LearningSettingsDataTests` (version 1 read as off, an unknown
limit kept), `PlayTimeDataTests`, `SaveStoreTests` (delete), `ProgressResetTests`, the string
tables (both languages, the gate's words in its lines). PlayMode: the gate (right, wrong, let go
early, the monkey: a minute of random taps on the map never opens it), each setting saved and
read back by the map (English only hides the Romanian stops), reset (the files gone, the
settings kept), the session limit (the lesson plays to its end; the map sleeps; the gate wakes
it). Emulator: every screen at every size, in both app languages.

**Another domain.** The gate and the limit know nothing of letters. The grid reads a track's
items: a numbers domain's track gives its numbers the same way (an `ItemKind` each, as
`progress-review` set out); the next lessons and the voice status are per lesson and per line.

### Built

- **Core** (`Scripts/Core/Parent`): `ParentGate` (+ `GateStage`, `ParentRules`: a 3 s hold, 11–19, three choices,
  and a 30 s rest after a wrong number, added after the review), `DailyPlay`, `SessionLimit`, `ParentReport`
  (`LetterRow`, `LetterCell`, `NextLesson`), `VoiceStatus` (`LanguageVoiceStatus`); `MapLines.Resting`.
- **Game:** `Scripts/Game/Parent` (`ParentRunner`, the Parent scene's root; `ParentGateFlow` with `IParentGateInput`
  and `ParentGateView`; `ParentAreaView`, `IParentAreaInput`, `ParentSettingsShown`, `ResetQuestion`;
  `PlayClock` and `PlayTimeData`; `ProgressReset`), `SaveStore.Delete`, `LearningSettingsData` version 2,
  `AppLocale` (the locale and the tables' words), `AndroidBack`, `AppScene.Parent`, `LessonVoice.Silence`,
  `RewardsSession.IsBookOpen`, `VoiceHost.SetVolume` (clips and the tablet's voice), `GuideMood.Sleepy`; `MapRunner`
  (the gate, play time, resting, Back) and `LessonRunner` (play time, the voice's volume, Back); `BootLoader` selects
  the app's language.
- **UI:** `Scripts/UI/Parent` (`ParentAreaScreen`, `ParentGatePanel`, `ParentHoldButton`, `PillButton`,
  `LetterCellView`, `SliderRelease`); `Prefabs/Parent/ParentArea.prefab` and `ParentButton.prefab` (in
  `MapScreen.prefab`'s `ParentPlace`, with the gate's panel at the top of the map's canvas); `Scenes/Parent.unity`;
  the guide's sleepy frames; Go, the bar's words, "Mai departe", "În cartea mea" and "Bravo!" from the string tables.
- **The string tables** (`Assets/Scolita/Localization/`): locales `ro` and `en-GB`, table `App` (58 keys), preloaded;
  `Assets/AddressableAssetsData/` local groups only, built with the player, no remote catalog, catalog updates off.
- **Content:** 20 voice lines at the end of `voice-lines.csv` (`<lang>-line-gate-11` … `-19`,
  `<lang>-line-rocket-rests`), no clips.

### How each screen meets "Usability"

For adults (the brief): text 18 to 36 px on the 1280 × 800 canvas (the note under the limit is 18, the rest 20 and
up), every target 64 px or more (pills, buttons, the switch's whole row, the slider's 64 px bar with a 44 px knob;
the lock 88 px; the gate's words 104 px tall), a back button top left on the area and top right on the gate, and
Android Back on every screen; the language rows show a badge with its shape and its name, not colour alone; the
grid's states differ by fill (full, half, outline) and not only by colour. The child's side: the sleeping map's line
is spoken and shown in the bubble, as every line is.

### Fresh-eyes review (a second agent; the gate and the save formats)

Findings and what was done:

1. *A child who has seen the hold wins one time in three by guessing.* Fixed: a wrong number rests the gate for 30 s
   (`ParentRules.GateRestSeconds`, in the rules asset); the hold doesn't fill meanwhile. The close button doesn't rest
   it.
2. *The numbers 11–19 mostly start with different letters, which the app teaches.* For the human (a range such as
   21–29 whose words share a start needs new lines): in `local/for-human-next-steps.md`.
3. *The hold could fill after a return from the background, or in one long frame.* Fixed: the map lets go of the
   hold when the app goes to the background, and a frame counts as 0.1 s of hold at most (a PlayMode test).
4. *A voice volume of 0 would lock the parent out* (the number said silently, so no digits). Fixed: the slider's
   lowest value is 0.2.
5. *A reset that fails is silent.* Fixed: "Progresul nu a putut fi șters de tot. Încearcă din nou."
6. *A setting that can't be saved still looks saved.* Fixed: it stays as it was, with a warning.
7. *Play time rounded to whole milliseconds per frame.* Fixed: counted in ticks.
8. *A map left open past midnight stays on the old day until it is reopened.* Left as it is (noted): any reopening
   (the next lesson, the app coming back) reads the new day.
9. *Any pass through the gate wakes the rocket, even to look.* As the Design note says; for the human.
10. *XML docs naming the lesson as a caller.* Fixed.
11. *`AddressableAssetsData` must be committed.* It is, in the `build:` commit.
12. *Romanian:* "Restul îl spune vocea tabletei." (restul is singular), and the reset's list now says "dârele" /
    "the trails". The rest checked fine.

Sound, as the review found: the gate opens only from asking; a finger sliding off or a second one lifting lets go;
the number never shows where a child can use it (the bubble silenced, the panel opaque, digits only without a voice);
no other way into the Parent scene; the save formats (version 1 read as no limit, unknown values kept, damaged
files never lock anyone out); nothing allocates per frame.

### Pushed

- `e0acc94` `core:` the gate, the daily limit and the report; `b54d91f` `core:` the gate's rest after a wrong
  number; `c20e0d3` `content:` the 20 lines; `5250ba1` `core:` the map's rest line; `7b5a9c3` `build:` Localization,
  its local Addressables, the string tables, and `BuildScript` switching to Android; `c24c8e8` `game:` the runner,
  play time, settings version 2, reset, the voice's volume, Back; `bc60eaa` `ui:` the gate, the parent area and the
  sleeping map; then this Progress.

### Test results

- **Batch mode on the final tree:** EditMode 752/752, PlayMode 49/49 (the monkey test a full minute of real time),
  `BuildAndroidDebug` exit 0 (the APK asks for no INTERNET: `BuildScript`'s check), no `warning CS` and no error in
  any log. The commits before `bc60eaa` were not run one by one (each compiles by construction: the Game side's
  contracts come before the UI that implements them); the Core commits' EditMode tests passed with them.
- **A bug the emulator found:** the first APK showed a black map with Addressables errors: its bundles were built for
  `StandaloneWindows64` (a fresh worktree opens on the desktop target), so `BuildScript` now switches the Editor to
  Android first (`7b5a9c3`), and `AppLocale` logs and carries on if the tables can't load. Any clean checkout (the
  human's release builds too) needed this.
- **Emulator** (`emulator-5600`, Pixel Tablet, Android 16, `-feature -Vulkan`, `wm size`): the lock (a tap and a 1.2 s
  hold do nothing; a 3.8 s hold opens the gate), the gate in digits ("Atinge numărul 18." — no tablet voice), a wrong
  number closing quietly and the 30 s rest (a hold during it opens nothing, after it the gate asks again), the right
  number opening the area, "Doar engleză" and Android Back (the map shows only English lessons), "Meniurile în
  engleză" (the area, "Go!" and "My sky" in English), the sleeping map at 10 min (the guide asleep, „Racheta se
  odihnește. Ne vedem mâine!” in the bubble, Go opening nothing), reset (two questions, then `progress.json` made
  again empty, `rewards.json` and `lesson-run.json` gone, the settings kept, "Progresul a fost șters."), Android Back
  in a lesson (home to the map). Sizes: the map at 2000 × 1200, 1280 × 800, 2048 × 1536, 2400 × 1080 and 2560 × 1600;
  the gate and the area at 2000 × 1200 and 2400 × 1080 (the phone, the tightest: everything fits); screenshots in the
  agent's scratch folder. Enough because both screens are anchored to a 1280 × 800 canvas matched half by width, half
  by height, and the phone is the narrowest height.
- **Not run:** the Device Simulator and the Profiler window (batch mode only; the gate and play clock allocate nothing
  per frame by construction, reviewed), a display cut-out profile, listening by ear (no clip and no tablet voice on
  the emulator: the lines were read in the bubble), two real fingers on the lock.

### Decisions

- **The parent area is its own scene** (`Parent.unity`), reached only from the gate on the map; the gate's panel is
  on the map, so a wrong number closes quietly without reloading the map.
- **Play time** counts while the map or a lesson is on screen and the app is in front; the parent area doesn't
  count. A lesson never checks the limit; the map checks when it opens and every frame.
- **Back on the map** closes the gate or an open book, else does nothing (the map has no back button).
- **The lesson's "Ascultă" labels** on the activity views stay in the prefabs (they follow the step's language, a
  later move into the tables with `TextIn`).

## How to test locally

### Game and UI slice (the gate, the parent area, the daily limit, the string tables)

**Prerequisites.** Unity 6000.3.29f1; the worktree's `game/` folder; an emulator (here `emulator-5600`, AVD
`Scolita_Tablet_tracing`, Pixel Tablet, Android 16, x86_64, `-feature -Vulkan`). In Git Bash:

```bash
UNITY="/c/Program Files/Unity/Hub/Editor/6000.3.29f1/Editor/Unity.exe"
ADB="$LOCALAPPDATA/Android/Sdk/platform-tools/adb.exe"; EMU=emulator-5600; S=<your scratch folder>
"$UNITY" -batchmode -nographics -quit -projectPath game -executeMethod Scolita.Editor.BuildScript.BuildAndroidDebug -logFile $S/build.log
"$ADB" -s $EMU install -r game/Builds/Android/Scolita-<version>-debug.apk
"$ADB" -s $EMU shell am start -S -n com.softshack.scolita/com.unity3d.player.UnityPlayerGameActivity -e unity -force-gles
F=/storage/emulated/0/Android/data/com.softshack.scolita/files/profiles/child-1
```

**Automated tests.** EditMode (`-testPlatform EditMode`) and PlayMode (`-testPlatform PlayMode`, about four minutes:
the monkey test alone takes a minute of real time), results in `$S/editmode.xml` and `$S/playmode.xml`. The parent
ones: EditMode `Core/Parent/*` (the gate, the limit, the report, the voice status), `PlayTimeDataTests`,
`ProgressResetTests`, `PlayClockTests`, `SettingsDataTests` (version 1 → 2), `AppStringTableTests`,
`LearningRulesAssetTests`; PlayMode `Parent/ParentGateTests`, `ParentAreaTests`, `SessionLimitTests`.

**Steps** (on the emulator at 2000 × 1200 unless said; `"$ADB" -s $EMU shell wm size 2000x1200`):

1. **The parent button.** The map opens; top right, a small lock in a translucent circle. Tap it, and tap it again
   quickly: nothing opens (the ring flickers at most). Press and hold it
   (`"$ADB" -s $EMU shell input swipe <x> <y> <x> <y> 3500`, the lock's centre): a yellow ring fills around the lock in
   3 seconds, then a lilac page covers the map: "Pentru părinți", "Atinge numărul pe care îl auzi.", "Ascultă din
   nou" and three numbers written as words (e.g. „paisprezece”, „unsprezece”, „optsprezece”).
2. **The number said.** With a tablet voice for Romanian, the number is said („Atinge numărul paisprezece.”); the
   emulator has none, so the question shows the digits instead: "Atinge numărul 14." Never in the mascot's bubble.
3. **Let go early.** Hold the lock 1 second (`input swipe x y x y 1000`): the ring empties, nothing opens.
4. **A wrong number.** Hold again, tap a word that isn't the number: the page closes without a sound, the map is as
   it was, and the lock must be held again.
5. **Close.** Hold again, tap the back arrow top right (or Android Back, `input keyevent KEYCODE_BACK`): it closes.
6. **The right number** opens the parent area: "Pentru părinți", "Versiunea …" top right; left, "Ce a învățat"
   (the legend, Română and Engleză with their badges and their letters, the 14 days with today green after a lesson,
   "Urmează" with the next three lessons, "Vocile înregistrate" with the counts); right, "Setări".
7. **The language mix.** Tap "Doar engleză", then Back: the map shows only the English lessons and „both” parties
   (the rocket on `en-a` for a new profile). Back in the area, "Amândouă" brings the Romanian ones back.
8. **The daily limit.** Tap "10 min". To try it without waiting ten minutes, write today's play:
   `"$ADB" -s $EMU shell "echo '{\"version\":1,\"day\":\"$(date +%F)\",\"seconds\":600}' > $F/play-time.json"`, then
   restart the app: the mascot sleeps (its sleepy pose) and says „Racheta se odihnește. Ne vedem mâine!”; Go and the
   stops say it again and open nothing. The lock still works: passing the gate wakes the rocket (back on the map, Go
   opens a lesson). With 590 seconds written, start a lesson: it plays to its end; "Mai departe" brings the map back
   asleep.
9. **The voice.** Drag "Vocea" to the left and let go; the next lines are quieter (clips and the tablet's voice).
10. **Meniurile în engleză.** Turn it on: every word of the area turns English at once ("For parents", "Settings",
    "Version …"); Back: Go says "Go!", the bar "My sky", "Stickers", "Trails"; a lesson's end "Keep going" and "In my
    book" (its "Bravo!" / "Well done!" follows the lesson's language, not the menus). Turn it off again.
11. **Reset.** "Șterge progresul…" asks „Ștergi progresul?” saying what is lost and that the settings stay; "Nu,
    păstrează-l" closes it; "Da, șterge" asks „Sigur?”; "Șterge tot" deletes it: "Progresul a fost șters.", every
    letter new, the next lesson `ro-a`; `"$ADB" -s $EMU shell ls $F` shows no `progress.json`, `lesson-run.json` or
    `rewards.json`, and `learning-settings.json` keeps the mix and the limit. Android Back at a question says no.
12. **Android Back** on every screen: in the area it goes back to the map; on the map it closes the gate or an open
    book (sky, stickers, trails) and otherwise does nothing; in a lesson it goes home to the map (or closes the sticker
    book opened from the end).
13. **A damaged save.** `"$ADB" -s $EMU shell "echo nonsense > $F/play-time.json"` and restart: the map is awake, the
    file is set aside as `play-time.broken-<date-time>.json`. `learning-settings.json` from 0.12.0
    (`{"version":1,"languageMix":"en"}`) reads as English only with no limit, and is saved back as version 2.
14. **Sizes.** `wm size` 2000x1200, 1280x800, 2560x1600, 2048x1536, 2400x1080: the lock, the gate's page and the parent
    area fit, nothing under the safe area's edges.

**Cleanup:** `"$ADB" -s $EMU shell pm clear com.softshack.scolita`, `"$ADB" -s $EMU shell wm size reset`.

## Completion record

_Finished 2026-11-25 by `parent-area`; released as 0.13.0 (tag `v0.13.0`)._

### What was built

- **Pushed:** `1a85b41` `docs:` (Design note), `e0acc94` `core:`, `b54d91f` `core:` (the gate's rest), `c20e0d3`
  `content:`, `5250ba1` `core:` (the map's rest line), `7b5a9c3` `build:`, `c24c8e8` `game:`, `bc60eaa` `ui:`,
  `14447d4` `docs:` (Progress), then this close-out (tagged).
- **Core** (`Scripts/Core/Parent`): `ParentGate`, `GateStage`, `ParentRules`, `DailyPlay`, `SessionLimit`,
  `ParentReport` (`LetterRow`, `LetterCell`, `NextLesson`), `VoiceStatus` (`LanguageVoiceStatus`); `MapLines.Resting`.
- **Game:** `Scripts/Game/Parent` (`ParentRunner`, `ParentGateFlow`, `ParentGateView`, `IParentGateInput`,
  `ParentAreaView`, `IParentAreaInput`, `ParentSettingsShown`, `ResetQuestion`, `PlayClock`, `PlayTimeData`,
  `ProgressReset`); `SaveStore.Delete`; `LearningSettingsData` version 2; `Settings/AppLocale`; `Scenes/AndroidBack`,
  `AppScene.Parent`; `VoiceHost.SetVolume`; `LessonVoice.Silence`; `RewardsSession.IsBookOpen`; `GuideMood.Sleepy`;
  `MapRunner` (the gate, play time, resting, Back), `LessonRunner` (play time, the voice's volume, Back),
  `BootLoader` (the app's language); the rules asset's gate and limit numbers.
- **UI:** `Scripts/UI/Parent` (`ParentAreaScreen`, `ParentGatePanel`, `ParentHoldButton`, `PillButton`,
  `LetterCellView`, `SliderRelease`); `Prefabs/Parent/ParentArea.prefab`, `ParentButton.prefab` (in `MapScreen.prefab`'s
  `ParentPlace`; the gate's panel on its own canvas, sorting 40, over the map); `Scenes/Parent.unity` (in the build);
  `MapScreen` (the gate, `ShowResting`, Go from the tables), `LessonEndScreen` ("Bravo!" in the lesson's language),
  `GuideFigure` (sleepy frames).
- **Localization** (the first string tables): `Assets/Scolita/Localization/` (settings, locales `ro` and `en-GB`,
  table `App`, 58 keys, preloaded) and `Assets/AddressableAssetsData/` (local groups, built with the player, no remote
  catalog, catalog updates off); `BuildScript` switches to the Android target before building.
- **Content:** 20 voice lines (`<lang>-line-gate-11` … `-19`, `<lang>-line-rocket-rests`).
- **Tests:** EditMode `ParentGateTests`, `ParentRulesTests`, `DailyPlayTests`, `SessionLimitTests`,
  `ParentReportTests`, `VoiceStatusTests`, `PlayTimeDataTests`, `ProgressResetTests`, `PlayClockTests`,
  `AppStringTableTests`, new cases in `SettingsDataTests`, `LearningRulesAssetTests`, `MapLinesTests`; PlayMode
  `Parent/ParentGateTests` (7, the monkey a minute long), `ParentAreaTests` (7), `SessionLimitTests` (4).

### Conventions for later briefs

- **The app's own words are in the string table `App`** (`Assets/Scolita/Localization/Tables/`): a fixed label gets a
  Localize String Event (its `OnUpdateString` set to the text's `text`); words built in code use `AppLocale.Text(key)`
  or `AppLocale.Text(key, args)` (`{0}` formats); words that belong to a lesson's language use
  `AppLocale.TextIn(language, key)`. Add keys in the Editor's Localization Tables window, both languages, new rows at
  the end; `AppStringTableTests` fails on a key missing a language, and the cedilla test reads every table.
- **The app's language** is `settings.json`'s `appLanguage`, selected by `AppLocale.Select` at boot, on the map and in
  the parent area.
- **A scene that counts play time** owns a `PlayClock` (ticked every frame, saved on a step, a pause and its close);
  the map alone decides when the rocket rests. Only the parent area wakes it.
- **Reset** deletes `ProgressReset.FileNames`: a new file that progress or rewards are rebuilt from goes in that list.
- **A new parent setting** goes in `LearningSettingsData` (per profile) or `TabletSettingsData` (the tablet) with a
  version raise and its `Upgraded` step, and in `ParentSettingsShown`.
- **PlayMode tests that change play time or the limit** leave their scene before restoring the Editor's files
  (`ParentScene.LeaveTheOpenScene`): a map or lesson left open saves its play time when the next test loads a scene.
- **Android Back:** a scene reads `AndroidBack.WasPressed()` once a frame and does what its back button does.

### Decisions and deviations

In Progress → "Design note", "Fresh-eyes review" and "Decisions". Deviations from the brief: **"Literele mici întâi"
is not built** (capitals first would change the curriculum's order; asked of the human); **the music and sounds
sliders and the vibration switch wait for `sound-feel`** (nothing plays music or effects or vibrates yet); the gate
**rests 30 s after a wrong number** (the review); the voice slider stops at 0.2 (a silent voice would lock a parent out
of the gate). "The basics" are filled in `docs/manual-testing.md`.

### Follow-ups

- **`profiles`:** the parent area shows the chosen profile (`ParentRunner` and `MapRunner` still open
  `BuiltInProfiles.FirstChild`); add the profile list and the tester profile to the area (design screen 10); reset
  the tester's own progress with `ProgressReset`; play time and the limit are per profile already.
- **`sound-feel`:** the "Muzica" and "Sunetele" sliders and the "Vibrații" switch in `ParentArea.prefab`'s settings
  card (with `settings.json`'s `musicOn` / `soundsOn` becoming volumes in a version 2, migrated); the hold's ring and
  the gate could get a soft sound (never on a wrong number: it closes quietly).
- **The string tables:** the activities' "Ascultă" labels (in the step's language, `TextIn`); the remaining prefab
  words as they come.
- **The human:** the number range (11–19 mostly start with different letters, which she learns; 21–29 would need
  new lines); whether opening the area should always wake the rocket; capitals first; hear the 20 lines.
- **Later:** a map left open past midnight keeps the old day until it is reopened; the Device Simulator, the Profiler
  window and a cut-out profile were not run.
