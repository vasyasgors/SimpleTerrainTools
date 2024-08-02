Shader "Custom/Terrain"
{
    Properties
    {
        _Color_1 ("Color 1", Color) = (1,1,1,1)
        _Color_2 ("Color 2 ", Color) = (1,1,1,1)

        _LimitAngle ("Transition angle to Color 2", Range(0, 90)) = 0
        
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

        // Use shader model 3.0 target, to get nicer looking lighting
        #pragma target 3.0

        sampler2D _MainTex;

        struct Input
        {
            float2 uv_MainTex;
            float3 worldNormal;
        };

        half _Glossiness;
        half _Metallic;
        fixed4 _Color_1;
        fixed4 _Color_2;
        float _LimitAngle;

       

        void surf (Input IN, inout SurfaceOutputStandard o)
        {
            float dotProduct = dot(IN.worldNormal, float3(0, 1, 0));

            float angle = cos((_LimitAngle * 3.14159265359 ) / 180.0 );

            float3 lerpFactor = smoothstep(angle, angle + 0.001, dotProduct);
            o.Albedo = lerp(_Color_1, _Color_2, 1 - lerpFactor);


            // Metallic and smoothness come from slider variables
            o.Metallic = _Metallic;
            o.Smoothness = _Glossiness;
        }
        ENDCG
    }
    FallBack "Diffuse"
}
