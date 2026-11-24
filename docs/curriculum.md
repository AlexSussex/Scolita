# Școlița — Curriculum (draft v1)

Nov 24, 2026 · @Alex · v1 by agent `curriculum`, waiting for the human to agree it

How she learns to read and write in Romanian and English at once: the
approach, the two alphabets, the order of the letters, the shape of a
lesson, the activities and the first words. **Draft v1**: v0 checked
against five Romanian clasa I manuals, the Romanian programme and a
clasa pregătitoare guide, two English phonics programmes and the research
on learning to read two languages. The research, with every source, is in
[`design/research/curriculum-research.md`](../design/research/curriculum-research.md)
(cited below as "research §n"). The human agrees v1 before
`content-letters-1` starts; changes to the order or the approach are the
human's call (`AGENTS.md` → "Do not, without asking"). What changed from
v0 is listed at the end.

**Assumed until the human answers:** D1 Romanian as the anchor, both
languages on; D3 the Romanian "litere de mână" for both languages; D4
British English.

## 1. The approach

- **Both languages, one child.** Children who learn two languages early
  carry phonological awareness (hearing the sounds inside words) and
  decoding from one language to the other, so practising them in one
  helps the other (research §5). Școlița builds those skills on purpose,
  in both: first sounds, syllables, blending, splitting a word into its
  sounds.
- **Romanian is the anchor** (decision D1; to confirm). Romanian is
  written almost exactly as it is said, and children learning such a
  spelling read familiar words almost perfectly by the end of their first
  year, while English learners take more than twice as long (research
  §5). So Romanian is where she cracks the code, and English, whose
  letters often have several sounds, comes beside it with more practice.
- **Shared letters first.** The first planets use letters that are in both
  alphabets and among the first taught by both school systems (a, m and
  the vowels in the Romanian manuals; s a t p i n m d in English phonics),
  with sounds close in both. Where a vowel differs (a, i, o), the
  difference is shown, not hidden.
- **Every item has one language,** shown by a colour **and** a badge of its
  own shape (design/README.md). The languages meet only in **planned
  bridges**: `TwoSounds` at the start of an English letter lesson when the
  sounds differ, and `WordTwins` (twins and false friends) at each
  planet's party, where both badges are on screen (research §5).
- **Hear, see, trace, read.** A letter is met as a sound with a picture,
  then as a shape to find, then traced, then used in words: from what she
  already knows (speaking) to what is new (the written code).
- **Sounds first, names later.** The sound (/m/) is what reading needs;
  both school systems teach it first and the letter's name later (the
  English name of e, "ee", is even the Romanian sound of i). In planets
  1–3 every letter is met and spoken of by its **sound**; the names are
  taught later, on their own (research §6.7; to agree). Sounds are said
  pure, with no extra vowel (/mmm/, not "muh"; /t/ short, not "tuh").
