Shader "Transparent/Cutout/Bumped Diffuse" {
	Properties {
		_Color ("Main Color", Color) = (1,1,1,1)
		_MainTex ("Base (RGB) Trans (A)", 2D) = "white" {}
		_BumpMap ("Normalmap", 2D) = "bump" {}
		_Cutoff ("Alpha cutoff", Range(0,1)) = 0.5
		
		_SnowColor ("Snow Color", Color) = (0.5, 0.5, 0.5, 1)
	    _Snow ("Snow Level", Range(-7,1) ) = 0
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
		fixed4 _Color;
		float4 _SnowColor;
		float _Snow;
		float _SnowSharpness;
		float3 _SnowDirection;

		struct Input {
			float3 worldNormal; INTERNAL_DATA
			float2 uv_MainTex;
			float2 uv_BumpMap;
		};

		void surf (Input IN, inout SurfaceOutput o) {
	
			half4 col = tex2D (_MainTex, IN.uv_MainTex) * _Color;
		  	o.Normal = UnpackNormal(tex2D(_BumpMap, IN.uv_BumpMap));

		  	fixed snowLerp = dot(o.Normal, _SnowDirection.xyz) + _Snow;
		  	snowLerp = clamp(snowLerp, 0, 1);
		  	snowLerp = pow(snowLerp, _SnowSharpness) * 25;
		  	snowLerp = clamp(snowLerp, 0, 1);
		  	o.Albedo = lerp (col, _SnowColor.rgb * 0.85, snowLerp * 0.5);
		  	o.Alpha = lerp (0, 1, snowLerp);
		  	o.Alpha *= (col.r + col.g + col.b)/3;
		  	o.Alpha *= col.a;
		}
		ENDCG
	}
	
	FallBack "Transparent/Cutout/Diffuse"
}
