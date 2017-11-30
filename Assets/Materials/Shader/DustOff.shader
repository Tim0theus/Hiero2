Shader "Custom/DustOff" {
	Properties{
		_Opacity("Opacity", Range(0,1.5)) = 1
		_Brightness("Brightness", Range(0,1.5)) = 1
		_Reveal("Reveal", Range(0,1)) = 0
		_Color("Dust Color", Color) = (1,1,1,1)
		_TransTex("Dust Mask", 2D) = "white" {}
		_MainTex("Dust Texture", 2D) = "white" {}
	}

		SubShader{
			Tags {"Queue"="Transparent" "RenderType" = "Transparent" }
			LOD 200

			CGPROGRAM
			// Physically based Standard lighting model, and enable shadows on all light types
			#pragma surface surf Lambert alpha:fade

			// Use shader model 3.0 target, to get nicer looking lighting
			#pragma target 3.0

			sampler2D _MainTex;
			sampler2D _TransTex;

			struct Input {
				float2 uv_MainTex;
				float2 uv_TransTex;
				float2 uv_BorderTex;
			};
			
			fixed _Opacity;
			fixed _Brightness;
			fixed _Reveal;
			fixed4 _Color;

			void surf(Input IN, inout SurfaceOutput o) {
				
				fixed dust = tex2D(_MainTex, IN.uv_MainTex);
				fixed4 transitionColor = tex2D(_TransTex, IN.uv_TransTex);
				fixed transition = transitionColor.r * transitionColor.a;

				o.Albedo = saturate(dust + transition) * _Color * _Brightness;
				o.Alpha = lerp(transition, 0, _Reveal) * _Opacity;
			}
			ENDCG
		}
			FallBack Off
}
