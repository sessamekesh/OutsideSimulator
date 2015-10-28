cbuffer cbPerObject
{
	float4x4 gWorldViewProj;
	float4 gSelectionColor;
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
	float4 SelColor : COLOR;
	float2 TexCoord : TEXCOORD;
};

VertexOut BasicVS(VertexIn vin)
{
	VertexOut vout;

	vout.PositionH = mul(float4(vin.Position, 1.0f), gWorldViewProj);
	vout.TexCoord = vin.TexCoord;
	vout.SelColor = gSelectionColor.rgba;

	return vout;
}

float4 BasicPS(VertexOut pin) : SV_Target
{
	float4 texColor = gBasicTexture.Sample(samAnisotropic, pin.TexCoord);

	if (pin.SelColor.r < 0.01f)
	{
		return texColor;
	}
	else
	{
		return texColor * float4(0.5f, 0.5f, 0.5f, 0.5f) + pin.SelColor * float4(0.5f, 0.5f, 0.5f, 0.5f);
	}
}

technique11 BasicTechnique
{
	pass P0
	{
		SetVertexShader(CompileShader(vs_5_0, BasicVS()));
		SetGeometryShader(NULL);
		SetPixelShader(CompileShader(ps_5_0, BasicPS()));
	}
};