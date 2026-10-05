# coding: utf-8
"""Package complete Unity sources plus independent offline desktop players."""
from pathlib import Path
import shutil,zipfile,hashlib,json,os,tempfile
source=Path(__file__).resolve().parents[1]
destination=Path.home()/'Desktop'/'最终版'
if destination.exists():
 manifest=destination/'文件校验.json'
 if not manifest.exists():raise SystemExit('Unmanaged destination exists: '+str(destination))
 previous=json.loads(manifest.read_text(encoding='utf-8'))
 for name,digest in previous.items():
  p=destination/name
  if not p.is_file() or hashlib.sha256(p.read_bytes()).hexdigest()!=digest:raise SystemExit('User changes detected; refusing to overwrite: '+str(p))
destination.mkdir(exist_ok=True)
for name in ['Assets','Packages','ProjectSettings','Documentation','Tools']:
 shutil.copytree(source/name,destination/name,dirs_exist_ok=True,ignore=shutil.ignore_patterns('.DS_Store','__pycache__'))
for name in ['README.md','CONTRIBUTING.md','LICENSE','.gitignore']:
 shutil.copy2(source/name,destination/name)
players=destination/'试玩程序';players.mkdir(exist_ok=True)
for platform in ['macOS','Windows','Linux']:
 shutil.copytree(source/'Builds'/platform,players/platform,symlinks=True,dirs_exist_ok=True,ignore=shutil.ignore_patterns('*_BurstDebugInformation_DoNotShip','.DS_Store'))
launcher=players/'Linux'/'开始游戏.sh'
launcher.write_text('#!/bin/sh\ncd -- "$(dirname -- "$0")" || exit 1\nexec ./WuxiaYijing.x86_64 "$@"\n',encoding='utf-8');launcher.chmod(0o755)
controls='A/D 或方向键移动；空格跳跃；J 挥剑；K 飞剑；Shift 御剑；L 万剑归宗；E 交互；Esc 暂停；H 返回主页；M 静音。'
guide='武侠遗境 Unity 1.7.1 · 离线完整版\n\n完整解压本包，然后选择你系统的启动程序：\nWindows：试玩程序/Windows/WuxiaYijing.exe\nMac：试玩程序/macOS/武侠遗境.app（Apple Silicon / Intel 均可）\nLinux：试玩程序/Linux/开始游戏.sh\n\n无需安装 Unity、Node 或 Python，无需联网。必须保留整个系统文件夹，不要单独发送 exe 或移动 Data 文件夹。\n系统要求：Windows 10 21H1+ / 11 x64；macOS 12+；Linux Ubuntu 22.04 / 24.04 x64。\nMac 首次若被系统拦截，可在系统设置「隐私与安全性」允许打开。Linux 如执行权限丢失，可在文件属性允许执行。\n\n'+controls+'\n\n编辑游戏才需 Unity：Hub 添加本文件夹，Unity 6000.3.24f1，打开 Assets/Wuxia/Scenes/WuxiaMain.unity 点 Play。\n完整说明见 README.md。素材出处与许可证在 Documentation 中。\n'
(destination/'先看这里.txt').write_text(guide,encoding='utf-8')
files={}
for p in sorted(destination.rglob('*')):
 if p.is_file() and not p.is_symlink() and p.name!='文件校验.json':files[str(p.relative_to(destination))]=hashlib.sha256(p.read_bytes()).hexdigest()
(destination/'文件校验.json').write_text(json.dumps(files,ensure_ascii=False,indent=2),encoding='utf-8')
archive=destination.parent/(destination.name+'.zip')
# zipfile preserves Unix execute permissions; record symlinks so the .app remains intact.
with zipfile.ZipFile(archive,'w',zipfile.ZIP_DEFLATED,compresslevel=6) as z:
 for p in sorted(destination.rglob('*')):
  rel=str(Path(destination.name)/p.relative_to(destination))
  if p.is_symlink():
   info=zipfile.ZipInfo(rel);info.create_system=3;info.external_attr=(0o120777<<16);z.writestr(info,os.readlink(p))
  elif p.is_file():z.write(p,rel)
with zipfile.ZipFile(archive) as z:
 bad=z.testzip()
 if bad:raise SystemExit('ZIP integrity error: '+bad)
print(destination)
print(archive)
print('Verified',len(files),'files; archive MiB',round(archive.stat().st_size/1048576,1))

staging=tempfile.TemporaryDirectory(prefix='wuxia-share-')
for platform in ['Windows','macOS','Linux']:
 playerroot=Path(staging.name)/('武侠遗境_1.7.1_'+platform+'_离线试玩')
 if playerroot.exists():raise SystemExit('Player destination already exists: '+str(playerroot))
 playerroot.mkdir()
 shutil.copytree(players/platform,playerroot/platform,symlinks=True)
 license_root=playerroot/'素材许可'
 for name in ['Attribution','HTML素材来源.md','Unity素材来源.md']:
  src=source/'Documentation'/name
  if src.is_dir():shutil.copytree(src,license_root/name)
  elif src.is_file():license_root.mkdir(exist_ok=True);shutil.copy2(src,license_root/name)
 shutil.copy2(source/'LICENSE',playerroot/'LICENSE')
 entry={'Windows':'Windows/WuxiaYijing.exe','macOS':'macOS/武侠遗境.app','Linux':'Linux/开始游戏.sh'}[platform]
 playerguide='武侠遗境 Unity 1.7.1 · '+platform+' 离线试玩\n\n完整解压，然后打开 '+entry+'。\n无需安装 Unity、Node 或 Python，无需联网。保留整个文件夹，不要只发送主程序。\n\n'+controls+'\n\nMac 首次若被系统拦截，可在系统设置「隐私与安全性」允许打开。Linux 若解压软件丢失执行权限，可在文件属性中允许执行。\n\n来源与许可见「素材许可」文件夹。完整可编辑 Unity 项目请使用跨平台完整项目包。\n'
 (playerroot/'开始前请看.txt').write_text(playerguide,encoding='utf-8')
 (playerroot/'文件校验.json').write_text(json.dumps({str(p.relative_to(playerroot)):hashlib.sha256(p.read_bytes()).hexdigest() for p in sorted(playerroot.rglob('*')) if p.is_file() and not p.is_symlink()},ensure_ascii=False,indent=2),encoding='utf-8')
 playerzip=destination.parent/(playerroot.name+'.zip')
 with zipfile.ZipFile(playerzip,'w',zipfile.ZIP_DEFLATED,compresslevel=6) as z:
  for p in sorted(playerroot.rglob('*')):
   rel=str(Path(playerroot.name)/p.relative_to(playerroot))
   if p.is_symlink():
    info=zipfile.ZipInfo(rel);info.create_system=3;info.external_attr=0o120777<<16;z.writestr(info,os.readlink(p))
   elif p.is_file():z.write(p,rel)
 with zipfile.ZipFile(playerzip) as z:
  if z.testzip():raise SystemExit('Corrupt player ZIP: '+platform)
 print('Player package:',playerzip,'MiB',round(playerzip.stat().st_size/1048576,1))

staging.cleanup()
