Shader "Custom/GlyphWithBackground" {
	Properties {
		_Color("Color", Color) = (1,1,1,1)
		_MainTex("Albedo", 2D) = "white" {}
		_EmissionColor("Emission Color", Color) = (0,0,0)
		_Background("Background", Color) = (1,1,1,1)
	}
	SubShader {
		Tags{ "RenderType" = "Opaque" }
		LOD 200

		CGPROGRAM
		#pragma surface surf Standard
		#pragma target 3.0
		#pragma shader_feature _EMISSION


		struct Input {
			float2 uv_MainTex;
		};

		sampler2D _MainTex;
		uniform fixed4 _Color;
		uniform fixed4 _EmissionColor;
		uniform fixed4 _Background;

		void surf(Input IN, inout SurfaceOutputStandard o) {
			fixed4 c = tex2D(_MainTex, IN.uv_MainTex);
			o.Albedo = lerp(_Background.rgb, _Color.rgb, c.a);
			o.Emission = _EmissionColor.rgb * c.a;

			o.Alpha = _Color.a;
		}
		ENDCG		
	}
	FallBack "Diffuse"
}
