"""Check offline runtime contents and native CPU targets without installing runtimes."""
from pathlib import Path
import struct,subprocess,json
root=Path(__file__).resolve().parents[1]
checks=[]
def check(ok,message):
 if not ok:raise RuntimeError(message)
 checks.append(message)
def required(platform,name):
 p=root/'Builds'/platform/name
 check(p.is_file() and p.stat().st_size>0,platform+': '+name+' present')
 return p
for platform in ['Windows','Linux']:
 for name in ['globalgamemanagers','resources.assets','resources.resource','Managed/Assembly-CSharp.dll','Managed/UnityEngine.CoreModule.dll']:
  required(platform,'WuxiaYijing_Data/'+name)
exe=required('Windows','WuxiaYijing.exe')
required('Windows','UnityPlayer.dll');required('Windows','MonoBleedingEdge/EmbedRuntime/mono-2.0-bdwgc.dll')
# Unity's player/Mono imports must resolve from the package or Windows itself.
system={'kernel32.dll','user32.dll','version.dll','ole32.dll','shlwapi.dll','setupapi.dll','advapi32.dll','gdi32.dll','shell32.dll','opengl32.dll','winmm.dll','oleaut32.dll','imm32.dll','iphlpapi.dll','winhttp.dll','bcrypt.dll','hid.dll','d3d11.dll','dxgi.dll','uiautomationcore.dll','crypt32.dll','secur32.dll','ws2_32.dll','dwmapi.dll','dbghelp.dll','psapi.dll','wininet.dll','ntdll.dll'}
local={p.name.lower() for p in (root/'Builds/Windows').rglob('*.dll')}
imports_report={}
for p in (root/'Builds/Windows').rglob('*'):
 if p.suffix.lower() not in ['.exe','.dll'] or 'Managed' in p.parts:continue
 b=p.read_bytes();pe=struct.unpack_from('<I',b,60)[0];check(b[pe:pe+4]==b'PE\0\0',p.name+' valid PE')
 check(struct.unpack_from('<H',b,pe+4)[0]==0x8664,p.name+' x64')
 n=struct.unpack_from('<H',b,pe+6)[0];opt=pe+24;sz=struct.unpack_from('<H',b,pe+20)[0]
 check(struct.unpack_from('<H',b,opt)[0]==0x20b,p.name+' PE64')
 sections=[struct.unpack_from('<IIII',b,opt+sz+i*40+8) for i in range(n)]
 def raw(rva):
  for vs,va,rs,rp in sections:
   if va<=rva<va+max(vs,rs):return rp+rva-va
  return rva
 rva=struct.unpack_from('<I',b,opt+120)[0];imports=[]
 if rva:
  o=raw(rva)
  while (name:=struct.unpack_from('<I',b,o+12)[0]):
   off=raw(name);imports.append(b[off:b.index(b'\0',off)].decode().lower());o+=20
 missing=[x for x in imports if x not in local and x not in system and not x.startswith(('api-ms-win-','ext-ms-win-'))]
 check(not missing,p.name+' imports resolve to bundled or OS libraries')
 imports_report[str(p.relative_to(root/'Builds/Windows'))]=imports
linux=required('Linux','WuxiaYijing.x86_64');b=linux.read_bytes()
check(b[:5]==b'\x7fELF\x02' and struct.unpack_from('<H',b,18)[0]==62,'Linux ELF64 x86_64')
required('Linux','UnityPlayer.so');required('Linux','WuxiaYijing_Data/MonoBleedingEdge/x86_64/libmonobdwgc-2.0.so')
app=root/'Builds/macOS/武侠遗境.app';native=app/'Contents/MacOS/武侠遗境'
check(native.is_file(),'macOS player present')
archs=subprocess.check_output(['lipo','-archs',str(native)],text=True).split()
check(set(archs)=={'x86_64','arm64'},'macOS universal Intel + Apple Silicon')
subprocess.run(['codesign','--verify','--deep','--strict',str(app)],check=True)
checks.append('macOS application signature and nested bundle verified')
check(native.stat().st_mode & 0o111,'macOS executable permission')
check(linux.stat().st_mode & 0o111,'Linux executable permission')
for target in ['StandaloneOSX','StandaloneWindows64','StandaloneLinux64']:
 p=root/'Documentation/Validation'/('build-'+target+'.txt')
 check('Result: Succeeded' in p.read_text(),target+' build succeeded')
folder=root/'Documentation/Validation'
(folder/'offline-distribution-validation.txt').write_text('\n'.join('PASS '+s for s in checks)+'\n\nWindows/Linux: build and package checks only; native play tested on macOS.\n',encoding='utf-8')
(folder/'windows-native-imports.json').write_text(json.dumps(imports_report,indent=2),encoding='utf-8')
print('Offline distribution checks:',len(checks),'PASS')
