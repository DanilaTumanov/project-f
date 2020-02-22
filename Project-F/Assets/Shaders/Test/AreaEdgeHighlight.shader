Shader "Custom/AreaEdgeHighlight"
{
    Properties
    {
        //_MainTex ("Texture", 2D) = "white" {}
        [HDR]_Tint ("Tint", Color) = (1,0,0,1)
        _Thikness ("Thickness", Range(0, 0.05)) = 0.01
        _GradientPower ("Gradient Power", Range(0, 0.5)) = 0.1
    }
    SubShader
    {
        Tags { "RenderType"="Transparent" "Queue"="Transparent" }
        LOD 100

        Pass
        {
        
            Blend SrcAlpha OneMinusSrcAlpha
            Cull Off
            ZWrite Off
        
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
                float4 screenPosUV : TEXCOORD1;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;
            
            //the depth texture
            sampler2D _CameraDepthTexture;
            
            fixed4 _Tint;
            fixed _Thikness;
            fixed _GradientPower;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                o.screenPosUV = ComputeScreenPos(o.vertex);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                // sample the texture
                float depth = tex2D(_CameraDepthTexture, i.screenPosUV.xy / i.screenPosUV.w).r;
                depth = Linear01Depth(depth);
                float figureDepth = Linear01Depth(i.vertex.z);
                
                fixed depthDiff = abs(depth - figureDepth);
                fixed4 col = depthDiff < _Thikness ? fixed4(_Tint.rgb, 1 - pow(depthDiff / _Thikness, _GradientPower)) : 0;
                
                /*col = figureDepth;
                col.a = 1;*/
                
                return col;
            }
            ENDCG
        }
    }
}
