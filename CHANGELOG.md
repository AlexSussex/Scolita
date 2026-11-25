# Changelog

What changed in each release of Școlița, newest first.

- **One version for the app,** in `VERSION` (made by `project-skeleton`).
  The parent area shows it.
- **Numbering** (`MAJOR.MINOR.PATCH`, each part 0–99). Before 1.0.0, a
  feature raises MINOR (0.4.0 → 0.5.0); a fix, a chore or new content alone
  raises PATCH (0.5.0 → 0.5.1). 1.0.0 is the human's call.
- **A release per finished brief:** the brief's close-out commit raises
  `VERSION` and adds the entry here; the commits before it don't.
- **Tags:** each version has a git tag `v<version>` on its close-out commit.
  Build a release APK from its tag (README → "Build a release APK").

Each entry has the version and the date, a short summary in plain words,
what changed for the child and for the parent, then technical notes (new
packages, save-format changes, content added, what the human must do) with
a link to the brief. The rules are in [`AGENTS.md`](AGENTS.md) →
"Versioning and releases".

## 0.14.0 (2026-11-25)

Profiles. Each child on the tablet has her own profile, with her own map, stars, stickers, letters and settings,
and the parent has a tester profile. With two or more children the app opens on "Who's playing?": each child finds
her card by its picture, and the mascot greets her by name. With one child it opens on her map, as before.

