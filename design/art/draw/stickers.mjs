// The stickers she earns (30, die-cut: a thick white border and a soft shadow),
// and the three constellations whose stars she lights at the end of a lesson.

import { Drawing, palette, ink, lighter, darker, sparkle, eye, happyEye, cheek, starPath } from '../lib/svg.mjs';

/** The die-cut look: the art's silhouette grown into a white border, with a soft shadow down-right. */
function stickerFilter(d) {
  const id = d.id('f');
  d.defs.push(
    `<filter id="${id}" x="-25%" y="-25%" width="150%" height="150%" color-interpolation-filters="sRGB">` +
      `<feGaussianBlur in="SourceAlpha" stdDeviation="8" result="b"/>` +
      `<feComponentTransfer in="b" result="grown"><feFuncA type="linear" slope="30" intercept="-1.5"/></feComponentTransfer>` +
      `<feFlood flood-color="#FFFFFF" result="w"/>` +
      `<feComposite in="w" in2="grown" operator="in" result="border"/>` +
      `<feGaussianBlur in="grown" stdDeviation="5" result="sb"/>` +
      `<feOffset in="sb" dx="4" dy="6" result="so"/>` +
      `<feFlood flood-color="${ink}" flood-opacity="0.3" result="sc"/>` +
      `<feComposite in="sc" in2="so" operator="in" result="shadow"/>` +
      `<feMerge><feMergeNode in="shadow"/><feMergeNode in="border"/></feMerge>` +
      `</filter>`,
  );
  return `url(#${id})`;
}

/** A sticker: the art twice, once as its border and shadow, once on top, and a tiny shine. */
function sticker(id, draw) {
  const d = new Drawing(320, 320, `sk${id.replace(/-/g, '')}`);
  const art = draw(d);
  const shine = sparkle(258, 62, 11, '#FFFFFF', 0.95) + sparkle(276, 88, 5, palette.sparkle, 0.9);
  return d.render(`<g filter="${stickerFilter(d)}">${art}</g>${art}${shine}`);
}

/** A kawaii face: two eyes, a smile and pink cheeks, centred on (x, y) at scale s. */
function face(d, x, y, s, color = ink, { happy = false } = {}) {
  const eyes = happy
    ? happyEye(x - 22 * s, y, 9 * s, color, 4.5 * s) + happyEye(x + 22 * s, y, 9 * s, color, 4.5 * s)
    : eye(x - 22 * s, y, 8 * s, color) + eye(x + 22 * s, y, 8 * s, color);
  return (
    eyes +
    `<path d="M ${x - 10 * s} ${y + 16 * s} Q ${x} ${y + 26 * s} ${x + 10 * s} ${y + 16 * s}" stroke="${color}" stroke-width="${4.5 * s}" fill="none" stroke-linecap="round"/>` +
    cheek(d, x - 38 * s, y + 14 * s, 9 * s, 6 * s) +
    cheek(d, x + 38 * s, y + 14 * s, 9 * s, 6 * s)
  );
}

const rainbow = [palette.pink, palette.orange, palette.yellow, palette.green, palette.blue, palette.purple];

// ---------- Animals ----------

function catPurple(d) {
  const p = palette.purple;
  const clip = d.clip('<ellipse cx="160" cy="158" rx="88" ry="76"/>');
  return (
    `<path d="M 84 128 L 92 46 L 146 98 Z" fill="${d.shade(p)}" stroke="${darker(p, 0.2)}" stroke-width="3" stroke-linejoin="round"/>` +
    `<path d="M 236 128 L 228 46 L 174 98 Z" fill="${d.shade(p)}" stroke="${darker(p, 0.2)}" stroke-width="3" stroke-linejoin="round"/>` +
    `<path d="M 98 112 L 102 70 L 132 100 Z" fill="#FFB3D1"/><path d="M 222 112 L 218 70 L 188 100 Z" fill="#FFB3D1"/>` +
    `<ellipse cx="160" cy="158" rx="88" ry="76" fill="${d.shade(p)}" stroke="${darker(p, 0.2)}" stroke-width="3"/>` +
    `<g clip-path="${clip}" fill="${ink}"><circle cx="96" cy="128" r="11"/><circle cx="224" cy="122" r="9"/><circle cx="232" cy="178" r="11"/><circle cx="88" cy="186" r="9"/><circle cx="160" cy="98" r="8"/></g>` +
    eye(128, 152, 13) +
    eye(192, 152, 13) +
    `<path d="M 152 172 L 168 172 L 160 181 Z" fill="${palette.pink}" stroke-linejoin="round" stroke="${palette.pink}" stroke-width="3"/>` +
    `<path d="M 148 188 Q 154 196 160 188 Q 166 196 172 188" stroke="${ink}" stroke-width="3.5" fill="none" stroke-linecap="round"/>` +
    `<path d="M 112 178 L 76 172 M 112 186 L 78 192 M 208 178 L 244 172 M 208 186 L 242 192" stroke="#FFFFFF" stroke-width="3" stroke-linecap="round" opacity="0.9"/>` +
    cheek(d, 108, 172, 10, 7) +
    cheek(d, 212, 172, 10, 7) +
    `<path d="M 86 222 Q 160 254 234 222 L 238 246 Q 160 278 82 246 Z" fill="${d.shade(palette.pink)}"/>` +
    `<path d="M 196 248 L 206 290 L 230 280 L 218 244 Z" fill="${d.shade(palette.pink)}"/>` +
    d.gloss(118, 112, 22, 12, 0.4)
  );
}

