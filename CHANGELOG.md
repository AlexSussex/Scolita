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
