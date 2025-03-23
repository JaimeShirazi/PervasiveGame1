Shader "Skybox/Simple Sky"
{
    SubShader
    {
        Pass
        {
            Tags { "Queue"="Background" "RenderType"="Background" "PreviewType"="Skybox" }
            Cull Off ZWrite Off ZTest LEqual

            HLSLPROGRAM

            #pragma vertex vert
            #pragma fragment frag

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            struct appdata
            {
                float4 vertex : POSITION;
            };

            struct v2f
            {
                float4 vertex : SV_POSITION;
            };
            
            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = TransformObjectToHClip(v.vertex);
                return o;
            }

            half3 frag (v2f i) : SV_Target
            {
                return (0.015).xxx;
            }

            ENDHLSL
        }
    }
}