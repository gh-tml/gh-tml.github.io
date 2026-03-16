import path from 'node:path';
import { fileURLToPath } from 'node:url';
import { runViewerAcceptance } from './shared-viewer-acceptance.mjs';
import { runUnifiedAcceptance } from './tml-ide-unified-acceptance.mjs';

const __filename = fileURLToPath(import.meta.url);
const __dirname = path.dirname(__filename);
const repoRoot = path.resolve(__dirname, '..');

function parseArgs(argv) {
    const args = Array.isArray(argv) ? argv.slice(2) : [];
    const out = {
        outputDir: path.join(repoRoot, 'tmp-playwright/artifacts/screenshots')
    };
    for (let i = 0; i < args.length; i += 1) {
        const key = args[i];
        const value = args[i + 1];
        if (key === '--output-dir' && value) {
            out.outputDir = path.resolve(value);
            i += 1;
        }
    }
    return out;
}

export async function runFullpageAcceptance(options) {
    const opts = options && typeof options === 'object' ? options : {};
    const outputDir = path.resolve(String(opts.outputDir || path.join(repoRoot, 'tmp-playwright/artifacts/screenshots')));
    const viewer = await runViewerAcceptance({
        outputDir,
        report: path.join(repoRoot, 'tmp-playwright/artifacts/viewer-report.v1.json')
    });
    const unified = await runUnifiedAcceptance({
        outputDir,
        report: path.join(repoRoot, 'tmp-playwright/artifacts/unified-ide-report.v1.json')
    });
    return {
        viewerReport: viewer.reportPath,
        unifiedReport: unified.reportPath
    };
}

async function main() {
    const args = parseArgs(process.argv);
    const result = await runFullpageAcceptance(args);
    console.log(`Fullpage acceptance completed.`);
    console.log(`Viewer report: ${result.viewerReport}`);
    console.log(`Unified report: ${result.unifiedReport}`);
}

if (process.argv[1] && path.resolve(process.argv[1]) === __filename) {
    main().catch((error) => {
        console.error(error.stack || error.message || String(error));
        process.exitCode = 1;
    });
}
