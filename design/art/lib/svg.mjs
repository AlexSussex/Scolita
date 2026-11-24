// Small SVG toolkit for Școlița's art: colours, the top-left light, soft shadows.
// Every drawing is a function returning an SVG string; build.mjs renders them to PNG.

export const ink = '#2A2140';

export const palette = {
  skyTop: '#2B1B5A',
  skyBottom: '#4B2C8F',
  sparkle: '#FFF3B0',
  pink: '#FF5C8A',
  orange: '#FF9F43',
  yellow: '#FFD93D',
  green: '#4CD97B',
  blue: '#3BA7F0',
  purple: '#9B6CFF',
  gold: '#F5B700',
  cream: '#FFF8EC',
  slate: '#24324A',
  slateRim: '#3A4C6C',
  lines: '#E8EEF8',
  cheek: '#FF9FB8',
  roGold: '#F5B700',
  roOutline: '#7A4D00',
  roText: '#3A2600',
  enBlue: '#3BA7F0',
  enOutline: '#114A78',
  enBand: '#BFE3FF',
};

function hexToRgb(hex) {
  const h = hex.replace('#', '');
  return [0, 2, 4].map((i) => parseInt(h.slice(i, i + 2), 16));
}

function rgbToHex(rgb) {
  return '#' + rgb.map((v) => Math.max(0, Math.min(255, Math.round(v))).toString(16).padStart(2, '0')).join('').toUpperCase();
}

/** Mixes two colours: t = 0 gives a, t = 1 gives b. */
export function mix(a, b, t) {
  const x = hexToRgb(a);
  const y = hexToRgb(b);
  return rgbToHex(x.map((v, i) => v + (y[i] - v) * t));
}

export const lighter = (c, t) => mix(c, '#FFFFFF', t);
export const darker = (c, t) => mix(c, '#000000', t);

/** Relative luminance, for the greyscale check of the badges. */
export function luminance(hex) {
  const [r, g, b] = hexToRgb(hex).map((v) => {
    const s = v / 255;
    return s <= 0.03928 ? s / 12.92 : Math.pow((s + 0.055) / 1.055, 2.4);
  });
  return 0.2126 * r + 0.7152 * g + 0.0722 * b;
}

/**
 * A drawing in progress: collects gradient and filter definitions with unique ids,
 * so shapes can ask for "this colour, lit from the top left".
 */
export class Drawing {
  constructor(width, height, prefix = 'd') {
    this.width = width;
    this.height = height;
    this.prefix = prefix;
    this.defs = [];
    this.count = 0;
    this.cache = new Map();
  }

  id(kind) {
    this.count += 1;
    return `${this.prefix}${kind}${this.count}`;
  }

  /** A radial fill lit from the top left: a pale highlight, the colour, a darker edge. */
  shade(base, { light = 0.55, dark = 0.28, cx = '35%', cy = '28%', r = '75%' } = {}) {
    const key = `shade:${base}:${light}:${dark}:${cx}:${cy}:${r}`;
    if (this.cache.has(key)) return this.cache.get(key);
    const id = this.id('s');
    this.defs.push(
      `<radialGradient id="${id}" cx="${cx}" cy="${cy}" r="${r}">` +
        `<stop offset="0" stop-color="${lighter(base, light)}"/>` +
        `<stop offset="0.45" stop-color="${base}"/>` +
        `<stop offset="1" stop-color="${darker(base, dark)}"/>` +
        `</radialGradient>`,
    );
    const url = `url(#${id})`;
    this.cache.set(key, url);
    return url;
  }

  /** A vertical fill from top to bottom. */
  linear(stops, { x1 = 0, y1 = 0, x2 = 0, y2 = 1 } = {}) {
    const key = `lin:${JSON.stringify(stops)}:${x1}${y1}${x2}${y2}`;
    if (this.cache.has(key)) return this.cache.get(key);
    const id = this.id('l');
    const s = stops
      .map(([o, c, a = 1]) => `<stop offset="${o}" stop-color="${c}" stop-opacity="${a}"/>`)
      .join('');
    this.defs.push(`<linearGradient id="${id}" x1="${x1}" y1="${y1}" x2="${x2}" y2="${y2}">${s}</linearGradient>`);
    const url = `url(#${id})`;
    this.cache.set(key, url);
    return url;
  }

  /** A radial fill from explicit stops. */
  radial(stops, { cx = '50%', cy = '50%', r = '50%', fx, fy } = {}) {
    const key = `rad:${JSON.stringify(stops)}:${cx}${cy}${r}${fx}${fy}`;
    if (this.cache.has(key)) return this.cache.get(key);
    const id = this.id('r');
    const s = stops
      .map(([o, c, a = 1]) => `<stop offset="${o}" stop-color="${c}" stop-opacity="${a}"/>`)
      .join('');
    const f = fx !== undefined ? ` fx="${fx}" fy="${fy}"` : '';
    this.defs.push(`<radialGradient id="${id}" cx="${cx}" cy="${cy}" r="${r}"${f}>${s}</radialGradient>`);
    const url = `url(#${id})`;
    this.cache.set(key, url);
    return url;
  }

