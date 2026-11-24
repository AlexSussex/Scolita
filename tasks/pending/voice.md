# Task: feature/voice

> **Status: In progress (voice).** Decision D4 (2026-11-24): the
> tablet's voice for every line to start with; the family (the human and
> her mother) records, in batch sessions, since none of the neural voices
> sampled (`local/voice-samples/2026-11-24/`) was right for every word.
> British English (`en-GB`). Items 8–10 (the recording kit) were added for
> that on 2026-11-24.

Read `AGENTS.md` first (especially "The five seams" → 4, "Audio" and
"Security and privacy"), and the completion records in `tasks/done/`.

**Feature:** `feature/voice`
**Implements:** PRD §3.4; `docs/curriculum.md` §8.
**Depends on:** `content-model`.

## Scope

1. **`IVoice`** (Core-facing interface, implemented on the Unity side):
   `Speak(VoiceLineId)`, `SpeakSequence(...)` (an instruction with a
   spliced letter or word: "Atinge litera" + "m"), `Stop()`, and events
   for started and finished (so effects wait for the voice, and the music
   ducks).
2. **`RecordedVoice`:** plays the line's clip from its catalog entry;
   loads a lesson's clips when the lesson starts and unloads them after.
3. **`AndroidTextToSpeechVoice`:** Android's `TextToSpeech` through
   `AndroidJavaObject` (no plugin, no permission), with the line's
   language (`ro-RO` or `en-GB`: decision D4, British English); checks
   at start whether each language is available and says so in the log;
   used for any line with no clip. In the Editor, a `SilentVoice` that
   logs the line stands in (or the Windows voice, if it costs nothing).
4. **`VoiceRouter`:** the composition picks the clip when there is one,
   the system voice when not, and silence (with a visible speech bubble of
   the text) when neither works; never a crash.
5. **Tap to hear:** a reusable `Hearable` component for any letter, word
   or picture on screen, which speaks its line and pulses while speaking.
6. **The recording guide** for the human, `local/todo/04-record-voices.md`
   (already drafted by the planner): check it against what was built (the
   file names, the folder, the loudness check) and add an Editor menu
   item **Școlița → Check voice clips** that reports each clip's length,
   loudness and silence at the ends against `AGENTS.md` → "Audio".
7. **`content/voice-lines.csv` status column:** recorded or not, filled by
   the importer from the clips found.
8. **Recording scripts** (Editor menu **Școlița → Make recording
   scripts**): from `content/voice-lines.csv`, the lines not yet recorded,
   grouped into sessions of at most about 50 lines (one language, one
   planet, sounds first, then words, syllables, instructions), each a
   printable, numbered page (HTML in `local/voice-inbox/scripts/`) with how
   to say each kind (from `local/todo/04-record-voices.md`) and a short
   test line at the start to set the level.
