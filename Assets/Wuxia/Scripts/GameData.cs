using System;
using System.Collections.Generic;
using UnityEngine;
namespace Wuxia {
 [Serializable] public class PlatformData {public float x,y,w,h;}
 [Serializable] public class EnemyData {public string kind;public float x,y,min,max;}
 [Serializable] public class ExitData {public float x,y,spawnX,spawnY;public int to;public string label;public bool requiresDash;}
 [Serializable] public class RoomData {public string name,subtitle;public float width;public PlatformData[] platforms;public EnemyData[] enemies;public ExitData[] exits;}
 [Serializable] public class UpgradeData {public string id,name,kind,icon,desc;public int max;}
 [Serializable] public class GameData {public string sourceVersion;public RoomData[] rooms;public UpgradeData[] upgrades;public float shrineX,healX;public static GameData Load(){var catalog=Resources.Load<GameContent>("Data/Content");return catalog?catalog.Export():JsonUtility.FromJson<GameData>(Resources.Load<TextAsset>("Data/GameData").text);}}
 public enum GamePhase {Menu,Story,Guide,Play,Paused,Unlock,Dead,Won,WaveClear,Rest,EndlessOver}
 [Serializable] public struct InputFrame {public float move;public bool jump,jumpHeld,attack,ranged,dash,ward,interact;}
 public struct WaveSpec {public string boss;public int count,batches;public float hpScale,bossScale,damage;}
 public class RogueState {
  public int wave,cleared,kills,melees,best;public float maxHp=10,nextOrbit,nextThunder,nextFrost,nextRain,nextFinger,nextBrand,shieldReady;public bool revived;public Dictionary<string,int> levels=new Dictionary<string,int>();
  public int Level(string id)=>levels.TryGetValue(id,out int n)?n:0;
  public float DamageScale=>1+Level("power")*.2f;
  public float MoveScale=>1+Level("swift")*.06f;
  public float DashCooldownScale=>1-Level("agility")*.12f;
  public float IncomingScale=>1-Level("armor")*.08f;
  public float CriticalChance=>Level("focus")*.08f;
  public float CooldownScale=>1-Mathf.Min(6,Level("haste"))*.08f;
  public RogueState Clone(){var copy=(RogueState)MemberwiseClone();copy.levels=new Dictionary<string,int>(levels);return copy;}
 }
 public static class Rules {
  // Count is the entire wave budget; later waves increase both batch count and the budget of EACH batch without a four-pack ceiling.
  public static WaveSpec Wave(int number){
   int w=Math.Max(1,number);bool boss=w%5==0;
   int batches=boss?(w>=15?1:0):w<4?1:w<9?2:3+(w-9)/10;
   int perBatch=12+Math.Max(0,w-4)/4;
   int count=boss?(w>=15?Math.Min(6,2+w/15):0):w<4?4+(w-1)*2:batches*perBatch;
   return new WaveSpec{boss=w%10==0?"master":boss?"golem":null,batches=batches,count=count,hpScale=1+.16f*(w-1)+.002f*(w-1)*(w-1),bossScale=1+.09f*(w-1),damage=Mathf.Round((1+.075f*(w-1))*10)/10};
  }
  public static string[] EnemyRoster(int wave)=>wave>=12?new[]{"crawler","moth","bone","cultist","eye"}:wave>=8?new[]{"crawler","moth","bone","cultist"}:wave>=6?new[]{"crawler","moth","cultist"}:new[]{"crawler","moth"};
  public static string[] Offers(GameData data,RogueState run,System.Random random){var pool=new List<string>();foreach(var u in data.upgrades)if(u.id!="heal"&&run.Level(u.id)<u.max)pool.Add(u.id);Shuffle(pool,random);if(pool.Count<2)throw new InvalidOperationException("At least two repeatable upgrades are required");var cards=new List<string>{pool[0],pool[1],"heal"};Shuffle(cards,random);return cards.ToArray();}
  static void Shuffle(List<string> list,System.Random r){for(int i=list.Count-1;i>0;i--){int j=r.Next(i+1);string a=list[i];list[i]=list[j];list[j]=a;}}
  public static float Apply(GameData data,RogueState state,string id,float hp){var u=Array.Find(data.upgrades,v=>v.id==id);if(u==null||state.Level(id)>=u.max)return hp;if(id=="heal"){if(hp>=state.maxHp)state.maxHp++;return Mathf.Min(state.maxHp,hp+state.maxHp*.6f);}state.levels[id]=state.Level(id)+1;if(id=="vitality"){state.maxHp+=2;hp+=2;}return Mathf.Min(state.maxHp,hp);}
  public static int MasterSkill(int last,System.Random random){int roll=random.Next(last<0?3:2);return last<0?roll:roll>=last?roll+1:roll;}
 }
 public static class PixelWorld {
  public static Vector3 Position(float x,float y,float z=0)=>new Vector3(x/100,(720-y)/100,z);
  public static float X(Vector3 p)=>p.x*100;public static float Y(Vector3 p)=>720-p.y*100;
 }
}
