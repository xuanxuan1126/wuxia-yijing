"""Technical atlas slicing/packing. Original licensed pixels are retained unchanged."""
from pathlib import Path
from PIL import Image, ImageSequence
import json, zipfile
from io import BytesIO
root=Path(__file__).resolve().parents[1];art=root/'Assets/Wuxia/Resources/Art';manifest=root/'Assets/Wuxia/Resources/Data/ArtFrames.json';data=json.loads(manifest.read_text())
data['frames']=[f for f in data['frames'] if f['sheet'] not in ('heroRun13','pixelHit','pixelMist','pixelFrost')]
# Frame-specific near sleeve contours, expressed in the untrimmed source image.
base_arm=[(230,108),(259,128),(263,155),(283,186),(309,202),(310,224),(287,238),(245,234),(195,229),(167,213),(198,169),(212,138)]
forward_arm=[(240,113),(266,139),(254,168),(267,186),(312,176),(350,164),(379,173),(381,195),(354,216),(290,233),(235,245),(184,236),(165,216),(191,180),(207,150)]
im=Image.open(art/'heroRun13.png').convert('RGBA');W,H=im.size
for i in range(8):
 row=i//4;col=i%4;cuts=([0,470,920,1350,1774] if row==0 else [0,480,920,1350,1774]);sx,ex=cuts[col],cuts[col+1];sy=0 if row==0 else 450;ey=450 if row==0 else H
 box=im.crop((sx,sy,ex,ey)).getchannel('A').point(lambda x:255 if x>100 else 0).getbbox();x,y,x2,y2=box;x+=sx;y+=sy;x2+=sx;y2+=sy
 forward=i in (3,4,5,6,7);points=forward_arm if forward else base_arm;ox=[0,450,890,1330][col];oy=0 if row==0 else 450
 points=[(px+ox,py+oy) for px,py in points]
 data['frames'].append(dict(sheet='heroRun13',name=str(i),x=x,y=y,w=x2-x,h=y2-y,anchorX=[300,755,1190,1630][col],baseline=440 if row==0 else 880,arm=[v for px,py in points for v in (px-x,py-y)]))
# Idle/jump sleeve pixels extracted by contour, never by a rectangle containing the torso.
for f in data['frames']:
 if f['sheet']=='hero':
  n=int(f['name']);poly=[(.41,.19),(.57,.22),(.53,.32),(.61,.38),(.72,.39),(.72,.45),(.56,.47),(.26,.60),(.14,.67),(.19,.51),(.29,.36)]
  if n in (4,5,9):poly=[(.44,.18),(.59,.26),(.55,.32),(.66,.35),(.69,.42),(.57,.46),(.42,.42),(.22,.51),(.11,.45),(.29,.34)]
  f['arm']=[v for px,py in poly for v in (px*f['w'],py*f['h'])]
# Six-frame, 16-bit pixel variant of Sinestesia's CC0 hit by Kelvin Shadewing.
gif=Image.open(root/'Documentation/Attribution/Unity1.3/Hit-Yellow.gif');w,h=gif.size;atlas=Image.new('RGBA',(w*gif.n_frames,h))
for i,frame in enumerate(ImageSequence.Iterator(gif)):
 atlas.paste(frame.convert('RGBA'),(i*w,0));data['frames'].append(dict(sheet='pixelHit',name=str(i),x=i*w,y=0,w=w,h=h,anchorX=i*w+w/2,baseline=h))
atlas.save(art/'pixelHit.png')
# Source frame canvas is 100x100; sample only complete authored frames and repack.
fx=zipfile.ZipFile(root/'Documentation/Attribution/Unity1.3/Free Pixel Effects Pack.zip')
for sheet,file,count,step in [('pixelFrost','19_freezing_spritesheet.png',36,2),('pixelMist','5_magickahit_spritesheet.png',24,2)]:
 source=Image.open(BytesIO(fx.read(file))).convert('RGBA');cols=source.width//100;atlas=Image.new('RGBA',(800,((count+7)//8)*100))
 for i in range(count):
  k=i*step;sx=k%cols*100;sy=k//cols*100;x=i%8*100;y=i//8*100;atlas.paste(source.crop((sx,sy,sx+100,sy+100)),(x,y));data['frames'].append(dict(sheet=sheet,name=str(i),x=x,y=y,w=100,h=100,anchorX=x+50,baseline=y+100))
 atlas.save(art/(sheet+'.png'))
manifest.write_text(json.dumps(data,ensure_ascii=False,indent=2))
print('Prepared run8 / hit6 / frost36 / sword mist24; sleeve contours')
