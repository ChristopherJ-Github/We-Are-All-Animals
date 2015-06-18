Shader "Custom/snow1" {
	Properties {
	    _MainTex ("Base (RGB)", 2D) = "black" {}
		_MainBump ("Model Bump", 2D) = "bump" {}
		_SnowColor ("Snow Color", Color) = (0.5, 0.5, 0.5, 1)
	    _Snow ("Snow Level", Range(-7,1) ) = 0
	    _SnowDirection ("Snow Direction", Vector) = (0,1,0)
	    
	    _Perlin ("Perlin Noise", 2D) = "black" {}
	    _GrassTex ("GrassBase (RGB)", 2D) = "green" {}
	    _Grass ("Grass Level", Range(-1,1) ) = 0
	    
	    _TestTex ("Test Text", 2D) = "red" {}
	    _SpecColor ("Specular Color", Color) = (0.5, 0.5, 0.5, 1)
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
		float4 _SnowColor;
		float _Snow;
		float3 _SnowDirection;
		half _Shininess;
		
		struct Input {
			float2 uv_MainTex; 
			float2 uv_MainBump;  
			float3 worldNormal; INTERNAL_DATA
		};
 
		void surf (Input IN, inout SurfaceOutput o) {
		
		    half4 col = tex2D (_MainTex, IN.uv_MainTex);

		    o.Albedo = col.rgb; 
		    o.Alpha = col.a;
		    o.Gloss = col.a;
		    o.Specular = _Shininess;

		    o.Normal = UnpackNormal(tex2D(_MainBump, IN.uv_MainBump));	
		}
		
		ENDCG
		
		Name "Grass"
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
			float2 uv_Perlin;			
			float2 uv_TestTex;
		};
		
		sampler2D _Perlin;
		sampler2D _TestTex;
		half _Grass;

		void surf (Input IN, inout SurfaceOutput o) { 
				
			fixed4 perlin_control = tex2D (_Perlin, IN.uv_Perlin);
			perlin_control += _Grass;
			o.Albedo = tex2D(_TestTex, IN.uv_TestTex).rgb;
			o.Alpha = perlin_control.r;
		    
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
			float2 uv_MainBump;
			fixed4 color : COLOR0;
		};
		
		float4 _SnowColor;
		float _Snow;
		float3 _SnowDirection;
		sampler2D _MainBump;
		

		void surf (Input IN, inout SurfaceOutput o) { 
				
		  	o.Normal = UnpackNormal(tex2D(_MainBump, IN.uv_MainBump));
		  	fixed snowLerp  = dot(WorldNormalVector(IN, o.Normal), _SnowDirection.xyz) + _Snow;
		  	o.Albedo = lerp (o.Albedo, _SnowColor.rgb, snowLerp);
		  	o.Alpha = lerp (0, 1, snowLerp);	    
		}
		
		ENDCG 
		
	} 
	FallBack "Diffuse"
}
