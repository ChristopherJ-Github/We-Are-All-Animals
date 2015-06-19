Shader "Custom/ModeledTreeBarkSnow" {
	Properties {
		_Distance ("Distance", Float) = 0
		_MaxAmount ("Max Amount", Float) = 0
	    _MainTex ("Base (RGB)", 2D) = "black" {}
		_MainBump ("Model Bump", 2D) = "bump" {}
		_SnowColor ("Snow Color", Color) = (0.5, 0.5, 0.5, 1)
	    _SnowOffset ("Snow Offset", Range(0,1) ) = 0
	    
	    _SpecColor ("Specular Color", Color) = (0.5, 0.5, 0.5, 1)
	    _Shininess ("Shininess", Range (0, 1)) = 0.078125
	    _Gloss ("Gloss", Range(0, 1)) = 1
	}

	SubShader {
		Name "Base"
		Tags { "RenderType"="Opaque" }
		LOD 200
		
		CGPROGRAM
		#pragma surface surf BlinnPhong vertex:TreeVertBarkSnow addshadow nolightmap
		#pragma exclude_renderers flash
		#pragma glsl_no_auto_normalization
		#pragma target 3.0 
		
		#include "TerrainEngine.cginc"
		
		void TreeVertBarkSnow (inout appdata_full v) {
			
			v.vertex = Squash(v.vertex);
			
			v.color = float4 (1, 1, 1, v.color.a);
			v.normal = normalize(v.normal);
			v.tangent.xyz = normalize(v.tangent.xyz); 
			
			// add worldnormal to color
		    v.color.g = normalize(mul((float3x3)_Object2World, v.normal)).y; // g - world up vector
		}
				
		struct Input {	
			float3 worldNormal; INTERNAL_DATA
			float2 uv_Control : TEXCOORD0;	
			float2 uv_MainTex;
			fixed4 color : COLOR;
		};
		
		float4 _SnowColor;
		half _SnowOffset;
		sampler2D _MainTex;
		sampler2D _MainBump;
		fixed4 _Color;
		float _Distance;
		float _MaxAmount;
		
		float _snowShininess;
		float _SnowAmount;
		float _SnowStartHeight;
		
		
		void surf (Input IN, inout SurfaceOutput o) { 
			
			fixed4 col = tex2D(_MainTex, IN.uv_MainTex);
			fixed4 bump = tex2D(_MainBump, IN.uv_MainTex);	
			o.Alpha = col.a;
			float maxAmount;
			if (_MaxAmount != 0) {
				maxAmount = _MaxAmount;
			} else {
				float maxDistance = 32.83898;
				float minDistance = 6.042689;
				float distanceNormalized = (_Distance - minDistance) / (maxDistance - minDistance);
				distanceNormalized = clamp(distanceNormalized, 0, 1);
				maxAmount = lerp (0.85, 0.90, distanceNormalized);
			}
			_SnowAmount = lerp(0.48, maxAmount, _SnowAmount);
			// lerp = allows snow even on orthogonal surfaces // (1-col.g) = take the blue channel to get some kind of heightmap // worldNormal is stored in IN.color
			float snowAmount = lerp(_SnowAmount * IN.color.y, 1, _SnowAmount) * (1-col.b) * .65 + o.Normal.y * _SnowAmount *.25;
			
			//float snowAmount = _SnowAmount;

			// clamp snow to _SnowStartHeight
			// billboards do not get effected by snowStartHeight anyway...
			//snowAmount = snowAmount * clamp((IN.worldPos.y - _SnowStartHeight)*.0125, 0, 1);
			
			// sharpen snow mask
			snowAmount = clamp(pow(snowAmount,8)*512, 0, 1);
			
			// mix all together
			o.Gloss = _Color.r * (1-snowAmount) + ((1-_SnowColor.rgb) * snowAmount);
			o.Albedo = (col.rgb  * (1-snowAmount) + _SnowColor.rgb*snowAmount);
			o.Specular = (1-snowAmount) + _snowShininess * snowAmount;
			o.Normal = UnpackNormal(bump);

			//o.Albedo = snowAmount;
			
			// smooth normal
			o.Normal = normalize(lerp(o.Normal, float3(0,0,1), snowAmount*.50));    
		}
		
		ENDCG 
		
	} 
	Dependency "BillboardShader" = "Hidden/TerrainEngine/BillboardTree"
}
