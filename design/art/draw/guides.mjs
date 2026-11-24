// The guides: the unicorn astronaut (decision D5) in three takes to choose from, and the
// co-pilot, her purple cat with black spots and a pink scarf. Each guide has four moods
// (happy, cheering, thinking, sleepy), a few frames each, so a guide is art only.

import { Drawing, palette, ink, lighter, darker, sparkle, cheek, label, roundStar } from '../lib/svg.mjs';

/** The frames every guide has, in the order the catalog lists them. */
export const frames = [
  { id: 'happy-idle', mood: 'happy', eyes: 'open', mouth: 'smile', arms: 'down' },
  { id: 'happy-blink', mood: 'happy', eyes: 'blink', mouth: 'smile', arms: 'down' },
  { id: 'happy-talk', mood: 'happy', eyes: 'open', mouth: 'talk', arms: 'wave' },
  { id: 'cheering-0', mood: 'cheering', eyes: 'happy', mouth: 'open-smile', arms: 'up', sparkles: 0 },
  { id: 'cheering-1', mood: 'cheering', eyes: 'happy', mouth: 'open-smile', arms: 'up-high', sparkles: 1 },
  { id: 'thinking-idle', mood: 'thinking', eyes: 'up', mouth: 'small', arms: 'chin', bubble: true },
  { id: 'thinking-talk', mood: 'thinking', eyes: 'up', mouth: 'talk', arms: 'chin', bubble: true },
  { id: 'sleepy-0', mood: 'sleepy', eyes: 'sleepy', mouth: 'small', arms: 'down', z: 0 },
  { id: 'sleepy-1', mood: 'sleepy', eyes: 'sleepy', mouth: 'yawn', arms: 'down', z: 1 },
];

export const unicornTakes = {
  helmet: 'A: the helmet (the design canvas): the head in a round glass helmet, a collar',
  suit: 'B: the whole astronaut: the helmet on a little white and lilac spacesuit',
  round: 'C: the round one: a chubby round suit with the face in its window',
};

const face = '#FFF8F2';
const snoutPink = '#FFE3EC';
const suitWhite = '#F4F1FB';
const suitLilac = '#C9B6FF';
const collar = '#C9D3E8';

function eyes(f, lx, rx, y, r) {
  const highlight = (x, yy) =>
    `<circle cx="${x + r * 0.38}" cy="${yy - r * 0.38}" r="${r * 0.4}" fill="#FFFFFF"/>` +
    `<circle cx="${x - r * 0.3}" cy="${yy + r * 0.42}" r="${r * 0.16}" fill="#FFFFFF" opacity="0.8"/>`;
  if (f.eyes === 'open') {
    return [lx, rx].map((x) => `<ellipse cx="${x}" cy="${y}" rx="${r}" ry="${r * 1.12}" fill="${ink}"/>` + highlight(x, y)).join('');
  }
  if (f.eyes === 'up') {
    return [lx, rx]
      .map((x) => `<ellipse cx="${x + r * 0.25}" cy="${y - r * 0.3}" rx="${r}" ry="${r * 1.12}" fill="${ink}"/>` + highlight(x + r * 0.45, y - r * 0.55))
      .join('');
  }
  if (f.eyes === 'blink') {
    return [lx, rx].map((x) => `<path d="M ${x - r} ${y + r * 0.2} Q ${x} ${y + r * 0.9} ${x + r} ${y + r * 0.2}" stroke="${ink}" stroke-width="${r * 0.5}" fill="none" stroke-linecap="round"/>`).join('');
  }
  if (f.eyes === 'happy') {
    return [lx, rx].map((x) => `<path d="M ${x - r * 1.1} ${y + r * 0.4} Q ${x} ${y - r * 1.1} ${x + r * 1.1} ${y + r * 0.4}" stroke="${ink}" stroke-width="${r * 0.5}" fill="none" stroke-linecap="round"/>`).join('');
  }
  return [lx, rx].map((x) => `<path d="M ${x - r} ${y} Q ${x} ${y + r * 0.8} ${x + r} ${y}" stroke="${ink}" stroke-width="${r * 0.45}" fill="none" stroke-linecap="round"/>`).join('');
}

