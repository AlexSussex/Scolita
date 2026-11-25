# The screens

The first eleven screens, as designed on the canvas
(https://claude.ai/artifact/RVfBkheGcrfB824aq4cEEf; source in
[`canvas/`](canvas/)). Each is drawn at **1280 × 800** (the canvas scaler's
reference size, landscape; the Tab A7 is 2000 × 1200, the same shape
scaled). Positions below are in those reference pixels. The colours, type
and rules are in [`README.md`](README.md).

The words on screen are in Romanian (D1: the app's own language defaults
to Romanian). **Every line in a bubble is spoken at the moment it appears**
(she can't read yet), in the same words as the bubble; the English version
of each line is in `content/voice-lines.csv` when it is written. Every
line is a complete, correct sentence (`AGENTS.md` → "Learning" →
"Correct, complete language").

## 1. The galaxy map (home) — `canvas/Main.dc.html`

**What it is for:** one tap to the next lesson; seeing where she has been
and where she is going.

- **Background:** the purple sky with star dust and two sparkles.
- **The rainbow path:** a wave across the whole screen from bottom left
  (60, 560) through the middle (640, 440) to the right edge (1300, 420),
  six bands.
- **Planet 1, finished (left, centre 230, 440, radius 84):** gold, beaming
  face, a sparkle; its letters `a m t p` on four 44 px cream tiles below it.
- **Planet 2, current (centre 640, 296, radius 110):** pink with a pale
  pink ring, awake face; its letters `s i n` on 52 px tiles below, the
  letter being learned (`s`) with a 4 px yellow ring, the others at 55%.
- **Stops on planet 2:** dots along the path under it: a done stop (white
  with a green tick), the current stop (a larger yellow dot with a white
  rim), stops to come (white at 35%).
- **The rocket:** white body, pink nose, a blue window, purple fins, an
  orange and yellow flame, tilted −12°, flying over planet 2.
- **Planet 3, ahead (right, centre 1030, 400, radius 78):** green, sleepy,
  at 55%, "z z z" rising from it; its letters `o d e` at 55%. **Planet 4**
  (purple) peeks in from the right edge at 35%.
- **The mascot** (top left, 150 px) with its bubble: "Bună! Azi învățăm
  litera s." and the Romanian star badge.
- **"Hai!"** (centre, 240 × 96, yellow, a play icon): starts the current
  lesson.
- **Top bar:** "Școlița" (40 px, bold, white) top left; the parent button
  top right ("Părinți" with a lock icon, translucent, 52 px tall): hold for
  3 seconds to open the gate.
- **Bottom bar** (104 px, a darker translucent band): three 72 px buttons:
  **Cerul meu** (the sky book, a constellation icon and the star count in a
  yellow pill), **Abțibilduri** (the sticker book), **Dâre** (the rocket's
  trails).

## 2. Meet a letter — `canvas/Meet.dc.html`

**What it is for:** the first meeting with a letter in one language: its
sound, its name, a word that starts with it.

- **Lesson top bar** (on every lesson screen): the home button (top left,
  80 px), the step dots (top centre; here step 1 of 5).
- **The letter card** (left, 560 × 560, cream, 48 px radius): `m` at
  300 px and `M` at 200 px in Andika, the Romanian star badge (64 px) top
  right; two buttons under the letter:
  - **the sound** (blue pill, a speaker icon and `/m/`);
  - **the name** (white pill with a blue outline, a speech-bubble icon and
    „me”).
- **The mascot** (right, 120 px) with "Ascultă: mmm… mmm… muzică!" and a
  replay icon.
- **The handwritten form** (to add in `handwriting`): a third button under
  the letter, **de mână** (a pencil icon), which writes the handwritten `m`
  and `M` stroke by stroke beside the print ones (drawn from the stroke
  data), so she sees from the first meeting how a hand makes the letter.
- **The picture card** (right, 500 × 400, cream): a music note picture
  (purple stems, a pink and a blue note head, a sparkle) and the word
  **muzică** in Andika 64 px with its first letter in pink; tapping it says
  the word.

## 3. Trace a letter — `canvas/Trace.dc.html`

**What it is for:** writing the letter with a finger, stroke by stroke.

- **Step dots:** step 4 of 5 (three done in green, the current in yellow).
- **The writing board** (left, 780 × 600, slate, 44 px radius): the three
  writing lines (top at 150, dashed middle at 300, baseline at 450 inside
  the board); a single-storey `a` as a dotted guide (its bowl and its
  stem); **stroke 1's start dot** (green, white rim, "1") at the bowl's top
  right, **stroke 2's dot** (outlined, "2") at the top of the stem; a
  **yellow arrow** over the bowl pointing the way (anticlockwise); her
  **rainbow trail** already covering the bowl's top, with stardust and a
  soft white glow where her finger is; the Romanian star badge top right.
- **The rounds** (right): three 72 px stars (the first gold: round 1), and
  the three rounds as rows: **pe puncte** (on dots, current), **pe umbră**
  (on a faint shadow), **fără ajutor!** (without help, from the start dot;
  not "singură!", which would be wrong for a boy).
- **The mascot** (right, 130 px) with "Începe de la punct și urmează
  săgeata."

## 4. One letter, two sounds — `canvas/TwoSounds.dc.html`

**What it is for:** hearing that the same letter sounds a little different
in each language, and sorting words by their first sound.

- **The letter** (top centre, a 170 × 150 cream card): `a` at 130 px.
- **The Romanian planet** (left, centre 245, 320, radius 110): gold, happy
  face; above it the star badge and a sound button **/a/** with a green
  tick (heard).
- **The English planet** (right, centre 1035, 320, radius 110): blue with
  a pale ring, happy face; above it a sound button **/æ/** with a green
  tick and the ringed-planet badge.
- **Trays** under each planet (280 × 150, dashed in the language's
  colour) with two slots each: the Romanian tray already holds an
  **airplane** (avion).
- **The picture in her hand** (centre, 180 × 180, tilted −6°, a yellow
  ring and a deep shadow): an **apple** (apple, English /æ/), with a dotted
  arrow towards the English planet.
- **The mascot** (bottom centre, 110 px) with "Ascultă cuvântul și du poza
  la planeta cu același sunet." (spoken).
- Sorting only starts once both sounds have been heard (the ticks).

## 5. Build a word — `canvas/BuildWord.dc.html`

**What it is for:** spelling a word she hears, letter by letter, into the
rocket's windows.

- **Step dots:** step 2 of 3 (a word lesson).
- **The picture card** (left, 300 × 280): a **bed** (pat), the Romanian
  star badge, and "ascultă" with a speaker: tapping says "pat".
- **The rocket** (centre right, lying on its side, about 800 px long):
  white body, pink nose, purple fins, a flame at the back; **three round
  windows** (120 px, grey-blue rims): `p` and `a` already in place, the
  third empty with a **yellow glowing rim** (the hint: the next letter
  goes here).
- **Letter tiles** (bottom centre, 130 px cream squares): `m`, `t`, `p`;
  `t` glows yellow (the hint after a miss).
- **The mascot** (top right, 96 px) with "Ce sunet urmează? p… a… ?"
- When the word is complete, the rocket takes off and the word is said
  slowly, then blended.

## 6. The end of a lesson — `canvas/Done.dc.html`

**What it is for:** celebrating, and showing where the stars go.

- **"Bravo!"** (top left, 120 px, yellow) and "Ai aprins 3 stele în
  constelația Unicornului." (30 px), both spoken as the stars fly in.
- **The constellation** (top right, the planet's "Unicorn"): stars joined
  by soft yellow lines, the lit ones solid, three empty places dotted; three
  big gold stars flying into the empty places along dotted trails.
- **The mascot cheering** (190 px, eyes closed in arcs, open smile, a
  sparkle) beside **the new sticker** (a guitar, on a 220 px cream sticker
  with a white border, tilted 5°).
- **Mai departe** (bottom left, yellow, 96 px, an arrow): back to the map,
  the rocket one stop on. **În cartea mea** (outlined): opens the sticker
  book with the new sticker.

## 7. The parent area — `canvas/Parent.dc.html`

**What it is for:** the parent sees what she has learned and sets things
up. For adults: smaller text, more on the screen, still calm.

- **Lilac page**, a back button and **Pentru părinți** (36 px) top left,
  **Versiunea …** top right.
- **Ce a învățat** (left card): a legend (learned: filled ink; learning:
  half-filled; new: outlined), then one row per language (the star badge
  and "Română", the ringed planet and "Engleză") with the track's letters
  as 34 px squares in their state; **the days played** in the last two
  weeks as 14 squares (green: played); **Vocile înregistrate:** how many
  lines are recorded per language, "Restul le spune vocea tabletei".
- **Setări** (right card):
  - **Limbile lecțiilor:** Amândouă / Doar română / Doar engleză (a pill
    group, the chosen one filled ink);
  - **Limită de timp pe zi:** Fără / 10 min / 15 min / 20 min, with "Lecția
    începută se termină mereu; apoi racheta se odihnește până mâine.";
  - sliders for **Vocea**, **Muzica**, **Sunetele**; switches for
    **Vibrații**, **Literele mici întâi**, **Meniurile în engleză**;
  - **Șterge progresul…** (outlined in dark red, bottom left of the card):
    asks twice.

## 8. Write by hand, joined — `canvas/Handwriting.dc.html`

**What it is for:** learning how letters are made **by hand**, not only how
they look typed (the human, 2026-11-24): the handwritten forms ("litere de
mână", as Romanian clasa I teaches them), joined into a word. The same
board serves `TraceLetter` in its handwritten form (one letter) and
`TraceWord` (a word).

- **Step dots:** the last step of a word lesson.
- **The writing board** (left, 900 × 600, slate): the three writing lines,
  a note under the baseline "Linia de jos: de aici pornești." (The bottom
  line: you start from here; spoken at her first handwritten letter); the
  word **mama** handwritten and joined:
  - the first `m`, **done**, in solid cream ink, with its **entry stroke**
    rising from the baseline and its **exit stroke** curving up towards
    the next letter;
  - the `a` being written: its dotted guide, the **start dot** (green) on
    the `m`'s exit, so the join is part of the letter; the yellow arrow
    showing the way round; her rainbow trail already over the join and the
    top of the oval, with stardust; the stem's dot ("2") waiting;
  - the rest (`ma`) as a faint dotted guide, still to come.
  - The Romanian star badge top right.
- **Right panel:** the word in print (**mama**, Andika, a speaker; "de
  tipar" under it), so she links the typed word to the one she writes; a
  pill pair **tipar / de mână** showing which form she is writing (here de
  mână); the three round stars; the mascot, marked "se aude acum" (a
  speaker: it is speaking now), with **"Bravo, ai făcut litera m! Acum
  leag-o de litera a, fără să ridici degetul."** (English: "Well done, you
  made the letter m! Now join it to the letter a without lifting your
  finger.") "Litera" is feminine, so it is "leag-o", for every letter.
- **The checkpoint voice:** she can't read yet, so the line is **spoken
  the moment she finishes the letter** (the checkpoint), while the bubble
  appears with the same words; the mascot's mouth moves while it speaks.
  She may keep writing while it speaks; the line is never cut off by her
  finger, and tapping the mascot says it again. Each letter of the word
  has its own checkpoint line (the letter just made, the one to join
  next); the last letter's line is the word's ("Bravo, ai scris cuvântul
  mama!" / "Well done, you wrote the word mama!").
- A lifted finger at a join is forgiven if it lands back near the next
  letter's start (tolerance in the learning rules); the letters already
  done stay in ink.

## 9. Who's playing? — `canvas/Profiles.dc.html`

**What it is for:** each child finds her own profile without reading (the
human, 2026-11-24). Shown at start only when there are two or more child
profiles; with one, the app opens straight on the map.

- **The mascot** (top centre, 110 px), marked "se aude acum", saying
  **"Cine se joacă? Atinge poza ta."** (Who's playing? Tap your picture.)
- **One card per child** (300 × 400, cream, 44 px radius, centred in a
  row with 48 px between): the avatar (200 px), the name (44 px, bold) and
  the stars count with a gold star. The last child who played has a yellow
  ring. Tapping a card **says the name**, then opens that child's map.
- **The parent button** top right, as on the map. The tester profile is
  never shown here.
- The names (Ana, Matei, Ioana) are examples. The avatars are the ones
  drawn on screen 11 (the human liked them, 2026-11-24); `look` may refine
  them, keeping their style.

## 10. Profiles in the parent area — `canvas/ParentProfiles.dc.html`

**What it is for:** the parent adds, edits and removes profiles, and opens
the tester profile. Behind the parent gate; for adults.

- **Profiluri** (36 px) with a back button; top right "Numele și pozele
  rămân doar pe tabletă." (The names and pictures stay on the tablet.)
- **Cine are profil** (left card): one row per profile with its avatar,
  name, kind and a summary ("Română: 5 litere învățate · Engleză: 3 · a
  jucat azi"), the current one outlined in violet, and **Schimbă** (edit);
  the **tester row** ("TEST" badge, "Profil de test · tester · nu apare la
  copii", "Toate lecțiile deschise, poți sări peste pași, informații de test")
  with **Joacă ca tester** and **Șterge progresul de test**; **Adaugă un
  copil** (yellow, a plus).
- **Schimbă profilul** (right card): the name field (up to 20 letters),
  the 14 avatars (drawn, as on screen 11; 7 per row, two even rows) as a grid (the chosen one outlined in violet; "o alege
  copilul"), **Limbile lecțiilor** (Amândouă / Doar română / Doar
  engleză), **Șterge profilul…** (outlined in dark red; asks twice) and
  **Salvează**.
- Wording never puts a name in the genitive or dative and never agrees
  with the child's gender, because those forms change with the name:
  "profilul Anei" but "profilul lui Matei" (and "lui Ana" is wrong); "aleasă"
  but "ales". So the heading is "Schimbă profilul" and the name stays in the
  field, and every line stays correct for any name (`AGENTS.md` →
  "Romanian, checked").

## 11. The 14 profile avatars — `canvas/Avatars.dc.html`

**What it is for:** the pictures a child chooses for her profile, all in one
place (the human, 2026-11-24). Each is drawn once, in
`canvas/Avatar.dc.html` (a component with a `kind` and a `size`), and used
by screens 10 and 11; the app's avatar catalog has the same 14, in the same
order, with stable ids:

| Id | Name on screen | What it is |
| --- | --- | --- |
| `kitty` | Pisicuța | A little girl with her face painted as a cat (a red-and-white crown on her forehead, a black nose and whiskers), light blonde hair in a topknot with a white bow, brown eyes, a big smile, a grey dotted pinafore over a grey jumper; drawn from photos of the human's daughter (2026-11-24), which stay in `local/` and are never copied anywhere |
| `cat` | Pisica mov | Her purple cat with black spots and a pink scarf (from SeliSnake) |
| `unicorn` | Unicornul | A white unicorn with a golden horn and a rainbow mane |
| `rocket` | Racheta | The app's rocket: white, a pink nose, a blue window, purple fins |
| `star` | Steaua | A smiling golden star |
| `moon` | Luna | A smiling crescent moon with a purple sparkle |
| `planet` | Planeta | A green planet with a purple ring |
| `whale` | Balena | A blue whale with a spout |
| `guitar` | Chitara | An orange guitar |
| `bicycle` | Bicicleta | A pink bicycle (cycling) |
| `rainbow` | Curcubeul | A rainbow between two clouds |
| `fox` | Vulpea | An orange fox |
| `owl` | Bufnița | A purple owl with big eyes |
| `dog` | Cățelul | A golden puppy with floppy brown ears, a patch over one eye, its tongue out and a purple collar with a gold tag |

- Each sits on a pale round background in its own colour (120 × 120 in the
  drawing), flat shapes with simple faces, as the planets: no outlines, the
  same ink `#2A2140` for eyes.
- On the sheet: the purple sky, "Pozele de profil" (48 px) and "14 poze:
  fiecare copil își alege una", seven cream cards per row, two rows (200 px tall, the
  avatar at 130 px, its name under it).

## 12. The sky book, as built by `rewards` (0.11.0) — `Prefabs/Rewards/RewardBooks.prefab`

**What it is for:** seeing where her stars went: a constellation per planet, lit star by star.

- **Opened by** "Cerul meu" on the map's bottom bar (its star count in the yellow pill).
- **The page:** the purple sky over the map; the back button top left (88 px, cream, a back
  arrow); the mascot and its bubble stay on top: "Acestea sunt constelațiile tale. Atinge una
  și îți spun cum se numește."
- **A card per planet** (380 × 480, translucent, in a row that scrolls sideways): the
  constellation drawn from `constellations.csv` (faint lilac lines, dotted rings, lit gold
  stars) over its planet's beaming picture, dimmed until its first star. Each lesson of the
  planet lights 3 places: the figure's stars first, then places along its lines.
- **Tapping a card** says its name: "Aceasta este constelația Unicornului."

## 13. The sticker book and scene, as built by `rewards` (0.11.0)

**What it is for:** playing with her stickers on a big space scene that keeps them.

- **Opened by** "Abțibilduri" on the map (shown once she has a sticker), or "În cartea mea"
  at a lesson's end.
- **The scene:** the Earth on the purple sky, most of the screen. **The book:** a strip along
  the bottom (150 px, translucent) with her stickers on cream tiles, scrolling sideways; a
  pink number when she has more than one copy left; faded when every copy is on the scene.
- **Drag a sticker up** out of the strip onto the scene: it stays where she lets go. **One
  finger** moves it, **two fingers** make it bigger or smaller (half to two and a half times),
  **dragging it back** onto the strip takes it off (it stays in the book). The last one moved
  is on top. Each copy she was given can be placed once.
- Said when it opens: "Trage abțibildurile pe Pământ. Cu două degete le faci mai mari sau mai
  mici."

## 14. The trail picker, as built by `rewards` (0.11.0)

- **Opened by** "Dâre" on the map (shown once a planet has given her a trail).
- **A cream card per trail she has** (300 × 200, the trail on a strip of sky), the one the
  rocket wears ringed in yellow; tapping one gives it to the rocket (it streams under the
  rocket on the map) and closes the picker. Said: "Alege dâra rachetei."

## 15. Who's playing?, as built by `profiles` (0.14.0) — `Prefabs/Profiles/ProfilesScreen.prefab`, `Scenes/Profiles.unity`

- **Shown** at start with two or more children (never with one), and from the map's avatar button.
- **The purple sky;** the mascot (130 px) top centre with its bubble to its right: „Cine se joacă? Atinge poza ta.” in
  the app's language; the parent button top right (hold 3 s, then the gate, as on the map).
- **The cards:** cream, one to three in a row (300 × 400), four to six in two rows of three (210 × 270): the avatar
  (two thirds of the card), the name (44 px, smaller for a long one; none for a profile without a name), a gold star and
  the stars count; the child who played last has a yellow ring. The tester never has a card.
- **A tap:** that card lifts, the others dim, the greeting is said (and shown) in her own map's language („Bună,
  Ana! Hai să zburăm!”, or „Bună! Hai să zburăm!” without a name), then her map opens.
- **The map's avatar button:** 64 px, the avatar on a white disc, beside "Școlița" (`AvatarPlace`); shown with two or
  more children, and for the tester.

## 16. The parent area's profiles, as built by `profiles` (0.14.0) — `Prefabs/Parent/ParentProfiles.prefab`

- **Opened by** the chosen profile's button at the top of the parent area (its avatar, its name or "Fără nume", a TEST
  badge for the tester, and "Profiluri"); the area's grid, settings and reset are that profile's.
- **The page** (lilac, over the area): the back button and **Profiluri** top left, "Numele și pozele rămân doar pe
  tabletă." top right. **Cine are profil** (left, 46 %): a row per profile (80 px: the avatar, the name, "Copil" or
  "Tester, nu apare la copii", the letters learned per language and the last day played, "Schimbă"), the chosen one
  outlined in violet; the tester's **Joacă ca tester** and **Șterge progresul de test** (or **Fă un profil de test**);
  **+ Adaugă un copil** (hidden at six children). **Schimbă profilul / Un copil nou** (right): the name field (20
  characters), the 14 avatars (7 × 2, 64 px; another child's dimmed), the lessons' languages, **Șterge profilul…**
  (a child, not the only one; asks twice) and **Salvează / Adaugă**; the tester's editor shows only the avatars.

## 17. The tester's strip, as built by `profiles` (0.14.0) — `Prefabs/Profiles/TesterStrip.prefab`

- Only for the tester's profile: a dark band (76 px) along the bottom (above the map's bottom bar), a yellow **TEST**
  badge, a line of test information (on the map: the version, "toate lecțiile se deschid"; in a lesson: the version,
  the lesson id, the step's number and kind, and the line being said with how: a recording, the tablet's voice or the
  bubble only) and, in a lesson, **Sari peste pas** (240 × 64, yellow).
