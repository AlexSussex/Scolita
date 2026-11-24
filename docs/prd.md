# Școlița — Product Requirements (v1)

Nov 23, 2026 · @Alex

A learning game for Android tablets, made for a child of about six: she
learns to read and write in Romanian and English at the same time, by
playing short games in a space world full of rockets, planets, rainbows,
unicorns, animals and music. It works fully offline, says everything out
loud, and is kind when she gets something wrong. Reading and writing come
first; numbers, music and other subjects come later on the same engine.
This is the authoritative v1 spec; the curriculum (`docs/curriculum.md`)
holds the teaching approach and the letter order, and the MVP Build Plan
holds the build order.

## 1. Overview

**What it is.** A galaxy of planets. Each planet is a unit of a few
letters; each stop on a planet is a lesson of 3 to 5 minutes made of short
activities: meet a letter (its sound, its name, a picture), find it, trace
it with a finger in print and **by hand** (the handwritten "de mână" form
taught at school), hear which word starts with it, build a word from
letters, read a word and match its picture, write a word by hand. Finishing a lesson earns stars
that light up constellations, and stickers for her sticker book.

**Who it is for.**

- **The learner is a child of about six,** who doesn't read yet or reads a
  little. Everything is spoken, pictures come first, and nothing needs
  reading to get around.
- **The parent** sets up the language mix, sees what she has learned, and
  records the family's voices for the letters and words.
- **More than one child, and the parent as a tester:** each has a profile
  with their own progress (the human, 2026-11-24).

**What matters most** (the human, 2026-11-23):

1. **Reading and writing in both languages at once,** so she learns them
   together.
2. **Fun:** a play mode she enjoys, in her favourite things: purple (her
   favourite colour), rainbows, unicorns, the moon, planet Earth and space, stars, space ships, animals,
   music and instruments, drawing and painting, cycling.
3. **Room to grow:** other domains (music and instruments first among the
   ideas) added later without a rewrite.
4. Offline and safe, always.

**MVP boundary.** v1 teaches the first three planets of letters in both
languages, with every activity kind, the galaxy map, rewards and the parent
area and profiles (several children, and a tester profile), on one tablet. The rest of the alphabet is content
added after the MVP, without code. Features are marked *(MVP)* or *(later)*
throughout.

**App identity**

- Name: Școlița ("the little school"). Android package id:
  `com.softshack.scolita`.
- Platform: Android tablets, landscape (the Galaxy Tab A7 10.4 first);
  phones work too; iOS later.
- Engine: Unity 6.3 LTS + C# (the human, 2026-11-23, chosen over Kotlin +
  Jetpack Compose for the game feel, the effects, and music later).
- Look: bright 2D with soft shading, in a purple space world (purple is
  her favourite colour), agreed 2026-11-23 (design/README.md,
  design/screens.md).

## 2. How she learns

The full approach is in `docs/curriculum.md`. In short:

- **Romanian is the anchor** (decision D1): it is written as it sounds, so
  its letters give quick wins; English comes beside it, with its different
  sounds shown openly.
- **Letters first shared by both languages,** in an order close to both an
  English phonics start (s, a, t, p, i, n, m, d) and a Romanian abecedar's
  first letters, so the first words work in both ("pat" is a bed in
  Romanian and a pat in English).
- **Each language is always marked** by a colour and a badge of its own
  shape; the two meet only in activities made for it.
- **Word twins** (words alike in both: rachetă / rocket, planetă / planet,
  pian / piano, unicorn / unicorn) are the bridges between the languages.
- **Sound before shape, shape before writing, writing before reading
  words.** Syllables matter in Romanian, so she claps them on a drum.
- **A letter is learned** after enough right answers across different
  games on at least two days; learned letters come back for short reviews.

## 3. Feature specification

### 3.1 The galaxy map *(MVP)*

