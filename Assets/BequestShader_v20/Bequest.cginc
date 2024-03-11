//v19 - Fixed specular
//v20 - custom inspector

#include "UnityCG.cginc"
#include "UnityLightingCommon.cginc"
#include "AutoLight.cginc"

#define OVERLAY(x, y) (x < 0.5 ? (2.0 * x * y) : (1.0 - 2.0 * (1.0 - x) * (1.0 - y)))
#define SCREEN(x, y) (1.0 - ((1.0 - x) * (1.0 - y)))

sampler2D _ColorTex;
sampler2D _LightmapTex;
float4 _LightmapTex_TexelSize;
sampler2D _HashesTex;
sampler2D _DirtTex;
float4 _DirtTex_TexelSize;
sampler2D _SpecMap;

float _OpacityLightmap;
float _OpacityHashes;
float _OpacityDirt;

float _InverseLightmap;
float _InverseHashes;
float _InverseDirt;

float _SpecularLevel;
float _SpecularOpacity;

float _Saturation;

float _TileX;
float _TileY;
float _TileAmount;
float _BleedingDelta;

float _TileXDirt;
float _TileYDirt;
float _TileAmountDirt;
float _BleedingDeltaDirt;

float _RimPower;
float4 _RimColor;

float3 _RimOffset;
float _RimContrast;
float _RimWidth;

float _RampContrast;
float _RampOffset;
float3 _RampColor1;
float3 _RampColor2;

float _Opacity;

float _LightingFix;

float _GlassFadeDist1;
float _GlassFadeDist2;

float _highlightFactor;

//wind params
float _WindStr;
float _WindSpeed;
float _WindFreq;

//distance squish
float _FadeStart;
float _FadeEnd;
float3 _ScaleInfluence;

fixed4 blendMultiply(fixed4 base, fixed4 layer)
{
	fixed4 blended = base * layer;
	return lerp(base, blended, layer.a);
}

fixed4 blendOverlay(fixed4 base, fixed4 layer)
{
	fixed4 blended = layer;
	blended.r = OVERLAY(base.r, layer.r);
	blended.g = OVERLAY(base.g, layer.g);
	blended.b = OVERLAY(base.b, layer.b);
	return lerp(base, blended, layer.a);
}

fixed4 blendScreen(fixed4 base, fixed4 layer)
{
	fixed4 blended = base + layer;
	return lerp(base, blended, layer.a);
}

fixed4 saturation(in fixed4 color, in float adjustment)
{
	const fixed3 W = fixed3(0.2125, 0.7154, 0.0721);
	fixed3 intensity = fixed3(1.0, 1.0, 1.0) * dot(color.rgb, W);
	fixed3 result = lerp(intensity, color.rgb, adjustment);
	return fixed4(result.r, result.g, result.b, color.a);
}

fixed4 BlendMaps(fixed4 color, fixed4 lightmap, fixed4 hashes, fixed4 dirt)
{
	fixed4 result = color;

	//lightmap
	#ifdef _LIGHTMAP_BLEND_MULTIPLY
		result = blendMultiply(result, lightmap);
	#endif
	#ifdef _LIGHTMAP_BLEND_OVERLAY
		result = blendOverlay(result, lightmap);
	#endif
	#ifdef _LIGHTMAP_BLEND_SCREEN
		result = blendScreen(result, lightmap);
	#endif

	//Hashes
	#ifdef _HASHES_BLEND_MULTIPLY
		result = blendMultiply(result, hashes);
	#endif
	#ifdef _HASHES_BLEND_OVERLAY
		result = blendOverlay(result, hashes);
	#endif
	#ifdef _HASHES_BLEND_SCREEN
		result = blendScreen(result, hashes);
	#endif

	//Dirt
	#ifdef _DIRT_BLEND_MULTIPLY
		result = blendMultiply(result, dirt);
	#endif
	#ifdef _DIRT_BLEND_OVERLAY
		result = blendOverlay(result, dirt);
	#endif
	#ifdef _DIRT_BLEND_SCREEN
		result = blendScreen(result, dirt);
	#endif

	return result;
}

