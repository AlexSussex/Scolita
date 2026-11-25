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

### Built

- **Core** (`Scripts/Core/Profiles`): `ProfileName` (letters with one space or hyphen between parts, the cedilla made
  comma-below, none; `ReadSaved` without today's limit), `ProfileNameProblem`, `ProfileKind`, `Profile` (the tester's
  `OpensEveryLesson`, `CanSkipSteps`, `ShowsTestInfo`), `ProfileList` (children and the tester, ids never reused and kept
  above every folder, one avatar per child, playing and last child, `FromFolders`), `ProfileRules` (6 children, names of
  20 characters, in the learning rules asset), `ProfileAvatar` (read from `avatars.csv` with the rewards' tables),
  `ProfileLines` ("Who's playing?" and the greeting); `ProfileId.IsSet`; `Core/Parent/ProfileSummary`;
  `MapRoute(…, everyStopOpen)` and `MapStop.OpensAhead`; `LessonRun.Skip()`.
- **Content and build:** `content/avatars.csv` (the 14 avatars of design screen 11, in its order), `AvatarAsset` and
  `AvatarCatalog` (`OfferedIds`, `SpriteOf`), imported into `Content/Avatars/` and `Content/AvatarCatalog.asset`; the
  ledger of released ids counts `avatars.csv`.
- **Game:** `Scripts/Game/Profiles` (`ProfilesData` and `ProfileData`: `profiles.json`; `ProfileStore`: the list, the
  migration, a dated copy before a list with a part that can't be read is saved over, a deleted profile's folder;
  `ProfilesRunner`, "Who's playing?"'s root; `ProfilesScreenView`, `IProfilesInput`, `ProfileCard`; `TesterStripView`,
  `ITesterInput`); `SaveStore.DeleteProfileFolder` and `KeepDatedCopy`; `ParentRunner` (the chosen profile, the profiles
  page: `ParentProfilesView`, `IParentProfilesInput`, `ParentProfileRow`, `AvatarChoice`, `ProfileEditorShown`,
  `ProfilesMessage`); `BootLoader` (the picker with two or more children, else the only child; never the tester),
  `MapRunner` (the profile to open, every stop open for the tester, the avatar button, the strip), `LessonRunner` (the
  profile to open, the strip, skip); `AppScene.Profiles`; `SpeechBubbleView.Words`; `BuiltInProfiles` removed
  (`ProfileList.FirstChildId`).
- **UI:** `Scripts/UI/Profiles` (`ProfilesScreen`, `ProfileCardView`, `TesterStrip`), `Scripts/UI/Parent`
  (`ParentProfilesScreen`, `ProfileRowView`, `AvatarChoiceView`; the header's chosen profile in `ParentAreaScreen`),
  `MapScreen`'s avatar button; `Prefabs/Profiles/ProfilesScreen.prefab` and `TesterStrip.prefab`,
  `Prefabs/Parent/ParentProfiles.prefab`, `MapScreen.prefab`'s `AvatarPlace`, `ParentArea.prefab`'s header button;
  `Scenes/Profiles.unity` (in the build, after Parent); the strip in `Map.unity` and `Lesson.unity`, the page in
  `Parent.unity`; 45 keys in the `App` table, both languages.

### How each screen meets "Usability"

- **"Who's playing?"** (for the child): the cards are 300 × 400 (one to three) or 210 × 270 (four to six), far above
  96 px; each child is found by her picture (one picture per child, enforced), the name under it; the mascot says what
  to do and the greeting is spoken; no text is needed to choose. The parent button is the map's (a hold of 3 s and the
  gate). Android Back does nothing (it is the start, nothing to go back to).
- **The avatar button** on the map: 64 px, top left in the safe area beside the title, shown only when it is useful (two
  or more children, or the tester).
- **The profiles page** (for adults): text 16 to 36 px on the 1280 × 800 canvas (the row's summary line is 16 px, the
  rest 18 and up), every target 64 px or more (the avatars 64 px, rows 80 px), a back button and Android Back; the
  names and pictures stay on the tablet, as it says top right.
- **The tester's strip:** only for the tester; it never covers the map's bottom bar.

### Fresh-eyes review (a second agent; the saves, the list's format, the tester's isolation)

Findings and what was done:

1. *If `profiles.json` can't be read (a newer version wrote it, or an IO error), the list is made from the folders each
   time and never saved, so a card tapped would open `child-1`'s map, another child's progress.* Fixed: when the chosen
   child can't be saved, "Who's playing?" doesn't open any map (it says its line again and logs an error).
2. *A child's entry that can't be read is dropped and its folder orphaned; a corrupt `nextChild` could give a new child
   an old folder.* Fixed: the next child's number is always above every `child-<n>` folder on the tablet
   (`ProfileList.KeepIdsAbove`), so no new child ever gets an old folder. A dropped entry is not adopted back (a
   folder left after a failed deletion would come back as a profile the parent deleted); its file is kept as a dated
   copy (3), for rescue by hand.
3. *A list that loads but has no usable child, or drops an entry, is saved over without a copy.* Fixed: a dated copy,
   `profiles.broken-<date-time>.json` (watched by the copies' ledger like every damaged save), is made first; if it
   can't be made, the file isn't saved over.
4. *`nextChild` has no upper bound: `child-1000000` couldn't be read back.* Fixed: a number above `child-999999` reads as
   the highest child's plus one; `AddChild` refuses to go past it.
5. *If Boot can't save that nobody plays the tester any more, the map would reopen as the tester.* Boot and the area's
   Back now log an error when the list can't be saved. Left as it is otherwise (a full disk at the moment of a
   restart, after the tester played); noted for the human.
6. *The page's actions didn't check an open deletion question.* Fixed: choosing, adding, making or opening the tester do
   nothing while it is asked.
7. *"cel mult 20 de litere" wrote the rules' number by hand.* Fixed: the label and the message take the number from the
   rules (`{0} de caractere`), and the rules keep the limit between 20 and 40 so "de" is always right.
8. *Romanian:* the tester's row says "Nu se arată copiilor"; the unused note line was removed.
9. *A saved name was read against today's limit.* Fixed: `ProfileName.ReadSaved` (letters checked, no length limit).
10. *Style:* the save results are checked and logged; `TryGet`'s doc says its callers.

Sound, as the review found: deleting a profile (the list saved first, then exactly `profiles/<id>`, never a listed
profile, a link unlinked, not followed); the migration from 0.13.0; the ids; the tester's isolation (no card, every
unlock asked of the profile, its reset touching only its folder, Boot never opening it); the Back paths; the Romanian
lines; the repo's conventions.

