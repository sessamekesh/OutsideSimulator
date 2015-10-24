cbuffer cbPerObject
{
	float4x4 gWorldViewProj;
};

struct VertexIn
{
	float3 Position : POSITION;
	float4 Color : COLOR;
};

struct VertexOut
{
	float4 PositionH : SV_POSITION;
	float4 Color : COLOR;
};

VertexOut TestVertexShader(VertexIn vin)
{
	VertexOut vout;

	vout.PositionH = mul(float4(vin.Position, 1.0f), gWorldViewProj);
	vout.Color = vin.Color;

	return vout;
}

float4 TestPixelShader(VertexOut pin) : SV_Target
{
	return pin.Color;
}

technique11 TestTechnique
{
	pass P0
	{
		SetVertexShader(CompileShader(vs_4_0, TestVertexShader()));
		SetGeometryShader(NULL);
		SetPixelShader(CompileShader(ps_4_0, TestPixelShader()));
	}
};