function unicorn(d) {
  const w = '#F8F3FF';
  return (
    `<path d="M 160 34 L 144 104 L 176 104 Z" fill="${d.shade(palette.yellow)}" stroke="#E0A800" stroke-width="3" stroke-linejoin="round"/>` +
    `<path d="M 150 84 L 172 76 M 147 96 L 175 88 M 154 68 L 168 62" stroke="#E0A800" stroke-width="3"/>` +
    `<path d="M 98 120 L 104 72 L 134 106 Z" fill="${d.shade(w, { dark: 0.12 })}"/>` +
    `<path d="M 222 120 L 216 72 L 186 106 Z" fill="${d.shade(w, { dark: 0.12 })}"/>` +
    `<circle cx="162" cy="166" r="76" fill="${d.shade(w, { dark: 0.14 })}"/>` +
    `<path d="M 100 120 C 62 150, 64 210, 92 250" stroke="${palette.pink}" stroke-width="16" fill="none" stroke-linecap="round"/>` +
    `<path d="M 116 104 C 74 136, 78 204, 112 262" stroke="${palette.yellow}" stroke-width="16" fill="none" stroke-linecap="round"/>` +
    `<path d="M 134 96 C 94 126, 98 200, 134 270" stroke="${palette.purple}" stroke-width="16" fill="none" stroke-linecap="round"/>` +
    `<ellipse cx="170" cy="210" rx="44" ry="30" fill="${d.shade('#FFE3EC', { dark: 0.1 })}"/>` +
    `<ellipse cx="156" cy="210" rx="5" ry="4" fill="#E58BA8"/><ellipse cx="184" cy="210" rx="5" ry="4" fill="#E58BA8"/>` +
    eye(146, 160, 12) +
    eye(196, 160, 12) +
    cheek(d, 220, 186, 11, 7) +
    d.gloss(186, 116, 22, 10, 0.5)
  );
}

function whale(d) {
  const b = palette.blue;
  return (
    `<path d="M 160 92 Q 146 60 118 62 M 160 92 Q 174 60 202 62 M 160 92 L 160 54" stroke="${lighter(b, 0.45)}" stroke-width="9" fill="none" stroke-linecap="round"/>` +
    `<circle cx="118" cy="60" r="8" fill="${lighter(b, 0.55)}"/><circle cx="202" cy="60" r="8" fill="${lighter(b, 0.55)}"/><circle cx="160" cy="48" r="8" fill="${lighter(b, 0.55)}"/>` +
    `<path d="M 44 176 C 44 120, 100 96, 160 100 C 222 104, 248 140, 250 166 C 268 152, 280 132, 286 116 C 292 148, 282 184, 258 198 C 238 234, 196 252, 148 250 C 88 246, 44 224, 44 176 Z" fill="${d.shade(b)}" stroke="${darker(b, 0.2)}" stroke-width="3" stroke-linejoin="round"/>` +
    `<path d="M 60 200 C 88 236, 170 246, 226 218 C 190 230, 110 230, 60 200 Z" fill="${lighter(b, 0.65)}"/>` +
    eye(100, 160, 13, '#0C2F4D') +
    `<path d="M 74 192 Q 96 206 118 196" stroke="#0C2F4D" stroke-width="5" fill="none" stroke-linecap="round"/>` +
    cheek(d, 128, 186, 11, 7) +
    d.gloss(112, 124, 30, 12, 0.45)
  );
}

function fox(d) {
  const o = '#FF7A1A';
  return (
    `<path d="M 74 136 L 84 50 L 140 98 Z" fill="${d.shade(o)}" stroke="${darker(o, 0.2)}" stroke-width="3" stroke-linejoin="round"/>` +
    `<path d="M 246 136 L 236 50 L 180 98 Z" fill="${d.shade(o)}" stroke="${darker(o, 0.2)}" stroke-width="3" stroke-linejoin="round"/>` +
    `<path d="M 92 118 L 98 76 L 126 100 Z" fill="${ink}"/><path d="M 228 118 L 222 76 L 194 100 Z" fill="${ink}"/>` +
    `<path d="M 62 146 C 70 90, 250 90, 258 146 C 252 212, 200 262, 160 266 C 120 262, 68 212, 62 146 Z" fill="${d.shade(o)}" stroke="${darker(o, 0.2)}" stroke-width="3"/>` +
    `<path d="M 68 162 C 96 182, 132 196, 160 266 C 188 196, 224 182, 252 162 C 244 222, 200 262, 160 266 C 120 262, 76 222, 68 162 Z" fill="${d.shade('#FFFFFF', { dark: 0.1 })}"/>` +
    eye(122, 160, 12) +
    eye(198, 160, 12) +
    `<ellipse cx="160" cy="234" rx="15" ry="11" fill="${ink}"/>` +
    cheek(d, 100, 196, 11, 7) +
    cheek(d, 220, 196, 11, 7) +
    d.gloss(120, 112, 26, 10, 0.4)
  );
}

function owl(d) {
  const p = palette.purple;
  return (
    `<path d="M 86 108 L 96 52 L 132 90 Z" fill="${darker(p, 0.2)}"/><path d="M 234 108 L 224 52 L 188 90 Z" fill="${darker(p, 0.2)}"/>` +
    `<path d="M 72 158 C 72 82, 248 82, 248 158 C 248 232, 210 278, 160 278 C 110 278, 72 232, 72 158 Z" fill="${d.shade(p)}" stroke="${darker(p, 0.2)}" stroke-width="3"/>` +
    `<path d="M 66 170 C 50 200, 60 240, 92 252 C 84 220, 84 196, 66 170 Z" fill="${darker(p, 0.15)}"/>` +
    `<path d="M 254 170 C 270 200, 260 240, 228 252 C 236 220, 236 196, 254 170 Z" fill="${darker(p, 0.15)}"/>` +
    `<path d="M 116 214 C 124 258, 196 258, 204 214 C 188 230, 132 230, 116 214 Z" fill="#D8CCFF"/>` +
    `<circle cx="124" cy="150" r="34" fill="#FFFFFF"/><circle cx="196" cy="150" r="34" fill="#FFFFFF"/>` +
    eye(124, 152, 17) +
    eye(196, 152, 17) +
    `<path d="M 148 182 L 172 182 L 160 202 Z" fill="${d.shade(palette.yellow)}" stroke="#E0A800" stroke-width="2" stroke-linejoin="round"/>` +
    `<path d="M 130 274 l -6 14 M 144 278 l -2 14 M 176 278 l 2 14 M 190 274 l 6 14" stroke="${palette.yellow}" stroke-width="6" stroke-linecap="round"/>` +
    d.gloss(112, 104, 24, 10, 0.4)
  );
}

