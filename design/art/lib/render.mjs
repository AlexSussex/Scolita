// Renders SVG drawings to PNG files with the PC's headless Chrome, in one run:
// a page draws each SVG on a canvas and writes the PNGs into its DOM, which Chrome prints.

import { execFileSync } from 'node:child_process';
import { existsSync, mkdirSync, mkdtempSync, readFileSync, rmSync, writeFileSync } from 'node:fs';
import { tmpdir } from 'node:os';
import { dirname, join } from 'node:path';
import { pathToFileURL } from 'node:url';

const chromeCandidates = [
  process.env.SCOLITA_CHROME,
  'C:/Program Files/Google/Chrome/Application/chrome.exe',
  'C:/Program Files (x86)/Microsoft/Edge/Application/msedge.exe',
  'C:/Program Files/Microsoft/Edge/Application/msedge.exe',
].filter(Boolean);

function findChrome() {
  const found = chromeCandidates.find((p) => existsSync(p));
  if (!found) throw new Error('No Chrome or Edge found; set SCOLITA_CHROME to its chrome.exe.');
  return found;
}

/** Puts the Baloo 2 font inside every SVG that writes text, since an SVG image can't load files. */
export function embedFont(svg, fontBase64) {
  if (!svg.includes('font-family="Baloo2"')) return svg;
  const style = `<style>@font-face{font-family:Baloo2;font-weight:400 800;src:url(data:font/ttf;base64,${fontBase64}) format('truetype');}</style>`;
  return svg.replace('<defs>', `<defs>${style}`);
}

const renderScript = `
(async () => {
  const results = {};
  for (const job of window.JOBS) {
    const img = new Image();
    img.src = 'data:image/svg+xml;base64,' + job.svg;
    await img.decode();
    await new Promise((r) => setTimeout(r, 0));
    const canvas = document.createElement('canvas');
    canvas.width = job.w;
    canvas.height = job.h;
    const ctx = canvas.getContext('2d');
    ctx.drawImage(img, 0, 0, job.w, job.h);
    results[job.name] = canvas.toDataURL('image/png').split(',')[1];
  }
  window.finish(results);
})();`;

/**
 * Runs a script in a headless Chrome page with window.JOBS set to the payload; the script calls
 * window.finish(results) with an object of name → base64 PNG. Returns a map of name → PNG bytes.
 */
export function runInChrome(script, payload) {
  const chrome = findChrome();
  const dir = mkdtempSync(join(tmpdir(), 'scolita-art-'));
  try {
    const page =
      '<!doctype html><html><head><meta charset="utf-8"></head><body><pre id="out"></pre>' +
      '<script>window.finish = (r) => { document.getElementById("out").textContent = "@@BEGIN@@" + JSON.stringify(r) + "@@END@@"; };' +
      'window.addEventListener("unhandledrejection", (e) => window.finish({ "@error": String(e.reason && e.reason.stack || e.reason) }));' +
      'window.addEventListener("error", (e) => window.finish({ "@error": String(e.message) }));</script>' +
      '<script src="jobs.js"></script><script src="script.js"></script></body></html>';
    writeFileSync(join(dir, 'jobs.js'), `window.JOBS = ${JSON.stringify(payload)};`);
    writeFileSync(join(dir, 'script.js'), script);
    writeFileSync(join(dir, 'page.html'), page);
    const out = execFileSync(
      chrome,
      [
        '--headless=new',
        '--disable-gpu',
        '--no-first-run',
        '--disable-extensions',
        `--user-data-dir=${join(dir, 'profile')}`,
        '--allow-file-access-from-files',
        '--virtual-time-budget=120000',
        '--dump-dom',
        pathToFileURL(join(dir, 'page.html')).href,
      ],
      { maxBuffer: 1 << 30, stdio: ['ignore', 'pipe', 'ignore'] },
    ).toString('utf8');
    const marker = '<pre id="out">@@BEGIN@@';
    const begin = out.indexOf(marker);
    const end = out.indexOf('@@END@@', begin);
    if (begin < 0 || end < 0) throw new Error('Chrome produced nothing.');
    const parsed = JSON.parse(out.slice(begin + marker.length, end).replace(/&quot;/g, '"').replace(/&amp;/g, '&'));
    if (parsed['@error']) throw new Error(`The page failed: ${parsed['@error']}`);
    return new Map(Object.entries(parsed).map(([k, v]) => [k, Buffer.from(v, 'base64')]));
  } finally {
    try {
      rmSync(dir, { recursive: true, force: true, maxRetries: 20, retryDelay: 250 });
    } catch (e) {
      console.warn(`Could not delete the temporary folder ${dir} (Chrome may still hold it): ${e.message}`);
    }
  }
}

/**
 * Renders jobs ({ name, width, height, svg }) and returns a map of name → PNG bytes.
 * Jobs go in batches so Chrome's printed page stays a reasonable size.
 */
export function renderAll(jobs, fontBase64, batchSize = 40) {
  const results = new Map();
  for (let start = 0; start < jobs.length; start += batchSize) {
    const batch = jobs.slice(start, start + batchSize);
    const payload = batch.map((j) => ({
      name: j.name,
      w: j.width,
      h: j.height,
      svg: Buffer.from(embedFont(j.svg, fontBase64), 'utf8').toString('base64'),
    }));
    const rendered = runInChrome(renderScript, payload);
    for (const job of batch) {
      if (!rendered.has(job.name)) throw new Error(`Chrome did not render ${job.name}.`);
      results.set(job.name, rendered.get(job.name));
    }
  }
  return results;
}

/** Writes a file, creating its folder. */
export function writeFile(path, bytes) {
  mkdirSync(dirname(path), { recursive: true });
  writeFileSync(path, bytes);
}

/** Reads the Baloo 2 font from the Unity project, as base64. */
export function readFont(path) {
  return readFileSync(path).toString('base64');
}
