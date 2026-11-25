import fs from 'node:fs/promises';
import path from 'node:path';
import http from 'node:http';
import { pathToFileURL } from 'node:url';

const DEFAULT_PORT = 41731;

const MIME_TYPES = {
    '.html': 'text/html; charset=utf-8',
    '.js': 'application/javascript; charset=utf-8',
    '.mjs': 'application/javascript; charset=utf-8',
    '.css': 'text/css; charset=utf-8',
    '.json': 'application/json; charset=utf-8',
    '.png': 'image/png',
    '.jpg': 'image/jpeg',
    '.jpeg': 'image/jpeg',
    '.svg': 'image/svg+xml',
    '.webp': 'image/webp',
    '.ico': 'image/x-icon',
    '.woff': 'font/woff',
    '.woff2': 'font/woff2',
    '.ttf': 'font/ttf',
    '.mp4': 'video/mp4',
    '.webm': 'video/webm',
    '.md': 'text/markdown; charset=utf-8'
};

function nowIso() {
    return new Date().toISOString();
}

function normalizeRequestPath(urlPath) {
    let safePath = decodeURIComponent(String(urlPath || '/'));
    safePath = safePath.split('?')[0].split('#')[0];
    if (safePath === '/' || safePath === '') return '/index.html';
    return safePath;
}

function resolveFilePath(rootDir, urlPath) {
    const normalized = normalizeRequestPath(urlPath);
    const target = path.resolve(rootDir, `.${normalized}`);
    const safeRoot = path.resolve(rootDir);
    if (!target.startsWith(safeRoot)) return '';
    return target;
}

async function readStaticFile(rootDir, urlPath) {
    const candidate = resolveFilePath(rootDir, urlPath);
    if (!candidate) return null;
    try {
        const stat = await fs.stat(candidate);
        if (stat.isDirectory()) {
            const indexPath = path.join(candidate, 'index.html');
            const buf = await fs.readFile(indexPath);
            return { path: indexPath, buf };
        }
        const buf = await fs.readFile(candidate);
        return { path: candidate, buf };
    } catch (_) {
        return null;
    }
}

function mimeFor(filePath) {
    const ext = path.extname(filePath || '').toLowerCase();
    return MIME_TYPES[ext] || 'application/octet-stream';
}

export async function startStaticServer(rootDir, port = DEFAULT_PORT) {
    const safeRoot = path.resolve(rootDir);
    const server = http.createServer(async (req, res) => {
        const payload = await readStaticFile(safeRoot, req.url || '/');
        if (!payload) {
            res.statusCode = 404;
            res.setHeader('content-type', 'text/plain; charset=utf-8');
            res.end('Not Found');
            return;
        }
        res.statusCode = 200;
        res.setHeader('content-type', mimeFor(payload.path));
        res.end(payload.buf);
    });
    await new Promise((resolve, reject) => {
        server.once('error', reject);
        server.listen(port, '127.0.0.1', resolve);
    });
    return {
        port,
        origin: `http://127.0.0.1:${port}`,
        close: () => new Promise((resolve, reject) => {
            server.close((error) => {
                if (error) reject(error);
                else resolve();
            });
        })
    };
}

export async function resolvePlaywright() {
    try {
        return await import('playwright');
    } catch (error) {
        const message = [
            'Playwright is required for acceptance automation.',
            'Install command: npm i -D playwright',
            `Original error: ${error.message}`
        ].join('\n');
        throw new Error(message);
    }
}

async function ensureDir(dirPath) {
    await fs.mkdir(dirPath, { recursive: true });
}

async function runInteraction(page, interaction) {
    const step = interaction && typeof interaction === 'object' ? interaction : {};
    const action = String(step.action || '').trim();
    if (!action) return { passed: true, detail: 'noop' };

    if (action === 'wait') {
        const ms = Math.max(0, Number(step.ms || 0));
        if (ms > 0) await page.waitForTimeout(ms);
        return { passed: true, detail: `wait:${ms}` };
    }
    if (action === 'click') {
        await page.click(String(step.selector || ''));
        return { passed: true, detail: `click:${step.selector}` };
    }
    if (action === 'type') {
        const selector = String(step.selector || '');
        await page.click(selector);
        await page.fill(selector, '');
        await page.type(selector, String(step.text || ''));
        return { passed: true, detail: `type:${selector}` };
    }
    if (action === 'key') {
        await page.keyboard.press(String(step.value || ''));
        return { passed: true, detail: `key:${step.value}` };
    }
    if (action === 'assertText') {
        const selector = String(step.selector || '');
        await page.waitForSelector(selector, { state: 'visible', timeout: 10000 });
        const text = await page.textContent(selector);
        const safeText = String(text || '');
        const expected = String(step.includes || '');
        if (!safeText.includes(expected)) {
            return { passed: false, detail: `assertText failed: ${selector} missing "${expected}"` };
        }
        return { passed: true, detail: `assertText:${selector}` };
    }
    if (action === 'assertVisible') {
        const selector = String(step.selector || '');
        await page.waitForSelector(selector, { state: 'visible', timeout: 10000 });
        return { passed: true, detail: `assertVisible:${selector}` };
    }
    if (action === 'assertBackdropTransparentWhenDialogOutOfViewport') {
        const passed = await page.evaluate(() => {
            const dialog = document.querySelector('#quick-create-modal .quick-create-dialog');
            const backdrop = document.getElementById('quick-create-backdrop');
            if (!dialog || !backdrop) return false;
            const prev = dialog.getAttribute('style') || '';
            dialog.setAttribute('style', `${prev}; transform: translate(150vw, 150vh);`);
            return new Promise((resolve) => {
                requestAnimationFrame(() => {
                    requestAnimationFrame(() => {
                        const ok = backdrop.classList.contains('quick-create-backdrop-transparent');
                        dialog.setAttribute('style', prev);
                        resolve(ok);
                    });
                });
            });
        });
        if (!passed) {
            return { passed: false, detail: 'backdrop did not become transparent when dialog moved out of viewport' };
        }
        return { passed: true, detail: 'backdrop transparency verified' };
    }
    return { passed: false, detail: `unsupported action: ${action}` };
}

