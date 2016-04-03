/*-------------------------------------------------
Created by: Daniel Peribáñez Sepúlveda
January 2013
peribanez.daniel@gmail.com

/**/

#ifndef __FAKE_WIPERS
#define __FAKE_WIPERS

#include "CheeseUtils.cginc"

float _AreasCounter;

float3 Wipers(float2 texCoordinate, float3 normal) 
{	
	//Opt: discarding up fragments
	if (texCoordinate.y > 0.5f) return normal;
	
	float invDecimals = 1.0f / _Decimals;
	
	//Num of Waves serialized
	int areasCounter = (int)_AreasCounter;//DecodeColor32_frag(tex2Dlod(_TexMem, float4(0, 0, 0, 0)),invDecimals);
	
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
		
		half Z = 0;//DecodeColor32_frag(tex2Dlod(_TexMem, float4( tx_coord.x,  tx_coord.y, 0, 0)),invDecimals);
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






//--------------------------------------------------------  

#endif