# Task: feature/tablet-feedback-1

> **Status: In progress (tablet-feedback-1).** A fix (PATCH release) from the human's first try of the
> handwriting lessons with her on the Tab A7.

Read `AGENTS.md` first (especially "Learning", "Usability", "Performance" and the voice seam), and the
completion records of `tracing`, `handwriting` and `voice` in `tasks/done/`.

**Feature:** `feature/tablet-feedback-1`
**Implements:** PRD §3.4 (tracing), §3.7 (the voice); `docs/curriculum.md` §4.2 (tracing, the letters by
hand); `AGENTS.md` → "Usability" (every bubble is spoken, kind feedback) and "Learning" (the letter shapes
she will write at school).
**Depends on:** `tracing`, `handwriting`, `voice`, `content-letters-1` (all done).

## The human's report (2026-11-26, verbatim)

1. "either the lines that she's expected to follow with her finger on the screen should be thicker and
   therefore allow a like a 5-10% mistake, or the fact that the user might do a mistake and the letter is
   almost identical 90-95% to what it should be, then it should still be accepted as correct; otherwise the
   kid(s) can get frustrated as they think they did pretty well - when it's actually not perfect. They need
   to practice it to learn it, but a 90% job is fine - for some letters it might be the first time when a kid
   sees how it's written and it can't be perfect from the first try. Even I struggle a bit to do a perfect m
   (but perhaps my fingers are too thick, compared to the area that the stroke expects)."
2. "I've noticed that the strokes of the t letters are wrong/confusing: after creating the first line from
   top to bottom, the pencil should be moved in a diagonal so that it would do the other line that a small t
   has, and not having to actually draw from the bottom right of the t to where the upper line actually
   starts to be drew."
3. "When I come back to the app and press on Hai!, the unicorn shows up as if it would talk, and I can see
   its text, but there's no sound; if i press on the unicorn - it plays the sound correctly of what it should
   say when entering the writting screen for the first time; I see that on the normal flow (where the kid
   finishes everything on the screen and moves on to the next screen), then the initial sound of the unicorn
   of the second screen worked as it should have."

## Scope

1. **Tracing forgives a 90% good letter.** Find every check a trace can fail and which one rejected a good
   m; widen the accepted band to suit a child's (and an adult's) finger, forgiving a small wobble (about
   5–10% of the stroke); draw the guide line as wide as what is accepted, so what looks inside is inside.
   Print and handwritten alike. All numbers stay in `Settings/LearningRules.asset`.
2. **The pen lifts between strokes.** The move from one stroke's end to the next stroke's start dot (the t's
   bar after its stem) is shown as a hop through the air, never as a line to trace, and the next stroke is
   accepted from its own start dot. Check every letter with more than one stroke in planets 1–3, print and
   by hand.
3. **The first line is said after coming back.** Find the real cause of the silent first line on the
   writing screen after a cold start or a return from the background, and fix it at its cause inside the
   voice seam (`IVoice` and its implementations): a line asked for before the voice is ready is said once
   it is ready; only the newest line counts. Nothing allocates in an activity's frame loop.

## Out of scope — do not build

- New shapes or a new stroke order (the human's call; doubtful ones are listed for the human).
- New voice lines, new activity kinds, new packages.
- `sound-feel`'s music, effects and prefabs.

## Acceptance criteria

- A handwritten m (and the other planet 1–3 letters) traced about 90% well, a little wobbly, or with an
  adult's thick finger, is accepted in every round; one drawn backwards, one that clearly leaves the
  letter and one far too short are still not.
- The guide she follows (dots and the faint shadow) is as wide as the accepted band.
- Between two strokes the board shows a hop through the air to the next start dot, never a line to trace.
- After a cold start, and after a return from the background, the first line of the first writing screen
  (and of any screen) is heard, not only shown.

## Tests required

- EditMode: recorded strokes "about 90% good, a little wobbly" and an adult's thick-finger m pass, the
  bad ones still fail; the voice with a system voice that is ready late (only the newest line said).
- PlayMode: the board's hop between strokes; the lesson's first line waiting for a late voice where it fits.
- Emulator: drawn strokes (good, wobbly, backwards, off the letter, too short, two fingers), the t's two
  strokes, a cold start and a return from the background, then **Hai!**.

## Touches

`Scripts/Core/Tracing`, `Scripts/Core/Voice`, `Scripts/Game/Voice`, `Scripts/UI/Tracing`,
`Prefabs/Lesson/TraceLetterView.prefab`, `Prefabs/Lesson/TraceWordView.prefab`,
`Settings/LearningRules.asset` (the tracing rows only), `Tests/EditMode/Tracing`,
`Tests/EditMode/Core/Voice`, `Tests/EditMode/Fakes`, `Tests/PlayMode` (tracing and voice tests).

## Screens

The writing board (design screens 3 and 8): its guide's width and the hop between strokes.

## Voice lines

None new.

## Progress

_Kept by `tablet-feedback-1`._

### Status

- Claimed 2026-11-26.
