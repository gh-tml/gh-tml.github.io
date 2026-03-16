const test = require('node:test');
const assert = require('node:assert/strict');

const frontMatterUtils = require('../../../shared/services/markdown/front-matter-utils.js');

test('front-matter utils parse and apply defaults', () => {
    const markdown = [
        '---',
        'title: Hello',
        'topic: article-contribution',
        'difficulty: intermediate',
        'colors:',
        '  accent: "#ff00aa"',
        '---',
        '',
        '# Hello'
    ].join('\n');

    const parsed = frontMatterUtils.parseFrontMatter(markdown);
    assert.equal(parsed.hasFrontMatter, true);
    assert.equal(parsed.metadata.title, 'Hello');
    assert.equal(parsed.metadata.topic, 'article-contribution');
    assert.equal(parsed.metadata.difficulty, 'intermediate');
    assert.equal(parsed.metadata.colors.accent, '#ff00aa');
});

test('front-matter utils ensure/merge keeps markdown body', () => {
    const source = '# Title\n\nBody\n';
    const ensured = frontMatterUtils.ensureFrontMatter(source, { title: 'New Doc' });
    assert.match(ensured, /^---\n/);
    assert.match(ensured, /title: New Doc/);
    assert.match(ensured, /# Title/);

    const merged = frontMatterUtils.mergeFrontMatter(ensured, {
        title: 'Updated',
        difficulty: 'advanced'
    });
    assert.match(merged, /title: Updated/);
    assert.match(merged, /difficulty: advanced/);
    assert.match(merged, /Body/);
});

test('front-matter utils parses extended metadata fields and keeps arrays', () => {
    const markdown = [
        '---',
        'title: Meta Demo',
        'category: 如何贡献',
        'date: 2026-03-16',
        'last_updated: 2026-03-16',
        'next_chapter: next.md',
        'prev_chapter: prev.md',
        'source_cs:',
        '  - code/A.cs',
        '  - code/B.cs',
        '---',
        '',
        '# Demo'
    ].join('\n');

    const parsed = frontMatterUtils.parseFrontMatter(markdown);
    assert.equal(parsed.metadata.category, '如何贡献');
    assert.equal(parsed.metadata.date, '2026-03-16');
    assert.equal(parsed.metadata.last_updated, '2026-03-16');
    assert.equal(parsed.metadata.next_chapter, 'next.md');
    assert.equal(parsed.metadata.prev_chapter, 'prev.md');
    assert.deepEqual(parsed.metadata.source_cs, ['code/A.cs', 'code/B.cs']);
});

test('front-matter utils merge preserves unknown metadata fields', () => {
    const markdown = [
        '---',
        'title: Keep Unknown',
        'custom_flag: enabled',
        'source_cs:',
        '  - code/Old.cs',
        '---',
        '',
        '# Body'
    ].join('\n');

    const merged = frontMatterUtils.mergeFrontMatter(markdown, {
        title: 'Updated',
        source_cs: ['code/New.cs']
    });
    const reparsed = frontMatterUtils.parseFrontMatter(merged);

    assert.equal(reparsed.metadata.title, 'Updated');
    assert.equal(reparsed.metadata.custom_flag, 'enabled');
    assert.deepEqual(reparsed.metadata.source_cs, ['code/New.cs']);
    assert.match(merged, /# Body/);
});

test('front-matter utils parses empty scalar fields as empty strings', () => {
    const markdown = [
        '---',
        'title: Empty Scalar Demo',
        'author:',
        'next_chapter:',
        '---',
        '',
        '# Body'
    ].join('\n');

    const parsed = frontMatterUtils.parseFrontMatter(markdown);
    assert.equal(parsed.metadata.author, '');
    assert.equal(parsed.metadata.next_chapter, '');

    const merged = frontMatterUtils.mergeFrontMatter(markdown, {
        title: 'Empty Scalar Demo'
    });
    assert.doesNotMatch(merged, /\[object Object\]/);
});
