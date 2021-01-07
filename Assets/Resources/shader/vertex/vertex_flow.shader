Shader "RoyShader/vertex/vertex_flow"
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

			struct v2f
			{
				half2 uv 	: TEXCOORD0;
				float4 pos	: SV_POSITION;
			};
			
			v2f vert(appdata_base v)
			{
				v2f o;
				o.uv = v.texcoord.xy;
				v.vertex.y = sin(v.vertex.x + _Time.y);
				o.pos = UnityObjectToClipPos(v.vertex);
				
				return o;
			}

			half4 frag (v2f i) : SV_TARGET
			{
				half4 color = 0;
				half2 uv = TRANSFORM_TEX(i.uv, _MainTex);
				color = tex2D(_MainTex, uv);
				
				return color;
			}
		ENDCG
		}
	}
	FallBack "Diffuse"
}