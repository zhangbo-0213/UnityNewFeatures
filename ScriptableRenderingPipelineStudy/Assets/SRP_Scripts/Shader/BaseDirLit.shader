Shader "Custom/BaseDirLit"
{
	Properties
	{
		_Color("Tint", Color) = (0.5,0.5,0.5,1)
		_DiffuseFactor("Diffuse Factor", Range(0,1)) = 1
		_SpecularColor("Specular Color",Color)=(1,1,1,1)
		_SpecularFactor("Specular Factor", Range(0,1)) = 1
		_SpecularPower("Specular Power",Float) = 100
	}
  
	HLSLINCLUDE

	#include "UnityCG.cginc"
	#define PI 3.14159265359

	uniform float4 _LightDir;
	uniform float4 _LightColor;
	uniform float4 _CameraPos;
	uniform float4 _Color;
	uniform float _DiffuseFactor;
	uniform float _SpecularFactor;
	uniform float _SpecularColor;
	uniform float _SpecularPower;

	struct a2v
	{
		float4 vertex : POSITION;
		float4 normal : NORMAL;
	};

	struct v2f
	{
		float4 pos : SV_POSITION;
		float4 normalWorld : TEXCOORD1;
		float4 worldPos : TEXCOORD2;
	};

	v2f vert(a2v v)
	{
		v2f o;
		UNITY_INITIALIZE_OUTPUT(v2f,o);
		o.pos = UnityObjectToClipPos(v.vertex);
		o.normalWorld = float4(normalize(mul(normalize(v.normal.xyz), (float3x3)unity_WorldToObject)),v.normal.w);
		o.worldPos = mul(unity_ObjectToWorld,v.vertex);
		return o;
	}

	half4 frag(v2f i) : SV_Target
	{
		fixed4 diffuse=_DiffuseFactor*max(0.0,dot(_LightDir.xyz,normalize(i.normalWorld.xyz)))*_Color*_LightColor;
		fixed3 viewDir=normalize(_CameraPos.xyz-i.worldPos.xyz);
		fixed3 halfDir=normalize(_LightDir.xyz+viewDir);
		fixed4 specular= _LightColor*_SpecularFactor*pow(max(0,dot(normalize(i.normalWorld.xyz),halfDir)),_SpecularPower)*max(0,saturate(dot(normalize(i.normalWorld.xyz),_LightDir.xyz)))*(_SpecularPower+8)/(8+PI);
		return diffuse+specular;
	}

	ENDHLSL

	SubShader
	{
		Tags{ "Queue" = "Geometry" }
		LOD 100
		Pass
		{
			Tags {"LightMode" = "BaseLit"}

			HLSLPROGRAM
			#pragma vertex vert
			#pragma fragment frag

			ENDHLSL
		}
	}

}
