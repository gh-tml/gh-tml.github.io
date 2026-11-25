const VIEWER_DOC = '如何贡献/教学文章写作指南.md';
const viewerQuery = `viewer.html?file=${encodeURIComponent(VIEWER_DOC)}&studio_preview=1`;

const VISUAL_PARITY_SUITES = [
    {
        id: 'viewer-embed',
        title: 'Viewer 新标签预览',
        url: `/site/pages/${viewerQuery}`,
        selectors: [
            '#main-content',
            '#markdown-content'
        ],
        interactions: [
            { action: 'wait', ms: 1200 }
        ],
        screenshot: 'viewer-embed.png'
    },
    {
        id: 'ide-visual',
        title: 'IDE Markdown 工具栏',
        url: '/tml-ide/index.html',
        selectors: [
            '#btn-markdown-open-viewer',
            '#markdown-toolbox-group',
            '#btn-add-file'
        ],
        interactions: [
            { action: 'click', selector: '#btn-add-file' },
            { action: 'assertVisible', selector: '#quick-create-modal' },
            { action: 'type', selector: '#quick-create-name', text: 'alignment-check.anim.ts' },
            { action: 'assertBackdropTransparentWhenDialogOutOfViewport' },
            { action: 'click', selector: '#btn-quick-create-submit' },
            { action: 'wait', ms: 600 }
        ],
        screenshot: 'ide-visual.png'
    },
    {
        id: 'source-editor',
        title: '源码编辑区',
        url: '/tml-ide/index.html',
        selectors: [
            '#btn-markdown-open-viewer',
            '#ide-context-menu',
            '#problems-list'
        ],
        interactions: [
            { action: 'key', value: 'Control+Shift+P' },
            { action: 'wait', ms: 500 },
            { action: 'key', value: 'Control+Period' },
            { action: 'wait', ms: 500 }
        ],
        screenshot: 'source-editor.png'
    }
];

const VSCODE_ACCEPTANCE_SUITES = [
    {
        id: 'ide-runtime-fingerprint',
        title: 'IDE 运行时指纹与快捷链路',
        url: '/tml-ide/index.html',
        selectors: [
            '#quick-create-modal',
            '#btn-quick-create-submit',
            '[data-panel-tab="output"]',
            '#problems-list',
            '#ide-context-menu'
        ],
        interactions: [
            { action: 'click', selector: '#btn-add-file' },
            { action: 'assertVisible', selector: '#quick-create-modal' },
            { action: 'type', selector: '#quick-create-name', text: 'fingerprint.anim.ts' },
            { action: 'click', selector: '#btn-quick-create-submit' },
            { action: 'key', value: 'Control+Shift+P' },
            { action: 'key', value: 'Control+Period' },
            { action: 'wait', ms: 500 }
        ],
        screenshot: 'ide-runtime-fingerprint.png'
    }
];

export const runtimeFingerprint = {
    expectedStrings: [
        'Runtime bundle mismatch',
        'Control+Shift+P',
        'Control+Period',
        'data-panel-tab="output"',
        'Item.',
        'hasDamage',
        'override',
        'SetDefaults',
        'SHADER_COMPILE_ERROR',
        '#problems-list',
        '#ide-context-menu'
    ]
};

function clone(suites) {
    return suites.map((suite) => JSON.parse(JSON.stringify(suite)));
}

export function getVisualParitySuites() {
    return clone(VISUAL_PARITY_SUITES);
}

export function getUnifiedAcceptanceSuites() {
    return clone(VSCODE_ACCEPTANCE_SUITES);
}

export function getViewerPreviewUrl() {
    return viewerQuery;
}
