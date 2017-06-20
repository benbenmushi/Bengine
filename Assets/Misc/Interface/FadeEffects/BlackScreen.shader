// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Hidden/BlackScreen"
{
	Properties
	{
		_MainTex("Texture", 2D) = "" {}
		_Intensity("Intensity", Float) = 1
	}
	SubShader
	{
		// No culling or depth
		Cull Off ZWrite Off ZTest Always

		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			
			#include "UnityCG.cginc"

			struct appdata_t {
				float4 vertex : POSITION;
				fixed4 color : COLOR;
				float2 texcoord : TEXCOORD0;
			};

			struct v2f {
				float4 vertex : POSITION;
				fixed4 color : COLOR;
				float2 texcoord : TEXCOORD0;
			};
			float _Intensity;

			sampler2D _MainTex;
			uniform float4 _MainTex_ST;
			uniform float4 _Color;

			v2f vert(appdata_t v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.color = v.color;
				o.texcoord = TRANSFORM_TEX(v.texcoord, _MainTex);
				return o;
			}

			fixed4 frag(v2f i) : COLOR
			{
				float4 texColor = tex2D(_MainTex, i.texcoord);
				fixed4 output = lerp(texColor, fixed4(0, 0, 0, 0), _Intensity);
				output.a = texColor.a;
				return output;
			}
			ENDCG
		}
	}
}
