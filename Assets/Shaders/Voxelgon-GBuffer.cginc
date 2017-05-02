#ifndef VG_GBUFFER_INCLUDE
#define VG_GBUFFER_INCLUDE

// Main structure that store the data from the standard shader (i.e user input)
struct VoxelgonStandardData {
    // RT0
    half3	diffuseColor;
    half	occlusion;

    // RT1
    half    hardness;

    // RT2
    half3	normalWorld;		// normal in world space
};


// This will encode VoxelgonStandardData into GBuffer
void VoxelgonStandardDataToGbuffer(VoxelgonStandardData data, out half4 outGBuffer0, out half4 outGBuffer1, out half4 outGBuffer2) {
    // RT0: diffuse color (rgb), occlusion (a) - sRGB rendertarget
    outGBuffer0 = half4(data.diffuseColor, data.occlusion);

    // RT1: hardness (r)
    outGBuffer1 = half4(data.hardness,0,0,0);

    // RT2: normal (rgb), some kind of boolean thingy? (a) 
    outGBuffer2 = half4(data.normalWorld * 0.5f + 0.5f, 1.0f);
}

// This decode the Gbuffer in a VoxelgonStandardData struct
VoxelgonStandardData VoxelgonStandardDataFromGbuffer(half4 inGBuffer0, half4 inGBuffer1, half4 inGBuffer2) {
    VoxelgonStandardData data;

    // RT0
    data.diffuseColor	= inGBuffer0.rgb;
    data.occlusion		= inGBuffer0.a;

    // RT1
    data.hardness       = inGBuffer1.r;

    // RT2
    data.normalWorld	= normalize(inGBuffer2.rgb * 2 - 1);

    return data;
}

// In some cases like for terrain, the user want to apply a specific weight to the attribute
// The function below is use for this
void VoxelgonStandardDataApplyWeightToGbuffer(inout half4 inOutGBuffer0, inout half4 inOutGBuffer1, inout half4 inOutGBuffer2, half alpha) {
	// With UnityStandardData current encoding, We can apply the weigth directly on the gbuffer
	inOutGBuffer0.rgb *= alpha; // diffuseColor
	inOutGBuffer1.rgb *= alpha; // ???
	inOutGBuffer2.rgb *= alpha; // Normal
}

void VoxelgonFillEmission(half3 color, out half4 outEmission) {
    #ifdef UNITY_HDR_ON
        outEmission = half4(color, 1);
    #else
        outEmission = half4(exp2(-color), 1);
    #endif
}

void VoxelgonFillEmissionMask(out half4 outEmission) {
    outEmission = 0;
}

void VoxelgonFillGBufferLit(VoxelgonStandardData data, half3 ambient, out half4 outGBuffer0, out half4 outGBuffer1, out half4 outGBuffer2, out half4 outEmission) {
    VoxelgonStandardDataToGbuffer(data, outGBuffer0, outGBuffer1, outGBuffer2);
    VoxelgonFillEmission(ambient, outEmission);
}

void VoxelgonFillGBufferUnlit(half3 color, out half4 outGBuffer0, out half4 outGBuffer1, out half4 outGBuffer2, out half4 outEmission) {
    VoxelgonStandardData data;
    data.diffuseColor	= 0;
    data.occlusion		= 1;
    data.normalWorld	= 0;
    data.hardness       = 0;

    VoxelgonStandardDataToGbuffer(data, outGBuffer0, outGBuffer1, outGBuffer2);
    VoxelgonFillEmission(color, outEmission);
}
#endif