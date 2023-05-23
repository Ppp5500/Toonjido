Shader "Custom/ColorFilter"
{
    Properties
    {
        _FilterStrength01("Filter Strength01", Range(0,1)) = 0.5
        _FilterStrength02("Filter Strength02", Range(0,1)) = 0.5
        _FilterStrength03("Filter Strength03", Range(0,1)) = 0.5
    }
        SubShader
    {
        Tags { "RenderType" = "Transparent" "Queue" = "Transparent"}
        zwrite off

        GrabPass{}

        CGPROGRAM
        #pragma surface surf nolight noambient alpha:fade
        #pragma target 3.0

        sampler2D _GrabTexture;
        float _FilterStrength01;
        float _FilterStrength02;
        float _FilterStrength03;


        struct Input
        {
            float4 color:COLOR;
            float4 screenPos;
            float2 uv_MainTex;
        };

        void surf(Input IN, inout SurfaceOutput o)
        {
            float3 screenUV = IN.screenPos.rgb / IN.screenPos.a;
            fixed4 c = tex2D(_GrabTexture , screenUV.xy);
            fixed3 result = fixed3(c.r + _FilterStrength01, c.g + _FilterStrength02, c.b + _FilterStrength03);
            o.Emission = result;
        }

        float4 Lightingnolight(SurfaceOutput s, float3 lightDir, float atten)
        {
            return float4(0, 0, 0, 1);
        }
        ENDCG
    }
        FallBack "Legacy Shaders/Transparent/Vertexlit"
}
