Shader "Extensions/Color/Simple" {
	Properties {
		_Color ("Main Color", Color) = (1,1,1,1)
	}
	SubShader {
		Pass {
			CGPROGRAM

			#pragma vertex vert
			#pragma fragment frag

			float4 _Color;

			float4 vert(float4 v:POSITION):SV_POSITION {
				return UnityObjectToClipPos(v);
			}

			fixed4 frag():SV_Target {
				return _Color;
			}

			ENDCG
		}
	} 
	FallBack "Diffuse"
}

