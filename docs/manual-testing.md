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
