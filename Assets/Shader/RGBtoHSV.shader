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

                // RGB 값을 HSV 값으로 변환하는 함수
                float3 RGBtoHSV(float3 RGB)
                {
                    // RGB 값을 0에서 1 사이로 정규화합니다.
                    RGB /= 255;

                    // RGB 값 중 최대값과 최소값을 구합니다.
                    float maxVal = max(RGB.r, max(RGB.g, RGB.b));
                    float minVal = min(RGB.r, min(RGB.g, RGB.b));

                    // 색조를 계산합니다.
                    float hue;
                    if (maxVal == minVal)
                    {
                        hue = 0; // 색조가 정의되지 않는 경우
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
                    hue *= 60; // 각도 단위로 변환합니다.
                    if (hue < 0) hue += 360; // 음수인 경우 양수로 만듭니다.

                    // 채도를 계산합니다.
                    float saturation;
                    if (maxVal == 0)
                    {
                        saturation = 0; // 채도가 정의되지 않는 경우
                    }
                    else
                    {
                        saturation = (maxVal - minVal) / maxVal;
                    }

                    // 명도를 계산합니다.
                    float value = maxVal;

                    // HSV 값을 반환합니다.
                    return float3(hue, saturation, value);
                }

                // HSV 값을 RGB 값으로 변환하는 함수
                float3 HSVtoRGB(float3 HSV)
                {
                    // 색조를 각도 단위에서 비율 단위로 변환합니다.
                    HSV.x /= 360;

                    // 색조에 해당하는 색상을 구합니다.
                    float3 color = abs(frac(HSV.x * 6 - 3) - 1);

                    // 채도와 명도를 적용합니다.
                    color = lerp(1, color, HSV.y) * HSV.z;

                    // RGB 값을 반환합니다.
                    return color * 255;
                }

                // 파라미터에 따라 채도를 조절하는 함수
                float3 AdjustSaturation(float3 RGB, float parameter)
                {
                    // RGB 값을 HSV 값으로 변환합니다.
                    float3 HSV = RGBtoHSV(RGB);

                    // 채도 값을 파라미터로 조정합니다.
                    HSV.y *= parameter;

                    // HSV 값을 다시 RGB 값으로 변환합니다.
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
