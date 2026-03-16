import fs from 'node:fs/promises';
import path from 'node:path';
import { fileURLToPath } from 'node:url';
import { executeSuites, writeJsonFile } from './lib/runner.mjs';
import { getUnifiedAcceptanceSuites, runtimeFingerprint } from './lib/suites.mjs';

const __filename = fileURLToPath(import.meta.url);
const __dirname = path.dirname(__filename);
const repoRoot = path.resolve(__dirname, '..');

const RUNTIME_FINGERPRINT_REQUIREMENTS = [
    'Runtime bundle mismatch',
    'Control+Shift+P',
    'data-panel-tab="output"',
    'Item.',
    'hasDamage',
    'override',
    'SetDefaults',
    'problems-list',
    'ide-context-menu',
    'Control+Period',
    'SHADER_COMPILE_ERROR'
];

function parseArgs(argv) {
    const args = Array.isArray(argv) ? argv.slice(2) : [];
    const out = {
        report: path.join(repoRoot, 'tmp-playwright/artifacts/vscode-acceptance-report.v1.json'),
        outputDir: path.join(repoRoot, 'tmp-playwright/artifacts/screenshots')
    };
    for (let i = 0; i < args.length; i += 1) {
        const key = args[i];
        const value = args[i + 1];
        if (key === '--report' && value) {
            out.report = path.resolve(value);
            i += 1;
        } else if (key === '--output-dir' && value) {
            out.outputDir = path.resolve(value);
            i += 1;
        }
    }
    return out;
}

async function readRuntimeSources() {
    const htmlPath = path.join(repoRoot, 'tml-ide/index.html');
    const html = await fs.readFile(htmlPath, 'utf8');
    let scriptBundle = '';
    const srcMatch = html.match(/<script[^>]+src="([^"]+index-[^"]+\.js)"/i);
    if (srcMatch && srcMatch[1]) {
        const bundlePath = path.join(repoRoot, 'tml-ide', srcMatch[1].replace(/^\//, ''));
        try {
            scriptBundle = await fs.readFile(bundlePath, 'utf8');
        } catch (_) {
            scriptBundle = '';
        }
    }
    return `${html}\n${scriptBundle}`;
}

export async function verifyRuntimeFingerprint() {
    const sources = await readRuntimeSources();
    const expected = Array.isArray(runtimeFingerprint.expectedStrings)
        ? runtimeFingerprint.expectedStrings
        : RUNTIME_FINGERPRINT_REQUIREMENTS;
    const missing = expected.filter((token) => !sources.includes(token));
    if (missing.length) {
        throw new Error(`Runtime bundle mismatch: missing tokens -> ${missing.join(', ')}`);
    }
}

export async function runVsCodeAcceptance(options) {
    const opts = options && typeof options === 'object' ? options : {};
    await verifyRuntimeFingerprint();
    const reportPath = path.resolve(String(opts.report || path.join(repoRoot, 'tmp-playwright/artifacts/vscode-acceptance-report.v1.json')));
    const outputDir = path.resolve(String(opts.outputDir || path.join(repoRoot, 'tmp-playwright/artifacts/screenshots')));
    const suites = getUnifiedAcceptanceSuites();
    const report = await executeSuites({
        rootDir: repoRoot,
        outputDir,
        suites
    });
    await writeJsonFile(reportPath, report);
    return {
        reportPath,
        report
    };
}

async function main() {
    const args = parseArgs(process.argv);
    const result = await runVsCodeAcceptance(args);
    console.log(`VSCode acceptance completed: ${result.reportPath}`);
}

if (process.argv[1] && path.resolve(process.argv[1]) === __filename) {
    main().catch((error) => {
        console.error(error.stack || error.message || String(error));
        process.exitCode = 1;
    });
}
