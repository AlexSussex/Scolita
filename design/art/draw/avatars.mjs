// The 14 profile avatars (decision D9, design screen 11): the canvas's drawings
// (design/canvas/Avatar.dc.html), kept as drawn, with the look's soft shading lit from the top left.
// Pisicuța is drawn only from the canvas's drawing, never from anything else.

import { Drawing, ink, darker } from '../lib/svg.mjs';

/** The avatars in the catalog's order, with their stable ids. */
export const avatarIds = ['kitty', 'cat', 'unicorn', 'rocket', 'star', 'moon', 'planet', 'whale', 'guitar', 'bicycle', 'rainbow', 'fox', 'owl', 'dog'];

const twinkle = (x, y) => `<path d="M ${x} ${y} l 3 7 l 7 3 l -7 3 l -3 7 l -3 -7 l -7 -3 l 7 -3 z"`;

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
  cat(d) {
    const P = d.shade('#9B6CFF');
    return (
      background(d, '#EEE6FF') +
      `<path d="M 30 44 L 34 16 L 52 34 Z" fill="${P}"/><path d="M 90 44 L 86 16 L 68 34 Z" fill="${P}"/>` +
      `<path d="M 36 38 L 38 24 L 48 34 Z" fill="#FFB3D1"/><path d="M 84 38 L 82 24 L 72 34 Z" fill="#FFB3D1"/>` +
      `<ellipse cx="60" cy="62" rx="36" ry="32" fill="${P}"/>` +
      `<circle cx="40" cy="48" r="5" fill="${ink}"/><circle cx="78" cy="44" r="4" fill="${ink}"/><circle cx="84" cy="66" r="5" fill="${ink}"/><circle cx="34" cy="70" r="4" fill="${ink}"/>` +
      `<circle cx="48" cy="60" r="6" fill="#FFFFFF"/><circle cx="72" cy="60" r="6" fill="#FFFFFF"/>` +
      `<circle cx="49" cy="61" r="3.5" fill="${ink}"/><circle cx="73" cy="61" r="3.5" fill="${ink}"/>` +
      `<circle cx="50.2" cy="59.6" r="1.2" fill="#FFFFFF"/><circle cx="74.2" cy="59.6" r="1.2" fill="#FFFFFF"/>` +
      `<path d="M 56 70 L 64 70 L 60 75 Z" fill="#FF5C8A"/>` +
      `<path d="M 52 78 q 8 6 16 0" stroke="${ink}" stroke-width="2.5" fill="none" stroke-linecap="round"/>` +
      `<path d="M 30 88 Q 60 102 90 88 L 92 98 Q 60 112 28 98 Z" fill="${d.shade('#FF5C8A', { light: 0.4 })}"/>` +
      `<path d="M 76 96 L 82 116 L 92 110 Z" fill="${d.shade('#FF5C8A', { light: 0.4 })}"/>` +
      d.gloss(46, 42, 8, 4, 0.35)
    );
  },
  unicorn(d) {
    return (
      background(d, '#FFE3EC') +
      `<path d="M 60 8 L 53 32 L 67 32 Z" fill="${d.shade('#FFD93D')}" stroke="#E0A800" stroke-width="1.5"/>` +
      `<path d="M 55 25 L 65 22 M 54 29 L 66 26" stroke="#E0A800" stroke-width="1.5"/>` +
      `<path d="M 36 40 L 40 22 L 50 34 Z" fill="#FFFFFF"/><path d="M 84 40 L 80 22 L 70 34 Z" fill="#FFFFFF"/>` +
      `<circle cx="60" cy="62" r="32" fill="${d.shade('#FFFFFF', { light: 0, dark: 0.1 })}"/>` +
      `<path d="M 32 50 C 18 62, 20 82, 30 96" stroke="#FF5C8A" stroke-width="7" fill="none" stroke-linecap="round"/>` +
      `<path d="M 38 42 C 24 54, 24 80, 36 100" stroke="#FFD93D" stroke-width="7" fill="none" stroke-linecap="round"/>` +
      `<path d="M 45 36 C 30 48, 32 76, 44 102" stroke="#9B6CFF" stroke-width="7" fill="none" stroke-linecap="round"/>` +
      `<ellipse cx="63" cy="80" rx="18" ry="12" fill="${d.shade('#FFE3EC', { dark: 0.08 })}"/>` +
      `<ellipse cx="57" cy="80" rx="2.5" ry="1.8" fill="#E58BA8"/><ellipse cx="69" cy="80" rx="2.5" ry="1.8" fill="#E58BA8"/>` +
      `<circle cx="54" cy="60" r="5" fill="${ink}"/><circle cx="74" cy="60" r="5" fill="${ink}"/>` +
      `<circle cx="56" cy="58" r="1.8" fill="#FFFFFF"/><circle cx="76" cy="58" r="1.8" fill="#FFFFFF"/>` +
      `<ellipse cx="82" cy="70" rx="5" ry="3" fill="#FF9FB8" opacity="0.8"/>`
    );
  },
  rocket(d) {
    return (
      background(d, '#E2F2FF') +
      `<g transform="rotate(25 60 60)">` +
      `<path d="M 44 66 L 32 84 L 46 80 Z" fill="${d.shade('#9B6CFF')}"/><path d="M 76 66 L 88 84 L 74 80 Z" fill="${d.shade('#9B6CFF')}"/>` +
      `<path d="M 60 14 C 76 26, 80 52, 76 78 L 44 78 C 40 52, 44 26, 60 14 Z" fill="${d.shade('#F2F4FA', { light: 0.6, dark: 0.15 })}" stroke="#C9D3E8" stroke-width="2"/>` +
      `<path d="M 60 14 C 70 20, 75 30, 77 38 L 43 38 C 45 30, 50 20, 60 14 Z" fill="${d.shade('#FF5C8A')}"/>` +
      `<circle cx="60" cy="54" r="8" fill="${d.shade('#3BA7F0')}" stroke="#9FB3D6" stroke-width="3"/>` +
      `<path d="M 50 80 Q 60 108 70 80 Z" fill="#FF9F43"/><path d="M 54 80 Q 60 96 66 80 Z" fill="#FFD93D"/>` +
      `</g>`
    );
  },
  star(d) {
    return (
      background(d, '#FFF1D6') +
      `<path d="M 60 14 L 72 44 L 104 46 L 79 66 L 88 98 L 60 80 L 32 98 L 41 66 L 16 46 L 48 44 Z" fill="${d.shade('#FFD93D')}" stroke="#E0A800" stroke-width="3" stroke-linejoin="round"/>` +
      `<circle cx="51" cy="58" r="4.5" fill="${ink}"/><circle cx="69" cy="58" r="4.5" fill="${ink}"/>` +
      `<circle cx="52.5" cy="56.5" r="1.6" fill="#FFFFFF"/><circle cx="70.5" cy="56.5" r="1.6" fill="#FFFFFF"/>` +
      `<path d="M 52 68 q 8 7 16 0" stroke="${ink}" stroke-width="2.5" fill="none" stroke-linecap="round"/>` +
      `<ellipse cx="44" cy="67" rx="4" ry="2.5" fill="#FF9FB8"/><ellipse cx="76" cy="67" rx="4" ry="2.5" fill="#FF9FB8"/>`
    );
  },
  moon(d) {
    const maskId = d.id('m');
    d.defs.push(`<mask id="${maskId}"><rect width="120" height="120" fill="#FFFFFF"/><circle cx="84" cy="50" r="32" fill="#000000"/></mask>`);
    return (
      background(d, '#E6E9FF') +
      `<g mask="url(#${maskId})"><circle cx="62" cy="60" r="40" fill="${d.shade('#FFE9A8', { dark: 0.15 })}" stroke="#E0B84A" stroke-width="2"/></g>` +
      `<path d="M 33 52 q 5 -5 10 0" stroke="${ink}" stroke-width="2.8" fill="none" stroke-linecap="round"/>` +
      `<path d="M 32 70 q 7 6 14 1" stroke="${ink}" stroke-width="2.5" fill="none" stroke-linecap="round"/>` +
      `<ellipse cx="30" cy="61" rx="3.5" ry="2.2" fill="#FF9FB8"/>` +
      `${twinkle(92, 30)} fill="#9B6CFF"/>` +
      `<circle cx="96" cy="80" r="3" fill="#9B6CFF"/>`
    );
  },
  planet(d) {
    return (
      background(d, '#DFF7E8') +
      `<path d="M 18 70 Q 60 96 102 62" fill="none" stroke="#9B6CFF" stroke-width="7" stroke-linecap="round"/>` +
      `<circle cx="60" cy="58" r="30" fill="${d.shade('#4CD97B')}"/>` +
      `<path d="M 34 56 Q 60 74 86 52" fill="none" stroke="#1F8C4A" stroke-width="3" opacity="0.5"/>` +
      `<circle cx="50" cy="54" r="4.5" fill="#0E4A26"/><circle cx="70" cy="54" r="4.5" fill="#0E4A26"/>` +
      `<circle cx="51.5" cy="52.5" r="1.6" fill="#FFFFFF"/><circle cx="71.5" cy="52.5" r="1.6" fill="#FFFFFF"/>` +
      `<path d="M 52 64 q 8 7 16 0" stroke="#0E4A26" stroke-width="2.5" fill="none" stroke-linecap="round"/>` +
      `<path d="M 20 66 Q 30 74 40 76" fill="none" stroke="#9B6CFF" stroke-width="7" stroke-linecap="round"/>`
    );
  },
  whale(d) {
    return (
      background(d, '#E2F2FF') +
      `<path d="M 60 30 Q 54 20 46 22 M 60 30 Q 66 20 74 22" stroke="#3BA7F0" stroke-width="4" fill="none" stroke-linecap="round"/>` +
      `<path d="M 18 66 C 18 44, 40 36, 62 38 C 86 40, 96 54, 96 64 C 104 58, 110 50, 112 44 C 114 56, 110 70, 100 76 C 92 88, 74 94, 56 92 C 34 90, 18 82, 18 66 Z" fill="${d.shade('#3BA7F0')}"/>` +
      `<path d="M 24 74 C 34 86, 66 90, 88 80" fill="#BFE3FF"/>` +
      `<circle cx="40" cy="58" r="5" fill="#0C2F4D"/><circle cx="41.5" cy="56.5" r="1.8" fill="#FFFFFF"/>` +
      `<path d="M 30 70 q 8 5 16 2" stroke="#0C2F4D" stroke-width="2.5" fill="none" stroke-linecap="round"/>` +
      `<ellipse cx="50" cy="68" rx="4" ry="2.5" fill="#FF9FB8"/>`
    );
  },
  guitar(d) {
    return (
      background(d, '#FFE9D6') +
      `<path d="M 76 14 L 88 10 L 94 18 L 84 24 Z" fill="#6B3A1E"/>` +
      `<rect x="68" y="20" width="9" height="44" rx="3" fill="${d.shade('#B57A3A')}" transform="rotate(30 72 42)"/>` +
      `<path d="M 56 52 C 42 40, 20 46, 22 64 C 12 72, 16 96, 38 100 C 58 104, 72 92, 68 76 C 80 68, 72 52, 56 52 Z" fill="${d.shade('#FF9F43')}"/>` +
      `<circle cx="46" cy="76" r="8" fill="#6B3A1E"/>` +
      `<path d="M 30 86 L 42 92" stroke="#6B3A1E" stroke-width="4" stroke-linecap="round"/>` +
      `<path d="M 30 60 Q 26 66 28 72" stroke="#FFFFFF" stroke-width="3" fill="none" stroke-linecap="round" opacity="0.7"/>`
    );
  },
  bicycle(d) {
    return (
      background(d, '#FFE3EC') +
      `<circle cx="34" cy="74" r="18" fill="none" stroke="${ink}" stroke-width="5"/>` +
      `<circle cx="86" cy="74" r="18" fill="none" stroke="${ink}" stroke-width="5"/>` +
      `<circle cx="34" cy="74" r="3" fill="${ink}"/><circle cx="86" cy="74" r="3" fill="${ink}"/>` +
      `<path d="M 34 74 L 52 48 L 78 48 L 86 74 M 52 48 L 60 74 L 78 48 M 34 74 L 60 74" stroke="#FF5C8A" stroke-width="5" fill="none" stroke-linejoin="round" stroke-linecap="round"/>` +
      `<path d="M 34 74 L 52 48 L 78 48" stroke="#FFFFFF" stroke-width="1.4" fill="none" stroke-linecap="round" opacity="0.6" transform="translate(-1 -1.5)"/>` +
      `<path d="M 48 40 L 58 40" stroke="${ink}" stroke-width="5" stroke-linecap="round"/>` +
      `<path d="M 78 48 L 74 36 L 84 34" stroke="${ink}" stroke-width="4" fill="none" stroke-linecap="round" stroke-linejoin="round"/>` +
      `${twinkle(94, 24)} fill="#FFD93D"/>`
    );
  },
  rainbow(d) {
    return (
      background(d, '#FFF1D6') +
      `<path d="M 30 62 A 30 30 0 0 1 90 62" fill="none" stroke="#FF5C8A" stroke-width="7"/>` +
      `<path d="M 37 62 A 23 23 0 0 1 83 62" fill="none" stroke="#FFD93D" stroke-width="7"/>` +
      `<path d="M 44 62 A 16 16 0 0 1 76 62" fill="none" stroke="#4CD97B" stroke-width="7"/>` +
      `<path d="M 51 62 A 9 9 0 0 1 69 62" fill="none" stroke="#3BA7F0" stroke-width="7"/>` +
      `<ellipse cx="34" cy="70" rx="14" ry="9" fill="${d.shade('#FFFFFF', { light: 0, dark: 0.08 })}"/><ellipse cx="86" cy="70" rx="14" ry="9" fill="${d.shade('#FFFFFF', { light: 0, dark: 0.08 })}"/>` +
      `${twinkle(84, 30)} fill="#FFD93D"/>`
    );
  },
  fox(d) {
    const O = d.shade('#FF7A1A');
    return (
      background(d, '#FFE9D6') +
      `<path d="M 26 46 L 30 14 L 52 34 Z" fill="${O}"/><path d="M 94 46 L 90 14 L 68 34 Z" fill="${O}"/>` +
      `<path d="M 32 40 L 34 24 L 46 34 Z" fill="${ink}"/><path d="M 88 40 L 86 24 L 74 34 Z" fill="${ink}"/>` +
      `<path d="M 22 50 C 26 30, 94 30, 98 50 C 96 76, 76 96, 60 98 C 44 96, 24 76, 22 50 Z" fill="${O}"/>` +
      `<path d="M 24 56 C 34 64, 48 70, 60 98 C 72 70, 86 64, 96 56 C 92 80, 76 96, 60 98 C 44 96, 28 80, 24 56 Z" fill="${d.shade('#FFFFFF', { light: 0, dark: 0.08 })}"/>` +
      `<circle cx="46" cy="58" r="5" fill="${ink}"/><circle cx="74" cy="58" r="5" fill="${ink}"/>` +
      `<circle cx="47.5" cy="56.5" r="1.8" fill="#FFFFFF"/><circle cx="75.5" cy="56.5" r="1.8" fill="#FFFFFF"/>` +
      `<ellipse cx="60" cy="88" rx="6" ry="4.5" fill="${ink}"/>`
    );
  },
  owl(d) {
    return (
      background(d, '#EEE6FF') +
      `<path d="M 30 36 L 36 16 L 50 30 Z" fill="${d.shade('#7A4FD6')}"/><path d="M 90 36 L 84 16 L 70 30 Z" fill="${d.shade('#7A4FD6')}"/>` +
      `<path d="M 26 56 C 26 30, 94 30, 94 56 C 94 84, 80 104, 60 104 C 40 104, 26 84, 26 56 Z" fill="${d.shade('#9B6CFF')}"/>` +
      `<path d="M 40 74 C 44 92, 76 92, 80 74 C 74 82, 46 82, 40 74 Z" fill="#D8CCFF"/>` +
      `<circle cx="45" cy="54" r="14" fill="#FFFFFF"/><circle cx="75" cy="54" r="14" fill="#FFFFFF"/>` +
      `<circle cx="45" cy="55" r="7" fill="${ink}"/><circle cx="75" cy="55" r="7" fill="${ink}"/>` +
      `<circle cx="47" cy="52" r="2.5" fill="#FFFFFF"/><circle cx="77" cy="52" r="2.5" fill="#FFFFFF"/>` +
      `<path d="M 55 66 L 65 66 L 60 74 Z" fill="#FFD93D"/>` +
      `<path d="M 46 96 l -4 8 M 52 98 l -2 8 M 68 98 l 2 8 M 74 96 l 4 8" stroke="#FFD93D" stroke-width="3" stroke-linecap="round"/>`
    );
  },
  dog(d) {
    return (
      background(d, '#E3F6EA') +
      `<path d="M 32 34 C 18 36, 14 62, 22 80 C 28 90, 38 84, 38 70 Z" fill="${d.shade('#8B5A2B')}"/>` +
      `<path d="M 88 34 C 102 36, 106 62, 98 80 C 92 90, 82 84, 82 70 Z" fill="${d.shade('#8B5A2B')}"/>` +
      `<ellipse cx="60" cy="62" rx="30" ry="32" fill="${d.shade('#E0A96D')}"/>` +
      `<ellipse cx="72" cy="54" rx="9" ry="8" fill="#B97A3E"/>` +
      `<ellipse cx="60" cy="78" rx="18" ry="14" fill="${d.shade('#FFF3E4', { light: 0.3, dark: 0.08 })}"/>` +
      `<circle cx="48" cy="56" r="4.8" fill="${ink}"/><circle cx="72" cy="56" r="4.8" fill="${ink}"/>` +
      `<circle cx="49.6" cy="54.4" r="1.7" fill="#FFFFFF"/><circle cx="73.6" cy="54.4" r="1.7" fill="#FFFFFF"/>` +
      `<ellipse cx="60" cy="70" rx="6" ry="4.5" fill="${ink}"/>` +
      `<path d="M 60 74 L 60 79 M 52 79 Q 60 85 68 79" stroke="${ink}" stroke-width="2.4" fill="none" stroke-linecap="round"/>` +
      `<path d="M 56 81 Q 60 92 64 81 Z" fill="#FF7FA0"/>` +
      `<ellipse cx="42" cy="68" rx="4" ry="2.4" fill="#FF9FB8" opacity="0.7"/><ellipse cx="78" cy="68" rx="4" ry="2.4" fill="#FF9FB8" opacity="0.7"/>` +
      `<path d="M 40 96 Q 60 104 80 96 L 80 102 Q 60 110 40 102 Z" fill="#9B6CFF"/>` +
      `<circle cx="60" cy="104" r="4" fill="${d.shade('#FFD93D')}" stroke="#E0A800" stroke-width="1.2"/>`
    );
  },
};

/** One avatar, 320 × 320 (drawn in the canvas's 120 space). */
export function avatar(id, size = 320) {
  const d = new Drawing(size, size, `av${id}`);
  return d.render(`<g transform="scale(${size / 120})">${drawers[id](d)}</g>`);
}

/** Every avatar sprite: Avatars/avatar-<id>.png. */
export function sprites() {
  return avatarIds.map((id) => ({ path: `Avatars/avatar-${id}`, width: 320, height: 320, svg: avatar(id) }));
}
