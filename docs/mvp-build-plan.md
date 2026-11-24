# Școlița — MVP Build Plan

Nov 23, 2026 · @Alex

## MVP scope — build this first, defer the rest

The first APK has to do one loop end to end: open the app → the galaxy map
on the next lesson → "Go" → a lesson of short activities, everything spoken
→ stars and a sticker → back on the map, the rocket one stop further → the
next day, a review mixed in. Everything in the right-hand column waits.

| Area | In the first APK | Deferred to later |
| --- | --- | --- |
| Letters | Planets 1–3: a m t p, s i n, o d e (10 letters), in both languages | The rest of both alphabets, Romanian ă â î ș ț, groups (ce, chi, sh, th …) — content only, no code |
| Activities | All 11 kinds (`docs/curriculum.md` §4.2) | New kinds only for new domains |
| Writing | Print letters and their **handwritten forms** ("litere de mână"), lower case first, traced with a finger; short words written by hand, joined | Joined capitals, sentences, free writing without a guide |
| Reading | Single words of 2–6 letters | Sentences, little stories read aloud |
| Voice | Recorded clips + the tablet's voice for the rest | Her own voice recorded (needs the microphone; not planned yet) |
| Progress | Learned / learning / new per letter and language, reviews | Words' mastery, a weekly picture for the parent |
| Rewards | Stars and constellations, stickers and the sticker scene, rocket trails | Unlockable mascot outfits, the drawing pad |
| Profiles | Several children, each with their own progress; a tester profile for the parent (all lessons open, skip, a debug overlay); managed in the parent area | Sync between tablets, export of progress |
| Domains | Reading and writing | Music and instruments, numbers, space, animals |
| Text | The app's own words in Romanian and English | More languages |
| Platforms | Android tablets (landscape), phones work | iOS, Google Play |

## Recommended stack

Unity with C# was chosen on 2026-11-23 over Kotlin + Jetpack Compose: the
app should feel like a game (particles, a flying rocket, a glowing tracing
brush, bouncing letters), music is a likely next domain (Unity has
low-latency audio and timing built in), and Unity 6.3, the Tab A7 and the
agent workflow are already set up and proven by SeliSnake. The trade-offs
accepted: a bigger APK (about 40–60 MB against 15) and a slower first start
(3–5 seconds against 1), and a small Android bridge for the system voice.

| Layer | Choice | Why |
| --- | --- | --- |
| Engine | Unity 6.3 LTS, Unity Personal | Same version as SeliSnake on this PC; long-term support to December 2027; splash screen optional |
| Language | C# | Close to Java; one language for rules and views |
| Rendering | URP, 2D Renderer, sprites, particles | Bright 2D with effects runs well on the Tab A7 |
| Input | Input System (touch) | Raw touch points for tracing; multi-touch ignored where it must be |
| UI | uGUI + TextMeshPro | Mature; TextMeshPro handles ș ț with the right font atlas |
| Text | Unity Localization | The app's own words in both languages from day one |
| Voice | Recorded OGG clips + Android TextToSpeech through `AndroidJavaObject` | The family's voices are best for pure letter sounds; the system voice fills the gaps; no plugin, no permission |
| Content | CSV in `content/` → importer → ScriptableObjects | Letters, words and lessons edited in a spreadsheet, checked by a validator; adding a planet is data |
| Tests | Unity Test Framework | Every learning rule tested in milliseconds |
| Build | Batch-mode build script → APK | Agents build and test without the Editor |
| VCS | Git + Git LFS on GitHub | Art, voice and music in LFS |

## Simplifications and better ways — your call

