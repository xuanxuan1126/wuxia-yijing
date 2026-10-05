Shader "Wuxia/JadeAdditive" {
 Properties {
  [PerRendererData] _MainTex ("Sprite Texture", 2D) = "white" {}
  _Color ("Tint", Color) = (1,1,1,1)
  [PerRendererData] _RendererColor ("Renderer Color", Color) = (1,1,1,1)
  [PerRendererData] _Flip ("Flip", Vector) = (1,1,1,1)
  _SoftGlow ("Soft radial light", Float) = 0
 }
 SubShader {
  Tags { "Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent" "CanUseSpriteAtlas"="True" }
  Cull Off Lighting Off ZWrite Off Blend SrcAlpha One
  Pass {
   CGPROGRAM
   #pragma vertex SpriteVert
   #pragma fragment frag
   #pragma multi_compile_instancing
   #include "UnitySprites.cginc"
   float _SoftGlow;
   fixed4 frag(v2f IN) : SV_Target {
    fixed4 c = SampleSpriteTexture(IN.texcoord) * IN.color;
    if (_SoftGlow > .5) {
     float r = length(IN.texcoord * 2 - 1);
     c.a *= saturate(1-r) * exp(-r*r*5);
    }
    return c;
   }
   ENDCG
  }
 }
}