9. **Import a recording session** (Editor menu **Școlița → Import
   recording session**): one take per session (WAV; if the human's phone
   records only M4A or similar, the Design note proposes how to convert,
   for the human's agreement, since a new tool is not in the stack): split
   at the pauses, matched to the session's script in order, each clip
   trimmed, levelled and named per `AGENTS.md` → "Audio", and a report page
   to listen to every clip beside its text, with the ones that don't fit
   flagged (a missed or repeated line, too quiet, too long) so only those
   are recorded again. Nothing reaches `content/voice/` until the human
   says the session is good.
10. **Slow words from the recorded sounds:** a "slow" line (`m-a-m-a`) is
    played as its letters' sound clips in sequence with short pauses
    (through `SpeakSequence`), unless a clip of its own exists, so the
    slow words need no recording.

## Out of scope — do not build

- Recording in the app (no microphone).
- Music and effects (`sound-feel`), beyond the ducking events.

## Acceptance criteria

- On the emulator, a line with a clip plays the clip; a line without one
  is spoken by the system voice in its language; with the system voice's
  language missing, the bubble shows the text and the app goes on.
- Tapping a `Hearable` letter while another line plays stops the first and
  plays the new one (no overlap).
- **Check voice clips** flags a clip that is too quiet, clipped, or has a
  second of silence at its start.
- No new Android permission in the APK.
- A test session (an agent's own voice-test take of 10 lines, never the
  family's voices) imports into 10 named, trimmed, levelled clips; a take
  with a line missed is flagged, not silently shifted.
- A slow word with no clip of its own plays its letters' sounds in order.

## Tests required

- EditMode: `VoiceRouter`'s choice (clip, system, bubble) with
  `FakeVoice`s; the sequence splicing; the loudness checker on test clips.
- PlayMode: a `Hearable` speaks its line on a tap.
- Emulator: the system voice in Romanian and English (install the Google
  TTS Romanian voice on your own AVD only); listening check of the sample
  lines.

## Touches

`Scripts/Core/Voice`, `Scripts/Game/Voice`, `Scripts/Editor/Voice`,
`content/voice-lines.csv` (a column), the `Hearable` prefab (new),
`local/todo/04-record-voices.md` (the recording guide, rewritten for batch
sessions).

## Screens

None new (a test scene in `Tests/PlayMode` only).

## Voice lines

None new; the planet 1 sample lines are heard through both voices.

## Before closing out

- `local/for-human-next-steps.md`: "record the first lines" (after
  `curriculum-research` is agreed), and "on the Tab A7, check the Google
  voice has Romanian and English downloaded" (Settings → General management
  → Text-to-speech).
- Release 0.3.0.

## Progress

_Kept by `voice` while the brief is in flight._

### Status

- **Claimed** in `13ea043` (worktree `C:\Projects\scolita-voice`, emulator
  `emulator-5606`).
- **The converter, agreed by the human on 2026-11-24 (through the lead):**
  option A, ffmpeg, in the Editor only, never in the repo or the APK. The
  pinned build is BtbN's `ffmpeg-n8.1.3-39-g660b5aa7db-win64-lgpl-8.1.zip`,
  from the release `autobuild-2026-11-23-12-43`. The zip's SHA-256 is
  `f0ca6f38361a853baaa010df76e62444e862437c29fbf09b13b008040641d10c`, and
  `bin/ffmpeg.exe`'s is
  `51f05f24611d447de5b562ed2f0daf72327f7f1413ca0e83086324b9d00f74dd`. It is
  installed in `C:/Tools/ffmpeg-n8.1.3-39-g660b5aa7db-win64-lgpl-8.1/` and
  found through `SCOLITA_FFMPEG`. Takes are **M4A from the Windows Voice
  Recorder app** (the main path); WAV still works. Added to `AGENTS.md`'s
  stack as an Editor-time tool.
- **Built and tested; pushed in four commits** (core, content, build,
  game), then this `docs:` commit. The hashes are in "Commits".

### Design note (medium change: a new system)

**The voice in the app.**

- **Core (`Scripts/Core/Voice`, `Scolita.Core.Voice`), plain C#:**
  - `IVoice`, what the app calls: `Speak(VoiceLineId)`,
    `SpeakSequence(VoiceSequence)`, `Stop()`, `IsSpeaking`, and the events
    `Started` and `Finished` (each with the line's id; `Finished` also comes
    when a line is stopped or replaced, so ducking always ends).
  - `VoiceSequence`: lines one after another, or a line with spliced
    parts. `VoiceSlot` names `{x}`, `{y}`, `{word}`, `{slow}`,
    `{stretched}` and `{name}`. A `VoiceFill` is another line (a sound, a
    word) with the text shown for it, or a plain text (a profile's name).
  - `VoiceTemplate`: splits a line's text at its slots. Its **spoken
    segments** are the text pieces with a letter in them. "Atinge litera
    {x}." has one ("Atinge litera"); a "." or "!" alone isn't said.
  - `VoiceRouter : IVoice`: the one owner of the choice. For each piece:
    - **the clip** when the line has one for that piece and the clip
      player has it;
    - else **the system voice** with the piece's text, in the line's
      language, when that language is available;
    - else **the bubble** with the text, held for a reading time.

    Neighbouring system-voice pieces in one language are said as one
    utterance, so "Bună, {name}!" keeps its tune. A **slow line** with no
    clip of its own is its word's letters' sound lines, with a pause
    between them (item 10). The router is driven by `Tick(seconds)` from
    the Unity side, so pauses and bubble times are deterministic in tests.
    A line id that isn't in the content is reported through an event
    (logged as an error on the Unity side) and skipped, never a crash.
  - What the router drives: `IClipVoice` (`Has`, `Play`, `Stop`,
    `IsPlaying`), `ISystemVoice` (`Speaks(Language)`, `Say`, `Stop`,
    `IsSaying`) and `ISpeechBubble` (`Show`, `Hide`). A refused clip or
    utterance falls to the next way.
  - `LessonVoiceLines`: the lines a lesson can say (its letters' sounds
    and names, its words' lines, its kinds' instruction lines), so the
    clips are loaded per lesson.
  - The timings (the pause between sounds, the bubble's reading time) are a
    `VoiceTiming` value set on the Voice prefab, not in code.
- **Game (`Scripts/Game/Voice`):**
  - `RecordedVoice : IClipVoice`: one `AudioSource`; the clips come from
    the voice line assets. Clips are imported with *Preload Audio Data* off
    and *Decompress On Load*. `LoadLesson(lines)` calls `LoadAudioData` for
    a lesson's clips, `UnloadAll()` frees them, and a clip asked for
    outside a lesson is loaded when it is played. Nothing allocates while
    it plays.
  - `AndroidTextToSpeechVoice : ISystemVoice`: Android's `TextToSpeech`
    through `AndroidJavaObject` and two `AndroidJavaProxy`s
    (`OnInitListener`, and the interface `OnUtteranceCompletedListener`,
    since `AndroidJavaProxy` can't extend the abstract
    `UtteranceProgressListener`; Android calls it on done, error and stop).
    It uses `ro-RO` and `en-GB` (D4). At init it logs whether each
    language is available, and `Speaks` is false for a missing one. No
    plugin and no permission.
  - `SilentVoice : ISystemVoice` (the Editor and tests): speaks no language,
    so the router shows the bubble, and the host logs the line.
  - `SpeechBubbleView : ISpeechBubble`: the bubble (look's `bubble`
    sprite), the text in Baloo 2, and the language badge (colour **and**
    shape, look's RO/EN badges).
  - `VoiceHost`: the `Voice` prefab's MonoBehaviour. It builds the router
    from the curriculum its scene's composition root gives it, ticks it,
    and logs. It is not a singleton and not found by name.
  - `Hearable` (prefab `Prefabs/Voice/Hearable.prefab`): a tap speaks
    its line through the `IVoice` it was bound to (by the composition
    root), and it pulses while that line plays. A tap while another line
    plays replaces that line (the router stops the old one first).
- **On screen for now:** a small voice sampler on the Lesson placeholder
  (`Prefabs/Voice/VoiceSampler.prefab`, wired by `LessonRunner`). It holds
  planet 1's letters, a word and a slow word in each language, with their
  badges, so the emulator and the human's tablet can hear both voices.
  `lesson-engine` replaces it with the real lesson.

**The content (the importer gains a column; fresh-eyes review).**

- `voice-lines.csv` gets a **`status`** column, filled by the importer
  from the clips found:
  - `recorded`: every clip named is in `content/voice/`;
  - `missing`: a clip is named but isn't there;
  - `sounds`: a slow line with no clip, said from its letters' sounds;
  - empty: still to record (the system voice says it).

  The importer writes only the status cells back into the CSV, byte for
  byte elsewhere: the CSV reader now keeps each cell's place in the text.
  The validator fails on a stale status ("run Import content").
- **The `clip` cell** may name one clip per spoken segment, separated by
  spaces. A line without slots has one clip, `ro-line-try-again.ogg`.
  "Bravo, ai făcut litera {x}! Acum leag-o de litera {y}, …" has three:
  `<id>.1.ogg <id>.2.ogg <id>.3.ogg`. The dot can't clash with an id. The
  validator checks the count, and that each name is `<line id>.ogg` or
  `<line id>.<n>.ogg`.
- **The importer copies** each named clip into
  `Assets/Scolita/Content/Voice/Clips/` (byte for byte, mono, Vorbis,
  Decompress On Load, Preload off) and links them as `AudioClip`s on the
  voice line assets. A copy whose clip is no longer named is removed (a
  clip isn't an id). The freshness test covers the copies and their import
  settings.

**The recording kit (Editor, `Scripts/Editor/Voice`).**

- **Check voice clips:** for each clip, its length, integrated loudness
  (ITU-R BS.1770-4: K-weighting, 400 ms blocks, gated), sample peak, and
  the silence at each end. The targets are AGENTS.md → "Audio": −16 LUFS
  ±1.5, peak under −1 dBFS, 100 ms of silence (flagged under 50 or over
  250 ms), and a length that suits its kind. It flags a clip that is too
  quiet or too loud, clipped (3 or more samples in a row at full scale), or
  has too much silence. The thresholds sit in one place,
  `VoiceClipStandard`. The clips are read through Unity's own import
  (`AudioClip.GetData`), so no decoder is needed.
- **Make recording scripts:** the lines still to record (status empty),
  one spoken segment per item. Grouped by language, then by the first
  planet that uses the line (from the lessons; lines that no lesson names
  go in a last "everywhere" group), then by kind: sounds, names, words,
  syllables, slow lines with no letters, then lines. Sessions hold at
  most 50 items. Each session is a printable numbered page
  (`local/voice-inbox/scripts/<session>.html`), with how to say each kind
  and a test line first to set the level, plus a manifest
  (`<session>.json`) that the import reads.
- **Import recording session:** reads `local/voice-inbox/<session>/` (the
  take, `take.wav`, and the manifest). It:
  - finds the pauses (a level threshold between the take's noise floor and
    its speech, and a pause of 1 s or more; the page asks for about 2 s
    between lines);
  - drops the test line;
  - matches the pieces to the items in order. If the count doesn't match,
    it flags the miss or the repeat, never shifts silently: with one piece
    too many or too few, it aligns by expected duration and flags every
    item whose place is uncertain. With two or more, it flags them all;
  - trims each clip to 100 ms of silence with 5 ms fades, makes it mono,
    resamples it to 44.1 kHz and levels it to −16 LUFS (lower if the peak
    would pass −1 dBFS);
  - writes the clips as WAV in `local/voice-inbox/<session>/clips/`, and a
    report page with each clip's player beside its text and flags (missed,
    repeated, too quiet, too long or short, clipped).

  **Accept recording session** (only when the human says the session is
  good): copies the clips that aren't flagged into `content/voice/`,
  writes their names into the `clip` cells, and runs the content import.
- **Tests:** EditMode, on signals made in code (tones, noise and silence
  at known levels; no audio committed): the WAV reader and writer, the
  loudness and peak meters (a 997 Hz sine at amplitude A reads
  20·log10(A) − 3.01 LUFS), the silence finder, the resampler, the
  splitter, and the matching with a line missed or repeated. The agent's
  own 10-line test take, from Piper (never a family voice), is run by
  hand.

**The recording kit's formats (a question for the human).** Everything
above runs on WAV in plain C#: 16, 24 or 32-bit PCM, or 32-bit float,
mono or stereo, at any rate. Two steps need a converter, which is not in
the stack. The first writes the accepted clips as **OGG Vorbis** (Unity
can play OGG but not write it). The second reads a phone's **M4A/AAC**
take. The options proposed:

- **(A) ffmpeg** (recommended): an LGPL Windows build, used in the Editor
  only, at a pinned version, found through `SCOLITA_FFMPEG`, never in the
  repo or the APK. It does both steps.
- **(B) No new tool:** keep the accepted clips as WAV in `content/voice/`
  (Git LFS, about 5–10 times the size). Unity encodes them to Vorbis in
  the APK, so the app is the same. AGENTS.md → "Audio" would say WAV, and
  takes must be WAV (Audacity, or a phone app that records WAV).
- **(C) oggenc2:** OGG only, no M4A.

Until the answer comes, the accept step stays behind one encoder seam
(`IClipEncoder`), and the rest is built and tested.

**Another domain.** Music and numbers reuse it as it is: a number's name
or an instrument's name is a voice line. An instrument's own sound isn't a
voice line, so it goes through `sound-feel`'s effects, not `IVoice`.

### Commits

- `8810c7d` `core:` the voice router, spliced lines, statuses, the CSV cell writer, their tests;
- `8665f27` `content:` the `status` column and the re-imported slow lines;
- `9816f74` `build:` the importer's clips and status, the recording kit, the TTS query in the manifest;
- `7f2728d` `game:` the voice, `Hearable`, the bubble, the prefabs, the Lesson's sampler, PlayMode tests;
- this `docs:` commit (Progress, How to test locally, `AGENTS.md`'s audio tool).

### What was built (beyond the Design note)

- **Core:** `Scripts/Core/Voice/` holds:
  - `IVoice`, `VoiceRouter` (with `PartStarted`, which says how each part is
    really said), `VoicePlanner`, `VoiceTemplate`, `VoiceSequence`, `VoiceFill`,
    `VoiceSlot(s)`, `VoiceTiming`, `VoiceUtterance`;
  - `IClipVoice`, `ISystemVoice`, `ISpeechBubble`, and `LessonVoiceLines`.

  `Scripts/Core/Content/` gains:
  - `VoiceLine.Clips` and `VoiceLine.Status`, with `VoiceLineStatus`,
    `VoiceLineStatuses` and `VoiceClipNames`;
  - `Csv/CsvCellWriter`, with the reader's cell spans (`CsvSpan`,
    `CsvRecord`);
  - the clip and slot rules in `VoiceLineRules`.
- **Content:**
  - `voice-lines.csv` has the `status` column, after `clip`. The eight slow
    lines say `sounds`; everything else is still to record.
  - `VoiceLineAsset` keeps the status and links its `AudioClip`s.
  - `content/README.md` documents both columns.
- **Build:**
  - `ContentImportPlan` fills the status cells (written back only on
    `Apply`, nothing on an error), copies the named clips into
    `Content/Voice/Clips/` (mono, Vorbis 0.7, Decompress On Load, Preload
    off) and removes stale copies. Its freshness test covers both.
  - The kit in `Scripts/Editor/Voice/`, with its four menu items and their
    batch forms in `RecordingKitMenu`.
  - The launcher manifest declares the `TTS_SERVICE` query (Android 11
    package visibility; not a permission).
- **Game:**
  - `Scripts/Game/Voice/` (`RecordedVoice`, `AndroidTextToSpeechVoice` with
    `TextToSpeechInitListener` and `TextToSpeechUtteranceListener`,
    `SilentVoice`, `SpeechBubbleView`, `VoiceHost`, `Hearable`);
  - the prefabs `Prefabs/Voice/Voice.prefab`, `Hearable.prefab` and
    `VoiceSampler.prefab`;
  - `LessonRunner` builds the curriculum and binds the sampler;
  - `Scolita.Game` references `UnityEngine.UI` (for the EventSystem tap and
    the bubble's Image).

### Decisions and deviations

- **Only installed, offline system voices.** On the emulator, Google's
  engine said Romanian was "available" with no voice installed, and spoke
  through its **server** voice (`ro-ro-x-vfv-server`). That sends the text,
  a spliced name included, off the tablet, and fails offline. So
  `AndroidTextToSpeechVoice` picks a voice per language from `getVoices()`
  that needs no network and isn't marked `notInstalled`. A language
  without one shows the bubble (the brief's "language missing"). The
  voices are checked again when the app comes back, so a voice installed
  meanwhile is used.
- **`OnUtteranceCompletedListener`** (deprecated, but an interface) and not
  `UtteranceProgressListener` (an abstract class, which `AndroidJavaProxy`
  can't extend). Android calls it on done and on error, not on stop, so
  `Stop` clears the wait itself. A lost callback gives up after 4 s plus
  0.2 s per character.
- **A sound line through the system voice** says its text without the
  slashes ("m"). No synthetic voice says pure sounds right (D4), so the
  sounds wait for the family's clips.
- **Spliced lines** are recorded one clip per spoken part (`<id>.1.ogg`,
  `.2.ogg`, …). Lines with `{name}` stay with the system voice (D9) and are
  not listed in the scripts.
- **A slow line** is said from its letters' sounds unless it has a clip of
  its own. The scripts never list it.
- **The voice sampler on the Lesson placeholder** (not in the brief's
  Touches; added to the claim). Without it, nothing in the APK speaks, and
  the emulator and the human's tablet couldn't check the two voices.
  `lesson-engine` replaces it. `Lesson.unity` changed only by adding two
  root prefab instances and setting `LessonRunner`'s fields.
- **The kit levels with a peak limiter** (5 ms look-ahead, 50 ms release),
  as −16 LUFS with peaks under −1 dBFS is out of reach for speech without
  one. Splitting uses a threshold between the noise floor and the speech,
  never more than 40 dB under the speech, so a take in digital silence
  doesn't count faint artefacts as sound.
- **The development build logs each part said** (`Voice: Clip
  ro-sound-m.ogg`, `Voice: System en, 3 characters`). It logs the length,
  never the text, so a spliced name isn't logged. Release builds log
  nothing.

### Fresh-eyes review (a second agent, before the push)

Findings and how each was resolved:

1. **No `<queries>` for `TTS_SERVICE`** in the launcher manifest. Added.
   The emulator (API 36) worked without it; Android 11 visibility rules
   recommend it.
2. **Re-entrancy: a `Finished` handler that speaks while a line is
   replaced** could leave two lines going. `Speak` and `SpeakSequence` now
   stop first and return if a newer line started meanwhile
   (`StopSpeakingFor`).
3. **Neighbouring texts joined with no space** across lines of a sequence.
   A space is now added when the next text starts with a letter or digit
   and the last one doesn't end in a space.
4. **The CSV writer's limits** (a rewritten quoted cell loses its quotes;
   values can't need quotes). These hold for the status codes, and the
   writer refuses a value that would need quotes. Left as is, documented.
5. **Equal counts never flagged.** A skipped line plus a noise gives equal
   counts. Now any piece more than 3 times longer or shorter than expected
   is flagged `Uncertain`. Accept takes only clips with no flag.
6. **ffmpeg's timeout never fired** (output read synchronously). It now
   reads asynchronously and waits with the timeout.
7. **The limiter has an instant attack** (no ramp). Kept; its doc no longer
   says "gentle". Follow-up if a click is ever heard.
8. **The development log printed spoken text.** It prints the length now.
9. **The timings are prefab fields.** Accepted by the reviewer.
10. **The voices are listed on the first line said** (a JNI walk, possibly
    over 100 ms once). Follow-up: warm it when the engine is ready.
11. **`OnDestroy` raising `Finished` into destroyed Hearables.** Hearable
    unsubscribes in `OnDisable`, so this is safe.

### Test results (2026-11-24, at the final state, before the rebase)

- **EditMode** 280/280 (`editmode.xml`) and **PlayMode** 6/6 in batch mode.
  `BuildAndroidDebug` succeeded. No `warning CS` or `error CS` in any log.
  `aapt dump permissions` lists only `DYNAMIC_RECEIVER_NOT_EXPORTED_PERMISSION`:
  no new permission.
- **Emulator** (`emulator-5606`, `Scolita_Tablet_voice`: Pixel Tablet,
  Android 16 with Play, x86_64, `-force-gles`):
  - **Clips** (with the nine test clips built in, then removed):
    - RO m and mama played `Clip ro-sound-m.ogg` and `Clip ro-word-mama.ogg`;
    - m-a-m-a played four clips with 0.35 s pauses;
    - m-a-m-a then m at once: the slow word stopped and m played. No
      overlap.
  - **Language missing:** en-GB had no installed voice ("no installed en-GB
    voice" in the log). EN m, map and m-a-p showed the bubble with the EN
    badge and the text (`clips-en-map.png`), and the app went on.
  - **System voice** (the final APK, no clips; Google's ro-RO and en-GB
    voices installed on this AVD only):
    - "speaks ro-RO with ro-ro-x-vfv-local" and "en-GB with
      en-gb-x-gba-local";
    - every RO and EN tile went `System ro` / `System en`;
    - each slow word's letters were said one at a time with pauses, driven
      by the completion callbacks;
    - `dumpsys audio` shows Google TTS's players starting.
  - **Pause:** Home in the middle of m-a-m-a stopped the voice within about
    a second (`OnApplicationPause`). Nothing went on talking.
  - **Screen sizes:** 2000 × 1200 and 1280 × 800 (`lesson-2000.png`,
    `lesson-1280.png`). The RO row is top right of the home button, the
    card is in the middle, the EN row is at the bottom, and nothing
    overlaps. The tiles are 112 reference pixels (about 112 dp on the Tab
    A7), and the bubble is centred. Only these two sizes were checked: the
    sampler is a placeholder row in a layout already checked by `look`.
- **Recording kit** (test takes by Piper `ro_RO-mihai-medium`, never a family
  voice, encoded as M4A, through ffmpeg):
  - **10 lines:** "11 pieces for 11 lines: they match", 10 clips at
    −15.6…−16.8 LUFS, peak ≤ −1.1 dBFS, 100–130 ms of silence. One was
    flagged (Piper's soft "pe" left 310 ms at its start).
  - **A line missed:** "10 pieces for 11 lines: a line seems to be missed",
    and every line flagged (all short and alike), so none was shifted
    silently.
  - **Accept:** 9 OGG Vorbis clips (mono, 44.1 kHz) went into
    `content/voice/`; the CSV named them `recorded`; the importer copied and
    linked them; the APK played them. They were all removed afterwards.
  - **Check voice clips:** read 9 clips, and flagged a 1 s late start, a
    clip 20 dB quieter and one 20 dB louder (clipped).
  - **Without ffmpeg:** the clear message, never a crash.
- **Listening:** an agent can't hear the emulator. The evidence is in the
  logs above: the route of each part, and the TTS players starting. The
  human listens on the tablet (manual tests 0.3.0).
- **Not run:** the Editor's Play mode and Device Simulator (batch mode only),
  and the Profiler. The router's `Tick` is allocation-free by construction
  (no LINQ, strings or closures); a JNI call and its strings happen once per
  line said, not per frame.

### Follow-ups

- **Warm the system voice's voice list** when the engine is ready, not on the
  first line (review 10).
- **A smoothed attack in the kit's limiter**, if a click is ever heard
  (review 7).
- **The guide's mouth and the words in its bubble** can use
  `VoiceRouter.PartStarted` (`look` / `lesson-engine`).
- **Pauses in a slow word** carry Romanian as their language whatever the
  word's (cosmetic, only in the development log).
- **`lesson-engine`:**
  - load a lesson's clips with `VoiceHost.LoadLines(LessonVoiceLines.Of(…))`
    when it starts;
  - replace the voice sampler.
- **`sound-feel`:** duck the music on `IVoice.Started` and `Finished`.
- **The Tab A7:** check that the Google voices for Romanian and English are
  installed (Settings → General management → Text-to-speech).

## How to test locally

Commands are for Git Bash from the worktree (or checkout) root, with
`UNITY="C:/Program Files/Unity/Hub/Editor/6000.3.29f1/Editor/Unity.exe"`,
`ADB="$LOCALAPPDATA/Android/Sdk/platform-tools/adb"`, `EMU=emulator-5606`
(your own emulator), `S=<your scratch folder>`, and
`export MSYS_NO_PATHCONV=1 ADB_LOCAL_TRANSPORT_MAX_PORT=5699`. Close your Editor
on this project before any batch-mode run. For the recording kit, ffmpeg must be
named by `SCOLITA_FFMPEG` (for a test in one shell:
`export SCOLITA_FFMPEG='C:\Tools\ffmpeg-n8.1.3-39-g660b5aa7db-win64-lgpl-8.1\bin\ffmpeg.exe'`;
the human sets it once for good, `local/todo/04-record-voices.md`). Start the app on
the emulator with
`"$ADB" -s $EMU shell am start -S -n com.softshack.scolita/com.unity3d.player.UnityPlayerGameActivity -e unity -force-gles`.
Reset with `"$ADB" -s $EMU shell pm clear com.softshack.scolita`.

### Core slice

1. **EditMode tests:**
   `"$UNITY" -batchmode -nographics -projectPath game -runTests -testPlatform EditMode -testResults $S/editmode.xml -logFile $S/editmode.log`.
   Expected: every test passes (no `warning CS` or `error CS` in the log), among them:
   - `VoiceRouterTests`: a clip when there is one, the system voice in the line's
     language when not, the bubble when the language is missing (and it hides after its
     reading time), the fallbacks when a clip or the system voice refuses, "Atinge
     litera" + the sound clip of m, "Bună, Ana! Hai să zburăm!" as one utterance, the
     pauses at "…", a slow word from its letters' sounds (or its own clip), a new line
     stopping the one being said (Finished, then Started), a missing line reported
     and skipped, a slot without its fill refused;
   - `VoiceTemplateTests`, `LessonVoiceLinesTests` (the real `ro-m` lesson),
     `VoiceLineStatusesTests`, `CsvCellWriterTests` (one column rewritten, every other
     byte kept: BOM, CRLF, quotes, `;`), `VoiceLineClipRulesTests`.
2. **A broken clip cell fails the import:** in `content/voice-lines.csv`, write
   `ro-sound-m.ogg ro-sound-m.2.ogg` in the `clip` cell of `ro-sound-m`, then run
   `"$UNITY" -batchmode -nographics -quit -projectPath game -executeMethod Scolita.Editor.ContentImport.ContentImporter.ImportFromCommandLine -logFile $S/import.log; echo $?`.
   Expected: exit 1 and `voice-lines.csv, row 4: the clip cell names 2 clips, but the
   text has 1 spoken parts` in the log. Undo with `git checkout content/voice-lines.csv`.

### Content slice

1. **The status column:** open `content/voice-lines.csv`. Expected: a `status` column
   after `clip`; the eight slow lines (`ro-slow-mama` …) say `sounds`; every other
   cell is empty (nothing is recorded yet).
2. **Fresh:** run the import (Core slice, step 2, without the change). Expected: exit
   0, "imported, with 17 warnings", and `git status` shows nothing changed.

### Build slice (the importer and the recording kit)

1. **The kit's tests** are in the EditMode run (Core slice, step 1): `LoudnessTests`
   (a 997 Hz sine of peak A reads 20·log10(A) − 3.01 LUFS; a too quiet, a clipped and
   a late-starting clip are flagged), `WavFileTests`, `TakeMatcherTests`,
   `RecordingSessionImportTests` (a made-up ten-line take gives ten clips at −16 LUFS
   with 100 ms of silence; with a line missed, every clip whose place is in doubt is
   flagged) and `RecordingSessionPlannerTests` (the real content).
2. **Make recording scripts:**
   `"$UNITY" -batchmode -nographics -quit -projectPath game -executeMethod Scolita.Editor.Voice.RecordingKitMenu.MakeScriptsFromCommandLine -voiceInbox $S/inbox -logFile $S/scripts.log`.
   Expected: exit 0, "8 sessions, 281 lines to record … ro-planet-1-1, ro-everywhere-1,
   … en-everywhere-3" in the log, and in `$S/inbox/scripts/` a page and a manifest per
   session. Open `ro-planet-1-1.html`: how to record with Voice Recorder, how to say
   sounds, names, words and lines, then 0 "Unu, doi, trei, patru, cinci.", 1 "/a/",
   2 "/m/" …, 41 items. No slow word and no line with `{name}` is listed; "Atinge
   litera" says "then stop, the app adds the letter". (In the Editor: Școlița → Make
   recording scripts writes into `local/voice-inbox/scripts/` and opens it.)
3. **Import a session (a synthetic voice, never a family voice):** make a test session
   of the level test and ten lines: copy `ro-planet-1-1.json` to
   `$S/inbox-test/scripts/`, keep its first 11 items, render them with Piper
   (`local/voice-samples/piper/README.md`; `$S/make_take.py` did it here) with about
   two seconds between lines, encode the take as M4A
   (`ffmpeg -i take.wav -c:a aac $S/inbox-test/ro-planet-1-1/Recording.m4a`), then run
   `"$UNITY" -batchmode -nographics -quit -projectPath game -executeMethod Scolita.Editor.Voice.RecordingKitMenu.ImportSessionFromCommandLine -voiceInbox $S/inbox-test -voiceSession ro-planet-1-1 -logFile $S/import-session.log`.
   Expected: exit 0, "11 pieces for 11 lines: they match. 9 of 10 clips are good" (one
   flagged: Piper's soft /p/ leaves a long silence at the start of "pe"); in
   `$S/inbox-test/ro-planet-1-1/`: `clips/` with `level-test.wav` and ten
   `ro-sound-a.wav` … `ro-word-tata.wav`, `result.json` and `report.html` (each clip with
   a player, its numbers, "good" or its flags).
4. **A take with a line missed:** the same with item 6 left out of the take.
   Expected: "10 pieces for 11 lines: a line seems to be missed (most likely number 1)"
   and every line flagged (the ten lines are all short and alike, so any of them could
   be the missed one): none is silently given to the wrong line, and 0 clips are good.
5. **Without ffmpeg:** unset `SCOLITA_FFMPEG` and run step 3. Expected: exit 1 and
   "ffmpeg was not found: set SCOLITA_FFMPEG … (local/todo/04-record-voices.md, "Set up
   once")", never a crash.
6. **Accept a session (only test clips, never committed):** back up
   `content/voice-lines.csv` and `game/Assets/Scolita/Content/`, then
   `… -executeMethod Scolita.Editor.Voice.RecordingKitMenu.AcceptSessionFromCommandLine -voiceInbox $S/inbox-test -voiceSession ro-planet-1-1 …`.
   Expected: "9 clips of ro-planet-1-1 are in content/voice/ and imported";
   `content/voice/` holds nine mono 44.1 kHz Vorbis files; their rows in
   `voice-lines.csv` name them and say `recorded`; `Content/Voice/Clips/` holds their
   copies and the voice line assets link them.
7. **Check voice clips:**
   `… -executeMethod Scolita.Editor.Voice.RecordingKitMenu.CheckClipsFromCommandLine …`.
   Expected: one line per clip with its length, LUFS, peak and silence at each end. Add
   a copy delayed by a second, one 20 dB quieter and one 20 dB louder (ffmpeg
   `-af adelay=1000`, `volume=-20dB`, `volume=20dB`): they are flagged
   `LongSilenceAtStart`, `TooQuiet`, and `TooLoud, Clipped`, and the run exits 1.
8. **Clean up:** delete `content/voice/` and `Content/Voice/Clips/` (with its `.meta`),
   restore the backups, and rerun the import: `git status` shows nothing changed.

### Game slice

1. **PlayMode tests:**
   `"$UNITY" -batchmode -projectPath game -runTests -testPlatform PlayMode -testResults $S/playmode.xml -logFile $S/playmode.log`.
   Expected: every test passes, among them `HearableTests`: a tap says the Hearable's
   line, it pulses until the line ends, another line stops its pulse, and on the
   Lesson placeholder the sampler's first letter speaks (in the Editor: the bubble).
2. **Debug APK:**
   `"$UNITY" -batchmode -nographics -quit -projectPath game -executeMethod Scolita.Editor.BuildScript.BuildAndroidDebug -logFile $S/build.log`;
   `aapt dump permissions` on it lists no new permission (only
   `DYNAMIC_RECEIVER_NOT_EXPORTED_PERMISSION`).
3. **The Lesson placeholder** (emulator, 2000 × 1200 and 1280 × 800 with `wm size`):
   tap **Hai!**. Expected: the home button top left, a row "RO" (gold star) with m a t p
   mama m-a-m-a at the top, the cream card in the middle, a row "EN" (blue planet) with
   m a t p map m-a-p at the bottom, nothing overlapping.
4. **Both voices, with the test clips from step 6 of the Build slice built in**
   (`"$ADB" -s $EMU logcat -s Unity:I | grep Voice` shows each part said, in a
   development build):
   - RO **m**: `Voice: Clip ro-sound-m.ogg`; the tile pulses while it plays;
   - RO **mama**: `Voice: Clip ro-word-mama.ogg`;
   - RO **m-a-m-a**: four clips, m a m a, with a short pause between;
   - RO **m-a-m-a** then RO **m** at once: the slow word stops, m plays; never both;
   - EN **m**, **map**, **m-a-p** (no clips): with the English voice installed,
     `Voice: System en "m"` …; without it, the bubble with the EN badge and the text.
5. **A language missing:** with no installed Romanian voice (Settings → System →
   Languages → Text-to-speech → Google → Install voice data → Romanian not
   downloaded), the log says "the system voice has no installed ro-RO voice"; a
   Romanian line without a clip (RO **t** after removing the test clips) shows the
   bubble with the RO badge and the text for a reading time, and the app goes on.
   Install the voice, go Home and back to the app: the next line is spoken
   (the voices are checked again on resume).
6. **Pause:** tap RO **m-a-m-a**, press Home at once, come back: nothing goes on
   talking, and the next tap speaks.
