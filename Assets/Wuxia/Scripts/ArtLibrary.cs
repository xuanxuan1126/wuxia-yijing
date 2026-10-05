using System;using System.Collections.Generic;using UnityEngine;
namespace Wuxia {
 [Serializable] public class ArtFrame {public string sheet,name;public int x,y,w,h;public float anchorX,baseline;public float[] arm;public float beltX,beltY;}
 [Serializable] public class ArtManifest {public ArtFrame[] frames;}
 public class ArtLibrary {
  readonly Dictionary<string,ArtFrame> frames=new Dictionary<string,ArtFrame>();readonly Dictionary<string,Sprite> sprites=new Dictionary<string,Sprite>();readonly Dictionary<string,Texture2D> textures=new Dictionary<string,Texture2D>();
  Material additive,glow,normal;
  public Material AdditiveMaterial=>additive?additive:additive=new Material(Resources.Load<Shader>("Materials/JadeAdditive"));
  public Material NormalMaterial=>normal?normal:normal=new Material(Shader.Find("Sprites/Default"));
  public Material GlowMaterial {get{if(!glow){glow=new Material(AdditiveMaterial);glow.SetFloat("_SoftGlow",1);}return glow;}}
  readonly List<SpriteView> views=new List<SpriteView>();readonly List<Texture2D> masks=new List<Texture2D>();
  public void Register(SpriteView view)=>views.Add(view);public void Unregister(SpriteView view)=>views.Remove(view);
  public void ForgetWorld(Transform world){views.RemoveAll(v=>!v.root||v.root.transform.IsChildOf(world));}
  public void BeginStep(){for(int i=views.Count-1;i>=0;i--){if(!views[i].root){views.RemoveAt(i);continue;}views[i].BeginStep();}}
  public int ViewCount=>views.Count;
  public void Render(float alpha){for(int i=views.Count-1;i>=0;i--){if(!views[i].root){views.RemoveAt(i);continue;}views[i].Render(alpha);}}
  public void Prewarm(){foreach(var f in new List<ArtFrame>(frames.Values))Sprite(f.sheet,f.name);foreach(string sheet in new[]{"hero","heroRun"})for(int i=0;i<(sheet=="hero"?12:8);i++)Sleeve(sheet,i.ToString());foreach(string sheet in new[]{"jianMetal","templeLedge","templeCap","demonFront","whitePixel"})Sprite(sheet);}
  public static int NativeFacing(string sheet)=>sheet=="bossAtlas"?-1:1;
  public ArtLibrary(){foreach(var f in JsonUtility.FromJson<ArtManifest>(Resources.Load<TextAsset>("Data/ArtFrames").text).frames)frames[f.sheet+":"+f.name]=f;}
  public string UpgradeSheet(string id)=>frames.ContainsKey("upgradeIconsExtra:"+id)?"upgradeIconsExtra":"upgradeIcons";
  public Texture2D Texture(string key){if(!textures.TryGetValue(key,out var tex)){tex=Resources.Load<Texture2D>("Art/"+key);if(tex==null)throw new Exception("Missing texture: "+key);textures[key]=tex;}return tex;}
  public ArtFrame Frame(string sheet,string frame="0") {if(frames.TryGetValue(sheet+":"+frame,out var f))return f;var t=Texture(sheet);f=new ArtFrame{sheet=sheet,name=frame,w=t.width,h=t.height,anchorX=t.width*.5f,baseline=t.height};frames[sheet+":"+frame]=f;return f;}
  public Sprite Sprite(string sheet,string frame="0"){string id=sheet+":"+frame;if(!sprites.TryGetValue(id,out var sprite)){var f=Frame(sheet,frame);var t=Texture(sheet);sprite=UnityEngine.Sprite.Create(t,new Rect(f.x,t.height-f.y-f.h,f.w,f.h),new Vector2(.5f,.5f),100,0,SpriteMeshType.FullRect);sprite.name=id;sprites[id]=sprite;}return sprite;}
  public Sprite SwordPart(bool hilt){string id=hilt?"blade:hilt":"blade:edge";if(!sprites.TryGetValue(id,out var sprite)){var f=Frame("blade");var t=Texture("blade");int cut=Mathf.RoundToInt(f.h*.70f),top=hilt?cut:0,h=hilt?f.h-cut:cut;sprite=UnityEngine.Sprite.Create(t,new Rect(f.x,t.height-f.y-top-h,f.w,h),new Vector2(.5f,.5f),100,0,SpriteMeshType.FullRect);sprite.name=id;sprites[id]=sprite;}return sprite;}
  // Copy only the near sleeve's existing pixels. A rectangular torso crop erased the hilt.
  public Sprite Sleeve(string sheet,string frame){string id=sheet+":"+frame+":sleeve";if(!sprites.TryGetValue(id,out var sprite)){var f=Frame(sheet,frame);var t=Texture(sheet);var pixels=t.GetPixels(f.x,t.height-f.y-f.h,f.w,f.h);var polygon=f.arm??Array.Empty<float>();for(int y=0;y<f.h;y++)for(int x=0;x<f.w;x++){bool inside=false;float py=f.h-1-y;for(int a=0,b=polygon.Length-2;a<polygon.Length;b=a,a+=2)if((polygon[a+1]>py)!=(polygon[b+1]>py)&&x<(polygon[b]-polygon[a])*(py-polygon[a+1])/(polygon[b+1]-polygon[a+1])+polygon[a])inside=!inside;if(!inside)pixels[y*f.w+x]=Color.clear;}var mask=new Texture2D(f.w,f.h,TextureFormat.RGBA32,false){filterMode=FilterMode.Point,wrapMode=TextureWrapMode.Clamp};mask.SetPixels(pixels);mask.Apply(false,true);masks.Add(mask);sprite=UnityEngine.Sprite.Create(mask,new Rect(0,0,f.w,f.h),new Vector2(.5f,.5f),100,0,SpriteMeshType.FullRect);sprites[id]=sprite;}return sprite;}
  public void Dispose(){NativeObject.Remove(additive);NativeObject.Remove(glow);NativeObject.Remove(normal);foreach(var s in sprites.Values)NativeObject.Remove(s);sprites.Clear();foreach(var mask in masks)NativeObject.Remove(mask);masks.Clear();}
 }
 public class SpriteView {
  public readonly GameObject root;public readonly SpriteRenderer renderer;readonly ArtLibrary art;Vector3 previous,current;Quaternion previousRotation,currentRotation;bool posed,snapNext;
  public void BeginStep(){previous=current;previousRotation=currentRotation;}
  public void SnapNext()=>snapNext=true;
  public void Position(Vector3 position,Quaternion rotation){current=position;currentRotation=rotation;if(!posed||snapNext){previous=current;previousRotation=currentRotation;posed=true;snapNext=false;}root.transform.SetPositionAndRotation(current,currentRotation);}
  // Pose changes are immediate, while physical movement remains interpolated. Mixing old
  // cloth/weapon offsets with a new animation frame made the sword slide through the robe.
  public void Attach(Actor actor,Vector3 offset,Quaternion rotation){previous=new Vector3(actor.previousPosition.x,actor.previousPosition.y,0)+offset;current=new Vector3(actor.body.position.x,actor.body.position.y,0)+offset;previousRotation=currentRotation=rotation;posed=true;snapNext=false;root.transform.SetPositionAndRotation(current,rotation);}
  public void MatchPose(SpriteView source,Sprite sprite){renderer.sprite=sprite;renderer.flipX=source.renderer.flipX;root.transform.localScale=source.root.transform.localScale;previous=source.previous;current=source.current;previousRotation=source.previousRotation;currentRotation=source.currentRotation;posed=source.posed;root.transform.SetPositionAndRotation(current,currentRotation);}
  public void Render(float alpha){if(root&&renderer&&root.activeInHierarchy&&posed&&renderer.enabled)root.transform.SetPositionAndRotation(Vector3.LerpUnclamped(previous,current,alpha),Quaternion.SlerpUnclamped(previousRotation,currentRotation,alpha));}
  public SpriteView(ArtLibrary a,Transform parent,string name,int order){art=a;art.Register(this);root=new GameObject(name);if(parent)root.transform.SetParent(parent,false);renderer=root.AddComponent<SpriteRenderer>();renderer.sortingOrder=order;}
  public void Pose(string sheet,int index,float x,float feet,float scale,int dir=1){Pose(sheet,index.ToString(),x,feet,scale,dir);}
  public void Pose(string sheet,string index,float x,float feet,float scale,int dir=1){var f=art.Frame(sheet,index);int orientation=dir*ArtLibrary.NativeFacing(sheet);renderer.sprite=art.Sprite(sheet,index);renderer.flipX=orientation<0;root.transform.localScale=Vector3.one*scale;Position(PixelWorld.Position(x+orientation*(f.x+f.w*.5f-f.anchorX)*scale,feet+(f.y+f.h*.5f-f.baseline)*scale),Quaternion.identity);}
  public void PoseActor(string sheet,int index,Actor actor,float scale,int dir=1,float lift=0){Pose(sheet,index,actor.X,actor.Feet-lift,scale,dir);Attach(actor,root.transform.position-(Vector3)actor.body.position,Quaternion.identity);}
  public void SwordSection(bool hilt,Actor actor,float x,float y,float w,float h,float radians){renderer.sprite=art.SwordPart(hilt);renderer.flipX=false;var full=art.Frame("blade");float start=hilt?Mathf.RoundToInt(full.h*.70f)/(float)full.h:0,end=hilt?1:Mathf.RoundToInt(full.h*.70f)/(float)full.h,offset=(.5f-(start+end)*.5f)*h;root.transform.localScale=new Vector3(w/renderer.sprite.rect.width,h*(end-start)/renderer.sprite.rect.height,1);var position=PixelWorld.Position(x+Mathf.Sin(radians)*offset,y-Mathf.Cos(radians)*offset);Attach(actor,position-(Vector3)actor.body.position,Quaternion.Euler(0,0,-radians*Mathf.Rad2Deg));}
  public void Image(string sheet,string frame,float x,float y,float w,float h,float radians=0){renderer.sprite=art.Sprite(sheet,frame);renderer.flipX=false;root.transform.localScale=new Vector3(w/renderer.sprite.rect.width,h/renderer.sprite.rect.height,1);Position(PixelWorld.Position(x,y),Quaternion.Euler(0,0,-radians*Mathf.Rad2Deg));}
  public void Additive(bool value=true,bool softGlow=false)=>renderer.sharedMaterial=value?(softGlow?art.GlowMaterial:art.AdditiveMaterial):art.NormalMaterial;
  public void Color(Color c)=>renderer.color=c;public void Visible(bool show)=>renderer.enabled=show;public void Destroy(){art.Unregister(this);NativeObject.Remove(root);}
 }
 public class Actor {
  public bool IsFlying=>kind=="moth"||kind=="eye";public float spawnGraceUntil,attackPower=-1;
  public string kind;public float hp,maxHp,min,max,baseY,hitAt=-100,windup,chargeUntil,aimX,aimY,frozenUntil;public int dir=-1,chargeDir=-1;public bool landed=true,active;public string state="sleep";public int move,lastSkill=-1;public float entered,next,targetX;
  public GameObject root;public Rigidbody2D body;public BoxCollider2D collider;public SpriteView view;public float width,height;
  public float TerrainHeight=>kind=="player"?90:kind=="crawler"?84:kind=="bone"||kind=="cultist"?88:height;
  public float X=>body.position.x*100;public float Feet=>720-body.position.y*100+height*.5f;public float CenterY=>Feet-height*.5f;
  public float VX {get=>body.linearVelocity.x*100;set=>body.linearVelocity=new Vector2(value/100,body.linearVelocity.y);}
  public float VY {get=>-body.linearVelocity.y*100;set=>body.linearVelocity=new Vector2(body.linearVelocity.x,-value/100);}
  public Vector2 previousPosition;public float RenderX(float alpha)=>Mathf.Lerp(previousPosition.x,body.position.x,alpha)*100;
  public void BeginStep()=>previousPosition=body.position;
  static PhysicsMaterial2D surface;public static PhysicsMaterial2D Surface {get{if(!surface)surface=new PhysicsMaterial2D("无摩擦 · 防台沿粘滞"){friction=0,bounciness=0};return surface;}}
  readonly RaycastHit2D[] feetHits=new RaycastHit2D[8];
  public Rect Bounds=>new Rect(X-width/2,Feet-height,width,height);
  public bool Grounded {get{if(body.linearVelocity.y>.03f)return false;var foot=body.position+Vector2.down*(height*.5f/100);int count=Physics2D.BoxCastNonAlloc(foot+Vector2.up*.035f,new Vector2(width*.78f/100,.02f),0,Vector2.down,feetHits,.07f,1<<9);for(int i=0;i<count;i++){var hit=feetHits[i];if(hit.collider&&hit.normal.y>.65f&&Mathf.Abs(hit.collider.bounds.max.y-foot.y)<.075f)return true;}return false;}}

