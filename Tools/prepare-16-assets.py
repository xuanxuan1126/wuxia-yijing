"""Slice licensed original animations, and extend the existing SVG icon family."""
from pathlib import Path
from io import BytesIO
from PIL import Image
import json, zipfile, shutil, xml.etree.ElementTree as ET
root=Path(__file__).resolve().parents[1]
sources=root/'Documentation/Attribution/Unity1.6'
art=root/'Assets/Wuxia/Resources/Art'
manifest=root/'Assets/Wuxia/Resources/Data/ArtFrames.json'
data=json.loads(manifest.read_text())
sheets=['boneIdle','boneWalk','boneAttack','eyeFlight','eyeAttack','demonPortal','summonBagua','upgradeIconsExtra']
data['frames']=[f for f in data['frames'] if f['sheet'] not in sheets]
z=zipfile.ZipFile(sources/'Monsters_Creatures_Fantasy.zip')
for sheet,folder,file,count,baseline in [('boneIdle','Skeleton','Idle',4,101),('boneWalk','Skeleton','Walk',4,101),('boneAttack','Skeleton','Attack',8,101),('eyeFlight','Flying eye','Flight',8,75),('eyeAttack','Flying eye','Attack',8,75)]:
    raw=z.read('Monsters_Creatures_Fantasy/'+folder+'/'+file+'.png')
    (art/(sheet+'.png')).write_bytes(raw)
    im=Image.open(BytesIO(raw)).convert('RGBA')
    for i in range(count):
        box=im.crop((150*i,0,150*(i+1),150)).getchannel('A').getbbox()
        x,y,x2,y2=box
        data['frames'].append(dict(sheet=sheet,name=str(i),x=150*i+x,y=y,w=x2-x,h=y2-y,anchorX=150*i+75,baseline=baseline))
data['frames'].append(dict(sheet='summonBagua',name='0',x=0,y=0,w=512,h=512,anchorX=256,baseline=512))
ET.register_namespace('','http://www.w3.org/2000/svg')
svg=ET.Element('{http://www.w3.org/2000/svg}svg',{'width':'512','height':'256','viewBox':'0 0 512 256'})
order=['swift','agility','armor','focus','rain','finger','brand','afterimage']
for i,name in enumerate(order):
    source=ET.fromstring((sources/(name+'.svg')).read_text())
    group=ET.SubElement(svg,'{http://www.w3.org/2000/svg}g',{'transform':f'translate({i%4*128+12} {i//4*128+12}) scale(0.203125)'})
    for child in list(source):
        for el in child.iter():
            if 'fill' in el.attrib and el.attrib['fill'] not in ['none','transparent']:el.set('fill','#eab18b')
            if el.tag.endswith('path'):el.set('fill','#eab18b')
        group.append(child)
    data['frames'].append(dict(sheet='upgradeIconsExtra',name=name,x=i%4*128,y=i//4*128,w=128,h=128,anchorX=i%4*128+64,baseline=i//4*128+128))
(sources/'upgrade-extra-v16.svg').write_bytes(ET.tostring(svg))
manifest.write_text(json.dumps(data,ensure_ascii=False,indent=2))
print('Original frames sliced: bone 16, eye 16, authored Bagua summon seal; eight CC BY SVG glyphs composed.')
