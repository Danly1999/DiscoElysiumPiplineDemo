Shader "Unlit/AllRender"
{
    Properties
    {
        //_MainTex ("Texture", 2D) = "white" {}
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 100
        ZWrite Off
        ZTest Always

        Pass
        {
            HLSLPROGRAM
            
            #pragma vertex vert
            #pragma fragment frag


            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            TEXTURE2D(_ShadowMaskTexture);
            SAMPLER(sampler_ShadowMaskTexture);
            TEXTURE2D(_BlitTexture);
            SAMPLER(sampler_BlitTexture);
            TEXTURE2D(_BaseColortexture);
            SAMPLER(sampler_BaseColortexture);
            float4 _ShadowColor;

            v2f vert (appdata v)
            {
                v2f o;
                VertexPositionInputs positionInputs = GetVertexPositionInputs(v.vertex.xyz);
                o.vertex = positionInputs.positionCS;
                o.uv = v.uv;
                return o;
            }

            half4 frag (v2f i) : SV_Target
            {
                Light light = GetMainLight();

                // sample the texture
                half shadow = SAMPLE_TEXTURE2D_X(_ShadowMaskTexture,sampler_ShadowMaskTexture, i.uv).r;
                half4 normalWS = SAMPLE_TEXTURE2D_X(_BlitTexture,sampler_BlitTexture, i.uv);
                half4 Basecol = SAMPLE_TEXTURE2D(_BaseColortexture,sampler_BaseColortexture, i.uv);
                normalWS = normalWS*2-1;
                half Lambert = clamp(dot(normalWS,light.direction),0,1);
                float4 colorShadow = lerp(_ShadowColor,1,Lambert*shadow);
                //half dark = max(Lambert*shadow,0.1);
                Basecol = float4((Basecol.rgb*light.color),1);

                return colorShadow*Basecol;
            }
            ENDHLSL
        }
    }
}