function mouth(f, x, y, w) {
  if (f.mouth === 'smile') {
    return `<path d="M ${x - w} ${y} Q ${x} ${y + w * 0.9} ${x + w} ${y}" stroke="${ink}" stroke-width="${w * 0.32}" fill="none" stroke-linecap="round"/>`;
  }
  if (f.mouth === 'talk') {
    return (
      `<ellipse cx="${x}" cy="${y + w * 0.3}" rx="${w * 0.62}" ry="${w * 0.55}" fill="#7A2848"/>` +
      `<ellipse cx="${x}" cy="${y + w * 0.6}" rx="${w * 0.38}" ry="${w * 0.22}" fill="#FF8FB0"/>`
    );
  }
  if (f.mouth === 'open-smile') {
    return (
      `<path d="M ${x - w * 1.1} ${y - w * 0.1} Q ${x} ${y + w * 1.6} ${x + w * 1.1} ${y - w * 0.1} Z" fill="#7A2848" stroke="${ink}" stroke-width="${w * 0.2}" stroke-linejoin="round"/>` +
      `<ellipse cx="${x}" cy="${y + w * 0.75}" rx="${w * 0.5}" ry="${w * 0.25}" fill="#FF8FB0"/>`
    );
  }
  if (f.mouth === 'yawn') {
    return `<ellipse cx="${x}" cy="${y + w * 0.35}" rx="${w * 0.45}" ry="${w * 0.6}" fill="#7A2848"/>`;
  }
  return `<path d="M ${x - w * 0.45} ${y + w * 0.1} Q ${x} ${y + w * 0.5} ${x + w * 0.45} ${y + w * 0.1}" stroke="${ink}" stroke-width="${w * 0.3}" fill="none" stroke-linecap="round"/>`;
}

/** The unicorn's head, drawn in the design canvas's 120 × 120 space (helmet glass not included). */
function unicornHead(d, f) {
  const strand = (path, colour, width) =>
    `<path d="${path}" stroke="${darker(colour, 0.12)}" stroke-width="${width + 1.4}" fill="none" stroke-linecap="round"/>` +
    `<path d="${path}" stroke="${colour}" stroke-width="${width}" fill="none" stroke-linecap="round"/>` +
    `<path d="${path}" stroke="${lighter(colour, 0.5)}" stroke-width="${width * 0.3}" fill="none" stroke-linecap="round" opacity="0.7" transform="translate(-1 -1)"/>`;
  return (
    `<path d="M 35 42 L 39 21 L 51 35 Z" fill="${d.shade(face, { light: 0.4, dark: 0.12 })}" stroke="${darker(face, 0.12)}" stroke-width="1.2" stroke-linejoin="round"/>` +
    `<path d="M 85 42 L 81 21 L 69 35 Z" fill="${d.shade(face, { light: 0.4, dark: 0.12 })}" stroke="${darker(face, 0.12)}" stroke-width="1.2" stroke-linejoin="round"/>` +
    `<path d="M 39 38 L 41 27 L 47 35 Z" fill="#FFB3D1"/><path d="M 81 38 L 79 27 L 73 35 Z" fill="#FFB3D1"/>` +
    strand('M 33 52 C 21 62, 23 80, 33 92', palette.pink, 6.5) +
    strand('M 39 44 C 25 56, 27 78, 37 96', palette.yellow, 6.5) +
    strand('M 46 38 C 31 50, 33 74, 43 98', palette.blue, 6.5) +
    `<circle cx="60" cy="62" r="30" fill="${d.shade(face, { light: 0.6, dark: 0.1 })}" stroke="${darker(face, 0.1)}" stroke-width="1"/>` +
    strand('M 52 36 C 46 40, 44 46, 47 52', palette.purple, 5.5) +
    strand('M 58 34 C 52 38, 52 44, 55 48', palette.pink, 5) +
    `<path d="M 60 6 L 52.5 33 L 67.5 33 Z" fill="${d.shade(palette.yellow, { light: 0.5, dark: 0.2 })}" stroke="#E0A800" stroke-width="1.4" stroke-linejoin="round"/>` +
    `<path d="M 55.5 24 L 64.5 21 M 54 29 L 66 26 M 57 18 L 62.5 16" stroke="#E0A800" stroke-width="1.5" stroke-linecap="round"/>` +
    `<ellipse cx="62" cy="79" rx="18" ry="12" fill="${d.shade(snoutPink, { light: 0.5, dark: 0.12 })}"/>` +
    `<ellipse cx="56" cy="76.5" rx="2.4" ry="1.8" fill="#E58BA8"/><ellipse cx="68" cy="76.5" rx="2.4" ry="1.8" fill="#E58BA8"/>` +
    mouth(f, 62, 83, 5) +
    eyes(f, 51, 72, 59, 5) +
    cheek(d, 43, 70, 5, 3) +
    cheek(d, 81, 70, 5, 3)
  );
}

