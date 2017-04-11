#ifndef VG_LIGHTINGCOMMON_INCLUDE
#define VG_LIGHTINGCOMMON_INCLUDE

half3 VoxelgonWrapLighting(half3 albedo, 
                   half3 worldNormal, half3 lightDir, 
                   half3 lightColor,
                   float atten, half shadow,
                   half hardness) {
    half nl = dot(worldNormal, lightDir);
    half wrap = ((nl * 0.5) + 0.5); 
    wrap *= lerp(wrap, 1, hardness);

    half3 c = albedo;
    c *= 1 - (1 - shadow) * saturate(nl);
    c *= lightColor * atten * wrap;
    return c;
}

#endif