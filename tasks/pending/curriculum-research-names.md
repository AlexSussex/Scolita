# Task: feature/curriculum-research-names

> **Status: In progress (curriculum-research-names), blocked: for the human to agree** (docs only; best done before planet 6's content, since
> v1 brings names in after planet 6; one curriculum research brief at a
> time edits `docs/curriculum.md`). The human agreed on 2026-11-24 that
> letters are met by their sound first and their names come later.

Read `AGENTS.md` first (above all "Learning (a child is the learner)",
"Romanian, checked"), `docs/curriculum.md` v1 (§1 "Sounds first, names
later"), and `tasks/done/curriculum-research.md` with its research note
(§6.7: why names wait).

**Feature:** `feature/curriculum-research-names`
**Implements:** `docs/curriculum.md` §1, §4.2 (`MeetLetter`), §8.
**Depends on:** `curriculum-research` (done).

## Scope

When and how the letter names come in, **in both languages equally**, so
that the names never blur the sounds or the two languages.

1. **What the schools do:** when Romanian manuals and teachers bring in
   the names ("a, be, ce, de …", the alphabet lesson of clasa I; which
   forms: "me" or "em", "ne" or "en", the names of q w x y) and when
   English phonics does ("Teaching letter names" in Letters and Sounds
   Phase 3, the alphabet song; at least one more programme); cite each.
2. **The clashes between the two languages' names and sounds** (the
   English name of e is the Romanian sound of i; the English name of i is
   no Romanian sound; a, o, u; the Romanian names of k, q, w, y) and what
   the research on bilingual children says about them; how to show a name
   is a name (not a sound) and which language it belongs to.
3. **The design:** when names start (v1: after planet 6), what activity
   teaches them (an alphabet song, an activity kind of its own, or a
   `MeetLetter` round), whether the alphabet's order is taught, how the
   name button on design screen 2 comes back, and how spliced lines ("the
   letter {x}") switch from the sound to the name; reusing an existing
   activity kind where one fits (AGENTS.md → "One activity contract").
4. **The voice lines** this needs in both languages (names, any new
   instructions, the song if any), checked against "Romanian, checked".
5. **Write it up:** `design/research/curriculum-research-names.md` (every
   teaching claim cited) and a section in the next version of
   `docs/curriculum.md`, agreed by the human.

## Out of scope — do not build

- Code, content, the song's music or recording.
- Changing what planets 1–3 do (sounds only, agreed in v1).

## Acceptance criteria

- The next version of `docs/curriculum.md` says when and how names are
  taught in both languages, with the activity, the lines and the switch
  from sounds to names; every teaching claim cited.
- Each name clash between the two languages is listed with how it is
  handled.
- The human has agreed it (recorded with its date).

## Tests required

None (docs only).

## Touches

`docs/curriculum.md`, `design/research/curriculum-research-names.md`,
`local/for-human-next-steps.md`.

## Screens

None designed here; a new screen or activity becomes a follow-up brief.

## Voice lines

The letter names of both alphabets (31 and 26) and any new lines, as a
draft list.

## Before closing out

- Add "agree when letter names come in" to
  `local/for-human-next-steps.md` when the draft is pushed.
- No release (docs only).

## Progress

_Agent `curriculum-research-names`, worktree `C:\Projects\scolita-names`
(branch `agent/curriculum-research-names`, local only). Docs only: no code,
no Unity, no emulator._

- **Pushed:**
  - `3cd62cf` docs: claim curriculum-research-names.
  - The write-up commit (this Progress section with it): the research
    note `design/research/curriculum-research-names.md`, the voice-line
    draft `design/research/voice-lines-names-draft.csv` (84 rows), and
    `docs/curriculum.md` with a **proposed** §9 "Letter names" and
    proposed lines in §1, §3, §4.2 and §8, each marked "proposed, not yet
    agreed".
- **How it was done:**
  - **Romanian:** the manuals [R1] [R2] and the programme [R6] were read
    again as PDFs for the alphabet lesson, with two plannings ([R11]
    [R12]) and the clasa pregătitoare letter [R7]. The 31 letters' DOOM 3
    entries [R10] were read on dexonline by a helper agent.
  - **English:** Letters and Sounds [E1], the national curriculum [E4],
    the DfE reading framework [E5] and Read Write Inc.'s parent guide and
    Year 1 map [E6] [E7] were read as PDFs or pages.
  - **Research:** the papers' abstracts were read on PubMed and Crossref
    ([B17]'s full text on PubMed Central). OpenAlex now asks for an
    account email, so it was not used.
  - **Checks:** the cedilla check and the id check (no draft id is
    already in `content/voice-lines.csv`) were run by script.
- **Not reached** (names research §1):
  - Letters and Sounds' *Notes of Guidance* (its rationale for names);
  - Little Wandle's own timing for names;
  - DOOM 3's introductory section on the letters (only its heading is
    served; the entries were read instead);
  - the Cambridge and Oxford dictionary pages (HTTP 403);
  - the abstract of Raynolds et al. (2013), which is behind a login.
  No study of Romanian–English letter names was found.
- **Recommendation** (names research §4):
  - **When:** names come at an **Alphabet Station** after planet 6, in both
    languages, Romanian first: six short lessons each (a few names at a
    time) and a party.
  - **Each lesson:** a `SingAlong` song, a `MeetLetter` name round (the
    name with its sound), then `ListenAndPick` with names. Each English
    lesson starts with a names bridge (`TwoSounds` with names) for the
    clashing letter.
  - **The Romanian names:** DOOM 3's forms that start with the sound (be,
    ce, me, not em or mî).
  - **Spliced lines:** a mention ("ai făcut litera {x}") switches to the
    name once she has met that letter's name; cue and sound lines keep the
    sound.
  - **One new kind,** `SingAlong`.
