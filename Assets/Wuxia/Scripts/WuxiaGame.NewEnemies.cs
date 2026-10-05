using UnityEngine;
namespace Wuxia {
 public partial class WuxiaGame {
  RaycastHit2D DiveObstruction(Actor e){Vector2 start=PixelWorld.Position(e.X,e.CenterY),target=PixelWorld.Position(player.X,player.CenterY),delta=target-start;return Physics2D.BoxCast(start,new Vector2(e.width/100,e.height/100),0,delta.normalized,delta.magnitude,1<<9);}
  bool UpdateNewEnemy(Actor e,float dt){
   if(e.kind!="bone"&&e.kind!="cultist"&&e.kind!="eye")return false;
   if(clock<e.spawnGraceUntil){e.VX=e.VY=0;return true;}if(clock<e.hitAt+.18f)return true;
   float dx=player.X-e.X,dy=player.CenterY-e.CenterY;e.dir=e.kind=="bone"&&e.windup>0?e.chargeDir:dx>=0?1:-1;
   if(e.kind=="bone"){
    if(e.windup>0){e.VX=0;float age=clock-e.windup;
     if(age>=.36f&&e.move==0){e.move=1;fx.Animated("jadeSlash",6,e.X+e.dir*39,e.Feet-48,95,76,clock,.2f,new Color(1,.86f,.61f),e.dir>0?0:Mathf.PI);var area=new Rect(e.X+e.dir*42-44,e.Feet-82,88,78);if(area.Overlaps(player.Bounds))Damage(false,e.X,custom:e.attackPower);}
     if(age>=.72f){e.windup=0;e.chargeUntil=clock+.95f;}
    }else if(clock<e.chargeUntil)e.VX=0;
    else if(Mathf.Abs(dx)<100&&Mathf.Abs(dy)<85&&e.Grounded){e.windup=clock;e.move=0;e.chargeDir=e.dir;}
    else e.VX=e.dir*62;
    if(e.windup>0)e.dir=e.chargeDir;
   }else if(e.kind=="cultist"){
    if(e.windup>0){e.VX=0;float age=clock-e.windup;
     if(age<.65f){e.aimX=player.X;e.aimY=player.CenterY;}
     if(age>=.95f){float a=Mathf.Atan2(e.aimY-(e.CenterY-10),e.aimX-e.X);SpawnProjectile(e.X,e.CenterY-10,a,true,true,e.attackPower);e.windup=0;e.chargeUntil=clock+2.7f;fx.Animated("pixelMist",24,e.X,e.CenterY-10,70,74,clock,.3f,new Color(.86f,.75f,1));}
    }else if(clock>=e.chargeUntil&&Mathf.Abs(dx)<660&&CanSee(e.X,e.CenterY,player.X,player.CenterY)){e.windup=clock;e.aimX=player.X;e.aimY=player.CenterY;}
    else e.VX=Mathf.Abs(dx)>420?e.dir*38:Mathf.Abs(dx)<210?-e.dir*48:0;
   }else{
    if(e.windup>0){e.VX=e.VY=0;fx.Line(new[]{new Vector2(e.X,e.CenterY),new Vector2(e.aimX,e.aimY)},1,new Color(1,.68f,.43f,.45f),clock,.06f);if(clock-e.windup>=.65f){e.windup=0;float distance=Vector2.Distance(new Vector2(e.X,e.CenterY),new Vector2(e.aimX,e.aimY));e.chargeUntil=clock+Mathf.Clamp(distance/300+.14f,.65f,1.8f);e.state="dive";float a=Mathf.Atan2(e.aimY-e.CenterY,e.aimX-e.X);e.VX=Mathf.Cos(a)*300;e.VY=Mathf.Sin(a)*300;}}
    else if(e.state=="dive"){if(clock>=e.chargeUntil){e.state="recover";e.chargeUntil=clock+1.5f;e.VX=e.VY=0;}}
    else {var obstruction=DiveObstruction(e);e.VX=Mathf.Clamp(dx*.3f,-55,55);if(obstruction.collider){var bounds=obstruction.collider.bounds;float left=bounds.min.x*100-e.width*.5f-95,right=bounds.max.x*100+e.width*.5f+95,goal=Mathf.Abs(left-player.X)<Mathf.Abs(right-player.X)?left:right;e.VX=Mathf.Clamp((goal-e.X)*1.5f,-95,95);}float hover=Mathf.Clamp(player.CenterY-170,320,460)+Mathf.Sin(clock*3+e.min)*13;e.VY=Mathf.Clamp((hover-e.CenterY)*2,-145,145);
     if(clock>=e.chargeUntil&&Mathf.Abs(dx)<470&&!obstruction.collider){e.windup=clock;e.aimX=player.X;e.aimY=player.CenterY;e.state="tell";}
    }
   }
   if((e.X<70&&e.VX<0)||(e.X>Width-70&&e.VX>0))e.VX=0;
   if(clock<e.frozenUntil){e.VX*=.45f;if(e.IsFlying)e.VY*=.65f;}return true;
  }
  bool RenderNewEnemy(Actor e){
   if(e.kind=="bone"){
    int index=e.windup>0?Mathf.Min(3,Mathf.FloorToInt((clock-e.windup)/.18f)):Mathf.Abs(e.VX)>5?4+Mathf.FloorToInt((clock+e.min*.0001f)/.15f)%8:Mathf.FloorToInt(clock/.24f)%4;
    string sheet=e.windup>0?"guardAttack":"guardMotion";e.view.PoseActor(sheet,index,e,84f/art.Frame(sheet,e.windup>0?"3":"0").h,e.dir);
   }else if(e.kind=="cultist"){
    int index=e.windup>0?12+Mathf.Min(3,Mathf.FloorToInt((clock-e.windup)/.24f)):Mathf.Abs(e.VX)>5?4+Mathf.FloorToInt((clock+e.min*.0001f)/.18f)%8:Mathf.FloorToInt(clock/.25f)%4;
    e.view.PoseActor("discipleMotion",index,e,84f/art.Frame("discipleMotion","0").h,e.dir);
   }else if(e.kind=="eye"){
    int index=e.state=="dive"?12+Mathf.FloorToInt(clock/.13f)%4:e.windup>0?Mathf.FloorToInt(clock/.2f)%4:4+Mathf.FloorToInt(clock/.14f)%8;
    e.view.PoseActor("ravenMotion",index,e,78f/art.Frame("ravenMotion","0").w,e.dir,e.height*.5f);
   }else return false;
   float alpha=Mathf.Clamp01(1-(e.spawnGraceUntil-clock)/.45f);e.view.Color(new Color(1,clock-e.hitAt<.14f?.85f:1,clock-e.hitAt<.14f?.7f:1,Mathf.Lerp(.25f,1,alpha)));return true;
  }
 }
}
