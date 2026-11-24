// The world: the sky, sparkles, the rainbow path, the planets and their faces,
// the rocket, its flame and trails, the moon and the Earth, the map's stops.

import { Drawing, palette, ink, lighter, darker, sparkle, eye, happyEye, sleepyEye, cheek, label, mix } from '../lib/svg.mjs';

/** A small seeded random generator (mulberry32), so the sky is the same every time. */
function seeded(seed) {
  let a = seed >>> 0;
  return () => {
    a = (a + 0x6d2b79f5) >>> 0;
    let t = a;
    t = Math.imul(t ^ (t >>> 15), t | 1);
    t ^= t + Math.imul(t ^ (t >>> 7), t | 61);
    return ((t ^ (t >>> 14)) >>> 0) / 4294967296;
  };
}

/** The purple sky at dusk with soft star dust (2016 × 1260: 16:10, and it fits a 2048 atlas with padding). */
export function sky() {
  const w = 2016;
  const h = 1260;
  const d = new Drawing(w, h, 'sky');
  const random = seeded(20261124);
  let dust = '';
  for (let i = 0; i < 170; i += 1) {
    const x = random() * w;
    const y = random() * h;
    const r = 1.6 + random() * 2.4;
    const o = 0.35 + random() * 0.45;
    dust += `<circle cx="${x.toFixed(1)}" cy="${y.toFixed(1)}" r="${r.toFixed(2)}" fill="#FFFFFF" opacity="${o.toFixed(2)}"/>`;
  }
  for (let i = 0; i < 9; i += 1) {
    dust += sparkle(random() * w, random() * h * 0.8, 6 + random() * 6, palette.sparkle, 0.5 + random() * 0.3);
  }
  const glow =
    `<ellipse cx="${w * 0.22}" cy="${h * 0.3}" rx="${w * 0.35}" ry="${h * 0.28}" fill="${palette.purple}" opacity="0.18" filter="${d.blur(120)}"/>` +
    `<ellipse cx="${w * 0.8}" cy="${h * 0.75}" rx="${w * 0.3}" ry="${h * 0.25}" fill="${palette.pink}" opacity="0.10" filter="${d.blur(140)}"/>`;
  const body =
    `<rect width="${w}" height="${h}" fill="${d.linear([[0, palette.skyTop], [1, palette.skyBottom]])}"/>` + glow + dust;
  return d.render(body);
}

/** A four-pointed sparkle on its own, to twinkle on the sky (64 × 64). */
export function skySparkle() {
  const d = new Drawing(64, 64, 'spk');
  return d.render(
    `<circle cx="32" cy="32" r="14" fill="${palette.sparkle}" opacity="0.35" filter="${d.blur(5)}"/>` + sparkle(32, 32, 28),
  );
}

/** A small round star-dust dot for particles (32 × 32). */
export function particleDot() {
  const d = new Drawing(32, 32, 'pd');
  return d.render(`<circle cx="16" cy="16" r="9" fill="#FFFFFF" filter="${d.blur(3)}"/><circle cx="16" cy="16" r="5" fill="#FFFFFF"/>`);
}

/** A gold particle star, for celebrations (64 × 64). */
export function particleStar() {
  const d = new Drawing(64, 64, 'ps');
  return d.render(
    `<path d="M32 6 L39 24 L58 25 L43 37 L48 56 L32 45 L16 56 L21 37 L6 25 L25 24 Z" fill="${d.shade(palette.yellow, { light: 0.6, dark: 0.2 })}" stroke="${darker(palette.yellow, 0.12)}" stroke-width="3" stroke-linejoin="round"/>`,
  );
}

const rainbow = [palette.pink, palette.orange, palette.yellow, palette.green, palette.blue, palette.purple];

/** A straight piece of the rainbow path, to repeat along a curve (128 × 72: six 10-px bands at 1.2×). */
export function rainbowBand() {
  const d = new Drawing(128, 72, 'rb');
  const band = 12;
  const body = rainbow
    .map((c, i) => `<rect x="0" y="${i * band}" width="128" height="${band}" fill="${d.linear([[0, lighter(c, 0.12)], [1, c]])}"/>`)
    .join('');
  return d.render(body);
}

/** The whole rainbow wave across the map, as designed on screen 1: canvas y 300 to 700 (2032 × 635). */
export function rainbowWave() {
  const s = 2032 / 1280;
  const d = new Drawing(2032, 635, 'rw');
  const body = rainbow
    .map((c, i) => {
      const o = i * 12;
      const p = (x, y) => `${(x * s).toFixed(1)} ${((y + o - 300) * s).toFixed(1)}`;
      return `<path d="M ${p(60, 560)} C ${p(260, 380)}, ${p(420, 600)}, ${p(640, 440)} S ${p(1000, 320)}, ${p(1300, 420)}" stroke="${c}" stroke-width="${12 * s}" fill="none" stroke-linecap="round"/>`;
    })
    .join('');
  return d.render(body);
}