async function runSuite(page, suite, outputDir, origin) {
    const safeSuite = suite && typeof suite === 'object' ? suite : {};
    const url = `${origin}${String(safeSuite.url || '')}`;
    await page.goto(url, { waitUntil: 'domcontentloaded' });
    const selectors = Array.isArray(safeSuite.selectors) ? safeSuite.selectors : [];
    const interactions = Array.isArray(safeSuite.interactions) ? safeSuite.interactions : [];

    const selectorResults = [];
    for (const selector of selectors) {
        const safeSelector = String(selector || '').trim();
        if (!safeSelector) continue;
        try {
            await page.waitForSelector(safeSelector, { timeout: 12000 });
            selectorResults.push({ selector: safeSelector, passed: true });
        } catch (error) {
            selectorResults.push({ selector: safeSelector, passed: false, error: error.message });
        }
    }

    const interactionResults = [];
    for (const interaction of interactions) {
        try {
            const result = await runInteraction(page, interaction);
            interactionResults.push(result);
            if (!result.passed) break;
        } catch (error) {
            interactionResults.push({
                passed: false,
                detail: error.message
            });
            break;
        }
    }

    const screenshotName = String(safeSuite.screenshot || `${safeSuite.id || 'suite'}.png`);
    const screenshotPath = path.join(outputDir, screenshotName);
    await page.screenshot({ path: screenshotPath, fullPage: true });

    const selectorsPassed = selectorResults.filter((item) => item.passed).length;
    const interactionsPassed = interactionResults.filter((item) => item.passed).length;
    const totalSelectors = Math.max(1, selectorResults.length);
    const totalInteractions = Math.max(1, interactionResults.length);
    const suitePassed = selectorResults.every((item) => item.passed) && interactionResults.every((item) => item.passed);

    return {
        id: String(safeSuite.id || ''),
        title: String(safeSuite.title || safeSuite.id || ''),
        url,
        passed: suitePassed,
        selectorResults,
        interactionResults,
        screenshotPath,
        selectorScore: (selectorsPassed / totalSelectors) * 100,
        interactionScore: (interactionsPassed / totalInteractions) * 100
    };
}

function round2(value) {
    return Math.round((Number(value) + Number.EPSILON) * 100) / 100;
}

export function deriveDimensions(results) {
    const list = Array.isArray(results) ? results : [];
    if (!list.length) {
        return {
            layout: 0,
            interaction: 0,
            writeback: 0,
            fallback: 0,
            backdropTransparency: 0
        };
    }
    const layout = list.reduce((sum, item) => sum + Number(item.selectorScore || 0), 0) / list.length;
    const interaction = list.reduce((sum, item) => sum + Number(item.interactionScore || 0), 0) / list.length;

    const ideVisual = list.find((item) => item.id === 'ide-visual');
    const sourceEditor = list.find((item) => item.id === 'source-editor');
    const writeback = ideVisual ? Number(ideVisual.interactionScore || 0) : interaction;
    const fallback = sourceEditor ? Number(sourceEditor.interactionScore || 0) : interaction;
    const backdropTransparency = ideVisual
        ? (ideVisual.interactionResults.some((entry) => String(entry.detail || '').includes('backdrop transparency'))
            ? 100
            : 0)
        : 0;

    return {
        layout: round2(layout),
        interaction: round2(interaction),
        writeback: round2(writeback),
        fallback: round2(fallback),
        backdropTransparency: round2(backdropTransparency)
    };
}

export async function executeSuites(options) {
    const opts = options && typeof options === 'object' ? options : {};
    const rootDir = path.resolve(String(opts.rootDir || process.cwd()));
    const suites = Array.isArray(opts.suites) ? opts.suites : [];
    const outputDir = path.resolve(String(opts.outputDir || path.join(rootDir, 'tmp-playwright/artifacts')));
    const port = Number(opts.port || DEFAULT_PORT);

    await ensureDir(outputDir);
    const playwright = await resolvePlaywright();
    const browser = await playwright.chromium.launch({
        headless: true,
        args: ['--no-sandbox']
    });
    const page = await browser.newPage({
        viewport: { width: 1440, height: 900 }
    });

    const server = await startStaticServer(rootDir, port);
    const startedAt = nowIso();
    const suiteResults = [];
    try {
        for (const suite of suites) {
            const result = await runSuite(page, suite, outputDir, server.origin);
            suiteResults.push(result);
        }
    } finally {
        await browser.close();
        await server.close();
    }

    const dimensions = deriveDimensions(suiteResults);
    return {
        startedAt,
        finishedAt: nowIso(),
        origin: server.origin,
        dimensions,
        suites: suiteResults
    };
}

export async function writeJsonFile(filePath, payload) {
    const target = path.resolve(String(filePath || ''));
    await ensureDir(path.dirname(target));
    await fs.writeFile(target, `${JSON.stringify(payload, null, 4)}\n`, 'utf8');
    return target;
}

export async function importJson(filePath) {
    const url = pathToFileURL(path.resolve(filePath)).href;
    const module = await import(url, { with: { type: 'json' } });
    return module.default;
}
