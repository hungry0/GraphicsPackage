Shader "SSS/CreateLUT"
{
    Properties
    {
    }

    SubShader
    {
        Tags { "RenderType"="Opaque" }

        Ztest Off
        Cull Off

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

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

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = v.vertex;
                o.uv = v.uv;
                return o;
            }

            static const float VList[6] = {0.0064, 0.0484, 0.187, 0.567, 1.99, 7.41};

            static const float3 ColorList[6] =
            {
                float3(0.233, 0.455, 0.649),
                float3(0.1, 0.366, 0.344),
                float3(0.118, 0.198, 0),
                float3(0.113, 0.007, 0.007),
                float3(0.358, 0.004, 0),
                float3(0.078, 0, 0)
            };

            float3 CreatePreIntegratedSkinBRDF(float2 uv, float offset, float radius)
            {
                float PI = 3.14159265359;
                uv.x = 1 - uv.x;
                float Theta = (offset - uv.x) * PI;

                float3 A = 0;
                float3 B = 0;
                float x = -PI / 2;

                for (int i = 0; i < 1000; i++)
                {
                    float step = 0.001;
              
                    float dis = abs(2 * (1 / (1 - uv.y) * radius) * sin(x * 0.5));
                    float3 Guss0 = exp(-dis * dis / (2 * VList[0])) * ColorList[0];
                    float3 Guss1 = exp(-dis * dis / (2 * VList[1])) * ColorList[1];
                    float3 Guss2 = exp(-dis * dis / (2 * VList[2])) * ColorList[2];
                    float3 Guss3 = exp(-dis * dis / (2 * VList[3])) * ColorList[3];
                    float3 Guss4 = exp(-dis * dis / (2 * VList[4])) * ColorList[4];
                    float3 Guss5 = exp(-dis * dis / (2 * VList[5])) * ColorList[5];
                    float3 D = Guss0 + Guss1 + Guss2 + Guss3 + Guss4 + Guss5;

                    A += saturate(cos(x + Theta)) * D;
                    B += D;
                    x += 0.01;

                    if (x == (PI / 2))
                    {
                        break;
                    }
                }

                float3 result = A / B;

                return result;
            }

            float4 frag (v2f i) : SV_Target
            {
                float3 col = CreatePreIntegratedSkinBRDF(i.uv, 0, 1);

                return float4(col, 1.0);
            }
            ENDCG
        }
    }
}
