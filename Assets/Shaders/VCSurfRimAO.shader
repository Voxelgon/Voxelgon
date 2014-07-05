Shader "Voxelgon/Vertex Colored Surf Shader (RimShaded+AO)" {
    Properties {
        _RimColor ("Rim Color", Color) = (0.26,0.19,0.16,0.0)
        _RimPower ("Rim Power", Range(0.5,8.0)) = 3.0
        _AOIntensity ("Ambient Occlusion Intensity", Range(0, 1)) = 1.0
        _AOPower ("Ambient Occlusion Power", Range(1, 10)) = 1.0

    }
    SubShader {
        Tags { "RenderType"="opaque" }
        LOD 200

        CGPROGRAM
        #pragma surface surf Lambert 

        float4 _RimColor;
        float _RimPower;
        float _AOIntensity;
        float _AOPower;

        struct Input {
            float3 viewDir;
            float4 color: Color; // Vertex color
        };

        void vert(inout appdata_full v)
        {
            v.color.a = 1-( pow((1-v.color.a)*_AOIntensity, _AOPower ) );
        }

        void surf(Input IN, inout SurfaceOutput o) {
            o.Albedo = IN.color.rgb * IN.color.a;
            half rim = 1.0 - saturate(dot (normalize(IN.viewDir), o.Normal));
            o.Emission = _RimColor.rgb * pow (rim, _RimPower);
        }
        ENDCG
    }
    FallBack "Diffuse"
}
