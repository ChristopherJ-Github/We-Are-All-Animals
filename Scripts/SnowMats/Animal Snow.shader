Shader "Custom/Animal Snow" {
	Properties {
		_Color ("Main Color", Color) = (1,1,1,1)
		_MainTex ("Base (RGB) Trans (A)", 2D) = "white" {}
		_BumpMap ("Normalmap", 2D) = "bump" {}
		_Cutoff ("Alpha cutoff", Range(0,1)) = 0.5
		
		_SnowColor ("Snow Color", Color) = (0.5, 0.5, 0.5, 1)
	    _Snow ("Snow Level", Float) = 0
	    _SnowTexture ("Snow Texture", 2D) = "white" {}
	    _SnowTint ("Snow Tint", Float) = 0.4
	    _SnowDirection ("Snow Direction", Vector) = (0,1,0)
	    _Shininess ("Shininess", Range (0.03, 1)) = 0.078125
	}

	SubShader {
		Name "Base"
		Tags {"Queue"="AlphaTest" "IgnoreProjector"="True" "RenderType"="TransparentCutout"}
		LOD 300
			
		CGPROGRAM
		#pragma surface surf Lambert alphatest:_Cutoff

		sampler2D _MainTex;
		sampler2D _BumpMap;
		fixed4 _Color;
		float _SnowTint;
		float _SnowNormalized;

		struct Input {
			float2 uv_MainTex;
			float2 uv_BumpMap;
		};

		void surf (Input IN, inout SurfaceOutput o) {
		
			o.Normal = UnpackNormal(tex2D(_BumpMap, IN.uv_BumpMap));
			fixed4 col = tex2D(_MainTex, IN.uv_MainTex) * _Color;
			o.Alpha = col.a;
			half colWhiteAmount = (col.r + col.g + col.b) /3;
			half4 snowTint = _SnowTint;
		    snowTint.w = 1;
		    half tintAmount = lerp (0, _SnowNormalized, colWhiteAmount);
		    col += snowTint * tintAmount;
			o.Albedo = col.rgb;
			o.Albedo = clamp (col.rgb, 0, 1);
		}
		ENDCG
		
		Name "Snow"
		Tags {"Queue"="AlphaTest" "IgnoreProjector"="True" "RenderType"="TransparentCutout"}
		LOD 300
			
		CGPROGRAM
		#pragma surface surf Lambert decal:blend alphatest:_Cutoff

		sampler2D _MainTex;
		sampler2D _BumpMap;
		sampler2D _SnowTexture;
		fixed4 _Color;
		float4 _SnowColor;
		float _Snow;
		float3 _SnowDirection;
		float _SnowNormalized;

		struct Input {
			float3 worldNormal; INTERNAL_DATA
			float2 uv_MainTex;
			float2 uv_BumpMap;
			float2 uv_SnowTexture;
		};

		void surf (Input IN, inout SurfaceOutput o) {
	
			half4 col = tex2D (_MainTex, IN.uv_MainTex);
			half4 snow = tex2D (_SnowTexture, IN.uv_SnowTexture);
		  	o.Normal = UnpackNormal(tex2D(_BumpMap, IN.uv_BumpMap));
		  	o.Albedo = _SnowColor.rgb;
		  	half colWhiteAmount = (col.r + col.g + col.b) /3;
		  	colWhiteAmount/= 2;
		  	o.Alpha = snow.a * 20 * colWhiteAmount;
		  	o.Alpha *= _SnowNormalized;
		  	o.Alpha = 0;
		}
		ENDCG
	}
	
	FallBack "Transparent/Cutout/Diffuse"
}
