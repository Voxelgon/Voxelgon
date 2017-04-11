#ifndef VG_LIGHTING_INCLUDE
#define VG_LIGHTING_INCLUDE

#include "UnityCG.cginc"
#include "Lighting.cginc"
#include "AutoLight.cginc"

// helper functions

#if defined(UNITY_PASS_FORWARDADD) || defined(UNITY_PASS_FORWARDBASE)
    #define UNITY_PASS_FORWARD
#endif

#ifndef USING_DIRECTIONAL_LIGHT
    #define LIGHTDIR normalize(UnityWorldSpaceLightDir(worldPos))
#else
    #define LIGHTDIR _WorldSpaceLightPos0.xyz;
#endif

half VoxelgonForwardCalcAtten(float3 worldPos) {
    #ifdef POINT
        float3 lightCoord = mul(unity_WorldToLight, float4(worldPos, 1)).xyz;

        return tex2D(_LightTexture0, dot(lightCoord, lightCoord).rr).UNITY_ATTEN_CHANNEL;
    #endif

    #ifdef SPOT
        float4 lightCoord = mul(unity_WorldToLight, float4(worldPos, 1));

        float cookie = tex2D(_LightTexture0, lightCoord.xy / lightCoord.w + 0.5).w;
        float atten = (lightCoord.z > 0) * tex2D(_LightTextureB0, dot(lightCoord.xyz, lightCoord.xyz).xx).UNITY_ATTEN_CHANNEL;

        return cookie * atten; 
    #endif

    #ifdef POINT_COOKIE
        float3 lightCoord = mul(unity_WorldToLight, float4(worldPos, 1)).xyz;
        return tex2D(_LightTextureB0, dot(lightCoord, lightCoord).rr).UNITY_ATTEN_CHANNEL * texCUBE(_LightTexture0, lightCoord).w;
    #endif

    #ifdef DIRECTIONAL_COOKIE
        float2 lightCoord = mul(unity_WorldToLight, float4(worldPos, 1)).xy;
        return tex2D(_LightTexture0, lightCoord).w;
    #endif

    // assume directional
    return 1.0;
}

#endif