- The home screen is a galaxy: **planets** (units) along a rainbow path,
  each with a few **stops** (lessons). The rocket sits on the next lesson;
  a big "Hai! / Go!" starts it. One tap to play.
- A planet shows its letters on its surface, in both languages' badges
  where the letter differs.
- Planets ahead are visible but sleepy (dimmed, with a gentle "zzz"), so
  she can see where she's going; she can always go back to any planet she
  has visited.
- Between planets the rocket flies a short trip (a few seconds, skippable)
  past stars, the moon and the Earth.
- **The mascot** (decision D5: a unicorn astronaut, named by her) greets
  her, speaks every instruction, and can be tapped to hear it again.

### 3.2 Lessons and activities *(MVP)*

A lesson is a list of steps; each step is one activity kind with its items.
The activity kinds (details in `docs/curriculum.md` §4):

| Kind | What she does | Skill |
| --- | --- | --- |
| `MeetLetter` | Sees and hears a new letter: its sound, its name, a picture that starts with it, in one language | sound–letter link |
| `ListenAndPick` | Hears a sound or a word, taps the right letter or picture among 2 to 4 (planets, bubbles, asteroids) | sound–letter, vocabulary |
| `LetterHunt` | Pops every star that carries the letter (upper and lower case mixed in) | recognising shapes |
| `FirstSound` | Sees a picture, hears its word, taps the letter it starts with | hearing the first sound |
| `TraceLetter` | Traces the letter with a finger, in print and then by hand (the handwritten form): on a dotted guide, on a faint guide, then from a start dot alone; stardust follows her finger | writing |
| `TraceWord` | Writes a short word by hand, its handwritten letters joined (mama, tata, pat) | writing words by hand |
| `Syllables` | Hears a word and taps the drum once per syllable (Romanian) or claps the beats (English) | syllables, rhythm |
| `BuildWord` | Drags letters into the rocket's windows to make a word she hears; the word is said slowly, then blended | spelling, blending |
| `ReadAndMatch` | Reads a word (it can be heard after a try) and taps its picture | reading |
| `TwoSounds` | One letter on two planets, one per language; she taps each to hear its sound, then sorts pictures to the planet whose sound they start with | the languages' differences |
| `WordTwins` | Matches a Romanian word with its English twin (rachetă ↔ rocket), hearing both | bridging the languages |

