# Task: feature/profiles

> **Status: Ready after `parent-area`** (the human, 2026-11-24: "a Profile
> section so that potentially multiple kids, or I as a tester, would have
> their own profile and progress details"; moved from after the MVP into
> it). Decision D9 (the avatars, and greeting a child by name) is needed
> for the art and the greeting; the rest can start.

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
