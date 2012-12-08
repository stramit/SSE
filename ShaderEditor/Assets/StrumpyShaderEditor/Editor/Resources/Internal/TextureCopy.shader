// Upgrade NOTE: replaced 'glstate.matrix.texture[0]' with 'UNITY_MATRIX_TEXTURE0'

Shader "Hidden/TextureCopy"
{
	Properties 
	{
		_MainTex("Main Texture", 2D) = "white" {}
	}
	
	SubShader
	{
	Tags
		{
			"Queue"="Background-999"
		}
		Pass
		{
			ZTest Always 
			Cull Off 
			ZWrite Off
			Fog { Mode off }
			Blend Off
		
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma fragmentoption ARB_precision_hint_fastest 
			#include "UnityCG.cginc"
			
			uniform float4 _MainTex_TexelSize;
			v2f_img vert(appdata_img v)
			{
				v2f_img o;
				o.pos = v.vertex;
				o.uv = MultiplyUV (UNITY_MATRIX_TEXTURE0, v.texcoord);				
				return o;
			}	
			
			uniform sampler2D _MainTex;
			float4 frag (v2f_img i) : COLOR
			{	
				return tex2D(_MainTex, i.uv);
			}
			ENDCG
		}
	}
	
	Fallback off
}