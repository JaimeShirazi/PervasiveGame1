Shader "Custom/Box"
{
    Properties
    {
        [MainTexture] _BaseTex("Base Texture", 2D) = "white" {}
        _AO_Open("AO Open", 2D) = "white" {}
        _AO_Closed("AO Closed", 2D) = "white" {}
        _AO_Blend("AO Blend", Float) = 0
    }
    SubShader
    {
        Tags
        {
            "RenderType" = "Opaque"
            "IgnoreProjector" = "True"
            "UniversalMaterialType" = "Unlit"
            "RenderPipeline" = "UniversalPipeline"
        }

        Pass
        {
            Cull Back
		    Blend Off
		    ZWrite On
            ZTest LEqual

            HLSLPROGRAM

            #pragma vertex vert
            #pragma fragment frag

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            CBUFFER_START(UnityPerMaterial)
                float _AO_Blend;
                sampler2D _BaseTex;
                sampler2D _AO_Open;
                sampler2D _AO_Closed;
            CBUFFER_END

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float4 vertex : SV_POSITION;
                float2 uv : TEXCOORD0;
            };
            
            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = TransformObjectToHClip(v.vertex);
                o.uv = v.uv;
                return o;
            }

            half3 frag (v2f i) : SV_Target
            {
                float ao = lerp(tex2D(_AO_Closed, i.uv).r, tex2D(_AO_Open, i.uv).r, _AO_Blend);
                return tex2D(_BaseTex, i.uv) * ao;
            }

            ENDHLSL
        }
    }
}