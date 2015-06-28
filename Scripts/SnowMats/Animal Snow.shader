Shader "Transparent/Cutout/Bumped Diffuse" {
	Properties {
		_Color ("Main Color", Color) = (1,1,1,1)
		_MainTex ("Base (RGB) Trans (A)", 2D) = "white" {}
		_BumpMap ("Normalmap", 2D) = "bump" {}
		_Cutoff ("Alpha cutoff", Range(0,1)) = 0.5
		
		_SnowColor ("Snow Color", Color) = (0.5, 0.5, 0.5, 1)
	    _Snow ("Snow Level", Float) = 0
	    _SnowTexture ("Snow Texture", 2D) = "white" {}
	    _SnowTint ("Snow Tint", Float) = 0.4
	    _SnowSharpness ("Snow Sharpness", Float) = 3
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
			fixed4 col = tex2D(_MainTex, IN.uv_MainTex) * _Color;
			//half4 snowTint = _SnowTint;
		    //snowTint.w = 1;
		    //half tintAmount = _SnowNormalized * 0.7;
		    //col = lerp(col, snowTint, tintAmount);
			o.Albedo = col.rgb;
			o.Alpha = col.a;
			o.Normal = UnpackNormal(tex2D(_BumpMap, IN.uv_BumpMap));
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
		float _SnowSharpness;
		float3 _SnowDirection;

		struct Input {
			float3 worldNormal; INTERNAL_DATA
			float2 uv_MainTex;
			float2 uv_BumpMap;
			float2 uv_SnowTexture;
		};

		void surf (Input IN, inout SurfaceOutput o) {
	
			half4 col = tex2D (_MainTex, IN.uv_MainTex) * _Color;
			half4 snow = tex2D (_SnowTexture, IN.uv_SnowTexture);
		  	o.Normal = UnpackNormal(tex2D(_BumpMap, IN.uv_BumpMap));
		  	o.Albedo = snow.rgb;
		  	half snowWhiteAmount = (snow.r + snow.g + snow.b)/3;
		  	o.Alpha = col.a * snow.a * snowWhiteAmount;
		}
		ENDCG
	}
	
	FallBack "Transparent/Cutout/Diffuse"
}
