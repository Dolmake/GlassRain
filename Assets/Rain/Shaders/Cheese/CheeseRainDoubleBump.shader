/*-------------------------------------------------
Created by: Daniel Peribáñez Sepúlveda
January 2013
peribanez.daniel@gmail.com

*/

Shader "Custom/CheeseRainDoubleBump" {
Properties {

	  _DrawNormals("Draw Normals", Range(0,1)) = 0     
      _Distorsion  ("Distortion", Range (0,128)) = 10      
      _ParticleBump ("Particle Bump", 2D) = "white" {}
      _ParticleDistortion("Particle Distortion", Range(0,1)) = 1
      
      _BumpMap ("Static Bump Map" , 2D) = "black" {}
      _BumpMapDistortion("Static Distortion", Range(0,1)) = 1
      
      _Rastro("Rastro", 2D) = "white" {}
      
      //----Wipers Trick-------
      _AreasCounter("Areas Counter", Float) = 0
	  
	  //----Cheese Areas-------	 	  
	  _AreaRatio ("Default Area Ratio", Range(0.0, 1.0)) = 0.5
	  _HeightToPlus ("Height to Plus", Range (0.0, 10.0)) = 1
	  _TextureSize ( "Texture width", Float) = 16
	  _NumParameters ("Struct Size", Float) = 7
	  _Decimals ("Decimals to encode", Float) = 100000
	  _TexMem("Mem Texture" ,2D) = "black" {}
	  //------------------
	  
	  //-----Diffuse---------
	  _DiffuseColor ("Diffuse Material Color", Color) = (1,1,1,1)      
      //-----------------------
    }
    
Category {

	// We must be transparent, so other objects are drawn before this one.
	Tags { "Queue"="Transparent" "RenderType"="Opaque" }


	SubShader {

		// This pass grabs the screen behind the object into a texture.
		// We can access the result in the next pass as _GrabTexture
		GrabPass {							
			Name "BASE"
			Tags { "LightMode" = "Always" }
 		}
 		
 		// Main pass: Take the texture grabbed above and use the bumpmap to perturb it
 		// on to the screen
		Pass {
			Name "BASE"
			Tags { "LightMode" = "Always" }
			
			CGPROGRAM
			
			#pragma target 4.0
			#pragma vertex vert
			#pragma fragment frag 
			#pragma fragmentoption ARB_precision_hint_fastest
			#include "UnityCG.cginc"
			//#include "CheeseUtils.cginc"
			#include "FakeWipers.cginc" 
			
			
			struct appdata_t {
				float4 vertex : POSITION;
				float2 texcoord: TEXCOORD0;
			};
			
			struct v2f {
				float4 vertex : POSITION;
				float4 posWorld: COLOR;
				float4 uvgrab : TEXCOORD0;
				float2 uvbump : TEXCOORD1;
				float2 uvmain : TEXCOORD2;
				float2 uvsbump : TEXCOORD3;
			};
			
			
			float4 _BumpMap_ST;	
			float4 _SlowBumpMap_ST;		
			
			
			v2f vert (appdata_t v)
			{
				v2f o;
				o.vertex = mul(UNITY_MATRIX_MVP, v.vertex);
				#if UNITY_UV_STARTS_AT_TOP
				float scale = -1.0;
				#else
				float scale = 1.0;
				#endif
				o.uvgrab.xy = (float2(o.vertex.x, o.vertex.y*scale) + o.vertex.w) * 0.5;
				o.uvgrab.zw = o.vertex.zw;
				o.uvbump = TRANSFORM_TEX( v.texcoord, _BumpMap );
				o.uvmain = v.texcoord;
				o.uvsbump = TRANSFORM_TEX( v.texcoord, _SlowBumpMap );
				float4x4 modelMatrix = _Object2World;
            	//float4x4 modelMatrixInverse = _World2Object; 
            	
                //multiplication with unity_Scale.w is unnecessary 
                //because we normalize transformed vectors
            	o.posWorld = mul(modelMatrix, v.vertex); 
				
				return o;
			}
			
			float _Distorsion;
			float _ParticleDistortion;
			float _BumpMapDistortion;
			sampler2D _GrabTexture;
			float4 _GrabTexture_TexelSize;
			sampler2D _BumpMap;	
			sampler2D _ParticleBump;	
			
			float _DrawNormals;			
			float4 _DiffuseColor;		
			
			
         	//------------FRAGMENT------------------
			
			half4 frag( v2f i ) : COLOR
			{	
				half3 staticNormal = UnpackNormal(tex2D(_BumpMap,i.uvbump)) * _BumpMapDistortion;
				half3 particleNormal = UnpackNormal_RGBA(tex2D( _ParticleBump, i.uvmain))* _ParticleDistortion;
				half3 normal = normalize(Wipers(i.uvmain,staticNormal+ particleNormal));
					
				half2 bump = normal.rg; 				
				
				// calculate perturbed coordinates
				float2 offset = bump * _Distorsion * _GrabTexture_TexelSize.xy;
				i.uvgrab.xy = offset * i.uvgrab.z + i.uvgrab.xy ;	
				
				half4 col = tex2Dproj( _GrabTexture, UNITY_PROJ_COORD(i.uvgrab)) * _DiffuseColor + half4(normal,1) * _DrawNormals;					
				return col;				
			}
				
			ENDCG
		}
	}

	// ------------------------------------------------------------------
	// Fallback for older cards and Unity non-Pro
	
	SubShader {
		Blend DstColor Zero
		Pass {
			Name "BASE"
			SetTexture [_BumpMap] {	combine texture }
		}
	}
}
}
