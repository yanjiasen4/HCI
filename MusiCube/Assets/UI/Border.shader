Shader "CustomFX/Border"
{
	Properties
	{
		_Border("Border Width",Range(1,2))=1.2
		_Color("Color", Color) = (1,1,1,1)
		_MainTex ("Texture", 2D) = "white" {}
	}
	SubShader
	{
		Tags { "RenderType"="Transparent" }
		Tags{ "Queue" = "Overlay" }
		LOD 200

		//Cull Off
		ZWrite Off
		
		Lighting Off
		Pass
		{
			

			Name"Cut"
			Blend SrcAlpha OneMinusSrcAlpha, One Zero
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#include "UnityCG.cginc"
			
			struct appdata
			{
				float4 vertex : POSITION;
			};

			struct v2f
			{

				float4 vertex : SV_POSITION;
			};

			v2f vert(appdata v) {
				v2f o;
				o.vertex = mul(UNITY_MATRIX_MVP, v.vertex);
				return o;
			}

			half4 frag(v2f i) :COLOR{
				return half4(0,0,0,0);
			}
			ENDCG
		}
		Pass
		{
			

			Name "Border"
			
			Blend DstAlpha OneMinusDstAlpha  , One Zero
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			// make fog work
			#pragma multi_compile_fog
			
			#include "UnityCG.cginc"

			struct appdata
			{
				float4 vertex : POSITION;
				float3 normal : NORMAL;
				float2 uv : TEXCOORD0;
			};

			struct v2f
			{
				float2 uv : TEXCOORD0;
				UNITY_FOG_COORDS(1)
				float4 vertex : SV_POSITION;
			};

			float4 _Color;
			sampler2D _MainTex;
			float4 _MainTex_ST;
			float _Border;
			v2f vert (appdata v)
			{
				v2f o;
				v.vertex.xyz *= _Border;
				o.vertex = mul(UNITY_MATRIX_MVP, v.vertex);
				o.uv = TRANSFORM_TEX(v.uv, _MainTex);
				
				return o;
			}
			
			fixed4 frag (v2f i) : SV_Target
			{
				// sample the texture
				fixed4 col = tex2D(_MainTex, i.uv);
				return col*_Color;
			}
			ENDCG
		}
		
	}
}
