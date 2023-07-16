Shader "Unlit/URPCopyDepth"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 100

        Pass
        {
            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag


            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            // #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
                float4 screenUV :TEXCOORD1;
            };

            TEXTURE2D(_CameraDepthTexture);
            SAMPLER(sampler_CameraDepthTexture);
            //float4 _MainTex_ST;

            v2f vert (appdata v)
            {
                v2f o;
                VertexPositionInputs positionInputs = GetVertexPositionInputs(v.vertex.xyz);
                o.vertex = positionInputs.positionCS;
                o.uv = v.uv;
                o.screenUV = ComputeScreenPos(o.vertex);
                return o;
            }

            half4 frag (v2f i) : SV_Target
            {
                // sample the texture
                float2 screenpos = i.screenUV.xy/i.screenUV.w;
                half4 col = SAMPLE_TEXTURE2D_X(_CameraDepthTexture,sampler_CameraDepthTexture,screenpos);
                float depthValue = Linear01Depth(col,_ZBufferParams);

                return depthValue;
            }
            ENDHLSL
        }
    }
}
