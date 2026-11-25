# Manual testing

What to test by hand in each release of Școlița, step by step, on the
tablet. Anyone can follow it: no tools, no command line. Every step names the
screen, what to tap, and what you should see **and hear**.

## How to use this file

1. **Install the release you are testing** and check its version in the
   **parent area** (hold the parent button, answer the gate; the version is
   at the bottom).
2. **Run "The basics" first**, every time: from the galaxy map through one
   whole lesson and back. Then run the section of each release you haven't
   tested yet, oldest first.
3. **Fill in "Tested on"** when a row passes: the date, and the device and
   version if more than one is in use (for example `2026-12-05, Tab A7,
   0.6.0`). If a row fails, write `Failed:` and what you saw or heard
   instead, and tell the lead. A row that doesn't apply gets `Skipped:` and
   the reason.
4. **Listen as well as look:** a line in the wrong language, a cut-off
   word, a wrong sound for a letter (/m/ said as "em") or a cedilla on ș ț
   is a failure.
5. **Once per release, also let her try it** and note what was hard to
   understand or reach, and what she enjoyed: that is the most useful test
   of all.

## The basics

Filled in by `parent-area` (0.13.0): the whole loop, from the galaxy map through one lesson and back, the
parent gate and a setting changed. Run it after every install.

| # | Screen | Do this | You should see and hear | Tested on |
| --- | --- | --- | --- | --- |
| B1 | Home screen of the tablet | Tap the Școlița icon | The app opens on the galaxy map within 5 seconds | |
| B2 | The map | Tap **Hai!** | The next lesson opens within 2 seconds; the mascot says its first instruction | |
| B3 | A lesson | Play it to the end (a wrong answer once, to hear the hint) | Each step says what to do; the wrong answer gets a soft hint, never a buzzer; at the end "Bravo!", the stars and **Mai departe** | |
| B4 | The end of the lesson | Tap **Mai departe** | The map comes back with the rocket on the next stop, and the mascot greets the next lesson | |
| B5 | A lesson | Start a lesson, tap the home button (top left) | The map comes back at once; Go opens the same lesson again where it was left | |
| B6 | The map | Press and hold the lock (top right) for 3 seconds | A yellow ring fills, then "Pentru părinți" with three numbers written as words; a number is said (or shown in digits if the tablet has no voice for it) | |
| B7 | The parent gate | Tap the number said | The parent area opens: what she has learned (left) and the settings (right) | |
| B8 | The parent area | Tap **Doar engleză**, then the back arrow | The map shows only the English lessons; set **Amândouă** again afterwards | |
| B9 | Android | Press the tablet's Back on the parent area, on the gate and in a lesson | Each goes back as its on-screen back button does; on the map itself Back does nothing | |

## Releases

The newest release first. Each close-out adds its section here
(`AGENTS.md` → "Versioning and releases").

### 0.13.0 (2026-11-25): the parent area

On the tablet, with the 0.13.0 APK (`local/apks/Scolita-0.13.0-debug.apk`), installed over 0.12.0 without
clearing its data. Run "The basics" first (B6 to B9 are new). The gate's number is said by the tablet's voice (no
clips yet); with no voice for the language it shows in digits.

| # | Screen | Do this | You should see and hear | Tested on |
| --- | --- | --- | --- | --- |
| 0.13.0-1 | The map | Tap the lock (top right) quickly, several times; then hold it about a second and let go | Nothing opens; the yellow ring empties when you let go | |
| 0.13.0-2 | The parent gate | Hold the lock 3 seconds; tap a word that isn't the number said; hold again at once | The page closes quietly; for 30 seconds holding the lock opens nothing, then it asks again | |
| 0.13.0-3 | The parent gate | Listen to „Atinge numărul …” (or "Tap the number …" with the menus in English) | The number said is the one written among the three; the words are spelled right (unsprezece … nouăsprezece) | |
| 0.13.0-4 | Ce a învățat | Look at the rows after she has played | Her letters: learned (dark), being learned (half), new (outlined), in Romanian and in English; today green in the days; the next three lessons; "Vocile înregistrate" with the counts | |
| 0.13.0-5 | Setări | Tap **10 min**, go back and let her play past 10 minutes (or a lesson that ends after them) | The lesson she is in finishes; then the map sleeps: the guide asleep, „Racheta se odihnește. Ne vedem mâine!”, Hai! opens nothing. Passing the gate wakes it; set **Fără** again | |
| 0.13.0-6 | Setări | Turn **Meniurile în engleză** on, go back to the map, then turn it off | The area's words in English at once; on the map "Go!", "My sky", "Stickers", "Trails"; a lesson's end "Keep going" | |
| 0.13.0-7 | Setări | Move **Vocea** lower and let go; play a lesson | The voice is quieter (the slider never goes fully silent) | |
| 0.13.0-8 | Șterge progresul… | Tap it, then "Nu, păstrează-l"; tap it again, "Da, șterge", "Șterge tot" (only on a test install, or when you mean it) | It asks twice, saying what is lost; after the second yes "Progresul a fost șters.", every letter new, the map back at the start, the stars and stickers gone; the settings stay | |
| 0.13.0-9 | The words | Read every line of the area and the gate, in both languages | Correct Romanian and English (the human checks the new words) | |

