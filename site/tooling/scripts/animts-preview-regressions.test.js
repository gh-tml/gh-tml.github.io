const test = require('node:test');
const assert = require('node:assert/strict');
const fs = require('node:fs');
const path = require('node:path');

function readSource(relPath) {
    return fs.readFileSync(path.resolve(relPath), 'utf8');
}

test('anim renderer selected-file preview registers local module before mount', () => {
    const source = readSource('site/pages/anim-renderer.html');

    assert.match(source, /singleFileModuleBySource/);
    assert.match(source, /function\s+prepareSingleFilePreview\s*\(/);
    assert.match(source, /singleFileModuleBySource\.get\(normalizedKey\)/);
    assert.match(source, /await\s+prepareSingleFilePreview\(currentFile\)/);
});
