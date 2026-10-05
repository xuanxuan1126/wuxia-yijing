# coding: utf-8
"""Technical alpha-safe atlas slicing; preserve generated artwork pixels unchanged."""
from pathlib import Path
from PIL import Image
import json,hashlib
root=Path(__file__).resolve().parents[1]
sources=root/'Documentation/Attribution/Unity1.7'
art=root/'Assets/Wuxia/Resources/Art'
manifest=root/'Assets/Wuxia/Resources/Data/ArtFrames.json'
def gutters(alpha,axis):
 values=[];start=None;length=alpha.size[axis]
 for n in range(length):
  box=(n,0,n+1,alpha.height) if axis==0 else (0,n,alpha.width,n+1)
  empty=alpha.crop(box).getextrema()[1]<8
  if empty and start is None:start=n
  if not empty and start is not None:
   if n-start>=3:values.append((start,n))
   start=None
 return values
def cuts(alpha,axis,divisions):
 length=alpha.size[axis];gaps=gutters(alpha,axis);result=[0]
 for index in range(1,divisions):
  goal=length*index/divisions
  choices=[(abs((a+b)/2-goal),(a+b)//2) for a,b in gaps if abs((a+b)/2-goal)<length/divisions*.35]
  if not choices:raise ValueError('No safe gutter '+str((axis,index,alpha.size)))
  result.append(min(choices)[1])
 return result+[length]
d=json.loads(manifest.read_text())
remove={'boneIdle','boneWalk','boneAttack','eyeFlight','eyeAttack','guardMotion','guardAttack','discipleMotion','ravenMotion'}
d['frames']=[f for f in d['frames'] if f['sheet'] not in remove]
entries=[('guardMotion','guard-motion-source.png',4,3),('guardAttack','guard-attack-source.png',2,2),('discipleMotion','disciple-motion-source.png',4,4),('ravenMotion','raven-motion-source.png',4,4)]
provenance=[]
ravenCenters=[(215,230),(525,235),(840,237),(1151,240),(218,538),(531,538),(844,529),(1152,534),(215,837),(531,830),(846,834),(1156,838),(217,1100),(527,1102),(837,1107),(1151,1108)]
for sheet,file,columns,rows in entries:
 im=Image.open(sources/file).convert('RGBA');alpha=im.getchannel('A')
 assert alpha.getextrema()==(0,255)
 rowCuts=cuts(alpha,1,4 if sheet=='guardMotion' else rows)
 frames=[]
 for row in range(rows):
  y0,y1=rowCuts[row:row+2];colCuts=cuts(alpha.crop((0,y0,im.width,y1)),0,columns)
  boxes=[]
  for col in range(columns):
   x0,x1=colCuts[col:col+2];box=alpha.crop((x0,y0,x1,y1)).point(lambda a:255 if a>=8 else 0).getbbox();assert box
   bx,by,bx2,by2=box;assert bx>0 and bx2<x1-x0 and by>0 and by2<y1-y0,(sheet,row,col,box)
   boxes.append((x0+bx,y0+by,x0+bx2,y0+by2,x0,x1))
  baseline=max(box[3] for box in boxes)
  for col,(x,y,x2,y2,cx0,cx1) in enumerate(boxes):
   index=row*columns+col
   anchor=(cx0+cx1)/2
   if sheet=='ravenMotion':anchor,baseline=ravenCenters[index]
   frames.append(dict(sheet=sheet,name=str(index),x=x,y=y,w=x2-x,h=y2-y,anchorX=anchor,baseline=baseline))
 (art/(sheet+'.png')).write_bytes((sources/file).read_bytes());d['frames']+=frames
 provenance.append(dict(sheet=sheet,source=file,frames=len(frames),alpha=list(alpha.getextrema()),sha256=hashlib.sha256((sources/file).read_bytes()).hexdigest(),rows=rowCuts))
for obsolete in ['boneIdle','boneWalk','boneAttack','eyeFlight','eyeAttack']:
 for suffix in ['.png','.png.meta']:
  p=art/(obsolete+suffix)
  if p.exists():p.unlink()
manifest.write_text(json.dumps(d,ensure_ascii=False,indent=2)+'\n')
(sources/'切片校验.json').write_text(json.dumps(provenance,ensure_ascii=False,indent=2)+'\n')
print('Prepared 48 independent enemy frames; alpha-safe gutters and unchanged pixels.')
