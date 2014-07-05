Shader "Voxelgon/Vertex Colored Surf Shader" {
    Properties {
    }
    SubShader {
        Tags { "RenderType"="opaque" }
        LOD 200

        CGPROGRAM
        #pragma surface surf Lambert addshadow nodirlightmap nolightmap

        struct Input {
            float4 color: Color; // Vertex color
        };
 
        void surf(Input IN, inout SurfaceOutput o) {
            o.Albedo = IN.color.rgb;
        }
        ENDCG
    }
    FallBack "VertexLit"
}
