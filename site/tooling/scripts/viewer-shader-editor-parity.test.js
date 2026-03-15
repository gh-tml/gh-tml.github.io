const test = require('node:test');
const assert = require('node:assert/strict');
const fs = require('node:fs');
const path = require('node:path');

function readViewer() {
    return fs.readFileSync(path.resolve('site/pages/viewer.html'), 'utf8');
}

test('viewer shader modal includes ide-like preview control blocks', () => {
    const viewer = readViewer();

    assert.match(viewer, /fx-embed-preview-controls/);
    assert.match(viewer, /fx-embed-timebox/);
    assert.match(viewer, /fx-embed-stage-head/);
    assert.match(viewer, /fx-embed-zoom-actions/);
    assert.match(viewer, /data-fx-embed-resize-dir="n"/);
    assert.match(viewer, /data-fx-embed-resize-dir="e"/);
    assert.match(viewer, /data-fx-embed-resize-dir="s"/);
    assert.match(viewer, /data-fx-embed-resize-dir="w"/);
    assert.match(viewer, /data-fx-embed-resize-dir="ne"/);
    assert.match(viewer, /data-fx-embed-resize-dir="nw"/);
    assert.match(viewer, /data-fx-embed-resize-dir="se"/);
    assert.match(viewer, /data-fx-embed-resize-dir="sw"/);
    assert.doesNotMatch(viewer, /fx-embed-aspect-resizer/);
    assert.doesNotMatch(viewer, /拖动右侧分隔条可调整比例/);
    assert.match(viewer, /fx-embed-upload-name/);
});

test('viewer shader modal defines preview mode normalizers and iTime operations', () => {
    const viewer = readViewer();

    assert.match(viewer, /normalizeFxEmbedPreviewPreset/);
    assert.match(viewer, /normalizeFxEmbedPreviewRenderMode/);
    assert.match(viewer, /normalizeFxEmbedPreviewAddressMode/);
    assert.match(viewer, /normalizeFxEmbedPreviewBgMode/);
    assert.match(viewer, /setFxEmbedPreviewRunning/);
    assert.match(viewer, /applyFxEmbedITimeFromInput/);
    assert.match(viewer, /offsetFxEmbedITime/);
    assert.match(viewer, /resetFxEmbedITimeOffset/);
});

test('viewer shader modal defines viewport zoom drag and edge resize interactions', () => {
    const viewer = readViewer();

    assert.match(viewer, /installFxEmbedViewportInteractions/);
    assert.match(viewer, /installFxEmbedEdgeResizeInteractions/);
    assert.match(viewer, /nextFxEmbedViewportSizeFromDrag/);
    assert.match(viewer, /setFxEmbedZoom/);
    assert.match(viewer, /resetFxEmbedView/);
    assert.match(viewer, /setFxEmbedViewportSize/);
});

test('viewer shader modal reapplies viewport size while syncing restored state', () => {
    const viewer = readViewer();

    assert.match(
        viewer,
        /function syncFxEmbedControls\(\)\s*{[\s\S]*?applyFxEmbedViewportSize\(\{ redraw: false, status: false \}\);[\s\S]*?applyFxEmbedViewTransform\(\);/
    );
    assert.match(
        viewer,
        /modal\.viewportWidth[\s\S]*?modal\.viewportHeight[\s\S]*?syncFxEmbedControls\(\);/
    );
});
