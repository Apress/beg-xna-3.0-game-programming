// -------------------------------------------------
// Matrices
// -------------------------------------------------
float4x4 matW : World;
float4x4 matVI : ViewInverse;
float4x4 matWVP : WorldViewProjection;

// Materials
// -------------------------------------------------
float3 diffuseColor;
float3 specularColor;
float specularPower;

// Lights
// -------------------------------------------------
float3 ambientLightColor;
float3 light1Position;
float3 light1Color;
float3 light2Position;
float3 light2Color;

// UV Tiles: 0-4 Diffuse textures
float2 uv1Tile;
float2 uv2Tile;
float2 uv3Tile;
float2 uv4Tile;
float2 uvNormalTile;

// Textures
// -------------------------------------------------
texture diffuseTexture1;
texture diffuseTexture2;
texture diffuseTexture3;
texture diffuseTexture4;
texture alphaTexture;
texture normalTexture;

sampler2D diffuseSampler1 = sampler_state {
    texture = <diffuseTexture1>;
    MagFilter = Linear;
    MinFilter = Linear;
    MipFilter = Linear;
    AddressU = Wrap;
    AddressV = Wrap;
};

sampler2D diffuseSampler2 = sampler_state {
    texture = <diffuseTexture2>;
    MagFilter = Linear;
    MinFilter = Linear;
    MipFilter = Linear;
    AddressU = Wrap;
    AddressV = Wrap;
};

sampler2D diffuseSampler3 = sampler_state {
    texture = <diffuseTexture3>;
    MagFilter = Linear;
    MinFilter = Linear;
    MipFilter = Linear;
    AddressU = Wrap;
    AddressV = Wrap;
};

sampler2D diffuseSampler4 = sampler_state {
    texture = <diffuseTexture4>;
    MagFilter = Linear;
    MinFilter = Linear;
    MipFilter = Linear;
    AddressU = Wrap;
    AddressV = Wrap;
};

sampler2D alphaSampler = sampler_state {
	Texture = <alphaTexture>;
	MinFilter = Linear;
	MagFilter = Linear;
	MipFilter = Linear;
	AddressU = Wrap;
	AddressV = Wrap;
};

sampler2D normalSampler = sampler_state {
	Texture = <normalTexture>;
	MinFilter = linear;
	MagFilter = linear;
	MipFilter = linear;
	AddressU = Wrap;
	AddressV = Wrap;
};

struct a2v
{
    float4 position : POSITION;
    float2 uv0      : TEXCOORD0;
    float3 tangent  : TANGENT;
    float3 binormal : BINORMAL;
    float3 normal   : NORMAL;
};

struct v2f
{
    float4 hposition	: POSITION;
    float4 uv1_2		: TEXCOORD0;
    float4 uv3_4		: TEXCOORD1;
    float4 uv5_6		: TEXCOORD2;
    float3 eyeVec		: TEXCOORD4;
    float3 lightVec1	: TEXCOORD5;
    float3 lightVec2	: TEXCOORD6;    
};
 
v2f TerrainVS(a2v IN)
{
	v2f OUT;	
    OUT.hposition = mul(IN.position, matWVP);

    float3x3 tangentSpace = float3x3(IN.tangent, IN.binormal, IN.normal);
    tangentSpace = mul(tangentSpace, matW);
    tangentSpace = transpose(tangentSpace);
    
    // Light vectors
    float3 worldPosition = mul(IN.position, matW).xyz;
    OUT.eyeVec = mul(matVI[3].xyz - worldPosition, tangentSpace);
    OUT.lightVec1 = mul(light1Position - worldPosition, tangentSpace);
    OUT.lightVec2 = mul(light2Position - worldPosition, tangentSpace);
    
    // Multitexturing
    OUT.uv1_2 = float4(IN.uv0 * uv1Tile, IN.uv0 * uv2Tile);
    OUT.uv3_4 = float4(IN.uv0 * uv3Tile, IN.uv0 * uv4Tile);
    OUT.uv5_6 = float4(IN.uv0, IN.uv0 * uvNormalTile);
    
    return OUT;
}

void phongShading(in float3 normal, in float3 lightVec, in float3 halfwayVec, in float3 lightColor, out float3 diffuseColor, out float3 specularColor)
{
	float diffuseInt = saturate(dot(normal, lightVec));
	diffuseColor = diffuseInt * lightColor;
	float specularInt = saturate(dot(normal, halfwayVec));
	specularInt = pow(specularInt, specularPower);
	specularColor = specularInt * lightColor;
}

float4 TerrainPS(v2f IN) : COLOR0
{
	float3 eyeVec = normalize(IN.eyeVec);
	float3 lightVec1 = normalize(IN.lightVec1);
	float3 lightVec2 = normalize(IN.lightVec2);
	float3 halfwayVec1 = normalize(lightVec1 + eyeVec);
	float3 halfwayVec2 = normalize(lightVec2 + eyeVec);

	float3 normal = tex2D(normalSampler, IN.uv5_6.zw);
	normal.xy = normal.xy * 2.0 - 1.0;
	normal.z = sqrt(1.0 - dot(normal.xy, normal.xy));
    
	float3 color1 = tex2D(diffuseSampler1, IN.uv1_2.xy);
	float3 color2 = tex2D(diffuseSampler2, IN.uv1_2.zw);
	float3 color3 = tex2D(diffuseSampler3, IN.uv3_4.xy);
	float3 color4 = tex2D(diffuseSampler4, IN.uv3_4.zw);
	float4 alpha = tex2D(alphaSampler, IN.uv5_6.xy);
	
	float3 combinedColor = lerp(color1, color2, alpha.x);
	combinedColor = lerp(combinedColor , color3, alpha.y);
	combinedColor = lerp(combinedColor , color4, alpha.z);
	
	// Calculate diffuse and specular color for each light        
	float3 diffuseColor1, diffuseColor2;
	float3 specularColor1, specularColor2;
	phongShading(normal, lightVec1, halfwayVec1, light1Color, diffuseColor1, specularColor1);
	phongShading(normal, lightVec2, halfwayVec2, light2Color, diffuseColor2, specularColor2);

    // Phong lighting result    
    float4 finalColor;
    finalColor.a = 1.0f;
    finalColor.rgb = combinedColor * 
		( (diffuseColor1 + diffuseColor2) * diffuseColor + ambientLightColor) + 
		(specularColor1 + specularColor2) * specularColor ;
    
    return finalColor;
}

technique Terrain
 {  
	pass p0
	{
		VertexShader = compile vs_2_0 TerrainVS();
		PixelShader = compile ps_2_0 TerrainPS();
	}	
}