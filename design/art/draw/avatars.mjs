// Pisicuța, the first of the 14 profile avatars (decision D9, design screen 11): the canvas's
// drawing (design/canvas/Avatar.dc.html), kept as drawn, with the look's soft shading. She is drawn
// only from the canvas's drawing, never from anything else. The other 13 are ChatGPT's (drafts.json).

import { Drawing, ink, darker } from '../lib/svg.mjs';

function background(d, colour) {
  return (
    `<circle cx="60" cy="60" r="56" fill="${d.radial([[0, '#FFFFFF'], [0.55, colour], [1, darker(colour, 0.06)]], { cx: '38%', cy: '32%', r: '70%' })}"/>` +
    `<circle cx="60" cy="60" r="55" fill="none" stroke="#FFFFFF" stroke-width="2" opacity="0.7"/>`
  );
}

const drawers = {
  kitty(d) {
    const S = (c) => d.shade(c, { light: 0.35, dark: 0.12 });
    const clip = d.clip('<circle cx="60" cy="60" r="56"/>');
    return (
      background(d, '#EEE6FF') +
      `<g clip-path="${clip}">` +
      `<path d="M 22 120 C 22 100, 36 92, 60 92 C 84 92, 98 100, 98 120 Z" fill="${S('#6E6E7A')}"/>` +
      `<path d="M 38 120 L 40 96 C 50 100, 70 100, 80 96 L 82 120 Z" fill="${S('#D6D6DC')}"/>` +
      `<g fill="${ink}"><circle cx="46" cy="102" r="1.6"/><circle cx="54" cy="108" r="1.6"/><circle cx="62" cy="102" r="1.6"/><circle cx="70" cy="108" r="1.6"/><circle cx="76" cy="101" r="1.6"/><circle cx="46" cy="114" r="1.6"/><circle cx="62" cy="114" r="1.6"/><circle cx="78" cy="114" r="1.6"/></g>` +
      `</g>` +
      `<rect x="53" y="82" width="14" height="12" rx="5" fill="#FBD9C4"/>` +
      `<path d="M 26 64 C 24 44, 36 30, 60 30 C 84 30, 96 44, 94 64 C 94 74, 90 82, 88 86 L 84 60 L 36 60 L 32 86 C 30 82, 26 74, 26 64 Z" fill="${S('#E9C77E')}"/>` +
      `<ellipse cx="60" cy="62" rx="27" ry="28" fill="${S('#FFE6D6')}"/>` +
      `<path d="M 33 56 C 34 40, 46 34, 60 34 C 74 34, 86 40, 87 56 C 80 46, 70 42, 60 43 C 50 42, 40 46, 33 56 Z" fill="${S('#F2D898')}"/>` +
      `<path d="M 33 56 C 31 66, 31 76, 33 84" stroke="#F2D898" stroke-width="3" fill="none" stroke-linecap="round"/>` +
      `<path d="M 87 56 C 89 66, 89 76, 87 84" stroke="#F2D898" stroke-width="3" fill="none" stroke-linecap="round"/>` +
      `<circle cx="60" cy="24" r="10" fill="${S('#F2D898')}"/>` +
      `<path d="M 56 26 Q 60 32 64 26" stroke="#E0B860" stroke-width="1.5" fill="none"/>` +
      `<path d="M 60 16 L 48 9 L 49 22 Z" fill="#FFFFFF" stroke="#C9D3E8" stroke-width="1.2" stroke-linejoin="round"/>` +
      `<path d="M 60 16 L 72 9 L 71 22 Z" fill="#FFFFFF" stroke="#C9D3E8" stroke-width="1.2" stroke-linejoin="round"/>` +
      `<circle cx="60" cy="16" r="2.6" fill="#FFFFFF" stroke="#C9D3E8" stroke-width="1.2"/>` +
      `<path d="M 44 54 L 46 44 L 52 50 L 56 42 L 60 49 L 64 42 L 68 50 L 74 44 L 76 54 Q 60 50 44 54 Z" fill="#FFFFFF" stroke="#E2405A" stroke-width="2.2" stroke-linejoin="round"/>` +
      `<path d="M 52 47 L 53 51 M 60 45 L 60 49 M 68 47 L 67 51" stroke="${ink}" stroke-width="1.6" stroke-linecap="round"/>` +
      `<ellipse cx="50" cy="62" rx="4.2" ry="4.6" fill="#4A2E1E"/><ellipse cx="70" cy="62" rx="4.2" ry="4.6" fill="#4A2E1E"/>` +
      `<circle cx="51.5" cy="60.2" r="1.5" fill="#FFFFFF"/><circle cx="71.5" cy="60.2" r="1.5" fill="#FFFFFF"/>` +
      `<path d="M 57 68 L 63 68 L 60 72 Z" fill="${ink}"/>` +
      `<path d="M 46 72 L 34 69 M 46 75 L 34 76 M 46 78 L 36 82" stroke="${ink}" stroke-width="1.8" stroke-linecap="round"/>` +
      `<path d="M 74 72 L 86 69 M 74 75 L 86 76 M 74 78 L 84 82" stroke="${ink}" stroke-width="1.8" stroke-linecap="round"/>` +
      `<path d="M 51 76 Q 60 86 69 76 Q 60 80 51 76 Z" fill="#C2485F"/>` +
      `<path d="M 53 76.6 Q 60 79.4 67 76.6 L 66 78.4 Q 60 80.4 54 78.4 Z" fill="#FFFFFF"/>` +
      `<ellipse cx="42" cy="70" rx="4" ry="2.4" fill="#FFB3C1" opacity="0.8"/><ellipse cx="78" cy="70" rx="4" ry="2.4" fill="#FFB3C1" opacity="0.8"/>` +
      `<circle cx="33" cy="70" r="1.6" fill="#FFD93D"/><circle cx="87" cy="70" r="1.6" fill="#FFD93D"/>`
    );
  },
};

/** One avatar, 320 × 320 (drawn in the canvas's 120 space). Called by sprites. */
function avatar(id, size = 320) {
  const d = new Drawing(size, size, `av${id}`);
  return d.render(`<g transform="scale(${size / 120})">${drawers[id](d)}</g>`);
}

/** The avatars the agent draws: Pisicuța only; the other 13 are ChatGPT's (drafts.json). */
export function sprites() {
  return Object.keys(drawers).map((id) => ({ path: `Avatars/avatar-${id}`, width: 320, height: 320, svg: avatar(id) }));
}
