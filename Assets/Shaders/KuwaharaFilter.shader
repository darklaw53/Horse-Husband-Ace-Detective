Shader "Custom/KuwaharaFilter"
{
    Properties
    {
        _Radius ("Radius", Int) = 4
    }

    SubShader
    {
        Tags { "RenderType"="Opaque" "RenderPipeline"="UniversalPipeline"}

        Pass
        {
            Name "Kuwahara"

            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            TEXTURE2D(_CameraOpaqueTexture);
            SAMPLER(sampler_CameraOpaqueTexture);

            int _Radius;

            struct Attributes
            {
                float4 positionOS : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct Varyings
            {
                float4 positionHCS : SV_POSITION;
                float2 uv : TEXCOORD0;
            };

            Varyings vert (Attributes v)
            {
                Varyings o;
                o.positionHCS = TransformObjectToHClip(v.positionOS.xyz);
                o.uv = v.uv;
                return o;
            }

            float4 frag (Varyings i) : SV_Target
            {
                float2 texel = 1.0 / _ScreenParams.xy;

                float3 mean[4];
                float3 var[4];
                float weight[4];

                for(int k=0;k<4;k++)
                {
                    mean[k]=0;
                    var[k]=0;
                    weight[k]=0;
                }

                for(int x=-_Radius; x<=_Radius; x++)
                {
                    for(int y=-_Radius; y<=_Radius; y++)
                    {
                        float2 offset=float2(x,y);
                        float2 uv=i.uv + offset*texel;

                        float3 c=SAMPLE_TEXTURE2D(_CameraOpaqueTexture,
                        sampler_CameraOpaqueTexture,uv).rgb;

                        float d2=dot(offset,offset);
                        float r2=_Radius*_Radius;

                        float w=pow(saturate(1-d2/r2),2);

                        int region=0;

                        if(x<=0 && y<=0) region=0;
                        else if(x>=0 && y<=0) region=1;
                        else if(x<=0 && y>=0) region=2;
                        else region=3;

                        mean[region]+=c*w;
                        var[region]+=c*c*w;
                        weight[region]+=w;
                    }
                }

                float bestVar=999999;
                float3 bestColor=0;

                for(int k=0;k<4;k++)
                {
                    float3 m=mean[k]/weight[k];
                    float3 v=abs(var[k]/weight[k]-m*m);
                    float variance=v.r+v.g+v.b;

                    if(variance<bestVar)
                    {
                        bestVar=variance;
                        bestColor=m;
                    }
                }

                return float4(bestColor,1);
            }

            ENDHLSL
        }
    }
}