function puppy(d) {
  const g = '#E0A96D';
  const ear = '#8B5A2B';
  return (
    `<path d="M 92 92 C 52 96, 40 166, 62 214 C 78 238, 104 222, 104 186 Z" fill="${d.shade(ear)}"/>` +
    `<path d="M 228 92 C 268 96, 280 166, 258 214 C 242 238, 216 222, 216 186 Z" fill="${d.shade(ear)}"/>` +
    `<ellipse cx="160" cy="166" rx="76" ry="80" fill="${d.shade(g)}" stroke="${darker(g, 0.2)}" stroke-width="3"/>` +
    `<ellipse cx="192" cy="146" rx="22" ry="20" fill="#B97A3E"/>` +
    `<ellipse cx="160" cy="208" rx="44" ry="34" fill="${d.shade('#FFF3E4', { dark: 0.08 })}"/>` +
    eye(128, 150, 12) +
    eye(192, 150, 12) +
    `<ellipse cx="160" cy="188" rx="15" ry="11" fill="${ink}"/>` +
    `<path d="M 160 198 L 160 210 M 140 210 Q 160 224 180 210" stroke="${ink}" stroke-width="5" fill="none" stroke-linecap="round"/>` +
    `<path d="M 150 214 Q 160 244 170 214 Z" fill="#FF7FA0"/>` +
    cheek(d, 108, 186, 10, 6) +
    cheek(d, 212, 186, 10, 6) +
    `<path d="M 104 250 Q 160 272 216 250 L 216 264 Q 160 286 104 264 Z" fill="${d.shade(palette.purple)}"/>` +
    `<circle cx="160" cy="272" r="10" fill="${d.shade(palette.yellow)}" stroke="#E0A800" stroke-width="2"/>` +
    d.gloss(128, 110, 24, 10, 0.4)
  );
}

function panda(d) {
  const w = '#FFFFFF';
  return (
    `<circle cx="96" cy="92" r="30" fill="${d.shade('#3A3450')}"/><circle cx="224" cy="92" r="30" fill="${d.shade('#3A3450')}"/>` +
    `<ellipse cx="160" cy="166" rx="92" ry="84" fill="${d.shade(w, { dark: 0.12 })}" stroke="#D8D4E4" stroke-width="3"/>` +
    `<ellipse cx="122" cy="154" rx="24" ry="30" fill="#3A3450" transform="rotate(25 122 154)"/>` +
    `<ellipse cx="198" cy="154" rx="24" ry="30" fill="#3A3450" transform="rotate(-25 198 154)"/>` +
    eye(124, 152, 10, '#FFFFFF') +
    `<circle cx="124" cy="152" r="6" fill="${ink}"/><circle cx="196" cy="152" r="10" fill="#FFFFFF"/><circle cx="196" cy="152" r="6" fill="${ink}"/>` +
    `<circle cx="126" cy="149" r="2.2" fill="#FFFFFF"/><circle cx="198" cy="149" r="2.2" fill="#FFFFFF"/>` +
    `<ellipse cx="160" cy="194" rx="14" ry="10" fill="#3A3450"/>` +
    `<path d="M 160 204 L 160 214 M 144 214 Q 160 228 176 214" stroke="#3A3450" stroke-width="5" fill="none" stroke-linecap="round"/>` +
    cheek(d, 98, 200, 12, 8) +
    cheek(d, 222, 200, 12, 8) +
    d.gloss(126, 106, 28, 10, 0.5)
  );
}

function bunny(d) {
  const w = '#F8F3FF';
  return (
    `<path d="M 116 150 C 92 100, 92 34, 120 30 C 148 28, 150 100, 146 150 Z" fill="${d.shade(w, { dark: 0.12 })}" stroke="#DCD3EE" stroke-width="3"/>` +
    `<path d="M 204 150 C 228 100, 228 34, 200 30 C 172 28, 170 100, 174 150 Z" fill="${d.shade(w, { dark: 0.12 })}" stroke="#DCD3EE" stroke-width="3"/>` +
    `<path d="M 120 140 C 106 104, 108 56, 120 50 C 134 50, 136 104, 134 140 Z" fill="#FFB3D1"/>` +
    `<path d="M 200 140 C 214 104, 212 56, 200 50 C 186 50, 184 104, 186 140 Z" fill="#FFB3D1"/>` +
    `<ellipse cx="160" cy="196" rx="86" ry="74" fill="${d.shade(w, { dark: 0.12 })}" stroke="#DCD3EE" stroke-width="3"/>` +
    eye(128, 188, 12) +
    eye(192, 188, 12) +
    `<path d="M 152 210 L 168 210 L 160 219 Z" fill="${palette.pink}" stroke="${palette.pink}" stroke-width="3" stroke-linejoin="round"/>` +
    `<path d="M 146 226 Q 153 234 160 226 Q 167 234 174 226" stroke="${ink}" stroke-width="3.5" fill="none" stroke-linecap="round"/>` +
    cheek(d, 108, 214, 11, 7) +
    cheek(d, 212, 214, 11, 7) +
    d.gloss(132, 146, 24, 10, 0.5)
  );
}

// ---------- Instruments ----------

function guitar(d) {
  const p = palette.purple;
  return (
    `<g transform="rotate(35 160 160)">` +
    `<rect x="148" y="34" width="24" height="130" rx="6" fill="${d.shade('#B57A3A')}"/>` +
    `<path d="M 140 18 L 180 18 L 184 50 L 136 50 Z" fill="${d.shade('#6B3A1E')}" stroke-linejoin="round"/>` +
    `<circle cx="134" cy="28" r="5" fill="#E0A800"/><circle cx="134" cy="42" r="5" fill="#E0A800"/><circle cx="186" cy="28" r="5" fill="#E0A800"/><circle cx="186" cy="42" r="5" fill="#E0A800"/>` +
    `<path d="M 160 140 C 120 140, 104 160, 114 186 C 84 200, 80 250, 110 276 C 136 298, 184 298, 210 276 C 240 250, 236 200, 206 186 C 216 160, 200 140, 160 140 Z" fill="${d.shade(p)}" stroke="${darker(p, 0.25)}" stroke-width="4"/>` +
    `<circle cx="160" cy="206" r="20" fill="#3A2266"/>` +
    `<rect x="138" y="246" width="44" height="12" rx="5" fill="#6B3A1E"/>` +
    `<path d="M 154 40 L 154 252 M 160 40 L 160 252 M 166 40 L 166 252" stroke="#FFF3B0" stroke-width="1.8"/>` +
    `<path d="M 122 166 Q 110 186 118 206" stroke="#FFFFFF" stroke-width="6" fill="none" stroke-linecap="round" opacity="0.6"/>` +
    `</g>`
  );
}

