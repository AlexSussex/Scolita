// The language badges (decision D6): Romanian is a gold five-pointed star with "RO",
// English a blue ringed planet with "EN". Told apart by shape first, colour second.

import { Drawing, palette, lighter, darker, starPath, label } from '../lib/svg.mjs';

/** The Romanian star badge (128 × 128). */
function romanian() {
  const d = new Drawing(128, 128, 'ro');
  const fill = palette.roGold;
  const path = starPath(64, 68, 58, 58 * 0.46);
  const body =
    `<path d="${path}" fill="#000000" opacity="0.25" transform="translate(3 4)" filter="${d.blur(2.5)}"/>` +
    `<path d="${path}" fill="${d.shade(fill, { light: 0.55, dark: 0.15 })}" stroke="${palette.roOutline}" stroke-width="5" stroke-linejoin="round"/>` +
    d.gloss(46, 46, 12, 6, 0.6) +
    label(64, 82, 34, 'RO', palette.roText);
  return d.render(body);
}

/** The English ringed-planet badge (128 × 128). */
function english() {
  const d = new Drawing(128, 128, 'en');
  const r = 37;
  const ringRx = 60;
  const ringRy = 15;
  const ring = (clipTop) =>
    `<g clip-path="${d.clip(clipTop ? '<rect x="0" y="0" width="128" height="64"/>' : '<rect x="0" y="64" width="128" height="64"/>')}">` +
    `<ellipse cx="64" cy="64" rx="${ringRx}" ry="${ringRy}" fill="none" stroke="${palette.enOutline}" stroke-width="7" transform="rotate(-18 64 64)"/>` +
    `<ellipse cx="64" cy="64" rx="${ringRx}" ry="${ringRy}" fill="none" stroke="${lighter(palette.enOutline, 0.35)}" stroke-width="2" transform="rotate(-18 64 64)"/>` +
    `</g>`;
  const ballClip = d.clip(`<circle cx="64" cy="64" r="${r}"/>`);
  const body =
    `<circle cx="67" cy="68" r="${r}" fill="#000000" opacity="0.25" filter="${d.blur(2.5)}"/>` +
    ring(true) +
    `<circle cx="64" cy="64" r="${r}" fill="${d.shade(palette.enBlue, { light: 0.5, dark: 0.2 })}"/>` +
    `<g clip-path="${ballClip}"><path d="M 0 56 Q 64 40 128 50 L 128 64 Q 64 56 0 72 Z" fill="${palette.enBand}" opacity="0.85"/></g>` +
    `<circle cx="64" cy="64" r="${r}" fill="none" stroke="${palette.enOutline}" stroke-width="4"/>` +
    ring(false) +
    d.gloss(50, 46, 10, 5, 0.6) +
    label(64, 76, 30, 'EN', '#FFFFFF', { extra: `stroke="${palette.enOutline}" stroke-width="5" paint-order="stroke" stroke-linejoin="round"` });
  return d.render(body);
}

/** The badge sprites, by language code. */
export function sprites() {
  return [
    { path: 'Badges/badge-ro', width: 128, height: 128, svg: romanian() },
    { path: 'Badges/badge-en', width: 128, height: 128, svg: english() },
  ];
}
