// Cuts a sprite sheet made with an image tool (ChatGPT, as in local/art-prompts) into clean sprites:
// keys out the flat green #00FF00 (or magenta #FF00FF) background with a soft edge, removes the
// green (or magenta) spill from the edges, finds each part (a sparkle or a bubble next to a
// character stays with it), and draws every part at the same scale onto a square, so the parts of
// one sheet keep their sizes relative to each other.
//
//   node design/art/cut-sheet.mjs <sheet.png> <out folder> [options]
//     --key green|magenta      the background colour (default green)
//     --size 384               the square's side in pixels (default 384)
//     --align bottom|center    characters stand on the square's bottom edge; objects are centred
//     --names a,b,c            names for the parts in reading order (default part-1, part-2, ...)
//     --merge 12               how close (in sheet pixels) two blobs must be to count as one part
//     --grid 4x2               the sheet is a grid of cells (columns x rows): one part per cell, empty cells skipped,
//                              small slivers of a neighbour reaching over the cell's edge left out
//     --background             the whole sheet is one picture (a sky): no keying, scaled to --size wide
//
// Raw sheets stay in local/art-inbox; only the cleaned parts are committed.

import { readFileSync } from 'node:fs';
import { basename, join, resolve } from 'node:path';
import { runInChrome, writeFile } from './lib/render.mjs';

const cutScript = `
const load = (png) => new Promise((ok, fail) => {
  const img = new Image();
  img.onload = () => ok(img);
  img.onerror = () => fail(new Error('The sheet is not a readable PNG.'));
  img.src = 'data:image/png;base64,' + png;
});

const keyOut = (p, green) => {
  for (let i = 0; i < p.length; i += 4) {
    const r = p[i], g = p[i + 1], b = p[i + 2];
    const keyness = green ? g - Math.max(r, b) : Math.min(r, b) - g;
    const a = Math.max(0, Math.min(1, 1 - (keyness - 40) / 90));
    if (keyness > 0) {
      if (green) p[i + 1] = Math.max(r, b);
      else { p[i] = r - keyness; p[i + 2] = b - keyness; }
    }
    p[i + 3] = Math.round(p[i + 3] * a);
  }
};

const solidAt = (p, W, x, y) => p[(y * W + x) * 4 + 3] > 64;

const findBlobs = (p, W, rect, merge) => {
  const cell = 4;
  const gw = Math.ceil(rect.w / cell), gh = Math.ceil(rect.h / cell);
  const solid = new Uint8Array(gw * gh);
  for (let y = rect.y; y < rect.y + rect.h; y++) for (let x = rect.x; x < rect.x + rect.w; x++) {
    if (solidAt(p, W, x, y)) solid[((y - rect.y) / cell | 0) * gw + ((x - rect.x) / cell | 0)] = 1;
  }
  const reach = Math.max(1, Math.round(merge / cell));
  const grown = new Uint8Array(gw * gh);
  for (let y = 0; y < gh; y++) for (let x = 0; x < gw; x++) if (solid[y * gw + x]) {
    for (let dy = -reach; dy <= reach; dy++) for (let dx = -reach; dx <= reach; dx++) {
      const yy = y + dy, xx = x + dx;
      if (yy >= 0 && yy < gh && xx >= 0 && xx < gw) grown[yy * gw + xx] = 1;
    }
  }
  const label = new Uint8Array(gw * gh);
  const blobs = [];
  for (let s = 0; s < gw * gh; s++) {
    if (!grown[s] || label[s]) continue;
    let minX = 1e9, minY = 1e9, maxX = -1, maxY = -1, count = 0;
    const stack = [s];
    label[s] = 1;
    while (stack.length) {
      const q = stack.pop();
      const x = q % gw, y = (q / gw) | 0;
      if (solid[q]) { count++; minX = Math.min(minX, x); maxX = Math.max(maxX, x); minY = Math.min(minY, y); maxY = Math.max(maxY, y); }
      const next = [x > 0 ? q - 1 : -1, x < gw - 1 ? q + 1 : -1, q - gw, q + gw];
      for (const n of next) {
        if (n < 0 || n >= gw * gh || label[n] || !grown[n]) continue;
        label[n] = 1; stack.push(n);
      }
    }
    if (maxX < 0) continue;
    const edge = minX === 0 || minY === 0 || maxX === gw - 1 || maxY === gh - 1;
    blobs.push({ x: rect.x + minX * cell, y: rect.y + minY * cell, w: (maxX - minX + 1) * cell, h: (maxY - minY + 1) * cell, area: count * cell * cell, edge });
  }
  return blobs;
};

const union = (blobs) => {
  const x = Math.min(...blobs.map((q) => q.x)), y = Math.min(...blobs.map((q) => q.y));
  const x1 = Math.max(...blobs.map((q) => q.x + q.w)), y1 = Math.max(...blobs.map((q) => q.y + q.h));
  return { x, y, w: x1 - x, h: y1 - y };
};

const blobParts = (p, W, H, merge) => {
  const parts = findBlobs(p, W, { x: 0, y: 0, w: W, h: H }, merge).filter((q) => q.area > W * H * 0.002);
  const medianH = parts.map((q) => q.h).sort((a, b) => a - b)[parts.length >> 1] || 1;
  parts.sort((a, b) => (a.y + a.h / 2) - (b.y + b.h / 2));
  const rows = [];
  for (const q of parts) {
    const row = rows.find((r) => Math.abs(r.cy - (q.y + q.h / 2)) < medianH / 2);
    if (row) row.items.push(q); else rows.push({ cy: q.y + q.h / 2, items: [q] });
  }
  return rows.flatMap((r) => r.items.sort((a, b) => a.x - b.x));
};

const gridParts = (p, W, H, cols, rows, merge) => {
  const parts = [];
  for (let r = 0; r < rows; r++) for (let c = 0; c < cols; c++) {
    const x0 = Math.round(c * W / cols), y0 = Math.round(r * H / rows);
    const rect = { x: x0, y: y0, w: Math.round((c + 1) * W / cols) - x0, h: Math.round((r + 1) * H / rows) - y0 };
    const blobs = findBlobs(p, W, rect, merge);
    if (!blobs.length) continue;
    const biggest = Math.max(...blobs.map((q) => q.area));
    const kept = blobs.filter((q) => q.area > W * H * 0.0004 && !(q.edge && q.area < biggest * 0.15));
    if (kept.length && biggest > W * H * 0.002) parts.push(union(kept));
  }
  return parts;
};

(async () => {
  const results = {};
  for (const job of window.JOBS) {
    const img = await load(job.png);
    const W = img.naturalWidth, H = img.naturalHeight;
    const src = document.createElement('canvas');
    src.width = W; src.height = H;
    const sctx = src.getContext('2d', { willReadFrequently: true });
    sctx.drawImage(img, 0, 0);
    if (job.background) {
      const h = Math.round(H * job.size / W);
      const c = document.createElement('canvas');
      c.width = job.size; c.height = h;
      const ctx = c.getContext('2d');
      ctx.imageSmoothingQuality = 'high';
      ctx.drawImage(src, 0, 0, job.size, h);
      results[job.names[0] || 'background'] = c.toDataURL('image/png').split(',')[1];
      continue;
    }
    const data = sctx.getImageData(0, 0, W, H);
    keyOut(data.data, job.key === 'green');
    sctx.putImageData(data, 0, 0);
    const parts = job.grid ? gridParts(data.data, W, H, job.grid[0], job.grid[1], job.merge) : blobParts(data.data, W, H, job.merge);
    const largest = Math.max(...parts.map((q) => Math.max(q.w, q.h)));
    const scale = (job.size * 0.94) / largest;
    parts.forEach((q, i) => {
      const c = document.createElement('canvas');
      c.width = job.size; c.height = job.size;
      const ctx = c.getContext('2d');
      ctx.imageSmoothingQuality = 'high';
      const w = q.w * scale, h = q.h * scale;
      const x = (job.size - w) / 2;
      const y = job.align === 'bottom' ? job.size * 0.98 - h : (job.size - h) / 2;
      ctx.drawImage(src, q.x, q.y, q.w, q.h, x, y, w, h);
      results[job.names[i] || ('part-' + (i + 1))] = c.toDataURL('image/png').split(',')[1];
    });
  }
  window.finish(results);
})();`;

