# Task: feature/voice

> **Status: Ready after `content-model`.** Decision D4 (whose voices) is
> not needed to build this: recorded clips and the system voice are both
> built.

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
   language (`ro-RO` or `en-GB`, decision D4 may make it `en-US`); checks
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

## Tests required

- EditMode: `VoiceRouter`'s choice (clip, system, bubble) with
  `FakeVoice`s; the sequence splicing; the loudness checker on test clips.
- PlayMode: a `Hearable` speaks its line on a tap.
- Emulator: the system voice in Romanian and English (install the Google
  TTS Romanian voice on your own AVD only); listening check of the sample
  lines.

## Touches

`Scripts/Core/Voice`, `Scripts/Game/Voice`, `Scripts/Editor/Voice`,
`content/voice-lines.csv` (a column), the `Hearable` prefab (new).

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