export const planetColours = {
  gold: { base: palette.gold, face: '#5A3A00', ring: '#FFE3A3' },
  pink: { base: palette.pink, face: '#4A0F27', ring: '#FFD1E0' },
  green: { base: palette.green, face: '#0E4A26', ring: '#C9F5D8' },
  purple: { base: palette.purple, face: '#2E1868', ring: '#D8CCFF' },
};

/** A planet's round body, lit from the top left, with soft surface marks (384 × 384). */
export function planetBody(key) {
  const { base } = planetColours[key];
  const d = new Drawing(384, 384, `pb${key}`);
  const c = 192;
  const r = 180;
  const clip = d.clip(`<circle cx="${c}" cy="${c}" r="${r}"/>`);
  const marks =
    `<g clip-path="${clip}">` +
    `<ellipse cx="${c + 70}" cy="${c - 70}" rx="34" ry="26" fill="${darker(base, 0.12)}" opacity="0.35"/>` +
    `<ellipse cx="${c - 90}" cy="${c + 70}" rx="26" ry="20" fill="${darker(base, 0.12)}" opacity="0.3"/>` +
    `<ellipse cx="${c + 96}" cy="${c + 96}" rx="18" ry="14" fill="${darker(base, 0.15)}" opacity="0.3"/>` +
    `<path d="M ${c - r} ${c + 30} Q ${c} ${c + 90} ${c + r} ${c + 10}" stroke="${lighter(base, 0.25)}" stroke-width="16" fill="none" opacity="0.35"/>` +
    `<circle cx="${c}" cy="${c}" r="${r}" fill="none" stroke="${darker(base, 0.35)}" stroke-width="18" opacity="0.25" filter="${d.blur(8)}"/>` +
    `</g>`;
  const body =
    `<circle cx="${c}" cy="${c}" r="${r}" fill="${d.shade(base)}"/>` + marks + d.gloss(c - 70, c - 82, 52, 30, 0.45);
  return d.render(body);
}

/**
 * A planet's face in one of three states, drawn on a square that covers the planet's
 * central 64% (246 × 246): put it on the planet's centre at 0.64 of the planet's size.
 */
export function planetFace(key, state) {
  const { face } = planetColours[key];
  const d = new Drawing(246, 246, `pf${key}${state}`);
  const cx = 123;
  const ey = 104;
  let body = '';
  if (state === 'beaming') {
    body += happyEye(cx - 44, ey, 22, face, 9) + happyEye(cx + 44, ey, 22, face, 9);
    body += `<path d="M ${cx - 46} ${ey + 42} Q ${cx} ${ey + 106} ${cx + 46} ${ey + 42} Z" fill="${mix(face, '#FF5C8A', 0.35)}" stroke="${face}" stroke-width="8" stroke-linejoin="round"/>`;
    body += `<path d="M ${cx - 30} ${ey + 46} Q ${cx} ${ey + 58} ${cx + 30} ${ey + 46}" stroke="#FFFFFF" stroke-width="8" fill="none" stroke-linecap="round" opacity="0.9"/>`;
    body += cheek(d, cx - 82, ey + 40, 20, 13, 0.85) + cheek(d, cx + 82, ey + 40, 20, 13, 0.85);
  } else if (state === 'awake') {
    body += eye(cx - 44, ey, 19, face) + eye(cx + 44, ey, 19, face);
    body += `<path d="M ${cx - 40} ${ey + 52} Q ${cx} ${ey + 92} ${cx + 40} ${ey + 52}" stroke="${face}" stroke-width="10" fill="none" stroke-linecap="round"/>`;
    body += cheek(d, cx - 82, ey + 44, 20, 13, 0.8) + cheek(d, cx + 82, ey + 44, 20, 13, 0.8);
  } else {
    body += sleepyEye(cx - 44, ey + 4, 20, face, 8) + sleepyEye(cx + 44, ey + 4, 20, face, 8);
    body += `<ellipse cx="${cx}" cy="${ey + 62}" rx="13" ry="10" fill="${face}" opacity="0.85"/>`;
    body += cheek(d, cx - 80, ey + 36, 18, 11, 0.6) + cheek(d, cx + 80, ey + 36, 18, 11, 0.6);
  }
  return d.render(body);
}