float4 DistanceSquishWorldPos(in float4 worldPos, in float4 localPos)
{
    float dist = -UnityWorldToViewPos(worldPos).z;
    float fadeScale = _FadeEnd - _FadeStart;
    float scale = lerp(0, 1, (dist - _FadeStart) / fadeScale);
    float4 newLocalPos = localPos;
    float3 scale3 = lerp(float3(1,1,1), float3(scale,scale,scale), _ScaleInfluence);
    newLocalPos.xyz *= saturate(scale3);
    //return mul(unity_ObjectToWorld, newLocalPos);

    float4 ret = worldPos;
    ret.xyz -= saturate(scale) * _ScaleInfluence;
    return ret;
}

struct v2f
{
	float4 uv12 : TEXCOORD0;
	float4 uv34 : TEXCOORD1;

	float3 tspace0 : TEXCOORD2; // tangent.x, bitangent.x, normal.x
	float3 tspace1 : TEXCOORD3; // tangent.y, bitangent.y, normal.y
	float3 tspace2 : TEXCOORD4; // tangent.z, bitangent.z, normal.z
	float4 ambient: COLOR;

	LIGHTING_COORDS(5, 6)
	UNITY_FOG_COORDS(7)
	
	float4 wpos : TEXCOORD8;
	float4 pos : SV_POSITION;

	#ifdef DETAIL_TEX
		float4 scaleOffset : TEXCOORD9;
		float4 scaleOffset2 : TEXCOORD10;
	#endif

	#ifdef DETAIL_DIRT
		float4 scaleOffsetDirt : TEXCOORD11;
		float4 scaleOffsetDirt2 : TEXCOORD12;
	#endif
    float3 viewDirWS : TEXCOORD13;
    float4 screenPos : TEXCOORD14;
};

v2f vert (appdata_full v)
{
	v2f o;
    float4 wPos = mul(unity_ObjectToWorld, v.vertex);

#ifdef DISTANCE_SQUISH
    wPos = DistanceSquishWorldPos(wPos, v.vertex);
#endif
    
    o.wpos = wPos;
    
#ifdef WIND
    float str = _WindStr * v.color.r;
    float speed = _WindSpeed * _Time.x;
    float offsetX = wPos.x * _WindFreq;
    float offsetY = wPos.z * _WindFreq;
    
    wPos.xz += float2(sin(speed + offsetX), cos(speed + offsetY)) * str;
    o.pos = UnityWorldToClipPos(wPos);
#else
    o.pos = UnityWorldToClipPos(wPos);
#endif

    o.uv12.xy = v.texcoord;
	o.uv12.zw = v.texcoord1;
	o.uv34.xy = v.texcoord2;
	o.uv34.zw = v.texcoord3;

	float3 worldNormal = UnityObjectToWorldNormal(v.normal);
	o.ambient.rgb = ShadeSH9(float4(worldNormal,1));

	//normalmap
	float3 wNormal = UnityObjectToWorldNormal(v.normal);
	float3 wTangent = UnityObjectToWorldDir(v.tangent.xyz);
	// compute bitangent from cross product of normal and tangent
	float tangentSign = v.tangent.w * unity_WorldTransformParams.w;
	float3 wBitangent = cross(wNormal, wTangent) * tangentSign;
	// output the tangent space matrix
	o.tspace0 = float3(wTangent.x, wBitangent.x, wNormal.x);
	o.tspace1 = float3(wTangent.y, wBitangent.y, wNormal.y);
	o.tspace2 = float3(wTangent.z, wBitangent.z, wNormal.z);
    
	o.ambient.a = 1.0 - v.color.r;	//invert red for specular
	
	TRANSFER_VERTEX_TO_FRAGMENT(o)

	UNITY_TRANSFER_FOG(o, o.pos);
    UNITY_TRANSFER_DITHER_CROSSFADE_HPOS(o, o.pos)

	#ifdef DETAIL_TEX
		//get atlas tile position
		float2 tileScale = float2(1.0 / _TileX, 1.0 / _TileY);
		float2 tileOffset = floor(v.texcoord1 * float2(_TileX, _TileY)) * tileScale;

		o.uv12.zw = (v.texcoord1 - tileOffset) / tileScale;

		o.scaleOffset.xy = tileScale;
		o.scaleOffset.zw = tileOffset;

		float2 halfPixel = _LightmapTex_TexelSize.xy * _BleedingDelta * 2.0;

		float2 s = float2(1.0, 1.0) - halfPixel * 2.0;
		float2 off = float2(1.0, 1.0) * halfPixel;

		o.scaleOffset2.xy = s;
		o.scaleOffset2.zw = off;
	#endif

	#ifdef DETAIL_DIRT
		//get atlas tile position
		float2 tileScaleD = float2(1.0 / _TileXDirt, 1.0 / _TileYDirt);
		float2 tileOffsetD = floor(v.texcoord3 * float2(_TileXDirt, _TileYDirt)) * tileScaleD;

		o.uv34.zw = (v.texcoord3 - tileOffsetD) / tileScaleD;

		o.scaleOffsetDirt.xy = tileScaleD;
		o.scaleOffsetDirt.zw = tileOffsetD;

		float2 halfPixelD = _DirtTex_TexelSize.xy * _BleedingDeltaDirt * 2.0;

		float2 sD = float2(1.0, 1.0) - halfPixelD * 2.0;
		float2 offD = float2(1.0, 1.0) * halfPixelD;

		o.scaleOffsetDirt2.xy = sD;
		o.scaleOffsetDirt2.zw = offD;
	#endif

    o.viewDirWS = normalize(WorldSpaceViewDir(v.vertex));
    o.screenPos = ComputeScreenPos(o.pos);

	return o;
}

