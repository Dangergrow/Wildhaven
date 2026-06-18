Shader "Wildhaven/Block"
{
    Properties
    {
        _BaseMap("Texture", 2D) = "white" {}
        _BaseColor("Color", Color) = (1,1,1,1)
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" "RenderPipeline"="UniversalPipeline" }
        Pass
        {
            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            TEXTURE2D(_BaseMap); SAMPLER(sampler_BaseMap);
            float4 _BaseMap_ST;
            float4 _BaseColor;

            struct Attributes { float4 pos : POSITION; float3 nrm : NORMAL; float2 uv : TEXCOORD0; };
            struct Varyings { float4 hcs : SV_POSITION; float3 nrm : TEXCOORD0; float2 uv : TEXCOORD1; };

            Varyings vert(Attributes i)
            {
                Varyings o;
                o.hcs = TransformObjectToHClip(i.pos.xyz);
                o.nrm = TransformObjectToWorldNormal(i.nrm);
                o.uv = i.uv * _BaseMap_ST.xy + _BaseMap_ST.zw;
                return o;
            }

            half4 frag(Varyings i) : SV_Target
            {
                float3 n = normalize(i.nrm);
                // Light from top-front-right
                float NdotL = dot(n, normalize(float3(0.4, 0.9, 0.3)));
                float shade = NdotL * 0.7 + 0.3; // ambient 0.3, diffuse 0.7
                half4 tex = SAMPLE_TEXTURE2D(_BaseMap, sampler_BaseMap, i.uv);
                return half4(tex.rgb * _BaseColor.rgb * shade, 1);
            }
            ENDHLSL
        }
    }
}