function violin(d) {
  const b = '#C2672A';
  return (
    `<g transform="rotate(-30 160 160)">` +
    `<rect x="151" y="40" width="18" height="110" rx="5" fill="${d.shade('#3A2416')}"/>` +
    `<circle cx="160" cy="36" r="13" fill="${d.shade(b)}" stroke="${darker(b, 0.3)}" stroke-width="3"/>` +
    `<path d="M 160 128 C 124 128, 112 150, 124 172 C 112 182, 112 200, 124 210 C 104 226, 106 274, 160 282 C 214 274, 216 226, 196 210 C 208 200, 208 182, 196 172 C 208 150, 196 128, 160 128 Z" fill="${d.shade(b)}" stroke="${darker(b, 0.3)}" stroke-width="4"/>` +
    `<path d="M 138 196 C 130 210, 146 222, 136 238 M 182 196 C 190 210, 174 222, 184 238" stroke="#3A2416" stroke-width="4" fill="none" stroke-linecap="round"/>` +
    `<rect x="146" y="234" width="28" height="8" rx="3" fill="#3A2416"/>` +
    `<path d="M 155 50 L 155 236 M 160 50 L 160 236 M 165 50 L 165 236" stroke="#FFF3B0" stroke-width="1.6"/>` +
    `<path d="M 130 150 Q 122 166 130 178" stroke="#FFFFFF" stroke-width="5" fill="none" stroke-linecap="round" opacity="0.55"/>` +
    `</g>` +
    `<path d="M 66 250 L 262 92" stroke="${d.shade('#8B5A2B')}" stroke-width="9" stroke-linecap="round"/>` +
    `<path d="M 72 254 L 266 98" stroke="#FFF8EC" stroke-width="3" stroke-linecap="round"/>`
  );
}

function piano(d) {
  const p = palette.purple;
  let keys = '';
  for (let i = 0; i < 8; i += 1) keys += `<rect x="${78 + i * 21}" y="176" width="19" height="52" rx="3" fill="#FFFFFF"/>`;
  for (const i of [0, 1, 3, 4, 5]) keys += `<rect x="${92 + i * 21}" y="176" width="12" height="30" rx="2" fill="${ink}"/>`;
  return (
    `<rect x="62" y="70" width="196" height="120" rx="22" fill="${d.shade(p)}" stroke="${darker(p, 0.25)}" stroke-width="4"/>` +
    `<rect x="84" y="90" width="152" height="60" rx="12" fill="${darker(p, 0.2)}"/>` +
    `<path d="M 122 132 L 122 104 L 146 98 L 146 126" stroke="#FFF3B0" stroke-width="4" fill="none" stroke-linejoin="round" opacity="0.85"/>` +
    `<ellipse cx="116" cy="133" rx="8" ry="6" fill="#FFF3B0" opacity="0.85"/><ellipse cx="140" cy="127" rx="8" ry="6" fill="#FFF3B0" opacity="0.85"/>` +
    sparkle(196, 118, 14, '#FFF3B0', 0.85) +
    `<rect x="62" y="166" width="196" height="72" rx="14" fill="${d.shade(p)}" stroke="${darker(p, 0.25)}" stroke-width="4"/>` +
    keys +
    `<rect x="72" y="236" width="16" height="40" rx="6" fill="${darker(p, 0.25)}"/><rect x="232" y="236" width="16" height="40" rx="6" fill="${darker(p, 0.25)}"/>` +
    d.gloss(100, 84, 30, 8, 0.5, -8)
  );
}

function drum(d) {
  const r = palette.pink;
  return (
    `<path d="M 66 126 L 66 230 Q 160 282 254 230 L 254 126 Z" fill="${d.shade(r, { cx: '30%', cy: '50%' })}" stroke="${darker(r, 0.25)}" stroke-width="4"/>` +
    `<path d="M 66 216 Q 160 268 254 216 L 254 238 Q 160 290 66 238 Z" fill="${d.shade(palette.yellow)}"/>` +
    `<path d="M 66 126 Q 160 178 254 126 L 254 148 Q 160 200 66 148 Z" fill="${d.shade(palette.yellow)}"/>` +
    `<path d="M 84 158 L 118 218 L 152 168 L 186 222 L 220 166 L 240 206" stroke="#FFFFFF" stroke-width="5" fill="none" stroke-linejoin="round" stroke-linecap="round"/>` +
    `<ellipse cx="160" cy="126" rx="94" ry="32" fill="${d.shade(palette.cream, { dark: 0.12 })}" stroke="${darker(palette.cream, 0.25)}" stroke-width="4"/>` +
    `<path d="M 100 40 L 156 116" stroke="#B57A3A" stroke-width="10" stroke-linecap="round"/><circle cx="100" cy="40" r="12" fill="${d.shade(palette.purple)}"/>` +
    `<path d="M 236 46 L 178 116" stroke="#B57A3A" stroke-width="10" stroke-linecap="round"/><circle cx="236" cy="46" r="12" fill="${d.shade(palette.purple)}"/>`
  );
}

function xylophone(d) {
  let bars = '';
  rainbow.forEach((c, i) => {
    const x = 70 + i * 32;
    const h = 150 - i * 14;
    const y = 160 - h / 2 + 10;
    bars +=
      `<rect x="${x}" y="${y}" width="26" height="${h}" rx="9" fill="${d.shade(c)}" stroke="${darker(c, 0.2)}" stroke-width="3"/>` +
      `<circle cx="${x + 13}" cy="${y + 14}" r="3.5" fill="${darker(c, 0.35)}"/><circle cx="${x + 13}" cy="${y + h - 14}" r="3.5" fill="${darker(c, 0.35)}"/>`;
  });
  return (
    `<path d="M 58 96 L 270 132 M 58 230 L 270 196" stroke="#B57A3A" stroke-width="12" stroke-linecap="round"/>` +
    bars +
    `<path d="M 230 270 L 290 220" stroke="#8B5A2B" stroke-width="7" stroke-linecap="round"/><circle cx="226" cy="274" r="14" fill="${d.shade(palette.pink)}"/>` +
    `<path d="M 196 286 L 268 252" stroke="#8B5A2B" stroke-width="7" stroke-linecap="round"/><circle cx="190" cy="288" r="14" fill="${d.shade(palette.blue)}"/>`
  );
}

