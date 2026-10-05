using System;using System.IO;using System.Collections.Generic;using UnityEngine;using UnityEditor;
namespace Wuxia.Editor {
 public static class EndlessExpansionValidation {
  static WuxiaGame game;static readonly List<string> checks=new List<string>();
  static void Check(bool value,string label){if(!value)throw new Exception("ENDLESS_17 FAILED: "+label);checks.Add(label);Debug.Log("PASS ENDLESS_17: "+label);}
  static void Tick(int count,InputFrame input=default){for(int i=0;i<count;i++){game.Step(1/60f,input);input.jump=input.dash=input.ward=false;}}
  static void Solo(string kind,float x,float feet){game.StartEndless();game.FinishGuide();game.run.wave=11;game.BeginWave();game.player.Place(1000,620);Tick(110);var selected=game.enemies.Find(e=>e.kind==kind);if(selected==null)throw new Exception("Missing real portal enemy "+kind);foreach(var e in game.enemies)if(e!=selected){e.hp=0;e.Disable();}selected.Place(x,feet);selected.spawnGraceUntil=-1;selected.windup=selected.chargeUntil=0;selected.hitAt=-100;selected.state="sleep";}
  public static void Prepare(){AssetDatabase.Refresh();var raw=JsonUtility.FromJson<GameData>(File.ReadAllText("Assets/Wuxia/Resources/Data/GameData.json"));var content=AssetDatabase.LoadAssetAtPath<GameContent>("Assets/Wuxia/Resources/Data/Content.asset");content.upgrades=raw.upgrades;content.sourceVersion=raw.sourceVersion;EditorUtility.SetDirty(content);AssetDatabase.SaveAssets();}
  public static void RunAndBuild(){Prepare();PortValidation.Run();OptimizationValidation.Run();AnimationCombatValidation.Run();HtmlArtValidation.Run();OcclusionTerrainValidation.Run();Run();ProjectBuilder.BuildMac();}
  public static void Run(){checks.Clear();var root=new GameObject("无尽扩展验证");try{
   game=root.AddComponent<WuxiaGame>();game.Initialize();game.skipSimulation=true;game.StartEndless();game.FinishGuide();game.invincibleUntil=10000;
   Check(game.data.upgrades.Length==20,"十二张旧卡与八张新卡共同进入真实游戏目录");
   foreach(string id in new[]{"swift","agility","armor","focus","rain","finger","brand","afterimage"})Check(game.art.Sprite(game.art.UpgradeSheet(id),id).rect.width==128,"新卡具有独立授权图标 "+id);
   var copy=game.run.Clone();float current=game.hp;Rules.Apply(game.data,copy,"swift",current);Check(game.run.Level("swift")==0&&copy.MoveScale>1,"新属性预览不提前修改当前角色");
   foreach(string id in new[]{"swift","agility","armor","focus"})game.hp=Rules.Apply(game.data,game.run,id,game.hp);
   Check(Mathf.Abs(game.run.MoveScale-1.06f)<.001f&&Mathf.Abs(game.run.DashCooldownScale-.88f)<.001f,"轻功属性分别影响移动速度与身法冷却");
   game.invincibleUntil=-1;float hp=game.hp;game.Damage(false,0);Check(Mathf.Abs(hp-game.hp-.92f)<.001f,"护体罡气按卡面降低八个百分点的伤害");game.invincibleUntil=10000;
   game.run.levels.Remove("focus");game.run.wave=11;game.BeginWave();game.invincibleUntil=10000;
   Check(game.PendingEnemies>0&&game.enemies.Count==0,"后期波次从预告魔门开始，敌人尚未瞬间出现");Tick(1);Check(game.PortalCount==2,"一批敌人由地图两侧的两个魔门入场");Check(game.WuxiaPortalsActive,"魔门使用阴阳八卦、墨烟与三张朱砂符，没有石拱门贴图");Check(game.phase==GamePhase.Play,"没有现存敌人但还有伏兵时不会提前领奖");
   int pending=game.PendingEnemies;Tick(60);Check(game.enemies.Count==0&&game.PendingEnemies==pending,"传送门一秒预警期间不生成有伤害的敌人");Tick(35);Check(game.enemies.Count==14&&game.PendingEnemies==pending-14,"预警后实际生成第一小批敌人");
   var kinds=new HashSet<string>();foreach(var e in game.enemies)kinds.Add(e.kind);Check(kinds.Contains("bone")&&kinds.Contains("cultist")&&kinds.Contains("eye"),"第十二波首批已混合玄甲剑卫、符刃门徒与墨羽灵鸦");
   Check(game.enemies.TrueForAll(e=>Mathf.Abs(e.X-game.player.X)>200),"魔门选址保持与角色的安全距离");
   int firstBatch=game.ReinforcementBatch;Tick(900);Check(game.ReinforcementBatch==firstBatch&&game.PendingEnemies==pending-14,"当前批次仍有敌人时，十五秒等待也不会自动打开下一批");
   foreach(var e in game.enemies){e.hp=0;e.Disable();}Tick(1);float clearX=game.player.X;Tick(30,new InputFrame{move=1});Check(game.phase==GamePhase.Play&&game.player.X>clearX+40&&game.ReinforcementBatch==firstBatch,"清完小批后的短暂间隔可自由移动且尚未选卡");Tick(200);Check(game.ReinforcementBatch==2&&game.Targets().Count==14,"清空后第二批重新出现完整十四只，而非拆分第一批人数");
   for(int i=0;i<3000&&game.phase==GamePhase.Play;i++){foreach(var e in game.enemies){e.hp=0;e.Disable();}game.Step(1/60f,default);}
   Check(game.phase==GamePhase.WaveClear&&game.run.cleared==12,"杀完全部三小批后才进入本波清场阶段");Tick(280);Check(game.phase==GamePhase.Rest&&game.offers.Length==3,"多批波次仍只有一次最终选卡，不重复打开界面");
   Solo("bone",520,620);game.player.Place(440,620);game.invincibleUntil=-1;hp=game.hp;Tick(3);var bone=game.enemies.Find(e=>e.kind=="bone");Check(bone.windup>0&&bone.dir==-1,"玄甲剑卫近身时进入有动作预告的左向挥剑");Tick(24);Check(game.hp<hp,"玄甲挥剑从接触距离以外真实伤害主角");game.player.Place(590,620);Tick(1);Check(bone.dir==-1,"玄甲挥剑预告后方向锁定，不随角色绕背瞬间翻转");
   Solo("cultist",720,620);game.player.Place(400,620);game.invincibleUntil=-1;hp=game.hp;Tick(155);Check(game.hp<hp,"符师蓄力后的追魂魔符真实追踪并命中主角");
   Solo("eye",510,427.5f);game.player.Place(400,620);game.invincibleUntil=-1;Tick(5);var eye=game.enemies.Find(e=>e.kind=="eye");Check(eye.windup==0,"墨羽灵鸦先检查完整碰撞体的俯冲路线，不朝高台迎面撞入");eye.Place(850,427.5f);game.player.Place(640,620);hp=game.hp;Tick(120);Check(game.hp<hp,"墨羽灵鸦蓄势后锁定方向俯冲并造成接触伤害");
   Solo("cultist",75,620);game.player.Place(140,620);game.invincibleUntil=10000;var edgeCultist=game.enemies.Find(e=>e.kind=="cultist");edgeCultist.chargeUntil=game.clock+100;Tick(30);Check(edgeCultist.X>=69,"符师退避也不会退出地图导致波次无法清场");
   game.StartFinal();game.FinishGuide();game.invincibleUntil=10000;game.player.Place(500,620);game.boss.active=true;game.boss.hp=game.boss.maxHp=1000;game.ChangeBoss("recover",100);game.run=new RogueState();
   game.run.levels["rain"]=1;game.run.nextRain=0;Tick(80);Check(game.boss.hp<1000&&!string.IsNullOrEmpty(game.boss.view.renderer.sprite.name),"惊鸿剑雨使用真正投射物命中首领");Check(game.dashUntil==0,"自动剑雨不错误占用普通飞剑或身法状态");
   game.run.levels.Clear();game.run.levels["finger"]=1;game.run.nextFinger=0;float before=game.boss.hp;Tick(60);Check(game.boss.hp<before,"太虚剑指向首领发射两道贯穿剑气");
   game.run.levels.Clear();game.run.levels["brand"]=1;game.run.nextBrand=0;before=game.boss.hp;Tick(20);Check(game.SwordBrandCount==1&&game.boss.hp==before,"焚邪剑印结印阶段先展示预告，尚未结算伤害");Tick(60);Check(game.boss.hp<before&&game.SwordBrandCount==0,"剑印延迟爆发伤害后清理纹章对象");
   game.run.levels.Clear();game.run.levels["afterimage"]=1;game.player.Place(game.boss.X-145,620);Tick(1,new InputFrame{dash=true,move=1});before=game.boss.hp;Tick(16);Check(game.boss.hp<before,"回风剑舞在真实御剑结束时自动挥斩附近敌人");Check(!game.fx.HasActiveSheet("hero"),"御剑收势保留回斩但不生成主角拖尾残像");
   game.StartEndless();game.FinishGuide();game.run.wave=49;game.BeginWave();game.invincibleUntil=10000;game.boss.hp=1;game.boss.active=true;game.Strike(game.boss,1,true,999);game.FinishWave();Check(game.phase==GamePhase.Play,"高波次首领死后仍须清完尚未出场的随从");Check(game.notice.Contains("伏兵")&&game.ReinforcementLabel.Contains("魔门"),"首领提前击败时明确提示仍有魔门伏兵，避免误以为卡死");
   int max=0;for(int i=0;i<1000&&game.phase==GamePhase.Play;i++){game.Step(1/60f,default);max=Math.Max(max,game.Targets().Count);if(i>700)foreach(var e in game.enemies){e.hp=0;e.Disable();}}Check(max<=WuxiaGame.MaxBatchEnemies,"持续等待的高波次也保持最多十八名存活小怪");
   game.StartEndless();game.FinishGuide();game.run.wave=22;game.BeginWave();Tick(1);Check(game.PortalCount>0,"切关清理测试确实已打开魔门");game.ShowMenu();Check(game.PortalCount==0&&game.PendingEnemies==0&&game.SwordBrandCount==0,"回主界面清理全部未出场伏兵、魔门与新技能纹章");
   foreach(int wave in new[]{4,6,12,23,39,99}){
    game.StartEndless();game.FinishGuide();game.run.wave=wave-1;game.BeginWave();game.invincibleUntil=10000;var spec=Rules.Wave(wave);int perBatch=spec.count/spec.batches;
    for(int batch=1;batch<=spec.batches;batch++){
     int budgetStart=(batch-1)*perBatch,budgetEnd=batch*perBatch;
     for(int i=0;i<600&&game.ReleasedEnemies==budgetStart;i++)game.Step(1/60f,default);
     int alive=game.Targets().Count;Check(game.ReinforcementBatch==batch&&alive>0&&alive<=WuxiaGame.MaxBatchEnemies,"第"+wave+"波第"+batch+"小批按同屏上限入场");
     Tick(120);Check(game.ReinforcementBatch==batch,"第"+wave+"波第"+batch+"小批未清空不提前跳到下批");
     bool stats=true;foreach(var e in game.enemies)if(e.hp>0){float baseHp=e.kind=="bone"?6:e.kind=="cultist"?5:e.IsFlying?3:4;stats&=e.maxHp==Mathf.Ceil(baseHp*spec.hpScale*(1+.04f*(batch-1)))&&Mathf.Abs(e.attackPower-spec.damage*(1+.03f*(batch-1)))<.001f;}
     Check(stats,"第"+wave+"波第"+batch+"小批真实敌人的生命与攻击按波次及批次增长");
     int previousBatch=batch;for(int i=0;i<2000&&game.phase==GamePhase.Play;i++){
      foreach(var e in game.enemies){e.hp=0;e.Disable();}game.Step(1/60f,default);
      if(game.ReleasedEnemies>=budgetEnd||game.phase==GamePhase.WaveClear)break;
      if(game.ReinforcementBatch!=previousBatch)throw new Exception("Skipped unspawned batch budget");
     }
     Check(game.enemies.Count<=WuxiaGame.MaxBatchEnemies,"第"+wave+"波第"+batch+"小批清理阵亡实体，场景不会随累计击杀膨胀");
     Check(game.phase==GamePhase.WaveClear||game.ReleasedEnemies==budgetEnd,"第"+wave+"波第"+batch+"小批实际完整释放"+perBatch+"只（超同屏上限也不会丢失或卡死）");
     foreach(var e in game.enemies){e.hp=0;e.Disable();}Tick(1);
    }
    Check(game.phase==GamePhase.WaveClear&&game.run.cleared==wave,"第"+wave+"波全部小批清完才进入休息");
   }
   Check(Rules.Wave(2).count>Rules.Wave(1).count&&Rules.Wave(3).count>Rules.Wave(2).count,"前三波入门数量也逐波递增");
   Check(Rules.Wave(49).batches>Rules.Wave(29).batches&&Rules.Wave(99).hpScale>Rules.Wave(49).hpScale&&Rules.Wave(99).damage>Rules.Wave(49).damage,"后期小批数、生命、伤害均继续随波次成长");
   Solo("bone",520,620);game.player.Place(440,620);game.invincibleUntil=-1;var attackGuard=game.enemies.Find(e=>e.kind=="bone");attackGuard.attackPower=3.7f;hp=game.hp;Tick(27);Check(Mathf.Abs(hp-game.hp-3.7f)<.01f,"近战命中实际使用敌人攻击属性，而非固定一伤害");
   Solo("cultist",720,620);game.player.Place(400,620);game.invincibleUntil=-1;var attackDisciple=game.enemies.Find(e=>e.kind=="cultist");attackDisciple.attackPower=2.8f;hp=game.hp;Tick(155);Check(Mathf.Abs(hp-game.hp-2.8f)<.01f,"符刃投射物携带敌人攻击属性并真实结算");
   foreach(string sheet in new[]{"guardMotion","guardAttack","discipleMotion","ravenMotion"}){
    int total=sheet=="guardMotion"?12:sheet=="guardAttack"?4:16;for(int i=0;i<total;i++)Check(game.art.Frame(sheet,i.ToString()).w>0&&game.art.Frame(sheet,i.ToString()).h>0,"完整动作切片："+sheet+":"+i);
    Check(game.art.Frame(sheet,(total-1).ToString()).w>0&&game.art.Texture(sheet)!=game.art.Texture("masterAtlas"),"独立敌人图集，不复用掌门素材："+sheet);
   }
   Check(Rules.Wave(99).count>Rules.Wave(4).count&&Rules.Wave(99).batches==12&&Rules.Wave(99).count==420,"后期持续增长，第九十九波十二小批、每批三十五只，不受四批上限限制");
   Directory.CreateDirectory("Documentation/Validation");File.WriteAllText("Documentation/Validation/endless-expansion-regression.txt",string.Join("\n",checks));Debug.Log("WUXIA_ENDLESS_17_OK "+checks.Count);
  }catch(Exception e){Debug.LogException(e);if(Application.isBatchMode)EditorApplication.Exit(1);throw;}finally{UnityEngine.Object.DestroyImmediate(root);}}
 }
}
