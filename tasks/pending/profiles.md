# Task: feature/profiles

> **Status: In progress (profiles)** (claimed 2026-11-25). Was: ready after `parent-area`
> (the human, 2026-11-24: "a Profile section so that potentially multiple kids, or I as a tester,
> would have their own profile and progress details"; moved from after the MVP into it). D9 is
> answered: the 14 avatars, greeting (a) with the tablet's voice.

Read `AGENTS.md` first (especially "The five seams" → 5, "Security and
privacy", "Usability", "Fresh-eyes review"), then `design/README.md` and
`design/screens.md` (build the screens as designed there: screens 9 and
10), and the completion records in `tasks/done/`, above all
`project-skeleton`'s (saving), `progress-review`'s and `parent-area`'s.

**Feature:** `feature/profiles`
**Implements:** PRD §3.8 (profiles); `AGENTS.md` → "The five seams" → 5.
**Depends on:** `parent-area` (the gate, the parent area's layout);
`progress-review` and `rewards` (their save files, already per profile).

A **medium change** to saving: Design note first (the profile list file,
what is per profile and what is per tablet, the migration from the single
`child-1` profile, deleting a profile's folder safely). A fresh-eyes
review before the push (it holds every child's progress).

## Scope

1. **Core's `Profile`:** a stable `ProfileId`, a display name (typed by the
   parent, 1–20 letters, Romanian letters allowed), an avatar id (from the
   avatar catalog), a kind (`Child` or `Tester`), and its creation date.
   `ProfileList` keeps the order and the last profile played.
2. **Saving:** `profiles.json` (the list) at the top of
   `persistentDataPath`; everything else stays under
   `profiles/<profileId>/` (progress, rewards, the sticker scene, the
   lesson in progress, the per-profile settings). **Per profile:** the
   language mix, the letter case first, the session limit and its count
   for today, the review schedule, stars, stickers, trails. **Per tablet:**
   volumes, vibration, the app's own language, the parent gate's numbers.
   The single `child-1` profile from earlier releases becomes the first
   profile in the list, nothing lost (a migration test with a real
   0.x save folder).
3. **"Cine se joacă?" / "Who's playing?"** (screen 9): shown at start when
   there are two or more child profiles; one big card per child profile
   (its avatar, its name under it, its stars count); tapping a card **says
   the name** (the tablet's voice, or a recorded clip if D9 says so) and
   opens that child's galaxy map. With one child profile the app goes
   straight to the map, as before.
4. **The avatar button on the map:** the current profile's avatar, top
   left beside "Școlița" (64 px); tapping it goes back to "Who's playing?"
   (no gate: siblings may switch, as they would pick up their own book).
   It is hidden when there is only one child profile.
5. **Profiles in the parent area** (screen 10, behind the gate): the list
   with each profile's avatar, name, kind and a short summary (letters
   learned per language, last played); **Add a child** (name, avatar from
   the catalog, language mix), **Edit** (name, avatar), **Delete** (asks
   twice, says what will be lost, then removes that profile's folder
   only); the grid of what she has learned and the per-profile settings
   now show the profile chosen at the top of the area.
6. **The tester profile** (for the human): created from the parent area
   ("Profil de test"), never shown in "Who's playing?", opened only from
   the parent area ("Joacă ca tester"). It keeps its own progress like any
   profile, and adds what testing needs:
   - every planet and lesson open (no locks), with a **skip step** button
     on each lesson screen;
   - a small overlay with the version, the lesson and step ids, and the
     voice line being played (clip or tablet voice);
   - **"Șterge progresul profilului de test"** in one tap (no double
     confirmation: it is only the tester's);
   - a badge on the map ("TEST") so it is never mistaken for a child's.
   The tester profile never changes a child profile's saves.
7. **The avatar catalog:** the 14 avatars drawn on design screen 11
   (`design/screens.md` → 11: ids, names and drawings), as content (a
   catalog and sprites made from `design/canvas/Avatar.dc.html`), in that
   order; `look` may refine them in the same style. No photos, no camera.

## Out of scope — do not build

- Accounts, sync between tablets, export or import of progress, a PIN per
  child, photos as avatars.

## Acceptance criteria

- With two child profiles, each keeps its own map position, stars,
  stickers, reviews and settings: playing as one never changes the other
  (a PlayMode test playing a lesson as each).
- With one child profile, the app opens straight on the map, as before
  `profiles`; installing this release over an older one keeps her
  progress as the first profile.
- "Who's playing?" says each name when its card is tapped; a child who
  can't read can find her own card by its avatar.
- Deleting a profile removes only its folder, after two confirmations; a
  random-tap monkey test never opens the parent area or deletes anything.
- The tester profile opens only from the parent area, shows "TEST" and its
  overlay, can skip any step, and never appears in "Who's playing?".
- Every screen meets "Usability"; names are spoken, never only written.

## Tests required

- EditMode: `Profile` and `ProfileList` rules (name bounds, Romanian
  letters, unique ids, last played), the per-profile and per-tablet split,
  the migration from `child-1`, deleting a profile's folder only, the
  tester's unlocks.
- PlayMode: start with one profile (straight to the map), with two (the
  picker), switching from the map, the tester flow, deleting through the
  gate, the monkey test.
- Emulator: every new screen at every size in `AGENTS.md`'s table; an APK
  of the previous release with progress, then this one installed over it.

## Touches

`Scripts/Core/Profiles`, `Scripts/Game/Saving` (`profiles.json`, the
split, the migration), `Scripts/UI/Profiles`, the parent area's prefabs,
the map's top bar, a new `Profiles` scene (or a prefab in `Map`: the
Design note decides), the avatar catalog (new), the string tables.

## Screens

"Who's playing?" (screen 9), the parent area's profiles (screen 10), the
map's avatar button, the tester overlay.

## Voice lines

"Cine se joacă?" / "Who's playing?", the greeting with the name ("Bună,
{name}! Hai să zburăm!" / "Hi, {name}! Let's fly!", if D9 allows the
name), complete sentences in both languages, each heard before the push.
The name is only ever in the nominative; no line puts it in the genitive
or agrees with the child's gender (`AGENTS.md` → "Romanian, checked":
"profilul Anei", never "profilul lui Ana").

## Before closing out

- `docs/manual-testing.md`: "The basics" gains choosing a profile.
- `local/for-human-next-steps.md`: "make your tester profile and a profile
  for her" with the steps.
- Release (the next MINOR). Technical notes: the save-format change.

## Progress

_Agent `profiles`, worktree `C:\Projects\scolita-profiles`, emulator `emulator-5600`._

### Design note (a medium change: a new save file, a migration, deleting a folder, three screens, a new scene)

**Where the logic lives.** Every rule is in Core (`Scripts/Core/Profiles`, no UnityEngine); the Unity side reads
and writes `profiles.json`, deletes a folder when Core says so, and draws.

- **`ProfileName`**: what the parent typed, trimmed; 1–20 characters of letters (any letter, so ă â î ș ț and every
  name), with one space or hyphen between letters ("Ana-Maria", "Ana Maria"); a cedilla ş ţ typed on an old
  keyboard is turned into ș ț. **`ProfileName.None`** is a profile without a name: the first profile, made by the
  app (no one ever typed a name for `child-1`), until the parent gives it one. Its card shows only its avatar, and
  tapping it greets without a name („Bună! Hai să zburăm!”, already in `voice-lines.csv`). A parent may also
  leave the name empty for a new child (a child who can't read finds her card by its picture anyway).
- **`Profile`**: a stable `ProfileId`, a `ProfileName`, an avatar id (a `ContentId` of the avatar catalog), a
  `ProfileKind` (`Child`, `Tester`) and its creation day. What the tester adds is asked of the profile, never of
  a flag anywhere else: `OpensEveryLesson`, `CanSkipSteps`, `ShowsTestInfo` (true only for `Tester`).
- **`ProfileList`** (the one owner of these rules): the profiles in the order they were made; `LastChild` (the
  child who played last: the yellow ring on "Who's playing?"); `Playing` (whose map, lessons and saves are open
  now, a child or the tester; none while "Who's playing?" waits); `NextChildNumber`.
  - `AddChild(name, avatar, day)` makes `child-<n>` from `NextChildNumber`, which only grows, so a deleted
    child's id (and folder name) is **never reused**; at most `ParentRules.MostChildProfiles` children (6, in the
    learning rules asset).
  - `AddTester(avatar, day)` makes the one tester, id `tester`.
  - `Change(id, name, avatar)`; `Delete(id)`: a child only, never the last child (the parent resets progress
    instead); the tester is never deleted, only its progress is reset (in one tap).
  - **Avatars are one per child** (a child finds hers by its picture): an avatar another child has can't be
    chosen (`IsAvatarFree`); the tester may share one.
  - `Play(id)` (a child also becomes `LastChild`), `StopPlaying()`; `ProfileToOpen` (the one playing, else the
    last child, else the first: never none, never a deleted one); `StartsWithPicker` (two or more children).
  - `FromFolders(folders, avatars, day)`: the list made from the profile folders on the tablet (below).
- **`ProfileSummary`** (Core/Parent): a row's "Litere învățate: română 5, engleză 3 · Ultima dată: azi", from the
  same `ParentReport` the grid shows (the learned letters per language) and `LearnerProgress.DaysPlayed`.
- **The tester's unlocks:** `MapRoute` takes `everyStopOpen` (every stop `CanOpen`; the rocket and the planets'
  faces stay as the tester's progress says); `LessonRun.Skip()` ends the current step as done with its answer
  shown, so a skipped step is "practised", never "right", in the tester's own progress, and the lesson moves on as
  after any step.

**The save files.**

- **`profiles.json`** (new, the tablet's folder, version 1; `ProfilesData`):
  ```json
  { "version": 1,
    "profiles": [ { "id": "child-1", "name": "", "avatar": "kitty", "kind": "child", "created": "2026-11-25" },
                  { "id": "tester", "name": "", "avatar": "rocket", "kind": "tester", "created": "2026-11-25" } ],
    "lastChild": "child-1", "playing": "child-1", "nextChild": 2 }
  ```
  Read with the usual rules (`SaveStore`): an entry with an id that isn't a profile id, a second entry with the
  same id, an unknown kind or a second tester is left out with a warning; a name that isn't a `ProfileName`
  reads as none; an avatar the catalog doesn't have is kept in the file and drawn as the first avatar; `playing`
  or `lastChild` naming no profile reads as none; `nextChild` is raised above every `child-<n>` in the list.
- **Per profile** (`profiles/<id>/`, unchanged files): `progress.json` (letters, words, reviews, lessons done,
  the rocket's stop), `rewards.json` (stars, stickers, the sticker scene, trails), `lesson-run.json`,
  `learning-settings.json` (the language mix, the daily limit), `play-time.json` (today's count). "Letter case
  first" is not built (`parent-area`'s deviation, still with the human).
- **Per tablet** (the save root): `settings.json` (volumes, vibration, the app's language) and `profiles.json`;
  the gate's numbers are in the learning rules asset. A test checks the split: every file a profile's scene saves
  is under its folder, and nothing of a profile's is in the root.
- **Migration (an older tablet) and a damaged list:** when `profiles.json` is missing (every release before this
  one) or damaged (set aside, dated, as every save), the list is **made from the profile folders on the tablet**:
  `child-1` first, then any other `child-<n>` by number, then `tester`; names none, avatars in catalog order
  (`kitty` first) skipping the used ones, today as their day. With no folder at all (a new install) it is
  `child-1` alone. So her `child-1` folder from 0.13.0 is the first child, untouched, with everything in it; and
  a damaged list never loses a profile's progress (only its name and avatar, which the parent sets again). The
  list is saved at once. Folders are never deleted because they are missing from the list.
- **Deleting a profile** (the parent area, two questions): the list without it is saved first; only then is its
  folder deleted (`ProfileFolders.Delete`), which deletes exactly `<root>/profiles/<id>` for a valid id that the
  saved list no longer has, and nothing else. If the folder can't be deleted, the profile is still gone from the
  list (its id is never reused, so no later profile sees those files) and the area says so. A failed list save
  deletes nothing.

**Who opens what.**

- **Boot** reads the list (making it, above), then: two or more children → `StopPlaying`, the new **Profiles**
  scene (`Profiles.unity`, "Who's playing?"); otherwise `Play(the only child)` and the Map, as before. So the app
  never starts as the tester.
- **The Map, Lesson and Parent scenes** open `ProfileToOpen`'s folder (no runner names `BuiltInProfiles.FirstChild`
  any more; it stays only as the id of the first profile in the migration). A tester's map opens every stop and
  shows "TEST" (with the version) by the title.
- **"Who's playing?"** (`ProfilesRunner`, design screen 9): the mascot says „Cine se joacă? Atinge poza ta.” (in the
  app's language); a card per child (avatar, name, stars from `rewards.json`), the last child ringed. A tap greets
  her in her own map's language („Bună, Ana! Hai să zburăm!” through the tablet's voice; with no tablet voice the
  bubble shows it) and opens her map when the line ends (never cut off). The parent button and gate as on the
  map (to the Parent scene). Android Back does nothing (it is the start). One to three cards in a row, four to six
  in two rows, each at least 96 px.
- **The map's avatar button** (`AvatarPlace`, 64 px, beside "Școlița"): the profile's avatar; shown with two or
  more children, and always for the tester (the tester's way back to a child); a tap opens "Who's playing?" (no
  gate). "Who's playing?" opened this way also shows a single child.
- **The parent area** shows the **chosen** profile (the one playing when it opens): a button in its header shows the
  chosen profile's avatar and name and opens **Profiluri** (design screen 10, `ParentProfiles.prefab`, over the
  area). There: a row per profile (avatar, name or "Fără nume", kind, the summary, **Schimbă**); tapping a row
  chooses it for the grid, the settings and reset; **Adaugă un copil** (hidden at the most children); the tester's
  row (**Joacă ca tester** opens the map as the tester; **Șterge progresul de test** in one tap) or **Fă un profil
  de test** when there is none. The right card edits the chosen profile or the new child: the name (a text field
  of 20, Romanian letters from the tablet's keyboard; a name that can't be used says why and isn't saved), the 14
  avatars (7 × 2, the ones other children have dimmed), the language mix, **Salvează**, and **Șterge profilul…**
  (children only, not the last one; asks twice, saying what is lost and that the other profiles stay). Back (and
  Android Back) close a question, then the page, then leave: to the map of the one playing, or to "Who's
  playing?" when nobody is.
- **The lesson as the tester:** a strip at the bottom with "TEST", the version, the lesson and step ids, the step's
  kind and the line being said (its id and whether a clip, the tablet's voice or the bubble said it), and a
  **Sari peste pas** button (64 px) that skips the step. A child's lesson has none of it (the strip is off unless
  `ShowsTestInfo`).

**Words and voice.** New lines: none (`ro|en-line-who-plays`, `-hello-name` and `-hello` are in `voice-lines.csv`
since `content-model`). The name goes only into „Bună, {name}! Hai să zburăm!” / "Hi, {name}! Let's fly!": the
nominative after "Bună,", the same for a girl or a boy, whatever the name (Ana, Matei, Ștefan, Ioana-Maria); with
no name, „Bună! Hai să zburăm!” / "Hello! Let's fly!". The parent area's new words go in the `App` table (both
languages), worded without a name in the genitive or dative and without agreeing with the child ("Schimbă
profilul", "Fără nume", "acest profil"). The 14 avatars become content: `content/avatars.csv` (`id`, `retired`,
`notes`; design screen 11's ids, in its order), read with the rewards' tables (the same pipeline: an id, a picture
per id, the ledger of released ids), imported into `AvatarCatalog` with `Art/Avatars/avatar-<id>.png`.

**Tests.** EditMode: `ProfileNameTests` (bounds, Romanian letters, spaces and hyphens, the cedilla, none),
`ProfileListTests` (ids never reused, the most children, the last child can't go, the tester once and never
deleted, avatars one per child, `Play`/`ProfileToOpen`/last child, from folders: `child-1` first, a new install,
the tester's folder), `ProfilesDataTests` (round trip, every damaged entry, the migration from a real 0.13.0
folder), `ProfileFoldersTests` (deletes only that folder; refuses a listed id), the per-profile/per-tablet split,
`MapRouteTests` (every stop open), `LessonRunTests` (skip), `ProfileSummaryTests`, the avatar table. PlayMode: one
child → straight to the map; two → the picker, a tap says the name and opens her map; the avatar button back to
the picker; two children each playing a lesson keep their own progress, stars and settings; the tester (from the
area only, "TEST", every stop open, skip, never on the picker, its one-tap reset leaving the children's files);
adding, changing and deleting through the gate (two questions, only that folder gone); the monkey test on the
picker and the map (random taps never open the area or delete anything). Emulator: every new screen at every size;
0.13.0 with progress, then this release over it.

**Another domain.** Profiles know nothing of letters: a numbers or music domain's progress and rewards are in the
same folders; only the row's summary reads the letters' report (a later domain adds its own line there).