/** The round glass helmet over a head in the 120 space: glass, rim and reflection. */
function helmetGlass(d) {
  return (
    `<circle cx="60" cy="62" r="50" fill="${d.radial([[0, '#FFFFFF', 0.05], [0.75, '#DDF1FF', 0.25], [1, '#BFE3FF', 0.55]], { cx: '40%', cy: '35%', r: '60%' })}"/>` +
    `<circle cx="60" cy="62" r="50" fill="none" stroke="#FFFFFF" stroke-width="3.2" opacity="0.95"/>` +
    `<circle cx="60" cy="62" r="47" fill="none" stroke="#BFE3FF" stroke-width="1.4" opacity="0.6"/>` +
    `<path d="M 26 40 Q 34 22 54 16" stroke="#FFFFFF" stroke-width="4.5" fill="none" stroke-linecap="round" opacity="0.85"/>` +
    `<path d="M 22 54 Q 22 50 24 46" stroke="#FFFFFF" stroke-width="3" fill="none" stroke-linecap="round" opacity="0.6"/>` +
    `<ellipse cx="60" cy="12.5" rx="9" ry="3.2" fill="${d.shade(collar, { light: 0.4 })}" stroke="${darker(collar, 0.2)}" stroke-width="1"/>`
  );
}

/** The helmet's collar with a purple star, in the 120 space. */
function helmetCollar(d) {
  return (
    `<path d="M 22 100 Q 60 118 98 100 L 94 112 Q 60 126 26 112 Z" fill="${d.shade(collar, { light: 0.45, dark: 0.18 })}" stroke="${darker(collar, 0.22)}" stroke-width="1.2" stroke-linejoin="round"/>` +
    roundStar(60, 113, 5.5, palette.purple, { strokeWidth: 1.4 })
  );
}

/** A white glove (a round hoof in a glove) at (x, y), radius r. */
function glove(d, x, y, r) {
  return `<ellipse cx="${x}" cy="${y}" rx="${r}" ry="${r * 0.9}" fill="${d.shade(suitWhite, { light: 0.5, dark: 0.16 })}" stroke="${darker(suitWhite, 0.2)}" stroke-width="${r * 0.12}"/>`;
}

/** A thought bubble with three dots, top right. */
function thoughtBubble(d, x, y, s) {
  return (
    `<circle cx="${x - s * 1.4}" cy="${y + s * 1.5}" r="${s * 0.22}" fill="#FFFFFF" opacity="0.95"/>` +
    `<circle cx="${x - s * 0.9}" cy="${y + s * 0.95}" r="${s * 0.34}" fill="#FFFFFF" opacity="0.95"/>` +
    `<ellipse cx="${x}" cy="${y}" rx="${s}" ry="${s * 0.72}" fill="${d.shade(palette.cream, { light: 0.3, dark: 0.08 })}"/>` +
    [-0.45, 0, 0.45].map((k) => `<circle cx="${x + s * k}" cy="${y}" r="${s * 0.12}" fill="${palette.purple}"/>`).join('')
  );
}

