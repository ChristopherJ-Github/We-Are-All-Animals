Shader "Hidden/Nature/Modeled Tree Leaves Rendertex Snow" {
	Properties {
		_Color ("Main Color", Color) = (1,1,1,1)
		_MainTex ("Main Texture", 2D) = "white" {  }
		_MainBump ("Model Bump", 2D) = "bump" {}
		_Cutoff ("Alpha cutoff", Range(0.25,0.9)) = 0.5
		_BaseLight ("Base Light", Range(0, 1)) = 0.35
		_AO ("Amb. Occlusion", Range(0, 10)) = 2.4
		_Occlusion ("Dir Occlusion", Range(0, 20)) = 7.5
		
		_SnowColor ("Snow Color", Color) = (0.5, 0.5, 0.5, 1)
		
		// These are here only to provide default values
		_Scale ("Scale", Vector) = (1,1,1,1)
		_SquashAmount ("Squash", Float) = 1
	}
	SubShader { 

		Tags { "Queue" = "Transparent-99" }
		Cull Off
		Fog { Mode Off}
		
		Pass {
			Lighting On
			ZWrite On

			CGPROGRAM
			#pragma vertex leaves
			#pragma fragment frag
			#pragma glsl_no_auto_normalization
			#define USE_CUSTOM_LIGHT_DIR 1
			#include "SH_Vertex.cginc"
			
			sampler2D _MainTex;
			sampler2D _MainBump;
			fixed _Cutoff;
			float4 _SnowColor;
			
			float _snowShininess;
			float _SnowAmount;
			float _SnowNormalized;
			float _SnowStartHeight;
			
			fixed4 frag(v2f input) : SV_Target
			{
				half tint = lerp(0, 0.15, _SnowAmount);
				_SnowAmount = lerp(0.2, 0.54, _SnowAmount);
				_SnowAmount = 1 - _SnowAmount;
				_SnowStartHeight = -100.6;
				
				fixed4 output;
				fixed4 col = tex2D( _MainTex, input.uv.xy);
				fixed3 snowTint = 1;
				float snowInfluence = 0.13;
				col.rgb = lerp(col.rgb, snowTint, _SnowNormalized * snowInfluence);
				fixed4 bump = tex2D(_MainBump, input.uv.xy);	
				
				clip (col.a - _Cutoff);
				
				// lerp = allows snow even on orthogonal surfaces // (1-col.g) = take the blue channel to get some kind of heightmap // worldNormal is stored in IN.color
				float snowAmount = lerp(_SnowAmount, 1, _SnowAmount) * (1-col.g) * .65 + input.Normal.y * _SnowAmount *.25;
				// clamp snow to _SnowStartHeight
				// billboards do not get effected by snowStartHeight anyway...
				snowAmount = snowAmount * clamp((input.worldPos.y - _SnowStartHeight)*.0125, 0, 1);
				// sharpen snow mask
				snowAmount = 1 - clamp( pow(snowAmount,6)*256, 0, 1);
				
				output.rgb = (col.rgb * (1-snowAmount) + snowTint * snowAmount);
				//output.rgb *= input.color.rgb * 2;
				output.rgb += tint;
				output.a = 1;
				return output;
			}
			ENDCG
		}
	}
	
	Fallback Off
}
