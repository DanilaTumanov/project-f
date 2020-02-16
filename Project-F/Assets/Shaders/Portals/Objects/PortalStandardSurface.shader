Shader "Portal/StandardSurface"
{
    Properties
    {
        _Color ("Color", Color) = (1,1,1,1)
        _MainTex ("Albedo (RGB)", 2D) = "white" {}
        _Glossiness ("Smoothness", Range(0,1)) = 0.5
        _Metallic ("Metallic", Range(0,1)) = 0.0    
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 200

        CGPROGRAM

        // Physically based Standard lighting model, and enable shadows on all light types
        #pragma surface surf Standard fullforwardshadows
        #pragma multi_compile PORTAL1 PORTAL2 PORTAL3 PORTAL4 PORTAL5

        // Use shader model 3.0 target, to get nicer looking lighting
        #pragma target 3.0

        sampler2D _MainTex;

        struct Input
        {
            float2 uv_MainTex;
            float3 worldPos;
        };

        half _Glossiness;
        half _Metallic;
        fixed4 _Color;

        float4 _PortalsOrigin[5];
        float _PortalsRadius[5];
        
        
        float _IsMainWorld;


        // Add instancing support for this shader. You need to check 'Enable Instancing' on materials that use the shader.
        // See https://docs.unity3d.com/Manual/GPUInstancing.html for more information about instancing.
        // #pragma instancing_options assumeuniformscaling
        UNITY_INSTANCING_BUFFER_START(Props)
            // put more per-instance properties here
        UNITY_INSTANCING_BUFFER_END(Props)
        
        
        
        fixed WorldSwitchVisible(float3 surfWorldPos, float3 portalsOrigin, fixed portalRadius, fixed isMainWorld, fixed visibleInAnotherPortal)
        {
            float isVisibleInMain = length(surfWorldPos.xz - portalsOrigin.xz) > portalRadius;
            
            /*visibleInMain isMain visible
		                0		0		0		1
                        0		0		1		1	
                        0		1		0		0
                        0		1		1		0
                        1		0		0		0
                        1		0		1		1	
                        1		1		0		0
                        1		1		1		1		
	
                        vim ? vis : !im*/
            
            return isVisibleInMain ? visibleInAnotherPortal : !isMainWorld;
        }
        
        
        #define GET_VISIBILITY_IN_PORTAL(portal) visible = WorldSwitchVisible(IN.worldPos, _PortalsOrigin[(portal)].xyz, _PortalsRadius[(portal)], _IsMainWorld, visible);
        

        void surf (Input IN, inout SurfaceOutputStandard o)
        {
        
            fixed visible = _IsMainWorld;
        
            #if defined(PORTAL1)
                GET_VISIBILITY_IN_PORTAL(0);
            #endif
        
            #if defined(PORTAL2)
                GET_VISIBILITY_IN_PORTAL(0);
                GET_VISIBILITY_IN_PORTAL(1);
            #endif
        
            #if defined(PORTAL3)
                GET_VISIBILITY_IN_PORTAL(0);
                GET_VISIBILITY_IN_PORTAL(1);
                GET_VISIBILITY_IN_PORTAL(2);
            #endif
            
            #if defined(PORTAL4)
                GET_VISIBILITY_IN_PORTAL(0);
                GET_VISIBILITY_IN_PORTAL(1);
                GET_VISIBILITY_IN_PORTAL(2);
                GET_VISIBILITY_IN_PORTAL(3);
            #endif
            
            #if defined(PORTAL5)
                GET_VISIBILITY_IN_PORTAL(0);
                GET_VISIBILITY_IN_PORTAL(1);
                GET_VISIBILITY_IN_PORTAL(2);
                GET_VISIBILITY_IN_PORTAL(3);
                GET_VISIBILITY_IN_PORTAL(4);
            #endif
            
        
            clip(lerp(-1, 1, visible));
        
            // Albedo comes from a texture tinted by color
            fixed4 c = tex2D (_MainTex, IN.uv_MainTex) * _Color;
            o.Albedo = c.rgb;
            // Metallic and smoothness come from slider variables
            o.Metallic = _Metallic;
            o.Smoothness = _Glossiness;
            o.Alpha = c.a;
        }
        ENDCG
    }
    //FallBack "Diffuse"
}