| Decision point | Lightest MVP (recommended) | Trade-off / what you give up |
| --- | --- | --- |
| Anchor language (D1) | **Answered 2026-11-24:** Romanian first, English beside it, both on | English first would suit a child stronger in English; the order is data, so it can switch |
| Letter case (D2) | **Answered 2026-11-24:** lower case first to trace, capitals later in the same planet, both shown to read | Romanian abecedars show both together; upper case to trace comes right after |
| Handwriting (D3) | **Answered 2026-11-24:** print and handwritten letters both in the MVP, print first in each lesson; joined words by hand | The MVP grows by one brief (`handwriting`) and one activity kind (`TraceWord`) |
| Voices (D4) | **Answered for now, 2026-11-24:** the tablet's voice for everything; the family records later, in batch sessions | All system voice is quicker but poor for pure sounds ("m" said as "em") |
| Tracing check | Own stroke-corridor check in Core (start, direction, coverage, and the entry, exit and joins of handwritten letters) | A handwriting-recognition library would need the internet or a large model |
| Content editing | CSV + importer | A custom editor window is nicer but costs a brief |
| Rewards | Stars, stickers, trails | Coins and a shop would tempt "earning" over learning |
| Parent area | Hold-then-pick-a-number gate | A PIN is stronger but one more thing to remember |

## Local dev and testing on the tablet

- **The human's PC** runs Unity Hub, Unity `6000.3.29f1` with Android Build
  Support, and the repository in `C:\Projects\Scolita`. Agents work in their
  own worktrees next to it (`C:\Projects\scolita-<agent>`), each a Unity
  project with its own `Library/`.
- **Agents test** in the Editor's Play mode (Device Simulator at tablet
  sizes, including the Tab A7's 2000 × 1200) and on their own Android
  emulator (an x86_64 tablet AVD, Android 12 or newer), with debug APKs,
  and listen to every new voice line.
- **The human tests on the Tab A7:** `adb install -r Scolita-<version>.apk`,
  or copy the APK and tap it. The same release key for every APK, so
  updates keep her progress. Then she plays, and her reactions are the most
  useful test of all.
- **No server, no tunnel, no hosting:** the app needs nothing but the tablet.

## Look

