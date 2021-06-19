// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Extensions/UI/UIImageColor"
{
	Properties {
		_MainTexure ("Main Texure", 2D) = "white" {}
		_MainColor ("Main Color", Color) = (1.0, 1.0, 1.0, 1.0)
	}

	SubShader {
		Pass {
			CGPROGRAM

			#include "UnityCG.cginc" 

			#pragma vertex vert
			#pragma fragment frag

			float4 _MainColor;
			sampler2D _MainTexure;
			float4 _MainTexure_ST;

			struct v2f 
			{
				float4 pos:POSITION;
				float2 uv:TEXCOORD0; 
			};

			v2f vert( appdata_base  v)
			{
				v2f o;
				o.pos = UnityObjectToClipPos(v.vertex);
				o.uv = TRANSFORM_TEX(v.texcoord, _MainTexure);
				return o;
			}

			fixed4 frag(v2f v):COLOR 
			{
				fixed4 col = tex2D(_MainTexure, v.uv) * _MainColor;
				return col;
			}
			
			ENDCG
		}
	}
}