### 0.12.0 (2026-11-25): stars, stickers and trails

On the tablet, with the 0.12.0 APK (`local/apks/Scolita-0.12.0-debug.apk`), installed over 0.11.0
without clearing its data. The lines are said by the tablet's voice (none recorded yet).

| # | Screen | Do this | You should see and hear | Tested on |
| --- | --- | --- | --- | --- |
| 0.12.0-1 | The map, after the update | Open the app | The bottom bar: **Cerul meu** with her stars (3 for each lesson she already finished); **Dâre** too if she finished planet 1 | |
| 0.12.0-2 | The end of a lesson | Finish a lesson | "Bravo!", three gold stars flying into the constellation one after another, each lighting as it lands; „Bravo! Ai aprins 3 stele în constelația …” | |
| 0.12.0-3 | The end of the next lesson | Finish one more | After the stars, the purple gift box opens and a sticker pops out; „Ai câștigat un abțibild!”; **În cartea mea** beside **Mai departe** | |
| 0.12.0-4 | The sticker book | Tap **În cartea mea** | The Earth and, below, her sticker; „Trage abțibildurile pe Pământ. …” | |
| 0.12.0-5 | The sticker book | Drag the sticker up onto the Earth, move it, pinch it with two fingers | It follows her finger, stays where she lets go, grows and shrinks (not too small, not too big) | |
| 0.12.0-6 | The sticker book | Drag it back down onto the strip | It leaves the Earth and is back in the strip | |
| 0.12.0-7 | It stays | Put a sticker on the Earth, close the app, open it, tap **Abțibilduri** | The sticker is where she left it, at its size | |
| 0.12.0-8 | The sky book | Tap **Cerul meu**, then a constellation | A card per planet, the stars she earned lit; its name is said („Aceasta este constelația Unicornului.”); the back arrow closes it | |
| 0.12.0-9 | A planet's end | Finish the last lesson of a planet | A sticker and the trail's card; „… Ai câștigat o dâră nouă pentru rachetă!”; on the map the rocket wears the trail | |
| 0.12.0-10 | The trails | Tap **Dâre**, then another trail | The one worn has a yellow ring; tapping another gives it to the rocket | |
| 0.12.0-11 | The words | Read and hear the new lines | „Ai câștigat …” or „Ai primit …”? „dâră” for the rocket's trail? The constellations' names right and natural? | |
| 0.12.0-12 | The pictures | Look at the constellations with her | Does she see a unicorn, a rocket and a whale? Which ones would she rather have (the guitar, the cat)? | |

### 0.11.0 (2026-11-25): her progress and the warm-ups

On the tablet, with the 0.11.0 APK (`local/apks/Scolita-0.11.0-debug.apk`), installed over 0.10.0
without clearing its data. A warm-up only comes once a letter is learned (6 right answers, in 2 kinds
of game, on 2 different days) and due again (the next day, then 3, then 7 days later), so rows 3 and 4
need her to play on a few different days.