  public Actor(ArtLibrary art,Transform parent,string name,string k,float x,float feet,float w,float h){kind=k;width=w;height=h;string prefab=k=="player"?"Player":k=="crawler"||k=="bone"||k=="cultist"?"Bandit":k=="moth"||k=="eye"?"Crane":k=="golem"?"StoneGuardian":"CorruptedMaster";var template=Resources.Load<GameObject>("Prefabs/"+prefab);root=template?UnityEngine.Object.Instantiate(template,parent):new GameObject(name);root.name=name;root.layer=8;root.transform.SetParent(parent,false);root.transform.position=PixelWorld.Position(x,feet-h/2);body=root.GetComponent<Rigidbody2D>();if(!body)body=root.AddComponent<Rigidbody2D>();body.gravityScale=IsFlying?0:1;body.constraints=RigidbodyConstraints2D.FreezeRotation;body.collisionDetectionMode=CollisionDetectionMode2D.Continuous;body.interpolation=RigidbodyInterpolation2D.None;body.sleepMode=RigidbodySleepMode2D.NeverSleep;collider=root.GetComponent<BoxCollider2D>();if(!collider)collider=root.AddComponent<BoxCollider2D>();collider.size=new Vector2(w/100,TerrainHeight/100);collider.offset=new Vector2(0,(TerrainHeight-h)/200);collider.sharedMaterial=Surface;body.position=PixelWorld.Position(x,feet-h/2);previousPosition=body.position;view=new SpriteView(art,parent,name+" · 动画",k=="player"?20:19);}
  public void Place(float x,float feet){var position=PixelWorld.Position(x,feet-height/2);root.transform.position=position;body.position=position;previousPosition=position;body.linearVelocity=Vector2.zero;view.SnapNext();}
  public void Disable(){body.simulated=false;collider.enabled=false;view.Visible(false);}
  public void Destroy(){Disable();NativeObject.Remove(root);view.Destroy();}
 }
 public class PlatformView {public PlatformData data;public GameObject root;public BoxCollider2D collider;public SpriteView top,fill;public readonly List<SpriteView> trim=new List<SpriteView>();public void Visible(bool show){top.Visible(show);foreach(var v in trim)v.Visible(show);}public bool broken;}
}
