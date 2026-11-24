# Școlița — Curriculum (draft v0)

Nov 23, 2026 · @Alex

How she learns to read and write in Romanian and English at once: the
approach, the two alphabets, the order of the letters, the shape of a
lesson, the activities and the first words. **Draft v0**, written with the
plan; `feature/curriculum-research` checks it against a Romanian abecedar
and an English phonics programme and turns it into v1, which the human
agrees before `content-letters-1` starts. Changes to the order or the
approach are the human's call (`AGENTS.md` → "Do not, without asking").

## 1. The approach

- **Both languages, one child.** Children who learn two languages early
  carry phonological awareness (hearing the sounds inside words) from one
  language to the other, so practising it in one helps the other. Școlița
  builds that skill on purpose, in both: first sounds, syllables, blending,
  splitting a word into its sounds.
- **Romanian is the anchor** (decision D1; to confirm). Romanian is
  written almost exactly as it is said: one letter, one sound, with a few
  rules (ce, ci, che, chi, ge, gi, ghe, ghi). That makes it the easiest
  place to crack the code. English, whose letters often have several
  sounds, comes beside it, starting with the letters whose sounds are
  close in both.
- **Shared letters first.** The first planets use letters that are in both
  alphabets with sounds close in both (m, t, p, s, n, d, and the vowels a,
  i, o, e with a small difference that is shown, not hidden). So the first
  words can be read in both languages, and the differences arrive one at a
  time.
- **Every item has one language,** shown by a colour **and** a badge of its
  own shape (design/README.md). The languages meet only in activities made
  for that (`TwoSounds`, `WordTwins`), where both badges are on screen.
- **Hear, see, trace, read.** A letter is met as a sound with a picture,
  then as a shape to find, then traced, then used in words: from what she
  already knows (speaking) to what is new (the written code).
- **Sounds and names apart.** The sound (/m/) is what reading needs; the
  name ("me" in Romanian, "em" in English) is for talking about letters.
  Both are taught, and every activity says which it means. Sounds are said
  pure, with no extra vowel (/mmm/, not "muh"; /t/ short, not "tuh").
