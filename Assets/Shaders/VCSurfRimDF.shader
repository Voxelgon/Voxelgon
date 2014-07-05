Shader "Voxelgon/Vertex Colored Surf Shader (RimShaded + Distance falloff)" {
    Properties {
        _RimColor ("Rim Color", Color) = (0.26,0.19,0.16,0.0)
        _RimPower ("Rim Power", Range(0.5,8.0)) = 3.0
        _falloffIntensity ("Distance Falloff Intensity", Float) = 0.2
        _falloffPower ("Distance Falloff Power", Float) = 1
        _falloffDM ( "Distance Falloff Multiplyer", Float) = 5
    }
    SubShader {
        Tags { "RenderType"="opaque"}
        LOD 200

        CGPROGRAM
        #pragma surface surf Lambert addshadow

        float4 _RimColor;
        float _RimPower;
        float _falloffIntensity;
        float _falloffPower;
        float _falloffDM;

        struct Input {
            float3 viewDir;
            float4 color: Color; // Vertex color
            float3 worldPos;
        };


        void surf(Input IN, inout SurfaceOutput o) {
            float dist = distance(_WorldSpaceCameraPos, IN.worldPos);
            float falloff = _falloffIntensity * (1 - (1 / (1 + pow (dist * _falloffDM, _falloffPower))));
            o.Albedo = IN.color.rgb - falloff;
            half rim = 1.0 - saturate(dot (normalize(IN.viewDir), o.Normal));
            o.Emission = _RimColor.rgb * pow (rim, _RimPower);
        }

        ENDCG
    }
    FallBack "vertexLit"
}
