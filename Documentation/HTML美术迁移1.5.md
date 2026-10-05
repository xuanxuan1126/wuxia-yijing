# HTML 美术迁移 · Unity 1.5

参考版本：创作者指定的「武侠遗境_雷符图标版_2026-10-04」，HTML 0.11.10。

## 取舍

原版 heroRun（swordsman-full-run-v8.png）衣褶与站立人物一致，恢复此八帧及84/400缩放。保留基于速度的跑步节奏与60Hz物理插值；两条腿沿用原画同色衣料。近侧袖臂从现有像素按轮廓提取，不重绘人物。腰剑沿用 jian-v3.png 原图与78像素长度，宽度为7像素以兼顾原版修长比例与护手清晰度。剑身/衣袍/护手/近袖四层遮挡继续使用1.4修复。

HTML 剑阵已有高质量 formation-outer-v16.svg、formation-inner-v16.svg、sword-impact-v15.svg 和 sword-pierce-v15.svg。此前 Unity 只复用了部分图像，缺少加色混合和蓄力阶段。新版恢复双环反向旋转、三段展开/释放/淡出，12剑向外辐射、交错射出，曲线渐变轨迹与两层命中特效。继续使用当前Unity的目标锁定与扫掠命中，避免飞剑无故掉头。每把剑最多保存9个轨迹采样点，结束与切关都会清理；命中效果仍使用固定复用池。

选卡恢复 HTML endless-ui.ts 的286×342卡幅与330间距，深玉色全屏背景、细金边、浅色纸面、86像素圆形纹章、60像素原版图标。取消巨大底部卷轴，确认按钮360×53；灰化未选卡、描亮已选卡，明确显示气血变化预览。所有文字与按钮仍为原生UGUI，可鼠标点击；应用增益仍在确认按钮执行。

## 许可与来源

没有从参考截图复制水印或引入新素材。heroRun、hero、blade、banditRun 与指定HTML源码逐文件SHA256一致，见 Validation/html-art-provenance.json；法阵与命中图为原SVG的既有PNG导入，源路径在 Tools/source-assets.json。全部既有授权、作者署名继续见 HTML素材来源.md、Unity素材来源.md 与 Attribution 文件夹。新增Shader、UI几何和移植代码遵循项目MIT许可。

## 平台

macOS 通用架构本机试玩与鼠标交互验证；Windows x64、Linux x64 构建与运行库结构验证。本机没有Windows/Linux实机测试环境，不能把交叉构建称为实机验证。
