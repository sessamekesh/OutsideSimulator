cbuffer cbPerObject
{
	float4 gBlendColor;
};

Texture2D gDiffuseMap;

SamplerState samAnisotropic
{
	Filter = ANISOTROPIC;
	MaxAnisotropy = 4;

	AddressU = WRAP;
	AddressV = WRAP;
};

struct VertexIn
{
	float2 PosL : POSITION;
	float2 Tex : TEXCOORD;
};

struct VertexOut
{
	float4 PosH : SV_POSITION;
	float2 Tex : TEXCOORD;
};

VertexOut MenuVS(VertexIn vin)
{
	VertexOut vout;

	// Transform to homogenous space
	vout.PosH = float4(vin.PosL, 0.0f, 1.0f);
	
	// Output texture position
	vout.Tex = vin.Tex;

	return vout;
}

float4 MenuPS(VertexOut pin) : SV_Target
{
	// Sample texture color
	float4 texColor = gDiffuseMap.Sample(samAnisotropic, pin.Tex);

	// Multiply texture color with blend color
	return texColor * gBlendColor;
}

technique11 MenuTechnique
{
	pass P0
	{
		SetVertexShader(CompileShader(vs_5_0, MenuVS()));
		SetGeometryShader(NULL);
		SetPixelShader(CompileShader(ps_5_0, MenuPS()));
	}
};