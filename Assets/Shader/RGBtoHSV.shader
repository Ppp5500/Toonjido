Shader "Custom/AdjustSaturation"
{
    Properties
    {
        _MainTex("Texture", 2D) = "white" {}
        _Parameter("Saturation Parameter", Range(0, 2)) = 1
    }
        SubShader
        {
            // No culling or depth
            Cull Off ZWrite Off ZTest Always

            Pass
            {
                HLSLPROGRAM
                #pragma vertex vert
                #pragma fragment frag

                // make fog work
                #include "UnityCG.cginc"

                struct appdata
                {
                    float4 vertex : POSITION;
                    float2 uv : TEXCOORD0;
                };

                struct v2f
                {
                    float2 uv : TEXCOORD0;
                    UNITY_FOG_COORDS(1)
                    float4 vertex : SV_POSITION;
                };

                sampler2D _MainTex;
                float4 _MainTex_ST;
                float _Parameter;

                v2f vert(appdata v)
                {
                    v2f o;
                    o.vertex = UnityObjectToClipPos(v.vertex);
                    o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                    UNITY_TRANSFER_FOG(o,o.vertex);
                    return o;
                }

                // RGB ���� HSV ������ ��ȯ�ϴ� �Լ�
                float3 RGBtoHSV(float3 RGB)
                {
                    // RGB ���� 0���� 1 ���̷� ����ȭ�մϴ�.
                    RGB /= 255;

                    // RGB �� �� �ִ밪�� �ּҰ��� ���մϴ�.
                    float maxVal = max(RGB.r, max(RGB.g, RGB.b));
                    float minVal = min(RGB.r, min(RGB.g, RGB.b));

                    // ������ ����մϴ�.
                    float hue;
                    if (maxVal == minVal)
                    {
                        hue = 0; // ������ ���ǵ��� �ʴ� ���
                    }
                    else if (maxVal == RGB.r)
                    {
                        hue = fmod((RGB.g - RGB.b) / (maxVal - minVal), 6);
                    }
                    else if (maxVal == RGB.g)
                    {
                        hue = (RGB.b - RGB.r) / (maxVal - minVal) + 2;
                    }
                    else // maxVal == RGB.b
                    {
                        hue = (RGB.r - RGB.g) / (maxVal - minVal) + 4;
                    }
                    hue *= 60; // ���� ������ ��ȯ�մϴ�.
                    if (hue < 0) hue += 360; // ������ ��� ����� ����ϴ�.

                    // ä���� ����մϴ�.
                    float saturation;
                    if (maxVal == 0)
                    {
                        saturation = 0; // ä���� ���ǵ��� �ʴ� ���
                    }
                    else
                    {
                        saturation = (maxVal - minVal) / maxVal;
                    }

                    // ���� ����մϴ�.
                    float value = maxVal;

                    // HSV ���� ��ȯ�մϴ�.
                    return float3(hue, saturation, value);
                }

                // HSV ���� RGB ������ ��ȯ�ϴ� �Լ�
                float3 HSVtoRGB(float3 HSV)
                {
                    // ������ ���� �������� ���� ������ ��ȯ�մϴ�.
                    HSV.x /= 360;

                    // ������ �ش��ϴ� ������ ���մϴ�.
                    float3 color = abs(frac(HSV.x * 6 - 3) - 1);

                    // ä���� ���� �����մϴ�.
                    color = lerp(1, color, HSV.y) * HSV.z;

                    // RGB ���� ��ȯ�մϴ�.
                    return color * 255;
                }

                // �Ķ���Ϳ� ���� ä���� �����ϴ� �Լ�
                float3 AdjustSaturation(float3 RGB, float parameter)
                {
                    // RGB ���� HSV ������ ��ȯ�մϴ�.
                    float3 HSV = RGBtoHSV(RGB);

                    // ä�� ���� �Ķ���ͷ� �����մϴ�.
                    HSV.y *= parameter;

                    // HSV ���� �ٽ� RGB ������ ��ȯ�մϴ�.
                    return HSVtoRGB(HSV);
                }

                fixed4 frag(v2f i) : SV_Target
                {
                    // sample the texture
                    fixed4 col = tex2D(_MainTex, i.uv);

                // convert to grayscale using NTSC weightings
                col.rgb = AdjustSaturation(col.rgb, _Parameter);

                UNITY_APPLY_FOG(i.fogCoord, col);
                return col;
            }
            ENDHLSL
        }
        }
}