- **Feedback:** a right answer gets a happy sound, a sparkle and praise
  from the mascot (varied lines, in the lesson's language); a wrong one gets
  a soft sound, a wobble and a spoken hint; the second miss shows the
  answer and lets her do it with the answer shown. No buzzers, no lost
  stars, no timers.
- **A lesson ends** with its three stars (finishing is what counts; hints
  never take a star away) and, now and then, a sticker.
- **Leaving** with the home button keeps what was done; the lesson resumes
  at its step next time.

### 3.3 Writing *(MVP: print letters, handwritten letters, joined words by hand)*

She learns how letters are made **by hand**, not only how they look typed
(the human, 2026-11-24).

- Each letter has **stroke data** in the content: the strokes in the order
  a teacher shows, each with its start point and direction, in a box with
  the writing lines (the baseline, the middle line, the top line).
- The child traces on a large writing board; a **start dot** and an
  **arrow** show where and which way; the finger leaves a glowing rainbow
  trail with stardust.
- A stroke counts when it starts near the start dot, stays within a
  corridor around the stroke, goes the right way, and covers enough of it
  (tolerances in the learning rules asset, more forgiving at first). A
  stroke that strays fades away gently and the dot pulses again.
- Three rounds per letter: on a dotted guide, on a faint guide, then from
  the start dot alone. A finished letter comes alive (it bounces, sings its
  sound).
- **Handwritten letters** ("litere de mână", as taught in Romanian clasa
  I): every letter has a handwritten form next to its print one, with an
  entry stroke from the line and an exit stroke towards the next letter.
  In each letter's lesson she traces the print form, then the handwritten
  one; the `MeetLetter` card shows the handwritten form being written.
  Handwritten letters are drawn from the stroke data, so what she sees is
  exactly what she traces (decision D3: in the MVP).
- **Words by hand:** once a planet's letters are met, she writes its short
  words (mama, tata, pat) by hand, letter by letter, with the joins
  (`TraceWord`).
- Upper and lower case each have their own strokes; which comes first is
  decision D2. Joined capitals and free writing are *(later)*.

### 3.4 The voice *(MVP)*

- Everything the app says is a **voice line** in one language: letter
  sounds, letter names, words, instructions, praise and hints.
- **Recorded clips first** (the family's voices, decision D4), the
  tablet's own voice (Android's TextToSpeech, Romanian and English) for
  any line not recorded yet. The parent area shows how many lines still
  use the tablet's voice.
- Instructions are said in the language of the activity; the parent can
  choose that the app's own words (menus, the parent area) are in
  Romanian or English.

### 3.5 Progress and review *(MVP)*

- For each letter (and later each word) and each language, Core keeps how
  often it was right, in which activity kinds, and on which days.
- **Learned:** enough right answers across at least two activity kinds on
  at least two different days (numbers in the learning rules asset).
- **Review:** learned items come back as short warm-ups at the start of a
  lesson, on a growing schedule (after 1, 3, 7, 14 days, in the rules
  asset). A miss in review moves the item back.
- **The next lesson** is chosen by the map order; reviews are mixed into it,
  never a separate chore.

### 3.6 Rewards *(MVP)*

- **Stars** light up constellations in a "sky book": each planet's lessons
  fill one constellation (a unicorn, a rocket, a whale, a guitar).
- **Stickers** of animals, instruments, planets and space things, given
  now and then, for her sticker book; she can place them on a big space
  scene and move them around.
- **Rocket trails:** new rainbow trail colours for the rocket after each
  planet.
- Nothing is ever taken away, and nothing can be bought.

### 3.7 The parent area *(MVP)*

- Behind a **parent gate** (press and hold a button for 3 seconds, then
  pick the number said out loud from three written as words; numbers in
  the rules asset).
- **What she has learned:** a grid of letters by language (new, learning,
  learned), and the last days played.
- **Settings:** the language mix (Romanian only / English only / both,
  decision D1), the app's own language, upper or lower case first (D2),
  the session limit (off, 10, 15, 20 minutes, decision D8), music volume,
  sounds volume, voice volume, vibration on or off, and the app's version.
- **Voice status:** how many lines are recorded per language.
- **Reset progress** (asks to confirm twice, says what will be lost).

### 3.8 Profiles *(MVP)*

Several children (siblings, friends), and the parent as a tester, each
with their own progress (the human, 2026-11-24).

- **A profile** is a name typed by the parent, an avatar picked from the
  app's pictures (the unicorn astronaut, her purple cat, a rocket, a star,
  the moon …; never a photo), and a kind: child or tester.
- **Its own everything:** the place on the map, stars, stickers and the
  sticker scene, the letters learned and their reviews, the lesson in
  progress, and its learning settings (the language mix, letter case
  first, the session limit). Volumes, vibration and the app's own
  language belong to the tablet.
- **"Cine se joacă?" / "Who's playing?"** at start when there are two or
  more children: one big avatar card each, the name under it and spoken
  when tapped, so a child who can't read finds her own. With one child the
  app opens straight on the map. An avatar button on the map switches
  child.
- **Managed in the parent area:** add, edit (name, avatar), delete (asks
  twice); the progress grid and the settings show the chosen profile.
- **The tester profile** (the human): opened only from the parent area,
  never offered to the children; every planet and lesson open, a "skip
  step" button, a small overlay with the version, the lesson and step ids
  and the voice line playing, a one-tap reset of its own progress, and a
  "TEST" badge on the map. It never touches a child's progress.

### 3.9 Free drawing *(later, early)*

