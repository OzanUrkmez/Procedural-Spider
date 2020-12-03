Shader "Questry/StandardInFront"
{
	Properties
	{
		_MainTex("Base (RGB)", 2D) = "white" {}
		_Color("Color", Color) = (0,0,0,255)
	}
		SubShader
		{
			Tags{"Queue" = "Geometry+501"}
			ZWrite Off
			ZTest Always
			
			CGPROGRAM

			#pragma surface surf Lambert

			struct Input
			{
				float2 uv_MainTex;
			};

			sampler2D _MainTex;
			fixed4 _Color;

			void surf(Input IN, inout SurfaceOutput o)
			{
				o.Albedo = (tex2D(_MainTex, IN.uv_MainTex) * _Color).rgb;
			}
			ENDCG
		}
			FallBack "Diffuse"
}