- **Letters made by hand, not only typed** (the human, 2026-11-24). She
  reads print letters, and she learns to write each letter the way a hand
  makes it: first its print shape, then its handwritten form ("litera de
  mână", as a Romanian clasa I teaches it), then joined handwritten words.
  Romanian school reads print and writes by hand, so Școlița does both
  from the first planet. (Romanian clasa pregătitoare does neither yet:
  Școlița runs a little ahead of it; research §2.2.)
- **Syllables, the Romanian way.** Romanian schools split words into
  syllables from the first weeks, before the letters (ma-ma, ra-che-tă);
  Școlița does it with a drum (English: clapping the beats), which also
  leads into the music domain later.
- **Little and often.** Lessons of 3 to 5 minutes, a review of learned
  letters at the start of each, and a gentle end when the parent's session
  limit is reached.

## 2. The two alphabets

Romanian has 31 letters; English has 26. Every English letter is also in
the Romanian alphabet (k, q, w, x, y only in borrowed words there).

| Group | Letters | What it means for the lessons |
| --- | --- | --- |
| **Shared, close sounds** | b d f l m n p s t v z, and k | One letter card, two badges, almost the same sound (English t, p, k breathe out a little more); taught once with both voices |
| **Shared, vowel differs** | a e i o u | Same letter, a different sound: Romanian /a/ "mama" and English /æ/ "cat"; /i/ "pin" (RO) and /ɪ/ "pin" (EN); /o/ "pom" and /ɒ/ "dot"; e is close in both (/e/). `TwoSounds` for a, i, o, u; a light one for e |
| **Shared, sound differs** | c g (soft before e and i in both, in different ways), h, j (Romanian /ʒ/, English /dʒ/), r (rolled in Romanian), w, x, y | `TwoSounds` is the main activity for each |
| **Romanian only** | ă â î ș ț | ă comes very early in Romanian school (research §4.2); â î ș ț their own planet ("the little hats and tails") |
| **Groups of letters** | Romanian: ce ci che chi ge gi ghe ghi, and the diphthongs ia ea ie oa taught with their vowel; English: sh ch th ck ng, ee oo ai oa … | After the single letters; each group is taught like a letter |

## 3. The order (v1)

Planets of three to four letters. Each letter is taught in both languages
(in "both" mode), Romanian first. The MVP is planets 1 to 3 (10 letters).
**Planets 1–3 are unchanged from v0**, now checked against both school
systems (research §4.1).

| Planet | Letters | First readable words, Romanian | First readable words, English | Notes |
| --- | --- | --- | --- | --- |
| 1 *(MVP)* | a m t p | mama, tata, pat, apa, pata, papa, pa, tam-tam (am) | map, mat, tap, pat (at, am, Pam) | English has 7 readable words here, Romanian 9 (research §4.1); "pat" is a bed in Romanian and a pat in English |
| 2 *(MVP)* | s i n | nas, masa, pian, nai, pin, sat, ananas, pas, spin, sita, mami, tati, nani (Ana) | pin, sit, man, tin, pan, ant, nap, sip (tip, sat, in, it, is, Sam); mint, pants | Music already: pian and nai; pin and sat are false friends |
| 3 *(MVP)* | o d e | pom, pod, dinte, stea, mop, dop, pepene, domino, desen, dans, nota, ponei, pisoi, panda, piton, tenis, sanie, patine, mops, nod, stop | dot, pen, mop, ten, pot, top, net, dad, pet, pod, nod (on, not); tent, nest, pond, stop, spot, mops | **stea** (star) is readable here; mop and stop are twins; pod, nod, mops are false friends |
| 4 | u r c l | lună (with ă, see the note), cal, rac, cer (soft c later) | cat, cup, run, red, lot, sun | r, c and u get `TwoSounds`. **Consider ă here** (research §4.2) |
| 5 | b f g h | balon, bufniță (listen only), floare | bat, bus, fan, fog, hat, hop | g: hard first; soft g with the Romanian groups |
| 6 | v z j k | vulpe, zebră (listen only), vioară (listen only) | van, zip, jet, kit | j gets `TwoSounds`; then the letter names (an alphabet song) |
| 7 (Romanian) | ă â î ș ț | rachetă, așa, țară | — | The "little hats and tails" planet (ă perhaps earlier, see planet 4) |
| 7 (English) | ck sh ch th ng | — | duck, ship, chip, moth, ring | The English-only planet, in parallel |
| 8 | w x y q | — (only in borrowed words) | wet, box, yes, quiz | |
| 9 (Romanian) | ce ci che chi ge gi ghe ghi | cerc, cireașă, ochi, ghete | — | |
| 9 (English) | ee oo ai oa igh … | — | moon, star (ar), rain, boat, night | The space words finally readable in English |

Words in brackets are for reading only (no picture) or names (a capital
first letter). English words after the semicolon have two consonants
together (tent, stop): English phonics teaches those a stage later, so
they come last in a planet's word lesson. Planets 4–9 are a sanity check
only; their words are chosen in `content-letters-2` and `-3`.

Words marked "listen only" may appear on screen as pictures with their
spoken word, but never as words to read until their letters are taught.

## 4. A lesson, and the activities

### 4.1 The shape of a planet

For each letter, in "both" mode:

1. **Romanian lesson:** `MeetLetter` (ro: the sound and a picture, then
   the print and the handwritten forms) → `ListenAndPick` → `LetterHunt` →
   `TraceLetter` (print, lower case) → `TraceLetter` (by hand, lower case)
   → `FirstSound`.
2. **English lesson:** `MeetLetter` (en) → `TwoSounds` (a, i, o; a short
   one for e) → `LetterHunt` → `TraceLetter` (one round by hand: the shape
   is the same) → `FirstSound`.

Then, once the planet's letters are met:

3. **Word lesson** per language: `Syllables` → `BuildWord` → `ReadAndMatch`
   → `TraceWord` (writing one of the planet's TraceWord words by hand,
   joined: §6). Plain words first; Romanian diphthongs (nai, stea) and
   English two-consonant words (tent, stop) last.
4. **Capitals:** the planet's capitals (A M T P …) to find and to trace,
   print then by hand (decision D2: lower case first; the Romanian manuals
   give each capital its own lesson right after its small letter).
5. **Planet party:** a review of all the planet's letters and words, with
   `WordTwins` (twins and false friends) when the planet has them, and the
   planet's constellation completed.

In "Romanian only" or "English only" mode, the other language's lessons
and the mixed activities are left out. Every lesson starts with up to two
review steps of learned items due for review. `Syllables` may use
listen-only words (it needs no reading): astronaut, planetă, rachetă.

### 4.2 The activities

| Kind | What happens | The hint after a miss | Learned from it |
| --- | --- | --- | --- |
| `MeetLetter` | The mascot holds up the letter; she hears its sound three times and a word with a picture ("m … m … muzică"); she taps the letter to hear it again, then taps the picture; the "de mână" button writes it by hand. The name is not taught in planets 1–3 | — (no wrong answers) | First contact |
| `ListenAndPick` | She hears a sound (or a word) and taps the right one of 2–4 letters (or pictures), shown as planets, bubbles or asteroids | The sound again, slower; the wrong choice fades | Sound → letter |
| `LetterHunt` | Stars float by; she pops each one that carries the letter (both cases), among 2 other letters | The right stars twinkle | Shape |
| `FirstSound` | A picture and its word; she taps the letter the word starts with | The word again, its first sound stretched ("mmmuzică") | Hearing the first sound |
| `TraceLetter` | The letter on the writing lines, in its **print** or its **handwritten** form; she traces each stroke from its start dot, in its direction (the handwritten form with its entry stroke and its exit stroke); three rounds: dotted guide, faint guide, start dot only | The stroke animates itself once, then she tries | Writing |
| `TraceWord` | A short word she can read, handwritten and joined; she writes it letter by letter, each letter's exit flowing into the next one's start; the dots and bars after the word; three rounds as `TraceLetter` | The next letter writes itself once, slowly | Writing words by hand |
| `Syllables` | She hears a word and taps the drum once per syllable (ma-ma: two beats); English: claps | The word said slowly with its beats | Syllables, rhythm |
| `BuildWord` | She hears a word; she drags letters into the rocket's windows, in order; the rocket takes off with the word said slowly, then blended | The next letter glows | Spelling, blending |
| `ReadAndMatch` | A word to read; she taps its picture among 2–3 | She can tap the word to hear it after the first try | Reading |
| `TwoSounds` | One letter on two little planets, each with its language badge; she taps each to hear its sound, then sorts pictures to the planet whose sound they start with | The two sounds again, one after the other | The languages' differences |
| `WordTwins` | Romanian words on one side, English on the other; she joins the twins (mop — mop, pian — piano), hearing both; false friends (pat — pat) are shown with both pictures | The pair's pictures appear | Bridging the languages |

### 4.3 Kind feedback

- Right: a happy sound, a sparkle, and a short praise line from the mascot,
  varied ("Bravo, ai reușit!", "Ești o stea!", "Well done, you did it!",
  "You're a star!"), in the lesson's language: always a whole sentence.
- First miss: a soft "boing", the choice wobbles, and the hint above.
- Second miss: the answer is shown (it glows), and she does it with the
  answer shown; it counts as practice, not as right.
- No buzzers, no red, no lost stars, no timers, no "wrong".

## 5. Learned, and reviewed

Progress is kept per profile (each child, and the tester, has their own).
Defaults for the learning rules asset (tuned with her in
`progress-review`):

| Rule | Default |
| --- | --- |
| Tries before the answer is shown | 2 |
| Right answers for "learned" | 6, across at least 2 activity kinds |
| Days for "learned" | right answers on at least 2 different days |
| Review schedule after "learned" | 1, 3, 7, 14, 30 days |
| A miss in review | back one step on the schedule |
| Review steps at a lesson's start | at most 2 |
| Session limit choices | off, 10, 15, 20 minutes (default: off, decision D8) |

English is learned more slowly than Romanian for the same letters
(research §5): `progress-review` may give English items more practice (a
higher count for "learned", or more review), per language in the rules
asset.

## 6. Words

- **Each word has** its language, its letters, its syllables
  (Romanian: as a Romanian school splits them, research §2.3), a picture,
  a recorded or system-voice clip, and its stable id.
- **Planets 1–3's words** are listed in full in research §6, each with its
  letters, syllables, whether it can be pictured, whether it is from her
  world, and its notes; with the listen-only words, the TraceWord words
  and the check that every word uses only letters taught before it. Four
  TraceWord words per planet and language:

| Planet | Romanian | English |
| --- | --- | --- |
| 1 | mama, tata, pat, apa | map, mat, tap, pat |
| 2 | nas, masa, pian, sat | pin, sit, man, tin |
| 3 | pom, pod, dinte, stea | dot, pen, mop, ten |

- **Romanian before ă:** many Romanian nouns end in ă, so planets 1–3 use
  their article forms (apa "the water", masa, pata, nota, sita), which are
  everyday words. Familiar words (mami, tati, tanti, nani, papa) are
  marked; the human decides whether to keep them. Words were checked on
  dexonline; "som" and "pisi" were dropped (not standard).
- **Chosen from her world** where possible: space (stea now; rachetă,
  planetă, lună, astronaut, cometă later), rainbows and unicorns (ponei
  now; curcubeu, unicorn later), animals (panda, piton, mops, pisoi),
  music (pian, nai, tam-tam, nota, dans; tobă, vioară, flaut, chitară
  later), drawing (desen, pata; pensulă, culori later), cycling (stop,
  patine; bicicletă, roată later).
- **Word twins** (alike in both languages), used by `WordTwins` once both
  are readable or as "listen only" pairs before:

| Romanian | English | Planet where both are readable |
| --- | --- | --- |
| mop | mop | 3 |
| stop | stop | 3 (English: two consonants together) |
| pian | piano | Romanian 2; English listen only |
| domino | domino | Romanian 3; English listen only |
| panda | panda | Romanian 3; English listen only |
| tenis | tennis | Romanian 3; English listen only |
| ponei | pony | Romanian 3; English listen only |
| rachetă | rocket | 7 |
| planetă | planet | 7 |
| astronaut | astronaut | 7 / 8 |
| unicorn | unicorn | 4 (with c) |
| robot | robot | 5 |
| telescop | telescope | 4 |
| cometă | comet | 7 |
| trompetă | trumpet | 7 |
| flaut | flute | listen only |
| vioară | violin | listen only |
| chitară | guitar | listen only |
| bicicletă | bicycle | listen only |
| delfin | dolphin | listen only |
| elefant | elephant | listen only |
| zebră | zebra | 7 |
| girafă | giraffe | listen only |
| tigru | tiger | listen only |
| muzică | music | listen only |

- **False friends** are welcome, as a joke, shown with both pictures: pat
  (a bed / a pat), sat (a village / sat down), pin (a pine tree / a pin),
  pod (a bridge / a pea pod), nod (a knot / a nod), mops (a pug / more
  than one mop). Twins help some children more than others (research §5):
  a bonus, not a pillar.

## 7. Letter shapes

Four forms of each letter, as a Romanian abecedar shows them: print lower
and upper case ("litere de tipar"), handwritten lower and upper case
("litere de mână"). The manuals teach all four of a letter together, the
small letter first, its capital in the next lesson (research §2.2).

- **To read:** a school print typeface with single-storey ɑ and ɡ, as
  children write them; the candidate is **Andika** (SIL, Open Font
  License, made for early readers), checked in `project-skeleton` for
  ă â î ș ț with the comma below.
- **To write, print (MVP):** print letters, lower case first (decision D2),
  with stroke data (start, direction, order) on the writing lines.
  Romanian schools prescribe no print stroke order (they write by hand);
  English schools start every letter at the top, round letters
  anticlockwise. **Proposed for D2:** the English top-start order, lower
  case first, capitals later in the same planet. Drawn for the 10 letters
  in [`print-stroke-order-draft.svg`](../design/research/print-stroke-order-draft.svg)
  (research §7).
- **To write, by hand (MVP; the human, 2026-11-24, decision D3):** the
  handwritten forms, as taught in Romanian clasa I, for both languages (one
  hand for both: the letters are the same, only their sounds differ;
  English schools accept any joined style). Drawn for the 10 letters, lower
  and upper case, in
  [`handwriting-model-draft.svg`](../design/research/handwriting-model-draft.svg),
  and joined in [`joined-words-draft.svg`](../design/research/joined-words-draft.svg)
  (research §8). As the manuals show it:
  - **the writing lines** of the "tip I" notebook: four lines (dashed top,
    solid middle, solid baseline, dashed bottom), slanted guide lines, the
    letters leaning right; small letters fill the middle space, t d and
    the capitals reach the top line, p the bottom line. The writing board
    gets the fourth line;
  - **one continuous line per letter**, lifting only for the dot of i and
    the bars of t and A (added after the word) and the second part of T
    and P;
  - **entry strokes** from the baseline for a d o i t p s e (and A M N
    with a small curl); **m and n start at the middle line** with a small
    hook; **exit strokes** to the right on the baseline, except o, which
    leaves at the middle line, so the next letter joins high;
  - **joins:** each letter's exit is the next letter's start, so letters
    join into words without lifting.

  Handwritten letters are drawn from that stroke data (no cursive font), so
  what she sees is exactly what she traces. Per letter: print first, then
  by hand, in the same lesson; per planet: joined words by hand
  (`TraceWord`). The draft points are in
  [`strokes-draft.json`](../design/research/strokes-draft.json).
- **Later:** handwritten capitals joined into names and sentences, and
  free writing without a guide.

## 8. What the voice says

Every line is in `content/voice-lines.csv`, per language. The draft for
planets 1–3 is
[`design/research/voice-lines-draft.csv`](../design/research/voice-lines-draft.csv)
(584 lines: 316 Romanian, 268 English; research §9):

- each letter's **sound** and **name** (names recorded now, taught later);
- each **word**, and each word said **slowly** (sound by sound, for
  `BuildWord`) and in **syllables** (for `Syllables`), the listen-only
  words too;
- **instructions** per activity ("Atinge litera {x}." / "Tap the letter
  {x}."), with the letter (its sound, in planets 1–3) or the word spliced
  in as its own clip;
- **praise** (10 per language, each a whole sentence) and **hints** (at
  least 4 per activity kind and language);
- the mascot's **greetings**, the map, the planet arrivals, the lesson's
  end, rewards, review and the session's end;
- the **checkpoints**: every moment the mascot speaks during an activity
  (a stroke done, a letter done, a word done), as in "Bravo, ai făcut
  litera m! Acum leag-o de litera a, fără să ridici degetul." Every bubble
  on screen has its line, spoken as it appears. Every letter pair the
  TraceWord words need is listed (research §6).

Every line is a complete, correct sentence, with its verb and the right
agreement ("litera" is feminine: "leag-o"), never a shortcut ("Bravo,
m!"). She learns her language from it. The lines the agent is unsure of
are in research §9.2, for the human.

## Sources

The full list, with what was read and what could not be, is in research
§1. The main ones:

- Corina Andrei, Constanța Bălan, *Comunicare în limba română, manual
  pentru clasa I*, Corint Logistic, 2023 —
  https://www.manuale.edituracorint.ro/2023/comunicare_I_l.pdf
- Rodica Gavriș-Pascu et al., *Comunicare în limba română, clasa I*, Art
  Klett, 2023 —
  https://catalog.manualedigitaleart.ro/art-romana-1-2023/v1/art-romana-1-2023.pdf
- *Programa școlară, Comunicare în limba română, clasa pregătitoare, I și
  a II-a*, OMEN 3418/2013 —
  https://www.edums.ro/invprimar/Lb%20romana%20P%20I%20II.pdf
- ISJ Dâmbovița, *Scrisoare metodică 2024–2025, clasa pregătitoare* —
  https://www.isj-db.ro/public/static/files/2024_2025/primar/Scrisoare_metodica_2024-2025_Clasa_pregatitoare_ISJ_Dambovita.pdf
- DfES, *Letters and Sounds*, 2007 —
  https://assets.publishing.service.gov.uk/media/5a7aa7b6e5274a34770e630c/Letters_and_Sounds_-_DFES-00281-2007.pdf
- Little Wandle Letters and Sounds Revised, progression (a school's copy) —
  https://elmridge.bright-futures.co.uk/wp-content/uploads/2022/12/Phonics-Curriculum-Overview-1.pdf
- Seymour, Aro & Erskine (2003), *British Journal of Psychology* 94(2) —
  https://doi.org/10.1348/000712603321661859
- Melby-Lervåg & Lervåg (2011), *Journal of Research in Reading* 34(1) —
  https://doi.org/10.1111/j.1467-9817.2010.01477.x
- Bhalloo & Molnar (2023), *Perspectives of the ASHA SIGs* 8(5) —
  https://doi.org/10.1044/2023_PERSP-23-00041

## Changes from v0

1. **Planets 1–3 kept** (a m t p · s i n · o d e), now checked against
   five Romanian manuals and two English phonics programmes (§3; research
   §4.1). English planet 1 has 7 readable words, not 8: accepted, for the
   human to agree.
2. **Sounds first, names later** (§1, §4.2): in planets 1–3 letters are
   met and spoken of by their sound; `MeetLetter` drops the name, and
   design screen 2's name button waits until names are taught. Both school
   systems do this. *Needs the human's agreement.*
3. **The words of planets 1–3** (§3, §6): full lists in both languages,
   checked letter by letter and on dexonline. "stea" (star) is readable
   from planet 3, not 7. Dropped: "som", "pisi" (not standard; v0's "pisi"
   becomes "pisoi", kitten, on planet 3). New in v1: ananas, pas, spin,
   sita, nani, desen, dans, nota, ponei, pisoi, panda, piton, tenis,
   sanie, patine, mops, nod, stop; English pan, ant, sip, tip, mint,
   pants, pet, dad, top, net, spot, mops.
4. **Word twins and false friends** (§6): mop, stop, pian/piano, domino,
   panda, tenis/tennis, ponei/pony; false friends pat, sat, pin, pod, nod,
   mops.
5. **Planned bridges** (§1, §4.1): `TwoSounds` for a, i, o (a light one
   for e) at the start of the English letter lesson; `WordTwins` at the
   planet party; backed by the bilingual research.
6. **The lesson shape** (§4.1): a capitals lesson per planet (D2: lower
   case first); plain words before diphthongs and English two-consonant
   words; `Syllables` may use listen-only words.
7. **Print stroke order** (§7): the English top-start order proposed for
   D2, drawn for the 10 letters.
8. **The handwriting model** (§7): drawn for the 10 letters, lower and
   upper case, from two Romanian manuals; four writing lines with slant
   guides (the board had three); m and n start at the middle line with a
   hook, not from the baseline (design screen 8's text to follow); o joins
   high; dots and bars after the word.
9. **The voice lines** (§8): a full draft for planets 1–3 (584 lines),
   checked against "Romanian, checked"; the lines the agent is unsure of
   are listed for the human.
10. **English needs more practice** (§5): a note for `progress-review`.
11. **Planets 4–9** (§3): ă is taught much earlier in Romanian school than
    planet 7; consider moving it to planet 4 or 5 (for
    `content-letters-2`, the human's call). Letter names after planet 6.
12. **Sources** for every claim about Romanian or English teaching, in the
    research note.
