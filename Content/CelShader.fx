// Copyright (c) 2010-2012 SharpDX - Alexandre Mutel
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// c
// Adapted for COMP30019 by Jeremy Nicholson, 10 Sep 2012
// Adapted further by Chris Ewin, 23 Sep 2013opies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// THE SOFTWARE.
//
// Adapted even further by Mathew Blair,

// Our primary information required to do our shading
// The world transform
float4x4 World;
// The view transform
float4x4 View;
// The projection transform transformation, which we will 
// use to properly transform the vertex normals
float4x4 Projection;
// Our position of our camera
float4 cameraPos;
// The transpose of the inverse of our world 
float4x4 worldInvTrp;

// Our world lighting setups Will obviously need to be changed at a later 
// date in order to properly having moving light sources etc.
float4 lightAmbCol = float4(0.8f, 0.8f, 0.8f, 1.0f);
float4 lightPntPos;
float4 lightPntCol = float4(1.0f, 1.0f, 1.0f, 1.0f);

// The direction of the diffuse light
float3 DiffuseLightDirection = float3(10.0, 30.0, 0.0);
 
// The color of the diffuse light
float4 DiffuseColour = float4(0.3, 0.3, 1, 1);
 
// The intensity of the diffuse light
float DiffuseIntensity = 1.5;

// The line colour we will be using for outlines
float4 LineColor = float4(0, 0, 0, 1);
// The amount we will expand the vertices to draw the lines, i.e. line thickness
float LineThickness = .3;

//Our textures for textured objects
Texture2D<float4> Texture;

sampler textureSampler {
    Filter = MIN_MAG_MIP_LINEAR;
    AddressU = Wrap;
    AddressV = Wrap;
};
 
struct VS_IN
{
//	float4 pos : SV_POSITION;
//	float4 nrm : NORMAL;
//	float2 texcoord : TEXCOORD0;
//	float	tangent : TANGENT;
//	float	bitangent : BITANGENT;
	float4 pos : SV_POSITION;
    float4 nrm : NORMAL;
    //float2 tex : TEXCOORD0;

// Other vertex properties, e.g. texture co-ords, surface Kd, Ks, etc
};


struct PS_IN
{
	float4 pos : SV_POSITION; //Position in camera co-ords
	float4 col : COLOR;
	float4 wpos : TEXCOORD0; //Position in world co-ords
	float3 wnrm : TEXCOORD1; //Normal in world co-ords     
	float2 texcoord : TEXCOORD2; // this is new

};


PS_IN CVS( VS_IN input )
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
	//output.col = input.col;
	//output.texcoord = input.tex;	
	//float4 textureColor = Texture.Sample(textureSampler, input.tex);

	output.col = float4(1,1,1,1);
	return output;
}



