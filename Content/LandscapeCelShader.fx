﻿// these won't change in a given iteration of the shader
float4x4 World;
float4x4 View;
float4x4 Projection;
float4 cameraPos;
float4x4 worldInvTrp;

// Our world lighting setups Will obviously need to be changed at a later 
// date in order to properly having moving light sources etc.
float3 lightPntPos;
float4 lightPntCol = float4(1.0f, 0.5f, 0.1f, 1.0f);
float4 sunPntCol = float4(0.55, 0.1, 0.9, 1);
//float4 sunPntCol = float4(1, 1, 1, 1);

// The direction of the diffuse light (I.E. our sun, this is static)
float3 sunPntPos = float3(-1000,20, 100);
 // The intensity of the diffuse light
float sunIntensity = 0.05f;

//Our missle
float3 missilePntPos;
float4 missilePntCol;

struct VS_IN
{
	float4 pos : POSITION;
	float4 nrm : NORMAL;
	float4 col : COLOR;
// Other vertex properties, e.g. texture co-ords, surface Kd, Ks, etc
};

struct PS_IN
{
	float4 pos : SV_POSITION; //Position in camera co-ords
	float4 col : COLOR;
	float4 wpos : TEXCOORD0; //Position in world co-ords
	float3 wnrm : TEXCOORD1; //Normal in world co-ords 
};


PS_IN VS( VS_IN input )
{
	PS_IN output = (PS_IN)0;

	// Convert Vertex position and corresponding normal into world coords
	// Note that we have to multiply the normal by the transposed inverse of the world 
	// transformation matrix (for cases where we have non-uniform scaling; we also don't
	// care about the "fourth" dimension, because translations don't affect the normal)
	output.wpos = mul(input.pos, World);
	output.wnrm = mul(input.nrm.xyz, (float3x3)worldInvTrp);

	// Transform vertex in world coordinates to camera coordinates
	float4 viewPos = mul(output.wpos, View);
    output.pos = mul(viewPos, Projection);

	// Just pass along the colour at the vertex
	output.col = input.col;

	return output;
}

