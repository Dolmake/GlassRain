/*-------------------------------------------------
Created by: Daniel Peribáñez Sepúlveda
January 2013
peribanez.daniel@gmail.com

/**/

#ifndef __CHEESE_UTILS
#define __CHEESE_UTILS

//------Decode color function, in a Color, out a float with "Decimals" precission
float _Decimals;

inline float DecodeColor32_frag( float4 c )
{
	int R = (int)(c.r * 255.0f) * 16777216;	
	int G = (int)(c.g * 255.0f) * 65536;	
	int B = (int)(c.b * 255.0f) * 256;	
	int A = (int)(c.a * 255.0f);
	int integer = R + G + B + A;
	return (float)integer / _Decimals;
}

inline float DecodeColor32_frag( float4 c, float inverse_decimals )
{	
	int R = (int)(c.r * 255.0f) * 16777216;	
	int G = (int)(c.g * 255.0f) * 65536;	
	int B = (int)(c.b * 255.0f) * 256;	
	int A = (int)(c.a * 255.0f);
	int integer = R + G + B + A;
	return (float)integer * inverse_decimals;
}

//----UnpackNormals from RGBA--------------------
inline float3 UnpackNormal_RGBA(float4 color)
{
	return color.rgb - (0.5f);
}

//----Increments Texture Coordinates------------
inline float4 IncrementCoords(float4 coords)
{
	float4 result = float4(coords.x + coords.z, coords.y, coords.z, coords.w);
	
	if (result.x >= 1.0f) 
	{
		result.x -= 1.0f;
		result.y += result.w;
	}
	return result;
}

//----Manhattan distance---------------------
inline half ManhattanDistance(float3 v)
{
	return abs(v.x) + abs(v.y) + abs(v.z);
}

inline half ManhattanDistance(float3 a, float3 b)
{
	return ManhattanDistance(b-a);
}

 //------Cheese Area-----------------------
 
float _AreaRatio;
sampler2D _TexMem;
float _TextureSize;
float _NumParameters;
float _HeightToPlus; 


float3 CHEESE_AreaNormal (float2 texCoordinate, float3 normal) 
{	
	float invDecimals = 1.0f / _Decimals;
	
	//Num of Waves serialized
	int areasCounter = (int)DecodeColor32_frag(tex2Dlod(_TexMem, float4(0, 0, 0, 0)),invDecimals);
	
	//Default displacement value		
	float px = 1.0f / _TextureSize; 
	
	//Offset value		
	float offsetPX = px;
	
	//Current tex-coordinates to access MemoryTexture
	float4 tx_coord = float4(offsetPX,0.0f,px,0.5f);
	
	for (int i =0 ; i < areasCounter; ++i)
	{
		//We use a global _AreaRatio for performance instead of particular AreaRatio
		half MaxDist = _AreaRatio;//DecodeColor32_frag(tex2Dlod(_TexMem, float4( tx_coord.x,  tx_coord.y, 0, 0)),invDecimals);			
		tx_coord = IncrementCoords(tx_coord);
		
		half X = DecodeColor32_frag(tex2Dlod(_TexMem, float4( tx_coord.x,  tx_coord.y, 0, 0)),invDecimals);
		tx_coord = IncrementCoords(tx_coord);
		
		half Z = DecodeColor32_frag(tex2Dlod(_TexMem, float4( tx_coord.x,  tx_coord.y, 0, 0)),invDecimals);
		tx_coord = IncrementCoords(tx_coord);	
			
		half2 center = half2(X,Z);
		
		half2 vectorVertex = half2(texCoordinate.x,texCoordinate.y) - center;
		
		float MaxDistSQR = MaxDist * MaxDist;
		float distSQR = dot(vectorVertex,vectorVertex);
		
		//If Vertex check the distance to the center...
		if (distSQR < MaxDistSQR)
		{
			fixed Percent = DecodeColor32_frag(tex2Dlod(_TexMem, float4( tx_coord.x,  tx_coord.y, 0, 0)),invDecimals);
			tx_coord = IncrementCoords(tx_coord);
		
			half startAngle = DecodeColor32_frag(tex2Dlod(_TexMem, float4( tx_coord.x,  tx_coord.y, 0, 0)),invDecimals);
			tx_coord = IncrementCoords(tx_coord);
			
			half endAngle =  DecodeColor32_frag(tex2Dlod(_TexMem, float4( tx_coord.x,  tx_coord.y, 0, 0)),invDecimals);
			tx_coord = IncrementCoords(tx_coord);		
		
			half vAngle = atan2(vectorVertex.y, vectorVertex.x);
			if (vAngle < 0) vAngle += 6.28;
			
			if (vAngle > startAngle && vAngle < endAngle)
			{
				fixed percent = 1.0f - Percent; 			
		    	normal += half3(0,0,_HeightToPlus * 10)  * percent;
		    }			
		}
		else
		{
			tx_coord = IncrementCoords(tx_coord);//Percent
			tx_coord = IncrementCoords(tx_coord);//StartAngle
			tx_coord = IncrementCoords(tx_coord);//EndAngle
		}
	
	}
	//End for
	
	//if (ManhattanDistance(normal) < 0)
		//normal = float3(0,1,0);
	return normal;
}
//End Cheese Area    



