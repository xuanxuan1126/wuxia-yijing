"""Restore authored HTML artwork without changing its pixels or other frame metadata."""
from pathlib import Path
import json, hashlib
root = Path(__file__).resolve().parents[1]
reference = Path.home()/'Desktop'/'武侠遗境_雷符图标版_2026-10-04'/'开发源码'/'public'
sources = json.loads((root/'Tools/source-assets.json').read_text())
proof = {}
for key in ('heroRun','hero','blade','banditRun'):
    original=reference/sources[key]
    imported=root/'Assets/Wuxia/Resources/Art'/(key+'.png')
    digest=lambda p:hashlib.sha256(p.read_bytes()).hexdigest()
    assert digest(original)==digest(imported), key+' pixels differ from selected HTML release'
    proof[key]={'html':sources[key],'sha256':digest(imported)}
manifest=root/'Assets/Wuxia/Resources/Data/ArtFrames.json'
data=json.loads(manifest.read_text())
# Contours refer to the original eight authored sleeves, not the discarded Unity 1.3 run.
back=[(213,118),(252,129),(265,157),(267,179),(302,197),(305,221),(285,231),(249,229),(197,221),(155,208),(176,175),(197,141)]
front=[(226,120),(257,134),(257,158),(278,177),(315,177),(336,160),(361,164),(374,177),(370,199),(345,218),(301,229),(251,228),(201,216),(183,199),(199,165)]
for f in data['frames']:
    if f['sheet']!='heroRun':continue
    i=int(f['name']);col=i%4;row=i//4
    ox=[0,440,880,1320][col];oy=0 if row==0 else 422
    points=back if i in (0,2) else front
    f['arm']=[v for px,py in points for v in (px+ox-f['x'],py+oy-f['y'])]
    # Belt follows torso sway in the authored frames, instead of a fixed world position.
    f['beltX']=[295,751,1190,1624,300,751,1190,1624][i]
    f['beltY']=[211,214,211,211,633,636,633,633][i]
manifest.write_text(json.dumps(data,ensure_ascii=False,indent=2))
target=root/'Documentation/Validation/html-art-provenance.json'
target.parent.mkdir(parents=True,exist_ok=True)
target.write_text(json.dumps(proof,ensure_ascii=False,indent=2))
print('Verified original HTML pixels and restored eight sleeve/belt anchors.')
