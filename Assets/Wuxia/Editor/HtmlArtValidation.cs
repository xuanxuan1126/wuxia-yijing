using System;using System.IO;using System.Collections;using System.Collections.Generic;using System.Reflection;using UnityEngine;using UnityEngine.UI;using UnityEditor;
namespace Wuxia.Editor {
 public static class HtmlArtValidation {
  static WuxiaGame game;static readonly List<string> passed=new List<string>();
  static void Check(bool ok,string name){if(!ok)throw new Exception("HTML_ART FAILED: "+name);passed.Add(name);Debug.Log("PASS HTML_ART: "+name);}
  static void Tick(int count,InputFrame input=default){for(int i=0;i<count;i++){game.Step(1/60f,input);input.ward=false;}}
  static object Field(object value,string name)=>value.GetType().GetField(name,BindingFlags.Public|BindingFlags.NonPublic|BindingFlags.Instance).GetValue(value);
  static int Trails(){int n=0;foreach(var line in game.GetComponentsInChildren<LineRenderer>())if(line.name=="万剑 · 渐变剑轨")n++;return n;}
  public static void Run(){passed.Clear();var root=new GameObject("HTML 美术迁移验证");try{
   game=root.AddComponent<WuxiaGame>();game.Initialize();game.skipSimulation=true;game.StartFinal();game.FinishGuide();game.invincibleUntil=10000;game.player.Place(420,620);game.boss.active=true;game.ChangeBoss("recover",100);
   for(int i=0;i<8;i++)Check(game.art.Frame("heroRun",i.ToString()).arm.Length>=20,"原 HTML 跑步帧具有独立近袖遮挡 "+i);
   Tick(1,new InputFrame{move=1});Check(game.player.view.renderer.sprite.texture==game.art.Texture("heroRun"),"实际移动使用原 HTML 跑步原画而非 Unity 1.3 重绘");Check(Mathf.Abs(game.player.view.root.transform.localScale.x-.21f)<.001f,"跑步按原画84/400比例，不沿用不同图集的缩放");
   Tick(20,new InputFrame{ward=true});var swords=(IList)Field(game,"formation");Check(swords.Count==12,"蓄力期十二剑同时围绕主角展开");float min=999,max=-999;foreach(var sword in swords){float a=(float)Field(sword,"angle");min=Mathf.Min(min,a);max=Mathf.Max(max,a);var view=(SpriteView)Field(sword,"view");Check(view.renderer.sharedMaterial==game.art.AdditiveMaterial,"飞剑复用同一柔和加色材质");}Check(max-min>5,"未发射剑朝向环形外侧，不是十二把竖直剑");
   var outer=(SpriteView)Field(game,"sealOuter");var glow=(SpriteView)Field(game,"sealGlow");Check(outer.renderer.sortingOrder<game.player.view.renderer.sortingOrder&&outer.renderer.sharedMaterial==game.art.AdditiveMaterial,"法阵在身体背后叠加，不遮住人物");Check(glow.renderer.sharedMaterial==game.art.GlowMaterial&&glow.renderer.enabled,"法阵柔光与纹线分层，蓄力展开时可见");
   Tick(12);Check(Trails()>0&&Trails()<=12,"发射剑使用最多十二条独立曲线剑轨");Tick(200);Check(game.FormationDamage>=22,"恢复原画效果仍可靠命中至少十一剑");Check(game.ProjectileCount==0&&Trails()==0,"技能结束销毁剑轨与剑对象，无残留分配泄漏");Check(!outer.renderer.enabled&&!glow.renderer.enabled,"法阵和柔光在1.12秒后完整退场");
   game.StartEndless();game.FinishGuide();game.hp=2;game.offers=new[]{"heal","damage","maxHp"};
   // Use real upgrade IDs from this release for the two non-healing choices.
   int index=1;foreach(var u in game.data.upgrades)if(u.id!="heal"&&index<3)game.offers[index++]=u.id;
   game.ui.ShowFormationDamage(24,game.player.X,game.player.CenterY,true);game.ui.Refresh();var feedback=(Text)Field(game.ui,"formationFeedback");Check(feedback.gameObject.activeSelf&&feedback.text=="24","剑阵累计伤害数字在实际游戏阶段可见");Tick(45);game.ui.Refresh();Check(!feedback.gameObject.activeSelf,"伤害数字在短暂反馈后退场，不常驻遮挡");
   game.phase=GamePhase.Rest;game.rewardTotal=game.rewardRemaining=1;game.pendingUpgrade=null;game.ui.ShowCards();
   var frames=game.ui.GetComponentsInChildren<CultivationFrame>();var medals=game.ui.GetComponentsInChildren<CultivationMedallion>();Check(frames.Length==3&&medals.Length==3,"三张纸卡各自使用圆形图标底座");
   Check(frames[0].rectTransform.sizeDelta==new Vector2(286,342)&&Mathf.Abs(frames[1].rectTransform.anchoredPosition.x-frames[0].rectTransform.anchoredPosition.x-330)<.001f,"选卡恢复HTML卡幅与均匀间距");Check(medals[0].rectTransform.sizeDelta==new Vector2(86,86),"图标圆框缩小到86像素而非100像素方框");
   var button=Array.Find(game.ui.GetComponentsInChildren<Button>(),b=>b.name=="机缘 · heal");float hp=game.hp;button.onClick.Invoke();Check(game.pendingUpgrade=="heal"&&game.hp==hp,"回春丹点击只预览，不提前修改气血或触发波次");
   game.ChooseCard(game.offers[1]);Check(game.pendingUpgrade==game.offers[1]&&game.hp==hp,"已经选丹药后仍可改选另一张卡");game.ChooseCard("heal");var confirm=Array.Find(game.ui.GetComponentsInChildren<Button>(),b=>b.name=="确认 · 进入下一步");Check(confirm.GetComponent<RectTransform>().sizeDelta==new Vector2(360,53),"确认按钮独立放置，移除占据画面的大块底部卷轴");confirm.onClick.Invoke();Check(game.phase==GamePhase.Play&&game.hp>hp,"确认回春丹后只应用一次，正常进入下一波");
   game.rewardTotal=game.rewardRemaining=2;game.phase=GamePhase.Rest;game.ui.ShowCards();game.ChooseCard("heal");game.ConfirmCard();Check(game.phase==GamePhase.Rest&&game.rewardRemaining==1,"首领双奖励仍分两次选择与确认");
   game.ShowMenu();Check(Trails()==0,"返回主界面彻底清理归宗剑轨");Directory.CreateDirectory("Documentation/Validation");File.WriteAllText("Documentation/Validation/html-art-regression.txt",string.Join("\n",passed));Debug.Log("WUXIA_HTML_ART_OK "+passed.Count);
  }catch(Exception e){Debug.LogException(e);if(Application.isBatchMode)EditorApplication.Exit(1);throw;}finally{UnityEngine.Object.DestroyImmediate(root);}}
 }
}
