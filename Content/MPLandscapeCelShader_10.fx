﻿// these won't change in a given iteration of the shader
float4x4 World;
float4x4 View;
float4x4 Projection;
float4 cameraPos;
float4x4 worldInvTrp;

#define MAX_LIGHTS 14
// Our world lighting setups Will obviously need to be changed at a later 
// date in order to properly having moving light sources etc.

struct PointLight
{
	float Ka;
	float Kd;
	float Ks;
	float fallOffTop;
	float specN;
	float x;
	float y; 
	float z;
	float r;
	float g;
	float b;
	float a;
};

//The lights we are currently rendering
PointLight player;
PointLight sun;
PointLight extra1;
PointLight extra2;
PointLight extra3;
PointLight extra4;
PointLight extra5;
PointLight extra6;
PointLight extra7;
PointLight extra8;
PointLight extra9;
PointLight extra10;
PointLight extra11;
PointLight extra12;


//The output colour
int NumLights;

struct VS_IN
{
	float4 pos : POSITION;
	float4 nrm : NORMAL;
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
	output.col = float4(1,1,1,1);
	return output;
}

float4 PS( PS_IN input ) : SV_Target
{
	// Our interpolated normal might not be of length 1
	float3 interpNormal = normalize(input.wnrm);

	//Make our array of lights
	PointLight lights[MAX_LIGHTS];
	lights[0] = player;
	lights[1] = sun;
	lights[2] = extra1;
	lights[3] = extra2;
	lights[4] = extra3;
	lights[5] = extra4;
	lights[6] = extra5;
	lights[7] = extra6;
	lights[8] = extra7;
	lights[9] = extra8;
	lights[10] = extra9;
	lights[11] = extra10;
	lights[12] = extra11;
	lights[13] = extra12;


	float4 colours[MAX_LIGHTS];

	for(int i=0; i<MAX_LIGHTS; i++){
		//Get the light
		PointLight light = lights[i];

		// Calculate our distance lightwise.
		float3 lightPntPos = float3(light.x,light.y,light.z);
		float4 lightPntCol = float4(light.r, light.g, light.b, 1.0);
		float dist = distance(lightPntPos.xyz,input.wpos.xyz);


		// Calculate ambient RGB intensities
		float Ka = light.Ka; // Very little ambient light generated by the lamp.
		float3 amb = input.col.rgb*lightPntCol.rgb*Ka;
		// Calculate diffuse RBG reflections
		float fAtt = light.fallOffTop/(dist+3); //Fall off with distance.
		float Kd = light.Kd; //All very flat diffuse reflections
		float3 L = normalize(lightPntPos.xyz - input.wpos.xyz);
		float LdotN = saturate(dot(L,interpNormal.xyz));
		float3 dif = fAtt*lightPntCol.rgb*Kd*input.col.rgb*LdotN;

		// Calculate specular reflections
		float Ks = light.Ks;
		float specN = light.specN; // Numbers>>1 give more mirror-like highlights
		float3 V = normalize(cameraPos.xyz - input.wpos.xyz);
		float3 R = normalize(2*LdotN*interpNormal.xyz - L.xyz);
		//float3 R = normalize(0.5*(L.xyz+V.xyz)); //Blinn-Phong equivalent
		float3 spe = fAtt*lightPntCol.rgb*Ks*pow(saturate(dot(V,R)),specN);

		//Combine everything
		float4 col = float4(0,0,0,1);
		col.rgb = amb.rgb+dif.rgb+spe.rgb;
		colours[i].rgb = col;
	}

	//Now combine both light sources
	float4 returnCol = float4(0.0f,0.0f,0.0f,0.0f);

	for(int j=0; j<MAX_LIGHTS; j++){
		returnCol.rgb += colours[j].rgb;
	//returnCol.rgb = colours[0].rgb+ colours[1].rgb +colours[2].rgb;
	}
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
		Profile = 10.0;
        VertexShader = VS;
        PixelShader = PS;
    }

}