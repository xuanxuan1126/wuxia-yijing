using UnityEngine;
namespace Wuxia {
 [CreateAssetMenu(menuName="武侠遗境/游戏内容")]
 public class GameContent:ScriptableObject {public string sourceVersion="0.11.10";public RoomAsset[] rooms;public UpgradeData[] upgrades;public float shrineX,healX;public GameData Export(){var data=new GameData{sourceVersion=sourceVersion,rooms=new RoomData[rooms.Length],upgrades=upgrades,shrineX=shrineX,healX=healX};for(int i=0;i<rooms.Length;i++)data.rooms[i]=rooms[i].layout;return data;}}
}
