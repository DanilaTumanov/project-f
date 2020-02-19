#if !defined(MY_SHADOWS_INCLUDED)
#define MY_SHADOWS_INCLUDED

	#include "UnityCG.cginc"

    float3 _WorldSwitchOrigin;
    float _WorldSwitchRadius;
    float _IsMainWorld;


	struct vertexData 
	{
		float4 position : POSITION;
		float3 normal : NORMAL;
	};

	#if defined(SHADOWS_CUBE)

		struct interpolators {
			float4 position : SV_POSITION;
			float3 lightVec : TEXCOORD0;
			float3 worldPos : TEXCOORD1;
		};

		interpolators shadows_vert(vertexData v){
			interpolators i;

			i.position = UnityObjectToClipPos(v.position);
			i.worldPos = mul(unity_ObjectToWorld, v.position);
			i.lightVec = i.worldPos.xyz - _LightPositionRange.xyz;
			
			return i;
		}

		half4 shadows_frag(interpolators i) : SV_TARGET{
			float depth = length(i.lightVec) + unity_LightShadowBias.x;
			depth *= _LightPositionRange.w;
			
			float isVisible = ((length(i.worldPos.xz - _WorldSwitchOrigin.xz) > _WorldSwitchRadius) - 0.5) * lerp(-1, 1, _IsMainWorld) + 0.5;
			
			return lerp(0, UnityEncodeCubeShadowDepth(depth), isVisible);
		}

	#else

		float4 shadows_vert(vertexData v) : SV_POSITION{
			float4 position = UnityClipSpaceShadowCasterPos(v.position.xyz, v.normal);
			
			float4 worldPos = mul(unity_ObjectToWorld, v.position);
            float isVisible = lerp(!_IsMainWorld, _IsMainWorld, length(worldPos.xz - _WorldSwitchOrigin.xz) > _WorldSwitchRadius);
			
			return lerp(0, UnityApplyLinearShadowBias(position), isVisible);
		}

		half4 shadows_frag() : SV_TARGET{
			return 0;
		}

	#endif

#endif