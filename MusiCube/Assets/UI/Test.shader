﻿Shader "Custom/Skybox" {
	Properties{
		_Color("Color", Color) = (1,1,1,1)
		_MainTex("Albedo (RGB)", 2D) = "white" {}
	}
	SubShader{
		Tags{ "RenderType" = "Background"
		"Queue" = "Background" }
		//LOD 200
		Cull Off
		CGPROGRAM
#pragma vertex vert
		// Physically based Standard lighting model, and enable shadows on all light types
		#pragma surface surf Nolight decal:blend

		// Use shader model 3.0 target, to get nicer looking lighting
		//#pragma target 3.0
		

		sampler2D _MainTex;

		struct Input {
			float2 uv_MainTex;
		};

		half _Glossiness;
		half _Metallic;
		fixed4 _Color;


		void vert(inout appdata_full v) {
			//convert the normal from world coordinates to object coordinates
			//lerp 插值函数，返回（1*(1-_Snow)+(-1)*_Snow)
			v.vertex.xyz += v.normal;
		}

		void surf(Input IN, inout SurfaceOutput o) {
			// Albedo comes from a texture tinted by color
			fixed4 c = tex2D(_MainTex, IN.uv_MainTex) *_Color;
			o.Albedo = c.rgb;
			// Metallic and smoothness come from slider variables
			//o.Metallic = _Metallic;
			//o.Smoothness = _Glossiness;
			o.Alpha = c.a;
		}

		half4 LightingNolight(SurfaceOutput s, half3 lightDir, half3 viewDir, half atten) {

			half4 c;
			c.rgb = s.Albedo;
			c.a = s.Alpha;
			return c;
		}
		ENDCG
	}
	FallBack "Diffuse"
}