function trumpet(d) {
  const g = '#F5B700';
  return (
    `<g transform="rotate(-15 160 160)">` +
    `<path d="M 44 150 L 200 150 L 200 172 L 44 172 Z" fill="${d.shade(g)}" stroke="${darker(g, 0.3)}" stroke-width="3"/>` +
    `<path d="M 70 172 Q 70 220 120 220 L 180 220 Q 210 220 210 190" stroke="${d.shade(g)}" stroke-width="18" fill="none"/>` +
    `<path d="M 196 150 L 268 108 Q 286 161 268 214 L 196 172 Z" fill="${d.shade(g)}" stroke="${darker(g, 0.3)}" stroke-width="3" stroke-linejoin="round"/>` +
    `<ellipse cx="270" cy="161" rx="14" ry="53" fill="${darker(g, 0.25)}"/>` +
    `<rect x="30" y="146" width="20" height="30" rx="6" fill="${d.shade(g)}" stroke="${darker(g, 0.3)}" stroke-width="3"/>` +
    `<rect x="110" y="110" width="14" height="40" rx="4" fill="${d.shade(g)}"/><rect x="134" y="110" width="14" height="40" rx="4" fill="${d.shade(g)}"/><rect x="158" y="110" width="14" height="40" rx="4" fill="${d.shade(g)}"/>` +
    `<circle cx="117" cy="106" r="9" fill="${palette.cream}"/><circle cx="141" cy="106" r="9" fill="${palette.cream}"/><circle cx="165" cy="106" r="9" fill="${palette.cream}"/>` +
    `<path d="M 60 156 L 190 156" stroke="#FFFFFF" stroke-width="4" stroke-linecap="round" opacity="0.6"/>` +
    `</g>`
  );
}

function tambourine(d) {
  let jingles = '';
  for (let i = 0; i < 6; i += 1) {
    const a = (i * 60 + 30) * (Math.PI / 180);
    const x = 160 + 92 * Math.cos(a);
    const y = 150 + 92 * Math.sin(a);
    jingles += `<ellipse cx="${x.toFixed(1)}" cy="${y.toFixed(1)}" rx="16" ry="12" fill="${d.shade('#C9D3E8', { dark: 0.2 })}" stroke="#8A97B4" stroke-width="2"/>`;
  }
  return (
    `<path d="M 206 230 C 222 262, 196 284, 214 300" stroke="${palette.pink}" stroke-width="10" fill="none" stroke-linecap="round"/>` +
    `<path d="M 222 222 C 252 246, 240 276, 266 288" stroke="${palette.blue}" stroke-width="10" fill="none" stroke-linecap="round"/>` +
    `<path d="M 230 206 C 262 220, 270 246, 292 248" stroke="${palette.yellow}" stroke-width="10" fill="none" stroke-linecap="round"/>` +
    `<circle cx="160" cy="150" r="104" fill="${d.shade(palette.orange)}" stroke="${darker(palette.orange, 0.3)}" stroke-width="4"/>` +
    `<circle cx="160" cy="150" r="80" fill="${d.shade(palette.cream, { dark: 0.12 })}"/>` +
    jingles +
    sparkle(160, 150, 26, palette.purple, 0.8) +
    d.gloss(122, 96, 26, 10, 0.5)
  );
}

function musicNote(d) {
  const p = palette.purple;
  return (
    `<path d="M 128 214 L 128 80 L 242 56 L 242 190" stroke="${darker(p, 0.1)}" stroke-width="14" fill="none" stroke-linejoin="round"/>` +
    `<path d="M 128 80 L 242 56 L 242 92 L 128 116 Z" fill="${d.shade(p)}" stroke="${darker(p, 0.2)}" stroke-width="3" stroke-linejoin="round"/>` +
    `<ellipse cx="102" cy="220" rx="38" ry="30" fill="${d.shade(palette.pink)}" stroke="${darker(palette.pink, 0.2)}" stroke-width="3" transform="rotate(-20 102 220)"/>` +
    `<ellipse cx="216" cy="196" rx="38" ry="30" fill="${d.shade(palette.blue)}" stroke="${darker(palette.blue, 0.2)}" stroke-width="3" transform="rotate(-20 216 196)"/>` +
    d.gloss(90, 206, 12, 6, 0.7) +
    d.gloss(204, 182, 12, 6, 0.7) +
    sparkle(76, 110, 20, palette.yellow)
  );
}

// ---------- Planets ----------

function ballPlanet(d, base, extra = '', faceColor = ink) {
  const clip = d.clip('<circle cx="160" cy="160" r="104"/>');
  return (
    `<circle cx="160" cy="160" r="104" fill="${d.shade(base)}" stroke="${darker(base, 0.25)}" stroke-width="4"/>` +
    `<g clip-path="${clip}">${extra}</g>` +
    face(d, 160, 160, 1.3, faceColor) +
    d.gloss(118, 104, 30, 14, 0.5)
  );
}

function planetGold(d) {
  const g = palette.gold;
  return (
    ballPlanet(
      d,
      g,
      `<path d="M 50 196 Q 160 236 270 186" stroke="${lighter(g, 0.3)}" stroke-width="18" fill="none" opacity="0.5"/>` +
        `<path d="M 54 120 Q 160 150 268 112" stroke="${darker(g, 0.12)}" stroke-width="12" fill="none" opacity="0.4"/>`,
      '#5A3A00',
    ) + sparkle(266, 254, 18, palette.sparkle)
  );
}

function planetPink(d) {
  const c = darker(palette.pink, 0.12);
  return ballPlanet(
    d,
    palette.pink,
    `<circle cx="206" cy="96" r="14" fill="${c}" opacity="0.5"/><circle cx="96" cy="214" r="12" fill="${c}" opacity="0.5"/><circle cx="226" cy="222" r="9" fill="${c}" opacity="0.5"/><circle cx="104" cy="112" r="7" fill="${c}" opacity="0.4"/>`,
    '#4A0F27',
  );
}

