Shader "Hidden/TerrainEngine/Details/BillboardWavingDoublePass" {
	Properties {
		_WavingTint ("Fade Color", Color) = (.7,.6,.5, 0)
		_MainTex ("Base (RGB) Alpha (A)", 2D) = "white" {}
		_WaveAndDistance ("Wave and distance", Vector) = (12, 3.6, 1, 1)
		_Cutoff ("Cutoff", float) = 0.5
	}
	
CGINCLUDE
#include "UnityCG.cginc"
#include "TerrainEngine.cginc"
#pragma glsl_no_auto_normalization

struct v2f {
	float4 pos : POSITION;
	fixed4 color : COLOR;
	float4 uv : TEXCOORD0;
};
v2f BillboardVert (appdata_full v) {
	v2f o;
	WavingGrassBillboardVert (v);
	o.color = v.color;
	
	o.color.rgb *= ShadeVertexLights (v.vertex, v.normal);
		
	o.pos = mul (UNITY_MATRIX_MVP, v.vertex);	
	o.uv = v.texcoord;
	return o;
}
ENDCG

	SubShader {
		Tags {
			"Queue" = "Geometry+200"
			"IgnoreProjector"="True"
			"RenderType"="GrassBillboard"
		}
		Cull Off
		LOD 200
				
CGPROGRAM
#pragma surface surf Lambert vertex:WavingGrassBillboardVert addshadow
#pragma exclude_renderers flash
			
sampler2D _MainTex;
// will be passed bey terrainscript
sampler2D _SnowTexture;
fixed _Cutoff;

//snow
half4 _GrassTint;
float _Saturation;
float _SnowAmount;
float _SnowStartHeight;

struct Input {
	float2 uv_MainTex;
	fixed4 color : COLOR;
	float3 worldPos;
};

void surf (Input IN, inout SurfaceOutput o) {
	fixed4 col = tex2D(_MainTex, IN.uv_MainTex);
	
	// get snow texture
	half3 snow = tex2D( _SnowTexture, IN.uv_MainTex).rgb;
	
	// (1-col.g) = take the blue channel to get some kind of heightmap...
	float snowAmount = _SnowAmount * (1-clamp(col.g*1.5, 0, 1)) * 1.6;
	// clamp snow to _SnowStartHeight
	snowAmount *= clamp((IN.worldPos.y - _SnowStartHeight)*.0125, 0, 1);
	// sharpen snow mask
	snowAmount = clamp(pow(snowAmount*(1-(IN.color.a*.25)),8)*256, 0, 1);
	
	// mix diffuse and snow and add terrain lighting
	col *= _GrassTint;
	half4 grayscale = (col.r + col.g + col.b)/3.0f;
	grayscale.a = col.a;
	col = lerp (grayscale, col, _Saturation);
	half4 snowTint1 = lerp (half4(1.0f, 1.0f, 1.0f, 1.0f),  half4(0.5f, 0.5f, 0.5f, 1.0f),  _SnowAmount);
	col *= snowTint1;
	half4 snowTint2 = half4(0.35f, 0.35f, 0.35f, 0.0f) * _SnowAmount;
	col += snowTint2;
	o.Albedo = (col.rgb * (1-snowAmount) + snow.rgb*snowAmount);
	o.Alpha = col.a;
	clip (o.Alpha - _Cutoff);
	o.Alpha *= IN.color.a;
}

ENDCG			
}
}