Bright 2D with soft shading, in a purple space world (purple is her
favourite colour): a rainbow path between planets, a unicorn astronaut as
the guide, language badges of two shapes. Agreed by the human on
2026-11-23. Details: `design/README.md`; the eleven designed screens:
`design/screens.md` and the design canvas
(https://claude.ai/artifact/RVfBkheGcrfB824aq4cEEf).

## Build order

Each step is one feature: build it, test it, push it to `main`, release it.
The order makes the app usable early (a lesson after step 5) and better
after every step, with Core's rules before the screens that need them.
Progress is tracked in [`tasks/README.md`](../tasks/README.md).

1. **`feature/project-skeleton`** — the Unity project, folders, assembly
   definitions, packages, test setup, the build script reading `VERSION`
   (with signing that keeps the passwords out of Gradle), Core's `IRandom`
   and `IClock`, profile saving, the fonts with ă â î ș ț, the Boot → Map →
   Lesson placeholders. *Starts on the tablet.*
2. **`feature/curriculum-research`** *(docs only, beside step 1)* — checks
   `docs/curriculum.md` against a Romanian abecedar and an English phonics
   programme, writes v1 with the first three planets' words, and the human
   agrees it.
3. **`feature/content-model`** — the content types and catalogs, the CSV
   format, the importer, the validator (diacritics, only taught letters,
   pictures and clips), with planet 1's letters as sample data.
4. **`feature/voice`** — `IVoice`, recorded clips, the Android system
   voice, tap-to-hear on every letter and word, the voice lines list, the
   recording guide for the human.
5. **`feature/lesson-engine`** — Core's `LessonRun` and `IActivityRules`,
   kind feedback and hints, the Lesson scene with `ActivityRegistry`, and
   the first two kinds (`MeetLetter`, `ListenAndPick`). *Playable: a real
   lesson.*
6. **`feature/tracing`** — stroke data, Core's stroke check, the writing
   board with the rainbow brush, the three rounds. *Playable: writing.*
7. **`feature/handwriting`** — the handwritten forms ("litere de mână")
   with their entry and exit strokes, drawn from the stroke data, traced
   after the print form; `TraceWord`, writing short words by hand, joined.
   *Playable: writing by hand.*
8. **`feature/activities-letters`** — `LetterHunt`, `FirstSound`,
   `TwoSounds`.
9. **`feature/activities-words`** — `Syllables`, `BuildWord`,
   `ReadAndMatch`, `WordTwins`.
10. **`feature/galaxy-map`** — the planets, stops and the rocket, one tap to
   continue, the trip between planets, the mascot. *Playable: the whole
   loop.*
11. **`feature/progress-review`** — mastery, the review schedule, reviews
    mixed into lessons, the language mix.
12. **`feature/rewards`** — stars and constellations, stickers and the
    sticker scene, rocket trails.
13. **`feature/parent-area`** — the gate, what she has learned, the
    settings, the voice status, reset.
14. **`feature/profiles`** — several children and a tester profile, each
    with their own progress and learning settings; "Who's playing?"; adding,
    editing and deleting profiles in the parent area. *The first APK for
    her.*
15. **`feature/content-letters-1`** — planets 1–3 in full: letters, strokes,
    words, pictures, voice lines, lessons, print and handwritten strokes,
    in both languages. Can start
    after step 3, in parallel (data and art only), and finishes with the
    human's recordings.
16. **`feature/look`** — the mascot, the planets, the backgrounds, the
    sticker art, the language badges, in the chosen style. After step 1, in
    parallel (art only).
17. **`feature/sound-feel`** — music, effects, haptics, the music ducking
    under the voice, and a performance pass on the Tab A7. *The MVP is
    complete.*

After the MVP, each is a brief of its own: `content-letters-2` (planets
4–6), `content-letters-3` (the Romanian and English planets 7–9),
`drawing-pad`, `handwriting-2` (joined capitals, free writing), `sentences`, then the
`domain-music` design (large: the human agrees it first), `domain-numbers`.

## Open decisions

None blocks step 1. Answers go in `local/for-human-next-steps.md` → "Open
decisions" (the details are in `local/todo/03-decisions.md`).

- **D1. The anchor language and the mix** — *answered 2026-11-24:*
  Romanian first with English beside it, both on.
- **D2. Letter case** — *answered 2026-11-24:* lower case first to trace,
  capitals later in the same planet; print letters in the English
  top-start order (`docs/curriculum.md`).
- **D3. Handwriting** — *answered 2026-11-24:* handwritten letters and
  joined words by hand are in the MVP (step 7), in the Romanian school's
  "litere de mână" for both languages.
- **D4. Whose voices** — *answered for now, 2026-11-24:* the tablet's voice
  for every line to start with; the family (the human and her mother)
  will record later, once the recording is made quick (batch sessions, not
  one file per line); British English (`en-GB`). Who records which
  language is still open; neural voices for the words and sentences are
  being sampled for the human. Needed by step 15.
- **D5. The mascot** — *answered 2026-11-24:* the unicorn astronaut, named
  by her, with her purple cat from SeliSnake as the co-pilot. The guide is
  built as data, so more characters to choose from can come later as art
  only (`feature/guides`, after the MVP). Needed by step 16.
- **D6. The language badges** — *answered 2026-11-24:* Romanian a gold star
  "RO", English a blue ringed planet "EN", no flags. Needed by step 5.
- **D7. Where the art comes from** — *answered 2026-11-24:* agents draw it
  in the shaded 2D style, AI image tools where they help, and some of her
  own drawings turned into stickers. Needed by step 16.
- **D8. The session limit default** — *answered 2026-11-24:* off, set by
  the parent (10, 15 or 20 minutes). Needed by step 13.
- **D9. Profiles** — *answered 2026-11-24:* the 14 avatars of design screen
  11 (she picks hers and may swap any), and each child greeted by name
  with the tablet's voice ("Bună, {name}!"). Needed by step 14.

## Built to switch later — the five seams

The deferrals above are cheap to add later because the code has a seam at
the right spot now (`AGENTS.md` → "The five seams"):

- **10 letters → both alphabets, then sentences; print and handwritten.** Content is data:
  a planet is rows in the CSV files plus art, clips and stroke files (a
  letter's print and handwritten forms are both stroke data).
- **Reading → music, numbers, animals.** The lesson engine runs any
  domain's steps; activity kinds are reused, and a new one is one Core
  rules class and one view.
- **The tablet's voice → the family's voices.** Everything spoken goes
  through `IVoice`; a recorded clip replaces the system voice with no
  lesson change.
- **One child → several.** Saves live per profile from the first release,
  so `profiles` adds the list and the picker, not a migration of the data.
- **Rules apart from views.** Every learning rule is in Core, tested
  without a scene, so a new look or a new screen never changes how she
  learns.
