# Task: feature/parent-area

> **Status: In progress (parent-area).**

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
