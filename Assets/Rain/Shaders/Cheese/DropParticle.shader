Shader "Custom/DropParticle" {
	Properties {
		_MainTex ("Base (RGB)", 2D) = "white" {}		
	}
	SubShader {
		//Tags {  "Queue" = "Transparent"  "RenderType"="Transparent" }
		//Tags {  "Queue" = "Opaque" }
		LOD 200
		
		Pass {
			Lighting Off
			Cull Back	
			//Blend SrcAlpha OneMinusSrcAlpha
			CGPROGRAM
				#pragma vertex vert
				#pragma fragment frag
				#include "UnityCG.cginc"
			
				struct v2f {
					float4 pos : SV_POSITION;
					float2 uv_MainTex : TEXCOORD0;
				};
			
				float4 _MainTex_ST;
			
				v2f vert(appdata_base v) {
					v2f o;
					o.pos = mul(UNITY_MATRIX_MVP, v.vertex);
					o.uv_MainTex = TRANSFORM_TEX(v.texcoord, _MainTex);
					return o;
				}
			
				sampler2D _MainTex;				
				float4 frag(v2f IN) : COLOR {
						return tex2D (_MainTex, IN.uv_MainTex);
					
				}
			ENDCG
		}
	}
}
