# coding: utf-8
from pathlib import Path
from PIL import Image,ImageDraw,ImageFont
import json,math,random,wave,struct,shutil
root=Path('Assets/Wuxia/Resources');art=root/'Art';frames=[]
def saveframe(sheet,name,rect,trim=True,anchor=None,baseline=None):
 im=Image.open(art/(sheet+'.png')).convert('RGBA');x,y,w,h=map(round,rect)
 if trim:
  alpha=im.crop((x,y,x+w,y+h)).getchannel('A').point(lambda p:255 if p>100 else 0);bbox=alpha.getbbox()
  if bbox:
   bx,by,ex,ey=bbox;x+=bx;y+=by;w=ex-bx;h=ey-by
 frames.append(dict(sheet=sheet,name=str(name),x=x,y=y,w=w,h=h,anchorX=(x+w/2 if anchor is None else anchor),baseline=(y+h if baseline is None else baseline)))
sheets={'hero':(4,3),'heroAttack':(4,2),'heroRun':(4,2),'bandit':(4,2),'banditRun':(4,2),'bossAtlas':(4,2),'masterAtlas':(4,2),'craneMotion':(4,1),'storyPast':(2,1),'storyArt':(2,2),'roomAtlas':(2,2),'heroMotion':(4,4),'heroDraw':(4,2)}
for key,(cols,rows) in sheets.items():
 im=Image.open(art/(key+'.png'));W,H=im.size
 for i in range(cols*rows):
  sx=round(i%cols*W/cols);ex=round((i%cols+1)*W/cols);sy=round(i//cols*H/rows);ey=round((i//cols+1)*H/rows);anchor=None;base=None
  cuts=None
  if key=='bossAtlas' and i>=4:cuts=[0,412,778,1141,1536];sx=round(cuts[i%4]*W/1536);ex=round(cuts[i%4+1]*W/1536)
  if key=='heroAttack':
   cuts=[0,470,900,1340,1774] if i<4 else [0,495,950,1340,1774]
   sx=round(cuts[i%4]*W/1774);ex=round(cuts[i%4+1]*W/1774);anchor=[220,685,1135,1550,225,685,1105,1560][i]*W/1774
  if key=='bandit' and i>=4:cuts=[0,443,945,1331,1774];sx=round(cuts[i%4]*W/1774);ex=round(cuts[i%4+1]*W/1774)
  if key=='bandit':anchor=[220,685,1155,1550,220,650,1120,1550][i]*W/1774
  if key in ['heroRun','banditRun']:
   cuts=[0,465,895,1335,1774] if key=='heroRun' else [0,465,930,1350,1774];sx=round(cuts[i%4]*W/1774);ex=round(cuts[i%4+1]*W/1774)
   anchor=([300,740,1180,1620]*2 if key=='heroRun' else [245,700,1150,1585]*2)[i]*W/1774;base=(434 if i<4 else 856)*H/887
  if key=='masterAtlas':
   sy=0 if i<4 else 480;ey=480 if i<4 else H
   if i>=4:sx,ex=[(0,447),(447,890),(935,1334),(1340,1774)][i-4]
   anchor=[250,720,1160,1590,220,720,1150,1600][i];base=480 if i<4 else 870
  if key=='storyPast':sx,ex=(150,620) if i==0 else (655,1536)
  if key=='craneMotion':sx+=10;ex-=10;anchor=i*W/4+350;base=440
  if key=='heroMotion':
   row=i//4;ys=[0,345,651,881,1086];cuts=[0,420,755,1080,1448] if row==3 else [0,362,724,1086,1448];sx=round(cuts[i%4]*W/1448);ex=round(cuts[i%4+1]*W/1448);sy=round(ys[row]*H/1086);ey=round(ys[row+1]*H/1086)
  saveframe(key,i,(sx,sy,ex-sx,ey-sy),key not in ['storyArt','roomAtlas'],anchor,base)
names=['stoneTop','stoneFill','wood','bamboo','gate','altar','talisman','herb','slash','qi','hostileQi','impact','golem','crane','spikes','sword']
rects=[[0,130,355,212],[360,88,265,250],[627,195,358,143],[978,0,276,350],[0,346,355,305],[360,347,295,305],[675,365,255,285],[960,360,294,290],[0,650,319,257],[319,705,333,180],[652,716,334,158],[987,650,267,276],[0,907,357,347],[360,968,292,286],[654,930,326,324],[941,1025,313,229]]
W,H=Image.open(art/'wuxiaAtlas.png').size
for name,r in zip(names,rects):saveframe('wuxiaAtlas',name,[v*W/1254 for v in r])
upgrades=json.loads((root/'Data/GameData.json').read_text())['upgrades']
for i,u in enumerate(upgrades):saveframe('upgradeIcons',u['id'],(i%4*128,i//4*128,128,128),False)
W,H=Image.open(art/'qiSlash.png').size
for y in range(H//47):
 for x in range(W//64):saveframe('qiSlash',y*(W//64)+x,(x*64,y*47,64,47),False)
for file in art.glob('*.png'):
 if file.stem not in sheets and file.stem not in ['wuxiaAtlas','upgradeIcons','qiSlash']:saveframe(file.stem,0,(0,0,*Image.open(file).size),file.stem in ['blade','enemyBlade'])
(root/'Data/ArtFrames.json').write_text(json.dumps({'frames':frames},ensure_ascii=False,indent=2))
# Freeze the HTML title into a portable raster using the bundled, redistributable Chinese font.
font=ImageFont.truetype(str(root/'Fonts/NotoSerifCJKsc-Regular.otf'),54);title=Image.new('RGBA',(384,112));draw=ImageDraw.Draw(title)
for dx,dy,col in [(2,4,'#091c20'),(-2,0,'#091c20'),(2,0,'#091c20'),(0,-2,'#091c20'),(0,3,'#795c36'),(0,0,'#e4cd96')]:
 for char,x in zip('武侠遗境',[76,137,247,308]):draw.text((x+dx,31+dy),char,font=font,anchor='mt',fill=col,stroke_width=0)
draw.polygon([(187,8),(197,8),(197,33),(211,37),(211,44),(199,44),(199,94),(192,110),(185,94),(185,44),(173,44),(173,37),(187,33)],fill='#10292d')
draw.rectangle((188,7,195,34),fill='#d1ae68')
for y in range(16,34,5):draw.rectangle((189,y,194,y+1),fill='#344b48')
draw.polygon([(174,38),(186,36),(198,36),(210,38),(207,42),(196,40),(188,40),(177,42)],fill='#e1c387');draw.polygon([(188,43),(196,43),(196,91),(192,104),(188,91)],fill='#b4cec7');draw.polygon([(188,43),(192,43),(192,104),(188,91)],fill='#e9edda')
title.save(art/'menuTitle.png')
# Portable static skull crest, translated from the existing pixel palette.
rows=['         111111','      112233332211','    112344554433221','   12345666665544321','  1234567777665544321',' 12345677777765544321',' 12345667777665443321','123445666666554433221','123344555555443322321','123334444444433222321','123233333333322212321','123211111331111112321','123110001331000011321','123100001331000001321','123100001441000001321','123210012552100012321','123321123663211233321',' 12344334565433444321','  123454510154554321','   123451001544321','   123451001543321','    2345522554321','    2356565654321','    2357575754321','    2357575754321','    2356565654321','    2343434344321','    123444444321','     1234554321','      12233221','        1111']
colors=['#110c09','#28170f','#533320','#805334','#a77d50','#d0ab77','#ead1a0','#ffedc6'];skull=Image.new('RGBA',(64,72));d=ImageDraw.Draw(skull)
for y,row in enumerate(rows):
 for x,c in enumerate(row):
  if c!=' ':d.rectangle((5+x*2,5+y*2,6+x*2,6+y*2),fill=colors[int(c)])
skull.save(art/'masterSkull.png')
# Original synthesized strike samples, porting the existing soft envelopes to native AudioClips.
random.seed(32);sr=22050
for kind,length in [('slash',.19),('hit',.125),('heavy',.19),('step',.035),('stoneBreak',.32),('dash',.25),('jump',.15),('heal',.6),('slam',.5)]:
 samples=[];low=0
 for i in range(int(length*sr)):
  t=i/sr;p=i/(length*sr);w=random.uniform(-1,1);low=.72*low+.28*w
  if kind=='slash':v=(w*.25+low*.75)*max(0,math.sin(math.pi*p**.72))**1.7*.45
  elif kind in ['hit','heavy']:v=(w*.18*math.exp(-t/.014)+low*1.5*math.exp(-t/.032)+math.sin(2*math.pi*172*t)*.16*math.exp(-t/.025))*min(1,t/.003)*(.8 if kind=='heavy' else .6)
  elif kind in ['slam','stoneBreak']:v=(low*.6+math.sin(2*math.pi*58*t)*.3)*(1-p)**2
  elif kind=='heal':v=sum(math.sin(2*math.pi*f*t) for f in [392,494,587])*.08*math.sin(math.pi*p)**2
  elif kind=='jump':v=math.sin(2*math.pi*(170+100*p)*t)*.13*math.sin(math.pi*p)
  else:v=low*.25*(1-p)**2
  samples.append(struct.pack('<h',round(max(-1,min(1,v))*32767)))
 with wave.open(str(root/'Audio'/(kind+'.wav')),'wb') as w:w.setnchannels(1);w.setsampwidth(2);w.setframerate(sr);w.writeframes(b''.join(samples))
print('Imported',len(frames),'sprite frames')