### Decisions

- **"Who's playing?" is its own scene** (`Profiles.unity`), so the map stays one profile's map; the picker has the parent
  button too (design screen 9), so the area can be reached before anyone plays.
- **Who plays is in `profiles.json`** (`playing`), not in a static: the Map, Lesson and Parent scenes read it; Boot
  resets it every start (nobody with two children, else the only child), so the tester never survives a restart.
- **A profile can have no name** (the first profile, made by the app; or a parent who leaves it empty): the card shows
  only the picture and the greeting has no name. The brief said 1–20 letters; empty is allowed because the app's own
  first profile has no typed name, and a child finds her card by its picture.
- **One picture per child** (not in the brief): two children with the same picture couldn't tell their cards apart. The
  tester may share one.
- **The tester is never deleted** (its progress is reset in one tap); **the last child can't be deleted** (reset it
  instead).
- **The avatars are read with the rewards' tables** (`RewardTables`, `RewardContent.Avatars`): the same pipeline (an id, a
  picture per id, the ledger); the app reads them from `AvatarCatalog`.
- **Commits:** the Game side's view contracts for the new screens went into the `ui:` commit with the UI that implements
  them (changing an existing abstract view needs both sides at once to compile).

### Pushed

- `86ce5cd` `content:` the 14 avatars; `8bd5b0f` `core:` profiles, the list, the tester's unlocks; `5ba3b1c` `build:`
  the avatars imported; `162a1fa` `game:` every scene opens the chosen profile, `profiles.json` and the migration;
  `3bddcea` `core:` ids above every folder, saved names whole (the review); `02b022a` `game:` a dated copy of a list
  with a part that can't be read (the review); `61948f4` `core:` the picker's lines; `87764a3` `ui:` "Who's playing?",
  the profiles page, the tester's strip; with this Progress.

### Test results so far

- The first four (before the rebase onto `49317dc`, docs only): EditMode and `BuildAndroidDebug` each on its own (752, 793, 793, 799
  passed; builds fine; no `warning CS`); the `game:` tree also PlayMode 49/49.
- The `ui:` tree before its last fixes: EditMode 810/810, PlayMode 57/59 (the tester row's words changed by the
  review, and the Editor's `child-1` left with a 10-minute limit by an earlier failed run of the monkey test, fixed by
  `ProfilesScene.Isolated`; both fixed in the commit), `BuildAndroidDebug` fine, no `warning CS`.

### Still to test

(AGENTS.md → "Testing fits the change": the full run comes once, after the last rebase, before the close-out.)

- The review's two commits and the picker's lines: EditMode and the build on their own (were running when the rule changed).
- Everything together: EditMode, PlayMode, `BuildAndroidDebug`, after the last rebase.
- The emulator: 0.13.0 with progress, then this over it (the migration); "Who's playing?", the avatar button, the
  profiles page, the tester's strip and skip, deleting a child; every new screen at every size in AGENTS.md's table.