/** The mood's extras: sparkles for cheering, z z z for sleepy, the bubble for thinking. */
function extras(d, f, box) {
  let s = '';
  if (f.mood === 'cheering') {
    const a = f.sparkles === 0 ? [[0.86, 0.18, 0.07], [0.12, 0.3, 0.05]] : [[0.9, 0.3, 0.05], [0.1, 0.16, 0.07], [0.8, 0.08, 0.04]];
    for (const [x, y, r] of a) s += sparkle(box * x, box * y, box * r, palette.yellow);
  }
  if (f.mood === 'sleepy') {
    const up = f.z === 0 ? 0 : -box * 0.04;
    s += label(box * 0.78, box * 0.26 + up, box * 0.1, 'z', palette.lines, { extra: 'opacity="0.9"' });
    s += label(box * 0.87, box * 0.15 + up, box * 0.075, 'z', palette.lines, { extra: 'opacity="0.7"' });
  }
  if (f.bubble) s += thoughtBubble(d, box * 0.84, box * 0.14, box * 0.1);
  return s;
}

/** Take A: the head in its helmet with the collar (as on the design canvas). */
function unicornHelmet(f, size = 384) {
  const d = new Drawing(size, size, `uA${f.id}`);
  const k = size / 132;
  let arms = '';
  if (f.arms === 'chin') arms = glove(d, 44, 98, 9);
  if (f.arms === 'up' || f.arms === 'up-high') {
    const y = f.arms === 'up' ? 92 : 84;
    arms = glove(d, 12, y, 8.5) + glove(d, 108, y, 8.5);
  }
  if (f.arms === 'wave') arms = glove(d, 109, 88, 8.5);
  const body =
    `<g transform="translate(${6 * k} ${4 * k}) scale(${k})">` +
    d.groundShadow(62, 124, 38, 4, 0.18) +
    helmetCollar(d) +
    unicornHead(d, f) +
    helmetGlass(d) +
    `<path d="M 60 4 L 54.5 14 L 65.5 14 Z" fill="${d.shade(palette.yellow, { light: 0.5 })}" stroke="#E0A800" stroke-width="1.2" stroke-linejoin="round"/>` +
    arms +
    `</g>` +
    extras(d, f, size);
  return d.render(body);
}

/** The spacesuit body for take B, in a 120 × 150 space under a 120 head; arms by pose. */
function suitBody(d, f) {
  const arm = (x1, y1, x2, y2) =>
    `<path d="M ${x1} ${y1} L ${x2} ${y2}" stroke="${darker(suitWhite, 0.2)}" stroke-width="17" stroke-linecap="round"/>` +
    `<path d="M ${x1} ${y1} L ${x2} ${y2}" stroke="${suitWhite}" stroke-width="14" stroke-linecap="round"/>` +
    glove(d, x2, y2, 9);
  let arms;
  if (f.arms === 'up') arms = arm(36, 120, 18, 98) + arm(84, 120, 102, 98);
  else if (f.arms === 'up-high') arms = arm(36, 120, 22, 88) + arm(84, 120, 98, 88);
  else if (f.arms === 'chin') arms = arm(36, 122, 24, 146) + arm(84, 122, 70, 104);
  else if (f.arms === 'wave') arms = arm(36, 122, 24, 146) + arm(84, 120, 104, 100);
  else arms = arm(36, 122, 26, 148) + arm(84, 122, 94, 148);
  return (
    `<path d="M 20 132 C 2 128, 0 150, 14 156" stroke="${palette.pink}" stroke-width="7" fill="none" stroke-linecap="round"/>` +
    `<path d="M 22 138 C 8 138, 8 156, 20 160" stroke="${palette.purple}" stroke-width="7" fill="none" stroke-linecap="round"/>` +
    `<rect x="30" y="108" width="60" height="44" rx="14" fill="${d.shade(suitLilac, { dark: 0.2 })}"/>` +
    `<path d="M 34 110 C 30 130, 32 150, 40 160 L 80 160 C 88 150, 90 130, 86 110 Z" fill="${d.shade(suitWhite, { light: 0.5, dark: 0.16 })}" stroke="${darker(suitWhite, 0.18)}" stroke-width="1.4"/>` +
    `<rect x="48" y="124" width="24" height="18" rx="6" fill="${d.shade(suitLilac)}"/>` +
    roundStar(60, 133, 6, palette.yellow, { strokeWidth: 1.6 }) +
    `<rect x="38" y="156" width="18" height="12" rx="6" fill="${d.shade(palette.purple)}"/>` +
    `<rect x="64" y="156" width="18" height="12" rx="6" fill="${d.shade(palette.purple)}"/>` +
    arms
  );
}

