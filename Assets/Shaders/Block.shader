Shader "Wildhaven/Block"
{
    Properties
    {
        _BaseColor("Color", Color) = (1,1,1,1)
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" "RenderPipeline"="UniversalPipeline" }
        Cull Off
        Pass
        {
            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            float4 _BaseColor;

            struct Attributes { float4 pos : POSITION; float3 nrm : NORMAL; };
            struct Varyings { float4 hcs : SV_POSITION; float3 nrm : TEXCOORD0; };

            Varyings vert(Attributes i)
            {
                Varyings o;
                o.hcs = TransformObjectToHClip(i.pos.xyz);
                o.nrm = TransformObjectToWorldNormal(i.nrm);
                return o;
            }

            half4 frag(Varyings i) : SV_Target
            {
                // Simple diffuse: light from above-right-front
                float3 lightDir = normalize(float3(0.5, 1.0, 0.5));
                float NdotL = max(0.4, dot(normalize(i.nrm), lightDir));
                return half4(_BaseColor.rgb * NdotL, 1);
            }
            ENDHLSL
        }
    }
}
