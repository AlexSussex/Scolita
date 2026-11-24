# Task: feature/look

> **Status: After `project-skeleton`, in parallel.** Decisions answered
> 2026-11-24: D5 the unicorn astronaut, named by her, with her purple cat
> as the co-pilot; build the guide as data (its poses and moods as art in
> a catalog), so more guides later are art only (`feature/guides`); D6 the
> gold star "RO" and the blue ringed planet "EN"; D7 agents draw, AI tools
> where they help, some of her drawings as stickers.

Read `AGENTS.md` first, `design/README.md`, `design/screens.md` and the design canvas
(source in `design/canvas/`), and the completion records in `tasks/done/`. SeliSnake's
`design/research/ai-art-prompts.md` and its art decisions
(`C:\Projects\SeliSnake\local\todo\03-decisions.md` → D3) show what worked
there.

**Feature:** `feature/look`
**Implements:** `design/README.md`; PRD §3.1 (the mascot), §3.6 (stickers).
**Depends on:** `project-skeleton`.

## Scope

1. **A comparison canvas** of the mascot (two or three takes on the
   unicorn astronaut, with and without the purple cat co-pilot), the
   planets, and the two badges, for the human and her to choose from
   (decisions D5, D6). Her own drawings, if the human adds any to
   `local/potential-art/`, are a source; they stay in `local/`.
2. **The chosen art,** as sprites in sprite atlases: the mascot in its four
   moods (a few frames each, or a skeletal 2D rig if the 2D Animation
   package is agreed first), three planets with their faces in three
   states, the rocket and its trails, the sky, the rainbow path, the moon
   and the Earth, the badges, the writing board, 30 stickers (animals,
   instruments, planets, space things), the three constellations, and the 14 profile avatars (decision D9, chosen
   with her; her purple cat among them).
3. **`design/README.md` updated** with what was chosen and the final
   palette; licences in `ThirdParty/` for anything not drawn by agents.

## Out of scope — do not build

- Code beyond swapping placeholder sprites for the real ones in prefabs
  (each prefab listed in Touches).
- Pictures for words (`content-letters-1`, in this style).

## Acceptance criteria

- The human (and she) chose the mascot and the badges (recorded with the
  date).
- Every sprite in an atlas; the map draws in at most 20 batches (Frame
  Debugger).
- The badges are told apart in greyscale (a screenshot in Progress).

## Tests required

- EditMode: the content validator (every sprite reference resolves).
- Emulator: the map, a lesson and the sticker book with the new art at
  2000 × 1200 and 1280 × 800.

## Touches

`Assets/Scolita/Art`, the atlases, the prefabs that show the art
(listed when claimed), `design/`.

## Screens

All that show the art.

## Voice lines

None.

## Before closing out

- `local/for-human-next-steps.md`: "choose the mascot with her" while the
  canvas is up; "name the mascot" (her choice).
- Release (the next MINOR).
