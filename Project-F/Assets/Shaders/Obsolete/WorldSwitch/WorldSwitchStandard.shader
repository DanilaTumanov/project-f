Shader "WorldSwitch/Standard (Legacy)"{

	Properties{
		_Tint("Tint", Color) = (1, 1, 1, 1)
		_MainTex("Main Texture", 2D) = "white" {}
		[NoScaleOffset] _NormalMap("Normal map", 2D) = "bump" {}
		_BumpScale("Bump Scale", Range(0, 1)) = 1
		_Smoothness("Smoothness", Range(0, 1)) = 0.5
		[Gamma] _Metallic("Metallic", Range(0, 1)) = 0	
		_DetailTex("Detail Texture", 2D) = "gray" {}
		[NoScaleOffset] _DetailNormalMap("Detail Bump", 2D) = "bump" {}
		_DetailBumpScale("Detail Bump Scale", Range(0, 1)) = 1
		
		//_WorldSwitchOrigin ("World Switch Origin", Vector) = (0, 0, 0, 0)
		//_WorldSwitchRadius ("World Switch Radius", Range(0, 100)) = 0
		//[Toggle] _IsMainWorld ("Is Main World", FLoat) = 1
	}


	// Этот блок будет включен во все CGPROGRAM сабшэйдеров
	CGINCLUDE

	#define BINORMAL_PER_FRAGMENT

	ENDCG



	SubShader{

		
		Pass{

            Cull Off

			Tags {
				"LightMode" = "ForwardBase"
			}

			CGPROGRAM

			#pragma target 3.0

			#pragma multi_compile _ SHADOWS_SCREEN
			#pragma multi_compile _ VERTEXLIGHT_ON

			#pragma vertex vert
			#pragma fragment frag

			#define FORWARD_BASE_PASS

			//#include "UnityCG.cginc"
			//#include "UnityStandardBRDF.cginc"
			//#include "UnityStandardUtils.cginc"
			#include "UnityPBSLighting.cginc"

			#include "Includes/WorldSwitchLighting.cginc"

			ENDCG
		
		}

		Pass{

			Blend One One
			ZWrite Off
			Cull Off

			Tags {
				"LightMode" = "ForwardAdd"
			}

			CGPROGRAM

			#pragma target 3.0

			#pragma multi_compile_fwdadd_fullshadows

			#pragma vertex vert
			#pragma fragment frag

			#include "UnityPBSLighting.cginc"

			#include "Includes/WorldSwitchLighting.cginc"

			ENDCG

		}

		Pass{

			Tags {
				"LightMode" = "ShadowCaster"
			}

			CGPROGRAM

			#pragma target 3.0

			#pragma multi_compile_shadowcaster

			#pragma vertex shadows_vert
			#pragma fragment shadows_frag

			#include "Includes/Shadows.cginc"

			ENDCG

		}

	}

}