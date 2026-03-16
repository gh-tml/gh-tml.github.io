(function (root, factory) {
    const api = factory();
    if (typeof module !== 'undefined' && module.exports) {
        module.exports = api;
    }
    if (root) {
        root.SharedFrontMatterUtils = api;
    }
})(typeof globalThis !== 'undefined' ? globalThis : this, function () {
    'use strict';

    const DEFAULT_METADATA = Object.freeze({
        title: '',
        author: '',
        topic: 'article-contribution',
        description: '',
        order: '',
        difficulty: 'beginner',
        time: '',
        category: '',
        date: '',
        last_updated: '',
        next_chapter: '',
        prev_chapter: '',
        source_cs: [],
        prefix: [],
        min_c: '',
        min_t: '',
        colors: {},
        colorChange: {}
    });

    const KNOWN_FIELD_ORDER = Object.freeze([
        'title',
        'author',
        'topic',
        'description',
        'order',
        'difficulty',
        'time',
        'category',
        'date',
        'last_updated',
        'next_chapter',
        'prev_chapter',
        'source_cs',
        'prefix',
        'min_c',
        'min_t',
        'colors',
        'colorChange'
    ]);

    const KNOWN_FIELD_SET = new Set(KNOWN_FIELD_ORDER);

    function ensureObject(value) {
        return value && typeof value === 'object' && !Array.isArray(value) ? value : {};
    }

    function countIndent(line) {
        const match = String(line || '').match(/^\s*/);
        return match ? match[0].length : 0;
    }

    function normalizeMetaNumberInput(value) {
        const text = String(value || '').trim();
        if (!text) return '';
        const num = Number(text);
        if (!Number.isFinite(num)) return '';
        const normalized = Math.max(0, Math.floor(num));
        return String(normalized);
    }

    function parseSimpleYamlArray(raw) {
        const text = String(raw || '').trim();
        if (!text.startsWith('[') || !text.endsWith(']')) return [];
        const body = text.slice(1, -1).trim();
        if (!body) return [];
        return body.split(',').map(function (item) {
            return parseSimpleYamlValue(item);
        }).filter(Boolean);
    }

    function parseSimpleYamlValue(raw) {
        const text = String(raw || '').trim();
        if (!text) return '';
        if (text.startsWith('[') && text.endsWith(']')) {
            return parseSimpleYamlArray(text);
        }
        if (
            (text.startsWith('"') && text.endsWith('"'))
            || (text.startsWith('\'') && text.endsWith('\''))
        ) {
            return text.slice(1, -1);
        }
        return text;
    }

    function normalizePrefixEntries(rawPrefix) {
        const source = Array.isArray(rawPrefix)
            ? rawPrefix
            : String(rawPrefix || '')
                .split(/\r?\n/)
                .map((line) => String(line || '').trim())
                .filter(Boolean);
        const deduped = [];
        const seen = new Set();
        source.forEach((item) => {
            const raw = parseSimpleYamlValue(item);
            const match = String(raw || '').trim().match(/^\[[^\]]+\]\(([^)]+)\)$/);
            if (!match) return;
            const href = String(match[1] || '').trim().replace(/\\/g, '/');
            if (!/\.md$/i.test(href)) return;
            if (seen.has(raw)) return;
            seen.add(raw);
            deduped.push(raw);
        });
        return deduped;
    }

    function normalizeStringList(rawValue) {
        let source = [];
        if (Array.isArray(rawValue)) {
            source = rawValue;
        } else if (typeof rawValue === 'string') {
            const safe = String(rawValue || '').trim();
            if (safe.startsWith('[') && safe.endsWith(']')) {
                source = parseSimpleYamlArray(safe);
            } else if (safe) {
                source = safe.split(/\r?\n/);
            }
        } else if (rawValue != null) {
            source = [rawValue];
        }

        const out = [];
        const seen = new Set();
        source.forEach((item) => {
            const text = String(parseSimpleYamlValue(item) || '').trim();
            if (!text) return;
            if (seen.has(text)) return;
            seen.add(text);
            out.push(text);
        });
        return out;
    }

    function normalizeColorsMap(rawMap) {
        const safe = ensureObject(rawMap);
        const next = {};
        Object.entries(safe).forEach((entry) => {
            const key = String(entry[0] || '').trim();
            const value = String(parseSimpleYamlValue(entry[1]) || '').trim();
            if (!key || !value) return;
            next[key] = value;
        });
        return next;
    }

    function normalizeColorChangeMap(rawMap) {
        const safe = ensureObject(rawMap);
        const next = {};
        Object.entries(safe).forEach((entry) => {
            const key = String(entry[0] || '').trim();
            if (!key) return;
            const list = normalizeStringList(entry[1]);
            if (!list.length) return;
            next[key] = list;
        });
        return next;
    }

    function hasRenderableYamlValue(value) {
        if (value == null) return false;
        if (Array.isArray(value)) {
            return value.some((item) => hasRenderableYamlValue(item));
        }
        if (typeof value === 'object') {
            return Object.values(value).some((item) => hasRenderableYamlValue(item));
        }
        if (typeof value === 'string') {
            return value.trim().length > 0;
        }
        return true;
    }

    function parseFrontMatterBlock(yamlText) {
        const metadata = {};
        const lines = String(yamlText || '').replace(/\r\n/g, '\n').split('\n');
        const stack = [{ indent: -1, container: metadata }];

        for (let i = 0; i < lines.length; i += 1) {
            const rawLine = String(lines[i] || '');
            const trimmed = rawLine.trim();
            if (!trimmed || trimmed.startsWith('#')) continue;

            const indent = countIndent(rawLine);
            while (stack.length > 1 && indent <= stack[stack.length - 1].indent) {
                stack.pop();
            }
            const current = stack[stack.length - 1];
            const parent = current ? current.container : metadata;

            if (trimmed.startsWith('- ')) {
                if (!Array.isArray(parent)) continue;
                const item = parseSimpleYamlValue(trimmed.slice(2));
                if (item === '' || item == null) continue;
                parent.push(item);
                continue;
            }

            const keyMatch = trimmed.match(/^([A-Za-z0-9_-]+)\s*:\s*(.*)$/);
            if (!keyMatch) continue;
            const key = String(keyMatch[1] || '').trim();
            const tail = String(keyMatch[2] || '').trim();
            if (!key) continue;

            if (tail) {
                if (parent && typeof parent === 'object') {
                    parent[key] = parseSimpleYamlValue(tail);
                }
                continue;
            }

            let nextContainer = null;
            for (let j = i + 1; j < lines.length; j += 1) {
                const probe = String(lines[j] || '');
                const probeTrimmed = probe.trim();
                if (!probeTrimmed || probeTrimmed.startsWith('#')) continue;
                const probeIndent = countIndent(probe);
                if (probeIndent <= indent) break;
                nextContainer = probeTrimmed.startsWith('- ') ? [] : {};
                break;
            }

            if (parent && typeof parent === 'object') {
                if (nextContainer != null) {
                    parent[key] = nextContainer;
                    stack.push({ indent, container: nextContainer });
                    continue;
                }
                parent[key] = '';
            }
        }

        return metadata;
    }

    function parseFrontMatter(markdownText) {
        const text = String(markdownText || '').replace(/\r\n/g, '\n');
        const frontMatterMatch = text.match(/^---\s*\n([\s\S]*?)\n---\s*\n?/);
        if (!frontMatterMatch) {
            return {
                hasFrontMatter: false,
                frontMatter: '',
                metadata: {},
                body: text
            };
        }

        const yamlText = String(frontMatterMatch[1] || '');
        const body = text.slice(frontMatterMatch[0].length);
        return {
            hasFrontMatter: true,
            frontMatter: yamlText,
            metadata: parseFrontMatterBlock(yamlText),
            body
        };
    }

    function applyMetadataDefaults(metadata) {
        const base = ensureObject(metadata);
        const merged = {
            ...base
        };

        merged.title = String(merged.title || '').trim();
        merged.author = String(merged.author || '').trim();
        merged.topic = String(merged.topic || 'article-contribution').trim() || 'article-contribution';
        merged.description = String(merged.description || '').trim();
        merged.order = normalizeMetaNumberInput(merged.order);
        merged.difficulty = String(merged.difficulty || 'beginner').trim() || 'beginner';
        merged.time = String(merged.time || '').trim();
        merged.category = String(merged.category || '').trim();
        merged.date = String(merged.date || '').trim();
        merged.last_updated = String(merged.last_updated || '').trim();
        merged.next_chapter = String(merged.next_chapter || '').trim();
        merged.prev_chapter = String(merged.prev_chapter || '').trim();
        merged.source_cs = normalizeStringList(merged.source_cs);
        merged.prefix = normalizePrefixEntries(merged.prefix);
        merged.min_c = normalizeMetaNumberInput(merged.min_c);
        merged.min_t = normalizeMetaNumberInput(merged.min_t);
        merged.colors = normalizeColorsMap(merged.colors);
        merged.colorChange = normalizeColorChangeMap(merged.colorChange);

        if (!['beginner', 'intermediate', 'advanced'].includes(merged.difficulty)) {
            merged.difficulty = 'beginner';
        }

        return merged;
    }

    function formatYamlScalar(value) {
        if (value == null) return '';
        if (typeof value === 'boolean') return value ? 'true' : 'false';
        if (typeof value === 'number') {
            if (!Number.isFinite(value)) return '';
            return String(value);
        }
        const text = String(value);
        if (!text) return '';
        const needsQuote = /[:#\n\r]/.test(text)
            || /^\s|\s$/.test(text)
            || /["'\[\]\{\}]/.test(text);
        if (!needsQuote) return text;
        return `"${text.replace(/\\/g, '\\\\').replace(/"/g, '\\"')}"`;
    }

    function appendYamlField(lines, key, value, indentLevel) {
        const safeKey = String(key || '').trim();
        if (!safeKey) return;
        const indent = '  '.repeat(indentLevel || 0);

        if (Array.isArray(value)) {
            const filtered = value.filter((item) => hasRenderableYamlValue(item));
            if (!filtered.length) return;
            lines.push(`${indent}${safeKey}:`);
            filtered.forEach((item) => {
                if (item && typeof item === 'object' && !Array.isArray(item)) {
                    const entries = Object.entries(item).filter((entry) => hasRenderableYamlValue(entry[1]));
                    if (!entries.length) return;
                    lines.push(`${indent}  -`);
                    entries.forEach((entry) => {
                        appendYamlField(lines, entry[0], entry[1], (indentLevel || 0) + 2);
                    });
                    return;
                }
                const scalar = formatYamlScalar(item);
                if (!scalar && scalar !== '0' && scalar !== 'false') return;
                lines.push(`${indent}  - ${scalar}`);
            });
            return;
        }

        if (value && typeof value === 'object') {
            const entries = Object.entries(value).filter((entry) => hasRenderableYamlValue(entry[1]));
            if (!entries.length) return;
            lines.push(`${indent}${safeKey}:`);
            entries.forEach((entry) => {
                appendYamlField(lines, entry[0], entry[1], (indentLevel || 0) + 1);
            });
            return;
        }

        const scalar = formatYamlScalar(value);
        if (!scalar && scalar !== '0' && scalar !== 'false') return;
        lines.push(`${indent}${safeKey}: ${scalar}`);
    }

    function buildFrontMatterLines(metadata) {
        const m = applyMetadataDefaults(metadata);
        const lines = ['---'];

        appendYamlField(lines, 'title', m.title || '新文章', 0);
        appendYamlField(lines, 'author', m.author || '', 0);
        appendYamlField(lines, 'topic', m.topic || 'article-contribution', 0);
        appendYamlField(lines, 'description', m.description || '', 0);
        appendYamlField(lines, 'order', m.order || '100', 0);
        appendYamlField(lines, 'difficulty', m.difficulty || 'beginner', 0);
        appendYamlField(lines, 'time', m.time || '', 0);
        appendYamlField(lines, 'category', m.category || '', 0);
        appendYamlField(lines, 'date', m.date || '', 0);
        appendYamlField(lines, 'last_updated', m.last_updated || '', 0);
        appendYamlField(lines, 'next_chapter', m.next_chapter || '', 0);
        appendYamlField(lines, 'prev_chapter', m.prev_chapter || '', 0);
        appendYamlField(lines, 'source_cs', m.source_cs, 0);
        appendYamlField(lines, 'prefix', m.prefix, 0);
        appendYamlField(lines, 'min_c', m.min_c || '', 0);
        appendYamlField(lines, 'min_t', m.min_t || '', 0);
        appendYamlField(lines, 'colors', m.colors, 0);
        appendYamlField(lines, 'colorChange', m.colorChange, 0);

        const unknownKeys = Object.keys(m)
            .filter((key) => !KNOWN_FIELD_SET.has(key))
            .sort();
        unknownKeys.forEach((key) => {
            appendYamlField(lines, key, m[key], 0);
        });

        lines.push('---', '');
        return lines;
    }

    function buildFrontMatterText(metadata) {
        return `${buildFrontMatterLines(metadata).join('\n')}`;
    }

    function mergeFrontMatter(markdownText, metadata) {
        const parsed = parseFrontMatter(markdownText);
        const body = String(parsed.body || markdownText || '').replace(/^\s+/, '');
        const currentMeta = ensureObject(parsed.metadata);
        const incomingMeta = ensureObject(metadata);
        const mergedMeta = applyMetadataDefaults({
            ...currentMeta,
            ...incomingMeta
        });
        const front = buildFrontMatterText(mergedMeta);
        return `${front}${body}`;
    }

    function ensureFrontMatter(markdownText, metadata) {
        const parsed = parseFrontMatter(markdownText);
        if (parsed.hasFrontMatter) return String(markdownText || '');
        return mergeFrontMatter(markdownText, metadata || {});
    }

    return {
        DEFAULT_METADATA,
        KNOWN_FIELD_ORDER,
        parseFrontMatter,
        applyMetadataDefaults,
        buildFrontMatterLines,
        buildFrontMatterText,
        mergeFrontMatter,
        ensureFrontMatter,
        normalizeMetaNumberInput
    };
});
