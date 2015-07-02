Shader "Custom/SnowTerrain" {
	Properties {
		[HideInInspector] _Control ("Control (RGBA)", 2D) = "red" {}
		[HideInInspector] _Splat3 ("Layer 3 (A)", 2D) = "white" {}
		[HideInInspector] _Splat2 ("Layer 2 (B)", 2D) = "white" {}
		[HideInInspector] _Splat1 ("Layer 1 (G)", 2D) = "white" {}
		[HideInInspector] _Splat0 ("Layer 0 (R)", 2D) = "white" {}
		[HideInInspector] _Normal3 ("Normal 3 (A)", 2D) = "bump" {}
		[HideInInspector] _Normal2 ("Normal 2 (B)", 2D) = "bump" {}
		[HideInInspector] _Normal1 ("Normal 1 (G)", 2D) = "bump" {}
		[HideInInspector] _Normal0 ("Normal 0 (R)", 2D) = "bump" {}
		// used in fallback on old cards & base map
		[HideInInspector] _MainTex ("BaseMap (RGB)", 2D) = "white" {}
		
		_SpecColor ("Specular Color", Color) = (1, 1, 1, 1)
		_SnowColor ("Snow Color", Color) = (0.5, 0.5, 0.5, 1)
		_Shininess ("Shininess", Range (0, 10)) = 0.078125
	    _Snow ("Snow Level", Range(-10,1) ) = 0
	    _SnowDirection ("Snow Direction", Vector) = (0,1,0)

	}
		
	SubShader {
	
		//=============main=================
		Tags {
			"SplatCount" = "4"
			"Queue" = "Geometry-100"
			"RenderType" = "Opaque"
		}
		CGPROGRAM
		#pragma surface surf BlinnPhong vertex:vert
		#pragma target 3.0 
		#pragma glsl 
				
		struct Input {
			float2 uv_Control : TEXCOORD0;
			float2 uv_Splat0 : TEXCOORD1;
			float2 uv_Splat1 : TEXCOORD2;
			float2 uv_Splat2 : TEXCOORD3;
			float2 uv_Splat3 : TEXCOORD4;
		};

		sampler2D _Control;
		sampler2D _Splat0,_Splat1,_Splat2,_Splat3;
		sampler2D _Normal0,_Normal1,_Normal2,_Normal3;
		half _Shininess;
		
		void vert (inout appdata_full v) {
			v.tangent.xyz = cross(v.normal, float3(0,0,1));
			v.tangent.w = 1;
		}

		void surf (Input IN, inout SurfaceOutput o) {
			
			fixed4 splat_control = tex2D (_Control, IN.uv_Control);
			fixed4 col;
			col  = splat_control.r * tex2D (_Splat0, IN.uv_Splat0);
			col += splat_control.g * tex2D (_Splat1, IN.uv_Splat1);
			col += splat_control.b * tex2D (_Splat2, IN.uv_Splat2);
			col += splat_control.a * tex2D (_Splat3, IN.uv_Splat3);	
			o.Albedo = col.rgb;
			
			fixed4 nrm;
			nrm  = splat_control.r * tex2D (_Normal0, IN.uv_Splat0);
			nrm += splat_control.g * tex2D (_Normal1, IN.uv_Splat1);
			nrm += splat_control.b * tex2D (_Normal2, IN.uv_Splat2);
			nrm += splat_control.a * tex2D (_Normal3, IN.uv_Splat3);
			// Sum of our four splat weights might not sum up to 1, in
			// case of more than 4 total splat maps. Need to lerp towards
			// "flat normal" in that case.
			fixed splatSum = dot(splat_control, fixed4(1,1,1,1));
			fixed4 flatNormal = fixed4(0.5,0.5,1,0.5); // this is "flat normal" in both DXT5nm and xyz*2-1 cases
			nrm = lerp(flatNormal, nrm, splatSum);
			o.Normal = UnpackNormal(nrm);

			o.Gloss = col.a * splatSum;
			o.Specular = _Shininess;
			
			o.Alpha = 1.0;
			
		}
		ENDCG  
		
		//===============snow================
		Tags {
			"SplatCount" = "4"
			"Queue" = "Geometry-100"
			"RenderType" = "Transparent"
		}
		
		CGPROGRAM
		#pragma surface surf Lambert vertex:vert decal:blend
		#pragma target 3.0 
		#pragma glsl 
				
		struct Input {
			INTERNAL_DATA	
			float3 worldNormal; 
			float2 uv_Control : TEXCOORD0;
			float2 uv_Splat0 : TEXCOORD1;
			float2 uv_Splat1 : TEXCOORD2;
			float2 uv_Splat2 : TEXCOORD3;
			float2 uv_Splat3 : TEXCOORD4;
		};

		float _Snow;
		float4 _SnowColor;
		float4 _SnowDirection;
		sampler2D _Control;
		sampler2D _Splat0,_Splat1,_Splat2,_Splat3;
		sampler2D _Normal0,_Normal1,_Normal2,_Normal3;
		
		void vert (inout appdata_full v) {
			v.tangent.xyz = cross(v.normal, float3(0,0,1));
			v.tangent.w = 1;
		}
		
		void surf (Input IN, inout SurfaceOutput o) { 
			
			fixed4 splat_control = tex2D (_Control, IN.uv_Control);
			
			fixed4 nrm;
			nrm  = splat_control.r * tex2D (_Normal0, IN.uv_Splat0);
			nrm += splat_control.g * tex2D (_Normal1, IN.uv_Splat1);
			nrm += splat_control.b * tex2D (_Normal2, IN.uv_Splat2);
			nrm += splat_control.a * tex2D (_Normal3, IN.uv_Splat3);
			// Sum of our four splat weights might not sum up to 1, in
			// case of more than 4 total splat maps. Need to lerp towards
			// "flat normal" in that case.
			fixed splatSum = dot(splat_control, fixed4(1,1,1,1));
			fixed4 flatNormal = fixed4(0.5,0.5,1,0.5); // this is "flat normal" in both DXT5nm and xyz*2-1 cases
			nrm = lerp(flatNormal, nrm, splatSum);
			nrm = normalize(nrm);
			o.Normal = UnpackNormal(nrm);
			fixed3 worldNormal;
			
			worldNormal = WorldNormalVector(IN, o.Normal);
			worldNormal = normalize(worldNormal);
			float dotProduct = dot(worldNormal , _SnowDirection.xyz);
			dotProduct = clamp(dotProduct, 0, 1);
			fixed snowLerp  = dotProduct + _Snow;
			
			snowLerp = pow(snowLerp, 6) * 25;
			snowLerp = clamp(snowLerp, 0, 1);
			o.Albedo = _SnowColor.rgb * 0.85;
			float snowAlpha = splat_control.r + splat_control.g + splat_control.b + splat_control.a;
			o.Alpha = lerp (0, snowAlpha, snowLerp);
			
			/*
			o.Alpha = 1;
			if (dotProduct < 0 || dotProduct > 1) {
				o.Albedo = fixed3(1,0,0);
			}
			*/
		}
		ENDCG  
	}

	Dependency "AddPassShader" = "Custom/SnowTerrainAdd"
	Dependency "BaseMapShader" = "Specular"

	// Fallback to Diffuse
	Fallback "Nature/Terrain/Diffuse"
}
