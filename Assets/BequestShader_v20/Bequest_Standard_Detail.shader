Shader "Bequest_v20/Bequest Standard (Detail)"
{
	Properties
	{
		//editor needed properties
		[HideInInspector] _DetailFoldout ("Settings", Float) = 0
		[HideInInspector] _MaterialFoldout ("Settings", Float) = 0
		[HideInInspector] _ShowDefault ("Show Default Editor", Float) = 0
		//end editor needed

		[NoScaleOffset] _ColorTex ("Color", 2D) = "white" {}
		[NoScaleOffset] _LightmapTex ("Detail", 2D) = "white" {}

		_TileX("Tiles X", Float) = 4
		_TileY("Tiles Y", Float) = 4
		_TileAmount ("Tile amount", Float) = 10
		_BleedingDelta ("Margin (pixels)", Int) = 1

		[NoScaleOffset] _HashesTex ("Hatches", 2D) = "white" {}
		[NoScaleOffset] _DirtTex ("Material", 2D) = "white" {}

		_TileXDirt("Tiles X Material", Float) = 4
		_TileYDirt("Tiles Y Material", Float) = 4
		_TileAmountDirt ("Tile amount Material", Float) = 10
		_BleedingDeltaDirt ("Margin (pixels) Material", Int) = 1

		[NoScaleOffset] _SpecMap("SpecularMap", 2D) = "white" {}
		_SpecularLevel ("Glossiness", Float) = 40.0
		_SpecularOpacity ("Intensity", Range(0.0, 1.0)) = 0.5

		_Saturation ("Saturation", Float) = 1.0

		_OpacityLightmap ("Opacity", Range(0.0, 1.0)) = 1.0
		[IntRange] _InverseLightmap ("Invert", Range(0.0, 1.0)) = 0.0
		[KeywordEnum(Multiply, Overlay, Screen)] _Lightmap_Blend("Blend", Float) = 0

		_OpacityHashes ("Opacity", Range(0.0, 1.0)) = 1.0
		[IntRange] _InverseHashes ("Invert", Range(0.0, 1.0)) = 0.0
		[KeywordEnum(Multiply, Overlay, Screen)] _Hashes_Blend("Blend", Float) = 0

		[Header(Dirt)] _OpacityDirt ("Opacity", Range(0.0, 1.0)) = 1.0
		[IntRange] _InverseDirt ("Invert", Range(0.0, 1.0)) = 0.0
		[KeywordEnum(Multiply, Overlay, Screen)] _Dirt_Blend("Blend", Float) = 0

		_RimPower ("Rim power", Float) = 4.0
		_RimColor ("Rim color", Color) = (1.0, 1.0, 1.0, 1.0)

		_RampContrast ("Ramp Contrast", Float) = 1.0
		_RampOffset ("Ramp Offset", Float) = 0.0

		_RampColor1 ("Ramp Color A", Color) = (0.0, 0.0, 0.0, 1.0)
		_RampColor2 ("Ramp Color B", Color) = (1.0, 1.0, 1.0, 1.0)

		_RimOffset ("Rim rotation", Vector) = (0.0, 0.0, 0.0)
		_RimContrast ("Rim Contrast", Float) = 1.0
		_RimWidth ("Rim Width", Float) = 0.0
	    
	    _LightingFix ("LightingFix", Range(0.0, 1.0)) = 0.0
	    
	    _highlightFactor ("Highlight", Range(0.0, 1.0)) = 0.0
		
		[Toggle(DISTANCE_SQUISH)]
		_enableDistanceSquish ("Distance squish", Float) = 0.0
		_FadeStart ("Start fade dist", Float) = 10
    	_FadeEnd ("End fade dist", Float) = 50
		_ScaleInfluence ("Scale influence per-axis", Vector) = (0, 0, 1, 0)
		
	}
	SubShader
	{
		Tags { "RenderType"="Opaque"
			"DisableBatching" = "LODFading"}
		Pass
		{
			Tags { "LightMode"="ForwardBase" }
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag

			#pragma skip_variants DIRECTIONAL_COOKIE
			#pragma skip_variants SHADOWS_SOFT DYNAMICLIGHTMAP_ON DIRLIGHTMAP_COMBINED DIRLIGHTMAP_SEPARATE

			#pragma multi_compile_fog
			#pragma multi_compile_fwdbase nolightmap nodirlightmap nodynlightmap novertexlight

			#pragma multi_compile _LIGHTMAP_BLEND_MULTIPLY _LIGHTMAP_BLEND_OVERLAY _LIGHTMAP_BLEND_SCREEN
			#pragma multi_compile _HASHES_BLEND_MULTIPLY _HASHES_BLEND_OVERLAY _HASHES_BLEND_SCREEN
			#pragma multi_compile _DIRT_BLEND_MULTIPLY _DIRT_BLEND_OVERLAY _DIRT_BLEND_SCREEN

			#pragma multi_compile _ LOD_FADE_CROSSFADE

			#pragma shader_feature DISTANCE_SQUISH

			#define DETAIL_TEX
			#define DETAIL_DIRT

			#include "Bequest.cginc"

			ENDCG
		}

		Pass
		{
			Tags { "LightMode" = "ForwardAdd" }
			ZWrite Off
			Blend One One

			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag

			#pragma skip_variants POINT_COOKIE SPOT_COOKIE
			#pragma skip_variants DYNAMICLIGHTMAP_ON DIRLIGHTMAP_COMBINED DIRLIGHTMAP_SEPARATE SHADOWS_SCREEN

			#pragma multi_compile_fwdadd_fullshadows

			#pragma multi_compile _LIGHTMAP_BLEND_MULTIPLY _LIGHTMAP_BLEND_OVERLAY _LIGHTMAP_BLEND_SCREEN
			#pragma multi_compile _HASHES_BLEND_MULTIPLY _HASHES_BLEND_OVERLAY _HASHES_BLEND_SCREEN
			#pragma multi_compile _DIRT_BLEND_MULTIPLY _DIRT_BLEND_OVERLAY _DIRT_BLEND_SCREEN

			#pragma shader_feature DISTANCE_SQUISH

			#define DETAIL_TEX
			#define DETAIL_DIRT

			#include "Bequest.cginc"
			ENDCG
		}
		UsePass "VertexLit/SHADOWCASTER"
	}

	CustomEditor "BequestShaderGUI"
}
