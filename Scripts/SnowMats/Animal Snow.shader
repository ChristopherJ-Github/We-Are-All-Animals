Shader "Custom/Animal Snow" {
	Properties {
		_Color ("Main Color", Color) = (1,1,1,1)
		_MainTex ("Base (RGB) Trans (A)", 2D) = "white" {}
		_BumpMap ("Normalmap", 2D) = "bump" {}
		_Cutoff ("Alpha cutoff", Range(0,1)) = 0.5
		
	    _Snow ("Snow Level", Float) = 0
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
			half colWhiteAmount = (col.r + col.g + col.b) /3;
			half4 snowTint = 0.87;
		    snowTint.w = 1;
		    half snowNormOffset = 0.2;
		    half snowNormOffseted = (_SnowNormalized - snowNormOffset) / (1 - snowNormOffset);
		    snowNormOffseted = clamp (snowNormOffseted, 0, 1);
		    						
			half tintAmount = colWhiteAmount;
			half3 colAfterTint = lerp (col, snowTint, snowNormOffseted * tintAmount);
			
		    half snowAmount = lerp (0, snowNormOffseted, colWhiteAmount);
		    half3 colAfterSnow = colAfterTint + (snowTint * snowAmount);
		    
			o.Albedo = colAfterSnow;
			o.Albedo = clamp (o.Albedo, 0, 1);
			o.Alpha = col.a;
			o.Normal = UnpackNormal(tex2D(_BumpMap, IN.uv_BumpMap));
		}
		ENDCG
	}
	
	FallBack "Transparent/Cutout/Diffuse"
}