| # | Screen | Do this | You should see and hear | Tested on |
| --- | --- | --- | --- | --- |
| 0.11.0-1 | The map, after the update | Open the app | The lessons she finished are still ticked and the rocket stands where it was, with no trip | |
| 0.11.0-2 | A lesson | Play a lesson to its end, close the app and open it again | The map shows it ticked as before; nothing is lost | |
| 0.11.0-3 | A Romanian lesson, a day after a letter was learned | Tap **Hai!** | The lesson starts with „Hai să ne amintim!” and a quick game on a letter she knows (hear it and tap it, or tap the letter a picture's word starts with), among letters she has met; the dots count it; then the lesson's own first game | |
| 0.11.0-4 | The warm-up | Tap a wrong letter once, then the right one | The usual soft hint, then the lesson goes on; nothing is taken away (that letter just comes back sooner) | |
| 0.11.0-5 | An English lesson with a learned English letter due | Tap **Hai!** | "Let's remember!" and the warm-up in English, with the blue English badge | |
| 0.11.0-6 | The words | Read and hear the line | „Hai să ne amintim!” / "Let's remember!": right and natural as the start of a quick review? | |

### 0.10.0 (2026-11-25): the galaxy map

On the tablet, with the 0.10.0 APK (the debug one is `local/apks/Scolita-0.10.0-debug.apk`). The lines are
the tablet's voice (or the bubble only) until they are recorded.

| # | Screen | Do this | You should see and hear | Tested on |
| --- | --- | --- | --- | --- |
| 0.10.0-1 | The map | Open the app | The purple sky, the rainbow path, the planet she is on awake (with its ring) and its letters on tiles below it (the letter being learned ringed in yellow), the rocket standing on the big yellow stop, **Hai!** at the bottom; the unicorn says „Bună! Azi învățăm litera …” with the letter's sound (or „Bună! Hai să zburăm!” before a words lesson) | |
| 0.10.0-2 | The map | Slide the map left and right with a finger | It follows the finger and glides; the unicorn and **Hai!** stay put; the next planet sleeps (its eyes closed, "z z", dimmed) | |
| 0.10.0-3 | The map | Tap **Hai!**, play the lesson to its end, tap **Mai departe** | The lesson the rocket stood on opens; back on the map its stop has a green tick and the rocket flies one stop along the path | |
| 0.10.0-4 | The map, before an English lesson | Look and listen | "Hello! Today we are learning the letter …" in English, with the blue ringed-planet badge in the bubble | |
| 0.10.0-5 | The map | Tap a sleeping planet, then a faint stop ahead of the rocket | Each wobbles; „Planeta aceasta încă doarme. Ajungem și acolo curând.” and „Aici ajungem mai târziu. Hai să mergem întâi la rachetă.”; nothing opens | |
| 0.10.0-6 | The map | Tap a stop with a green tick | That lesson opens from its start | |
| 0.10.0-7 | Planet 1's last lesson (the party) | Finish it and tap **Mai departe** | The trip: the rocket flies past the moon and the Earth to the pink planet (about 4 seconds); then planet 1 beams (happy face, sparkle), planet 2 is awake, and „Am aterizat pe o planetă nouă! Bună! Azi învățăm litera s.” | |
| 0.10.0-8 | The trip | Tap anywhere during it | It stops at once and the rocket has landed on planet 2 | |
| 0.10.0-9 | The map | Tap the unicorn | It says its line again | |
| 0.10.0-10 | The map, after updating from 0.9.0 | Open the app (don't clear its data) | The lessons she played before are ticked and the rocket stands on her next one | |
| 0.10.0-11 | The words | Read the new lines | „Aici ajungem mai târziu. Hai să mergem întâi la rachetă.” and „Bravo! Ai fost pe toate planetele. Hai să le vizităm din nou!” (when every planet is done): right and natural? | |

### 0.9.0 (2026-11-25): the look

On the tablet, with the 0.9.0 APK (the debug one is `local/apks/Scolita-0.9.0-debug.apk`). The
art you chose: the sky, the word pictures, the lesson's planets and the icon show now. The
rocket, the planets on the map, the profile pictures and the stickers wait for their screens.

| # | Screen | Do this | You should see and hear | Tested on |
| --- | --- | --- | --- | --- |
| 0.9.0-1 | The launcher | Look for Școlița among the apps | The unicorn astronaut (ChatGPT's) on purple, inside the launcher's circle, its horn not cut off | |
| 0.9.0-2 | The map | Open the app | ChatGPT's starry purple sky; the round unicorn (the stand-in) top left; the big yellow **Hai!** with its darker lip | |
| 0.9.0-3 | A lesson | Tap **Hai!** | The same sky behind the cards; the home button is a cream square with a house | |
| 0.9.0-4 | `ro-m`'s last step (the first sound of mama), then `en-m` (meet m) | Look at the pictures | mama: a smiling mum; moon: a smiling crescent moon; not plain circles | |
| 0.9.0-5 | The two sounds of a (`en-a`, its second step) | Look at the two planets | A gold planet with a ring for Romanian, a blue one with gold stars for English, both awake with faces, each standing above its tray | |
| 0.9.0-6 | Any word lesson of planets 1–3 | Look at the pictures | Each picture matches its word (a bed for pat, a pug for mops, a pony for ponei). „nas” (a boy's head) and „dot” (a pink blob) are to be redrawn: do they confuse her? | |
| 0.9.0-7 | Any lesson | Look at the guide | The round unicorn stands in. The chosen unicorn with a full body comes after you make it (step 8 in your next steps) | |

### 0.8.1 (2026-11-25): planets 2 and 3

On the tablet, with the 0.8.1 APK (the debug one is `local/apks/Scolita-0.8.1-debug.apk`). Planets 2–3 come after
planet 1's party: tap **Hai!** after each lesson (there is no map yet). The pictures are still plain circles; the
lines are the tablet's voice (or the bubble only) until they are recorded. Every word and line is listed for ticking in
`tasks/done/content-letters-1.md` → Progress → "For the human to check".

| # | Screen | Do this | You should see and hear | Tested on |
| --- | --- | --- | --- | --- |
| 0.8.1-1 | Meeting s (`ro-s`, first lesson after `party-1`) | Listen; tap the letter, then the picture | „Hai să cunoaștem o literă nouă! Ascultă: s… s… soare!”, the s, S, the s by hand, the picture with „soare”; the sound is a pure /sss/, never „se” | |
| 0.8.1-2 | The s in print and by hand (`ro-s`) | Trace the print s from its top right dot; then the s by hand from the baseline | Print: round first to the left, down and round to the bottom left. By hand: the entry up from the baseline to the s's top, down the right side, round the bottom and out to the right. Does it match her manual? | |
| 0.8.1-3 | `ro-i`, `ro-n` by hand | Write the i (its dot last), the n (from its hook at the middle line, like m) | i: „Foarte bine! Acum mergi la punctul cu numărul doi.” after the line, then the dot. n: no "bottom line" sentence | |
| 0.8.1-4 | `en-i` and `en-o`: two sounds | Tap the RO and the EN planet; sort the pictures | i: /i/ (inel, insulă) and /ɪ/ (insect, igloo); o: /o/ (ochi, ochelari) and /ɒ/ (octopus, orange) | |
| 0.8.1-5 | `ro-words-2` | Drum ananas and sita, build nas and pas, read masa / pin / sat and nani / nai / pian, write nas and masa by hand | ananas three beats, sita two; nas joined n–a–s, masa m–a–s–a: „Bravo, ai făcut litera n! Acum leag-o de litera a, fără să ridici degetul.” | |
| 0.8.1-6 | `party-2` | Play to the end | Pick s, i, n in both languages; read; write pian and man; twins: pin / pin (false friends, both pictures), pian / piano, rachetă / rocket | |
| 0.8.1-7 | The o and d by hand (`ro-o`, `ro-d`) | Write the o, then the d | o: the entry up its left side, round, out high at the middle line. d: like the a, then on up to the top line, the stem down. Do they match her manual? | |
| 0.8.1-8 | `ro-words-3` | Write pom, then dinte | pom: the o's high exit joins the m; dinte: after the word „Acum pune punctul pe i.”, then „Acum trage liniuța literei t.” | |
| 0.8.1-9 | `en-words-3` | Write dot and pen | "Well done, you made the letter o! Now join it to the letter t without lifting your finger." | |
| 0.8.1-10 | `capitals-2`, `capitals-3` | Hunt S I N, O D E; trace each capital in print and by hand | The capitals by hand: S (a leaning S with a curl), I (like a J?), N, O, D, E: do they match her manual? | |
| 0.8.1-11 | `party-3` | Play to the end | Twins mop, stop, panda, ponei / pony, tenis / tennis, domino; false friends pod (bridge / pea pod), nod (knot / nodding) | |
| 0.8.1-12 | Any planet 2–3 lesson | Listen to every new word and sound | Each word said as written, in the right language; ș ț with the comma below on screen | |

### 0.8.0 (2026-11-25): writing by hand

On the tablet, with the 0.8.0 APK (the debug one is `local/apks/Scolita-0.8.0-debug.apk`; or
README → "Build a release APK" from tag `v0.8.0`, then "Install on the tablet"). Play from the
map with **Hai!**: the letter by hand comes right after the print one in a Romanian letter
lesson (`ro-a` first); the words by hand end `ro-words-1` and `en-words-1`. Write with one
finger. If the tablet's voices aren't installed, the lines show in the bubble only.

| # | Screen | Do this | You should see and hear | Tested on |
| --- | --- | --- | --- | --- |
| 0.8.0-1 | Meeting a letter (`ro-a`, first step) | Look between the letter card and the picture; tap the small card | A small cream card with the a by hand (and the RO star). The tap: the a writes itself again, „Uite cum se scrie litera de mână.” | |
| 0.8.0-2 | The letter by hand (`ro-a`, after the print a) | Look, then tap the unicorn | The board with four lines (dashed top, solid middle, solid baseline, dashed bottom) and faint slanted lines; a dotted, leaning a; dot "1" on the baseline at the left, the yellow arrow up. „Scrie litera de mână. Linia de jos: de aici pornești. Începe de la punct și urmează săgeata.” | |
| 0.8.0-3 | The a by hand, round 1 | Write the whole a without lifting: up from dot 1, over the top and back round, then down the stem and out to the right | The rainbow trail stays; „Bravo, ai scris litera a de mână! Acum scrie pe umbră.”; the second star lights. (Lifting after the up stroke or after the round is fine too: dots 2 and 3 show where to go on) | |
| 0.8.0-4 | Round 2 | Start at the top of the round instead of on the line | The trail fades, dot 1 pulses: „Pornește de pe linie.” A second miss: „Uită-te cum se scrie, apoi încearcă tu.” and the stroke draws itself | |
| 0.8.0-5 | `ro-m` by hand; `en-a` by hand | Write m (from its hook at the middle line); in English, write the a | m: no "bottom line" sentence (it starts at the middle line). English: one round only (one star), "Well done, you wrote the letter a by hand!" | |
| 0.8.0-6 | A word by hand (`ro-words-1`, mama) | Listen; write the m alone and lift | The word "mama" in print on the right; „Scrie cuvântul de mână, fără să ridici degetul.” and „m-a-m-a”. After the m, at once: „Bravo, ai făcut litera m! Acum leag-o de litera a, fără să ridici degetul.” (the same words in the bubble); she can write on while it speaks | |
| 0.8.0-7 | mama, going on | Put the finger down a little beside the join, write the a; then write "ma" in one line | The lifted join is accepted; after the a „… ai făcut litera a! Acum leag-o de litera m …”; the word done: „Bravo, ai scris cuvântul mama! Acum scrie pe umbră.”; three rounds in all | |
| 0.8.0-8 | tata | Write the whole word in one line, then the two bars | After the word: „Acum trage liniuța literei t.” and the first bar's dot; after each bar the same line, then the word's line | |
| 0.8.0-9 | `en-words-1` (map, mat) | Write map and mat | "Well done, you made the letter m! Now join it to the letter a without lifting your finger." and "Well done, you wrote the word map!" | |
| 0.8.0-10 | All of the above, with her | Let her write mama by hand | **To tell the agent:** (1) do the joins feel natural under her finger (the human's check from the brief)? (2) do the handwritten a m t p and A M T P match her manual, above all the a's three strokes and the t's tall entry? (3) are the word lines right with the letters' sounds („litera mmm”), or should they say the names? (4) is „Linia de jos: de aici pornești.” right, and every line of rows 1–9 correct Romanian? | |

### 0.7.0 (2026-11-25): tracing letters

On the tablet, with the 0.7.0 APK (the debug one is `local/apks/Scolita-0.7.0-debug.apk`; or
README → "Build a release APK" from tag `v0.7.0`, then "Install on the tablet"). Play from the
map with **Hai!**: the board comes right after meeting a letter in a Romanian lesson (`ro-a`
first). Trace with one finger. If the tablet's voices aren't installed, the lines show in the
bubble only.

| # | Screen | Do this | You should see and hear | Tested on |
| --- | --- | --- | --- | --- |
| 0.7.0-1 | The writing board (`ro-a`, after meeting a) | Look, then tap the unicorn | A dark board with four lines (a thin top line, a dashed middle line, a thicker baseline, a faint dashed line below), a dotted a, a green dot "1" at the round's top right with a smaller "2" beside it, a yellow arrow; three stars on the right (the first gold) beside a dotted a, a faint a and a green dot; the RO star on the board. „Scrie litera de tipar. Începe de la punct și urmează săgeata.” | |
| 0.7.0-2 | The board, round 1 | Trace the round from dot 1, the way the arrow points, back to the top; then the stem from dot 2 down | A rainbow trail with sparkles under the finger; the round stays in rainbow colours, „Foarte bine! Acum mergi la punctul cu numărul doi.”; after the stem the a bounces, „Bravo, ai scris litera a! Acum scrie pe umbră.”, the second star lights | |
| 0.7.0-3 | Round 2 (a faint a) | Trace the round **backwards** (from its end, clockwise); then wander far off the letter | Each time the trail fades and dot 1 pulses: „Urmează săgeata.”; the second time „Uită-te cum se scrie, apoi încearcă tu.” and the round draws itself in cream with its dots shown | |
| 0.7.0-4 | Round 2, then round 3 (the dot only) | Trace the round and the stem; in round 3 tap dot 1 once; then start a stroke in the middle of where the round would be | Round 3: only the green dot, no guide, no arrow, all three stars gold. The tap: „Urmează săgeata.” and the arrow shows; the middle: „Uită-te cum se scrie …” (the second miss) | |
| 0.7.0-5 | Round 3 | Put a second finger on the board while tracing the round with the first; rest the side of the hand on the board, then trace the stem | Only the first finger draws; the palm draws nothing. The last stroke: „Bravo, ai scris litera a!”, a praise, and the lesson moves on | |
| 0.7.0-6 | `ro-m`, `ro-t`, `ro-p`, `capitals-1` | Trace m (one stroke: down, up and over twice), t (the stem, then the bar), p (down below the baseline, up and round), and the capitals A M T P | Each letter as above. **To tell the agent:** (1) is any stroke she drew well refused, or a messy one accepted (round 1 is the most forgiving, round 3 the strictest)? (2) do the print letters match how her school writes them (where each stroke starts, its order and direction)? (3) after the answer is shown, should any stroke from the dot count? (4) should a quick tap or brush be ignored instead of counting as a try? | |

### 0.6.0 (2026-11-24): the drum, build a word, read and match, word twins

On the tablet, with the 0.6.0 APK (README → "Build a release APK" from tag `v0.6.0`,
then "Install on the tablet"). Play from the map with **Hai!**; the word lessons come after
the four letters of planet 1 in both languages (`ro-words-1` after `en-p`), and the party
(`party-1`) after the capitals. If the tablet's voices aren't installed, the lines show in the
bubble only. The drum and the rocket make no sound yet (`sound-feel`).

| # | Screen | Do this | You should see and hear | Tested on |
| --- | --- | --- | --- | --- |
| 0.6.0-1 | The drum (`ro-words-1`, first) | Listen, then tap the green tick before the drum | A picture with "mama" under it, a pink drum, a green tick; "Ascultă cuvântul și bate toba pentru fiecare silabă. mama. Când ai terminat, atinge bifa verde."; the tick alone: "Bate toba o dată pentru fiecare silabă." | |
| 0.6.0-2 | The drum | Tap the drum once and the tick; then three times and the tick; then twice and the tick | Each tap bounces the drum and lights a yellow dot. One beat: the dots clear, "Ascultă cum spun cuvântul pe silabe. ma-ma." Three: two glowing white circles, "ma-ma. Bate toba o dată pentru fiecare silabă." Two: a praise, then the same with apa | |
| 0.6.0-3 | Build a word (after the drum) | Drag the m to the rocket, then an a; then t, a, t, a | The picture (no word written), the rocket with four round windows, the tiles t a t a m. m floats back: "Ascultă cuvântul încet: t-a-t-a." (sound by sound); a: the t tile glows, "Litera care urmează strălucește." Each right tile fills the next window and says its sound; the last: the rocket flies off, "Racheta pornește! t-a-t-a. tata", a praise | |
| 0.6.0-4 | Read and match (after build a word) | Tap the word card first; then a wrong picture; then the word card again; then the right picture | "mama" on a card and three pictures without words; "Citește cuvântul și atinge poza potrivită." The card first: the instruction again, **never the word**. A wrong picture fades, "Citește cuvântul sunet cu sunet." and its sounds, the letters lighting pink one by one; the card now says "mama"; the right picture: a praise | |
| 0.6.0-5 | Word twins (`party-1`, last) | Tap the Romanian "pat", then the English planet; draw a line from "pat" to the English planet twice; tap the English "pat"; draw a line between the planets | Romanian left (stars), English right (ringed planets); "Unește cuvintele care seamănă: unul în română, unul în engleză." A tap says the word (a yellow ring); tapping another word only says it. A wrong line: both "pat" pictures appear, "Caută cuvântul care se scrie la fel."; again: the English "pat" glows, "Uite, acesta este cuvântul potrivit. Unește cele două cuvinte!". Joined: a green line and "Se scriu la fel, dar înseamnă altceva!"; the planets: "Sunt gemeni!" | |
| 0.6.0-6 | The new lines and choices | Listen to rows 1–5 in both languages (`en-words-1` is the English word lesson) | Each a whole, correct sentence. **To answer** (tell the agent): (1) „Când ai terminat, atinge bifa verde.” / "When you have finished, tap the green tick." — does she know „bifă”, and is a tick to finish right for her (without it she couldn't drum too few or too many)? (2) the English drum lines (they said "clap"; the screen has a drum in both languages): "Listen to the word and tap the drum once for each beat.", "Tap the drum once for each beat." (3) „Caută cuvântul care se scrie la fel.” and „Uite, acesta este cuvântul potrivit. Unește cele două cuvinte!” (4) a tap on a word that isn't the partner only says it, never a miss: agreed? | |

### 0.5.0 (2026-11-24): the letter hunt, first sounds and two sounds

On the tablet, with the 0.5.0 APK (README → "Build a release APK" from tag `v0.5.0`,
then "Install on the tablet"). Play from the map with **Hai!**; each lesson opens where
the last one stopped. If the tablet's voices aren't installed, the lines show in the
bubble only.

| # | Screen | Do this | You should see and hear | Tested on |
| --- | --- | --- | --- | --- |
| 0.5.0-1 | The first lessons | Play `ro-a` and `en-a` (the first two lessons) | `ro-a`: meeting a only (the tracing comes later). `en-a`: after meeting a, the game below (row 0.5.0-5). No star hunt and no first-sound game in these two | |
| 0.5.0-2 | The star hunt (`ro-m`, after "hear it, find it") | Watch the stars | Ten yellow stars with m, M, a, A and a gold "RO" star each, drifting slowly in two rows, never over the home button, the dots or the unicorn; "Atinge toate stelele care au litera m. Caută și litera mare!" | |
| 0.5.0-3 | The star hunt | Tap a star with a, then another one with a | First: it bounces back, the m stars twinkle, "Stelele cu litera m strălucesc. Prinde-le!". Second: every m star glows yellow, "Uite, aceasta este litera m. Atinge-o!". No red, no buzzer | |
| 0.5.0-4 | The star hunt | Tap each star with m or M | Each pops and says /m/ (the sound, not "me"); after the last, a praise and the next game | |
| 0.5.0-5 | One letter, two sounds (`en-a`) | Tap the gold planet, then the blue ringed one | Each gets a green tick and says its sound: Romanian /a/, then English /æ/; "The same letter has two sounds! …", then "Listen to the word and take the picture to the planet with the same sound." and the first word; a picture appears in the middle (no badge on it) | |
| 0.5.0-6 | One letter, two sounds | Drag the picture to the wrong planet twice, then to the right one; sort the rest | It floats back, "Listen to the two sounds again." and both sounds; then the right planet glows, "Look, this is the right planet. Take the picture there!"; dropped there, it sits in the planet's tray with its badge and the next word is said. avion and albină go to the gold planet, apple and astronaut to the blue one | |
| 0.5.0-7 | The first sound (`ro-m`, its last game) | Tap the picture, then **a** twice, then **m** | The picture: "mama" again. First **a**: it fades, "Ascultă primul sunet: mmmama." (until recorded, the tablet says /m/, a pause, "mama"). Second: **m** glows. **m**: a praise and the end of the lesson | |
| 0.5.0-8 | The new lines | Listen to the lines in rows 2–7 in both languages (`en-m` has the English hunt and first sound) | Each is a whole, correct sentence in the right language. The ones to check: "Uite, aceasta este planeta potrivită. Du poza acolo!" (the Romanian of row 6, said in a Romanian-led lesson later) and the stretched words "mmmama", "t-t-tata", "p-p-pat", "mmmoon", "t-t-tiger" | |

### 0.4.0 (2026-11-24): the first lessons

On the tablet, with the 0.4.0 APK (README → "Build a release APK" from tag `v0.4.0`,
then "Install on the tablet"). If the tablet's Romanian and English voices are
installed (0.3.0-1), every line is said; without them it shows in the bubble only.

| # | Screen | Do this | You should see and hear | Tested on |
| --- | --- | --- | --- | --- |
| 0.4.0-1 | The map | Tap **Hai!** | The first lesson: a big card with **a A** and a gold star "RO", a card with a coloured circle and **avion** (its a in pink), one dot at the top, the unicorn top right saying "Hai să cunoaștem o literă nouă! Ascultă: a… a… avion! Atinge litera ca s-o auzi din nou.", the same words in its bubble | |
| 0.4.0-2 | The lesson | Tap the **a** card, then the picture | The letter's sound, "Acum atinge poza."; then "avion", a praise (for example "Bravo, ai reușit!"), the dot turns green, the unicorn cheers: "Ai terminat lecția. Bravo!" and a big yellow **Mai departe** | |
| 0.4.0-3 | The end of the lesson | Tap **Mai departe**, then **Hai!** | The map, then the English lesson: blue ringed planets "EN" on the cards and the bubble, "Let's meet a new letter! Listen: a… a… apple! Tap the letter to hear it again." | |
| 0.4.0-4 | The lessons | Play on (**Hai!** after each) to the third lesson (`ro-m`) | After meeting **m**: two cards, **m** and **a**, "Ascultă și atinge litera potrivită. Atinge litera m." | |
| 0.4.0-5 | Hear it, find it | Tap the wrong letter, then the wrong one again | First: the card wobbles and fades, the unicorn thinks, "Ascultă încă o dată." and the sound. Second: the right card glows yellow, "Uite, aceasta este litera m. Atinge-o!" No red, no buzzer | |
| 0.4.0-6 | Hear it, find it | Tap **Ascultă**, then the unicorn, then the right card | "Atinge litera m." again; the last line again; then a sparkle and a praise | |
| 0.4.0-7 | Any lesson step | Tap the home button (top left), then **Hai!** | The map at once (no question); then the same lesson at the same step, the steps before it green | |
| 0.4.0-8 | Any lesson step | While the unicorn speaks, press the tablet's Home button; come back | The voice stops at once; back in the app, the sentence starts again from its beginning | |
| 0.4.0-9 | The lines | Listen to every line in rows 1–6 in both languages | Each is a whole, correct sentence, in the right language, not cut off; tell the agent any word said wrong. The one to check: "Uite răspunsul. Acum fă-o și tu." (not used yet, for picture questions) | |

### 0.3.1 (2026-11-24): the ledger of released ids

Nothing changes on the tablet. These rows are in Unity, in your own project
(`C:\Projects\Scolita\game`), after `git pull`.

| # | Screen | Do this | You should see and hear | Tested on |
| --- | --- | --- | --- | --- |
| 0.3.1-1 | Unity, your project | Școlița → Import content | A dialog: "imported, with 17 warnings" (the pictures still to draw) | |
| 0.3.1-2 | Unity, the Project window | Open `Assets/Scolita/Content/Pictures/` and click `mother.png`, then `moon.png` | Each is a plain coloured circle, with no letter on it | |
| 0.3.1-3 | Unity, your project | Școlița → Record released ids | A dialog: "every id is in the ledger already; nothing was added." `git status` shows no change | |
| 0.3.1-4 | A text editor | Open `content/released-ids.txt` | A few comment lines, then `# 0.2.0` and one line per id (`letters.csv a`, `words.csv ro-mama`, …) | |

### 0.3.0 (2026-11-24): the voice

The app speaks. On the tablet, the lesson placeholder has a row of
Romanian tiles and a row of English ones to tap. On the PC, the recording
kit is in Unity's Școlița menu. Listen for the right language and for
words that are said correctly; tell the agent about any word the tablet's
voice says wrong.

| # | Screen | Do this | You should see and hear | Tested on |
| --- | --- | --- | --- | --- |
| 0.3.0-1 | The tablet's Settings | General management → Text-to-speech → Google (the gear) → Install voice data: install **Romanian (Romania)** and **English (United Kingdom)** | Both show as downloaded | |
| 0.3.0-2 | The lesson (tap **Hai!**) | Look at the screen | The home button top left; a row with a gold star "RO" and m, a, t, p, mama, m-a-m-a at the top; the cream card in the middle; a row with a blue planet "EN" and m, a, t, p, map, m-a-p at the bottom; nothing overlaps | |
| 0.3.0-3 | The lesson, RO row | Tap m, a, t, p, then mama | Each is said in Romanian by the tablet's voice (a letter alone may sound like its name, not its pure sound, until the sounds are recorded); the tile pulses while it speaks | |
| 0.3.0-4 | The lesson, RO row | Tap m-a-m-a | The four letters said one after another with a short pause (m, a, m, a), not the word | |
| 0.3.0-5 | The lesson, EN row | Tap m, a, t, p, map, m-a-p | Each in British English; m-a-p as three letters with pauses | |
| 0.3.0-6 | The lesson | Tap m-a-m-a, and at once tap p | The slow word stops and p is said: never two at once | |
| 0.3.0-7 | The lesson | Tap mama, then press Home at once, then come back | Nothing goes on talking in the background; the next tap speaks | |
| 0.3.0-8 | The tablet's Settings, then the lesson | Remove the English (United Kingdom) voice data, come back to the app, tap EN map | A bubble in the middle with the blue "EN" planet and "map", for about two seconds, and no voice; the app goes on. Install the voice again afterwards | |
| 0.3.0-9 | Unity, your project | Set `SCOLITA_FFMPEG` once (`local/todo/04-record-voices.md`, "Set up once"), restart Unity Hub, then Școlița → Make recording scripts | A dialog: "8 sessions, 281 lines to record …"; the folder `local\voice-inbox\scripts\` opens; `ro-planet-1-1.html` is a printable page starting with 0 "Unu, doi, trei, patru, cinci." then the sounds /a/ /m/ /t/ /p/ | |
| 0.3.0-10 | `content/voice-lines.csv` | Read the new `status` column | Empty for every line (nothing recorded yet), `sounds` for the eight slow words | |

### 0.2.0 (2026-11-24): the content model

Nothing new on the tablet: the lessons are now spreadsheets the app will
read from `lesson-engine` on. These rows are checked on the PC, in the
repository's `content/` folder, with a spreadsheet program. They are the
human's check of planet 1's words, which a child will later read and
hear.

| # | Screen | Do this | You should see and hear | Tested on |
| --- | --- | --- | --- | --- |
| 0.2.0-1 | The tablet, with 0.2.0 installed | Run "The basics" and the 0.1.0 rows | Everything as in 0.1.0; nothing new on screen, the version **0.2.0** bottom right | |
| 0.2.0-2 | `content/words.csv` in Excel (Data → From Text/CSV, UTF-8) | Read the 18 words, their letters and syllables | Romanian: mama (ma-ma), tata (ta-ta), pat (pat), apa (a-pa), avion (a-vi-on), albină (al-bi-nă), muzică (mu-zi-că), tobă (to-bă), planetă (pla-ne-tă); English: map, mat, tap, pat, apple (ap-ple), astronaut (as-tro-naut), moon, tiger (ti-ger), planet (plan-et). The splits are as her school would make them; ă shows as ă | |
| 0.2.0-3 | `content/pictures.csv` and the pictures in `game/Assets/Scolita/Content/Pictures/` | Open a few of the PNGs | Coloured circles with the word's first letter (placeholders until the real pictures come); the words they stand for are right (pat in Romanian is a bed, in English patting a dog) | |
| 0.2.0-4 | `content/voice-lines.csv` | Read the 16 letter rows at the top | Romanian sounds /a/ /m/ /t/ /p/ and names a, me, te, pe; English sounds /æ/ /m/ /t/ /p/ and names ay, em, tee, pee | |
| 0.2.0-5 | `content/voice-lines.csv` | Read the `line` rows, Romanian and English | The lines you agreed in the curriculum draft, word for word, with ș and ț (comma below) and „…” quotation marks | |
| 0.2.0-6 | Unity, the project open | Școlița → Import content | A dialog: "imported, with 17 warnings"; the Console lists the 17 pictures that are still placeholders, and nothing changes in the folder | |

### 0.1.0 (2026-11-24): the foundation

The first APK: placeholder screens only. Install it on the Tab A7
(`Scolita-0.1.0.apk`, built from the tag with the release key).

| # | Screen | Do this | You should see and hear | Tested on |
| --- | --- | --- | --- | --- |
| 0.1.0-1 | Home screen of the tablet | Look for the new app icon | It is labelled **Școlița**, with a comma under the Ș and under the ț (not a little hook) | |
| 0.1.0-2 | Home screen of the tablet | Tap the Școlița icon | Within 5 seconds: a purple screen with "Școlița" at the top and a big yellow **Hai!** button; no Unity logo; the app doesn't close by itself (this is also the check that the tablet's graphics, Vulkan, work) | |
| 0.1.0-3 | The map (purple, "Hai!") | Look at the bottom-right corner | Small pale text **0.1.0** | |
| 0.1.0-4 | The map | Tap **Hai!** | A lesson screen: a cream card with **ă â î ș ț** and **Ă Â Î Ș Ț**, a's drawn the way she writes them (one round part, no hook on top), commas under ș ț Ș Ț; a cream **Acasă** button top left | |
| 0.1.0-5 | The lesson screen | Tap **Acasă** | Back on the map at once, with no question asked | |
| 0.1.0-6 | Any screen | Turn the tablet the other way round, then hold it upright | The picture turns to stay the right way up in landscape; held upright, the app stays landscape (with black bands) | |
| 0.1.0-7 | The lesson screen | Leave the tablet alone for longer than its screen timeout | The screen stays on | |
| 0.1.0-8 | The map | Leave the tablet alone for longer than its screen timeout | The screen turns off as usual | |
| 0.1.0-9 | The lesson screen | Press the tablet's Home button, then tap the Școlița icon again | The app comes back on the lesson screen | |
| 0.1.0-10 | The tablet's Settings → Apps → Școlița → Permissions | Look at the list | "No permissions requested" (or nothing allowed) | |
