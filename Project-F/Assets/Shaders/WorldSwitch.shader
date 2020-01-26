Shader "Unlit/WorldSwitch"
{
    Properties
    {
        [HideInInspector] _MainTex ("First world", 2D) = "white" {}
        [HideInInspector] _SecondWorld ("Second world", 2D) = "white" {}
        
        [HideInInspector] _SecondWorldDepth ("Second world depth", 2D) = "black" {}
        
        _OriginViewPos ("Origin view pos", Vector) = (0,0,0,0)
        
        _Radius ("Radius", Range(0, 50)) = 0
        
        [Toggle]_Debug ("Debug", Float) = 0
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 100

        Pass
        {
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
            };


            sampler2D _MainTex;
            float4 _MainTex_ST;
            
            //the depth texture
            sampler2D _CameraDepthTexture;
            
            sampler2D _SecondWorld;
            sampler2D _SecondWorldDepth;
            
            float4x4 _InverseRotationMatrix;
            
            fixed _Radius;
            float4 _OriginViewPos;
            
            
            
            // DEBUG
            fixed _Debug;
            
            

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {                
                float depthUnits1 = (1 - tex2D(_CameraDepthTexture, i.uv).r) * _ProjectionParams.z;
                float depthUnits2 = (1 - tex2D(_SecondWorldDepth, i.uv).r) * _ProjectionParams.z;

                fixed firstIsClosest = depthUnits1 < depthUnits2;

                float viewSpaceUnitsX = unity_OrthoParams.x * 2 * (i.uv.x - 0.5);
                float viewSpaceUnitsY = unity_OrthoParams.y * 2 * (i.uv.y- 0.5);

                float3 viewSpacePoint1 = float3(viewSpaceUnitsX, viewSpaceUnitsY, depthUnits1) - _OriginViewPos;
                float3 viewSpacePoint2 = float3(viewSpaceUnitsX, viewSpaceUnitsY, depthUnits2) - _OriginViewPos;
                float3 worldSpacePoint1 = mul(_InverseRotationMatrix, viewSpacePoint1);
                float3 worldSpacePoint2 = mul(_InverseRotationMatrix, viewSpacePoint2);

                fixed4 firstWorld = tex2D(_MainTex, i.uv);
                fixed4 secondWorld = tex2D(_SecondWorld, i.uv);

                fixed firstIsFirst = length(worldSpacePoint1.xz) >= _Radius;
                fixed secondIsSecond = length(worldSpacePoint2.xz) < _Radius;
                
                fixed4 col;
                
                col = firstIsClosest && firstIsFirst 
                    ? firstWorld
                    : !firstIsClosest && secondIsSecond 
                        ? secondWorld 
                        : firstIsClosest && !firstIsFirst
                            ? secondWorld
                            : firstWorld;
                
                
                //col = isFirstWorld /*&& (depthUnits1 >= depthUnits2)*/ ? firstWorld : secondWorld;
                //col = isSecondWorld /*&& (depthUnits2 >= depthUnits1)*/ ? secondWorld : col;

                col = _Debug ? unity_OrthoParams.y / _Radius : col;
                //col = depthUnits2;
                //col = Linear01Depth(tex2D(_SecondWorldDepth, i.uv).r);
                //col = tex2D(_SecondWorldDepth, i.uv).r;
                //float4 col = tex2D(_SecondWorld, i.uv);
                
                //col = 1;
                
                return col;
            }
            ENDCG
        }
    }
}