function planetGreen(d) {
  const leaf = darker(palette.green, 0.2);
  return ballPlanet(
    d,
    palette.green,
    `<path d="M 84 210 q 20 -26 44 -6 q -20 24 -44 6 Z" fill="${leaf}" opacity="0.55"/>` +
      `<path d="M 196 92 q 24 -20 44 4 q -24 18 -44 -4 Z" fill="${leaf}" opacity="0.55"/>` +
      `<path d="M 200 228 q 16 -18 34 -2 q -16 16 -34 2 Z" fill="${leaf}" opacity="0.55"/>`,
    '#0E4A26',
  );
}

function planetPurpleRinged(d) {
  const p = palette.purple;
  return (
    `<path d="M 34 180 A 132 40 -14 0 1 286 120" stroke="${d.shade('#FFD93D')}" stroke-width="16" fill="none" stroke-linecap="round"/>` +
    ballPlanet(d, p, `<path d="M 50 150 Q 160 190 270 140" stroke="${lighter(p, 0.25)}" stroke-width="14" fill="none" opacity="0.4"/>`, '#2E1868') +
    `<path d="M 286 120 A 132 40 -14 0 1 34 180" stroke="${d.shade('#FFD93D')}" stroke-width="16" fill="none" stroke-linecap="round" transform="translate(0 0)" clip-path="${d.clip('<rect x="0" y="150" width="320" height="170"/>')}"/>` +
    sparkle(64, 70, 14, palette.sparkle) +
    sparkle(262, 262, 10, palette.sparkle)
  );
}

function earthSticker(d) {
  const clip = d.clip('<circle cx="160" cy="160" r="104"/>');
  return (
    `<circle cx="160" cy="160" r="104" fill="${d.shade(palette.blue)}" stroke="${darker(palette.blue, 0.25)}" stroke-width="4"/>` +
    `<g clip-path="${clip}">` +
    `<path d="M 66 110 C 92 80, 132 90, 136 116 C 140 140, 112 146, 104 168 C 94 190, 66 182, 58 154 Z" fill="${d.shade(palette.green)}"/>` +
    `<path d="M 178 70 C 214 66, 250 90, 256 124 C 232 130, 214 118, 196 124 C 178 128, 168 98, 178 70 Z" fill="${d.shade(palette.green)}"/>` +
    `<path d="M 168 214 C 196 196, 236 204, 244 230 C 232 260, 196 268, 178 256 C 160 242, 150 224, 168 214 Z" fill="${d.shade(palette.green)}"/>` +
    `<path d="M 50 236 Q 90 220 128 240 Q 98 252 60 246 Z" fill="#FFFFFF" opacity="0.85"/>` +
    `</g>` +
    face(d, 160, 160, 1.2, '#0C2F4D') +
    d.gloss(118, 104, 30, 14, 0.45)
  );
}

function moonCrescent(d) {
  const m = '#FFE9A8';
  return (
    `<path d="M 190 46 A 116 116 0 1 0 190 274 A 92 92 0 1 1 190 46 Z" fill="${d.shade(m, { light: 0.5, dark: 0.22 })}" stroke="#E0B84A" stroke-width="4" stroke-linejoin="round"/>` +
    `<circle cx="104" cy="214" r="10" fill="${darker(m, 0.1)}" opacity="0.6"/><circle cx="86" cy="120" r="7" fill="${darker(m, 0.1)}" opacity="0.6"/>` +
    happyEye(110, 152, 12, '#6B4A00', 5) +
    `<path d="M 104 186 Q 116 198 130 188" stroke="#6B4A00" stroke-width="5" fill="none" stroke-linecap="round"/>` +
    cheek(d, 90, 176, 10, 6) +
    `<path d="${starPath(228, 160, 36, 17)}" fill="${d.shade(palette.yellow)}" stroke="#E0A800" stroke-width="4" stroke-linejoin="round"/>` +
    d.gloss(116, 82, 22, 10, 0.5, -60)
  );
}

function sun(d) {
  let rays = '';
  for (let i = 0; i < 10; i += 1) {
    rays += `<path d="M 160 26 Q 174 52 160 70 Q 146 52 160 26 Z" fill="${d.shade(palette.orange)}" transform="rotate(${i * 36} 160 160)"/>`;
  }
  return (
    rays +
    `<circle cx="160" cy="160" r="82" fill="${d.shade(palette.yellow, { light: 0.6 })}" stroke="#E0A800" stroke-width="4"/>` +
    face(d, 160, 156, 1.15, '#6B4A00', { happy: true }) +
    d.gloss(126, 116, 24, 12, 0.6)
  );
}

// ---------- Space things ----------

function rocketSticker(d) {
  const bodyPath = 'M 160 34 C 206 70, 214 150, 202 220 L 118 220 C 106 150, 114 70, 160 34 Z';
  const clip = d.clip(`<path d="${bodyPath}"/>`);
  return (
    `<g transform="rotate(30 160 160)">` +
    `<path d="M 142 220 Q 160 300 178 220 Z" fill="${d.shade(palette.orange)}"/><path d="M 150 220 Q 160 270 170 220 Z" fill="${palette.yellow}"/>` +
    `<path d="M 122 176 C 94 192, 86 226, 88 262 L 126 238 Z" fill="${d.shade(palette.purple)}"/>` +
    `<path d="M 198 176 C 226 192, 234 226, 232 262 L 194 238 Z" fill="${d.shade(palette.purple)}"/>` +
    `<path d="${bodyPath}" fill="${d.shade('#F2F4FA', { light: 0.6, dark: 0.18 })}" stroke="#C9D3E8" stroke-width="3"/>` +
    `<g clip-path="${clip}"><rect x="0" y="0" width="320" height="96" fill="${d.shade(palette.pink)}"/><rect x="0" y="196" width="320" height="24" fill="#C9D3E8"/></g>` +
    `<circle cx="160" cy="138" r="28" fill="#C9D3E8"/><circle cx="160" cy="138" r="21" fill="${d.shade(palette.blue, { light: 0.5 })}"/>` +
    d.gloss(152, 130, 8, 5, 0.85) +
    `</g>`
  );
}

