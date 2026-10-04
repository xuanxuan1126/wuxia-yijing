import fs from 'node:fs/promises';import path from 'node:path';
import sharp from '/Users/xiehaoxuan/.cache/codex-runtimes/codex-primary-runtime/dependencies/node/node_modules/sharp/dist/index.mjs';
const assets=JSON.parse(await fs.readFile('Tools/source-assets.json','utf8'));
for(const [key,file] of Object.entries(assets)){
 const from=path.resolve('../wuxia-game/public',file);
 if(file.endsWith('.wav'))await fs.copyFile(from,`Assets/Wuxia/Resources/Audio/${key}.wav`);
 else if(file.endsWith('.svg'))await sharp(await fs.readFile(from)).png().toFile(`Assets/Wuxia/Resources/Art/${key}.png`);
 else await fs.copyFile(from,`Assets/Wuxia/Resources/Art/${key}.png`);
}
