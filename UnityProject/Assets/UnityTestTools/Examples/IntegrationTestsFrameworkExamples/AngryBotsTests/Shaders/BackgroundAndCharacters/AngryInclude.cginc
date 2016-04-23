// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'

// Upgrade NOTE: unity_Scale shader variable was removed; replaced 'unity_Scale.w' with '1.0'


#ifndef ANGRY_CG_INCLUDED
#define ANGRY_CG_INCLUDED

#include "UnityCG.cginc"

void WriteTangentSpaceData (appdata_full v, out half3 ts0, out half3 ts1, out half3 ts2) {
	TANGENT_SPACE_ROTATION;
	ts0 = mul(rotation, unity_ObjectToWorld[0].xyz * 1.0);
	ts1 = mul(rotation, unity_ObjectToWorld[1].xyz * 1.0);
	ts2 = mul(rotation, unity_ObjectToWorld[2].xyz * 1.0);				
}

half2 EthansFakeReflection (half4 vtx) {
	half3 worldSpace = mul(unity_ObjectToWorld, vtx).xyz;
	worldSpace = (-_WorldSpaceCameraPos * 0.6 + worldSpace) * 0.07;
	return worldSpace.xz;
}

#endif