Shader "Voxelgon/Standard Unlit" {
    Properties {
        _BaseColor ("Base Color", Color) = (1,1,1)
    }
    SubShader {
        Tags { "RenderType"="opaque" }
        LOD 200

        CGPROGRAM
        #pragma surface surf Unlit addshadow nodirlightmap nolightmap noforwardadd

        fixed3 _BaseColor;

        struct Input {
            fixed4 color: Color; // Vertex color
        };


        half4 LightingUnlit(SurfaceOutput s, half3 lightDir, half atten) {
            half4 c;
            c.rgb = s.Albedo;
            return c;
        }

         void surf(Input IN, inout SurfaceOutput o) {
            o.Albedo = lerp(_BaseColor.rgb, IN.color.rgb, IN.color.a);
        }
        ENDCG
    }
    FallBack "VertexLit"
}
