// Writes the choosing page: the guide's takes, the badges' takes, the planets, the avatars and
// the stickers, as one offline HTML page for the human and her to look at together.
//
//   node design/art/choose.mjs <folder> [--inbox <list.json>]
//
// Writes <folder>/choose.html (keep it in local/). The inbox list names sheets made with an image
// tool and how to cut them (cut-sheet.mjs's options, plus "section" and "label"); their parts are
// shown as takes of their own beside the agent's.

import { existsSync, readFileSync } from 'node:fs';
import { dirname, join, resolve } from 'node:path';
import { fileURLToPath, pathToFileURL } from 'node:url';
import { readFont, renderAll, writeFile } from './lib/render.mjs';
import { unicornTakes, unicorn, catFrame } from './draw/guides.mjs';
import { badgeTakes, romanian, english } from './draw/badges.mjs';
import { planetBody, planetFace, planetColours, rocket, rocketFlame, moon, earth, sky } from './draw/world.mjs';
import { cutSheet } from './cut-sheet.mjs';
import { avatarIds, avatar } from './draw/avatars.mjs';
import { iconAdaptiveBackground, iconAdaptiveForeground } from './draw/ui.mjs';

const here = dirname(fileURLToPath(import.meta.url));
const repo = resolve(here, '..', '..');
const defaultInbox = 'C:\\Projects\\Scolita\\local\\art-inbox';

/** Cuts every sheet in the inbox list; returns the parts by page section, and the inbox folder. */
function cutInbox(listPath) {
  const bySection = {};
  if (!listPath) return { bySection, inbox: null };
  const list = JSON.parse(readFileSync(listPath, 'utf8'));
  for (const sheet of list.sheets) {
    const path = join(list.inbox, sheet.file);
    if (!existsSync(path)) {
      console.warn(`Skipped ${sheet.file}: not in the inbox.`);
      continue;
    }
    const parts = cutSheet(path, {
      key: sheet.key ?? 'green',
      size: sheet.size ?? (sheet.background ? 1280 : 384),
      align: sheet.align ?? 'center',
      grid: sheet.grid ? sheet.grid.split('x').map(Number) : null,
      names: sheet.names,
      background: sheet.background ?? false,
    });
    (bySection[sheet.section] ??= []).push({ label: sheet.label, note: sheet.note ?? '', background: sheet.background ?? false, parts: [...parts.entries()] });
  }
  return { bySection, inbox: list.inbox };
}

const moods = [
  ['happy-idle', 'happy'],
  ['happy-talk', 'talking'],
  ['cheering-0', 'cheering'],
  ['thinking-idle', 'thinking (a hint)'],
  ['sleepy-0', 'sleepy (the rocket rests)'],
];

