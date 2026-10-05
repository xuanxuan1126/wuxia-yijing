using System.Collections.Generic;
using UnityEngine;
namespace Wuxia {
 public partial class WuxiaGame {
  class SwordBrand {public SpriteView seal;public float x,y,born;public int level;public bool burst;}
  readonly List<SwordBrand> brands=new List<SwordBrand>(4);bool dashEchoPending;
  public int SwordBrandCount=>brands.Count;
  void UpdateNewRogueSkills(){
   int n=run.Level("rain");if(n>0&&clock>=run.nextRain){var target=Nearest(player.X,player.CenterY,900,true);if(target==null)run.nextRain=clock+.25f;else{
    run.nextRain=clock+7.5f*Cooldown;int count=4+n;
    for(int i=0;i<count&&projectiles.Count<48;i++){float x=target.X+(i-(count-1)*.5f)*36,y=Mathf.Max(145,target.CenterY-260-Mathf.Abs(i-(count-1)*.5f)*12);float angle=Mathf.Atan2(target.CenterY-y,target.X-x);
     var v=new SpriteView(art,effectRoot,"惊鸿剑雨",30);v.Additive();projectiles.Add(new Projectile{view=v,x=x,y=y,angle=angle,born=clock,speed=570+i*20,damage=4+n*2,rain=true});
    }fx.Sprite("formationInner","0",target.X,Mathf.Max(135,target.CenterY-270),135,34,clock,.5f,new Color(.68f,1,.86f,.6f),additive:true,grow:1.2f);
   }}
   n=run.Level("finger");if(n>0&&clock>=run.nextFinger){var target=Nearest(player.X,player.CenterY,1000,true);if(target==null)run.nextFinger=clock+.25f;else{
    run.nextFinger=clock+8*Cooldown;float angle=Mathf.Atan2(target.CenterY-player.CenterY,target.X-player.X);
    for(int i=0;i<2&&projectiles.Count<48;i++)projectiles.Add(new Projectile{view=new SpriteView(art,effectRoot,"太虚剑指",28),x=player.X,y=player.CenterY-9+i*18,angle=angle,born=clock,speed=820,range=850+n*80,damage=5+n*2,qi=true});
    fx.Animated("jadeSlash",6,player.X+Mathf.Cos(angle)*30,player.CenterY,94,78,clock,.25f,Color.white,angle);
   }}
   n=run.Level("brand");if(n>0&&clock>=run.nextBrand){var target=Nearest(player.X,player.CenterY,850,true);if(target==null)run.nextBrand=clock+.25f;else{
    run.nextBrand=clock+6.5f*Cooldown;if(brands.Count<4){var seal=new SpriteView(art,effectRoot,"焚邪剑印",18);seal.Additive();brands.Add(new SwordBrand{seal=seal,x=target.X,y=target.Feet-4,born=clock,level=n});}
   }}
   for(int i=brands.Count-1;i>=0;i--){var b=brands[i];float age=clock-b.born,radius=105+b.level*12;b.seal.Image("formationInner","0",b.x,b.y,radius*1.7f,42,age*.8f);b.seal.Color(new Color(1,.71f,.37f,Mathf.Clamp01(age/.3f)*Mathf.Clamp01((1.25f-age)/.3f)));
    if(age>=.8f&&!b.burst){b.burst=true;fx.Animated("pixelMist",24,b.x,b.y-48,radius*1.7f,160,clock,.42f,new Color(1,.81f,.59f));fx.Sprite("swordImpact","0",b.x,b.y-55,radius*1.8f,radius*1.8f,clock,.25f,new Color(1,.82f,.52f,.68f),additive:true,grow:1.25f);Shake(3,.13f);
     foreach(var target in Targets())if(Mathf.Abs(target.X-b.x)<radius&&Mathf.Abs(target.CenterY-(b.y-42))<115)Strike(target,target.X>b.x?1:-1,true,7+b.level*3);
    }if(age>=1.25f){b.seal.Destroy();brands.RemoveAt(i);}
   }
   if(dashEchoPending&&clock>=dashUntil){dashEchoPending=false;n=run.Level("afterimage");if(n>0){
    fx.Animated("jadeSlash",6,player.X,player.CenterY-6,190,155,clock,.32f,Color.white,facing>0?0:Mathf.PI);
    foreach(var target in Targets())if(Mathf.Abs(target.X-player.X)<125+n*8&&Mathf.Abs(target.CenterY-player.CenterY)<110){Strike(target,facing,true,4+n*2);SwordImpact(target.X,target.CenterY,0,false);}Shake(2,.1f);
   }}
  }
  void ClearNewRogueSkills(){foreach(var b in brands)b.seal.Destroy();brands.Clear();dashEchoPending=false;}
 }
}
