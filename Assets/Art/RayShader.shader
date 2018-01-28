Shader "Custom/RayShader"
{
	Properties
	{
		_Color ("Color", Color) = (1,1,1,1)
		_Speed("Speed", float) = 1
	}
	SubShader
	{
		Tags { "RenderType"="Transparent" "Queue"="Transparent" }
		LOD 100
		Blend SrcAlpha OneMinusSrcAlpha

		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			
			#include "UnityCG.cginc"

			struct appdata
			{
				float4 vertex : POSITION;
				float2 uv : TEXCOORD0;
			};

			struct v2f
			{
				float2 uv : TEXCOORD0;
				float4 vertex : SV_POSITION;
			};

			fixed4 _Color;
			float _Speed;
			
			v2f vert (appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = v.uv;
				return o;
			}
			
			fixed4 frag (v2f i) : SV_Target
			{
				float spacing = -0.75f;
				float sinus = sin(_Time.y * _Speed + i.uv.y * 10.0f) - spacing;
				sinus = saturate(sinus) / abs(spacing);
				float mult = sinus * sinus;
				float alpha = mult;
				return fixed4(_Color.rgb, alpha);
			}
			ENDCG
		}
	}
}