- **Letters made by hand, not only typed** (the human, 2026-11-24). She
  reads print letters, and she learns to write each letter the way a hand
  makes it: first its print shape, then its handwritten form ("litera de
  mână", as a Romanian clasa I teaches it), then joined handwritten words.
  Romanian school reads print and writes by hand, so Școlița does both
  from the first planet.
- **Syllables, the Romanian way.** Romanian schools split words into
  syllables (ma-ma, ra-che-tă) from the start; Școlița does it with a drum,
  which also leads into the music domain later.
- **Little and often.** Lessons of 3 to 5 minutes, a review of learned
  letters at the start of each, and a gentle end when the parent's session
  limit is reached.

## 2. The two alphabets

Romanian has 31 letters; English has 26. Every English letter is also in
the Romanian alphabet (k, q, w, x, y only in borrowed words there).

| Group | Letters | What it means for the lessons |
| --- | --- | --- |
| **Shared, close sounds** | b d f l m n p s t v z, and k | One letter card, two badges, almost the same sound; taught once with both voices |
| **Shared, vowel differs a little** | a e i o u | Same letter, a slightly different sound (Romanian /a/ as in "mama", English /æ/ as in "cat"); a `TwoSounds` moment |
| **Shared, sound differs** | c g (soft before e and i in both, in different ways), h, j (Romanian /ʒ/, English /dʒ/), r (rolled in Romanian), w, x, y | `TwoSounds` is the main activity for each |
| **Romanian only** | ă â î ș ț | Their own planet ("the little hats and tails"): ă wears a cup, â and î a hat, ș and ț a tail |
| **Groups of letters** | Romanian: ce ci che chi ge gi ghe ghi; English: sh ch th ck ng, ee oo ai oa … | After the single letters; each group is taught like a letter |

## 3. The order (draft v0)

Planets of three to four letters. Each letter is taught in both languages
(in "both" mode), Romanian first. The MVP is planets 1 to 3 (10 letters).

| Planet | Letters | First words, Romanian | First words, English | Notes |
| --- | --- | --- | --- | --- |
| 1 *(MVP)* | a m t p | mama, tata, pat (bed), papa | at, am, map, mat, tap, pat | "pat" is a word in both, with two meanings: a first "same letters, different word" moment |
| 2 *(MVP)* | s i n | nas (nose), pisi (kitty), pin (pine), pian (piano), nai (pan flute), sat (village) | sit, pin, pan, tin, man, nap, it, is, in | Music already: pian and nai |
| 3 *(MVP)* | o d e | pom (tree), pod (bridge), dop (cork), mop, dinte (tooth), pepene (watermelon), domino | dot, pot, top, mop, pen, ten, net, tent, nest, pond | "mop" is a word twin; "pod" means a bridge in Romanian and a pea pod in English |
| 4 | u r c l | lună (moon, with ă from planet 7 as "listen only" until then), cal (horse), rac (crab), cer (sky, a soft c later) | cat, cup, run, red, lot, sun | r and c get `TwoSounds` |
| 5 | b f g h | balon, bufniță (owl, listen only), floare | bat, bus, fan, fog, hat, hop | g: hard first; soft g with the Romanian groups |
| 6 | v z j k | vulpe, zebră (listen only), vioară (listen only) | van, zip, jet, kit | j gets `TwoSounds` |
| 7 (Romanian) | ă â î ș ț | rachetă, stea (already readable), așa, țară | — | The "little hats and tails" planet; English learners see it as a Romanian-only planet |
| 7 (English) | ck sh ch th ng | — | duck, ship, chip, moth, ring | The English-only planet, in parallel |
| 8 | w x y q | — (only in borrowed words) | wet, box, yes, quiz | |
| 9 (Romanian) | ce ci che chi ge gi ghe ghi | cerc, cireașă, ochi, ghete | — | |
| 9 (English) | ee oo ai oa igh … | — | moon, star (ar), rain, boat, night | The space words finally readable in English |

Words marked "listen only" may appear on screen as pictures with their
spoken word, but never as words to read until their letters are taught.

## 4. A lesson, and the activities

### 4.1 The shape of a planet

For each letter, in "both" mode:

1. **Romanian lesson:** `MeetLetter` (ro, showing the print and the
   handwritten forms) → `ListenAndPick` → `LetterHunt` → `TraceLetter`
   (print) → `TraceLetter` (by hand) → `FirstSound`.
2. **English lesson:** `MeetLetter` (en) → `TwoSounds` (when the sounds
   differ) → `LetterHunt` → `TraceLetter` (one round: the shape is the same)
   → `FirstSound`.

Then, once the planet's letters are met:

3. **Word lesson** per language: `Syllables` → `BuildWord` → `ReadAndMatch`
   → `TraceWord` (writing one of the words by hand, joined).
4. **Planet party:** a review of all the planet's letters and words, with
   `WordTwins` when the planet has twins, and the planet's constellation
   completed.

In "Romanian only" or "English only" mode, the other language's lessons
and the mixed activities are left out. Every lesson starts with up to two
review steps of learned items due for review.

### 4.2 The activities

| Kind | What happens | The hint after a miss | Learned from it |
| --- | --- | --- | --- |
| `MeetLetter` | The mascot holds up the letter; she hears its sound three times, its name once, and a word with a picture ("m … m … m … mama"); she taps the letter to hear it again, then taps the picture | — (no wrong answers) | First contact |
| `ListenAndPick` | She hears a sound (or a word) and taps the right one of 2–4 letters (or pictures), shown as planets, bubbles or asteroids | The sound again, slower; the wrong choice fades | Sound → letter |
| `LetterHunt` | Stars float by; she pops each one that carries the letter (both cases), among 2 other letters | The right stars twinkle | Shape |
| `FirstSound` | A picture and its word; she taps the letter the word starts with | The word again, its first sound stretched ("mmmama") | Hearing the first sound |
| `TraceLetter` | The letter on writing lines, in its **print** or its **handwritten** form; she traces each stroke from its start dot, in its direction (the handwritten form with its entry stroke from the line and its exit stroke); three rounds: dotted guide, faint guide, start dot only | The stroke animates itself once, then she tries | Writing |
| `TraceWord` | A short word she can read (mama, tata, pat), handwritten and joined; she writes it letter by letter, each letter's exit flowing into the next one's entry; three rounds as `TraceLetter` | The next letter writes itself once, slowly | Writing words by hand |
| `Syllables` | She hears a word and taps the drum once per syllable (ma-ma: two beats) | The word said slowly with its beats | Syllables, rhythm |
| `BuildWord` | She hears a word; she drags letters into the rocket's windows, in order; the rocket takes off with the word said slowly, then blended | The next letter glows | Spelling, blending |
| `ReadAndMatch` | A word to read; she taps its picture among 2–3 | She can tap the word to hear it after the first try | Reading |
| `TwoSounds` | One letter on two little planets, each with its language badge; she taps each to hear its sound, then sorts pictures to the planet whose sound they start with | The two sounds again, one after the other | The languages' differences |
| `WordTwins` | Romanian words on one side, English on the other; she joins the twins (rachetă — rocket), hearing both | The pair's pictures appear | Bridging the languages |

### 4.3 Kind feedback

- Right: a happy sound, a sparkle, and a short praise line from the mascot,
  varied ("Bravo!", "Super!", "Great job!", "Wow!"), in the lesson's
  language.
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

## 6. Words

- **Each word has** its language, its letters, its syllables
  (Romanian: as a Romanian school splits them), a picture, a recorded or
  system-voice clip, and its stable id.
- **Chosen from her world** where possible: space (rachetă, planetă, lună,
  stea, astronaut, cometă), rainbows and unicorns (curcubeu, unicorn),
  animals, music (pian, nai, tobă, vioară, flaut, chitară), drawing
  (pensulă, culori), cycling (bicicletă, roată).
- **Word twins** (alike in both languages), used by `WordTwins` once both
  are readable or as "listen only" pairs before:

| Romanian | English | Planet where both are readable |
| --- | --- | --- |
| mop | mop | 3 |
| pian | piano | listen only (English "piano" needs more letters) |
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

- **False friends** are welcome, as a joke: "pat" (a bed / a pat), "pod"
  (a bridge / a pea pod), shown with both pictures.

## 7. Letter shapes

Four forms of each letter, as a Romanian abecedar shows them: print lower
and upper case ("litere de tipar"), handwritten lower and upper case
("litere de mână").

- **To read:** a school print typeface with single-storey ɑ and ɡ, as
  children write them; the candidate is **Andika** (SIL, Open Font
  License, made for early readers), checked in `project-skeleton` for
  ă â î ș ț with the comma below.
- **To write, print (MVP):** print letters, lower case first (decision D2),
  with stroke data (start, direction, order) on three writing lines.
- **To write, by hand (MVP; the human, 2026-11-24, decision D3):** the
  handwritten forms, as taught in Romanian clasa I, for both languages (one
  hand for both: the letters are the same, only their sounds differ). Each
  has stroke data with its **entry stroke** (from the baseline) and its
  **exit stroke** (to the next letter), so letters **join into words**.
  Handwritten letters are drawn from that stroke data (no cursive font), so
  what she sees is exactly what she traces. Per letter: print first, then
  by hand, in the same lesson; per planet: joined words by hand
  (`TraceWord`).
- **Later:** handwritten capitals joined into names and sentences, and
  free writing without a guide.

## 8. What the voice says

Every line is in `content/voice-lines.csv`, per language:

- each letter's **sound** and **name**;
- each **word**, and each word said **slowly** (for `BuildWord`) and in
  **syllables** (for `Syllables`);
- **instructions** per activity ("Atinge litera m" / "Tap the letter m"),
  with the letter or word spliced in as its own clip;
- **praise** (at least 8 per language) and **hints** (at least 4 per
  activity kind and language);
- the mascot's **greetings** and the planet arrivals;
- the **checkpoints**: every moment the mascot speaks during an activity
  (a stroke done, a letter done, a word done), as in "Bravo, ai făcut
  litera m! Acum leag-o de litera a, fără să ridici degetul." Every bubble
  on screen has its line, spoken as it appears.

Every line is a complete, correct sentence, with its verb and the right
agreement ("litera" is feminine: "leag-o"), never a shortcut ("Bravo,
m!"). She learns her language from it.

## Sources

- Early precursors of reading in simultaneous bilinguals (systematic
  review and meta-analysis, ASHA Perspectives, 2023): within-language and
  cross-language effects of phonological awareness and vocabulary —
  https://pubs.asha.org/doi/10.1044/2023_PERSP-23-00041
- Letters and Sounds, Phase 2 (UK): the order s a t p / i n m d / g o c k /
  ck e u r / h b f l, and the pure-sound rule —
  https://home.oxfordowl.co.uk/reading/what-is-letters-and-sounds/
- A Romanian clasa I manual's structure (pre-abecedarian, abecedarian and
  post-abecedarian periods; handwritten letters in clasa I) —
  https://catalog.manualedigitaleart.ro/art-romana-1-2023/v1/art-romana-1-2023.pdf
