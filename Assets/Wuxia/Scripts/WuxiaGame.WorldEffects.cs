using UnityEngine;
namespace Wuxia {
 public partial class WuxiaGame {
  const float NovaSpeed=1800;
  SpriteView novaLeft,novaRight,novaVeil,novaCore;
  bool novaTouched;float novaPreviousReach;
  public float NovaProgress=>boss!=null&&boss.state=="novaTell"?Mathf.Clamp01((clock-boss.entered)/(boss.next-boss.entered)):0;
  public float NovaReach=>boss!=null&&boss.state=="masterNova"?(clock-boss.entered)*NovaSpeed:0;
  void BeginNova(){ChangeBoss("masterNova",Mathf.Max(boss.X,Width-boss.X)/NovaSpeed+.3f);novaPreviousReach=0;novaTouched=false;audio.Play("slam");Shake(9,.32f);fx.Animated("demonSlash",6,boss.X,boss.CenterY,390,430,clock,.4f,Color.white);}
  void UpdateNovaHit(){float reach=NovaReach;float distance=Mathf.Abs(player.X-boss.X),previousDistance=Mathf.Abs(player.previousPosition.x*100-boss.X);bool crossing=previousDistance-novaPreviousReach>=-75&&distance-reach<=75;if(!novaTouched&&crossing){novaTouched=true;if(!Sheltered())Damage(false,boss.X,true);}novaPreviousReach=reach;}
  void RenderWorldEffects(){bool active=boss!=null&&boss.hp>0&&(boss.state=="novaTell"||boss.state=="masterNova");
   if(!active){novaLeft?.Visible(false);novaRight?.Visible(false);novaVeil?.Visible(false);novaCore?.Visible(false);return;}
   if(novaLeft==null){novaLeft=new SpriteView(art,effectRoot,"魔潮 · 左波前",35);novaRight=new SpriteView(art,effectRoot,"魔潮 · 右波前",35);novaVeil=new SpriteView(art,effectRoot,"魔潮 · 蓄压",13);novaCore=new SpriteView(art,effectRoot,"魔潮 · 核心",18);}
   bool tell=boss.state=="novaTell";float age=clock-boss.entered,p=NovaProgress;novaLeft.Visible(!tell);novaRight.Visible(!tell);novaVeil.Visible(true);novaCore.Visible(tell);
   novaVeil.Image("whitePixel","0",boss.X,360,tell?Width:Mathf.Min(Width,NovaReach*2),720);novaVeil.Color(new Color(.22f,.035f,.32f,tell?.025f+p*.08f:.09f*Mathf.Clamp01((boss.next-clock)*3)));
   if(tell){float size=230+p*160;novaCore.Image("formationOuter","0",boss.X,boss.CenterY-35,size,size,-age*.48f);novaCore.Color(new Color(.86f,.48f,1,.35f+p*.55f));if(clock>=novaFxNext){novaFxNext=clock+.15f;float a=clock*3;fx.Sprite("wuxiaAtlas","impact",boss.X+Mathf.Cos(a)*145,boss.CenterY+Mathf.Sin(a)*120,13,13,clock,.32f,new Color(.9f,.67f,1),-Mathf.Cos(a)*210,-Mathf.Sin(a)*170);}}
   else {float reach=NovaReach,fade=Mathf.Clamp01((boss.next-clock)*5);foreach(int dir in new[]{-1,1}){var front=dir<0?novaLeft:novaRight;float x=boss.X+dir*reach;front.Image("demonFront","0",x-dir*66,360,260,880);front.renderer.flipX=dir<0;front.Color(new Color(1,.72f,1,fade));if(clock>=novaFxNext){fx.Animated("demonSlash",6,x-dir*60,450,180,400,clock,.22f,new Color(1,.7f,1,.65f),dir<0?Mathf.PI:0);fx.Burst(x,610,5,new Color(.9f,.68f,1),clock);}fx.Line(new[]{new Vector2(x,120),new Vector2(x+dir*21,320),new Vector2(x+dir*12,480),new Vector2(x,620)},8,new Color(1,.8f,1,.5f*fade),clock,.045f);}if(clock>=novaFxNext)novaFxNext=clock+.1f;}
   foreach(float x in ShelterXs){var points=new Vector2[25];for(int i=0;i<25;i++){float a=Mathf.PI*i/24;points[i]=new Vector2(x-Mathf.Cos(a)*82,620-Mathf.Sin(a)*155);}fx.Line(points,3,new Color(.62f,1,.75f,.82f),clock,.045f);fx.Line(new[]{new Vector2(x-82,620),new Vector2(x+82,620)},4,new Color(.7f,1,.76f,.9f),clock,.045f);}
  }
  void ClearWorldEffects(){novaLeft?.Visible(false);novaRight?.Visible(false);novaVeil?.Visible(false);novaCore?.Visible(false);novaPreviousReach=0;novaTouched=false;}
 }
}