function parseArgs(argv) {
  const args = { sheet: argv[0], out: argv[1], key: 'green', size: 384, align: 'center', names: [], merge: 12, background: false, grid: null };
  for (let i = 2; i < argv.length; i += 1) {
    const value = argv[i + 1];
    if (argv[i] === '--key') args.key = value;
    else if (argv[i] === '--size') args.size = Number(value);
    else if (argv[i] === '--align') args.align = value;
    else if (argv[i] === '--names') args.names = value.split(',');
    else if (argv[i] === '--merge') args.merge = Number(value);
    else if (argv[i] === '--grid') args.grid = value.split('x').map(Number);
    else if (argv[i] === '--background') {
      args.background = true;
      continue;
    } else throw new Error(`Unknown option ${argv[i]}.`);
    i += 1;
  }
  if (!args.sheet || !args.out) throw new Error('Usage: node design/art/cut-sheet.mjs <sheet.png> <out folder> [options]');
  if (!['green', 'magenta'].includes(args.key)) throw new Error('--key is green or magenta.');
  return args;
}

/** Cuts one sheet and returns a map of part name → PNG bytes. Called from main and choose.mjs. */
export function cutSheet(sheetPath, options) {
  const job = {
    png: readFileSync(sheetPath).toString('base64'),
    key: options.key ?? 'green',
    size: options.size ?? 384,
    align: options.align ?? 'center',
    names: options.names ?? [],
    merge: options.merge ?? 12,
    background: options.background ?? false,
    grid: options.grid ?? null,
  };
  return runInChrome(cutScript, [job]);
}

function main() {
  const args = parseArgs(process.argv.slice(2));
  const parts = cutSheet(resolve(args.sheet), args);
  for (const [name, bytes] of parts) writeFile(join(resolve(args.out), `${name}.png`), bytes);
  console.log(`Cut ${parts.size} parts from ${basename(args.sheet)} into ${resolve(args.out)}.`);
}

if (process.argv[1] && resolve(process.argv[1]) === resolve(new URL(import.meta.url).pathname.replace(/^\/([A-Za-z]:)/, '$1'))) {
  try {
    main();
  } catch (e) {
    console.error(e.message);
    process.exit(1);
  }
}
