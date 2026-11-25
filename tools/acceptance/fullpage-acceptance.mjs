import path from 'node:path';
import { fileURLToPath } from 'node:url';
import { runViewerAcceptance } from './shared-viewer-acceptance.mjs';

const __filename = fileURLToPath(import.meta.url);
const __dirname = path.dirname(__filename);
const repoRoot = path.resolve(__dirname, '../..');

function parseArgs(argv) {
    const args = Array.isArray(argv) ? argv.slice(2) : [];
    const out = {
        outputDir: path.join(repoRoot, 'tools/acceptance/artifacts/screenshots')
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
    const outputDir = path.resolve(String(opts.outputDir || path.join(repoRoot, 'tools/acceptance/artifacts/screenshots')));
    const viewer = await runViewerAcceptance({
        outputDir,
        report: path.join(repoRoot, 'tools/acceptance/artifacts/viewer-report.v1.json')
    });
    return {
        viewerReport: viewer.reportPath
    };
}

async function main() {
    const args = parseArgs(process.argv);
    const result = await runFullpageAcceptance(args);
    console.log(`Fullpage acceptance completed.`);
    console.log(`Viewer report: ${result.viewerReport}`);
}

if (process.argv[1] && path.resolve(process.argv[1]) === __filename) {
    main().catch((error) => {
        console.error(error.stack || error.message || String(error));
        process.exitCode = 1;
    });
}