/** Take B: the whole astronaut, the helmet on a little suit. */
function unicornSuit(f, size = 384) {
  const d = new Drawing(size, size, `uB${f.id}`);
  const k = size / 178;
  const body =
    `<g transform="translate(${28 * k} ${2 * k}) scale(${k})">` +
    d.groundShadow(62, 170, 34, 4, 0.18) +
    suitBody(d, f) +
    `<g transform="translate(6 2) scale(0.9)">` +
    helmetCollar(d) +
    unicornHead(d, f) +
    helmetGlass(d) +
    `<path d="M 60 4 L 54.5 14 L 65.5 14 Z" fill="${d.shade(palette.yellow, { light: 0.5 })}" stroke="#E0A800" stroke-width="1.2" stroke-linejoin="round"/>` +
    `</g></g>` +
    extras(d, f, size);
  return d.render(body);
}

/** Take C: the round one, a chubby round suit with the face in its big window. */
function unicornRound(f, size = 384) {
  const d = new Drawing(size, size, `uC${f.id}`);
  const k = size / 140;
  const hand = (x, y) => glove(d, x, y, 9);
  let arms;
  if (f.arms === 'up') arms = hand(10, 64) + hand(130, 64);
  else if (f.arms === 'up-high') arms = hand(14, 50) + hand(126, 50);
  else if (f.arms === 'chin') arms = hand(10, 104) + hand(54, 108);
  else if (f.arms === 'wave') arms = hand(10, 104) + hand(130, 66);
  else arms = hand(12, 104) + hand(128, 104);
  const body =
    `<g transform="translate(0 ${2 * k}) scale(${k})">` +
    d.groundShadow(72, 134, 40, 4, 0.18) +
    `<ellipse cx="52" cy="128" rx="13" ry="8" fill="${d.shade(palette.purple)}"/>` +
    `<ellipse cx="90" cy="128" rx="13" ry="8" fill="${d.shade(palette.purple)}"/>` +
    `<circle cx="70" cy="78" r="56" fill="${d.shade(suitWhite, { light: 0.5, dark: 0.18 })}" stroke="${darker(suitWhite, 0.2)}" stroke-width="1.5"/>` +
    `<path d="M 22 104 Q 70 132 118 104" stroke="${suitLilac}" stroke-width="7" fill="none" stroke-linecap="round"/>` +
    roundStar(70, 120, 6, palette.yellow, { strokeWidth: 1.6 }) +
    `<g transform="translate(16 12) scale(0.9)">` +
    `<circle cx="60" cy="62" r="44" fill="${d.radial([[0, '#3C2A78'], [1, '#2B1B5A']])}"/>` +
    `<g clip-path="${d.clip('<circle cx="60" cy="62" r="44"/>')}">${unicornHead(d, f)}</g>` +
    `<circle cx="60" cy="62" r="44" fill="${d.radial([[0, '#FFFFFF', 0.05], [0.8, '#DDF1FF', 0.2], [1, '#BFE3FF', 0.5]], { cx: '40%', cy: '35%', r: '60%' })}"/>` +
    `<circle cx="60" cy="62" r="45" fill="none" stroke="${collar}" stroke-width="5"/>` +
    `<path d="M 30 44 Q 38 28 54 23" stroke="#FFFFFF" stroke-width="4" fill="none" stroke-linecap="round" opacity="0.85"/>` +
    `</g>` +
    `<path d="M 70 0 L 63 22 L 77 22 Z" fill="${d.shade(palette.yellow, { light: 0.5, dark: 0.2 })}" stroke="#E0A800" stroke-width="1.4" stroke-linejoin="round"/>` +
    `<path d="M 66 12 L 74 10 M 65 17 L 75 15" stroke="#E0A800" stroke-width="1.4" stroke-linecap="round"/>` +
    arms +
    `</g>` +
    extras(d, f, size);
  return d.render(body);
}