/**
 * The ring around the current planet, in two halves (544 × 160 each, for a 384 planet):
 * the back half goes behind the planet, the front half in front of it.
 */
export function planetRing(key, half) {
  const { ring } = planetColours[key];
  const d = new Drawing(544, 160, `pr${key}${half}`);
  const clip = half === 'back' ? d.clip('<rect x="0" y="0" width="544" height="80"/>') : d.clip('<rect x="0" y="80" width="544" height="80"/>');
  return d.render(
    `<g clip-path="${clip}">` +
      `<ellipse cx="272" cy="80" rx="252" ry="52" fill="none" stroke="${ring}" stroke-width="13" opacity="0.9"/>` +
      `<ellipse cx="272" cy="76" rx="252" ry="52" fill="none" stroke="#FFFFFF" stroke-width="4" opacity="0.45"/>` +
      `</g>`,
  );
}

/** The soft shadow under a planet (384 × 96). */
export function planetShadow() {
  const d = new Drawing(384, 96, 'psh');
  return d.render(`<ellipse cx="200" cy="48" rx="170" ry="24" fill="#000000" opacity="0.5" filter="${d.blur(10)}"/>`);
}

/** "z z z" rising from a sleepy planet (128 × 128). */
export function sleepyZ() {
  const d = new Drawing(128, 128, 'zzz');
  return d.render(
    label(24, 116, 46, 'z', palette.lines, { extra: 'opacity="0.85"' }) +
      label(66, 76, 34, 'z', palette.lines, { extra: 'opacity="0.65"' }) +
      label(100, 44, 24, 'z', palette.lines, { extra: 'opacity="0.5"' }),
  );
}

/** A stop on the path: done (a green tick), current (yellow) or ahead (faint) (64 × 64). */
export function stop(kind) {
  const d = new Drawing(64, 64, `st${kind}`);
  if (kind === 'done') {
    return d.render(
      `<circle cx="32" cy="32" r="24" fill="${d.shade('#FFFFFF', { light: 0, dark: 0.08 })}"/>` +
        `<path d="M 21 32 l 8 8 l 15 -16" stroke="${palette.green}" stroke-width="7" fill="none" stroke-linecap="round" stroke-linejoin="round"/>`,
    );
  }
  if (kind === 'current') {
    return d.render(
      `<circle cx="32" cy="32" r="29" fill="${palette.yellow}" opacity="0.35" filter="${d.blur(3)}"/>` +
        `<circle cx="32" cy="32" r="25" fill="#FFFFFF"/>` +
        `<circle cx="32" cy="32" r="19" fill="${d.shade(palette.yellow)}"/>`,
    );
  }
  return d.render(`<circle cx="32" cy="32" r="22" fill="#FFFFFF" opacity="0.35"/>`);
}

/** The rocket, upright: white body, pink nose, a blue window, purple fins (192 × 320). */
export function rocket() {
  const d = new Drawing(192, 320, 'rk');
  const bodyPath = 'M 96 14 C 150 56, 160 150, 146 236 L 46 236 C 32 150, 42 56, 96 14 Z';
  const clip = d.clip(`<path d="${bodyPath}"/>`);
  const body =
    `<path d="M 50 190 C 20 210, 12 250, 14 290 L 56 262 Z" fill="${d.shade(palette.purple)}"/>` +
    `<path d="M 142 190 C 172 210, 180 250, 178 290 L 136 262 Z" fill="${d.shade(palette.purple, { cx: '20%' })}"/>` +
    `<path d="${bodyPath}" fill="${d.shade('#F2F4FA', { light: 0.6, dark: 0.18 })}"/>` +
    `<g clip-path="${clip}">` +
    `<rect x="0" y="0" width="192" height="88" fill="${d.shade(palette.pink)}"/>` +
    `<rect x="0" y="84" width="192" height="8" fill="${darker(palette.pink, 0.2)}" opacity="0.35"/>` +
    `<rect x="0" y="212" width="192" height="24" fill="#C9D3E8"/>` +
    `<path d="M 140 30 C 160 90, 162 170, 146 236 L 128 236 C 140 170, 140 90, 124 40 Z" fill="#9FB3D6" opacity="0.35"/>` +
    `</g>` +
    `<circle cx="96" cy="140" r="32" fill="#C9D3E8"/>` +
    `<circle cx="96" cy="140" r="24" fill="${d.shade(palette.blue, { light: 0.5 })}"/>` +
    d.gloss(86, 130, 10, 6, 0.8) +
    `<path d="M 96 236 L 96 262" stroke="${palette.purple}" stroke-width="14" stroke-linecap="round"/>` +
    `<path d="M 66 236 L 126 236 L 118 252 L 74 252 Z" fill="#9FB3D6"/>` +
    d.gloss(66, 90, 10, 40, 0.5, 8);
  return d.render(body);
}

