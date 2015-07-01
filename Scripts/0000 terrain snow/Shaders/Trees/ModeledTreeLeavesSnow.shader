Shader "Custom/ModeledTreeLeavesSnow" {
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
		Tags {
			"Queue" = "Transparent-99"
			"IgnoreProjector"="True"
			"RenderType" = "TreeTransparentCutout"
		}
		Cull Off
		ColorMask RGB
		
		Pass {
			Lighting On
		
			CGPROGRAM
			// Upgrade NOTE: excluded shader from DX11 and Xbox360; has structs without semantics (struct v2f members worldPos)
			#pragma exclude_renderers d3d11 xbox360
			#pragma vertex leaves
			#pragma fragment frag 
			#pragma glsl_no_auto_normalization
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
				output.rgb *= input.color.rgb;
				output.a = col.a;
				return output;
			}
			ENDCG
		}
		
		Pass {
			Name "ShadowCaster"
			Tags { "LightMode" = "ShadowCaster" }
			
			Fog {Mode Off}
			ZWrite On ZTest LEqual Cull Off
			Offset 1, 1
	
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma glsl_no_auto_normalization
			#pragma multi_compile_shadowcaster
			#include "UnityCG.cginc"
			#include "TerrainEngine.cginc"
			
			struct v2f { 
				V2F_SHADOW_CASTER;
				float2 uv : TEXCOORD1;
			};
			
			struct appdata {
			    float4 vertex : POSITION;
			    fixed4 color : COLOR;
			    float4 texcoord : TEXCOORD0;
			};
			v2f vert( appdata v )
			{
				v2f o;
				TerrainAnimateTree(v.vertex, v.color.w);
				TRANSFER_SHADOW_CASTER(o)
				o.uv = v.texcoord;
				return o;
			}
			
			sampler2D _MainTex;
			fixed _Cutoff;
					
			float4 frag( v2f i ) : SV_Target
			{
				fixed4 texcol = tex2D( _MainTex, i.uv );
				clip( texcol.a - _Cutoff );
				SHADOW_CASTER_FRAGMENT(i)
			}
			ENDCG	
		}
	}
	
	// This subshader is never actually used, but is only kept so
	// that the tree mesh still assumes that normals are needed
	// at build time (due to Lighting On in the pass). The subshader
	// above does not actually use normals, so they are stripped out.
	// We want to keep normals for backwards compatibility with Unity 4.2
	// and earlier.
	SubShader {
		Tags {
			"Queue" = "Transparent-99"
			"IgnoreProjector"="True"
			"RenderType" = "TransparentCutout"
		}
		Cull Off
		ColorMask RGB
		Pass {
			Tags { "LightMode" = "Vertex" }
			AlphaTest GEqual [_Cutoff]
			Lighting On
			Material {
				Diffuse [_Color]
				Ambient [_Color]
			}
			SetTexture [_MainTex] { combine primary * texture DOUBLE, texture }
		}		
	}

	Dependency "BillboardShader" = "Hidden/Nature/Modeled Tree Leaves Rendertex Snow"
	Fallback Off
}