const catPurple = '#9B6CFF';

/** The co-pilot: her purple cat with black spots and a pink scarf, sitting (a 120 × 120 space). */
function catFigure(d, f) {
  const spot = (x, y, r) => `<ellipse cx="${x}" cy="${y}" rx="${r}" ry="${r * 0.8}" fill="${ink}" opacity="0.92"/>`;
  let paws = `<ellipse cx="48" cy="112" rx="9" ry="6" fill="${d.shade(catPurple)}"/><ellipse cx="72" cy="112" rx="9" ry="6" fill="${d.shade(catPurple)}"/>`;
  if (f.arms === 'up' || f.arms === 'up-high') {
    const y = f.arms === 'up' ? 72 : 64;
    paws += `<ellipse cx="22" cy="${y}" rx="8" ry="7" fill="${d.shade(catPurple)}"/><ellipse cx="98" cy="${y}" rx="8" ry="7" fill="${d.shade(catPurple)}"/>`;
  } else if (f.arms === 'wave') {
    paws += `<ellipse cx="98" cy="66" rx="8" ry="7" fill="${d.shade(catPurple)}"/>`;
  } else if (f.arms === 'chin') {
    paws += `<ellipse cx="50" cy="74" rx="8" ry="7" fill="${d.shade(catPurple)}"/>`;
  }
  const catMouth =
    f.mouth === 'talk' || f.mouth === 'open-smile' || f.mouth === 'yawn'
      ? `<ellipse cx="60" cy="66" rx="4.5" ry="${f.mouth === 'yawn' ? 5 : 3.8}" fill="#7A2848"/>`
      : `<path d="M 54 64 q 3 3 6 0 q 3 3 6 0" stroke="${ink}" stroke-width="2" fill="none" stroke-linecap="round"/>`;
  let catEyes;
  if (f.eyes === 'open' || f.eyes === 'up') {
    const dx = f.eyes === 'up' ? 1.2 : 0;
    const dy = f.eyes === 'up' ? -1.6 : 0;
    catEyes = [48, 72]
      .map((x) => `<ellipse cx="${x}" cy="52" rx="7" ry="7.5" fill="#FFFFFF"/><ellipse cx="${x + dx}" cy="${53 + dy}" rx="4.4" ry="4.8" fill="${ink}"/><circle cx="${x + dx + 1.6}" cy="${51 + dy}" r="1.6" fill="#FFFFFF"/>`)
      .join('');
  } else if (f.eyes === 'happy') {
    catEyes = [48, 72].map((x) => `<path d="M ${x - 6} 54 Q ${x} 45 ${x + 6} 54" stroke="${ink}" stroke-width="3" fill="none" stroke-linecap="round"/>`).join('');
  } else {
    catEyes = [48, 72].map((x) => `<path d="M ${x - 6} 52 Q ${x} 58 ${x + 6} 52" stroke="${ink}" stroke-width="3" fill="none" stroke-linecap="round"/>`).join('');
  }
  return (
    `<path d="M 86 104 C 112 102, 112 74, 100 68" stroke="${darker(catPurple, 0.2)}" stroke-width="10" fill="none" stroke-linecap="round"/>` +
    `<path d="M 86 104 C 110 102, 110 76, 100 68" stroke="${catPurple}" stroke-width="8" fill="none" stroke-linecap="round"/>` +
    `<circle cx="106" cy="86" r="2.6" fill="${ink}"/>` +
    `<ellipse cx="60" cy="96" rx="27" ry="21" fill="${d.shade(catPurple)}"/>` +
    spot(46, 98, 4) +
    spot(74, 92, 3.5) +
    spot(64, 108, 3) +
    `<path d="M 30 38 L 33 12 L 51 28 Z" fill="${d.shade(catPurple)}"/><path d="M 90 38 L 87 12 L 69 28 Z" fill="${d.shade(catPurple, { cx: '20%' })}"/>` +
    `<path d="M 35 33 L 37 20 L 46 29 Z" fill="#FFB3D1"/><path d="M 85 33 L 83 20 L 74 29 Z" fill="#FFB3D1"/>` +
    `<ellipse cx="60" cy="54" rx="33" ry="28" fill="${d.shade(catPurple)}"/>` +
    spot(36, 44, 4) +
    spot(80, 40, 3.5) +
    spot(86, 60, 4) +
    spot(32, 62, 3) +
    catEyes +
    `<path d="M 57 59 L 63 59 L 60 62.5 Z" fill="${palette.pink}"/>` +
    catMouth +
    `<path d="M 40 62 L 26 59 M 40 65 L 26 67 M 80 62 L 94 59 M 80 65 L 94 67" stroke="#FFFFFF" stroke-width="1.4" stroke-linecap="round" opacity="0.9"/>` +
    cheek(d, 40, 64, 4.5, 2.8) +
    cheek(d, 80, 64, 4.5, 2.8) +
    `<path d="M 34 76 Q 60 88 86 76 L 87 84 Q 60 96 33 84 Z" fill="${d.shade(palette.pink, { light: 0.45 })}"/>` +
    `<path d="M 74 84 L 80 104 L 90 98 Z" fill="${d.shade(palette.pink, { light: 0.45 })}"/>` +
    paws
  );
}

