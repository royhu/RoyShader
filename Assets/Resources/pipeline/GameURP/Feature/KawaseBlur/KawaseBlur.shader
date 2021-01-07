Shader "RoyShader/KawaseBlur"
{
	Properties
	{
		_MainTex ("Base (RGB)", 2D) = "white" {}
	}

	SubShader
	{
		Tags
		{
			"Queue" 			= "Geometry"
			"IgnoreProjector" 	= "True"
			"RenderType" 		= "Opaque"
		}
		LOD 200
		
		Pass
		{
		CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#include "UnityCG.cginc"

			sampler2D _MainTex;
			float4 _MainTex_ST;
			float4 _MainTex_TexelSize;
			float _Offset;

			struct v2f
			{
				half2 uv 	: TEXCOORD0;
				float4 pos	: SV_POSITION;
			};

			half4 blur(sampler2D tex, half2 uv, float offset, float2 texelSize)
			{
				float2 off = float2(offset, -offset);

				half4 o = (half4)0;
				o += tex2D(tex, uv + off.xx * texelSize);
				o += tex2D(tex, uv + off.xy * texelSize);
				o += tex2D(tex, uv + off.yx * texelSize);
				o += tex2D(tex, uv + off.yy * texelSize);
				return o * 0.25;
			}
			
			v2f vert(appdata_base v)
			{
				v2f o;
				o.uv = v.texcoord.xy;
				o.pos = UnityObjectToClipPos(v.vertex);
				
				return o;
			}

			half4 frag (v2f i) : SV_Target
			{
				half4 color;
				half2 uv = TRANSFORM_TEX(i.uv, _MainTex);
				color = blur(_MainTex, uv, _Offset, _MainTex_TexelSize.xy);
				
				return color;
			}
		ENDCG
		}
	}
	FallBack "Diffuse"
}
