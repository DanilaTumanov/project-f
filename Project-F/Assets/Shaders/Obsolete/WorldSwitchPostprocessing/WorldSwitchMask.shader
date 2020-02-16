Shader "Unlit/WorldSwitchMask"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 100
    
        
        Pass
        {
            Cull Off
        
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            
            #pragma multi_compile __ SECOND_WORLD_RENDER

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
                float3 normal : NORMAL;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
                float4 worldPos : TEXCOORD1;
                float3 normal : NORMAL;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;
            
            float4 _WorldSwitchOrigin;
            float _WorldSwitchRadius;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.worldPos = mul(unity_ObjectToWorld, v.vertex);
                o.normal = v.normal;
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                // sample the texture
                fixed4 col = tex2D(_MainTex, i.uv);
                
                //fixed facing = i.normal 
                
                fixed4 show = fixed4(1, 0, 0, 1);
                fixed4 hide = fixed4(0, 1, 0, 1);
                
                #ifdef SECOND_WORLD_RENDER
                    col = lerp(hide, show, length(i.worldPos.xz - _WorldSwitchOrigin.xz) < _WorldSwitchRadius);
                #else
                    col = lerp(hide, show, length(i.worldPos.xz - _WorldSwitchOrigin.xz) >= _WorldSwitchRadius);
                #endif
                
                return col;
            }
            ENDCG
        }
    }
}