/** The co-pilot cat in one frame. */
function cat(f, size = 384) {
  const d = new Drawing(size, size, `cat${f.id}`);
  const k = size / 128;
  const body =
    `<g transform="translate(${4 * k} ${6 * k}) scale(${k})">` + d.groundShadow(60, 118, 34, 4, 0.18) + catFigure(d, f) + `</g>` + extras(d, f, size);
  return d.render(body);
}

const takeDrawers = { helmet: unicornHelmet, suit: unicornSuit, round: unicornRound };

/** The unicorn in one take and one frame (for the choosing page and the chosen take's sprites). */
export function unicorn(take, frameId, size = 384) {
  const f = frames.find((x) => x.id === frameId);
  return takeDrawers[take](f, size);
}

/** The co-pilot cat in one frame. */
export function catFrame(frameId, size = 384) {
  return cat(frames.find((x) => x.id === frameId), size);
}

/** The chosen take (decision D5, the canvas's helmet until she chooses on the candidates page). */
export const chosenTake = 'helmet';

/** Every guide sprite: Guides/<guide-id>/<frame>.png, 384 × 384. */
export function sprites() {
  const list = [];
  for (const f of frames) {
    list.push({ path: `Guides/guide-unicorn/${f.id}`, width: 384, height: 384, svg: unicorn(chosenTake, f.id) });
    list.push({ path: `Guides/guide-cat/${f.id}`, width: 384, height: 384, svg: catFrame(f.id) });
  }
  return list;
}
