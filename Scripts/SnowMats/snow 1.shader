Shader "Custom/snow1" {
	Properties {
	    _MainTex ("Base (RGB)", 2D) = "black" {}
		_MainBump ("Model Bump", 2D) = "bump" {}
		_SnowColor ("Snow Color", Color) = (0.5, 0.5, 0.5, 1)
	    _Snow ("Snow Level", Range(-7,1) ) = 0
	    _SnowTint ("Snow Tint", Float) = 0.4
	    _SnowSharpness ("Snow Sharpness", Float) = 3
	    _SnowDirection ("Snow Direction", Vector) = (0,1,0)
	    _Shininess ("Shininess", Range (0.03, 1)) = 0.078125
	}

	SubShader {
		Name "Base"
		Tags { "RenderType"="Opaque" }
		LOD 200
		
		CGPROGRAM
		#pragma surface surf BlinnPhong
		#pragma target 3.0 
		
		sampler2D _MainTex;
		sampler2D _MainBump;
		half _Shininess;
		float _SnowNormalized;
		float _SnowTint;
		
		struct Input {
			float2 uv_MainTex; 
			float2 uv_MainBump;  
			float3 worldNormal; INTERNAL_DATA
		};
 
		void surf (Input IN, inout SurfaceOutput o) {
		
		    half4 col = tex2D (_MainTex, IN.uv_MainTex);
		    half4 snowTint = _SnowTint;
		    snowTint.w = 1;
		    half tintAmount = _SnowNormalized * 0.7;
		    col = lerp(col, snowTint, tintAmount);
		    o.Albedo = col.rgb; 
		    o.Alpha = col.a;
		    o.Gloss = col.a;
		    o.Specular = _Shininess;
		    o.Normal = UnpackNormal(tex2D(_MainBump, IN.uv_MainBump));	
		}	
		ENDCG
		
		Name "Snow"

		Tags {
			"SplatCount" = "4"
			"Queue" = "Geometry-100"
			"RenderType" = "Transparent"
		}
		
		CGPROGRAM
		#pragma surface surf Lambert decal:blend
		#pragma target 3.0 
		#pragma glsl 
				
		struct Input {	
			float3 worldNormal; INTERNAL_DATA
			float2 uv_Control : TEXCOORD0;
			float2 uv_MainTex;
			float2 uv_MainBump;
			fixed4 color : COLOR0;
		};
		
		float4 _SnowColor;
		float _Snow;
		float _SnowSharpness;
		float3 _SnowDirection;
		sampler2D _MainTex;
		sampler2D _MainBump;
		
		void surf (Input IN, inout SurfaceOutput o) { 
			
			half4 originalColor = tex2D (_MainTex, IN.uv_MainTex);
		  	o.Normal = UnpackNormal(tex2D(_MainBump, IN.uv_MainBump));

		  	fixed snowLerp = dot(WorldNormalVector(IN, o.Normal), _SnowDirection.xyz) + _Snow;
		  	snowLerp = clamp(snowLerp, 0, 1);
		  	snowLerp = pow(snowLerp, _SnowSharpness) * 25;
		  	snowLerp = clamp(snowLerp, 0, 1);
		  	o.Albedo = lerp (originalColor, _SnowColor.rgb * 0.85, snowLerp);
		  	o.Alpha = lerp (0, 1, snowLerp);	    
		}
		ENDCG 
		
	} 
	FallBack "Diffuse"
}