/** The rocket's flame, two frames to flicker (128 × 160; its top joins the rocket's base). */
export function rocketFlame(frame) {
  const d = new Drawing(128, 160, `fl${frame}`);
  const len = frame === 0 ? 150 : 128;
  const wob = frame === 0 ? 6 : -6;
  return d.render(
    `<path d="M 30 8 Q ${64 + wob} ${len + 10} 98 8 Z" fill="${palette.orange}" opacity="0.4" filter="${d.blur(6)}"/>` +
      `<path d="M 34 4 Q ${64 + wob} ${len} 94 4 Z" fill="${d.linear([[0, palette.yellow], [1, palette.orange]])}"/>` +
      `<path d="M 48 4 Q ${64 - wob / 2} ${len * 0.62} 80 4 Z" fill="${d.linear([[0, '#FFFFFF'], [1, palette.yellow]])}"/>`,
  );
}

/** The rocket's trails (decision: three to choose from on the map, "Dâre"): a ribbon to repeat behind it (256 × 96). */
export function trail(kind) {
  const d = new Drawing(256, 96, `tr${kind}`);
  const fade = d.linear([[0, '#FFFFFF', 0], [0.6, '#FFFFFF', 1], [1, '#FFFFFF', 1]], { x2: 1, y2: 0 });
  const maskId = d.id('m');
  d.defs.push(`<mask id="${maskId}"><rect width="256" height="96" fill="${fade}"/></mask>`);
  let body = '';
  if (kind === 'rainbow') {
    body = rainbow.map((c, i) => `<rect x="0" y="${12 + i * 12}" width="256" height="12" rx="6" fill="${c}"/>`).join('');
  } else if (kind === 'stars') {
    const random = seeded(7);
    body += `<rect x="0" y="40" width="256" height="16" rx="8" fill="${palette.sparkle}" opacity="0.45" filter="${d.blur(4)}"/>`;
    for (let i = 0; i < 9; i += 1) {
      const x = 14 + i * 28 + random() * 8;
      const y = 26 + random() * 44;
      body += i % 2 === 0 ? sparkle(x, y, 9 + random() * 6, palette.sparkle) : `<circle cx="${x}" cy="${y}" r="${3 + random() * 3}" fill="#FFFFFF"/>`;
    }
  } else {
    const random = seeded(11);
    for (let i = 0; i < 8; i += 1) {
      const x = 18 + i * 31;
      const y = 30 + random() * 36;
      const r = 8 + i * 1.6;
      body += `<circle cx="${x}" cy="${y}" r="${r}" fill="${palette.purple}" opacity="0.35" stroke="${lighter(palette.purple, 0.55)}" stroke-width="3"/>`;
      body += `<circle cx="${x - r * 0.35}" cy="${y - r * 0.35}" r="${r * 0.25}" fill="#FFFFFF" opacity="0.85"/>`;
    }
  }
  return d.render(`<g mask="url(#${maskId})">${body}</g>`);
}

/** The moon: a friendly full moon with craters and a sleepy smile (256 × 256). */
export function moon() {
  const d = new Drawing(256, 256, 'moon');
  const base = '#FFE9A8';
  const clip = d.clip('<circle cx="128" cy="128" r="112"/>');
  return d.render(
    `<circle cx="128" cy="128" r="124" fill="${palette.sparkle}" opacity="0.25" filter="${d.blur(8)}"/>` +
      `<circle cx="128" cy="128" r="112" fill="${d.shade(base, { light: 0.5, dark: 0.22 })}"/>` +
      `<g clip-path="${clip}">` +
      `<circle cx="176" cy="80" r="20" fill="${darker(base, 0.1)}" opacity="0.6"/>` +
      `<circle cx="70" cy="176" r="16" fill="${darker(base, 0.1)}" opacity="0.55"/>` +
      `<circle cx="186" cy="176" r="11" fill="${darker(base, 0.1)}" opacity="0.5"/>` +
      `<circle cx="82" cy="84" r="9" fill="${darker(base, 0.1)}" opacity="0.45"/>` +
      `</g>` +
      happyEye(98, 124, 15, '#6B4A00', 6) +
      happyEye(158, 124, 15, '#6B4A00', 6) +
      `<path d="M 110 156 Q 128 172 146 156" stroke="#6B4A00" stroke-width="6" fill="none" stroke-linecap="round"/>` +
      cheek(d, 80, 148, 14, 9) +
      cheek(d, 176, 148, 14, 9) +
      d.gloss(90, 70, 30, 16, 0.4),
  );
}

