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
- **Asked the human** (through the lead, 2026-11-24) which converter the
  recording kit may use (see "The recording kit's formats" below). Building
  everything that doesn't depend on the answer.

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
