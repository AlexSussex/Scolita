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
