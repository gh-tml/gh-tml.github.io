import path from 'node:path';
import { fileURLToPath } from 'node:url';
import { executeSuites, writeJsonFile } from './lib/runner.mjs';
import { getVisualParitySuites } from './lib/suites.mjs';

const __filename = fileURLToPath(import.meta.url);
const __dirname = path.dirname(__filename);
const repoRoot = path.resolve(__dirname, '..');

function parseArgs(argv) {
    const args = Array.isArray(argv) ? argv.slice(2) : [];
    const out = {
        report: path.join(repoRoot, 'tmp-playwright/artifacts/unified-ide-report.v1.json'),
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

export async function runUnifiedAcceptance(options) {
    const opts = options && typeof options === 'object' ? options : {};
    const reportPath = path.resolve(String(opts.report || path.join(repoRoot, 'tmp-playwright/artifacts/unified-ide-report.v1.json')));
    const outputDir = path.resolve(String(opts.outputDir || path.join(repoRoot, 'tmp-playwright/artifacts/screenshots')));
    const suites = getVisualParitySuites();
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
    const result = await runUnifiedAcceptance(args);
    const report = result.report || {};
    console.log(`Unified acceptance completed: ${result.reportPath}`);
    console.log(`Dimensions: ${JSON.stringify(report.dimensions || {}, null, 2)}`);
}

if (process.argv[1] && path.resolve(process.argv[1]) === __filename) {
    main().catch((error) => {
        console.error(error.stack || error.message || String(error));
        process.exitCode = 1;
    });
}
