var __assign = (this && this.__assign) || function () {
    __assign = Object.assign || function(t) {
        for (var s, i = 1, n = arguments.length; i < n; i++) {
            s = arguments[i];
            for (var p in s) if (Object.prototype.hasOwnProperty.call(s, p))
                t[p] = s[p];
        }
        return t;
    };
    return __assign.apply(this, arguments);
};
var __spreadArray = (this && this.__spreadArray) || function (to, from, pack) {
    if (pack || arguments.length === 2) for (var i = 0, l = from.length, ar; i < l; i++) {
        if (ar || !(i in from)) {
            if (!ar) ar = Array.prototype.slice.call(from, 0, i);
            ar[i] = from[i];
        }
    }
    return to.concat(ar || Array.prototype.slice.call(from));
};
var BASE_ROOMS = [
    { name: '青竹山径', subtitle: '第一章 / BAMBOO PATH', width: 2200,
        platforms: [{ x: 0, y: 620, w: 640 }, { x: 735, y: 620, w: 600 }, { x: 1420, y: 620, w: 780 }, { x: 380, y: 520, w: 160 }, { x: 865, y: 490, w: 170 }, { x: 1110, y: 425, w: 160 }, { x: 1550, y: 510, w: 180 }],
        enemies: [{ kind: 'crawler', x: 1690, y: 575, min: 1470, max: 1910 }],
        exits: [{ x: 2110, y: 565, to: 1, spawnX: 130, spawnY: 545, label: '进入悬桥古道' }] },
    { name: '悬桥古道', subtitle: '第二章 / CLOUD BRIDGE', width: 3200,
        platforms: [{ x: 0, y: 620, w: 790 }, { x: 880, y: 620, w: 700 }, { x: 1670, y: 620, w: 430 }, { x: 2410, y: 620, w: 790 }, { x: 425, y: 495, w: 185 }, { x: 720, y: 330, w: 160 }, { x: 1030, y: 455, w: 160 }, { x: 1330, y: 500, w: 190 }, { x: 1715, y: 490, w: 200 }, { x: 1750, y: 380, w: 150 }, { x: 2720, y: 505, w: 210 }],
        enemies: [{ kind: 'crawler', x: 550, y: 580, min: 200, max: 740 }, { kind: 'moth', x: 1010, y: 355, min: 900, max: 1270 }, { kind: 'crawler', x: 1440, y: 580, min: 960, max: 1510 }, { kind: 'moth', x: 1790, y: 380, min: 1640, max: 1970 }],
        exits: [{ x: 85, y: 565, to: 0, spawnX: 2040, spawnY: 545, label: '返回青竹山径' }, { x: 1870, y: 565, to: 2, spawnX: 140, spawnY: 545, label: '下行 · 藏剑石窟' }, { x: 3090, y: 565, to: 3, spawnX: 180, spawnY: 545, label: '进入镇岳山门', requiresDash: true }] },
    { name: '藏剑石窟', subtitle: '修行支线 / SWORD SHRINE', width: 2000,
        platforms: [{ x: 0, y: 620, w: 510 }, { x: 620, y: 620, w: 420 }, { x: 1150, y: 620, w: 850 }, { x: 325, y: 500, w: 110 }, { x: 580, y: 330, w: 145 }, { x: 830, y: 465, w: 150 }, { x: 1180, y: 495, w: 170 }, { x: 1465, y: 540, w: 185 }],
        enemies: [{ kind: 'moth', x: 770, y: 350, min: 640, max: 1010 }, { kind: 'crawler', x: 1340, y: 580, min: 1200, max: 1510 }],
        exits: [{ x: 80, y: 565, to: 1, spawnX: 1770, spawnY: 545, label: '返回悬桥古道' }, { x: 1840, y: 565, to: 1, spawnX: 1940, spawnY: 545, label: '开启捷径 · 返回断桥', requiresDash: true }] },
    { name: '镇岳山门', subtitle: '第三章 / MOUNTAIN SEAL', width: 1900,
        platforms: [{ x: 0, y: 620, w: 1900 }, { x: 400, y: 495, w: 150 }, { x: 1370, y: 495, w: 150 }], enemies: [], exits: [] },
    { name: '归元禁庭', subtitle: '第四章 / THE FALLEN MASTER', width: 1280, platforms: [{ x: 0, y: 620, w: 1280 }], enemies: [], exits: [] }
];
// Compact the first two rooms; preserve every authored gap, including the 310px dash bridge.
var LAND_SCALE = [1.65, 1.8, 3, 1];
export function worldX(room, x) {
    if (room >= 3)
        return x;
    var floors = BASE_ROOMS[room].platforms.filter(function (p) { return p.y === 620; }).sort(function (a, b) { return a.x - b.x; });
    var removed = 0;
    for (var i = 0; i < floors.length - 1; i++) {
        var a = floors[i].x + floors[i].w, b = floors[i + 1].x;
        removed += Math.max(0, Math.min(x, b) - a) * (LAND_SCALE[room] - 1);
    }
    return x * LAND_SCALE[room] - removed;
}
export var ROOMS = BASE_ROOMS.map(function (base, index) {
    if (index >= 3)
        return base;
    var wx = function (x) { return worldX(index, x); };
    var platforms = base.platforms.map(function (p) { return (__assign(__assign({}, p), { x: wx(p.x), w: p.y === 620 ? wx(p.x + p.w) - wx(p.x) : p.w * LAND_SCALE[index] })); });
    var protectedX = __spreadArray(__spreadArray(__spreadArray(__spreadArray(__spreadArray([], base.exits.map(function (e) { return wx(e.x); }), true), base.enemies.map(function (e) { return wx(e.x); }), true), [wx(150)], false), (index === 2 ? [wx(1600)] : []), true), (index === 1 ? [wx(1815), wx(1940), wx(2070)] : []), true);
    var floors = [];
    for (var _i = 0, _a = platforms.filter(function (p) { return p.y === 620; }); _i < _a.length; _i++) {
        var p = _a[_i];
        var start = p.x;
        var _loop_1 = function (cut) {
            if (protectedX.some(function (x) { return Math.abs(x - cut) < 210; }) || platforms.some(function (q) { return q.y !== 620 && cut + 140 > q.x - 100 && cut < q.x + q.w + 100; }))
                return "continue";
            floors.push({ x: start, y: 620, w: cut - start });
            start = cut + (index === 0 ? 95 : 110);
        };
        for (var cut = p.x + 650; cut < p.x + p.w - 400; cut += 820) {
            _loop_1(cut);
        }
        floors.push({ x: start, y: 620, w: p.x + p.w - start });
    }
    var enemies = base.enemies.map(function (e) { return (__assign(__assign({}, e), { x: wx(e.x), min: wx(e.min), max: wx(e.max) })); });
    var added = 0;
    var maxAdded = index === 1 ? 4 : 2;
    var _loop_2 = function (p) {
        if (added >= maxAdded || p.w < 620 || p.x < 400)
            return "continue";
        var x = p.x + p.w * .6;
        if (protectedX.some(function (v) { return Math.abs(v - x) < 210; }) || enemies.some(function (e) { return Math.abs(e.x - x) < 340; }))
            return "continue";
        enemies.push({ kind: added % 2 ? 'moth' : 'crawler', x: x, y: added % 2 ? 425 : 575, min: p.x + 65, max: p.x + p.w - 65 });
        added++;
    };
    for (var _b = 0, floors_1 = floors; _b < floors_1.length; _b++) {
        var p = floors_1[_b];
        _loop_2(p);
    }
    var _loop_3 = function (e) {
        if (e.kind === 'crawler') {
            var floor = floors.find(function (p) { return e.x >= p.x && e.x <= p.x + p.w; });
            if (floor) {
                e.min = Math.max(e.min, floor.x + 48);
                e.max = Math.min(e.max, floor.x + floor.w - 48);
            }
        }
    };
    for (var _c = 0, enemies_1 = enemies; _c < enemies_1.length; _c++) {
        var e = enemies_1[_c];
        _loop_3(e);
    }
    return __assign(__assign({}, base), { width: wx(base.width), platforms: __spreadArray(__spreadArray([], floors, true), platforms.filter(function (p) { return p.y !== 620; }), true), enemies: enemies, exits: base.exits.map(function (e) { return (__assign(__assign({}, e), { x: wx(e.x), spawnX: worldX(e.to, e.spawnX) })); }) });
});
export var ASSETS = { qiSlash: 'assets/qi-slash-cc0.png', upgradeIcons: 'assets/upgrade-icons-ccby3-v32.svg', formationOuter: 'assets/formation-outer-v16.svg', formationInner: 'assets/formation-inner-v16.svg', swordImpact: 'assets/sword-impact-v15.svg', swordPierce: 'assets/sword-pierce-v15.svg', ruins: 'assets/wuxia-valley.png', roomAtlas: 'assets/wuxia-rooms.png', wuxiaAtlas: 'assets/wuxia-atlas.png', hero: 'assets/white-swordsman-v2.png', bossAtlas: 'assets/golem-motion-v2.png', blade: 'assets/jian-v3.png', heroAttack: 'assets/swordsman-melee-v3.png', bandit: 'assets/bandit-v3.png', heroMotion: 'assets/swordsman-motion-v4.png', craneMotion: 'assets/crane-motion-v4.png', taiji: 'assets/taiji-inspired.svg', enemyBlade: 'assets/enemy-fire-sword-cc0.png', heroRun: 'assets/swordsman-full-run-v8.png', banditRun: 'assets/bandit-full-run-v8.png', heroDraw: 'assets/swordsman-draw-v5.png', masterAtlas: 'assets/master-v9.png', finalBg: 'assets/final-sanctuary-v9.png', storyArt: 'assets/story-four-panels-v9.png', storyPast: 'assets/story-past-v12.png', mountainStudy: 'assets/mountain-forest-ccby4.png', parchment: 'assets/parchment-cc0.png' };
export var STORY_IS_COMPOSED = true;
export var MUSIC_ASSETS = { musicExplore: 'assets/stream-flute-v18.wav', musicMaster: 'assets/quiet-seal-v18.wav' };
