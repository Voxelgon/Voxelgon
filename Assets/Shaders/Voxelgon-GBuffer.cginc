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
void VoxelgonStandardDataToGbuffer(VoxelgonStandardData data, out half4 outGBuffer0, out half4 outGBuffer1, out half4 outGBuffer2)
{
    // RT0: diffuse color (rgb), occlusion (a) - sRGB rendertarget
    outGBuffer0 = half4(data.diffuseColor, data.occlusion);

    // RT1: hardness (r)
    outGBuffer1 = half4(data.hardness,0,0,0);

    // RT2: normal (rgb), --unused, very low precision-- (a) 
    outGBuffer2 = half4(data.normalWorld * 0.5f + 0.5f, 1.0f);
}

// This decode the Gbuffer in a VoxelgonStandardData struct
VoxelgonStandardData VoxelgonStandardDataFromGbuffer(half4 inGBuffer0, half4 inGBuffer1, half4 inGBuffer2)
{
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
void VoxelgonStandardDataApplyWeightToGbuffer(inout half4 inOutGBuffer0, inout half4 inOutGBuffer1, inout half4 inOutGBuffer2, half alpha)
{
	// With UnityStandardData current encoding, We can apply the weigth directly on the gbuffer
	inOutGBuffer0.rgb *= alpha; // diffuseColor
	inOutGBuffer1.rgb *= alpha; // ???
	inOutGBuffer2.rgb *= alpha; // Normal
}

#endif