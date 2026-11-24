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

Filled in by `parent-area` (release 0.12.0 or near it), when the whole loop
first exists on the tablet. Until then, each release's section below is the
test.

| # | Screen | Do this | You should see and hear | Tested on |
| --- | --- | --- | --- | --- |
| B1 | Home screen of the tablet | Tap the Școlița icon | The app opens on the galaxy map within 5 seconds | |

## Releases

The newest release first. Each close-out adds its section here
(`AGENTS.md` → "Versioning and releases").

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
