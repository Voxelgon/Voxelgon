Shader "Voxelgon/Vertex Colored Surf Shader (RimShaded) (No Culling)" {
    Properties {
        _RimColor ("Rim Color", Color) = (0.26,0.19,0.16,0.0)
        _RimPower ("Rim Power", Range(0.5,8.0)) = 3.0
    }
    SubShader {
        Tags { "RenderType"="opaque" }
        LOD 200
        Cull Off
        

        CGPROGRAM
        #pragma surface surf Lambert addshadow nodirlightmap nolightmap

        float4 _RimColor;
        float _RimPower;

        struct Input {
            float3 viewDir;
            float4 color: Color; // Vertex color
        };


        void surf(Input IN, inout SurfaceOutput o) {
            o.Albedo = IN.color.rgb;
            half rim = 1.0 - saturate(dot (normalize(IN.viewDir), o.Normal));
            o.Emission = _RimColor.rgb * pow (rim, _RimPower);
        }
        ENDCG
    }
    FallBack "VertexLit"
}
