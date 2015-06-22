Shader "Custom/BarkMaterial" {
	Properties {
		_MainTex ("Base (RGB)", 2D) = "white" {}
		_Bump ("Bump (GBA)", 2D) = "bump" {}  
	}
	SubShader {
		Tags { "RenderType"="Opaque" }
		LOD 200
		
		CGPROGRAM
		#pragma surface surf Lambert

		sampler2D _MainTex;
		sampler2D _Bump;

		struct Input {
			float2 uv_MainTex;
		};

		void surf (Input IN, inout SurfaceOutput o) {
			fixed4 control = tex2D(_Bump, IN.uv_MainTex);
			fixed3 nrml;
			
			half4 c = tex2D (_MainTex, IN.uv_MainTex);
			nrml.r = control.g;
			nrml.g = control.b;
			nrml.b = control.a;
			nrml.a = 1.0;
			
			o.Normal = UnpackNormal(nrml);
			o.Albedo = c.rgb;
			o.Alpha = c.a;
		}
		ENDCG
	} 
	FallBack "Diffuse"
}