- A drawing pad ("Atelierul") with rainbow brushes, stamps of her stickers
  and the letters she has learned as stencils, saved on the tablet only.
  Built on the tracing brush. First after the MVP, because she loves
  drawing.

### 3.10 Other domains *(later)*

The same engine, new data (and an activity kind only where none fits):

- **Music and instruments:** hear and name instruments (pian / piano,
  vioară / violin, tobă / drum, chitară / guitar), high and low, loud and
  soft, clap a rhythm, play a short tune on a rainbow xylophone, then notes.
- **Numbers:** counting stars, numbers 0–20 in both languages, adding with
  rockets.
- **Space and Earth:** the planets, the moon's shapes, day and night.
- **Animals:** names, sounds, where they live, in both languages.
- **Reading further:** all the letters, Romanian groups (ce, ci, che, chi,
  ge, gi, ghe, ghi), English digraphs (sh, ch, th, ee, oo), short
  sentences, little stories read aloud with words lighting up.

## 4. Technical architecture

- **Unity 6.3 LTS, C#, URP 2D,** for Android (later iOS). Details and the
  locked stack: `AGENTS.md`.
- **The learning Core** in plain C# with no UnityEngine reference: the
  curriculum, lesson runs, every activity's rules, tracing checks,
  progress, mastery, review and rewards, all unit-tested; seeded
  randomness and an injected clock make it deterministic.
- **Content as data:** CSV files in `content/` (letters with their sounds
  and strokes, words with their syllables, pictures and clips, lessons
  with their steps, voice lines) are imported by an Editor tool into
  ScriptableObjects in catalogs; a validator checks every rule of the
  curriculum (only taught letters, both languages' clips, Romanian
  diacritics).
- **Unity side:** a composition root per scene (`LessonRunner` for a
  lesson) builds Core from the content and the profile, picks each step's
  view from `ActivityRegistry`, and forwards taps and strokes.
- **Voice:** `IVoice` with recorded clips and the Android system voice.
- **Saving:** small versioned JSON files per profile (progress, rewards,
  the profile's settings) under `profiles/<id>/`, and the profile list and
  the tablet's own settings beside them.
- **Performance target:** 60 fps on the Tab A7, a lesson ready within
  2 seconds, no memory allocation while an activity runs.

## 5. Safety and privacy

- No internet access (no `INTERNET` permission), no microphone, no camera.
- No accounts, no personal data, no analytics, no crash reporting services,
  no ads, no in-app purchases, no external links.
- Her name, drawings and recordings never go in the repository; the parent
  area is behind a parent gate.
- Profiles hold only a name typed by the parent and a built-in avatar, on
  the tablet only: no photos, no birthdays, nothing sent anywhere.

## 6. Distribution

- **MVP:** a signed APK installed on the tablet by the human (sideloading),
  updates installed over it with the same release key, keeping her
  progress.
- *(Later)* Google Play (Teacher Approved / family-friendly listing) if the
  human wants to share it; iOS through Apple's developer programme (needs a
  Mac).

## 7. Metrics of success

Not measured by the app (it collects nothing). Success is: she asks to play
it; she can start a lesson alone; she recognises and sounds out the letters
of the first three planets in both languages; she traces them in the right
direction, in print and by hand; she writes "mama" by hand, joined; she reads her first words (pat, mama, tata, sat, man); and the
parent can see where she is at a glance.

## 8. Roadmap and future iterations

| When | What |
| --- | --- |
| MVP | The galaxy map, 11 activity kinds, writing in print and by hand (handwritten letters and joined words), the first three planets of letters in both languages, the voice (recorded + system), progress and review, stars, stickers and trails, the parent area, profiles (several children and a tester), a signed APK |
| Right after | More planets (the whole Romanian and English alphabets, Romanian ă â î ș ț, English digraphs), the drawing pad |
| Later | Joined capitals and free writing, syllables to words to sentences, little stories read aloud, the music domain, numbers, space and animals, iOS, Google Play |