float4 PS( PS_IN input ) : SV_Target
	{
	// Our interpolated normal might not be of length 1
	float3 interpNormal = normalize(input.wnrm);
	
	//First calculate the lamp
	// Calculate our distance lightwise.
	float dist = distance(lightPntPos.xyz,input.wpos.xyz);

	// Calculate ambient RGB intensities
	float Ka = 0.15; // Very little ambient light generated by the lamp.
	float3 amb = input.col.rgb*lightPntCol.rgb*Ka;
	// Calculate diffuse RBG reflections
	float fAtt = 6/(dist+3); //Fall off with distance.
	float Kd = 1; //All very flat diffuse reflections
	float3 L = normalize(lightPntPos.xyz - input.wpos.xyz);
	float LdotN = saturate(dot(L,interpNormal.xyz));
	float3 dif = fAtt*lightPntCol.rgb*Kd*input.col.rgb*LdotN;

	// Calculate specular reflections
	float Ks = 1;
	float specN = 1; // Numbers>>1 give more mirror-like highlights
	float3 V = normalize(cameraPos.xyz - input.wpos.xyz);
	float3 R = normalize(2*LdotN*interpNormal.xyz - L.xyz);
	//float3 R = normalize(0.5*(L.xyz+V.xyz)); //Blinn-Phong equivalent
	float3 spe = fAtt*lightPntCol.rgb*Ks*pow(saturate(dot(V,R)),specN);

	// Combine components
	float4 lampCol = float4(0.0f,0.0f,0.0f,0.0f);
	lampCol.rgb = amb.rgb+dif.rgb+spe.rgb;
	lampCol.a = input.col.a;


	
	
	//Now calculate the missiles
	// Calculate our distance lightwise.
	float distProj = distance(missilePntPos.xyz,input.wpos.xyz);
	// Calculate ambient RGB intensities
	float KaProj = 0.05; // Very little ambient light generated by the lamp.
	float3 ambProj = input.col.rgb*missilePntCol.rgb*KaProj;
	// Calculate diffuse RBG reflections
	float fAttProj = 2/(distProj+3); //Fall off with distance.
	float KdProj = 1; //All very flat diffuse reflections
	float3 LProj = normalize(missilePntPos.xyz - input.wpos.xyz);
	float LdotNProj = saturate(dot(LProj,interpNormal.xyz));
	float3 difProj = fAttProj*missilePntCol.rgb*KdProj*input.col.rgb*LdotNProj;

	// Calculate specular reflections
	float KsProj = 2.0f;
	float specNProj = 2; // Numbers>>1 give more mirror-like highlights
	float3 VProj = normalize(cameraPos.xyz - input.wpos.xyz);
	float3 RProj = normalize(2*LdotNProj*interpNormal.xyz - LProj.xyz);
	float3 speProj = fAttProj*missilePntCol.rgb*KsProj*pow(saturate(dot(VProj,RProj)),specNProj);

	// Combine components
	float4 projCol = float4(0.0f,0.0f,0.0f,0.0f);
	projCol.rgb = ambProj.rgb+difProj.rgb+speProj.rgb;
	projCol.a = input.col.a;




	
	//Now calculate the suns
	// Calculate ambient RGB intensities
	float3 ambSun = input.col.rgb*sunPntCol.rgb*sunIntensity;
	// Calculate diffuse RBG reflections
	float fAttSun = 0.6; // Consider the sun difference around the map change to be negligible, therefore no fall off
	float KdSun = 5;
	float3 LSun = normalize(sunPntPos.xyz - input.wpos.xyz);
	float LdotNSun = saturate(dot(LSun,interpNormal.xyz));
	float3 difSun = fAttSun*sunPntCol.rgb*KdSun*input.col.rgb*LdotNSun;
	// Calculate specular reflections
	float specNSun = 10; // Numbers>>1 give more mirror-like highlights
	float3 VSun = normalize(cameraPos.xyz - input.wpos.xyz);
	float3 RSun = normalize(2*LdotNSun*interpNormal.xyz - LSun.xyz);
	float3 speSun = fAtt*sunPntCol.rgb*0.90*pow(saturate(dot(VSun,RSun)),specNSun);

	// Combine components
	float4 sunCol = float4(0.0f,0.0f,0.0f,0.0f);
	sunCol.rgb = ambSun.rgb+difSun.rgb+speSun.rgb;

	//Now combine both light sources
	float4 returnCol = float4(0.0f,0.0f,0.0f,0.0f);
	returnCol.rgb = sunCol.rgb + lampCol.rgb + projCol.rgb;
	returnCol.a = input.col.a;
	
	float totalIntensity = 0.3126 * returnCol.r + 0.7152 * returnCol.g + 0.0722 * returnCol.b;

	// Now we will cel shade the rendering by discretizing the color
	if(totalIntensity >0.98 ) {
		returnCol = float4(1.0,1.0,1.0,1.0) * returnCol;
	} else if (totalIntensity > 0.80 ) {
		returnCol = float4(0.9,0.9,0.9,1.0) * returnCol;
	} else if (totalIntensity > 0.70) {
		returnCol = float4(0.85,0.85,0.85,1.0) * returnCol;
	} else if (totalIntensity > 0.50) {
		returnCol = float4(0.65,0.65,0.65,1.0) * returnCol;
	} else {
		returnCol = float4(0.5,0.5,0.5,1.0) * returnCol;
	}


	return returnCol;
}



technique Lighting
{
    pass Pass1
    {
		Profile = 9.3;
        VertexShader = VS;
        PixelShader = PS;
    }
	pass Pass2 
	{
		Profile = 9.3;
        VertexShader = VS;
        PixelShader = PS;
	}
}