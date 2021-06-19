// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Extensions/UI/UIImageGray" 
{
	Properties {
		_MainTex("Texture", 2D) = "white" {}
	}

	SubShader {
		Pass {
			CGPROGRAM

			#include "UnityCG.cginc" 

			#pragma vertex vert
			#pragma fragment frag 			

			sampler2D _MainTex;
			float4 _MainTex_ST;

			struct v2f
			{
				float4 pos:POSITION;
				float2 uv:TEXCOORD0; 
			};  

			v2f vert(appdata_base  v)
			{
				v2f o;
				o.pos = UnityObjectToClipPos(v.vertex);
				o.uv = TRANSFORM_TEX(v.texcoord, _MainTex);
				return o;
			}

			fixed4 frag(v2f v) : COLOR 
			{
				fixed4 col = tex2D(_MainTex, v.uv);
				float grey = dot(col.rgb, float3(0.299, 0.587, 0.114));
				col.rgb = float3 (grey, grey, grey);
				return col;
			}

			ENDCG
		}
	}

	FallBack "Diffuse"
}
