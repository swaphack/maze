// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Extensions/Shadow/ShadowColor" {
	Properties {
		_MainColor ("Main Color", Color) = (1.0, 1.0, 1.0, 1.0)
		_ShadowColor ("Shadow Color", Color) = (1.0, 1.0, 1.0, 1.0)
	}
	SubShader {
		Tags { "RenderType"="Opaque" }

		Pass {
			Tags { "LightMode"="ForwardBase" }

			CGPROGRAM

			#pragma multi_compile_fwdbase

			#include "UnityCG.cginc" 
			#include "Lighting.cginc" 
			#include "AutoLight.cginc" 

			#pragma vertex vertmain
			#pragma fragment fragmain

			fixed4 _MainColor;
			fixed4 _ShadowColor;

			struct v2f
			{
				float4 pos : SV_POSITION ;
				half4 uv : TEXCOORD0 ; 
				SHADOW_COORDS(1)
			};

			v2f vertmain( appdata_full v)
			{
				v2f o;
				o.pos = UnityObjectToClipPos(v.vertex);
				o.uv.xy = v.texcoord.xy;
				TRANSFER_SHADOW(o);
				return o;
			}

			fixed4 fragmain(v2f v):COLOR0
			{
				float attenuation = SHADOW_ATTENUATION(v);
				return lerp(_MainColor*_ShadowColor, _MainColor, attenuation);
			}
			
			ENDCG
		}

		Pass {
			Name "ShadowCollector"
			Tags { "LightMode"="ShadowCollector" }

			Fog {Mode Off}
			ZWrite On ZTest LEqual

			CGPROGRAM

			#pragma vertex vert
			#pragma fragment frag
			#pragma multi_compile_shadowcollector

			#define SHADOW_COLLECTOR_PASS
			#include "UnityCG.cginc"

			struct appdata
			{
				float4 vertex:POSITION ;
			};  

			struct v2f
			{
				V2F_SHADOW_COLLECTOR;
			}; 

			v2f vert( appdata v )
			{
				v2f o;
				TRANSFER_SHADOW_COLLECTOR(o);
				return o;
			}

			fixed4 frag(v2f v) : SV_Target
			{
				SHADOW_COLLECTOR_FRAGMENT(v)
			}
			  
			ENDCG  
		}
	}

	Fallback "Diffuse"
}