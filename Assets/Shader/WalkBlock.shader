Shader "Unlit/WalkBlock"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _MainColor ("Color", Color) = (1,1,1,1)
    }
    SubShader
	{
		Tags
		{
			"RenderMode" = "Transparent"
			"Queue" = "Transparent"
			"RenderPipeline" = "UniversalPipeline"
		}

		Pass
		{
			Tags
			{
				"LightMode" = "UniversalForward"
			}
			HLSLPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/DeclareDepthTexture.hlsl"

			struct appdata
			{
				float4 positionOS: POSITION;
			};

			struct v2f
			{
				float4 positionCS: SV_Position;
				float4 positionSS: TEXCOORD0;
			};

			CBUFFER_START(UnityPerMaterial)
				float4 _MainColor;
			CBUFFER_END

			v2f vert(appdata v)
			{
				v2f o;
				o.positionCS = TransformObjectToHClip(v.positionOS.xyz);
				o.positionSS = ComputeScreenPos(o.positionCS);
				return o;
			}

			float4 frag(v2f i):SV_Target
			{
				float2 screenspaceUVs = i.positionSS.xy / i.positionSS.w;
				float rawDepth = SampleSceneDepth(screenspaceUVs);
				float scene01Depth = Linear01Depth(rawDepth, _ZBufferParams);
                float alpha = lerp(0, 0.5, scene01Depth);
                float4 outputColor = _MainColor;
                outputColor.x = alpha;
				// float4 outputColor = _MainColor;
				// if(scene01Depth < 0){
				// 	outputColor = (1,1,1,1);
				// }
				// float4 outputColor = lerp(_ForegroundColor, _BackgroundColor, scene01Depth);
				return outputColor;
			}
			ENDHLSL
		}
	}
}