float4 CPS( PS_IN input ) : SV_Target
	{
	// Our interpolated normal might not be of length 1
	float3 interpNormal = normalize(input.wnrm);

	// Calculate ambient RGB intensities
	float Ka = 1;
//	float3 amb = input.col.rgb*lightAmbCol.rgb*Ka;
	float ambientIntensity = dot(normalize(DiffuseLightDirection),interpNormal);

	float4 colSpe = float4(0,0,0,1);
	//Check if in lantern range
	float3 distToLamp = lightPntPos - input.wpos.xyz;
	
	if(length(distToLamp)<30){
		
		// Calculate diffuse RBG reflections
		float fAtt = 1.0;
		float Kd = 1;
		float3 L = normalize(distToLamp);
		float LdotN = saturate(dot(L,interpNormal.xyz));
		float3 dif = fAtt*lightPntCol.rgb*Kd*input.col.rgb*LdotN;
		float difIntensity = 0.3126 * dif.r + 0.7152 * dif.g + 0.0722 * dif.b;
		
		// Calculate specular reflections
		float Ks = 1;
		float specN = 0.8; 
		float3 V = normalize(cameraPos.xyz - input.wpos.xyz);
		float3 R = normalize(2*LdotN*interpNormal.xyz - L.xyz);
		float3 spe = fAtt*lightPntCol.rgb*Ks*pow(saturate(dot(V,R)),specN);
		float speIntensity =  0.3126 * spe.r + 0.7152 * spe.g + 0.0722 * spe.b;
	
		// Calculate ambient lighting intensity, just work on ambient for now add the others later.
		colSpe.rgb= lightPntCol.rgb;
		speIntensity += difIntensity;
		//Discretize colours
		if(speIntensity > 0.98 ) {
			colSpe = float4(0.9,0.9,0.9,1.0)* colSpe;
		} else if (speIntensity > 0.70 ) {
			colSpe = float4(0.75,0.75,0.75,1.0) * colSpe;
		}else if (speIntensity > 0.35) {
			colSpe = float4(0.35,0.35,0.35,1.0) * colSpe;
		} else if (speIntensity > 0.25) {
			colSpe = float4(0.15,0.15,0.15,1.0) * colSpe;
		} else {
			colSpe = float4(0,0,0,0);
		}

	} else {
		// Calculate diffuse RBG reflections
		float fAtt = 1.5;
		float Kd = 1;
		float3 L = normalize(distToLamp);
		float LdotN = saturate(dot(L,interpNormal.xyz));
		float3 dif = fAtt*lightPntCol.rgb*Kd*input.col.rgb*LdotN;
		float difIntensity = 0.3126 * dif.r + 0.7152 * dif.g + 0.0722 * dif.b;

		// Calculate ambient lighting intensity, just work on ambient for now add the others later.
		colSpe.rgb= lightPntCol.rgb;
		colSpe.a = 1.0;

		if(difIntensity > 0.9 ) {
			colSpe = float4(0.9,0.9,0.9,1.0) * colSpe;
		} else if (difIntensity > 0.50 ) {
			colSpe = float4(0.75,0.75,0.75,1.0) * colSpe;
		}else if (difIntensity > 0.35) {
			colSpe = float4(0.35,0.35,0.35,1.0) * colSpe;
		} else if (difIntensity > 0.2) {
			colSpe = float4(0.15,0.15,0.15,1.0) * colSpe;
		} else {
			colSpe = float4(0,0,0,0);
		}	
	}  
	

	//Work out color based on just ambient
	float4 ambC = input.col * DiffuseColour * DiffuseIntensity;
	float4 colour = float4(0.0f,0.0f,0.0f,0.0f);
	colour.rgb = ambC.rgb;
	colour.a = input.col.a;

	// Combine reflection components 
	//float4 colour = float4(0.0f,0.0f,0.0f,0.0f);
	//colour.rgb = amb.rgb+dif.rgb+spe.rgb;
	//colour.a = input.col.a; 

	float totalIntensity = ambientIntensity;

	// Now we will cel shade the rendering by discretizing the color
	if(totalIntensity > 0.95 ) {
		colour = float4(1.0,1.0,1.0,1.0) * colour;
	} else if (totalIntensity > 0.6) {
		colour = float4(0.7,0.7,0.7,1.0) * colour;
	} else if (totalIntensity > 0.35) {
			colSpe = float4(0.35,0.35,0.35,1.0) * colSpe;
	} else if (totalIntensity > 0.25) {
			colSpe = float4(0.15,0.15,0.15,1.0) * colSpe;
	} else {
			colSpe = float4(0,0,0,0);
	}
	colour.rgb = colour.rgb + colSpe.rgb;
	return colour;
}

technique CelShading
{
	// Second pass, draw the model like normal, with discretized colours
	// which will make it look better than shitty shading.
	pass Pass2 
	{
		Profile = 9.3;
		VertexShader = CVS;
        PixelShader = CPS;
		//CullMode = CCW;
	}
	
} 

/*
float4x4 World;
float4x4 View;
float4x4 Projection;
float4x4 worldInvTrp;
 
float4 AmbientColor = float4(1, 1, 1, 1);
float AmbientIntensity = 0.1;
 
float4 DiffuseLightDirection = float4(1, 0, 0, 0);
float4 DiffuseColor = float4(1, 1, 1, 1);
float DiffuseIntensity = 1.0;
 
float Shininess = 200;
float4 SpecularColor = float4(1, 1, 1, 1);
float SpecularIntensity = 1;
float4 ViewVector = float4(1, 0, 0, 0);
 
Texture2D<float4> Texture;

sampler textureSampler {
    Filter = MIN_MAG_MIP_LINEAR;
    AddressU = Wrap;
    AddressV = Wrap;
};
 
struct VertexShaderInput
{
    float4 Position : SV_POSITION;
    float4 Normal : NORMAL;
    float2 TextureCoordinate : TEXCOORD0;
};
 
struct VertexShaderOutput
{
    float4 Position : SV_POSITION;
    float4 Color : COLOR;
    float4 Normal : TEXCOORD0;
    float2 TextureCoordinate : TEXCOORD1;
};
 
VertexShaderOutput VertexShaderFunction(VertexShaderInput input)
{
    VertexShaderOutput output;
 
    float4 worldPosition = mul(input.Position, World);
    float4 viewPosition = mul(worldPosition, View);
    output.Position = mul(viewPosition, Projection);
 
    float4 normal = normalize(mul(input.Normal, worldInvTrp));
    float lightIntensity = dot(normal, DiffuseLightDirection);
    output.Color = saturate(DiffuseColor * DiffuseIntensity * lightIntensity);
 
    output.Normal = normal;
 
    output.TextureCoordinate = input.TextureCoordinate;
    return output;
}
 
float4 PixelShaderFunction(VertexShaderOutput input) : SV_TARGET
{
    float4 textureColor = Texture.Sample(textureSampler, input.TextureCoordinate);
    return textureColor;
}
 
technique Textured
{
    pass Pass1
    {
		Profile = 9.1;
        VertexShader = VertexShaderFunction;
        PixelShader = PixelShaderFunction;
    }
} */
