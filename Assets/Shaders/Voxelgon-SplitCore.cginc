#ifndef VG_STANDARDCORE_INCLUDE
#define VG_STANDARDCORE_INCLUDE

#include "UnityCG.cginc"
#include "Lighting.cginc"
#include "AutoLight.cginc"

#include "Voxelgon-LightingForward.cginc"
#include "Voxelgon-LightingCommon.cginc"
#include "Voxelgon-GBuffer.cginc"

// COMMON VARIABLES
half _Hardness;
half3 _BaseColor;
half3 _PlaneNormal;
half3 _PlanePosition;

// COMMON FUNCTIONS

bool Visible(half3 worldPos) {
    return dot(worldPos - _PlanePosition, _PlaneNormal) > 0;
}

// vertex-to-fragment interpolation data
struct v2f {
    float4 pos : SV_POSITION;
    half3 worldPos: TEXCOORD0;

    UNITY_VERTEX_INPUT_INSTANCE_ID
    UNITY_VERTEX_OUTPUT_STEREO

    #ifdef SPLIT_FRONT
        half3 color : COLOR0;
        half3 worldNormal : TEXCOORD1;

        #ifdef UNITY_PASS_FORWARD
            UNITY_SHADOW_COORDS(2)
        #endif

        #ifdef UNITY_SHOULD_SAMPLE_SH
            half3 sh : TEXCOORD3;
        #endif
    #endif
};

// common vertex shader
v2f vert (appdata_full v) {
    UNITY_SETUP_INSTANCE_ID(v);
    v2f o;
    UNITY_INITIALIZE_OUTPUT(v2f,o);
    UNITY_TRANSFER_INSTANCE_ID(v,o);
    UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);

    o.pos = UnityObjectToClipPos(v.vertex);
    o.worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;

    #ifdef SPLIT_FRONT
        o.worldNormal = UnityObjectToWorldNormal(v.normal);
        o.color = lerp(_BaseColor.rgb, v.color.rgb, v.color.a);

        #ifdef UNITY_PASS_FORWARD
    
            UNITY_TRANSFER_SHADOW(o,v.texcoord1.xy); // pass shadow coordinates to pixel shader
        #endif
        
        #ifdef UNITY_SHOULD_SAMPLE_SH
            // Approximated illumination from non-important point lights
            #ifdef VERTEXLIGHT_ON
                o.sh += Shade4PointLights (
                unity_4LightPosX0, unity_4LightPosY0, unity_4LightPosZ0,
                unity_LightColor[0].rgb, unity_LightColor[1].rgb, unity_LightColor[2].rgb, unity_LightColor[3].rgb,
                unity_4LightAtten0, o.worldPos, o.worldNormal);
            #endif
            o.sh += ShadeSH9(half4(o.worldNormal, 1.0));
        #endif
    #endif

    return o;
}

#ifdef UNITY_PASS_FORWARD //ifdef to prevent compile errors
// fragment shader
half4 frag_forward (v2f i) : SV_Target {
    UNITY_SETUP_INSTANCE_ID(i);

    if (!Visible(i.worldPos)) discard;

    half4 c = 0;

    #ifdef SPLIT_FRONT
        half3 lightDir = LIGHTDIR;


        // compute lighting & shadowing factor
        float atten = VoxelgonForwardCalcAtten(i.worldPos);
        half shadow = SHADOW_ATTENUATION(i);

        c.rgb = VoxelgonWrapLighting(i.color, i.worldNormal, lightDir, _LightColor0, atten, shadow, _Hardness);

        #ifdef UNITY_PASS_FORWARDBASE
            // apply ambient and vertex lighting
            c.rgb += i.sh * i.color;
        #endif
    #endif

    return c;
}
#endif

#ifdef UNITY_PASS_DEFERRED //ifdef to prevent compile errors
void frag_deferred (v2f i,
    out half4 outGBuffer0 : SV_Target0,
    out half4 outGBuffer1 : SV_Target1,
    out half4 outGBuffer2 : SV_Target2,
    out half4 outEmission : SV_Target3){

    UNITY_SETUP_INSTANCE_ID(i);

    if (!Visible(i.worldPos)) discard;

    #ifdef SPLIT_FRONT
        VoxelgonStandardData data;
        data.diffuseColor	= i.color;
        data.occlusion		= 1;
        data.normalWorld	= i.worldNormal;
        data.hardness       = _Hardness;

        VoxelgonStandardDataToGbuffer(data, outGBuffer0, outGBuffer1, outGBuffer2);

        half3 emission = i.color * i.sh;
        #ifdef UNITY_HDR_ON
            outEmission = half4(emission, 1);
        #else
            outEmission = half4(exp2(-emission), 1);
        #endif
    #else
       VoxelgonStandardData data;
        data.diffuseColor	= half4(1,0,0,1);
        data.occlusion		= 0;
        data.normalWorld	= _PlaneNormal;
        data.hardness       = _Hardness;

        VoxelgonStandardDataToGbuffer(data, outGBuffer0, outGBuffer1, outGBuffer2);

        outEmission = 0;
    #endif
}
#endif

#endif