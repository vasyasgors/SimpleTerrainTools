Shader "Custom/TerrainTexture"
{
    Properties
    {
         _MainTex ("Texture", 2D) = "white" {}

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


       

        void surf (Input IN, inout SurfaceOutputStandard o)
        {
            float dotProduct = dot(IN.worldNormal, float3(0, 1, 0));
            //dotProduct = clamp((dotProduct + 1) / 2, 0, 1);
            dotProduct = (dotProduct + 1) / 2;

            fixed4 col = tex2D(_MainTex, fixed2(dotProduct, 0.5f));
            o.Albedo = col;

            //fixed4 col = tex2D(_MainTex, float2(0.01f, 0.01f));
            //o.Albedo = col;


            // Metallic and smoothness come from slider variables
            o.Metallic = _Metallic;
            o.Smoothness = _Glossiness;
        }
        ENDCG
    }
    FallBack "Diffuse"
}
