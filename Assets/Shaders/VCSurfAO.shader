Shader "Voxelgon/Vertex Colored Surf Shader (AO)" {
    Properties {
        _AOIntensity ("Ambient Occlusion Intensity", Range(0, 1)) = 1.0
        _AOPower ("Ambient Occlusion Power", Range(1, 10)) = 1.0

    }
    SubShader {
        Tags { "RenderType"="opaque" }
        LOD 200

        CGPROGRAM
        #pragma surface surf Lambert 

        float _AOIntensity;
        float _AOPower;

        struct Input {
            float4 color: Color; // Vertex color
        };

        void vert(inout appdata_full v)
        {
            v.color.a = 1-( pow((1-v.color.a)*_AOIntensity, _AOPower ) );
        }

        void surf(Input IN, inout SurfaceOutput o) {
            o.Albedo = IN.color.rgb * IN.color.a;
        }
        ENDCG
    }
    FallBack "Diffuse"
}