  /** A Gaussian blur filter, for soft shadows and glows. */
  blur(std) {
    const key = `blur:${std}`;
    if (this.cache.has(key)) return this.cache.get(key);
    const id = this.id('b');
    this.defs.push(
      `<filter id="${id}" x="-50%" y="-50%" width="200%" height="200%"><feGaussianBlur stdDeviation="${std}"/></filter>`,
    );
    const url = `url(#${id})`;
    this.cache.set(key, url);
    return url;
  }

  /** A clip path from the given shape markup. */
  clip(shape) {
    const id = this.id('c');
    this.defs.push(`<clipPath id="${id}">${shape}</clipPath>`);
    return `url(#${id})`;
  }

  /** A soft oval shadow under a thing, down and to the right of the light. */
  groundShadow(cx, cy, rx, ry, opacity = 0.22) {
    return `<ellipse cx="${cx}" cy="${cy}" rx="${rx}" ry="${ry}" fill="#000000" opacity="${opacity}" filter="${this.blur(Math.max(1, ry * 0.35))}"/>`;
  }

  /** A gloss highlight: a soft white oval up and to the left. */
  gloss(cx, cy, rx, ry, opacity = 0.55, rotate = -30) {
    return `<ellipse cx="${cx}" cy="${cy}" rx="${rx}" ry="${ry}" fill="#FFFFFF" opacity="${opacity}" transform="rotate(${rotate} ${cx} ${cy})" filter="${this.blur(Math.max(0.6, ry * 0.25))}"/>`;
  }

  /** The finished SVG document. */
  render(body, viewBox = `0 0 ${this.width} ${this.height}`) {
    return (
      `<svg xmlns="http://www.w3.org/2000/svg" width="${this.width}" height="${this.height}" viewBox="${viewBox}">` +
      `<defs>${this.defs.join('')}</defs>${body}</svg>`
    );
  }
}

/** A four-pointed sparkle centred on (x, y). */
export function sparkle(x, y, r, fill = palette.sparkle, opacity = 1) {
  const k = r * 0.28;
  return `<path d="M ${x} ${y - r} Q ${x + k} ${y - k} ${x + r} ${y} Q ${x + k} ${y + k} ${x} ${y + r} Q ${x - k} ${y + k} ${x - r} ${y} Q ${x - k} ${y - k} ${x} ${y - r} Z" fill="${fill}" opacity="${opacity}"/>`;
}

/** A five-pointed star's path centred on (x, y), outer radius r, inner radius ri. */
export function starPath(x, y, r, ri = r * 0.45, rotation = -90) {
  const points = [];
  for (let i = 0; i < 10; i += 1) {
    const rad = ((rotation + i * 36) * Math.PI) / 180;
    const rr = i % 2 === 0 ? r : ri;
    points.push(`${(x + rr * Math.cos(rad)).toFixed(2)} ${(y + rr * Math.sin(rad)).toFixed(2)}`);
  }
  return `M ${points.join(' L ')} Z`;
}

/** A five-pointed star with rounded corners (drawn with a round stroke of its own colour). */
export function roundStar(x, y, r, fill, { ri = r * 0.5, stroke = fill, strokeWidth = r * 0.18, extra = '' } = {}) {
  return `<path d="${starPath(x, y, r, ri)}" fill="${fill}" stroke="${stroke}" stroke-width="${strokeWidth}" stroke-linejoin="round" ${extra}/>`;
}

/** Eyes with a white highlight, the face's ink. */
export function eye(x, y, r, color = ink) {
  return (
    `<circle cx="${x}" cy="${y}" r="${r}" fill="${color}"/>` +
    `<circle cx="${x + r * 0.35}" cy="${y - r * 0.35}" r="${r * 0.38}" fill="#FFFFFF"/>` +
    `<circle cx="${x - r * 0.35}" cy="${y + r * 0.4}" r="${r * 0.15}" fill="#FFFFFF" opacity="0.8"/>`
  );
}

/** A closed, happy eye: an arc bending up. */
export function happyEye(x, y, w, color = ink, width = w * 0.32) {
  return `<path d="M ${x - w} ${y + w * 0.3} Q ${x} ${y - w * 0.9} ${x + w} ${y + w * 0.3}" stroke="${color}" stroke-width="${width}" fill="none" stroke-linecap="round"/>`;
}

/** A closed, sleepy eye: an arc bending down. */
export function sleepyEye(x, y, w, color = ink, width = w * 0.3) {
  return `<path d="M ${x - w} ${y - w * 0.2} Q ${x} ${y + w * 0.7} ${x + w} ${y - w * 0.2}" stroke="${color}" stroke-width="${width}" fill="none" stroke-linecap="round"/>`;
}

/** Soft pink cheeks. */
export function cheek(d, x, y, rx, ry, opacity = 0.75) {
  return `<ellipse cx="${x}" cy="${y}" rx="${rx}" ry="${ry}" fill="${palette.cheek}" opacity="${opacity}" filter="${d.blur(rx * 0.18)}"/>`;
}

/** Text for the badges and the z z z, in Baloo 2 (embedded by build.mjs). */
export function label(x, y, size, text, fill, { weight = 800, anchor = 'middle', extra = '' } = {}) {
  return `<text x="${x}" y="${y}" font-family="Baloo2" font-weight="${weight}" font-size="${size}" fill="${fill}" text-anchor="${anchor}" ${extra}>${text}</text>`;
}
