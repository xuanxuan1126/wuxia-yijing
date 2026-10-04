# Unity 迁移素材记录

本次沿用 HTML 0.11.10 当前实际加载的角色、跑步、武器、首领、山水、纸张、图标、原创音乐、法阵与命中特效。历史资料保留在 HTML素材来源.md；未加载第一代授权不明确的角色包。

原 PNG 直接复制；SVG 用 Sharp 栅格化为透明 PNG，保留原署名和许可。增益图集是 Delapouite / Lorc 的 CC BY 3.0 图案改编；九霄引雷保留云纹朱砂雷符。源 SVG 与署名文件随项目保存在 Attribution/。

Unity 的标题像素文字和骷髅纹饰由本项目生成，重用既有样式。像素骷髅使用原代码调色盘；未复制用户参考图的像素。音效按原游戏柔和的噪声和包络重制，非真人乐器或刀剑录音。

为保证其他电脑上的中文可显示，新增 Noto Serif CJK SC Regular，来自官方 notofonts/noto-cjk 项目，SIL Open Font License 1.1。完整字体和 OFL.txt 在 Assets/Wuxia/Resources/Fonts/。

- 官方字体：https://github.com/notofonts/noto-cjk
- 原游戏：https://github.com/Electro-Dig/lantern-ruins
- 山水作者 Crisisworks（CC BY 4.0）：https://opengameart.org/content/asian-mountain-forest-bg
- 纸张作者 Mattias Lejbrink（CC0）：https://opengameart.org/content/parchment
- 敌方火剑 MELLE / Melissa Krautheim（CC0）：https://opengameart.org/content/fantasy-sword-set
- 剑气 tbbk（CC0）：https://opengameart.org/content/pixel-art-sword-slash-effect
- 技能图标 Delapouite、Lorc（CC BY 3.0）：https://game-icons.net/

Unity 编辑器与运行时依照 Unity 自身的许可；不将 Unity 软件包含在项目源代码的 MIT 范围内。

## Unity 优化版新增 CC0 资源

见 [完整来源及修改记录](Attribution/Unity优化版/来源与修改说明.md)：Cethiel 金属窄剑、蓝/紫六帧剑光，Ogrebane 木纹练武台。原始下载包一起保留。地震素材恢复 HTML spikes 图块，卷轴颜色按原版米白纸/墨字重建。

## Unity 1.2

按用户要求恢复原 blade 武器，Cethiel 新剑文件仅作为之前迭代素材记录保留；剑光动画仍在使用。应用图标由内置 imagegen 生成，非 CC0 下载素材，原图与完整提示词见应用图标制作说明。
