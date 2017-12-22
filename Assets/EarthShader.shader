Shader "Unlit/EarthShader"
{
	Properties
	{
		_Textures ("Textures", 2DArray) = "" {}
		_TileCount ("Tiles", Int) = 0
	}
	SubShader
	{
		Tags { "RenderType"="Opaque" }
		LOD 100

		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			// make fog work
			#pragma multi_compile_fog
			
			#include "UnityCG.cginc"

			struct appdata
			{
				float4 vertex : POSITION;
				float2 uv : TEXCOORD0;
			};

			struct v2f
			{
				float2 uv : TEXCOORD0;
				UNITY_FOG_COORDS(1)
				float4 vertex : SV_POSITION;
			};

			float4 _Textures_ST;
			
			v2f vert (appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = TRANSFORM_TEX(v.uv, _Textures);
				UNITY_TRANSFER_FOG(o, o.vertex);
				return o;
			}

			UNITY_DECLARE_TEX2DARRAY(_Textures);
			int _TileCount;
			
			fixed4 frag (v2f i) : SV_Target
			{
				fixed4 col = UNITY_SAMPLE_TEX2DARRAY(
					_Textures,
					fixed3(
						(i.uv.x * _TileCount) % 1,
						(i.uv.y * _TileCount) % 1,
						floor(i.uv.x * _TileCount) + floor(_TileCount - i.uv.y * _TileCount) * _TileCount
					)
				);

				// apply fog
				UNITY_APPLY_FOG(i.fogCoord, col);
				return col;
			}
			ENDCG
		}
	}
}