- **Blocked: for the human to agree** the proposal, with these questions
  (names research §7):
  1. Names at an Alphabet Station after planet 6, both languages, Romanian
     first, six short lessons each?
  2. The Romanian forms be, ce, de, fe, le, me, ne, re, se, ze …, haș, ca,
     i grec, ics, dublu ve, â din a, î din i (not em/el/en, not mî)? Or her
     teacher's?
  3. q: "kü" (DOOM 3) or "chiu"?
  4. g: "ge" or "ghe"?
  5. Spliced lines: mentions switch to the name once she has met it; asks
     and sound lines keep the sound?
  6. Songs: the British ABC song; for Romanian, a chant on the drum or a
     tune she knows?
  7. The alphabet's order heard and practised lightly, not tested?
  8. One new activity kind, `SingAlong`, the rest reused?
  9. The lines in names research §5.4.
- **When agreed:**
  - the human's answers and the date go at the top of
    `docs/curriculum.md` and here;
  - "proposed" comes off §9;
  - the brief closes: moved to `tasks/done/` with its completion record,
    and no release (docs only);
  - the follow-ups (names research §6) are added to `tasks/README.md`: a
    `letter-names` brief before planet 7, and notes for
    `progress-review`, `curriculum-research-3`, `sound-feel` and `look`.

## How to test locally

### Docs slice

Docs only: nothing to build, install or run, and no emulator.

1. **Get the latest `main`:** `git pull` in `C:\Projects\Scolita`.
2. **Read `docs/curriculum.md`:**
   - the note "Proposed, not yet agreed" at the top;
   - §9 "Letter names";
   - "Changes proposed by `curriculum-research-names`" at the end.
   Every proposed line in §1, §3, §4.2 and §8 says "proposed".
3. **Check the sources:** in `design/research/curriculum-research-names.md`,
   every teaching claim has a label ([R…], [E…], [B…]), and each label is in
   §1, which says what was read and what could not be.
4. **Check the clash table** (names research §3.2): each letter of both
   alphabets is in a row (e, i, a, u, o, the mirror pairs, b d p t v, c, g,
   j, h, k, r, z, q, w, x, y, and ă â î ș ț), each with how it is handled.
5. **Check the draft CSV** `design/research/voice-lines-names-draft.csv`:
   - 84 rows: 21 Romanian and 16 English names, 23 Romanian and 22
     English lines, and two songs;
   - no cedilla: search for U+015F and U+0163 (the cedilla s and t);
   - none of its ids is in `content/voice-lines.csv`.
6. **Cleanup:** none.
