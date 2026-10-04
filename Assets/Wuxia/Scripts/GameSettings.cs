using UnityEngine;
namespace Wuxia {
 [CreateAssetMenu(menuName="武侠遗境/玩法参数")]
 public class GameSettings:ScriptableObject {
  public float moveSpeed=240,jumpSpeed=740,gravity=1450,dashSpeed=780,dashDuration=.21f,dashCooldown=.68f;
  public float rangedCooldown=5,formationCooldown=8,invincibleDuration=1.25f,clearRestDuration=4.5f,musicVolume=.0224f;
  public float heroHeight=84,stoneHeight=180;
 }
}
