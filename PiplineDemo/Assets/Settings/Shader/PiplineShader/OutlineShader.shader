Shader "Unlit/OutlineShader"
{
    Properties
    {
        //_OutlineTexture ("Texture", 2D) = "white" {}
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
            };

            TEXTURE2D(_OutlineTexture);
            SAMPLER(sampler_OutlineTexture);
            float4 _OutlineTexture_ST;
            float4 _OutlineTexture_TexelSize;

            v2f vert (appdata v)
            {
                v2f o;
                VertexPositionInputs positionInputs = GetVertexPositionInputs(v.vertex.xyz);
                o.vertex = positionInputs.positionCS;
                o.uv = TRANSFORM_TEX(v.uv, _OutlineTexture);
                return o;
            }

            half4 frag (v2f i) : SV_Target
            {
                // // sample the texture
                // float2 dx = _OutlineTexture_TexelSize.x;
                // float2 dy = _OutlineTexture_TexelSize.y;

                // float4 color = SAMPLE_TEXTURE2D(_OutlineTexture,sampler_OutlineTexture, i.uv);

                // // 计算Sobel算子
                // float gx =  SAMPLE_TEXTURE2D(_OutlineTexture,sampler_OutlineTexture, i.uv + float2(-dx.x, -dy.x)).r * 2.0 +
                //             SAMPLE_TEXTURE2D(_OutlineTexture,sampler_OutlineTexture, i.uv + float2(-dx.x,  dy.x)).r * 1.0 +
                //             SAMPLE_TEXTURE2D(_OutlineTexture,sampler_OutlineTexture, i.uv + float2( dx.x, -dy.x)).r * (-2.0) +
                //             SAMPLE_TEXTURE2D(_OutlineTexture,sampler_OutlineTexture, i.uv + float2( dx.x,  dy.x)).r * (-1.0);

                // float gy =  SAMPLE_TEXTURE2D(_OutlineTexture,sampler_OutlineTexture, i.uv + float2(-dx.y, -dy.y)).r * 2.0 +
                //             SAMPLE_TEXTURE2D(_OutlineTexture,sampler_OutlineTexture, i.uv + float2(-dx.y,  dy.y)).r * 1.0 +
                //             SAMPLE_TEXTURE2D(_OutlineTexture,sampler_OutlineTexture, i.uv + float2( dx.y, -dy.y)).r * (-2.0) +
                //             SAMPLE_TEXTURE2D(_OutlineTexture,sampler_OutlineTexture, i.uv + float2( dx.y,  dy.y)).r * (-1.0);

                // // 计算边缘强度
                // float edgeStrength = sqrt(gx * gx + gy * gy);

                // // 设置边缘为纯白色，其他区域为原始颜色
                // float3 Albedo = edgeStrength > 0.5 ? float3(1, 1, 1) : 0;
                // float Alpha = color.a;
                // return float4(Albedo,Alpha);

                    float3 normal = UnpackNormal(SAMPLE_TEXTURE2D_X(_OutlineTexture,sampler_OutlineTexture, i.uv));

                // 获取相邻像素的法线值
                float3 normalRight = UnpackNormal(SAMPLE_TEXTURE2D_X(_OutlineTexture,sampler_OutlineTexture, i.uv + float2(1, 0) / _ScreenParams.x));
                float3 normalUp = UnpackNormal(SAMPLE_TEXTURE2D_X(_OutlineTexture,sampler_OutlineTexture, i.uv + float2(0, 1) / _ScreenParams.y));

                // 计算法线之间的角度差
                float angleDiffRight = dot(normal, normalRight);
                float angleDiffUp = dot(normal, normalUp);
                float _Threshold = 0.9;
                float Albedo;
                // 设置边缘颜色
                if (angleDiffRight < _Threshold || angleDiffUp < _Threshold)
                {
                    Albedo = 1;
                }
                else
                {
                    Albedo = 0;
                }
                return Albedo;
            }
            ENDHLSL
        }
    }
}