sampler2D _Rastro;

float3 CHEESE_AreaNormal_Rastro (float2 texCoordinate, float3 normal) 
{	
	float invDecimals = 1.0f / _Decimals;
	
	//Num of Waves serialized
	int areasCounter = (int)DecodeColor32_frag(tex2Dlod(_TexMem, float4(0, 0, 0, 0)),invDecimals);
	
	//Default displacement value		
	float px = 1.0f / _TextureSize; 
	
	//Offset value		
	float offsetPX = px;
	
	//Current tex-coordinates to access MemoryTexture
	float4 tx_coord = float4(offsetPX,0.0f,px,0.5f);
	
	for (int i =0 ; i < areasCounter; ++i)
	{
		//We use a global _AreaRatio for performance instead of particular AreaRatio
		half MaxDist = _AreaRatio;//DecodeColor32_frag(tex2Dlod(_TexMem, float4( tx_coord.x,  tx_coord.y, 0, 0)),invDecimals);			
		tx_coord = IncrementCoords(tx_coord);
		
		half X = DecodeColor32_frag(tex2Dlod(_TexMem, float4( tx_coord.x,  tx_coord.y, 0, 0)),invDecimals);
		tx_coord = IncrementCoords(tx_coord);
		
		half Z = DecodeColor32_frag(tex2Dlod(_TexMem, float4( tx_coord.x,  tx_coord.y, 0, 0)),invDecimals);
		tx_coord = IncrementCoords(tx_coord);	
			
		half2 center = half2(X,Z);
		
		half2 vectorVertex = half2(texCoordinate.x,texCoordinate.y) - center;
		
		float MaxDistSQR = MaxDist * MaxDist;
		float distSQR = dot(vectorVertex,vectorVertex);
		
		//If Vertex check the distance to the center...
		if (distSQR < MaxDistSQR)
		{
			fixed Percent = DecodeColor32_frag(tex2Dlod(_TexMem, float4( tx_coord.x,  tx_coord.y, 0, 0)),invDecimals);
			tx_coord = IncrementCoords(tx_coord);
		
			half startAngle = DecodeColor32_frag(tex2Dlod(_TexMem, float4( tx_coord.x,  tx_coord.y, 0, 0)),invDecimals);
			tx_coord = IncrementCoords(tx_coord);
			
			half endAngle =  DecodeColor32_frag(tex2Dlod(_TexMem, float4( tx_coord.x,  tx_coord.y, 0, 0)),invDecimals);
			tx_coord = IncrementCoords(tx_coord);		
		
			half vAngle = atan2(vectorVertex.y, vectorVertex.x);
			if (vAngle < 0) vAngle += 6.28;
			
			if (vAngle > startAngle && vAngle < endAngle)
			{
				fixed percent = 1.0f - Percent; 	
				float2 uv = (vectorVertex + 0.5);
				normal += UnpackNormal(tex2Dlod(_Rastro, float4(uv ,0,0)))  * percent * _HeightToPlus;
		    	normal += half3(0,0,_HeightToPlus * 10)  * percent;
		    }			
		}
		else
		{
			tx_coord = IncrementCoords(tx_coord);//Percent
			tx_coord = IncrementCoords(tx_coord);//StartAngle
			tx_coord = IncrementCoords(tx_coord);//EndAngle
		}
	
	}
	//End for
	
	//if (ManhattanDistance(normal) < 0)
		//normal = float3(0,1,0);
	return normal;
}
//End Cheese Area    



//--------------------------------------------------------  

#endif