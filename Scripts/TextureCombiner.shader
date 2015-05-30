Shader "Custom/TextureCombiner" {
	Properties {
		_InputTex1 ("Texture 1", 2D) = "red" {}
		_InputTex2 ("Texture 2", 2D) = "blue" {}
		_Blend ("Blend", Range(0,1) ) = 0
		_OutputTex ("Base (RGB)", 2D) = "black" {}
	}
	SubShader {
		Tags { "RenderType"="Opaque" }
		LOD 200
		
		CGPROGRAM
		#pragma surface surf Lambert

		sampler2D _InputTex1;
		sampler2D _InputTex2;
		sampler2D _OutputTex;
		half _Blend;

		struct Input {
			float2 uv_InputTex1;
		};

		void surf (Input IN, inout SurfaceOutput o) {
			half input1 = tex2D (_InputTex1, IN.uv_InputTex1);
			half input2 = tex2D (_InputTex2, IN.uv_InputTex1);
			half output = lerp(input1, input2, _Blend);
			o.Albedo = output;
			//o.Alpha = c.a;
		}
		ENDCG
	} 
	FallBack "Diffuse"
}
