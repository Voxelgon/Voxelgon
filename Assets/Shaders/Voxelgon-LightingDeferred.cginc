#ifndef VG_DEFERREDLIGHTING_INCLUDED
#define VG_DEFERREDLIGHTING_INCLUDED

// Deferred lighting / shading helpers

//#include "Voxelgon-Lighting.cginc"

#if defined (SPOT)	
    #define LIGHTDIR normalize (_LightPos.xyz - worldPos)
#elif defined (DIRECTIONAL) || defined (DIRECTIONAL_COOKIE)
    #define LIGHTDIR -_LightDir.xyz
#elif defined (POINT) || defined (POINT_COOKIE)
    #define LIGHTDIR -normalize (worldPos - _LightPos.xyz)
#else
    #define LIGHTDIR half3(0,0,0)
#endif

float4 _LightDir;
float4 _LightPos;
float4 _LightColor;
float4 unity_LightmapFade;


struct voxelgon_v2f_deferred {
    float4 pos : SV_POSITION;
    float4 uv : TEXCOORD0;
    float3 ray : TEXCOORD1;
};

// Shared uniforms

sampler2D_float _CameraDepthTexture;


float4x4 unity_WorldToLight;
sampler2D _LightTextureB0;

#if defined (POINT_COOKIE)
    samplerCUBE _LightTexture0;
#else
    sampler2D _LightTexture0;
#endif


#if defined (SHADOWS_SCREEN)
sampler2D _ShadowMapTexture;
#endif


// Shadow/fade helpers

#include "UnityShadowLibrary.cginc"

float VoxelgonDeferredCalcFade(float3 worldPos, float z)
{
    float sphereDist = distance(worldPos, unity_ShadowFadeCenterAndType.xyz);
    float fadeDist = lerp(z, sphereDist, unity_ShadowFadeCenterAndType.w);
    return saturate(fadeDist * _LightShadowData.z + _LightShadowData.w);
}


half VoxelgonDeferredCalcShadow(float3 worldPos, float3 toLight, float z, float2 uv)
{
    float fade = VoxelgonDeferredCalcFade(worldPos, z);
    
    #if defined(SHADOWS_DEPTH)
        float4 shadowCoord = mul (unity_WorldToShadow[0], float4(worldPos, 1));
        return saturate(UnitySampleShadowmap (shadowCoord) + fade);
    #endif //SHADOWS_DEPTH
    
    #if defined(SHADOWS_SCREEN)
        return saturate(tex2D (_ShadowMapTexture, uv).r + fade);
    #endif
    
    #if defined(SHADOWS_CUBE)
        return UnitySampleShadowmap (toLight);
    #endif //SHADOWS_CUBE
    
    return 1.0;
}


// Common lighting data calculation (direction, attenuation, ...)

float3 ToLight(float3 wpos) {
    #if defined (SPOT)	
        return _LightPos.xyz - wpos;
    // point light case	
    #elif defined (POINT) || defined (POINT_COOKIE)
        return wpos - _LightPos.xyz;
    #else
        return float3(0,0,0);
    #endif
}

float3 LightDir(float3 ToLight) {

}

float VoxelgonDeferredCalcAtten (float3 worldPos, float3 tolight) {

    // spot light case
    #if defined (SPOT)	
        //float3 tolight = _LightPos.xyz - worldPos;
        
        float4 uvCookie = mul (unity_WorldToLight, float4(worldPos,1));
        // negative bias because http://aras-p.info/blog/2010/01/07/screenspace-vs-mip-mapping/
        float atten = tex2Dbias (_LightTexture0, float4(uvCookie.xy / uvCookie.w, 0, -8)).w;
        atten *= uvCookie.w < 0;
        float att = dot(tolight, tolight) * _LightPos.w;
        atten *= tex2D (_LightTextureB0, att.rr).UNITY_ATTEN_CHANNEL;

        return atten;
    
    // directional light case		
    #elif defined (DIRECTIONAL) || defined (DIRECTIONAL_COOKIE)
        #if defined (DIRECTIONAL_COOKIE)
            float atten = tex2Dbias (_LightTexture0, float4(mul(unity_WorldToLight, half4(worldPos,1)).xy, 0, -8)).w;
        #else
            float atten = 1.0;
        #endif //DIRECTIONAL_COOKIE

        return atten;
        
    // point light case	
    #elif defined (POINT) || defined (POINT_COOKIE)
        //float3 tolight = worldPos - _LightPos.xyz;
        
        float att = dot(tolight, tolight) * _LightPos.w;
        float atten = tex2D (_LightTextureB0, att.rr).UNITY_ATTEN_CHANNEL;

        #if defined (POINT_COOKIE)
            atten *= texCUBEbias(_LightTexture0, float4(mul(unity_WorldToLight, half4(worldPos,1)).xyz, -8)).w;
        #endif //POINT_COOKIE	

        return atten;

    #else
        return 1;
    #endif
}

void VoxelgonDeferredCalculateLightParams (
    voxelgon_v2f_deferred i,
    out float2 outUV,
    out half3 outLightDir,
    out float outAtten,
    out float outShadow) {

    i.ray = i.ray * (_ProjectionParams.z / i.ray.z);
    float2 uv = i.uv.xy / i.uv.w;
    
    // read depth and reconstruct world position
    float depth = Linear01Depth(SAMPLE_DEPTH_TEXTURE(_CameraDepthTexture, uv));
    float4 vpos = float4(i.ray * depth,1);
    float3 worldPos = mul(unity_CameraToWorld, vpos).xyz;

    half3 toLight = ToLight(worldPos);
    half3 lightDir = LIGHTDIR;

    float shadow = VoxelgonDeferredCalcShadow(worldPos, toLight, vpos.z, uv);
    float atten = VoxelgonDeferredCalcAtten(worldPos, toLight);
    //float shadow = 1;

    //VoxelgonCalculateLightParams(worldPos, fadeDist, uv, _LightDir, lightDir, atten, shadow);

    outUV = uv;
    outLightDir = lightDir;
    outAtten = atten;
    outShadow = shadow;
}


#endif