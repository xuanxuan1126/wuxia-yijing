using System.Collections.Generic;
using UnityEngine;
namespace Wuxia {
 public partial class WuxiaGame {
  class ReinforcementPortal {
   public SpriteView view,seal,veil;public SpriteView[] charms=new SpriteView[3];public float x,born,spawnAt;public int count,released;public float nextRelease;public bool spawned=>released>=count;
  }
  readonly List<ReinforcementPortal> portals=new List<ReinforcementPortal>(2);
  public const int MaxBatchEnemies=18;
  int batchCount,batchIndex,plannedEnemies,spawnedEnemies;float nextBatchAt,batchClearAt=-1;
  public int ReleasedEnemies=>spawnedEnemies;
  public int PendingEnemies=>Mathf.Max(0,plannedEnemies-spawnedEnemies);
  public int ReinforcementBatch=>batchIndex;
  public int ReinforcementBatches=>batchCount;
  public int PortalCount=>portals.Count;
  public bool WuxiaPortalsActive=>portals.Count>0&&portals.TrueForAll(p=>p.veil!=null&&p.charms.Length==3&&p.view.renderer.sprite&&p.view.renderer.sprite.name=="summonBagua:0");
  public bool ReinforcementsComplete=>PendingEnemies==0;
  public string ReinforcementLabel=>batchCount>0&&plannedEnemies>0&&run!=null&&run.wave>=4?$" · 魔门 {batchIndex}/{batchCount}":"";
  int LivingEnemies(){int n=0;foreach(var enemy in enemies)if(enemy.hp>0)n++;return n;}
  void PrepareReinforcements(WaveSpec spec){
   batchCount=spec.batches;batchIndex=spawnedEnemies=0;plannedEnemies=spec.count;batchClearAt=-1;
   if(spec.count==0)return;
   // Keep the first three waves approachable; later waves use visible entrance tells.
   if(run.wave<4){batchIndex=1;for(int i=0;i<spec.count;i++)SpawnReinforcement(570+i*255,i%3==2?"moth":"crawler");}
   else nextBatchAt=clock+(spec.boss!=null?8:0);
  }
  void QueueReinforcementBatch(){
   int left=batchCount-batchIndex,count=Mathf.CeilToInt(PendingEnemies/(float)left);batchIndex++;
   float[] candidates={240,760,1240,1760};int first=-1,second=-1;float best=-1;
   for(int i=0;i<candidates.Length;i++){float distance=Mathf.Abs(candidates[i]-player.X);if(distance>best){best=distance;first=i;}}
   best=-1;for(int i=0;i<candidates.Length;i++){if(i==first||Mathf.Abs(candidates[i]-player.X)<230)continue;float score=Mathf.Abs(candidates[i]-candidates[first]);if(score>best){best=score;second=i;}}
   if(second<0)second=(first+2)%candidates.Length;
   int a=(count+1)/2,b=count-a;CreatePortal(candidates[first],a);if(b>0)CreatePortal(candidates[second],b);
   batchClearAt=-1;
  }
  void CreatePortal(float x,int count){
   var p=new ReinforcementPortal{x=x,count=count,born=clock,spawnAt=clock+1.3f,nextRelease=clock+1.3f,view=new SpriteView(art,effectRoot,"八卦镇魂 · 虚空符阵",15),seal=new SpriteView(art,effectRoot,"魔门 · 地面阴阳阵",13),veil=new SpriteView(art,effectRoot,"魔道墨烟",14)};
   for(int i=0;i<3;i++)p.charms[i]=new SpriteView(art,effectRoot,"朱砂召灵符 · "+i,16);portals.Add(p);p.seal.Additive();
  }
  void SpawnReinforcement(float x,string requested=null){
   var roster=Rules.EnemyRoster(run.wave);string kind=requested??roster[(spawnedEnemies+run.wave)%roster.Length];
   // Later batches have a guided mix, with a little variation in the remaining slots.
   if(requested==null&&spawnedEnemies%5==4)kind=roster[random.Next(roster.Length)];
   bool flying=kind=="moth"||kind=="eye";
   SpawnEnemy(new EnemyData{kind=kind,x=Mathf.Clamp(x,90,1910),y=flying?410+spawnedEnemies%2*35:601.5f,min=70,max=1930});
   var e=enemies[enemies.Count-1];var spec=Rules.Wave(run.wave);float baseHp=kind=="bone"?6:kind=="cultist"?5:flying?3:4;
   e.hp=e.maxHp=Mathf.Ceil(baseHp*spec.hpScale*(1+.04f*Mathf.Max(0,batchIndex-1)));e.attackPower=spec.damage*(1+.03f*Mathf.Max(0,batchIndex-1));e.spawnGraceUntil=clock+.45f;spawnedEnemies++;
  }
  void UpdateReinforcements(){
   if(!endless||phase!=GamePhase.Play)return;
   // Expanding wave budgets must not accumulate hundreds of dead physics objects.
   for(int i=enemies.Count-1;i>=0;i--)if(enemies[i].hp<=0){enemies[i].Destroy();enemies.RemoveAt(i);}
   int living=LivingEnemies();bool opening=false;foreach(var portal in portals)if(!portal.spawned)opening=true;
   // The next pack is gated by a genuinely empty arena, rather than a periodic timer.
   // Keep a short breathing gap after clearing, with normal player control throughout.
   if(living>0||opening)batchClearAt=-1;
   if(batchIndex<batchCount&&!opening&&living==0){
    if(batchIndex==0){if(clock>=nextBatchAt)QueueReinforcementBatch();}
    else {if(batchClearAt<0)batchClearAt=clock;if(portals.Count==0&&clock>=batchClearAt+1.2f)QueueReinforcementBatch();}
   }
   for(int i=portals.Count-1;i>=0;i--){var p=portals[i];float age=clock-p.born;
    float open=Mathf.SmoothStep(0,1,Mathf.Clamp01(age/.45f)),fade=Mathf.Clamp01((3.2f-age)/.55f);
    p.view.Image("summonBagua","0",p.x,552,126*open,126*open,-age*.14f);p.view.Color(new Color(1,1,1,.8f*fade));
    p.seal.Image("summonBagua","0",p.x,607,178*open,26*open);p.seal.Color(new Color(.85f,.68f,.82f,.68f*fade));
    p.veil.Image("pixelMist",(Mathf.FloorToInt(age/.075f)%24).ToString(),p.x,566,142*open,119*open);p.veil.Color(new Color(.48f,.32f,.58f,.44f*fade));
    for(int n=0;n<3;n++){float x=p.x+(n-1)*48+Mathf.Sin(age*2+n)*3,y=n==1?493:556;y+=Mathf.Cos(age*2.4f+n)*4;p.charms[n].Image("wuxiaAtlas","talisman",x,y,14*open,32*open,Mathf.Sin(age*1.7f+n)*.13f);p.charms[n].Color(new Color(1,.85f,.78f,.9f*fade));}
    if(!p.spawned&&clock>=p.nextRelease){
     int release=Mathf.Min(9,Mathf.Min(p.count-p.released,MaxBatchEnemies-LivingEnemies()));
     if(release>0){for(int n=0;n<release;n++)SpawnReinforcement(p.x+(n-(release-1)*.5f)*32);p.released+=release;p.nextRelease=clock+.75f;p.born=clock-1.6f;fx.Animated("pixelMist",24,p.x,561,102,130,clock,.45f,new Color(.68f,.48f,.78f));}
    }
    if(!p.spawned&&age>2.5f){p.born=clock-1.6f;continue;}
    if(p.spawned&&age>=3.2f){DestroyPortal(p);portals.RemoveAt(i);}
   }
  }
  void DestroyPortal(ReinforcementPortal p){p.view.Destroy();p.seal.Destroy();p.veil.Destroy();foreach(var charm in p.charms)charm.Destroy();}
  void ClearReinforcements(){foreach(var p in portals)DestroyPortal(p);portals.Clear();batchCount=batchIndex=plannedEnemies=spawnedEnemies=0;batchClearAt=-1;}
 }
}
