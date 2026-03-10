import test from 'node:test';
import assert from 'node:assert/strict';
import fs from 'node:fs';
import path from 'node:path';

const scriptDir = path.dirname(new URL(import.meta.url).pathname);
const viewerPath = path.resolve(scriptDir, '../../pages/viewer.html');

test('viewer sidebar keeps quick search input and learn-tree render call', () => {
    const html = fs.readFileSync(viewerPath, 'utf8');
    assert.match(html, /id="sidebar-quick-search"/);
    assert.match(html, /renderLearnFolderTreeNavigation\(categorySidebar,\s*\{\s*docs:\s*ALL_DOCS,\s*defaultCollapseDepth:\s*1\s*\}\);/);
});

test('viewer sidebar init removes legacy hard-coded branch wrapper', () => {
    const html = fs.readFileSync(viewerPath, 'utf8');
    assert.doesNotMatch(
        html,
        /if\s*\(\s*true\s*\)\s*\{\s*renderLearnFolderTreeNavigation\(categorySidebar,\s*\{\s*docs:\s*ALL_DOCS,\s*defaultCollapseDepth:\s*1\s*\}\);/s
    );
});

test('viewer mobile navigation keeps dedicated toggle and route-safe close hooks', () => {
    const html = fs.readFileSync(viewerPath, 'utf8');
    assert.match(html, /id="mobile-nav-toggle"/);
    assert.match(html, /function\s+closeHeaderMainNavMenu\(\)/);
    assert.match(html, /window\.addEventListener\('popstate',\s*handleRouteOrHashChange\)/);
    assert.match(html, /window\.addEventListener\('hashchange',\s*handleRouteOrHashChange\)/);
});

test('viewer AI floating controls prefer scrolling on touch devices', () => {
    const html = fs.readFileSync(viewerPath, 'utf8');
    assert.match(html, /\.viewer-ai-fab-wrap\s*\{[^}]*touch-action:\s*pan-y;[^}]*\}/s);
    assert.match(html, /\.viewer-ai-fab-wrap\.viewer-ai-fab-wrap--dragging\s*\{[^}]*touch-action:\s*none;[^}]*\}/s);
    assert.match(html, /\.viewer-ai-fab\s*\{[^}]*touch-action:\s*manipulation;[^}]*\}/s);
});
