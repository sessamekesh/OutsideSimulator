cbuffer cbPerObject
{
	float4x4 gWorldViewProj;
};

Texture2D gBasicTexture;

SamplerState samAnisotropic
{
	Filter = ANISOTROPIC;
	MaxAnisotropy = 4;

	AddressU = WRAP;
	AddressV = WRAP;
};

struct VertexIn
{
	float3 Position : POSITION;
	float2 TexCoord : TEXCOORD;
};

struct VertexOut
{
	float4 PositionH : SV_POSITION;
	float2 TexCoord : TEXCOORD;
};

VertexOut BasicVS(VertexIn vin)
{
	VertexOut vout;

	vout.PosH = mul(float4(vin.Position, 1.0f), gWorldViewProj);
	vout.TexCoord = vin.TexCoord;

	return vout;
}

float4 BasicPS(VertexOut pin) : SV_Target
{
	float4 texColor = gBasicTexture.Sample(samAnisotropic, pin.TexCoord);

	return texColor;
}

technique11 BasicTechnique
{
	pass P0
	{
		SetVertexShader(CompileShader(vs_5_0, BasicVS()));
		SetGeometryShader(NULL);
		SetPixelShader(CompileShader(ps_5_0, BasicPS()));
	};
};