- **For the child:**
  - **"Cine se joacă?" / "Who's playing?"** (only with two or more children): a big card per child with her picture,
    her name and her stars; the mascot says „Cine se joacă? Atinge poza ta.”; a tap greets her („Bună, Ana! Hai să
    zburăm!”, said by the tablet's voice) and opens her map.
  - **Her picture on the map,** beside "Școlița" (with two or more children): a tap goes back to "Who's playing?", so a
    brother or sister can play.
  - Playing as one child never changes another's map, stars, stickers, letters or settings.
- **For the parent:**
  - **Profiluri** in the parent area (the button at the top shows the profile chosen; the grid, the settings and
    reset are that profile's): add a child (a name of up to 20 letters, a picture from the 14, the lessons'
    languages), change a name or picture, delete a child (asked twice; only that child's files go).
  - **The tester profile:** "Fă un profil de test", then **Joacă ca tester**: every lesson open, a "TEST" strip with
    the version, the lesson and step and the line being said, **Sari peste pas** in lessons; **Șterge progresul de test**
    in one tap. It never shows on "Who's playing?" and never changes a child's files.
  - A tablet that already has her progress keeps it as the first child's profile after the update.

### Technical notes

- **Save format:** a new `profiles.json` in the tablet's folder (version 1: the profiles with their id, name, avatar,
  kind and day; the last child; who is playing; the next child's number). Every profile's files stay in
  `profiles/<id>/`. A tablet without the list (0.13.0 and earlier) or with a damaged one gets it made from the profile
  folders (`child-1` first), nothing lost. A child's id (`child-<n>`) is never reused; the tester's is `tester`.
- **Content:** `content/avatars.csv` (14 avatars, design screen 11's ids; read with the rewards' files, imported into
  `AvatarCatalog`); no new voice lines (`who-plays`, `hello-name` and `hello` were already there). New words in the `App`
  string tables (the profiles page, the tester's strip).
- **New:** the `Profiles` scene; `Prefabs/Profiles/` (the picker, the tester's strip); `Prefabs/Parent/ParentProfiles.prefab`;
  the learning rules asset's `mostChildProfiles` (6) and `nameMostCharacters` (20).
- **The human:** make the tester profile and a profile for her (`local/for-human-next-steps.md`, step 1). Hear the
  greeting with her name on the tablet (it needs the tablet's Romanian voice).
- Brief: [tasks/done/profiles.md](tasks/done/profiles.md).

## 0.13.0 (2026-11-25)

The parent area. Behind a gate a child can't open by tapping around: what she has learned, the days she
played, the next lessons and the recorded voices; the languages of her lessons, a daily time limit, the
voice's volume and the menus' language; and deleting her progress (asked twice). The app's own words now come
from string tables, in Romanian or English.

- **For the child:**
  - Nothing new to do; the menus' words can be in English when the parent chooses it.
  - **The daily limit** (off unless a parent sets it): when today's time is reached, the lesson she is in
    always plays to its end; then the map sleeps, the mascot yawns and says „Racheta se odihnește. Ne vedem
    mâine!” / "The rocket needs a rest. See you tomorrow!", and no lesson opens until the next day, or until a
    parent passes the gate.
  - **Android Back** on the tablet does what the screen's back button does (a lesson goes home; on the map it
    closes an open book).
- **For the parent:**
  - **The lock** at the top right of the map: press and hold it 3 seconds (a yellow ring fills), then tap the
    number said, among three written as words. If the tablet has no voice for the language, the number is shown
    in digits instead. A wrong number closes it quietly.
  - **Ce a învățat:** each letter in Romanian and in English (new, being learned, learned), the days played in
    the last two weeks, the next three lessons, how many voice lines are recorded.
  - **Setări:** the lessons' languages (both, Romanian only, English only), the daily limit (none, 10, 15, 20
    minutes), the voice's volume, "Meniurile în engleză", the version.
  - **Șterge progresul…:** asks twice, says what is lost (letters, lessons, place on the map, stars and
    stickers) and keeps the settings.

### Technical notes

- **Saves:** `learning-settings.json` version 2 adds `sessionLimitMinutes` (a version 1 file reads as no limit,
  D8); new per profile `play-time.json` (today's seconds; damaged or missing reads as nothing played). Reset
  deletes `progress.json`, `map-progress.json`, `lesson-run.json` and `rewards.json`; `SaveStore.Delete`.
- **The first string tables** (Unity Localization, already installed): locales `ro` and `en-GB`, table `App`
  under `Assets/Scolita/Localization/`, its Addressables groups local only, built with the APK (no new package;
  the APK still asks for no internet). "Hai!"/"Go!", the bar's words, "Mai departe", "În cartea mea" and
  "Bravo!"/"Well done!" moved in.
- **The gate rests 30 s after a wrong number** (the fresh-eyes review); the voice slider stops at 0.2, so the gate's
  number is always heard.
- **Build:** `BuildScript` switches the Editor to the Android target first (the tables' Addressables are built for
  the active target).
- **Content:** 20 voice lines (the gate's numbers 11–19 and the rocket's rest, both languages; no clips yet).
- **Not built (the human decides):** the "Literele mici întâi" switch (capitals first would change the
  curriculum's order); the music and sounds sliders and the vibration switch come with `sound-feel`.
- Brief: [tasks/done/parent-area.md](tasks/done/parent-area.md).

## 0.12.0 (2026-11-25)

Rewards: stars, stickers and rocket trails. Every lesson she finishes lights three stars in her
planet's constellation; every second lesson, and at the end of each planet, a sticker pops out of a
gift box for her sticker book; each finished planet gives the rocket a new trail. Nothing can ever be
lost or bought.

- **For the child:**
  - **The end of a lesson:** "Bravo!", and three gold stars fly one after another into the planet's
    constellation (the unicorn, the rocket, the whale), each lighting as it lands; „Bravo! Ai aprins
    3 stele în constelația Unicornului.” Then, when one is earned, the gift box opens and a sticker
    pops out („Ai câștigat un abțibild!”), and at a planet's end the new trail („Ai câștigat o dâră
    nouă pentru rachetă!”). **Mai departe** goes back to the map; **În cartea mea** opens the sticker
    book.
  - **The map's bottom bar:** **Cerul meu** (her stars in a yellow pill), **Abțibilduri** (once she
    has a sticker) and **Dâre** (once she has a trail).
  - **The sky book:** a constellation per planet over its planet, lit star by star; tapping one says
    its name.
  - **The sticker book:** the Earth on the purple sky and her stickers in a strip below. She drags a
    sticker onto the scene, moves it, makes it bigger or smaller with two fingers, and drags it back
    to the strip to take it off. It stays where she put it, even after the app is closed.
  - **The trails:** she picks the rocket's trail (the rainbow, the stars, the bubbles), and the rocket
    wears it on the map.
- **For the parent:** nothing to set. A tablet that played an earlier version gets the stars of the
  lessons she already finished and the trail of a finished planet (no stickers for the past).

### Technical notes

- **A new save file:** `profiles/<id>/rewards.json` (version 1): the stars earned, the lessons lit
  (in order), the lessons finished, the stickers given (in order), the planets rewarded, the trails
  earned and the one chosen, the stickers on the scene (x, y, size). Missing: made from her progress;
  damaged: set aside, dated, and made again the same way.
- **Content:** three new files, `content/stickers.csv` (the 18 stickers of `look`),
  `content/constellations.csv` (unicorn, rocket, whale, drawn from their stars and lines) and
  `content/trails.csv` (rainbow, stars, bubbles), imported into `Content/Rewards/`, in the ledger of
  released ids (recorded under 0.11.0, whose close-out came after they were pushed). 14 new voice lines in both languages (a new trail, the constellations' names, the
  sky book, the sticker book, the trail picker), not recorded yet.
- **The reward rules asset** (`Settings/RewardRules.asset`): 3 stars a lesson, a sticker every 2
  lessons, sticker sizes from 0.5 to 2.5.
- **Art** (drawn by agents): the gift box and its lid, the bar's three icons, the constellation's line.
- The brief: [`tasks/done/rewards.md`](tasks/done/rewards.md).

## 0.11.0 (2026-11-25)

Her progress and reviews. The app now keeps, for each letter in each language, how often she got it
right, in which games and on which days. A letter answered right 6 times, in at least 2 kinds of game,
on at least 2 different days, is learned; a learned letter comes back as a short warm-up at the start
of a lesson the next day, then 3 days later, then 7, 14 and 30.

- **For the child:**
  - **Warm-ups:** a lesson may start with up to two quick games on letters she has learned that are
    due for review, after „Hai să ne amintim!” / "Let's remember!": hear a letter and tap it, or see a
    picture and tap the letter its word starts with, among letters she already knows. They count among
    the lesson's dots; a miss brings that letter back sooner, never takes anything away.
  - Nothing else changes on screen: the map, the lessons and her place on the map are as before.
- **For the parent:** nothing to set yet (the parent area, next, will show which letters are new,
  being learned or learned). Her place on the map is kept through the update. Switching the language
  mix (later, in the parent area) hides the other language's lessons and keeps their progress.

### Technical notes

- **A save-format change:** `profiles/<id>/progress.json` (version 1) replaces `map-progress.json`:
  the lessons done, the rocket's stop, the days played, and each letter's and word's progress (rights
  by kind, practice, the days right, learned, the review step and the next review). It is made from
  0.10.0's `map-progress.json` (or, from 0.9.0 and earlier, from `lesson-run.json`) the first time;
  `map-progress.json` is left as it is and no longer written. A damaged `progress.json` is set aside,
  dated, and her place is made again from those files (the letters' counts are only in the copy).
- **The damaged copies go by themselves** (the human's rule, Q1): each save folder has
  `broken-copies.json`; a file's dated copies are removed at start once a newer version of the app
  has read and saved that file fine on 3 different days (`LearningRules.asset`).
- **The learning rules asset** holds the numbers of `docs/curriculum.md` §5: 6 right answers (each
  language its own number), 2 kinds, 2 days; reviews after 1, 3, 7, 14 and 30 days; a miss one step
  back; 2 warm-ups of 3 letters.
- **Content:** none new; the warm-ups say the existing „Hai să ne amintim!” / "Let's remember!"
  (`ro-`/`en-line-lets-remember`), not recorded yet.
- The brief: [`tasks/done/progress-review.md`](tasks/done/progress-review.md).

## 0.10.0 (2026-11-25)

The galaxy map. The app opens on the purple sky with the rainbow path and the planets of the
lessons: the planet she is on is awake with its letters below it, the ones she has finished beam,
the ones ahead sleep. The rocket stands on her next lesson and the big **Hai!** opens it. When she
finishes a planet, the rocket flies to the next one past the moon and the Earth.

- **For the child:**
  - **The map:** a planet per group of letters (planets 1–3 now), each with its letters on tiles
    (the letter being learned ringed in yellow, those to come faint), and a stop on the path for
    every lesson: a green tick when done, the big yellow dot under the rocket, faint ones ahead.
    The map slides sideways with a finger.
  - **Hai!** opens the lesson the rocket stands on (the one she left half-way resumes where she
    was); finishing it brings her back with the rocket one stop further.
  - **The trip:** after a planet's last lesson the rocket flies past the moon and the Earth to the
    next planet (4 seconds; a tap skips it), and the unicorn says „Am aterizat pe o planetă
    nouă!”.
  - **The unicorn greets her** with the next lesson's letter, in that lesson's language: „Bună!
    Azi învățăm litera m.” or "Hello! Today we are learning the letter m." Tapping it says the
    line again.
  - **Not yet, kindly:** a sleeping planet or a stop ahead wobbles and says it isn't time yet;
    it never opens. A finished stop opens again to play it once more.
- **For the parent:** nothing to set. Where she is on the map is kept per profile; a tablet that
  played an earlier version starts the map where she was.

### Technical notes

- **A new save file:** `profiles/<id>/map-progress.json` (version 1): the lessons done and the stop
  where the map last showed the rocket. With no file (an earlier version) or a damaged one, the
  lessons before the one in `lesson-run.json` count as done. Unknown lesson ids are kept.
- **Damaged saves are kept, dated** (the human's rule, Q1): `<name>.broken-<yyyyMMdd-HHmmss>.json`,
  never replacing an older copy, for every save file. Removing old copies comes with
  `progress-review`.
- **Content:** 4 voice lines (`ro-`/`en-line-stop-ahead`, `ro-`/`en-line-all-planets`); the map also
  says the existing `today-letter`, `hello`, `planet-landed` and `planet-asleep`. No clip yet: the
  tablet's voice says them.
- **Art:** `look`'s whole planet pictures (`Art/Planets/`) and rocket (`Art/Trip/`).
- **Room kept, nothing to tap yet:** the avatar (`profiles`), the parent button (`parent-area`)
  and the bottom bar's sky book and sticker book (`rewards`) have empty places on the map.
- The brief: [`tasks/done/galaxy-map.md`](tasks/done/galaxy-map.md).

## 0.9.0 (2026-11-25)

The look. The art her parents chose: ChatGPT's purple sky, planets with faces, the rocket, the
moon and the Earth, her purple cat in a space helmet, 14 profile pictures, 18 stickers, the
app's icon and pictures for 40 words. The agents drew the rest: the cards, the buttons, the
writing board, the badges, the rainbow path. The sky, the icon, the word pictures and the
lesson's planets show now. The map, the profiles and the sticker book use the rest when they
come. The unicorn guide is a stand-in until its full astronaut body is made.

- **For the child:**
  - **A starry purple sky** behind the map and every lesson.
  - **The unicorn astronaut** on the map and in the lessons: a round stand-in for now, until
    the chosen unicorn gets its full body.
  - **Pictures for 40 words** of planets 1–3 instead of plain circles: in Romanian mama, tata,
    pat, apă, nas, masa, pian, sat, dinte, stea, panda, pom, pod, pin, mop, ponei, mops,
    pisoi, piton, stop, tenis, domino, patine, nota, nod, rachetă, planetă, nani (a sleeping
    moon); in English map, mat, tap, pin, man, tin, sit, pen, dot, ten, pod, moon.
  - **One letter, two sounds** has a gold Romanian planet and a blue English planet, with faces.
  - **The home button** is a house; **Hai!** is a big yellow button.
- **For the parent:** the launcher shows the unicorn astronaut's icon.

### Technical notes

- **The art:** `game/Assets/Scolita/Art/`, one sprite atlas per folder, each fitting one page:
  Sky, Map, Planets, Trip, Path, Effects, Guides, Badges, Board, UI, Stickers, Constellations,
  Avatars. `Art/SOURCE.md` says which pictures are ChatGPT's (the human's) and which the agents
  drew.
- **For `galaxy-map`:** the chosen planets are whole pictures in `Planets/`, and the rocket's
  six pictures are in `Trip/`. The old planet parts and rocket stay in `Map/` until
  `galaxy-map` has switched.
- **The tools:**
  - `node design/art/build.mjs` redraws the agents' art.
  - `node design/art/import-drafts.mjs` cuts the chosen parts of the ChatGPT sheets kept in
    `local/art-inbox/`, as `design/art/drafts.json` lists them.
  - Școlița → Rebuild sprite atlases rebuilds the atlases.
- **Content:** 40 pictures in `content/pictures/` and `pictures.csv`; 48 still have
  placeholders. No new ids.
- **Prefabs:** the map, the lesson HUD and `TwoSoundsView` show the art. `TwoSoundsView`'s
  planets are whole pictures (its face and ring objects removed).
- **The launcher icon** is adaptive (two layers).
- **No package, save-format or voice-line change.**
- **For the human:**
  - Make the guide's full body (and 12 more stickers, and the 48 missing word pictures) with
    ChatGPT: step 8 in `local/for-human-next-steps.md`, prompts in
    `local/art-prompts/look-guide-body.md`.
  - The badges stay as they are for now.
- **The brief:** [`tasks/done/look.md`](tasks/done/look.md).

## 0.8.1 (2026-11-25)

Planets 2 and 3. After planet 1 (a m t p) come s i n and o d e, each letter in Romanian and in English, with the same
games as planet 1: meet the letter, pick it, hunt it, trace it in print and by hand, find a word's first sound, the
two sounds of i, o and e, the drum, build a word, read and match, words written by hand and joined, the capitals, and
a party with word twins and false friends. Content only: no screen changed.

- **For the child:**
  - **Six new letters:** s i n (planet 2) and o d e (planet 3), each met with a word and a picture (soare, insulă,
    nor, ochelari, delfin, elefant; star, insect, nose, octopus, dolphin, egg), traced in print and by hand (the i's
    dot last; the o leaves high and joins high).
  - **New words to read:** Romanian nas, masa, pian, nai, pin, sat, ananas, mami, tati, nani, pas, spin, sita; pom,
    pod, dinte, stea, mop, dop, pepene, domino, desen, dans, nota, ponei, pisoi, panda, piton, tenis, sanie, patine,
    mops, nod, stop. English pin, sit, man, tin, pan, ant, nap, sip, mint, pants; dot, pen, mop, ten, pot, top, net,
    dad, pet, pod, nod, tent, nest, pond, stop, spot, mops.
  - **Words by hand:** nas, masa, pian; pom, dinte, stea; sit, pin, man; dot, pen, ten.
  - **Bridges:** the two sounds of i, o (and a light one for e); twins pian / piano, rachetă / rocket, mop, stop,
    panda, ponei / pony, tenis / tennis, domino; false friends pin, pod, nod.
  - Planets 2–3 follow planet 1's party with **Hai!** (no map yet). Pictures are still plain circles.
- **For the parent:** nothing new to set. The lessons speak with the tablet's voice until the lines are recorded.

### Technical notes

- **Content added:** 2 units, 6 letters, 20 lessons, 128 steps, 87 words (61 to read, 26 listen-only), 71
  placeholder pictures, 190 voice lines, 24 letter shapes (`content/strokes/`, all `draft`); 376 ids recorded in
  `content/released-ids.txt`. No code, package or save-format change.
- **Tests:** `RealLessonsTests` plays every lesson of planets 1–3 to its end (one case per lesson); recorded
  handwritten lines for s i n o d e, pom, dinte, dot and stea.
- **The human:** tick the review table (shapes, words, pictures, the questions), and record planets 2–3's lines when
  planet 1's are done (`local/for-human-next-steps.md`).
- Brief: [`tasks/done/content-letters-1.md`](tasks/done/content-letters-1.md).

## 0.8.0 (2026-11-25)

Writing by hand. After the print letter, each Romanian letter lesson has the letter by hand
("litera de mână", as Romanian clasa I teaches it), and each planet's word lessons end with a
short word written by hand and joined: mama, tata, map, mat. The handwritten letters are drawn
from their stroke data, never from a font, so what she sees is exactly what she traces.

- **For the child:**
  - **Meeting a letter:** a small card beside the letter shows it by hand; a tap writes it
    before her eyes, „Uite cum se scrie litera de mână.”
  - **The letter by hand** (`ro-a` … `ro-p`, after the print one): the board with the school's
    four lines (dashed, solid, solid, dashed) and faint slanted lines; „Scrie litera de mână.
    Linia de jos: de aici pornești. Începe de la punct și urmează săgeata.” (m starts at the
    middle line, so it has no "bottom line" sentence). The a has three strokes (its entry
    from the line, its round, its stem with the exit); she may lift between them or write it
    in one line. Three rounds as in print; „Bravo, ai scris litera a de mână!”. Begun
    without the entry: „Pornește de pe linie.”
  - **In English** (`en-a` … `en-p`): one round of the letter by hand.
  - **A word by hand** (`ro-words-1`: mama, tata; `en-words-1`: map, mat): the word in print
    on the right, said slowly; she writes it letter by letter or in one line. At each letter,
    at once: „Bravo, ai făcut litera m! Acum leag-o de litera a, fără să ridici degetul.”;
    the line plays to its end while she writes on. tata's bars come after the word („Acum
    trage liniuța literei t.”). The word done: „Bravo, ai scris cuvântul mama!”. A finger
    lifted at a join and put down near it is forgiven; lifted inside a letter: „Nu-i nimic!
    Pune degetul înapoi pe punct și continuă.”; the answer shown: „Uite cum se leagă
    literele.”
- **For the parent:** the handwritten shapes of a m t p and A M T P are drafts until you
  check them against her manual (above all the a's three strokes); the letters in the word
  lines are said by their sounds (names come later).

### Technical notes

- No new package; no save-format change (the lessons' steps are unchanged except the English
  handwritten steps' `rounds=1`, so an English lesson in progress restarts at its first step).
- **Content:** eight handwritten shapes (`content/strokes/*-handwritten.json`, all `draft`);
  the stroke format is unchanged: a stroke that begins where the one before ends continues it,
  the strokes after the exit are marks, joins are read from the entry and exit runs
  (`content/README.md` → "strokes/"). A new step option `rounds` (1–3). The import now
  refuses a tracing step whose letter has no shape for its form, and checks handwritten
  entries, exits and lines. No new voice line (all were in `voice-lines.csv`).
- **Core:** `TraceFigure`, `JoinedWord`, `LineCheck`, `StrokeVerdict.SkippedEntry`,
  `TraceWordRules`, `TracingTuning.JoinRadius` (in `Settings/LearningRules.asset`),
  `MeetLetterRules`' "de mână" button. **Unity:** `WritingBoardView` (was `TraceLetterView`),
  `HandwritingView`, `FigureFit`, `Prefabs/Lesson/TraceWordView.prefab`.
- **For the human:** try the test APK `local/apks/Scolita-0.7.0-handwriting-try-debug.apk` (or
  this release's) with her and say whether the joins feel natural; check the handwritten
  shapes against her manual; say whether the word lines should name the letters instead.
- Brief: [`tasks/done/handwriting.md`](tasks/done/handwriting.md).

## 0.7.0 (2026-11-25)

Tracing letters with a finger. In each Romanian letter lesson and in the capitals lesson, she
writes the print letter on a dark writing board: its strokes in order, from each green start
dot, in the direction of the yellow arrow, in three rounds: on dots, on a faint shadow, then
from the start dot only. Her finger leaves a rainbow trail with stardust.

- **For the child:**
  - **The writing board** (after meeting a letter, `ro-a` … `ro-p`; A M T P in
    `capitals-1`): the four writing lines, the letter as a dotted guide, the start dot with
    its number ("1", and the next stroke's "2" smaller beside it) and a yellow arrow;
    „Scrie litera de tipar. Începe de la punct și urmează săgeata.”
  - **Her trail** is a rainbow under her finger, with sparkles; a good stroke stays in
    rainbow ink and the next dot appears („Foarte bine! Acum mergi la punctul cu numărul
    doi.”). A round done: the letter bounces, „Bravo, ai scris litera a! Acum scrie pe
    umbră.”, then „… fără ajutor, de la punct.”; the three stars on the right light up.
  - **A stroke that misses** fades and the start dot pulses, with a kind hint for what went
    wrong: „Începe de la punctul verde.”, „Urmează săgeata.”, „Rămâi pe linia punctată.”
    or „Încet și cu grijă: o să reușești!”. A second miss: „Uită-te cum se scrie, apoi
    încearcă tu.” and the stroke draws itself once, its dots shown, for her to trace.
  - **One finger draws:** a second finger, or a palm resting on the board, is ignored.
- **For the parent:** the print letters of planet 1 (a m t p, A M T P) are traced; the
  handwritten tracing steps and the English lessons' tracing (by hand) wait for
  `handwriting`. The letters' strokes are drafts until you check them against her manual.

### Technical notes

- No new package; no save-format change (a lesson in progress resumes at its step; the
  steps are unchanged, only the tracing steps now play).
- **Content:** a new content type, the letters' shapes: `content/strokes/<letter>-<case>-
  <form>.json` (`content/README.md` → "strokes/"), imported to `Content/Strokes/` and
  `LetterShapeCatalog`. Eight print shapes, all `draft` (the import lists them). The print
  a is two strokes (the human's answer). Shapes are not in the ledger of released ids;
  removing a shape's file removes its asset. No new voice line: `TraceLetter` now needs
  `trace-print`, `trace-hand`, `trace-start`, `trace-hint-dot`, `-arrow`, `-stay`, `-slow`,
  `-watch`, `trace-next-2`, `-3`, `trace-done`, `trace-round-2`, `-3`.
- **Core:** `Tracing/` (`LetterShape`, `StrokePath`, `StrokeCheck`, `TracePoints`,
  `TracingTuning`, …), a JSON reader, `TraceLetterRules`, `PlayableSteps` (a step plays when
  its kind has a view and its content is there), `ShapeRules` in the validator; the tracing
  tolerances in `Settings/LearningRules.asset`.
- **For the human:** try the test APK `local/apks/Scolita-0.6.0-tracing-try-debug.apk` with
  her (or this release's) and say whether the tolerances feel right; check the eight print
  shapes against her manual; answer the review's questions (brief → Follow-ups).
- Brief: [`tasks/done/tracing.md`](tasks/done/tracing.md).

## 0.6.0 (2026-11-24)

Four word games in the lessons. Once a planet's letters are met, its word lessons let her
drum a word's syllables, build a word in the rocket's windows, read a word and find its
picture, and, at the planet's party, join the Romanian words to their English twins.

- **For the child:**
  - **The drum** (`ro-words-1`, `en-words-1`): a picture and its word said; she taps the
    big drum once per syllable (in English, per beat), a yellow dot lighting for each, then
    the green tick. A wrong count clears the dots and the word is said in syllables
    ("ma-ma"); a second shows one glowing circle per syllable for her to fill.
  - **Build a word:** a word said and pictured (not written); she drags the letters into the
    rocket's windows in order (the next window glows). A wrong letter floats back and the
    word is said slowly; a second makes the next letter glow. Built, the rocket flies off and
    the word is said slowly, then blended.
  - **Read and match:** a word written on a card and three pictures; she taps the picture.
    The word is never said before her first try; a miss lights its letters one by one with
    their sounds, and from then on she can tap the word to hear it.
  - **Word twins** (`party-1`): Romanian words on the left, English on the right; she taps
    one then its partner, or draws a line between them. Twins ("planetă / planet") say "Sunt
    gemeni!"; false friends ("pat / pat") show both pictures and the unicorn giggles "Se scriu
    la fel, dar înseamnă altceva!".
- **For the parent:** the word lessons and the planet party now play (only the writing steps
  wait for `tracing` and `handwriting`). Every extra letter in "build a word" is one she has
  met.

### Technical notes

- No new package; no save-format change (the word lessons' steps are unchanged, so a lesson
  in progress resumes where it was).
- **Content:** new lines `syll-tick`, `twins-hint-spelled`, `twins-hint-show` in both
  languages; the English `syll` and `syll-hint-once` say "tap the drum" (the brief: the drum in
  both languages). The validator: a `Syllables` word has 1 to 6 syllables; a `BuildWord` word at
  most 6 letters, its extra tiles taught and not in the word, its slow line present; a
  `ReadAndMatch` step's pictures differ; a `WordTwins` step's words pair up one to one (twin or
  false friend, never both), a false friend readable, as many Romanian as English.
- **Core:** `SyllablesRules`, `BuildWordRules`, `ReadAndMatchRules`, `WordTwinsRules`; the round's
  `Places` and `PlacesFilled`; the `Place` and `Join` gestures; the `Word` and `Control` looks; the
  `SpellOut`, `RevealPictures` and `ShowPlaces` cues; `ActivityLimits`.
- **For the human:** hear the new lines (rows 0.6.0-1 to -6); answer the three points in row
  0.6.0-6 (the tick, "bifă", a tap on another twin never a miss). The drum's sound, the soft
  sound of a miss and the rocket's take-off come with `sound-feel`.
- Brief: [`tasks/done/activities-words.md`](tasks/done/activities-words.md).

## 0.5.0 (2026-11-24)

Three new letter games in the lessons. She hunts the stars that carry the letter, finds
the letter a picture's word starts with, and, in the English lessons, hears that the same
letter sounds different in Romanian and in English and sorts pictures to the planet of
their sound. Every wrong choice is a letter she has already met.

- **For the child:**
  - **The letter hunt** (from `ro-m` and `en-m` on): ten yellow stars drift slowly across
    the sky, some with the letter (small and capital), the rest with a letter she knows;
    each star she finds pops and says its sound. A miss bounces back and the right stars
    twinkle; a second miss makes them all glow. They never pass over the home button or
    the unicorn.
  - **The first sound** (from `ro-m` and `en-m` on): a picture (its word not written, so
    it doesn't give the answer away) and its word said; she taps the letter it starts
    with. A miss says the word with its first sound held ("mmmama"), a second miss makes
    the answer glow. Tap the picture to hear the word again.
  - **One letter, two sounds** (`en-a`): the letter a between a gold Romanian planet and
    a blue English one; she taps each to hear its sound (a green tick each), then the
    pictures come one by one and she drags each to the planet of its first sound. A
    picture on the wrong planet floats back with both sounds said again; a second miss
    makes the right planet glow.
  - Every card and planet carries its language's badge (the picture in her hand gets its
    badge once sorted, so she listens instead of matching badges).
- **For the parent:** the first lessons are shorter: `ro-a` and `en-a` no longer hunt or
  ask for first sounds (only a is known there), and `ro-m` and `en-m` use m and a only
  (the human, 2026-11-24: wrong choices are only letters already taught).

### Technical notes

- No new package; no save-format change (`lesson-run.json` is unchanged; a lesson whose
  steps changed, `ro-a`, `en-a`, `ro-m`, `en-m`, starts again from its first step, as
  designed).
- **Content:** `steps.csv` (the four lessons above); a new voice line kind,
  **`stretched`** (`ro-stretched-mama` "mmmama", "t-t-tata", "p-p-pat"; `en-stretched-moon`,
  "t-t-tiger", "p-p-pat"), needed by every `FirstSound` word; `two-hint-show` in both
  languages. The validator: `LetterHunt` and `FirstSound` choose among taught letters only,
  `TwoSounds` only on a letter whose sound differs, with words of both languages starting
  with it. Until a stretched line is recorded, the tablet says the word's first sound, a
  pause, then the word.
- **Core:** `LetterHuntRules`, `FirstSoundRules`, `TwoSoundsRules`; the round's `Subject`,
  `StateOf`, `Current`; `ActivityAnswer.Drop`; `LearningRules.LetterHunt` (10 stars, 4 with
  the letter, at most 60 reference pixels a second, in `Settings/LearningRules.asset`).
- **For the human:** hear the new lines (rows 0.5.0-1 to -8), and record the six stretched
  words with the next recording session (`local/todo/04-record-voices.md`).
- Brief: [`tasks/done/activities-letters.md`](tasks/done/activities-letters.md).

## 0.4.0 (2026-11-24)

The first lessons she can play. **Hai!** opens a lesson: the unicorn introduces a
letter, says its sound and a word that starts with it, and asks her to tap the letter
and the picture; from the second lesson of each language she also hears a sound and
finds its letter among the letters she already knows. A miss gets a gentle hint, a
second miss shows the answer, and every line is said and shown in the unicorn's bubble.

- **For the child:**
  - **Meet a letter** (planet 1: a, m, t, p, in Romanian, then in English): the letter
    big and small, a picture card with its word, the gold star "RO" or the blue ringed
    planet "EN" on every card.
  - **Hear it, find it** (from `ro-m` and `en-m` on): 2 to 4 cream cards; a miss wobbles
    the card and fades it with "Ascultă încă o dată." and the sound; a second miss makes
    the answer glow ("Uite, aceasta este litera t. Atinge-o!"); right gets a sparkle
    and a praise, never the same one twice in a row. "Ascultă" says the question again.
  - **The unicorn** speaks every instruction, hint and praise, with the same words in
    its bubble; tap it to hear the last line again. Dots at the top show the steps.
  - **The end of a lesson:** "Ai terminat lecția. Bravo!" and a big yellow
    **Mai departe** back to the map; **Hai!** then opens the next lesson.
  - **Home** (top left) leaves at once; **Hai!** brings her back to the same step.
    When the tablet sleeps mid-sentence, the sentence starts again when she comes back.
- **For the parent:** the lessons play in the order of the content (`ro-a`, `en-a`,
  `ro-m`, …), skipping for now the activities still to be built (tracing, the letter
  hunt, first sounds, words); each later release adds them back to the same lessons.

### Technical notes

- **Core:** the lesson engine (`Scripts/Core/Lessons`: `LessonRun` with the one hint
  ladder for every kind, `LessonPlan`, `LessonOrder`, `LessonResume`,
  `LessonSignature`, `PraisePicker`, `LearningRules`, the events `AnswerChecked`,
  `HintGiven`, `StepCompleted`, `LessonCompleted`) and the activity contract
  (`Scripts/Core/Activities`: `IActivityRules`, `ActivityRulesFactory`,
  `MeetLetterRules`, `ListenAndPickRules`).
- **Unity:** `LessonRunner` (the Lesson scene's composition root), `LessonVoice`,
  `ActivityRegistry` (`Prefabs/Lesson/ActivityRegistry.asset`, with the kinds not built
  yet), the learning rules asset (`Settings/LearningRules.asset`: 2 tries, praise not
  repeated within 3), the lesson screen and the two views (`Prefabs/Lesson/`); the voice
  sampler is gone.
- **A new save file per profile:** `profiles/<id>/lesson-run.json` (version 1: the lesson
  in progress or next, the step to resume at, the signature of its steps).
- **Content** (agreed by the human, 2026-11-24): wrong choices are only taught letters,
  so `ro-a` no longer has a `ListenAndPick` step and `ro-m`'s is `m a`; the validator
  refuses a choice not taught yet, a choice listed twice, and a kind's missing hint
  lines; `MeetLetter` takes exactly one word. No new letter, word or voice line (no new
  id for the ledger).
- **For the human:** try the first lessons on the tablet (rows 0.4.0-1 to -9 below), and
  listen to the lines.
- The brief: [`tasks/done/lesson-engine.md`](tasks/done/lesson-engine.md).

## 0.3.1 (2026-11-24)

A chore for the content, with nothing new on the tablet. Her progress names
letters, words and lessons by their ids, so an id that has been released is
now kept for good. The pictures still to draw no longer give an answer away.

- **For the child:** nothing changes yet. The placeholder pictures (still on
  every picture until its art is drawn) are a plain coloured circle, with
  no letter. A letter on them would have told her the answer in "which
  sound does it start with?".
- **For the parent:**
  - **A list of every id ever released,** `content/released-ids.txt`. The
    content import (Școlița → Import content) refuses a change that
    deletes one of them, even when its imported file is deleted too, and
    asks for the row to be retired instead.
  - **Școlița → Record released ids** adds each release's new ids to the
    list. The agents run it when they close out a release.

### Technical notes

- **Editor** (`Scripts/Editor/ContentImport`):
  - `ReleasedIdLedger` reads the ledger (`<file> <id>` lines under
    `# <version>` lines; a missing, malformed, non-UTF-8 or repeated line
    is an error). `ContentImportPlan.Make` refuses a released id missing
    from its CSV.
  - `ReleasedIdsRecorder` adds the new ids, under `VERSION`, at a
    close-out (menu item and `RecordFromCommandLine`). It refuses when
    `VERSION` is not after the ledger's latest release.
  - `PlaceholderPicture.Render(pictureId)` draws a plain circle.
- **Content:**
  - `content/released-ids.txt` is seeded with the 331 ids released in
    0.2.0 (0.3.0 added none).
  - The 17 placeholder PNGs under `Assets/Scolita/Content/Pictures/` are
    re-imported.
  - No letter, word or voice line was added.
- **The close-out** of every release now runs Record released ids after
  raising `VERSION` (`AGENTS.md` → "Versioning and releases",
  `tasks/README.md` → "Lifecycle", `content/README.md` → "The ledger of
  released ids").
- **Tests:** EditMode 296 (was 280), PlayMode 6.
- No new package and no save-format change. The human has nothing to do
  but the 0.3.1 rows in `docs/manual-testing.md`, in Unity.
- **Brief:** [tasks/done/content-ledger.md](tasks/done/content-ledger.md).

## 0.3.0 (2026-11-24)

The app speaks. Every line has its voice:
- a recorded clip when there is one;
- else the tablet's own voice, in Romanian or British English;
- else a speech bubble with the words.

Tapping a letter or a word says it. A recording kit in Unity makes the
family's recordings into clips, one session at a time.

- **For the child:**
  - On the lesson screen (still a placeholder), a row of Romanian letters
    and words (gold star "RO") and a row of English ones (blue planet
    "EN"): m, a, t, p, a word, and the word said slowly.
  - Tapping one says it, and the tile pulses while it speaks.
  - Tapping another stops the first; two never talk at once.
  - A slow word ("m-a-m-a") is said as its letters' sounds, one after
    another.
- **For the parent:**
  - **The tablet's voice** says every line for now. It uses only voices
    installed on the tablet, so nothing it says leaves it. Check that
    Google's Romanian and English (United Kingdom) voices are installed:
    Settings → General management → Text-to-speech.
  - **Without a voice for a language,** the line shows in a bubble with its
    language's badge instead, and the app goes on.
  - **Recording in batch sessions** (`local/todo/04-record-voices.md`):
    - **Școlița → Make recording scripts** prints a numbered page per
      session.
    - You read one session into the Windows Voice Recorder.
    - **Import recording session** cuts the take into clips, levels them,
      and shows a page to listen to each one. It flags a missed or
      repeated line, or a clip that is too quiet or clipped.
    - **Accept recording session** puts the good clips into the app.
    - **Check voice clips** measures every clip.

### Technical notes

- **Core** (`Scripts/Core/Voice`):
  - `IVoice` and `VoiceRouter` (clip → system voice → bubble, driven by
    `Tick`, events `Started`, `Finished`, `PartStarted`, `LineMissing`);
  - `VoicePlanner`, `VoiceTemplate` (spliced `{x}` … slots),
    `VoiceSequence`, `LessonVoiceLines`.
- **Game** (`Scripts/Game/Voice`):
  - `RecordedVoice` (clips loaded per lesson);
  - `AndroidTextToSpeechVoice` (`TextToSpeech` through
    `AndroidJavaObject`, `ro-RO` and `en-GB`, installed offline voices
    only);
  - `SilentVoice` (the Editor), `SpeechBubbleView`, `VoiceHost`, `Hearable`;
  - prefabs `Prefabs/Voice/` (Voice, Hearable, VoiceSampler).
- **Content format:**
  - `voice-lines.csv` has a new `status` column, filled by the importer
    (`recorded`, `missing`, `sounds`, or empty);
  - the `clip` cell may name one clip per spoken part
    (`<id>.1.ogg <id>.2.ogg`);
  - the importer copies the clips into `Assets/Scolita/Content/Voice/Clips/`
    and links them.
- **The recording kit** (`Scripts/Editor/Voice`): WAV I/O, BS.1770
  loudness, splitting a take at its pauses, matching it to its script,
  levelling to −16 LUFS with a limiter, ffmpeg for M4A in and OGG out.
- **A new Editor-only tool, agreed by the human:** ffmpeg (an LGPL build,
  `C:\Tools\ffmpeg-n8.1.3-39-g660b5aa7db-win64-lgpl-8.1\`), named by
  `SCOLITA_FFMPEG`. It is never in the repo or the APK. Added to
  `AGENTS.md`'s stack.
- **The manifest** declares the `TTS_SERVICE` query (Android 11
  visibility). This is not a permission: the APK asks for no new one.
- No new package, no save-format change, no clips yet (all 279 lines are
  said by the tablet; the 8 slow words from their letters' sounds).
- **For the human:**
  - set `SCOLITA_FFMPEG` once (`local/todo/04-record-voices.md`, "Set up
    once");
  - check the tablet's voices;
  - record the first session when ready.
- Brief: [tasks/done/voice.md](tasks/done/voice.md).

## 0.2.0 (2026-11-24)

The lessons are now data. Every letter, word, picture, voice line,
planet, lesson and step is written in spreadsheets in `content/`. An
importer checks them and turns them into what the app will read.
Planet 1 (a m t p) is there as sample content. Nothing changes on the
tablet yet: the first playable lesson comes with `lesson-engine`.

- **For the child:** nothing new to see or hear yet; the app looks and
  behaves as in 0.1.0.
- **For the parent:**
  - **The spreadsheets:** planet 1's words and the lines the app will
    say can be read and edited in `content/` (any spreadsheet, saved as
    "CSV UTF-8").
  - **The import:** **Școlița → Import content** checks the content and
    names every problem by file, row and reason. For example, a word that
    uses a letter not taught yet, a cedilla ş instead of ș, or a missing
    line or picture.
  - The format is in `content/README.md`.

### Technical notes

- **The content model in Core** (`Scripts/Core/Content`, no Unity):
  - letters, words, pictures and voice lines;
  - domains, units (planets), lessons and steps;
  - a language's track, computed from its lessons;
  - which letters are taught before a lesson;
  - what each activity kind needs (`ActivityKinds`);
  - the CSV reader (BOM, CRLF, quotes, `,` or `;`);
  - every validator rule.
- **Seven CSVs** in `content/`: `letters`, `words`, `pictures`,
  `voice-lines`, `units`, `lessons` (one row per lesson) and `steps` (one
  row per step). Ids are stable: a row is retired, never deleted (the
  importer refuses a deleted one).
- **The importer** (`Scripts/Editor/ContentImport`; menu, or batch mode
  with exit code 1 on any error) writes one asset per id, updated in
  place, under `Assets/Scolita/Content` (`Letters/`, `Words/`,
  `Pictures/`, `Voice/`, `Domains/`), and five catalogs. It draws a
  placeholder for each picture without art (a coloured circle with the
  word's first letter). A freshness test fails if a CSV changes without
  importing.
- **Content added (planet 1, text only, no clips):**
  - 4 letters;
  - 18 words: mama, tata, pat, apa; map, mat, tap, pat; and 10
    listen-only words for the pictures;
  - 17 placeholder pictures;
  - 279 voice lines, including all 224 general lines of the agreed
    draft;
  - 12 lessons, 71 steps.
- **No new package, no save-format change.** `ProfileId` now shares the
  id check with the content.
- **For the human:**
  - the Design note's choices were agreed on 2026-11-24;
  - three small questions are open in `local/for-human-next-steps.md`
    (Q2 to Q4: a list of released ids, untaught letters as wrong choices,
    the placeholders' letter).
- Brief: [tasks/done/content-model.md](tasks/done/content-model.md).

## 0.1.0 (2026-11-24)

The first build of Școlița: the foundation every later feature sits on. It
opens on a placeholder map with a big "Hai!" button and a placeholder
lesson screen; no lessons yet.

- **For the child:** the app opens in landscape on a purple screen with
  "Școlița" and a big yellow "Hai!" button; "Hai!" opens a lesson screen
  showing ă â î ș ț in the reading typeface, and "Acasă" goes back. Nothing
  is spoken yet.
- **For the parent:** the app installs as "Școlița" (with ș and ț written
  correctly) and asks for no permissions and no internet. The version is
  shown small in the bottom-right corner of the map. The tablet may sleep
  on the map; it stays awake during a lesson.

### Technical notes

- The Unity project in `game/` (Unity 6000.3.29f1, Universal 2D, URP),
  with SeliSnake's settings: IL2CPP, ARM64, Android 8.0 (API 26) and up,
  landscape only, Input System only, no splash, services off.
- Packages: URP 17.3.0, Input System 1.20.1, uGUI/TextMeshPro 2.0.0,
  Localization 1.5.8, Test Framework 1.6.0, 2D Sprite 1.0.0, and the
  editor-only Visual Studio 2.0.26 and Rider 3.1.0 packages.
- Fonts (OFL 1.1): Andika 7.000 for reading and Baloo 2 1.700 for the
  app's words, as TextMeshPro atlases with ă â î ș ț and their capitals
  (comma below).
- Save files: `settings.json` for the tablet (music, sounds, voice volume,
  vibration, the app's language, Romanian by default) and
  `profiles/child-1/learning-settings.json` for the first profile (the
  language mix, both by default), version 1. A damaged file is set aside as
  `<name>.broken.json`; a file from a newer version or one that can't be
  opened is kept and never saved over.
- Release signing keeps the key's passwords out of Unity's settings,
  `build.gradle` and Gradle's caches (a file in `%TEMP%\scolita-signing\`
  read by the launcher's Gradle template, deleted after the build); the
  build refuses an APK that asks for the internet, and a release APK
  signed with the debug key or debuggable.
- Brief: [tasks/done/project-skeleton.md](tasks/done/project-skeleton.md).