async function main() {
  const out = resolve(process.argv[2] ?? '.');
  const inboxArg = process.argv.indexOf('--inbox');
  const { bySection, inbox } = cutInbox(inboxArg > 0 ? resolve(process.argv[inboxArg + 1]) : null);
  const jobs = [];
  const add = (name, svg, size = 384) => jobs.push({ name, width: size, height: size, svg });
  for (const take of Object.keys(unicornTakes)) for (const [f] of moods) add(`u-${take}-${f}`, unicorn(take, f));
  for (const [f] of moods) add(`cat-${f}`, catFrame(f));
  for (const take of Object.keys(badgeTakes)) {
    add(`ro-${take}`, romanian(take), 128);
    add(`en-${take}`, english(take), 128);
    add(`ro-${take}-grey`, romanian(take, true), 128);
    add(`en-${take}-grey`, english(take, true), 128);
  }
  for (const key of Object.keys(planetColours)) {
    add(`planet-${key}`, planetBody(key));
    for (const s of ['beaming', 'awake', 'sleepy']) add(`face-${key}-${s}`, planetFace(key, s), 246);
  }
  for (const id of avatarIds) add(`avatar-${id}`, avatar(id), 320);
  add('rocket', rocket(), 192);
  jobs[jobs.length - 1].height = 320;
  add('flame', rocketFlame(0), 128);
  jobs[jobs.length - 1].height = 160;
  add('moon', moon(), 256);
  add('earth', earth(), 256);
  add('icon-bg', iconAdaptiveBackground(), 432);
  add('icon-fg', iconAdaptiveForeground(), 432);
  add('sky', sky(), 1008);
  jobs[jobs.length - 1].height = 630;
  let stickerIds = [];
  const stickersModule = join(here, 'draw', 'stickers.mjs');
  if (existsSync(stickersModule)) {
    const stickers = await import(pathToFileURL(stickersModule).href);
    for (const s of stickers.sprites()) {
      add(s.path, s.svg, s.width);
      jobs[jobs.length - 1].height = s.height;
      stickerIds.push(s.path);
    }
  }
  const png = renderAll(jobs, readFont(join(repo, 'game', 'Assets', 'ThirdParty', 'Baloo2', 'Baloo2-Variable.ttf')));
  const src = (name) => `data:image/png;base64,${png.get(name).toString('base64')}`;
  const im = (name, w, extra = '') => `<img src="${src(name)}" width="${w}" ${extra}>`;
  const raw = (bytes, w, extra = '') => `<img src="data:image/png;base64,${bytes.toString('base64')}" width="${w}" ${extra}>`;
  const drafts = (section, width = 150, backgroundWidth = 520) =>
    (bySection[section] ?? [])
      .map(
        (sheet) =>
          `<div class="take draft"><h3>${sheet.label}</h3>${sheet.note ? `<p class="note">${sheet.note}</p>` : ''}<div class="row">` +
          sheet.parts.map(([caption, bytes]) => `<figure>${raw(bytes, sheet.background ? backgroundWidth : width)}<figcaption>${caption}</figcaption></figure>`).join('') +
          `</div></div>`,
      )
      .join('');

  let guide = '';
  for (const [take, text] of Object.entries(unicornTakes)) {
    guide += `<div class="take"><h3>${text}</h3><div class="row">`;
    for (const [f, cap] of moods) guide += `<figure>${im(`u-${take}-${f}`, 150)}<figcaption>${cap}</figcaption></figure>`;
    guide += `<figure class="pair">${im(`u-${take}-happy-idle`, 150)}${im('cat-happy-idle', 96, 'class="copilot"')}<figcaption>with the cat beside</figcaption></figure>`;
    guide += `</div></div>`;
  }
  guide += drafts('guide');
  let cat = `<div class="take"><h3>Take A–C (agent): her purple cat, sitting, no helmet</h3><div class="row">`;
  for (const [f, cap] of moods) cat += `<figure>${im(`cat-${f}`, 150)}<figcaption>${cap}</figcaption></figure>`;
  cat += `</div></div>` + drafts('cat');

  let badges = '';
  for (const [take, text] of Object.entries(badgeTakes)) {
    badges += `<div class="take"><h3>${text}</h3><div class="row cream">`;
    badges += `<figure>${im(`ro-${take}`, 96)}<figcaption>Romanian</figcaption></figure><figure>${im(`en-${take}`, 96)}<figcaption>English</figcaption></figure>`;
    badges += `<figure>${im(`ro-${take}-grey`, 96)}<figcaption>in grey</figcaption></figure><figure>${im(`en-${take}-grey`, 96)}<figcaption>in grey</figcaption></figure>`;
    badges += `<figure>${im(`ro-${take}`, 40)}${im(`en-${take}`, 40)}<figcaption>bubble size</figcaption></figure>`;
    badges += `</div></div>`;
  }

  badges += drafts('badges', 96);
  let planets = '<div class="row">';
  for (const key of Object.keys(planetColours)) {
    for (const s of ['beaming', 'awake', 'sleepy']) {
      planets += `<figure class="planet${s === 'sleepy' ? ' sleepy' : ''}"><span>${im(`planet-${key}`, 120)}${im(`face-${key}-${s}`, 77, 'class="face"')}</span><figcaption>${key}, ${s}</figcaption></figure>`;
    }
  }
  planets += '</div>' + drafts('planets', 120);
  const rocketHtml =
    `<div class="take"><h3>Agent: the rocket and its flame</h3><div class="row"><figure><span class="rocket">${im('rocket', 96)}${im('flame', 64, 'class="flame"')}</span><figcaption>flying</figcaption></figure></div></div>` +
    drafts('rocket');
  const moonEarth =
    `<div class="take"><h3>Agent: the moon and the Earth</h3><div class="row"><figure>${im('moon', 150)}<figcaption>moon</figcaption></figure><figure>${im('earth', 150)}<figcaption>Earth</figcaption></figure></div></div>` +
    drafts('moon-earth');
  const iconHtml =
    `<div class="take"><h3>Agent: the app icon (Android draws it in a circle or a rounded square)</h3><div class="row"><figure><span class="icon">${im('icon-bg', 160)}${im('icon-fg', 160, 'class="fg"')}</span><figcaption>adaptive icon</figcaption></figure></div></div>` +
    drafts('icon', 160, 160);
  const words = drafts('words', 110);
  const ideas = drafts('ideas', 130, 300);
  const skyHtml =
    `<div class="take"><h3>Agent: the sky</h3><div class="row"><figure>${im('sky', 520)}<figcaption>sky</figcaption></figure></div></div>` + drafts('sky');

  let avatars = '<div class="row cream">';
  for (const id of avatarIds) avatars += `<figure>${im(`avatar-${id}`, 110)}<figcaption>${id}</figcaption></figure>`;
  avatars += '</div>' + drafts('avatars', 110);

  let stickers = '';
  if (stickerIds.length) {
    stickers = '<section><h2>5. Stickers she earns (30)</h2><p>Drawn by the agent, in four groups: animals, instruments, planets, space things. Any she dislikes can be swapped (her own drawings too).</p><div class="row">';
    for (const id of stickerIds) stickers += `<figure>${im(id, 110)}<figcaption>${id.split('/').pop().replace('sticker-', '').replace('constellation-', '✦ ')}</figcaption></figure>`;
    stickers += '</div>' + drafts('stickers', 110) + '</section>';
  }

  const draftCount = Object.values(bySection).reduce((n, sheets) => n + sheets.length, 0);
  const inboxNote = draftCount
    ? `${draftCount} ChatGPT sheets from the inbox are shown below as takes of their own ("ChatGPT" or "Take D"), cut out on the purple sky; they are as good a choice as the agent's.`
    : 'Nothing there yet.';
  const inboxPath = inbox ?? defaultInbox;

  const html = `<!doctype html><html lang="en"><head><meta charset="utf-8"><title>Școlița: choose the look</title>
<style>
body{margin:0;font-family:system-ui,sans-serif;background:linear-gradient(#2B1B5A,#4B2C8F);color:#fff}
main{max-width:1100px;margin:auto;padding:24px}
h1{font-size:34px;margin:8px 0}h2{margin-top:40px;color:#FFD93D}h3{font-size:17px;font-weight:600;margin:6px 0}
section{background:rgba(255,255,255,.06);border-radius:24px;padding:16px 20px;margin:20px 0}
.row{display:flex;flex-wrap:wrap;gap:10px;align-items:flex-end}.cream{background:#FFF8EC;border-radius:16px;padding:10px;color:#2A2140}
figure{margin:0;text-align:center}figcaption{font-size:13px;opacity:.85}
.pair{position:relative}.pair .copilot{position:absolute;right:-40px;bottom:22px}.pair{margin-right:40px}
.planet span{position:relative;display:inline-block;width:120px;height:120px}.planet .face{position:absolute;left:21.5px;top:21.5px}.sleepy span{opacity:.55}
.draft h3{color:#BFE3FF}.note{font-size:14px;color:#FFE3A3;margin:2px 0 6px}.icon{position:relative;display:inline-block;width:160px;height:160px;border-radius:36px;overflow:hidden}.icon .fg{position:absolute;left:0;top:0}.rocket{position:relative;display:inline-block;width:96px;height:200px}.rocket .flame{position:absolute;left:16px;top:150px;z-index:-1}
.ask{background:#FFF8EC;color:#2A2140;border-radius:20px;padding:14px 20px}
</style></head><body><main>
<h1>Școlița: choose the look with her</h1>
<p>The agent <b>look</b>'s drawings and your ChatGPT drafts, side by side, cut out on the purple sky. Nothing is final until you two choose;
the app shows the agent's first take of each until then. One thing to weigh: ChatGPT's pictures are glossier and more painted than the agent's,
so choosing ChatGPT's for some things and the agent's for others mixes two styles (the agent can bring them closer once you've chosen).</p>
<div class="ask"><b>What to choose</b> (tell the lead, or write it in <code>local/look/answers.txt</code>):
<ol><li><b>The guide:</b> take A, B, C (the agent's) or D (ChatGPT), and whether the cat sits beside it on screen.</li>
<li><b>The cat:</b> the agent's (sitting) or ChatGPT's (in a helmet).</li>
<li><b>The badges:</b> A, B (the agent's) or C (ChatGPT's shapes, with the app's letters).</li>
<li><b>The rocket, the planets, the moon and the Earth, the sky, the app icon:</b> the agent's or ChatGPT's, for each.</li>
<li><b>The avatars and the stickers:</b> the agent's or ChatGPT's (or a mix: say which).</li>
<li><b>Anything she doesn't like</b> among the planets, avatars or stickers (her words are best).</li>
<li><b>Her name for the unicorn</b> (and the cat), whenever she has one.</li></ol>
ChatGPT drafts go in <code>${inboxPath}</code> (a <code>notes.txt</code> with what she liked helps). ${inboxNote}</div>
<section><h2>1. The guide: the unicorn astronaut (D5)</h2>${guide}</section>
<section><h2>1b. The co-pilot: her purple cat</h2>${cat}</section>
<section><h2>2. The language badges (D6)</h2><p>Both told apart by shape alone (see the grey ones).</p>${badges}</section>
<section><h2>3. The planets and their faces</h2><p>Beaming: finished. Awake: today's planet. Sleepy: still ahead.</p>${planets}</section>
<section><h2>4. The 14 profile avatars (D9)</h2><p>As drawn on design screen 11, with soft shading added. The moon's crescent is now filled in.</p>${avatars}</section>
${stickers}
<section><h2>6. The rocket</h2>${rocketHtml}</section>
<section><h2>7. The moon and the Earth</h2>${moonEarth}</section>
<section><h2>8. The sky</h2>${skyHtml}</section>
<section><h2>9. The app icon</h2>${iconHtml}</section>
${words ? `<section><h2>10. Word pictures (for <code>content-letters-1</code>, not chosen here)</h2><p>ChatGPT's pictures for planets 1–3's words, checked against each word. The agent of <code>content-letters-1</code> uses them; notes say which may confuse her.</p>${words}</section>` : ''}
${ideas ? `<section><h2>Ideas for later (not a choice now)</h2><p>From the surprise rounds: more guides, later planets and constellations. Notes say what each would need before it can be used.</p>${ideas}</section>` : ''}
</main></body></html>`;
  writeFile(join(out, 'choose.html'), Buffer.from(html, 'utf8'));
  console.log(`Wrote ${join(out, 'choose.html')}.`);
}

main().catch((e) => {
  console.error(e.message);
  process.exit(1);
});
