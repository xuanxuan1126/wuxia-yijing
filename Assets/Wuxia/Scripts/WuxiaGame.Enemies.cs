using UnityEngine;
namespace Wuxia {
 public partial class WuxiaGame {
  float masterWaveNext,novaFxNext;public string BossWarning=>boss==null||!boss.active||boss.hp<=0?"":boss.state=="novaTell"?"魔潮将至 · 进入护心草庇所":boss.state=="masterSlamTell"||boss.state=="slamTell"?"震地蓄势 · 跃起避波":boss.state=="sealTell"?"追魂魔符 · 御剑闪避":boss.state=="chargeTell"?"石像蓄势 · 越顶或引向高台":"";
  bool CanSee(float x,float y,float tx,float ty){foreach(var p in platforms){if(p.broken||p.data.h<=28)continue;for(int i=1;i<=12;i++){float t=i/12f;if(new Rect(p.data.x,p.data.y,p.data.w,p.data.h).Contains(new Vector2(Mathf.Lerp(x,tx,t),Mathf.Lerp(y,ty,t))))return false;}}return true;}
  void UpdateEnemies(float dt){foreach(var e in enemies){if(e.hp<=0)continue;if(UpdateNewEnemy(e,dt))continue;float dx=player.X-e.X,dy=player.CenterY-e.CenterY;if(e.kind=="moth"){
    bool visible=CanSee(e.X,e.CenterY,player.X,player.CenterY);if(e.windup>0){e.VX=e.VY=0;float age=clock-e.windup;if(!visible||Mathf.Abs(dx)>600||Mathf.Abs(dy)>400){e.windup=0;e.chargeUntil=clock+.3f;}else {if(age<.45f){float follow=1-Mathf.Exp(-dt*8);e.aimX=Mathf.Lerp(e.aimX,player.X,follow);e.aimY=Mathf.Lerp(e.aimY,player.CenterY-8,follow);e.dir=dx>0?1:-1;}bool locked=age>=.45f;fx.Line(new[]{new Vector2(e.X,e.CenterY+6),new Vector2(e.aimX,e.aimY)},locked?2:1,new Color(1,.53f,.32f,locked?.7f:.35f),clock,.06f);fx.Ring(e.aimX,e.aimY,locked?24:34,locked?24:34,new Color(1,.6f,.3f,.6f),clock,.06f);if(age>=.8f){float angle=Mathf.Atan2(e.aimY-e.CenterY-6,e.aimX-e.X);SpawnProjectile(e.X,e.CenterY+6,angle-.13f,true,damage:e.attackPower);SpawnProjectile(e.X,e.CenterY+6,angle+.13f,true,damage:e.attackPower);fx.Burst(e.X,e.CenterY,8,new Color(1,.55f,.3f),clock);audio.Play("slash");e.windup=0;e.chargeUntil=clock+.43f;e.VX=-e.dir*90;e.VY=-25;}}}
    else if(clock<e.chargeUntil){e.VX*=Mathf.Max(0,1-dt*4);e.VY*=Mathf.Max(0,1-dt*4);}else if(visible&&Mathf.Abs(dx)<520&&Mathf.Abs(dy)<360&&clock>e.chargeUntil+2.1f){e.windup=clock;e.aimX=player.X;e.aimY=player.CenterY-8;e.dir=dx>0?1:-1;}else {if(e.X<e.min)e.dir=1;if(e.X>e.max)e.dir=-1;e.VX=e.dir*35;e.VY=(e.baseY+Mathf.Sin(clock*2.8f+e.min)*24-e.CenterY)*1.8f;}
   }else if(clock>e.hitAt+.18f){if(endless&&e.windup<=0&&clock>=e.chargeUntil&&Mathf.Abs(dx)>26)e.dir=dx>=0?1:-1;if(e.windup<=0&&(e.landed||clock>=e.chargeUntil)){if(e.X<e.min)e.dir=1;if(e.X>e.max)e.dir=-1;}
    if(e.windup>0){e.VX=0;fx.Line(new[]{new Vector2(e.X,e.Feet),new Vector2(e.aimX,e.Feet)},2,new Color(1,.8f,.5f,.65f),clock,.06f);if(clock-e.windup>=.62f){e.windup=0;e.chargeUntil=clock+1;e.landed=false;e.VX=e.dir*270;e.VY=-340;audio.Play("dash");}}
    else if(clock<e.chargeUntil&&!e.landed){if(e.Grounded&&e.VY>=0){e.landed=true;e.chargeUntil=clock+.4f;e.VX=0;fx.Burst(e.X,e.Feet,9,new Color(.8f,.73f,.5f),clock);audio.Play("step");}else e.VX=e.X<e.min||e.X>e.max?0:e.dir*270;}
    else if(clock<e.chargeUntil)e.VX=0;
    else if(Mathf.Abs(dx)<210&&Mathf.Abs(dy)<85&&clock>e.chargeUntil+1.35f&&e.Grounded){e.windup=clock;e.dir=dx>0?1:-1;e.aimX=Mathf.Clamp(e.X+e.dir*145,e.min,e.max);}
    else e.VX=e.dir*47;
   }if(clock<e.frozenUntil)e.VX*=.45f;
  }}
  public void ChangeBoss(string state,float seconds){boss.state=state;boss.entered=clock;boss.next=clock+seconds;if(boss.kind=="golem"&&(state=="chargeTell"||state=="charge")){boss.chargeDir=boss.dir>=0?1:-1;boss.VX=0;boss.view.SnapNext();}}
  void UpdateBoss(float dt){if(boss==null||boss.hp<=0)return;if(!boss.active&&player.X>(boss.kind=="master"?260:650)){boss.active=true;ChangeBoss("recover",1.8f);audio.Boss(true);Notice(boss.kind=="master"?"魔灵老祖 · 玄衡":"镇山傀儡",2);}
   if(!boss.active){boss.VX=0;return;}if(boss.kind=="master"){UpdateMaster(dt);return;}
   if(boss.state=="recover"&&clock>boss.next){boss.dir=player.X>=boss.X?1:-1;boss.move++;bool slam=boss.move%2==1;ChangeBoss(slam?"slamTell":"chargeTell",slam?1:1.1f);boss.targetX=Mathf.Clamp(boss.X+boss.dir*Mathf.Max(430,Mathf.Min(1050,Mathf.Abs(player.X-boss.X)+170)),endless?465:290,endless?1535:1610);}
   else if(boss.state=="slamTell"&&clock>boss.next){ChangeBoss("slam",.35f);audio.Play("slam");Shake(6,.24f);fx.Burst(boss.X,604,32,new Color(.88f,.75f,.5f),clock);SpawnWave(boss.X-70,-1);SpawnWave(boss.X+70,1);for(int i=-3;i<=3;i++)fx.Line(new[]{new Vector2(boss.X,619),new Vector2(boss.X+i*32,627),new Vector2(boss.X+i*62,620),new Vector2(boss.X+i*76,624)},2,new Color(.92f,.78f,.54f),clock,.9f);}
   else if(boss.state=="slam"&&clock>boss.next)ChangeBoss("recover",2.2f);
   else if(boss.state=="chargeTell"&&clock>boss.next){ChangeBoss("charge",Mathf.Max(1.6f,Mathf.Abs(boss.targetX-boss.X)/360+.95f));audio.Play("dash");}
   else if(boss.state=="charge"&&(clock>boss.next||boss.dir*(boss.targetX-boss.X)<8))ChangeBoss("brake",.3f);
   else if(boss.state=="wallStun"&&clock>boss.next)ChangeBoss("recover",1.2f);
   else if(boss.state=="brake"&&clock>boss.next)ChangeBoss("recover",2.1f);
   if(boss.state=="charge"||boss.state=="chargeTell")boss.dir=boss.chargeDir;
   float remaining=Mathf.Abs(boss.targetX-boss.X),desired=boss.state=="charge"?boss.chargeDir*Mathf.Min(360,Mathf.Sqrt(2*750*remaining)):0;boss.VX=Mathf.MoveTowards(boss.VX,desired,(boss.state=="charge"?1000:1200)*dt);
   if(boss.state=="charge")foreach(var p in platforms){if(p.broken||p.data.y==620)continue;float front=boss.X+boss.dir*boss.width*.5f,edge=boss.dir>0?p.data.x:p.data.x+p.data.w;float gap=boss.dir*(edge-front);if(p.data.y<boss.Feet&&p.data.y+p.data.h>boss.Feet-boss.height&&gap>=-14&&gap<=Mathf.Abs(boss.VX)*dt+4){BreakPlatform(p,boss.dir);break;}}
   float age=clock-boss.entered;if(boss.state=="slamTell")fx.Ring(boss.X,619,150+age*90,24,new Color(1,.8f,.5f),clock,.06f,3);if(boss.state=="chargeTell")fx.Line(new[]{new Vector2(boss.X,615),new Vector2(boss.targetX,615)},3,new Color(1,.6f,.3f,.6f),clock,.06f);if(boss.state=="wallStun")fx.Ring(boss.X,boss.Feet-190,46,10,new Color(1,.88f,.55f),clock,.06f,2);if(boss.state=="charge")fx.Burst(boss.X-boss.dir*65,boss.Feet,2,new Color(.78f,.67f,.5f),clock);
  }
  public void BreakPlatform(PlatformView p,int dir){if(p==null||p.broken)return;p.broken=true;p.collider.enabled=false;p.Visible(false);for(int i=0;i<18;i++)fx.Sprite("wuxiaAtlas","stoneTop",p.data.x+(i+.5f)*p.data.w/18,p.data.y+14,12+i%4*4,12+i%3*4,clock,.7f,new Color(.85f,.75f,.6f),dir*(110+i*8),-130-i%5*42,true);fx.Burst(p.data.x+p.data.w/2,p.data.y,30,new Color(.9f,.8f,.55f),clock);audio.Play("stoneBreak");Shake(8,.28f);if(boss!=null&&boss.kind=="golem"){boss.VX=0;boss.targetX=boss.X;ChangeBoss("wallStun",1.8f);} }
  void UpdateMaster(float dt){if(Mathf.Abs(player.X-boss.X)>26)boss.dir=player.X>=boss.X?1:-1;boss.VX=0;if(boss.state=="recover"&&clock>=boss.next){boss.move++;int skill=Rules.MasterSkill(boss.lastSkill,random);boss.lastSkill=skill;ChangeBoss(skill==0?"sealTell":skill==1?"masterSlamTell":"novaTell",skill==2?3:skill==1?1.2f:1);}
   else if(boss.state=="sealTell"&&clock>=boss.next){for(int i=0;i<3;i++){float x=boss.X-boss.dir*70,y=440+i*42;SpawnProjectile(x,y,Mathf.Atan2(player.CenterY-14-y,player.X-x)+(i-1)*.24f,true,true);}audio.Play("slash");ChangeBoss("masterCast",.4f);}
   else if(boss.state=="masterCast"&&clock>=boss.next)ChangeBoss("recover",1.8f);
   else if(boss.state=="masterSlamTell"&&clock>=boss.next){ChangeBoss("masterQuake",1.1f);masterWaveNext=clock;audio.Play("slam");Shake(7,.22f);}
   else if(boss.state=="masterQuake"){if(clock>=masterWaveNext){SpawnWave(boss.X-40,-1,true);SpawnWave(boss.X+40,1,true);masterWaveNext+=.4f;fx.Ring(boss.X,620,210,32,new Color(.85f,.6f,1),clock,.4f,4);}if(clock>=boss.next)ChangeBoss("recover",2.2f);}
   else if(boss.state=="novaTell"&&clock>=boss.next){BeginNova();}
   else if(boss.state=="masterNova"){if(clock>=boss.next)ChangeBoss("recover",2.2f);}
   float age=clock-boss.entered;if(boss.state=="sealTell")for(int i=0;i<3;i++)fx.Sprite("wuxiaAtlas","talisman",boss.X-boss.dir*70+Mathf.Sin(clock*3+i)*8,440+i*42,20,40,clock,.08f,new Color(.84f,.58f,1));if(boss.state=="masterSlamTell")fx.Ring(boss.X,619,150+age*90,24,new Color(.85f,.64f,.95f),clock,.06f,3);
   RenderWorldEffects();
  }
 }
}