/** The Earth, home: blue seas, green lands, white clouds, a smile (256 × 256). */
export function earth() {
  const d = new Drawing(256, 256, 'earth');
  const clip = d.clip('<circle cx="128" cy="128" r="112"/>');
  return d.render(
    `<circle cx="128" cy="128" r="124" fill="${palette.blue}" opacity="0.3" filter="${d.blur(8)}"/>` +
      `<circle cx="128" cy="128" r="112" fill="${d.shade(palette.blue)}"/>` +
      `<g clip-path="${clip}">` +
      `<path d="M 40 70 C 70 40, 110 50, 118 78 C 126 104, 96 112, 84 136 C 72 160, 40 150, 30 120 Z" fill="${d.shade(palette.green)}"/>` +
      `<path d="M 150 40 C 186 36, 220 60, 226 96 C 200 104, 186 90, 168 96 C 150 100, 140 70, 150 40 Z" fill="${d.shade(palette.green)}"/>` +
      `<path d="M 140 170 C 170 150, 214 160, 222 190 C 210 226, 170 236, 150 222 C 130 206, 120 184, 140 170 Z" fill="${d.shade(palette.green)}"/>` +
      `<path d="M 20 196 Q 60 180 100 200 Q 70 212 30 206 Z" fill="#FFFFFF" opacity="0.85"/>` +
      `<path d="M 170 120 Q 210 108 246 126 Q 214 136 176 130 Z" fill="#FFFFFF" opacity="0.85"/>` +
      `</g>` +
      eye(100, 126, 11, '#0C2F4D') +
      eye(156, 126, 11, '#0C2F4D') +
      `<path d="M 110 156 Q 128 174 146 156" stroke="#0C2F4D" stroke-width="6" fill="none" stroke-linecap="round"/>` +
      cheek(d, 80, 150, 13, 8) +
      cheek(d, 176, 150, 13, 8) +
      d.gloss(88, 70, 32, 16, 0.4),
  );
}

/** Every world sprite, by its path under Art/. */
export function sprites() {
  const list = [
    { path: 'Sky/sky', width: 2016, height: 1260, svg: sky() },
    { path: 'Sky/sky-sparkle', width: 64, height: 64, svg: skySparkle() },
    { path: 'Effects/particle-dot', width: 32, height: 32, svg: particleDot() },
    { path: 'Effects/particle-star', width: 64, height: 64, svg: particleStar() },
    { path: 'Map/rainbow-band', width: 128, height: 72, svg: rainbowBand() },
    { path: 'Map/rainbow-wave', width: 2032, height: 635, svg: rainbowWave() },
    { path: 'Map/planet-shadow', width: 384, height: 96, svg: planetShadow() },
    { path: 'Map/planet-sleepy-z', width: 128, height: 128, svg: sleepyZ() },
    { path: 'Map/stop-done', width: 64, height: 64, svg: stop('done') },
    { path: 'Map/stop-current', width: 64, height: 64, svg: stop('current') },
    { path: 'Map/stop-ahead', width: 64, height: 64, svg: stop('ahead') },
    { path: 'Map/rocket', width: 192, height: 320, svg: rocket() },
    { path: 'Map/rocket-flame-0', width: 128, height: 160, svg: rocketFlame(0) },
    { path: 'Map/rocket-flame-1', width: 128, height: 160, svg: rocketFlame(1) },
    { path: 'Map/trail-rainbow', width: 256, height: 96, svg: trail('rainbow') },
    { path: 'Map/trail-stars', width: 256, height: 96, svg: trail('stars') },
    { path: 'Map/trail-bubbles', width: 256, height: 96, svg: trail('bubbles') },
    { path: 'Map/moon', width: 256, height: 256, svg: moon() },
    { path: 'Map/earth', width: 256, height: 256, svg: earth() },
  ];
  for (const key of Object.keys(planetColours)) {
    list.push({ path: `Map/planet-${key}`, width: 384, height: 384, svg: planetBody(key) });
    list.push({ path: `Map/planet-${key}-ring-back`, width: 544, height: 160, svg: planetRing(key, 'back') });
    list.push({ path: `Map/planet-${key}-ring-front`, width: 544, height: 160, svg: planetRing(key, 'front') });
    for (const state of ['beaming', 'awake', 'sleepy']) {
      list.push({ path: `Map/planet-${key}-face-${state}`, width: 246, height: 246, svg: planetFace(key, state) });
    }
  }
  return list;
}
