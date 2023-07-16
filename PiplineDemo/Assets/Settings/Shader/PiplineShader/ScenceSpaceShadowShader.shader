Shader "URP/Unlit/DeferredDrawShader"
{
    HLSLINCLUDE
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Shadows.hlsl"



            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
                float3 normal : NORMAL;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 pos : SV_POSITION;
                float fogCoord : TEXCOORD1;
                float3 posWorld : TEXCOORD2;
                float3 normalDir : TEXCOORD3;
                float4 scrPos : TEXCOORD4;
            };

            TEXTURE2D_X(_CameraDepthTexture);

            SAMPLER(sampler_CameraDepthTexture );

            float4x4 _ScreenToWorld;

            v2f vert (appdata v)
            {
                v2f o;
                VertexPositionInputs vertexInput = GetVertexPositionInputs(v.vertex.xyz);
                VertexNormalInputs NormalInput = GetVertexNormalInputs(v.normal);
                o.pos = vertexInput.positionCS;
                o.posWorld = vertexInput.positionWS;
                o.normalDir = NormalInput.normalWS;
                o.scrPos = ComputeScreenPos(vertexInput.positionCS);
                o.uv = v.uv;
                return o;
            }
            half MyMainLightRealtimeShadow(float4 shadowCoord)
            {
            // #if !defined(MAIN_LIGHT_CALCULATE_SHADOWS)
            //     return 1.0h;
            // #endif

                ShadowSamplingData shadowSamplingData = GetMainLightShadowSamplingData();
                half4 shadowParams = GetMainLightShadowParams();
                return SampleShadowmap(TEXTURE2D_ARGS(_MainLightShadowmapTexture, sampler_MainLightShadowmapTexture), shadowCoord, shadowSamplingData, shadowParams, false);
            }
            float packColor(float3 color) {   return (color.r + (color.g*256.) + (color.b*256.*256.)) / (256.*256.); }

            float3 decodeColor(float f) 
            { 
            float b = floor(f * 256.0);
            float g = floor(f * 65536.0) - (b*256.);
            float r = (floor(f * 16777216.0) - (b*65536.)) - (g*256.);
            return float3(r,g,b)/ 256.0;//vec3(r,b) / 256.0;  
            }
            float4 frag (v2f i) : SV_TARGET
            {
                Light mainLight;
                mainLight= GetMainLight();
                
                float Depth = SAMPLE_TEXTURE2D(_CameraDepthTexture,sampler_CameraDepthTexture, i.uv).r;

                //World Space Postion
                float4 posWS = mul(_ScreenToWorld, float4(i.pos.xy, Depth, 1.0));
                posWS.xyz *= rcp(posWS.w);

                float4 shadowCoord = TransformWorldToShadowCoord(posWS.xyz);
                float shadowAttenuation = MyMainLightRealtimeShadow(shadowCoord);
                // if(shadowAttenuation>shadowCoord.z)
                // {
                //     shadowAttenuation = 0;
                // }else
                // {
                //     shadowAttenuation = 1;
                // }

                shadowAttenuation = ApplyShadowFade(shadowAttenuation, posWS.xyz);
                return shadowAttenuation;

            }

    ENDHLSL

    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 100

        Pass
        {
            ZTest Always
            ZWrite Off
            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag


            ENDHLSL
        }


    }
}