function astronautHelmet(d) {
  return (
    `<rect x="84" y="236" width="152" height="40" rx="18" fill="${d.shade('#C9D3E8', { dark: 0.2 })}"/>` +
    `<circle cx="160" cy="150" r="106" fill="${d.shade('#F2F4FA', { light: 0.6, dark: 0.2 })}" stroke="#C9D3E8" stroke-width="4"/>` +
    `<rect x="84" y="92" width="152" height="112" rx="52" fill="${d.shade('#2B3E78', { light: 0.35, dark: 0.3 })}"/>` +
    `<path d="M 104 118 Q 120 100 150 100" stroke="#FFFFFF" stroke-width="9" fill="none" stroke-linecap="round" opacity="0.75"/>` +
    `<circle cx="210" cy="180" r="6" fill="#FFFFFF" opacity="0.6"/>` +
    `<path d="M 160 236 C 136 220, 128 204, 140 196 C 150 190, 160 200, 160 206 C 160 200, 170 190, 180 196 C 192 204, 184 220, 160 236 Z" fill="${d.shade(palette.pink)}" stroke="${darker(palette.pink, 0.2)}" stroke-width="2"/>` +
    `<circle cx="58" cy="150" r="14" fill="${d.shade(palette.purple)}"/><circle cx="262" cy="150" r="14" fill="${d.shade(palette.purple)}"/>`
  );
}

function comet(d) {
  let tail = '';
  rainbow.forEach((c, i) => {
    const o = (i - 2.5) * 13;
    tail += `<path d="M ${206 + o * 0.4} ${116 + o} Q 120 ${170 + o * 1.4} ${42 + o * 0.6} ${262 + o * 0.3}" stroke="${c}" stroke-width="13" fill="none" stroke-linecap="round"/>`;
  });
  return (
    tail +
    `<circle cx="222" cy="104" r="52" fill="${d.shade(palette.yellow, { light: 0.7 })}" stroke="#E0A800" stroke-width="4"/>` +
    face(d, 222, 100, 0.9, '#6B4A00') +
    d.gloss(204, 80, 16, 8, 0.6)
  );
}

function telescope(d) {
  return (
    `<path d="M 150 196 L 96 286 M 160 196 L 160 290 M 170 196 L 224 286" stroke="${d.shade('#8B5A2B')}" stroke-width="10" stroke-linecap="round"/>` +
    `<g transform="rotate(-32 160 170)">` +
    `<rect x="70" y="148" width="184" height="46" rx="18" fill="${d.shade(palette.purple)}" stroke="${darker(palette.purple, 0.25)}" stroke-width="4"/>` +
    `<rect x="236" y="138" width="34" height="66" rx="12" fill="${d.shade(palette.blue)}" stroke="${darker(palette.blue, 0.25)}" stroke-width="4"/>` +
    `<rect x="44" y="156" width="34" height="30" rx="10" fill="${d.shade(palette.blue)}" stroke="${darker(palette.blue, 0.25)}" stroke-width="4"/>` +
    `<rect x="140" y="146" width="16" height="50" fill="${palette.yellow}"/>` +
    `<path d="M 92 160 L 220 160" stroke="#FFFFFF" stroke-width="5" stroke-linecap="round" opacity="0.5"/>` +
    `</g>` +
    `<circle cx="160" cy="196" r="12" fill="${d.shade('#6B3A1E')}"/>` +
    `<path d="${starPath(258, 58, 30, 14)}" fill="${d.shade(palette.yellow)}" stroke="#E0A800" stroke-width="4" stroke-linejoin="round"/>`
  );
}

function ufoFriendly(d) {
  const g = palette.green;
  return (
    `<ellipse cx="160" cy="112" rx="30" ry="28" fill="${d.shade(g)}"/>` +
    `<path d="M 148 88 L 140 68 M 172 88 L 180 68" stroke="${darker(g, 0.2)}" stroke-width="4" stroke-linecap="round"/><circle cx="140" cy="66" r="6" fill="${palette.pink}"/><circle cx="180" cy="66" r="6" fill="${palette.pink}"/>` +
    eye(149, 108, 6.5, '#0E4A26') +
    eye(171, 108, 6.5, '#0E4A26') +
    `<path d="M 151 123 Q 160 130 169 123" stroke="#0E4A26" stroke-width="3" fill="none" stroke-linecap="round"/>` +
    cheek(d, 136, 120, 6, 4) +
    cheek(d, 184, 120, 6, 4) +
    `<path d="M 104 160 C 104 76, 216 76, 216 160 Z" fill="#DDF1FF" opacity="0.4" stroke="#FFFFFF" stroke-width="4"/>` +
    `<path d="M 118 116 Q 128 98 146 94" stroke="#FFFFFF" stroke-width="5" fill="none" stroke-linecap="round" opacity="0.85"/>` +
    `<ellipse cx="160" cy="178" rx="124" ry="42" fill="${d.shade(palette.purple, { cy: '20%' })}" stroke="${darker(palette.purple, 0.25)}" stroke-width="4"/>` +
    `<ellipse cx="160" cy="162" rx="90" ry="16" fill="${lighter(palette.purple, 0.35)}"/>` +
    `<circle cx="88" cy="188" r="9" fill="${palette.yellow}"/><circle cx="134" cy="200" r="9" fill="${palette.pink}"/><circle cx="186" cy="200" r="9" fill="${palette.blue}"/><circle cx="232" cy="188" r="9" fill="${palette.yellow}"/>`
  );
}

function starShooting(d) {
  return (
    `<path d="M 160 170 L 40 268" stroke="${palette.pink}" stroke-width="14" stroke-linecap="round" opacity="0.85"/>` +
    `<path d="M 168 190 L 66 290" stroke="${palette.blue}" stroke-width="10" stroke-linecap="round" opacity="0.85"/>` +
    `<path d="M 144 158 L 30 226" stroke="${palette.purple}" stroke-width="10" stroke-linecap="round" opacity="0.85"/>` +
    `<path d="${starPath(196, 132, 96, 46)}" fill="${d.shade(palette.yellow, { light: 0.6 })}" stroke="#E0A800" stroke-width="10" stroke-linejoin="round"/>` +
    face(d, 196, 136, 0.95, '#6B4A00') +
    d.gloss(170, 96, 16, 8, 0.6)
  );
}

