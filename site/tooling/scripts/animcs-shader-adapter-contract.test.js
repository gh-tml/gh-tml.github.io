const test = require('node:test');
const assert = require('node:assert/strict');
const fs = require('node:fs');
const path = require('node:path');

function read(filePath) {
    return fs.readFileSync(path.resolve(filePath), 'utf8');
}

test('viewer loads site shader hlsl adapter before animcs runtime', () => {
    const html = read('site/pages/viewer.html');

    assert.match(html, /\/site\/assets\/js\/shader-hlsl-adapter\.js/);
    assert.match(
        html,
        /shader-hlsl-adapter\.js[\s\S]*animts-runtime\.js/
    );
});

test('anim-renderer loads site shader hlsl adapter before animcs runtime', () => {
    const html = read('site/pages/anim-renderer.html');

    assert.match(html, /shader-hlsl-adapter\.js/);
    assert.match(
        html,
        /shader-hlsl-adapter\.js[\s\S]*animts-runtime\.js/
    );
});
