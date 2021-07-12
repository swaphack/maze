Shader "Extensions/Color/Terrian2d" {
	Properties{
	}
	SubShader{
		Pass {
			CGPROGRAM

			#pragma vertex vert
			#pragma fragment frag

			struct a2v
			{
				float4 vertex : POSITION;
				fixed3 color : COLOR0;
			};

			struct v2f
			{
				float4 pos : SV_POSITION;
				fixed3 color : COLOR0;
			};

			v2f vert(a2v v)
			{
				v2f f;
				f.pos = UnityObjectToClipPos(v.vertex);
				f.color = v.color;
				return f;
			}

			fixed4 frag(v2f f) : SV_Target 
			{
				return fixed4(f.color,1.0);
			}

			ENDCG
		}
	}
	FallBack "Diffuse"
}

