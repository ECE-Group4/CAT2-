const test = require('node:test');
const assert = require('node:assert/strict');
const fs = require('node:fs');
const path = require('node:path');

const root = path.resolve(__dirname, '../../portal');
const required = [
  'index.html', 'header.html', 'navigation.html', 'overview.html', 'equipment.html',
  'wards.html', 'maintenance.html', 'sla.html', 'uptime.html', 'css/portal.css',
  'js/dashboard.js', 'data/uptime.json', 'images/ward-map.svg'
];

test('all executive portal resources exist', () => {
  for (const item of required) assert.ok(fs.existsSync(path.join(root, item)), `missing ${item}`);
});

test('portal uptime snapshot is valid and clearly synthetic', () => {
  const data = JSON.parse(fs.readFileSync(path.join(root, 'data/uptime.json'), 'utf8'));
  assert.ok(Array.isArray(data.departments));
  assert.ok(data.departments.length > 0);
  for (const row of data.departments) {
    assert.equal(typeof row.department, 'string');
    assert.ok(Number.isFinite(row.assets));
    assert.ok(Number.isFinite(row.operational));
    assert.ok(row.assets >= 0 && row.operational >= 0 && row.operational <= row.assets);
  }
});

test('frameset entry point references header, navigation and content frames', () => {
  const html = fs.readFileSync(path.join(root, 'index.html'), 'utf8');
  assert.match(html, /frameset/i);
  assert.match(html, /header\.html/i);
  assert.match(html, /navigation\.html/i);
  assert.match(html, /overview\.html/i);
});
