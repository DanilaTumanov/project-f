#if !defined(WORLD_SWITCH_LIGHTING_INCLUDED)
#define WORLD_SWITCH_LIGHTING_INCLUDED

	#include "UnityPBSLighting.cginc"
	#include "AutoLight.cginc"

	struct vertexData {
		float4 vertex : POSITION;
		float3 normal : NORMAL;
		float2 uv : TEXCOORD0;
		float4 tangent : TANGENT;
	};

	struct interpolators {
		float4 pos : SV_POSITION;
		float4 uv : TEXCOORD0;
		float3 normal : TEXCOORD1;

		#if defined(BITANGENT_PER_FRAGMENT)
			float4 tangent : TEXCOORD2;
		#else
			float3 tangent : TEXCOORD2;
			float3 bitangent : TEXCOORD3;
		#endif
		
		float3 worldPos : TEXCOORD4;


		SHADOW_COORDS(5)

		#if defined(VERTEXLIGHT_ON)
			float3 vertexLightColor : TEXCOORD6;
		#endif
	};

	sampler2D _MainTex;
	float4 _MainTex_ST;
	sampler2D _DetailTex;
	float4 _DetailTex_ST;
	sampler2D _NormalMap;
	sampler2D _DetailNormalMap;

	float4 _Tint;
	float _Metallic;
	float _Smoothness;
	float _BumpScale;
	float _DetailBumpScale;


    float3 _WorldSwitchOrigin;
    float _WorldSwitchRadius;
    float _IsMainWorld;



	UnityLight CreateLight(interpolators i) {
		UnityLight light;
		float3 lightVec = _WorldSpaceLightPos0.xyz - i.worldPos;

		//#if defined(SHADOWS_SCREEN)
		//	float atten = SHADOW_ATTENUATION(i);
		//	/*tex2D(
		//		_ShadowMapTexture, 
		//		i.shadowCoordinates.xy / i.pos.w
		//	);*/
		//#else
			UNITY_LIGHT_ATTENUATION(atten, i, i.worldPos);
		//#endif

		#if defined(POINT) || defined(POINT_COOKIE) || defined(SPOT)
			light.dir = normalize(lightVec);
		#else
			light.dir = _WorldSpaceLightPos0.xyz;
		#endif

		light.color = _LightColor0.rgb * atten;
		light.ndotl = DotClamped(normalize(i.normal), light.dir);

		return light;
	}



	void ComputeVertexLightColor(inout interpolators i) {
		#if defined(VERTEXLIGHT_ON)
			i.vertexLightColor = Shade4PointLights(
				unity_4LightPosX0, unity_4LightPosY0, unity_4LightPosZ0,
				unity_LightColor[0].rgb, unity_LightColor[1].rgb,
				unity_LightColor[2].rgb, unity_LightColor[3].rgb,
				unity_4LightAtten0, i.worldPos, i.normal
			);

		#endif
	}


	UnityIndirect CreateIndirectLight(interpolators i) {
		UnityIndirect indirectLight;
		indirectLight.diffuse = 0;
		indirectLight.specular = 0;

		#if defined(VERTEXLIGHT_ON)
			indirectLight.diffuse = i.vertexLightColor;
		#endif

		#if defined(FORWARD_BASE_PASS)
			indirectLight.diffuse += max(0, ShadeSH9(float4(i.normal, 1)));
		#endif

		return indirectLight;
	}



	float3 CreateBitangent(float3 normal, float3 tangent, float bitangentSign) {
		return cross(normal, tangent.xyz) * (bitangentSign * unity_WorldTransformParams.w);
	}



	void InitFragNormals(inout interpolators i) {

		float3 mainNormal = UnpackScaleNormal(tex2D(_NormalMap, i.uv.xy), _BumpScale);
		float3 detailNormal = UnpackScaleNormal(tex2D(_DetailNormalMap, i.uv.zw), _DetailBumpScale);
		// WHITEOUT BLENDING
		float3 tangentSpaceNormal = BlendNormals(mainNormal, detailNormal);
		
		float3 bitangent;
		#if defined(BITANGENT_PER_FRAGMENT)
			bitangent = CreateBitangent(i.normal, i.tangent.xyz, i.tangent.w);
		#else
			bitangent = i.bitangent;
		#endif

		i.normal = normalize(
			tangentSpaceNormal.x * i.tangent +
			tangentSpaceNormal.y * bitangent +
			tangentSpaceNormal.z * i.normal
		);
	}





	interpolators vert(vertexData v) {
		interpolators i;

		i.pos = UnityObjectToClipPos(v.vertex);
		i.uv.xy = TRANSFORM_TEX(v.uv, _MainTex);
		i.uv.zw = TRANSFORM_TEX(v.uv, _DetailTex);
		i.normal = UnityObjectToWorldNormal(v.normal);

		#if defined(BITANGENT_PER_FRAGMENT)
			i.tangent = float4(UnityObjectToWorldDir(v.tangent.rgb), v.tangent.w);
		#else
			i.tangent = UnityObjectToWorldDir(v.tangent.rgb);
			i.bitangent = CreateBitangent(i.normal, i.tangent, v.tangent.w);
		#endif
		
		i.worldPos = mul(unity_ObjectToWorld, v.vertex);

		//#if defined(SHADOWS_SCREEN)
		//	i.shadowCoordinates = ComputeScreenPos(i.pos);

		//	/*float2 shadowPos = i.pos.xy;
		//	#if UNITY_UV_STARTS_AT_TOP	
		//		shadowPos.y *= -1;
		//	#endif
		//	i.shadowCoordinates.xy = (shadowPos + i.pos.w) * 0.5;
		//	i.shadowCoordinates.zw = i.pos.zw;*/
		//#endif

		TRANSFER_SHADOW(i);

		ComputeVertexLightColor(i);

		return i;
	}

	float4 frag(interpolators i) : SV_TARGET{

		InitFragNormals(i);

        float isVisibleInMain = length(i.worldPos.xz - _WorldSwitchOrigin.xz) > _WorldSwitchRadius;
        
        clip(lerp(-1, 1, isVisibleInMain) * lerp(-1, 1, _IsMainWorld));

		float3 viewDir = normalize(_WorldSpaceCameraPos - i.worldPos);
		float3 albedo = tex2D(_MainTex, i.uv.xy).rgb * _Tint.rgb;
		albedo *= tex2D(_DetailTex, i.uv.zw) * unity_ColorSpaceDouble;

		float3 specularTint;
		float oneMinusReflectivity;

		albedo = DiffuseAndSpecularFromMetallic(
			albedo, _Metallic, specularTint, oneMinusReflectivity
		);

        fixed facing = dot(viewDir, i.normal);

		return /*facing < -0.1 ? float4(1, 0, 0, 0) :*/ UNITY_BRDF_PBS(
			albedo, specularTint,
			oneMinusReflectivity, _Smoothness,
			i.normal, viewDir,
			CreateLight(i), CreateIndirectLight(i)
		);
	}

#endif