float3 GetRamp(float val)
{
	float factor = saturate(((saturate(val) - 0.5) * _RampContrast + 0.5) + _RampOffset);

	return lerp(_RampColor1, _RampColor2, factor);
}

float3x3 rotateX(float rad)
{
    float c = cos(rad);
    float s = sin(rad);
    return float3x3(
        1.0, 0.0, 0.0,
        0.0, c, s,
        0.0, -s, c
    );
}

float3x3 rotateY(float rad) {
    float c = cos(rad);
    float s = sin(rad);
    return float3x3(
        c, 0.0, -s,
        0.0, 1.0, 0.0,
        s, 0.0, c
    );
}

float3x3 rotateZ(float rad) {
    float c = cos(rad);
    float s = sin(rad);
    return float3x3(
        c, s, 0.0,
        -s, c, 0.0,
        0.0, 0.0, 1.0
    );
}

fixed4 frag (v2f i) : SV_Target
{
    #ifdef LOD_FADE_CROSSFADE
    float2 vpos = i.screenPos.xy / i.screenPos.w * _ScreenParams.xy;
    UnityApplyDitherCrossFade(vpos);
    #endif
    
	float2 lightmapUV = i.uv12.zw;
	#ifdef DETAIL_TEX
		lightmapUV = frac(i.uv12.zw * _TileAmount);
		lightmapUV = lightmapUV * i.scaleOffset2.xy + i.scaleOffset2.zw;
		lightmapUV = lightmapUV * i.scaleOffset.xy + i.scaleOffset.zw;
	#endif

	float2 dirtUV = i.uv34.zw;
	#ifdef DETAIL_DIRT
		dirtUV = frac(i.uv34.zw * _TileAmountDirt);
		dirtUV = dirtUV * i.scaleOffsetDirt2.xy + i.scaleOffsetDirt2.zw;
		dirtUV = dirtUV * i.scaleOffsetDirt.xy + i.scaleOffsetDirt.zw;
	#endif

	fixed4 col_Color = tex2D(_ColorTex, i.uv12.xy);
	fixed4 col_Lightmap = tex2D(_LightmapTex, lightmapUV, ddx(i.uv12.zw), ddy(i.uv12.zw));
	fixed4 col_Hashes = tex2D(_HashesTex, i.uv34.xy);
	fixed4 col_Dirt = tex2D(_DirtTex, dirtUV, ddx(i.uv34.zw), ddy(i.uv34.zw));

	col_Hashes.rgb = col_Hashes.bbb;

	col_Lightmap.a *= _OpacityLightmap;
	col_Hashes.a *= _OpacityHashes;
	col_Dirt.a *= _OpacityDirt;

	float finalOpacity = saturate(_Opacity + (1.0 - col_Lightmap.r) * _OpacityLightmap + (1.0 - col_Hashes.r) * _OpacityHashes + (1.0 - col_Dirt.r) * _OpacityDirt);
	float specMask = lerp(1.0, col_Lightmap.r, _OpacityLightmap) * lerp(1.0, col_Hashes.r, _OpacityHashes) * lerp(1.0, col_Dirt, _OpacityDirt);

	col_Lightmap.rgb = lerp(col_Lightmap.rgb, 1.0 - col_Lightmap.rgb, _InverseLightmap);
	col_Hashes.rgb = lerp(col_Hashes.rgb, 1.0 - col_Hashes.rgb, _InverseHashes);
	col_Dirt.rgb = lerp(col_Dirt.rgb, 1.0 - col_Dirt.rgb, _InverseDirt);

	fixed4 col = BlendMaps(col_Color, col_Lightmap, col_Hashes, col_Dirt);

	#ifdef GLASS
		col.a = finalOpacity;
	#endif

	col = saturation(col, _Saturation);
	float3 light;

	half3 tnormal = half3(0.0, 0.0, 1.0);
	half3 worldNormal = half3(dot(i.tspace0, tnormal), dot(i.tspace1, tnormal), dot(i.tspace2, tnormal));

	float3 lightDirection;

	#ifdef DIRECTIONAL
		lightDirection = normalize(_WorldSpaceLightPos0.xyz);
	#else
		float3 vertexToLightSource = _WorldSpaceLightPos0.xyz - i.wpos.xyz;
		lightDirection = normalize(vertexToLightSource);
	#endif

	float3 viewDirection = i.viewDirWS;//normalize(_WorldSpaceCameraPos - i.wpos.xyz);

	half atten = LIGHT_ATTENUATION(i);

	half nl = saturate(dot(worldNormal, lightDirection));

	#ifdef DIRECTIONAL
	light = atten * _LightColor0 * (lerp(1, GetRamp(nl), _LightingFix)) + i.ambient;
	#else
	light = GetRamp(nl) * atten * _LightColor0 + i.ambient;
	#endif

    float3 shadowOnly = light * nl;

	//rimlight
	float3 rimDirection = viewDirection;

	rimDirection = mul(rotateX(_RimOffset.x), rimDirection);
	rimDirection = mul(rotateY(_RimOffset.y), rimDirection);
	rimDirection = mul(rotateZ(_RimOffset.z), rimDirection);

	float rim = pow(1.0 - dot(rimDirection, worldNormal), _RimPower);
	rim = saturate(((saturate(rim) - 0.5) * _RimContrast + 0.5) + _RimWidth);
	light += (rim * _RimColor.rgb) * _RimColor.a;

	float specMap = tex2D(_SpecMap, i.uv34.zw).r;
	half specularLevel = (1.0 - i.ambient.a) * _SpecularLevel;
	half specF = max(0.0, dot(reflect(-lightDirection, worldNormal), viewDirection)) * atten * 10;
	half specularFactor = pow(specF, specularLevel) * specMap;
	float3 specularReflection = _LightColor0.rgb * GetRamp(specularFactor) * atten;

    light += _highlightFactor;
    
	col.rgb *= light;
	//col.a = max(col.a, light.r);
	#ifndef DUSTY
		specMask = 1.0;
	#endif

	#ifdef GLASS
		float3 specularFinal = (saturate(pow(specF, _SpecularLevel))) * specMap;
	#else
		float3 specularFinal = (saturate(pow(i.ambient.a * specF, _SpecularLevel))) * specMap;
	#endif
	specularFinal = saturate(((saturate(specularFinal) - 0.5) * _RampContrast + 0.5) + _RampOffset);
	specularFinal = specularFinal * _SpecularOpacity;
	col.rgb += specularFinal.r;
	col.a = max(col.a, specularFinal.r);

    float camDist = length(_WorldSpaceCameraPos - i.wpos.xyz);

    #ifdef GLASS
    #ifdef DIRECTIONAL
    col.a = lerp(col.a, 1.0, smoothstep(_GlassFadeDist1, _GlassFadeDist2, camDist));
    //col.rgb = specularFinal;
    
    #endif
    #endif

    //col.rgb = float3(unity_LODFade.x, 0, 0);
    
    //col.a = 1;
    
	UNITY_APPLY_FOG(i.fogCoord, col);
	return col;
}