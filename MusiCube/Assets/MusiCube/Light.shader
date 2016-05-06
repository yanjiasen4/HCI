Shader "Custom/Light" {
	Properties{
		_Color("Color", Color) = (1,1,1,1)
		_MainTex("Albedo (RGB)", 2D) = "white" {}
		_OutAlpha("Alpha", Float) = 1.0
	}
		SubShader{
		Tags{ "RenderType" = "Transparent"
		"Queue" = "Transparent" }
		//LOD 200
		Cull Off
		CGPROGRAM
		// Physically based Standard lighting model, and enable shadows on all light types
#pragma surface surf Nolight decal:blend

		// Use shader model 3.0 target, to get nicer looking lighting
		//#pragma target 3.0


		sampler2D _MainTex;
		float _OutAlpha;
	struct Input {
		float2 uv_MainTex;
	};

	half _Glossiness;
	half _Metallic;
	fixed4 _Color;

	void surf(Input IN, inout SurfaceOutput o) {
		// Albedo comes from a texture tinted by color
		fixed4 c = tex2D(_MainTex, IN.uv_MainTex);
		fixed4 nc = c*c.a + _Color*(1 - c.a);
		o.Albedo = nc.rgb;
		// Metallic and smoothness come from slider variables
		//o.Metallic = _Metallic;
		//o.Smoothness = _Glossiness;
		o.Alpha = c.a * _OutAlpha;
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
