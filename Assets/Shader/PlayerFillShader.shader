Shader "Fade/PlayerFillShader"
{
    Properties
    {
        _Color("Main Color", Color) = (1,1,1,1)
        _MainTex("Base (RGB) Trans (A)", 2D) = "white" {}
        _FadeMap("Fade Map", 2D) = "white" {}
        _Cutoff("Alpha cutoff", Range(0,1)) = 0.5
    }

        SubShader
        {
            Tags {"Queue" = "Transparent" "IgnoreProjector" = "True" "RenderType" = "Transparent"}
            LOD 300
            Cull Off
            CGPROGRAM
            #pragma surface surf Standard alpha:fade

            sampler2D _MainTex;
            sampler2D _FadeMap;
            float _Cutoff;
            fixed4 _Color;


            struct Input
            {
                float2 uv_MainTex;
            };

            void surf(Input IN, inout SurfaceOutputStandard o)
            {
                fixed4 diffuse = tex2D(_MainTex, IN.uv_MainTex) * _Color;
                fixed4 fadeSample = tex2D(_FadeMap, IN.uv_MainTex);


                bool cut = (fadeSample.r + fadeSample.g + fadeSample.b) / 3.0 < _Cutoff ? false : true;
                o.Albedo = cut ? diffuse.rgb : float3(1,1,1);
                o.Alpha = cut ? tex2D(_MainTex, IN.uv_MainTex).a : _Color.a;


            }
            ENDCG
        }

            FallBack "Transparent/Cutout/Diffuse"
}