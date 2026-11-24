# Curriculum research for v1 (planets 1–3)

Nov 24, 2026 · agent `curriculum` (`tasks/pending/curriculum-research.md`)

The research behind `docs/curriculum.md` v1: what Romanian and English
schools teach first, how they teach writing by hand, what is known about
learning to read two languages at once, and the words, strokes and voice
lines for planets 1–3. **Every claim about how a school teaches is tied to
a source in §1** ([S1], [E2], [B3] …). Where a source could not be read
online, it says so.

**Assumed until the human answers** (`local/for-human-next-steps.md`):

- **D1:** Romanian is the anchor, both languages on (the brief's
  assumption). Nothing found argues against it (§5).
- **D3 (the model):** the Romanian school's "litere de mână" for both
  languages (the brief's assumption). §8 supports it.
- **D4 (British or American English):** British is assumed for the
  English vowels (/ɒ/ in "dot") and spelling; it changes a few sounds, not
  the order.

**Files of this research** (all drafts for the human to check, none of
them content):

| File | What it is |
| --- | --- |
| [`handwriting-model-draft.svg`](handwriting-model-draft.svg) | The 20 handwritten letters (a m t p s i n o d e, lower and upper case) on the school's writing lines, with numbered start dots, direction arrows, and the entry and exit strokes in orange |
| [`joined-words-draft.svg`](joined-words-draft.svg) | mama, tata, pat and apa joined by hand, as `TraceWord` will draw them |
| [`print-stroke-order-draft.svg`](print-stroke-order-draft.svg) | The same 20 letters in print, with the proposed stroke order (D2) |
| [`strokes-draft.json`](strokes-draft.json) | The points behind both drawings, for `tracing` and `handwriting` to start from (a draft, not the content format) |
| [`voice-lines-draft.csv`](voice-lines-draft.csv) | The voice lines of planets 1–3 in both languages (584 rows), the draft of `content/voice-lines.csv` |

## 1. Sources

### Romanian

- **[R1]** Corina Andrei, Constanța Bălan. *Comunicare în limba română.
  Manual pentru clasa I.* Corint Logistic, 2023 (approved by OM
  5022/06.07.2023; ISBN 978-630-6526-21-5).
  https://www.manuale.edituracorint.ro/2023/comunicare_I_l.pdf —
  **read**: the contents, the letter order, the a m n i e o t s p d pages
  and their handwriting models (the "lupă" with stroke arrows at the
  bottom of each letter page).
- **[R2]** Rodica Gavriș-Pascu, Ioana Camelia Iovănaș, Bianca Sabou,
  Marinela Scripcariu. *Comunicare în limba română, clasa I.* Art Klett,
  2023 (OM 5022/06.07.2023; ISBN 978-606-076-541-7).
  https://catalog.manualedigitaleart.ro/art-romana-1-2023/v1/art-romana-1-2023.pdf
  — **read**: the contents, the structure of a lesson, the a m i n e pages
  and their handwriting models.
- **[R3]** Editura Litera. *Comunicare în limba română, clasa I —
  planificare calendaristică 2023–2024* (for the Litera 2023 manual).
  https://edu.litera.ro/manuale/2023_CLR1/2023_545_CLR1_planificare.pdf —
  **read** (the manual's authors are not named in it).
- **[R4]** Petronela-Vali Slavu, Cristina-Cornelia Truță. *Comunicare în
  limba română, clasa I — planificare, unitățile 1–4.* Editura Sigma,
  2023.
  https://editurasigma.ro/wp-content/uploads/2023/09/Planificare-Comunicare-Unitati-1-4-1.pdf
  — **read** (units 1–4 only).
- **[R5]** Corina-Daciana Oprițoiu, Emanuela Patrichi, Laura Piroș,
  Anca-Veronica Tăut, Irina Vasile, Raluca Voina. *Comunicare în limba
  română, clasa I — planificare* (Corint Books manual), 2023.
  https://manuale.edituracorint.ro/2023/comunicare_I_b_planificare.pdf —
  **read**.
- **[R6]** Ministerul Educației Naționale. *Programa școlară pentru
  disciplina Comunicare în limba română, clasa pregătitoare, clasa I și
  clasa a II-a*, anexa 2 la OMEN 3418/19.03.2013 (still the programme in
  force for the 2023 manuals, [R1] [R2]).
  https://www.edums.ro/invprimar/Lb%20romana%20P%20I%20II.pdf — **read**
  (a copy hosted by a school; not checked word for word against the
  Monitorul Oficial).
- **[R7]** Inspectoratul Școlar Județean Dâmbovița. *Scrisoare metodică
  2024–2025, clasa pregătitoare* (the clasa pregătitoare guide).
  https://www.isj-db.ro/public/static/files/2024_2025/primar/Scrisoare_metodica_2024-2025_Clasa_pregatitoare_ISJ_Dambovita.pdf
  — **read**. (A publisher's teacher's guide for clasa pregătitoare was
  found but its PDF could not be opened.)
- **[R8]** Academia Română, Institutul de Lingvistică „Iorgu Iordan – Al.
  Rosetti”. *DOOM 3* (2021), §5.1 „Despărțirea în interiorul cuvintelor”.
  https://doom.lingv.ro/studiu_introductiv — the section's text **could
  not be read online** (only its heading); word entries were checked on
  dexonline (**[R9]**, https://dexonline.ro, which shows the DOOM 3 entry
  and its "desp." notes).

### English

- **[E1]** Department for Education and Skills. *Letters and Sounds:
  Principles and Practice of High Quality Phonics* (00281-2007BKT-EN),
  Primary National Strategy, 2007 (withdrawn as guidance in 2021, still
  the reference order).
  https://assets.publishing.service.gov.uk/media/5a7aa7b6e5274a34770e630c/Letters_and_Sounds_-_DFES-00281-2007.pdf
  — **read**.
- **[E2]** Wandle Learning Trust. *Little Wandle Letters and Sounds
  Revised: programme progression, Reception and Year 1* (a school's copy
  of the overview).
  https://elmridge.bright-futures.co.uk/wp-content/uploads/2022/12/Phonics-Curriculum-Overview-1.pdf
  — **read**. (The trust's own copy was not found; the order matches other
  schools' copies.)
- **[E3]** Wandle Learning Trust. *Little Wandle Handwriting Formation:
  lower case and capital letters* (a school's copy).
  https://files.schudio.com/brookfields-primary-school/files/documents/classes/little-wandle-handwriting-formation.reception.pdf
  — **read**.
- **[E4]** Department for Education. *English programmes of study: key
  stages 1 and 2. National curriculum in England*, 2013.
  https://assets.publishing.service.gov.uk/media/5a7de93840f0b62305b7f8ee/PRIMARY_national_curriculum_-_English_220714.pdf
  — **read** (the handwriting requirements).

### Bilingual learning (abstracts read through OpenAlex where the
publisher refused automated access; full text read only where noted)

- **[B1]** Melby-Lervåg, M., & Lervåg, A. (2011). Cross-linguistic
  transfer of oral language, decoding, phonological awareness and reading
  comprehension: a meta-analysis of the correlational evidence. *Journal
  of Research in Reading*, 34(1), 114–135.
  https://doi.org/10.1111/j.1467-9817.2010.01477.x
- **[B2]** Branum-Martin, L., Tao, S., Garnaat, S., Bunta, F., & Francis,
  D. J. (2012). Meta-analysis of bilingual phonological awareness.
  *Journal of Educational Psychology*, 104(4), 932–944.
  https://doi.org/10.1037/a0027755
- **[B3]** Durgunoğlu, A. Y., Nagy, W. E., & Hancin-Bhatt, B. J. (1993).
  Cross-language transfer of phonological awareness. *Journal of
  Educational Psychology*, 85(3), 453–465.
  https://doi.org/10.1037/0022-0663.85.3.453
- **[B4]** Bhalloo, I., & Molnar, M. T. (2023). Early precursors of
  reading development in simultaneous bilinguals: a systematic review and
  meta-analysis. *Perspectives of the ASHA Special Interest Groups*, 8(5),
  1103–1120. https://doi.org/10.1044/2023_PERSP-23-00041
- **[B5]** Seymour, P. H. K., Aro, M., & Erskine, J. M. (2003). Foundation
  literacy acquisition in European orthographies. *British Journal of
  Psychology*, 94(2), 143–174. https://doi.org/10.1348/000712603321661859
  (full text read).
- **[B6]** Majorano, M., Bastianello, T., Bodea Hațegan, C., et al.
  (2021). Early literacy skills and later reading and writing performance
  across countries. *Child & Youth Care Forum*, 50(6), 1063–1085.
  https://doi.org/10.1007/s10566-021-09611-7 (abstract only).
- **[B7]** Cummins, J. (1979). Linguistic interdependence and the
  educational development of bilingual children. *Review of Educational
  Research*, 49(2), 222–251. https://doi.org/10.3102/00346543049002222
- **[B8]** Soltero-González, L., Sparrow, W., Butvilofsky, S., Escamilla,
  K., & Hopewell, S. (2016). Effects of a paired literacy program on
  emerging bilingual children's biliteracy outcomes in third grade.
  *Journal of Literacy Research*, 48(1), 80–104.
  https://doi.org/10.1177/1086296X16653842
- **[B9]** Escamilla, K., et al. (2014). *Biliteracy from the Start:
  Literacy Squared in Action.* Caslon (the book was **not read**; its
  author's account was: https://nepc.colorado.edu/publication/newsletter-escamilla).
- **[B10]** Beeman, K., & Urow, C. (2012). *Teaching for Biliteracy:
  Strengthening Bridges between Languages.* Caslon (the book was **not
  read**; the authors' handout was:
  https://archive.carla.umn.edu/conferences/past/immersion2012/handouts/UrowBeeman_ThirdWay_Handout_Oct2012.pdf).
- **[B11]** August, D., Carlo, M., Dressler, C., & Snow, C. (2005). The
  critical role of vocabulary development for English language learners.
  *Learning Disabilities Research & Practice*, 20(1), 50–57.
  https://doi.org/10.1111/j.1540-5826.2005.00120.x
- **[B12]** Squires, L. R., Ohlfest, S. J., Santoro, K. E., & Roberts,
  J. L. (2020). Factors influencing cognate performance for young
  multilingual children's vocabulary: a research synthesis. *American
  Journal of Speech-Language Pathology*, 29(4), 2170–2188.
  https://doi.org/10.1044/2020_AJSLP-19-00167
- **[B13]** Simpson Baird, A., Palacios, N., & Kibler, A. (2016). The
  cognate and false cognate knowledge of young emergent bilinguals.
  *Language Learning*, 66(2), 448–470. https://doi.org/10.1111/lang.12160
- **[B14]** Petrescu, M. C., & Helms-Park, R. (2018). The lexical
  development of Canadian-born Romanian L1 bilingual kindergarteners.
  *Languages*, 3(3), 33. https://doi.org/10.3390/languages3030033
- **[B15]** Fashola, O. S., Drum, P. A., Mayer, R. E., & Kang, S.-J.
  (1996). A cognitive theory of orthographic transitioning: predictable
  errors in how Spanish-speaking children spell English words. *American
  Educational Research Journal*, 33(4), 825–843.
  https://doi.org/10.3102/00028312033004825
- **[B16]** Genesee, F., Nicoladis, E., & Paradis, J. (1995). Language
  differentiation in early bilingual development. *Journal of Child
  Language*, 22(3), 611–631. https://doi.org/10.1017/S0305000900009971

## 2. The Romanian order

### 2.1 The letters, in the order the manuals teach them

| Manual | Order of the single letters (groups in brackets where they come) |
| --- | --- |
| Corint Logistic [R1] | a m u n · i e r o · c ă l (ce ci) · t s p v · d ș î â b · j h g (ge) ț · z (gi) f (che chi) · x (ghe ghi) k q y w |
| Art Klett [R2] | a m i n e · u r o c ă · l (ce) t s p v (ci) · d ș î â b · j h g (ge) ț · z (gi) f (che chi) · x (ghe ghi) k q w y |
| Litera [R3] | a m u n o r · i e c ă l · t (ce ci) · s â î j v · g d p ș · b z ț (ge) · h f (gi che chi) · x k q w y (ghe ghi) |
| Sigma [R4] (units 1–4 only) | a m i n · r e u l · c o ă d · … |
| Corint Books [R5] | a m i n e · r u c o l · t ă (ce) d (ci) · p s v î â b · j ș ț h g (ge) · z (gi) f (che chi) · (ghe ghi) x k q y w |

(· separates the manuals' learning units.)

What they share:

- **a, then m, in all five.** Then the vowels i, e, o, u and the
  consonants n, r, c, l in the first 10–12 letters.
- **t, s, p, d come later** (from about the 10th letter on), and **ă
  comes early** (9th to 13th), because so many Romanian words end in ă.
- **Diphthongs come with their vowel:** "ia" with i, "ea, ie" with e,
  "oa" with o ([R1] contents; [R2] contents: "i, I, ia", "e, E, ea").
- **The groups** ce ci ge gi che chi ghe ghi come as their own lessons,
  spread through the year; k q w x y at the end; the whole alphabet is a
  lesson in the last unit ("Alfabetul limbii române", [R1] p. 126; [R3]
  unit 10).

### 2.2 Upper and lower case, print and handwritten

- **Each letter is taught in its four forms together:** "Sunetul și
  literele a și A de tipar și de mână" ([R4] unit 2); one page for the
  small letter, the next for the capital ([R1] pp. 20–21: the a page,
  then "Încercuiește literele a și A, scrise de mână"); "Literele mici și
  mari de tipar (a, m, u, n, o, r)" and "Literele mici și mari de mână (a,
  m, u, n, o, r)" in the same unit ([R3]).
- **Print is read, handwriting is written.** The programme's writing
  competence in clasa I is "4.1 Scrierea literelor de mână", with
  "trasarea în duct continuu a literelor de mână" (a letter written in one
  continuous line) [R6]. Children read words "scrise cu litere de tipar
  sau de mână" [R6]. No manual read here asks children to write print
  letters in clasa I.
- **Before the letters, the graphic elements** (the "perioada
  preabecedară", the first unit): "liniuță, cârlig, bastonaș, za, oval,
  semioval, noduleț, buclă, steguleț", written "pe liniatură tip I" [R4];
  "Elemente grafice care intră în componența literelor de mână" [R3]; in
  [R1] one element per page (linia oblică, bastonașul cu întorsătura sus
  and jos, zaua, bucla, cârligul, ovalul și semiovalul, pp. 8–17).
- **Clasa pregătitoare (age 6) does not teach reading or handwriting.**
  The guide says: "Competența de a citi NU este în programă", children
  read only labels as whole words, and "NU se vor folosi caietele de
  scriere tip I, specifice clasei I, nu se vor scrie litere de mână" [R7].
  The programme's clasa pregătitoare writing competence is "4.1 Trasarea
  elementelor grafice și a contururilor literelor", and its reading
  competence is recognising and naming "literele mari și mici de tipar"
  [R6]. **So Școlița is ahead of the Romanian clasa pregătitoare**, which
  is fine at home, but the human should know it if she is in clasa
  pregătitoare this year: her teacher will not yet ask for handwritten
  letters.

### 2.3 Syllables

- **From the first unit, before the letters:** "Cuvântul format din două
  silabe / trei silabe / dintr-o silabă" ([R1] pp. 10–12), "Despărțirea
  cuvintelor în silabe intuitiv" and "stabilirea numărului de silabe
  dintr-un cuvânt" [R4], "Silabe la braț" ([R2] p. 14). The programme
  lists "despărțirea cuvintelor în silabe, numărarea silabelor unui cuvânt
  și reprezentarea lor prin…" and "rostirea cuvintelor pe silabe, în
  jocuri, cântece" for clasa pregătitoare [R6].
- **How the manuals show it:** a dash between the syllables and one
  circle per syllable under a picture ("a-●●●●", "●●-●●"; [R1] p. 20,
  [R2] p. 20: "Desparte cuvintele în silabe").
- **The rules** are DOOM 3's (§5.1 [R8], not readable online; checked
  word by word on dexonline [R9]). The splits used here follow the
  phonetic rules schools teach: one consonant between vowels goes with the
  next vowel (ma-ma, pe-pe-ne); two consonants split (pan-da, din-te,
  pas-ta); two vowels in hiatus split (pi-an, sa-ni-e: dexonline shows
  "sa-ni-e" and DOOM 3 "desp. -ni-e"); a diphthong stays in one syllable
  (nai, stea, po-nei, pi-soi). Only two of the planet words have a split
  printed on dexonline (sanie; telescop, where DOOM 3 allows both
  "te-les-cop" and "te-le-scop"); **the human checks the rest** (§6.6).

## 3. The English order

### 3.1 Letters and Sounds [E1]

- **Phase 2** (one set a week): **s a t p · i n m d · g o c k · ck e u r ·
  h b f ff l ll ss**, "securing first the starter letters s, a, t, p, i,
  n". Tricky words: **the, to, I, no, go** (and "into").
- **Phase 3:** j v w x · y z zz qu · ch sh th ng · ai ee igh oa oo ar or
  ur ow oi ear air ure er. Tricky words: he, she, we, me, be, was, my,
  you, her, they, all, are. **Letter names are taught in Phase 3**
  ("Teaching letter names (if not already taught)", with an alphabet
  song), after the sounds.
- **Phase 4:** no new graphemes; **adjacent consonants** (CVCC and CCVC
  words like tent, stop). Tricky words: said, have, like, so, do, some,
  come, were, there, little, one, when, out, what.
- **Phase 5:** ay ou ie ea oy ir ue aw wh ph ew oe au a-e e-e i-e o-e u-e,
  and alternative pronunciations. Tricky words: oh, their, people, Mr,
  Mrs, looked, called, asked, could.
- **The pure-sound rule**, in its own words: "Do not add an 'uh' to the
  end of sounds: 'ssss' not 'suh', 'mmm' not 'muh', 't' not 'tuh'".

### 3.2 Little Wandle Letters and Sounds Revised [E2]

- **Reception autumn 1:** s a t p i n m d g o c k ck e u r h b f l, with
  the tricky words **is, I, the**. Autumn 2: ff ll ss j v w x y z zz qu ch
  sh th ng nk (tricky words as and has his her go no to into she he of we
  me be). Spring: the Phase 3 vowel digraphs. Summer: **Phase 4, short
  vowels with adjacent consonants** (CVCC, CCVC …). Year 1: Phase 5.
- **Letter formation** [E3]: every lower-case letter starts at the top
  ("Around the astronaut's helmet and down into space" for a; "Down, up
  and over" for m and n; "Down the penguin's back, up and around its head"
  for p).

### 3.3 What it means for Școlița

- Both English programmes start with **s a t p i n m d**: all of
  planets 1–3's consonants and two of its vowels are in the first two
  weeks of English phonics; o and e come in the next two.
- **CVC words first, adjacent consonants later.** Words like tent, nest,
  pond, stop, mint use only taught letters but belong to Phase 4: they are
  marked and come at the end of a planet's words.
- **"is"** is decodable only if s can say /z/; Little Wandle teaches it as
  a tricky word [E2]. It is "reading only" and never a target in
  planets 1–3.

## 4. The bilingual order: planets 1–3 checked

### 4.1 The check

| Letter | Planet (v0 and v1) | Romanian manuals: position | English (L&S [E1]) | Sound RO / EN | Verdict |
| --- | --- | --- | --- | --- | --- |
| a | 1 | 1st in all | set 1 | /a/ / /æ/ | Keep; `TwoSounds` |
| m | 1 | 2nd in all | set 2 | /m/ / /m/ | Keep |
| t | 1 | 10th–14th | set 1 | /t/ / /t/ (EN aspirated) | Keep: needed for tata, pat in RO and for English set 1 |
| p | 1 | 15th–18th | set 1 | /p/ / /p/ (EN aspirated) | Keep: pat, apa, papa |
| s | 2 | 14th–16th | set 1 | /s/ / /s/ (and /z/ in "is") | Keep |
| i | 2 | 3rd–6th | set 2 | /i/ / /ɪ/ | Keep; `TwoSounds` |
| n | 2 | 3rd–4th | set 2 | /n/ / /n/ | Keep |
| o | 3 | 5th–9th | set 3 | /o/ / /ɒ/ | Keep; `TwoSounds` |
| d | 3 | 12th–19th | set 2 | /d/ / /d/ | Keep |
| e | 3 | 5th–7th | set 4 | /e/ / /e/ | Keep; a light `TwoSounds` (the vowels are close) |

**Result: planets 1–3 stay as in v0** (a m t p · s i n · o d e), each in
the same order inside its planet. The reasons:

- **a and m first** match every Romanian manual, and both are in English
  phonics' first two weeks.
- **t, p, s, d come earlier than in a Romanian manual** on purpose: they
  are English phonics' first letters [E1] [E2], their sounds are almost
  the same in both languages, and they give the Romanian words the family
  and her world need at once (tata, pat, apa, nas, pian, stea).
- **The alternative considered:** planet 1 = a m i n (the letters every
  early list shares). It gives Romanian mama, Ana, nai, mina but English
  only man, an, am, in. Worse for both; not taken.
- **English planet 1 is thin** (map, mat, tap, pat, at, am, and the name
  Pam: 7 words, 4 of them pictures; §6.1). Letters and Sounds has the same
  limit in its own first week (s a t p: "sat, pat, tap, at …"). The brief
  asks for 8 per planet per language: **English planet 1 has 7, Romanian
  9.** Recommended: accept it (the English word lesson of planet 1 needs
  4–5 words). The other way out, swapping t and n between planets 1 and
  2, loses tata and pat in Romanian. **For the human to agree.**

### 4.2 Planets 4–9: a sanity check only

- **Planet 4 (u r c l)** matches the Romanian manuals' early letters
  (u, r, c, l are in every manual's first 12); c, r and u differ in
  English and get `TwoSounds`.
- **ă is on planet 7 in v0, but 9th–13th in every Romanian manual.**
  Almost every Romanian feminine noun ends in ă (lună, rachetă, pisică,
  tobă), so until ă is taught the Romanian words must use the article form
  (apa, masa, pata, nota) or avoid feminine nouns. **Recommended for
  `content-letters-2`:** consider bringing ă to planet 4 (u r c l ă) or 5,
  and keeping â î ș ț on planet 7. Not changed here (out of scope; the
  order is the human's call).
- **Planets 5, 6 and 8** follow the manuals' and English phonics' later
  letters; no conflict found. The groups (planet 9) come later in both.

## 5. Bilingual teaching: what is known (one page)

**Learning to read two languages at once works, and skills carry
across.** Phonological awareness (hearing the sounds in words) and
decoding correlate moderately to strongly between a child's two
languages, while oral vocabulary correlates only weakly [B1]; in a classic
study, Spanish phonological awareness predicted English word reading,
and oral proficiency did not [B3]; a review of 41 reports on simultaneous
bilinguals finds both within-language and cross-language effects [B4].
How strongly they transfer depends on the pair of languages and on the
size of the unit (phoneme or syllable) [B2] [B1]. **No study of
Romanian–English children learning to read was found.**

**A transparent spelling is learned much faster.** By the end of their
first school year, children learning a consistent orthography read
familiar words at 95–98% accuracy, while Scottish children learning
English read 34%; English foundation literacy is "more than twice as
slow" [B5]. Romanian was not in that study; a later study including
Romanian children found higher early literacy scores for more transparent
spellings [B6]. **Romanian as the anchor is the sensible place to crack
the code**, and English needs more practice for the same letters.

**Teach each language on its own, then build planned bridges.** A paired
programme teaching Spanish and English literacy side by side from
kindergarten beat teaching one after the other in both languages
(effect sizes 0.42–0.90 by grade 3) [B8]. Its authors describe the method
as explicit teaching in each language plus "careful attention to
cross-language connections" [B9]; Beeman and Urow's "Bridge" is a planned
moment where both languages are side by side and the child compares their
sounds and words [B10]. Cummins' interdependence hypothesis is the
classic account of why skills built in one language help the other [B7].

**Show the differences openly, and mark each language.** Children carry
their first language's letter–sound rules into the second's spelling in
predictable ways [B15]. Young bilinguals keep their two languages apart
from very early on, even when they mix words [B16]. Nothing tests
language badges as such, but marking each item's language matches what
children already do.

**Cognates help a little, and not every child.** Drawing on shared words
is a useful vocabulary strategy [B11]; across 16 studies of children aged
3–8, 12 found a cognate advantage, but not for every child, and almost
none taught cognates on purpose [B12]. Young children were not confused
by false cognates [B13]. The one Romanian–English study (three children)
found no clear cognate benefit [B14].

**What this changes in `docs/curriculum.md`:**

- §1 stays (both at once, Romanian as the anchor, one language per item),
  now with sources.
- **§1 and §4.1: the bridges are planned moments**, not mixing: each
  letter's English lesson starts from the Romanian one (`TwoSounds` when
  the sounds differ), and each planet party has its `WordTwins` and false
  friends. Twins are a bonus, not a pillar [B12] [B14].
- **§5: English gets more practice** for the same letters (a suggestion
  for `progress-review` to tune: English items may need more right
  answers before "learned", or more review) [B5].
- **§1 and §4.2: names later, sounds first** (§6.7).

## 6. The words of planets 1–3

### 6.1 Summary

| Planet | Romanian readable (pictures) | English readable (pictures) |
| --- | --- | --- |
| 1 (a m t p) | 9 (8) | 7 (4, plus the name Pam) |
| 2 (+ s i n) | 14 (13, plus the name Ana) | 16 (10, plus the name Sam) |
| 3 (+ o d e) | 21 (21) | 19 (17) |

- **Picture:** "yes" when a 6-year-old can tell it from one drawing
  without words. **Her world:** her favourite things (PRD §1) or close to
  her (family, home, body).
- **TraceWord** marks the four words per planet and language chosen for
  writing by hand (up to 5 letters, `handwriting` brief).
- **Romanian articles:** apa, pata, masa, sita, nota are the article forms
  ("the water") of nouns ending in ă; they are ordinary Romanian words and
  readable before ă is taught. The bare forms come with ă.
- **Checked on dexonline [R9]:** "som" (catfish) is not in DOOM 3 (the
  word is "somn"), and "pisi" (kitty) is not in any dictionary: **both
  were dropped**. "mami", "tati" (not in DOOM 3), "tanti", "nani" (DOOM 3:
  fam.) and "papa" (food, DEX: "în graiul copiilor") are familiar or child
  talk: **kept and marked, for the human to decide**.
- **Diphthongs and hiatus** are marked (nai, stea, ponei, pisoi; pian,
  sanie): the Romanian manuals teach ia, ea, ie, oa with their vowels
  [R1] [R2], so these words are fair once the vowel is known, but they
  come after the plain words in a lesson.
- **English clusters** (Phase 4) and "reading only" words are marked
  (§3.3).

### 6.2–6.5 The lists, the listen-only words, and the taught-letters check

The tables below are generated from one list (also used for the voice
lines), so the words, the check and the CSV agree.

#### Planet 1, Romanian (letters taught: a m t p)

| Word | Letters | Syllables | Picture | Her world | Notes |
| --- | --- | --- | --- | --- | --- |
| mama | m a m a | ma-ma | yes | family | TraceWord |
| tata | t a t a | ta-ta | yes | family | TraceWord |
| pat | p a t | pat | yes | home | false friend: en pat; TraceWord; the bed of design screen 5 |
| apa | a p a | a-pa | yes | Earth, nature | TraceWord; 'the water' (apă with its article: apă needs ă, planet 7) |
| pata | p a t a | pa-ta | yes | drawing | 'the spot' (a paint blot); pată needs ă |
| papa | p a p a | pa-pa | yes | home | child talk (DEX: în graiul copiilor); 'food' (hai la papa), stressed PA-pa; the human decides |
| pa | p a | pa | yes | — | interjection; 'bye!' with a waving hand |
| tam-tam | t a m t a m | tam-tam | yes | music | a gong; two syllables with a hyphen |
| am | a m | am | no | — | reading only; 'I have' |

#### Planet 2, Romanian (letters taught: a m t p s i n)

| Word | Letters | Syllables | Picture | Her world | Notes |
| --- | --- | --- | --- | --- | --- |
| nas | n a s | nas | yes | body | TraceWord |
| masa | m a s a | ma-sa | yes | home | TraceWord; 'the table' (masă needs ă) |
| pian | p i a n | pi-an | yes | music | twin: en piano (listen only); TraceWord; hiatus: two syllables |
| nai | n a i | nai | yes | music | diphthong ai; the pan flute; one syllable |
| pin | p i n | pin | yes | nature | false friend: en pin; a pine tree |
| sat | s a t | sat | yes | Earth | false friend: en sat; a village |
| ananas | a n a n a s | a-na-nas | yes | food | three beats for the drum |
| mami | m a m i | ma-mi | yes | family | familiar, not in DOOM 3; very common at home; the human decides |
| tati | t a t i | ta-ti | yes | family | familiar, not in DOOM 3; the Corint manual writes it for t; the human decides |
| nani | n a n i | na-ni | yes | moon, night | familiar (DOOM 3: fam.); 'sleep' (hai la nani): a sleeping moon |
| pas | p a s | pas | yes | — | a step (a footprint) |
| spin | s p i n | spin | yes | nature | a thorn |
| sita | s i t a | si-ta | yes | home | 'the sieve' (sită needs ă) |
| Ana | a n a | A-na | yes | — | name; a name: capital A; the Art manual's first sentence |

#### Planet 3, Romanian (letters taught: a m t p s i n o d e)

| Word | Letters | Syllables | Picture | Her world | Notes |
| --- | --- | --- | --- | --- | --- |
| pom | p o m | pom | yes | nature | TraceWord; a fruit tree |
| pod | p o d | pod | yes | Earth | false friend: en pod; TraceWord; a bridge |
| dinte | d i n t e | din-te | yes | body | TraceWord |
| stea | s t e a | stea | yes | space | diphthong ea; TraceWord; a star, readable from planet 3 (v0 said planet 7) |
| mop | m o p | mop | yes | home | twin: en mop |
| dop | d o p | dop | yes | home | a cork |
| pepene | p e p e n e | pe-pe-ne | yes | food | a watermelon |
| domino | d o m i n o | do-mi-no | yes | games | twin: en domino (listen only) |
| desen | d e s e n | de-sen | yes | drawing | a drawing |
| dans | d a n s | dans | yes | music | a dance |
| nota | n o t a | no-ta | yes | music | 'the note' (notă needs ă) |
| ponei | p o n e i | po-nei | yes | unicorns | diphthong ei; twin-ish: en pony (listen only); a pony |
| pisoi | p i s o i | pi-soi | yes | animals | diphthong oi; a kitten |
| panda | p a n d a | pan-da | yes | animals | twin: en panda (listen only) |
| piton | p i t o n | pi-ton | yes | animals | a python |
| tenis | t e n i s | te-nis | yes | sport | twin: en tennis (listen only) |
| sanie | s a n i e | sa-ni-e | yes | winter | a sled; hiatus: three syllables |
| patine | p a t i n e | pa-ti-ne | yes | sport | skates |
| mops | m o p s | mops | yes | animals | false friend: en mops; a pug |
| nod | n o d | nod | yes | — | false friend: en nod; a knot |
| stop | s t o p | stop | yes | cycling, Earth | twin: en stop; a stop sign |

#### Planet 1, English (letters taught: a m t p)

| Word | Letters | Syllables | Picture | Her world | Notes |
| --- | --- | --- | --- | --- | --- |
| map | m a p | map | yes | space, Earth | TraceWord |
| mat | m a t | mat | yes | home | false friend: ro mat; TraceWord |
| tap | t a p | tap | yes | home | TraceWord; a water tap |
| pat | p a t | pat | yes | animals | false friend: ro pat; TraceWord; patting a dog |
| at | a t | at | no | — | reading only |
| am | a m | am | no | — | reading only |
| Pam | p a m | Pam | yes | — | name; a name: capital P |

#### Planet 2, English (letters taught: a m t p s i n)

| Word | Letters | Syllables | Picture | Her world | Notes |
| --- | --- | --- | --- | --- | --- |
| pin | p i n | pin | yes | drawing | false friend: ro pin; TraceWord |
| sit | s i t | sit | yes | — | TraceWord |
| man | m a n | man | yes | family | TraceWord |
| tin | t i n | tin | yes | home | TraceWord |
| pan | p a n | pan | yes | home | — |
| ant | a n t | ant | yes | animals | VCC |
| nap | n a p | nap | yes | moon, night | — |
| sip | s i p | sip | yes | — | — |
| tip | t i p | tip | no | — | — |
| sat | s a t | sat | no | — | false friend: ro sat |
| in | i n | in | no | — | reading only |
| it | i t | it | no | — | reading only |
| is | i s | is | no | — | reading only; s says /z/; Little Wandle teaches is as a tricky word |
| Sam | s a m | Sam | yes | — | name |
| mint | m i n t | mint | yes | food | adjacent consonants (Phase 4) |
| pants | p a n t s | pants | yes | home | adjacent consonants (Phase 4); underpants in British English |

#### Planet 3, English (letters taught: a m t p s i n o d e)

| Word | Letters | Syllables | Picture | Her world | Notes |
| --- | --- | --- | --- | --- | --- |
| dot | d o t | dot | yes | drawing | TraceWord |
| pen | p e n | pen | yes | drawing | TraceWord |
| mop | m o p | mop | yes | home | twin: ro mop; TraceWord |
| ten | t e n | ten | yes | numbers | TraceWord |
| pot | p o t | pot | yes | home | — |
| top | t o p | top | yes | games | a spinning top |
| net | n e t | net | yes | sport | — |
| dad | d a d | dad | yes | family | — |
| pet | p e t | pet | yes | animals | — |
| pod | p o d | pod | yes | nature | false friend: ro pod; a pea pod |
| nod | n o d | nod | yes | — | false friend: ro nod; nodding |
| on | o n | on | no | — | reading only |
| not | n o t | not | no | — | reading only |
| tent | t e n t | tent | yes | space camp | adjacent consonants (Phase 4) |
| nest | n e s t | nest | yes | animals | adjacent consonants (Phase 4) |
| pond | p o n d | pond | yes | nature | adjacent consonants (Phase 4) |
| stop | s t o p | stop | yes | cycling | adjacent consonants (Phase 4); twin: ro stop |
| spot | s p o t | spot | yes | drawing | adjacent consonants (Phase 4) |
| mops | m o p s | mops | yes | home | adjacent consonants (Phase 4); false friend: ro mops |

#### Listen-only words (pictures and sounds, never to read in planets 1–3)

| Language | First letter | Word | Syllables | Her world |
| --- | --- | --- | --- | --- |
| ro | a | avion | a-vi-on | flying |
| ro | a | astronaut | as-tro-na-ut | space (word twin) |
| ro | a | albină | al-bi-nă | animals |
| ro | m | muzică | mu-zi-că | music |
| ro | m | melc | melc | animals |
| ro | t | tobă | to-bă | music |
| ro | t | telescop | te-les-cop | space (DOOM 3 also allows te-le-scop) |
| ro | p | planetă | pla-ne-tă | space |
| ro | p | pisică | pi-si-că | animals (the standard word for kitty) |
| ro | s | soare | soa-re | space |
| ro | s | saxofon | sa-xo-fon | music |
| ro | i | inel | i-nel | unicorns |
| ro | i | insulă | in-su-lă | Earth |
| ro | n | nor | nor | sky |
| ro | n | nucă | nu-că | food |
| ro | o | ochelari | o-che-lari | home |
| ro | o | orologiu | o-ro-lo-giu | home |
| ro | d | delfin | del-fin | animals |
| ro | d | dinozaur | di-no-za-ur | animals |
| ro | e | elefant | e-le-fant | animals |
| ro | e | elicopter | e-li-cop-ter | flying |
| ro | r | rachetă | ra-che-tă | space (word twin) |
| ro | u | unicorn | u-ni-corn | unicorns (word twin) |
| ro | c | curcubeu | cur-cu-beu | rainbows |
| ro | v | vioară | vi-oa-ră | music (word twin) |
| ro | c | chitară | chi-ta-ră | music (word twin) |
| ro | b | bicicletă | bi-ci-cle-tă | cycling (word twin) |
| en | a | apple | ap-ple | food |
| en | a | astronaut | as-tro-naut | space |
| en | a | ant | ant | animals (readable from planet 2) |
| en | m | moon | moon | space |
| en | m | music | mu-sic | music |
| en | t | telescope | tel-e-scope | space |
| en | t | tiger | ti-ger | animals |
| en | p | planet | plan-et | space |
| en | p | piano | pi-an-o | music (word twin) |
| en | s | star | star | space |
| en | s | sun | sun | space |
| en | i | insect | in-sect | animals |
| en | i | igloo | ig-loo | Earth |
| en | n | nest | nest | animals (readable from planet 3) |
| en | n | nose | nose | body |
| en | o | octopus | oc-to-pus | animals |
| en | o | orange | or-ange | food |
| en | d | dolphin | dol-phin | animals |
| en | d | drum | drum | music |
| en | e | egg | egg | food |
| en | e | elephant | el-e-phant | animals |
| en | r | rocket | rock-et | space (word twin) |
| en | u | unicorn | u-ni-corn | unicorns (word twin) |
| en | r | rainbow | rain-bow | rainbows |
| en | v | violin | vi-o-lin | music (word twin) |
| en | g | guitar | gui-tar | music (word twin) |
| en | b | bicycle | bi-cy-cle | cycling (word twin) |
| en | p | panda | pan-da | animals (word twin) |
| en | t | tennis | ten-nis | sport (word twin) |
| en | d | domino | dom-i-no | games (word twin) |
| en | p | pony | po-ny | unicorns |

#### The taught-letters check

| Language | Planet | Word | Its letters | Letters taught by then | Only taught letters? |
| --- | --- | --- | --- | --- | --- |
| ro | 1 | mama | m a m a | a m t p | yes |
| ro | 1 | tata | t a t a | a m t p | yes |
| ro | 1 | pat | p a t | a m t p | yes |
| ro | 1 | apa | a p a | a m t p | yes |
| ro | 1 | pata | p a t a | a m t p | yes |
| ro | 1 | papa | p a p a | a m t p | yes |
| ro | 1 | pa | p a | a m t p | yes |
| ro | 1 | tam-tam | t a m t a m | a m t p | yes |
| ro | 1 | am | a m | a m t p | yes |
| ro | 2 | nas | n a s | a m t p s i n | yes |
| ro | 2 | masa | m a s a | a m t p s i n | yes |
| ro | 2 | pian | p i a n | a m t p s i n | yes |
| ro | 2 | nai | n a i | a m t p s i n | yes |
| ro | 2 | pin | p i n | a m t p s i n | yes |
| ro | 2 | sat | s a t | a m t p s i n | yes |
| ro | 2 | ananas | a n a n a s | a m t p s i n | yes |
| ro | 2 | mami | m a m i | a m t p s i n | yes |
| ro | 2 | tati | t a t i | a m t p s i n | yes |
| ro | 2 | nani | n a n i | a m t p s i n | yes |
| ro | 2 | pas | p a s | a m t p s i n | yes |
| ro | 2 | spin | s p i n | a m t p s i n | yes |
| ro | 2 | sita | s i t a | a m t p s i n | yes |
| ro | 2 | Ana | a n a | a m t p s i n | yes |
| ro | 3 | pom | p o m | a m t p s i n o d e | yes |
| ro | 3 | pod | p o d | a m t p s i n o d e | yes |
| ro | 3 | dinte | d i n t e | a m t p s i n o d e | yes |
| ro | 3 | stea | s t e a | a m t p s i n o d e | yes |
| ro | 3 | mop | m o p | a m t p s i n o d e | yes |
| ro | 3 | dop | d o p | a m t p s i n o d e | yes |
| ro | 3 | pepene | p e p e n e | a m t p s i n o d e | yes |
| ro | 3 | domino | d o m i n o | a m t p s i n o d e | yes |
| ro | 3 | desen | d e s e n | a m t p s i n o d e | yes |
| ro | 3 | dans | d a n s | a m t p s i n o d e | yes |
| ro | 3 | nota | n o t a | a m t p s i n o d e | yes |
| ro | 3 | ponei | p o n e i | a m t p s i n o d e | yes |
| ro | 3 | pisoi | p i s o i | a m t p s i n o d e | yes |
| ro | 3 | panda | p a n d a | a m t p s i n o d e | yes |
| ro | 3 | piton | p i t o n | a m t p s i n o d e | yes |
| ro | 3 | tenis | t e n i s | a m t p s i n o d e | yes |
| ro | 3 | sanie | s a n i e | a m t p s i n o d e | yes |
| ro | 3 | patine | p a t i n e | a m t p s i n o d e | yes |
| ro | 3 | mops | m o p s | a m t p s i n o d e | yes |
| ro | 3 | nod | n o d | a m t p s i n o d e | yes |
| ro | 3 | stop | s t o p | a m t p s i n o d e | yes |
| en | 1 | map | m a p | a m t p | yes |
| en | 1 | mat | m a t | a m t p | yes |
| en | 1 | tap | t a p | a m t p | yes |
| en | 1 | pat | p a t | a m t p | yes |
| en | 1 | at | a t | a m t p | yes |
| en | 1 | am | a m | a m t p | yes |
| en | 1 | Pam | p a m | a m t p | yes |
| en | 2 | pin | p i n | a m t p s i n | yes |
| en | 2 | sit | s i t | a m t p s i n | yes |
| en | 2 | man | m a n | a m t p s i n | yes |
| en | 2 | tin | t i n | a m t p s i n | yes |
| en | 2 | pan | p a n | a m t p s i n | yes |
| en | 2 | ant | a n t | a m t p s i n | yes |
| en | 2 | nap | n a p | a m t p s i n | yes |
| en | 2 | sip | s i p | a m t p s i n | yes |
| en | 2 | tip | t i p | a m t p s i n | yes |
| en | 2 | sat | s a t | a m t p s i n | yes |
| en | 2 | in | i n | a m t p s i n | yes |
| en | 2 | it | i t | a m t p s i n | yes |
| en | 2 | is | i s | a m t p s i n | yes |
| en | 2 | Sam | s a m | a m t p s i n | yes |
| en | 2 | mint | m i n t | a m t p s i n | yes |
| en | 2 | pants | p a n t s | a m t p s i n | yes |
| en | 3 | dot | d o t | a m t p s i n o d e | yes |
| en | 3 | pen | p e n | a m t p s i n o d e | yes |
| en | 3 | mop | m o p | a m t p s i n o d e | yes |
| en | 3 | ten | t e n | a m t p s i n o d e | yes |
| en | 3 | pot | p o t | a m t p s i n o d e | yes |
| en | 3 | top | t o p | a m t p s i n o d e | yes |
| en | 3 | net | n e t | a m t p s i n o d e | yes |
| en | 3 | dad | d a d | a m t p s i n o d e | yes |
| en | 3 | pet | p e t | a m t p s i n o d e | yes |
| en | 3 | pod | p o d | a m t p s i n o d e | yes |
| en | 3 | nod | n o d | a m t p s i n o d e | yes |
| en | 3 | on | o n | a m t p s i n o d e | yes |
| en | 3 | not | n o t | a m t p s i n o d e | yes |
| en | 3 | tent | t e n t | a m t p s i n o d e | yes |
| en | 3 | nest | n e s t | a m t p s i n o d e | yes |
| en | 3 | pond | p o n d | a m t p s i n o d e | yes |
| en | 3 | stop | s t o p | a m t p s i n o d e | yes |
| en | 3 | spot | s p o t | a m t p s i n o d e | yes |
| en | 3 | mops | m o p s | a m t p s i n o d e | yes |

#### Join pairs in the TraceWord words (every {x}–{y} of the letter checkpoint)

| Language | Pair | In |
| --- | --- | --- |
| en | a–n | man |
| en | a–p | map, tap |
| en | a–t | mat, pat |
| en | d–o | dot |
| en | e–n | pen, ten |
| en | i–n | pin, tin |
| en | i–t | sit |
| en | m–a | man, map, mat |
| en | m–o | mop |
| en | o–p | mop |
| en | o–t | dot |
| en | p–a | pat |
| en | p–e | pen |
| en | p–i | pin |
| en | s–i | sit |
| en | t–a | tap |
| en | t–e | ten |
| en | t–i | tin |
| ro | a–m | mama |
| ro | a–n | pian |
| ro | a–p | apa |
| ro | a–s | masa, nas |
| ro | a–t | pat, sat, tata |
| ro | d–i | dinte |
| ro | e–a | stea |
| ro | i–a | pian |
| ro | i–n | dinte |
| ro | m–a | mama, masa |
| ro | n–a | nas |
| ro | n–t | dinte |
| ro | o–d | pod |
| ro | o–m | pom |
| ro | p–a | apa, pat |
| ro | p–i | pian |
| ro | p–o | pod, pom |
| ro | s–a | masa, sat |
| ro | s–t | stea |
| ro | t–a | tata |
| ro | t–e | dinte, stea |

### 6.6 Word twins and false friends

| Romanian | English | Kind | Readable |
| --- | --- | --- | --- |
| mop | mop | twin | both from planet 3 |
| stop | stop | twin | both from planet 3 (English: Phase 4 cluster) |
| domino | domino | twin | Romanian from planet 3; English listen only (final o) |
| panda | panda | twin | Romanian from planet 3; English listen only |
| tenis | tennis | twin | Romanian from planet 3; English listen only |
| pian | piano | twin | Romanian from planet 2; English listen only |
| ponei | pony | near twin | Romanian from planet 3; English listen only |
| astronaut, planetă, rachetă, unicorn, telescop, vioară, chitară, bicicletă | astronaut, planet, rocket, unicorn, telescope, violin, guitar, bicycle | twins | listen only in planets 1–3 (pictures for `WordTwins`) |
| pat (a bed) | pat (a pat) | false friend | both from planet 1 |
| sat (a village) | sat (sat down) | false friend | both from planet 2 |
| pin (a pine tree) | pin (a pin) | false friend | both from planet 2 |
| pod (a bridge) | pod (a pea pod) | false friend | both from planet 3 |
| nod (a knot) | nod (a nod) | false friend | both from planet 3 |
| mops (a pug) | mops (more than one mop) | false friend | both from planet 3 |

### 6.7 Sounds and names

Both school systems teach the **sound** first and the letter's **name**
later: Letters and Sounds in Phase 3 [E1], the Romanian manuals with the
alphabet at the end of clasa I ([R1] p. 126, [R3] unit 10); every
Romanian lesson is "Sunetul și litera …" [R1] [R4] [R5]. For a bilingual
child the vowel names also clash: the English **name** of e ("ee") is
the Romanian **sound** of i, and the English name of i ("eye") is no
Romanian sound at all. **Recommended for v1:** in planets 1–3, letters are
met and referred to by their **sound** only (the spliced {x} in "Atinge
litera {x}" is the sound clip); the names are recorded but taught later,
on their own (an alphabet song, after planet 6). This changes
`MeetLetter` (no name in planets 1–3) and design screen 2's name button
(hidden until names are taught): **for the human to agree**.

## 7. Print stroke order (decision D2)

- **Romanian schools prescribe no print stroke order:** children read
  print and write by hand ([R6]: clasa I writes "literele de mână"; clasa
  pregătitoare traces "contururile literelor" and graphic elements, not
  print letters stroke by stroke).
- **English schools do** [E3], [E4] ("begin to form lower-case letters in
  the correct direction, starting and finishing in the right place", Year
  1): every letter starts at the top; round letters (a d o s, and e's
  curve) go anticlockwise; m n p go down and then back up the same line.
- **So there is no conflict; the proposal for D2:** print letters follow
  the English top-start order [E3], drawn in
  [`print-stroke-order-draft.svg`](print-stroke-order-draft.svg); **lower
  case first** (as both programmes and v0's recommendation), each capital
  later in the same planet (the Romanian manuals give each capital its own
  lesson right after its small letter [R1] [R4]).

| Letter | Print (English top-start, [E3]) | How the handwritten form differs (§8) |
| --- | --- | --- |
| a | From the top right, round anticlockwise, up and down the stem (1 stroke) | Starts with an entry stroke from the baseline; ends with an exit hook |
| m | Down; back up, over, down; back up, over, down (1 stroke) | Starts with a small hook at the middle line; ends with an exit hook |
| t | Down from above the middle line with a flick; lift; across (2 strokes) | Entry stroke from the baseline up to the top line; the bar last |
| p | Down below the line; back up, round to close the bowl (1 stroke) | Entry, the long stroke down, back up and over like n, exit hook |
| s | From the top right, round like a snake (1 stroke) | A different shape: entry up to a point, then the belly, exit |
| i | Down; lift; dot (2 strokes) | Entry from the baseline; exit hook; the dot last |
| n | Down; back up, over, down (1 stroke) | Like m, one arch |
| o | From the top, round anticlockwise (1 stroke) | Entry from the baseline; a small loop at the top; exit at the middle line |
| d | Round like a; up to the top line; down (1 stroke) | Like a, with a tall stem |
| e | Across from the middle, then round anticlockwise (1 stroke) | Entry from the baseline into a loop; exit |
| A | Down left from the top; back up; down right; lift; across (2 strokes) | Starts at the baseline with a curl, up to the top, down with a hook; the bar after |
| M | Down; back up; down to the middle; up; down (1 stroke) | Starts at the baseline with a curl; up, down, up, down with a hook |
| T | Down; lift; across the top (2 strokes) | A wavy top line, then the stem with a hook to the left |
| P | Down; back up; round to the middle (1 stroke) | The stem with a hook to the left; lift; the bowl from the top |
| S | From the top right, round like a snake (1 stroke) | Up into a loop at the top, a long stroke down, a hook to the left |
| I | Down (1 stroke) | A small wave at the top, then the stem with a hook to the left |
| N | Down; back up; diagonal down; up (1 stroke) | Starts with a curl, up, diagonal down, up, a curl at the top |
| O | From the top, round anticlockwise (1 stroke) | The same, closing with a small loop at the top |
| D | Down; back up; round to the bottom (1 stroke) | Down with a loop at the bottom left, then the big curve round to the top |
| E | Down; back up; across the top; lift; middle; lift; bottom (3 strokes) | Two bowls with a small loop in the middle, like a reversed 3 |

## 8. The handwriting model (decision D3)

### 8.1 What the Romanian manuals show

Both manuals read here draw the same model ([R1] pp. 20–73, the
magnifying glass at the bottom of each letter page with its stroke
arrows; [R2] pp. 20–29, "Scrie după model"). It is **drawn in
[`handwriting-model-draft.svg`](handwriting-model-draft.svg)** (traced
by eye from the manuals' pictures: the human compares it with them).

- **The writing lines ("liniatura tip I"):** four horizontal lines (a
  dashed top line, a solid middle line, a solid baseline, a dashed line
  below), three equal spaces, and **slanted guide lines**; the letters
  lean right (measured on the pages at roughly 75–80°). Small letters fill
  the middle space; t, d and the capitals reach the top line; p reaches
  the bottom line.
- **One continuous line per letter**, lifting only for the dot of i, the
  bar of t and A, and the second part of T and P ("trasarea în duct
  continuu" [R6]).
- **Entry strokes** ("linioara") rise from the baseline into the letter
  for a, d, o (to the top right of the oval, which is then drawn
  anticlockwise back over it), i, t, p, s and e. **m and n have no
  baseline entry:** they start with a small hook at the middle line
  ("bastonașul cu întorsătura în partea de sus", [R1] p. 11), and the
  previous letter's exit reaches up to it.
- **Exit strokes:** a small hook to the right on the baseline for a, m,
  n, i, t, p, d, s, e; **o exits at the middle line**, so the next letter
  joins high (o + m in "om", o + i in "oi", [R1] p. 40).
- **Joins:** in a word, the exit of one letter becomes the entry of the
  next, without lifting ("aa", "am", "mama", "tati", "sus", [R1] and
  [R2]); the dots and bars are added after the word ("tati", [R1] p. 60).
  Joined words are in [`joined-words-draft.svg`](joined-words-draft.svg).
- **Capitals:** A, M, N start at the baseline with a small curl and join
  to the next letter ("Ama", "Mia", "Nima", [R1] [R2]); T, P, D, I, O, S,
  E are drawn as in the sheet. The MVP joins lower case only (brief
  `handwriting`).

### 8.2 How English school handwriting differs

- The national curriculum asks for correct formation in Year 1 and
  "diagonal and horizontal strokes needed to join letters" from Year 2
  (age 6–7), with joined writing the norm by Year 3; it names **no single
  style**, and asks pupils to learn "which letters, when adjacent to one
  another, are best left unjoined" [E4]. Schools choose a scheme; Little
  Wandle starts with print formation from the top [E3].
- So an English school would teach print first and join a year later,
  usually without the Romanian entry strokes, loops on capitals or slant
  lines, and with some letters left unjoined.
- **The proposal for D3 stands:** one hand for both languages, the
  Romanian model (the letters are the same; only the sounds differ;
  English schools accept any joined style [E4]). If she later goes to an
  English school, her handwriting will be joined and slanted where her
  classmates print: worth knowing, not a problem.

### 8.3 What this changes for `tracing`, `handwriting` and `look`

- **The writing board needs four lines** (it has three in
  `design/README.md`): p goes below the baseline, and the Romanian lines
  are dashed–solid–solid–dashed. Faint slant lines are worth trying.
- **m and n start at the middle line** with a hook; design screen 8's
  description of the first m ("its entry stroke rising from the
  baseline") should follow the model: in "mama" the first m starts with
  its hook, and the a's join starts on the m's exit.
- **A letter with an entry stroke starts on the baseline**, so "Pornește
  de pe linie." fits a, d, o, i, t, p, s, e, and A, M, N; it must not be
  said for m, n (they start at the middle line).
- The points in [`strokes-draft.json`](strokes-draft.json) are a starting
  point for the stroke files, not the content format.

## 9. The voice lines

- **The draft is [`voice-lines-draft.csv`](voice-lines-draft.csv)**: 584
  rows, 316 Romanian and 268 English: the 10 sounds and 10 names per
  language; every readable and listen-only word, its slow form (sound by
  sound, for `BuildWord`) and its syllables (every Romanian word; English
  words of two or more syllables); and 112 lines per language: greetings,
  the map, every activity's instruction, at least 4 hints per activity
  kind, 10 praise lines, the checkpoints of `TraceLetter` and
  `TraceWord`, the lesson's end, rewards, review and the session's end.
- **Columns:** `id` (`<lang>-<kind>-<key>`, stable), `language`, `kind`
  (sound, name, word, slow, syllables, line), `text`, `clip` (the id with
  `.ogg`, as AGENTS.md → "Audio" names clips), `planet` (0 for listen-only
  words, empty for lines of every planet), `used_by`, `notes`.
  `content-model` decides the final columns.
- **Spliced parts:** `{x}` and `{y}` are a letter's clip (its **sound** in
  planets 1–3, §6.7), `{word}` a word's clip, `{slow}` its slow clip,
  `{stretched}` the word with its first sound held, `{name}` the
  profile's name (nominative only, the system voice, D9). The letter
  checkpoint "Bravo, ai făcut litera {x}! Acum leag-o de litera {y}, fără
  să ridici degetul." is correct for any two letters ("litera" is
  feminine: "leag-o"); every {x}–{y} pair of the `TraceWord` words is in
  the table at the end of §6.

### 9.1 Checked against AGENTS.md → "Romanian, checked"

- No cedilla anywhere (ş ţ checked by script: 0; ș ț with the comma
  below: 44).
- Every line is a full sentence with its verb; the praise lines too ("Bravo,
  ai reușit!", never "Bravo, m!").
- Nothing agrees with the child's gender ("fără ajutor", "ai reușit", "o
  să reușești", "Ești o stea!": "stea" is a noun).
- The name only in the nominative ("Bună, {name}! Hai să zburăm!").
- Agreement with the word: "litera … Atinge-o / s-o auzi / leag-o"
  (feminine), "cuvântul … să-l auzi" (neuter); numbers: "3 stele" (no
  "de" under 20).
- Romanian quotation marks („Hai!”), no space before "!" or "?", hyphens
  ("s-o", "să-l", "leag-o", "Nu-i", "Uită-te", "Atinge-o", "Prinde-le").

### 9.2 Lines to check (the agent is not sure)

| Line | Why |
| --- | --- |
| „Linia de jos: de aici pornești.” | From the design; the first half has no verb. A full sentence would be „Pornești de pe linia de jos.” |
| „Ascultă: {x}… {x}… {word}!” | From the design ("Ascultă: mmm… mmm… muzică!"); a list after the verb, fine spoken, but check |
| „Uite răspunsul. Acum fă-o și tu.” | Is "fă-o" natural for "do it" here, or „Acum încearcă și tu.”? |
| „Acum trage liniuța literei t.” | "liniuța literei t" is correct but stiff; „Acum taie litera t cu o liniuță.”? |
| „Ești o stea!” | Fine for a girl or a boy; check it sounds natural as praise |
| „Pune mâna sub bărbie și spune cuvântul.” | A classroom trick for counting syllables, **not found in the sources read**; keep only if the human knows it |
| „Bate toba pentru fiecare silabă.” | "bate toba" or "atinge toba" (she taps a drum on screen)? |
| „constelația Unicornului / Rachetei / Balenei” | Constellation names in the genitive; check they sound right |
| „Am aterizat pe o planetă nouă!” | "am aterizat" (we) is the mascot and the child together; check |
| English: "You're a star!", "Have another go!" | British; check they suit her |

## 10. Open questions for the human

1. **English planet 1 has 7 readable words** (the brief asked for 8):
   accept, or swap t and n (§4.1)?
2. **Names later** (§6.7): letters by their sound only in planets 1–3, the
   names taught later?
3. **Familiar words** (§6.1): mami, tati, tanti, nani, papa: keep (she
   hears them at home) or only standard words?
4. **ă earlier** (§4.2), for `content-letters-2`.
5. **The handwriting drawings** (§8): do they match the manual she will
   use (or her teacher's)? Is the slant right?
6. **The lines in §9.2.**
7. **British English** for the tablet's voice and the vowels (D4)?