function satellite(d) {
  const panel = (x) => {
    let grid = '';
    for (let i = 1; i < 3; i += 1) grid += `<path d="M ${x + i * 24} 132 L ${x + i * 24} 188" stroke="#BFE3FF" stroke-width="2"/>`;
    return (
      `<rect x="${x}" y="132" width="72" height="56" rx="6" fill="${d.shade('#2B6FCF')}" stroke="#114A78" stroke-width="3"/>` +
      grid +
      `<path d="M ${x} 160 L ${x + 72} 160" stroke="#BFE3FF" stroke-width="2"/>`
    );
  };
  return (
    `<g transform="rotate(-18 160 160)">` +
    `<path d="M 110 160 L 210 160" stroke="#8A97B4" stroke-width="8"/>` +
    panel(34) +
    panel(214) +
    `<rect x="122" y="122" width="76" height="76" rx="18" fill="${d.shade(palette.yellow)}" stroke="#E0A800" stroke-width="4"/>` +
    `<path d="M 160 122 L 160 88" stroke="#8A97B4" stroke-width="6"/>` +
    `<path d="M 130 88 Q 160 112 190 88 Z" fill="${d.shade('#F2F4FA', { dark: 0.2 })}" stroke="#C9D3E8" stroke-width="3"/>` +
    `<circle cx="160" cy="76" r="7" fill="${palette.pink}"/>` +
    face(d, 160, 156, 0.75) +
    `</g>`
  );
}

const stickerList = [
  ['cat-purple', catPurple],
  ['unicorn', unicorn],
  ['whale', whale],
  ['fox', fox],
  ['owl', owl],
  ['puppy', puppy],
  ['panda', panda],
  ['bunny', bunny],
  ['guitar', guitar],
  ['violin', violin],
  ['piano', piano],
  ['drum', drum],
  ['xylophone', xylophone],
  ['trumpet', trumpet],
  ['tambourine', tambourine],
  ['music-note', musicNote],
  ['planet-gold', planetGold],
  ['planet-pink', planetPink],
  ['planet-green', planetGreen],
  ['planet-purple-ringed', planetPurpleRinged],
  ['earth', earthSticker],
  ['moon-crescent', moonCrescent],
  ['sun', sun],
  ['rocket', rocketSticker],
  ['astronaut-helmet', astronautHelmet],
  ['comet', comet],
  ['telescope', telescope],
  ['ufo-friendly', ufoFriendly],
  ['star-shooting', starShooting],
  ['satellite', satellite],
];

/** The ids of the 30 stickers, in the sticker book's order (stable: never renamed or reused). */
export const stickerIds = stickerList.map(([id]) => id);

// ---------- Constellations ----------

const constellations = {
  unicorn: {
    stars: [[470, 56], [418, 150], [356, 146], [282, 182], [196, 250], [190, 336], [270, 360], [344, 304], [380, 214]],
    lines: [[0, 1], [1, 2], [2, 3], [3, 4], [4, 5], [5, 6], [6, 7], [7, 8], [8, 1]],
  },
  rocket: {
    stars: [[320, 50], [262, 168], [378, 168], [262, 318], [378, 318], [196, 404], [444, 404], [320, 432]],
    lines: [[0, 1], [0, 2], [1, 3], [2, 4], [3, 5], [4, 6], [3, 7], [7, 4]],
  },
  cat: {
    stars: [[226, 78], [270, 142], [350, 142], [394, 78], [384, 224], [424, 400], [216, 400], [236, 224], [530, 320]],
    lines: [[0, 1], [1, 2], [2, 3], [3, 4], [4, 5], [5, 6], [6, 7], [7, 0], [5, 8]],
  },
};

/** The star places of each constellation, in its 640 × 480 picture (for lighting them later). */
export function constellationStars() {
  return Object.fromEntries(Object.entries(constellations).map(([id, c]) => [id, c.stars.map((s) => [...s])]));
}

function emptyRing(x, y, r = 16) {
  return `<circle cx="${x}" cy="${y}" r="${r}" fill="none" stroke="${palette.sparkle}" stroke-width="3.5" stroke-dasharray="5 6" stroke-linecap="round" opacity="0.85"/>`;
}

/** A constellation's faint figure: lilac lines joining its star places, each a dotted ring (640 × 480). */
function constellation(id) {
  const { stars, lines } = constellations[id];
  const d = new Drawing(640, 480, `cn${id}`);
  const lineMarkup = lines
    .map(([a, b]) => `<path d="M ${stars[a][0]} ${stars[a][1]} L ${stars[b][0]} ${stars[b][1]}" stroke="#D8CCFF" stroke-width="4" stroke-linecap="round" opacity="0.35"/>`)
    .join('');
  return d.render(lineMarkup + stars.map(([x, y]) => emptyRing(x, y)).join(''));
}

/** A lit star: glowing gold with a soft halo (96 × 96). */
function starLit() {
  const d = new Drawing(96, 96, 'sl');
  return d.render(
    `<circle cx="48" cy="48" r="34" fill="${palette.yellow}" opacity="0.45" filter="${d.blur(9)}"/>` +
      `<path d="${starPath(48, 50, 34, 16)}" fill="${d.shade(palette.yellow, { light: 0.65 })}" stroke="#E0A800" stroke-width="4" stroke-linejoin="round"/>` +
      d.gloss(40, 38, 7, 4, 0.7),
  );
}

/** An empty star place: a dotted ring (96 × 96). */
function starEmpty() {
  const d = new Drawing(96, 96, 'se');
  return d.render(emptyRing(48, 48, 30));
}

/** Every sticker and constellation sprite, by its path under Art/. */
export function sprites() {
  const list = stickerList.map(([id, draw]) => ({ path: `Stickers/sticker-${id}`, width: 320, height: 320, svg: sticker(id, draw) }));
  for (const id of Object.keys(constellations)) {
    list.push({ path: `Constellations/constellation-${id}`, width: 640, height: 480, svg: constellation(id) });
  }
  list.push({ path: 'Constellations/star-lit', width: 96, height: 96, svg: starLit() });
  list.push({ path: 'Constellations/star-empty', width: 96, height: 96, svg: starEmpty() });
  return list;
}
