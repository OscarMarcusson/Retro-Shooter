Shader "Custom/StandardGround"
{
    Properties
    {
        _MainTex("Albedo (RGB)", 2D) = "white" {}
        _Resolution("Pixel Resolution", int) = 256
    }
        SubShader
    {
        Tags { "RenderType" = "Opaque" }
        LOD 200

        CGPROGRAM
        #pragma surface surf StandardSpecular fullforwardshadows
        #pragma target 3.0

        sampler2D _MainTex;

        struct Input
        {
            float3 worldPos;
        };


        UNITY_INSTANCING_BUFFER_START(Props)
        UNITY_INSTANCING_BUFFER_END(Props)

        fixed _Resolution;

        void surf(Input IN, inout SurfaceOutputStandardSpecular o)
        {
            // Force 64 pixels per meter, xz projection
            fixed4 c = tex2D(_MainTex, (IN.worldPos.xz / _Resolution) * 64.0);
            o.Albedo = c.rgb * 0.5;
            o.Specular = c.rgb;
            o.Smoothness = c.a;
            o.Alpha = 1.0;
        }
        ENDCG
    }
        FallBack "Diffuse"
}
