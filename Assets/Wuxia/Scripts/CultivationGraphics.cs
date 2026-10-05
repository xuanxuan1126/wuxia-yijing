using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
namespace Wuxia {
 // Native UI geometry retains crisp circular medallions and thin parchment borders.
 public class CultivationMedallion : MaskableGraphic {
  public float rim=2;
  public Color ring=new Color(.73f,.62f,.40f);
  protected override void OnPopulateMesh(VertexHelper vh){
   vh.Clear();var r=rectTransform.rect;var center=r.center;float radius=Mathf.Min(r.width,r.height)*.5f;
   const int segments=64;
   for(int i=0;i<segments;i++){
    float a=i*Mathf.PI*2/segments,b=(i+1)*Mathf.PI*2/segments;
    var pa=new Vector2(Mathf.Cos(a),Mathf.Sin(a));var pb=new Vector2(Mathf.Cos(b),Mathf.Sin(b));int n=vh.currentVertCount;
    vh.AddVert(center,color,Vector2.zero);vh.AddVert(center+pa*(radius-rim),color,Vector2.zero);vh.AddVert(center+pb*(radius-rim),color,Vector2.zero);vh.AddTriangle(n,n+1,n+2);
    n=vh.currentVertCount;vh.AddVert(center+pa*(radius-rim),ring,Vector2.zero);vh.AddVert(center+pa*radius,ring,Vector2.zero);vh.AddVert(center+pb*radius,ring,Vector2.zero);vh.AddVert(center+pb*(radius-rim),ring,Vector2.zero);vh.AddTriangle(n,n+1,n+2);vh.AddTriangle(n,n+2,n+3);
   }
  }
 }
 public class CultivationFrame : MaskableGraphic,IPointerEnterHandler,IPointerExitHandler {
  bool hovered,selected;public float inset;
  public void Select(bool value){selected=value;SetVerticesDirty();}
  public void OnPointerEnter(PointerEventData e){hovered=true;SetVerticesDirty();}
  public void OnPointerExit(PointerEventData e){hovered=false;SetVerticesDirty();}
  protected override void OnPopulateMesh(VertexHelper vh){
   vh.Clear();var r=rectTransform.rect;float w=(hovered||selected)?3:2;var c=(hovered||selected)?new Color(.95f,.82f,.53f):new Color(.68f,.57f,.37f);
   Quad(vh,r.xMin+inset,r.yMin+inset,r.width-inset*2,w,c);Quad(vh,r.xMin+inset,r.yMax-inset-w,r.width-inset*2,w,c);
   Quad(vh,r.xMin+inset,r.yMin+inset,w,r.height-inset*2,c);Quad(vh,r.xMax-inset-w,r.yMin+inset,w,r.height-inset*2,c);
  }
  static void Quad(VertexHelper vh,float x,float y,float w,float h,Color c){int n=vh.currentVertCount;vh.AddVert(new Vector2(x,y),c,Vector2.zero);vh.AddVert(new Vector2(x+w,y),c,Vector2.zero);vh.AddVert(new Vector2(x+w,y+h),c,Vector2.zero);vh.AddVert(new Vector2(x,y+h),c,Vector2.zero);vh.AddTriangle(n,n+1,n+2);vh.AddTriangle(n,n+2,n+3);}
 }
}
