# 素材来源

## 原项目

源代码基于 [Electro-Dig/lantern-ruins](https://github.com/Electro-Dig/lantern-ruins)，参考提交 `50073daeffe33d6808220f258bbdfca68f16c140`。保留根目录原 MIT LICENSE。原版音乐音效逻辑继续使用。

## 第一代历史素材（角色包已不再加载）

| 文件 | 来源 | 用途 |
| --- | --- | --- |
| 008-1787672703578-frames64.png | FrameRonin 下载的“剑与魔法角色包” | 主角现成像素帧 |
| 012-1787673021553-frames64.png | 同一角色包 | 地面敌人 |
| wuxia-valley.png | 本次 OpenAI 图像工具生成 | 标题背景 |
| wuxia-rooms.png | 本次 OpenAI 图像工具生成 | 四区背景图集 |
| wuxia-atlas.png | 本次 OpenAI 图像工具生成 | 地形、场景道具、敌人、特效 |

角色包 README 标注免费 PNG 和 Godot 演示，但未包含明确的商业授权条款；本地设计验证先使用，公开发行前应另行核实角色包授权。不能将原项目 MIT 自动套用于下载角色。原始 PNG 未改动；游戏加载时通过 `src/wuxia-art.ts` 切分纹理。006、017 是备用候选，没有加载进游戏。

`ruins.png`、`traveler.png`、`creatures.png` 是保留的原版参考文件，当前 `ASSETS` 不再引用。场景生成说明见 `art/WUXIA-PROMPTS.md`。

## 依赖

Phaser / Vite：MIT；TypeScript / Playwright：Apache-2.0。Phaser 许可位于 `public/assets/PHASER-LICENSE.txt`。

## 第二代追加与替换

- `jade-sword-cc0.png`：作者 Cethiel，[Swords – Set 1](https://opengameart.org/content/swords-set-1-0)，页面标注 CC0。原始包保存在 `art/research/cethiel-swords-cc0.zip`；使用 Sword_1/01.png，不做文件像素修改，游戏内旋转缩放。
- `white-swordsman-v2.png`：本次生成的白衣中国剑客12姿势图集，替换008主角，008保留为历史素材。
- `golem-motion-v2.png`：本次生成的镇山傀儡8姿势图集，替换第一版单张Boss图。
- 012人物继续作为人形小怪；它原有授权核实事项仍适用。

搜索记录与未采用候选：[二代素材筛选](art/research/ASSET-RESEARCH-V2.md)。生成提示说明见 `art/WUXIA-PROMPTS.md`。

## 第三代当前使用

- `bandit-v3.png`：本次生成的黑衣红巾山匪八姿势，参考 LuizMelo 的 [Martial Hero](https://luizmelo.itch.io/martial-hero)（CC0）下载跑步图集，以及第二代白衣剑客的细节风格。参考留档 `art/research/martial-hero-cc0/Run.png`。当前加载的是重新生成素材，不是原作者的原图。
- `swordsman-melee-v3.png`：本次生成的白衣剑客八帧持剑挥砍。
- `jian-v3.png`：本次生成的玉柄中国直剑；第二代 Cethiel 剑仅保留为参考。
- 第二代白衣剑客、Boss和山水场景继续使用。第一代008、012角色不再加载。

CC0参考的许可不自动赋予生成素材同一许可证。代码MIT许可与图片来源分开记录。详情见 [第三代素材筛选](art/research/ASSET-RESEARCH-V3.md)。

## 第四代生成素材

- `swordsman-motion-v4.png`：以第二代白衣剑客为参考，本次重新生成八帧跑步与八帧持剑挥砍；游戏运行时切分与校准挂点，不修改原PNG。替代跑步与近战旧姿势。
- `crane-motion-v4.png`：本次生成的红白纸鹤四帧振翅图集，替代静态纸鹤显示。
- 所有新增图均为图像工具生成素材，不冒充网上原版开源图集；第三代CC0参考记录继续保留。

## 第五代

- `enemy-fire-sword-cc0.png`：MELLE / Melissa Krautheim 的 [FANTASY-sword-set](https://opengameart.org/content/fantasy-sword-set)，CC0，使用原fire_sword.png，独立于主角武器。
- `taiji-inspired.svg`：本项目重新绘制的传统太极几何矢量；Commons公开领域太极图为参考，原文件未成功下载。
- `swordsman-run-v5.png`、`swordsman-draw-v5.png`：本次图像工具生成补帧，并非开源原作者提供的素材。
- 来源、未采用候选与下载情况见 [第五代筛选](art/research/ASSET-RESEARCH-V5.md)。

## 第六代

`swordsman-stride-v6.png`：本次图像工具生成的16帧大跨步、抬膝、蹬地跑步图集，替代第五代跑步。用户上传图仅用于理解佩剑位置、长度与人物遮挡，没有将该图或水印复制进游戏。裸剑继续使用jian-v3.png；近战重新使用无鞘的swordsman-melee-v3.png，带鞘姿势不再显示。

本次内置 imagegen 生成替换跑步素材：`public/assets/swordsman-run-v7.png`、`public/assets/bandit-run-v7.png`。以既有角色图集为设计参考，透明背景4×2、8帧完整接触/下压/经过/抬腿循环，统一比例与定位；生成提示记录 `art/run-v7-prompts.txt`。

完整角色跑步修正：内置imagegen以原站立角色和用户步态参考为参照生成 `public/assets/swordsman-full-run-v8.png` 与 `public/assets/bandit-full-run-v8.png`。保留原画风、服饰及靴子，整身8帧透明图集。运行时使用完整图片，移除程序绘制腿部。完整提示与生成方式记录于 `art/full-body-run-v8-prompts.json`。

## 归元终章 Demo（0.7）
新增掌门动作 `public/assets/master-v9.png` 与最终关背景 `public/assets/final-sanctuary-v9.png`，使用内置 imagegen，以现有角色与场景作画风参考生成。提示记录：`art/chapter4-generation-prompts.json`。
序章最终采用程序排版的四格构图：绘景底图复用 wuxia-rooms.png（以 story-four-panels-v9.png 保存），叠加现有主角、傀儡、归元草、灵符及新增掌门动作，并由代码绘制文字。单独的漫画生成首次网络失败，重试未采用；不把复用底图称作新生成的漫画图片。最终截图 art/chapter4-story-composed.png。

### 0.7.1 视觉布局
三格斜切分镜、魔潮波墙、魔主角骨面饰与云纹托剑徽记为项目原生图形绘制，复用现有已列明的素材，无新增外部下载。

血条像素骷髅为原生图形重新绘制，造型参考用户提供的骷髅图片；未直接复制图片像素。

## 0.8.0 秘卷完善版新增素材

- **Asian Mountain Forest BG** — Crisisworks, CC BY 4.0。
  来源：https://opengameart.org/content/asian-mountain-forest-bg
  文件：`public/assets/mountain-forest-ccby4.png`
  许可：https://creativecommons.org/licenses/by/4.0/
  用途与修改：踏云诀秘卷中的山水底画；运行时缩放、着色和叠加主角，未修改原始文件。
  作者游戏 **Spirit of the Wind**：https://evilartbunny.itch.io/spirit-of-the-wind
- **Parchment** — Mattias Lejbrink（由 Anonymous 提交），CC0。
  来源：https://opengameart.org/content/parchment
  文件：`public/assets/parchment-cc0.png`
  许可：https://creativecommons.org/publicdomain/zero/1.0/
  用途与修改：秘卷与菜单的纸张底纹；运行时缩放、叠色与木轴装饰。
- **少年沈砚 / 未入魔玄衡立绘**：内置 image_gen 根据项目已有角色参考新绘；并非下载的开源素材。
  文件：`public/assets/story-past-v12.png`
  完整提示词与模式记录：`art/story-past-v12-prompt.json`。
  游戏中通过图集裁切显示；原始输出保留，未改动已有战斗角色。
- 卷轴木轴、边框、文字布局：项目原生代码绘制。

说明：上述新下载素材的免费使用与再分发许可已在来源页面核对；本清单不将 AI 生成素材或其他既有第三方素材统一宣称为 CC0。

0.9.0 回风剑阵特效使用项目原生 Phaser 图形、已有剑气素材与声音绘制，无新增第三方素材。


技能反馈素材 0.10.1：sword-impact-v15.svg、sword-pierce-v15.svg 为本项目新绘制的矢量爆光与穿透剑痕，按本项目 MIT 授权。音效由 Web Audio 合成，无外部音频依赖。

法阵素材 0.10.2：formation-outer-v16.svg、formation-inner-v16.svg 为本项目原创剑意法阵纹样，按 MIT 授权。


古风器乐配乐 0.10.3：mountain-xiao-v17.wav（山径清音，72 BPM）与 seal-master-v17.wav（禁庭旧梦，84 BPM），由本项目原创旋律及合成音色制作，模拟箫声、拨弦与柔和持续伴奏，无人声，MIT 授权。生成源码与音频峰值记录见 scripts/music/；非真实民族乐器录音。


柔和器乐0.10.4：stream-flute-v18.wav与quiet-seal-v18.wav为上述原创合成配乐的柔和重编版，模拟笛子与古筝音色，MIT授权。

## 无尽模式新增素材（2026-10-03）
- **Magic spell icons** — steefie92，CC0。https://opengameart.org/content/magic-spell-icons
- 原始SVG：`art/endless-source/spellicons.svg`；用于雷、霜技能卡面的改编图集：`public/assets/rogue-sigils-cc0.svg`。提取雷/冰图案并移除背景、重排，原许可不变。
- 无尽玩法、古籍卡牌排版与程序技能特效：本项目原创，MIT。

沧浪剑气：tbbk, Pixel art sword slash effect，CC0。https://opengameart.org/content/pixel-art-sword-slash-effect


## 0.11.7 增益图标（2026-10-04）

Game-icons.net / game-icons/icons，CC BY 3.0，作者 Delapouite 与 Lorc。许可：https://creativecommons.org/licenses/by/3.0/ 。原始许可与下载记录：`art/upgrade-icons-source/license.txt`、`manifest.json`；原始SVG随该目录保留。

| 增益 | 原图 | 作者 |
|---|---|---|
| 淬锋诀 | [ancient-sword.svg](https://raw.githubusercontent.com/game-icons/icons/master/delapouite/ancient-sword.svg) | Delapouite |
| 流光御剑 | [wingfoot.svg](https://raw.githubusercontent.com/game-icons/icons/master/lorc/wingfoot.svg) | Lorc |
| 易筋锻骨 | [meditation.svg](https://raw.githubusercontent.com/game-icons/icons/master/lorc/meditation.svg) | Lorc |
| 回春丹 | [potion-ball.svg](https://raw.githubusercontent.com/game-icons/icons/master/lorc/potion-ball.svg) | Lorc |
| 护体飞剑 | [swords-power.svg](https://raw.githubusercontent.com/game-icons/icons/master/delapouite/swords-power.svg) | Delapouite |
| 九霄引雷 | [focused-lightning.svg](https://raw.githubusercontent.com/game-icons/icons/master/lorc/focused-lightning.svg) | Lorc |
| 寒潭照影 | [snowflake-1.svg](https://raw.githubusercontent.com/game-icons/icons/master/lorc/snowflake-1.svg) | Lorc |
| 沧浪剑气 | [wave-strike.svg](https://raw.githubusercontent.com/game-icons/icons/master/lorc/wave-strike.svg) | Lorc |
| 金钟护心 | [bell-shield.svg](https://raw.githubusercontent.com/game-icons/icons/master/lorc/bell-shield.svg) | Lorc |
| 饮露归元 | [drop.svg](https://raw.githubusercontent.com/game-icons/icons/master/lorc/drop.svg) | Lorc |
| 青莲养息 | [lotus.svg](https://raw.githubusercontent.com/game-icons/icons/master/lorc/lotus.svg) | Lorc |
| 涅槃归真 | [fire-flower.svg](https://raw.githubusercontent.com/game-icons/icons/master/delapouite/fire-flower.svg) | Delapouite |

修改：移除黑底，改为金、玉、淡蓝配色，重排为4×3图集；各图案轮廓来自上述作者，组合改编仍按CC BY 3.0提供。文件：`public/assets/upgrade-icons-ccby3-v29.svg`。用于三选一增益卡和已学增益列表。旧版雷霜图标在本版卡面由统一新图集替换。

0.11.10：九霄引雷改为云纹朱砂雷符。使用Lorc的focused-lightning（CC BY 3.0）雷纹轮廓，组合符纸、朱砂配色和阶梯云纹。新图集`public/assets/upgrade-icons-ccby3-v32.svg`；独立图案及原SVG在`art/upgrade-icons-source`保存。组合改编CC BY 3.0，其他11项图标保持原轮廓与配色。旧lightning-arc及v29图集保留为历史素材。
