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
    }
];

function clone(suites) {
    return suites.map((suite) => JSON.parse(JSON.stringify(suite)));
}

export function getVisualParitySuites() {
    return clone(VISUAL_PARITY_SUITES);
}

export function getViewerPreviewUrl() {
    return viewerQuery;
}
