import test from 'node:test';
import assert from 'node:assert/strict';
import fs from 'node:fs';
import path from 'node:path';
import { fileURLToPath } from 'node:url';

const __filename = fileURLToPath(import.meta.url);
const __dirname = path.dirname(__filename);
const mainPath = path.resolve(__dirname, '../src/main.js');

test('ide defines mobile-lite viewport guard and ui state', () => {
    const source = fs.readFileSync(mainPath, 'utf8');

    assert.match(source, /const MOBILE_LITE_MAX_WIDTH = 860;/);
    assert.match(source, /mobileLite:\s*false,/);
    assert.match(source, /function isMobileLiteViewport\(\)/);
    assert.match(source, /function applyMobileLiteMode\(options\)/);
    assert.match(source, /classList\.toggle\('is-mobile-lite',\s*!!state\.ui\.mobileLite\)/);
});

test('ide mobile-lite mode blocks heavy features and submit routing', () => {
    const source = fs.readFileSync(mainPath, 'utf8');

    assert.match(source, /if \(state\.ui\.mobileLite\)\s*\{\s*setSubmitPanelRouteState\(false,/s);
    assert.match(source, /if \(open && state\.ui\.mobileLite\)\s*\{\s*if \(!opts\.silent\)\s*\{\s*notifyMobileLiteBlocked\('Shader 渲染预览'\);/s);
    assert.match(source, /if \(nextOpen && state\.ui\.mobileLite\)\s*\{\s*if \(!opts\.silent\)\s*\{\s*notifyMobileLiteBlocked\('流程图工作台'\);/s);
    assert.match(source, /setMobileLiteControlDisabled\(dom\.btnOpenUnifiedSubmit,\s*disabled,\s*MOBILE_LITE_DISABLED_HINT\);/);
    assert.match(source, /setMobileLiteControlDisabled\(dom\.btnMdFlowchart,\s*disabled,\s*MOBILE_LITE_DISABLED_HINT\);/);
    assert.match(source, /setMobileLiteControlDisabled\(dom\.btnShaderPreviewPopup,\s*disabled,\s*MOBILE_LITE_DISABLED_HINT\);/);
});
