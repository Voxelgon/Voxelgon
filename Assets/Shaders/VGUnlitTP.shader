Shader "Voxelgon/Standard Unlit Transparent" {
    Properties {
        _Alpha ("Alpha", Range(0, 1)) = 0.1 
    }
    SubShader {
        Tags { "Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent" }
        LOD 200
        Blend SrcAlpha One
        Cull Off Lighting Off ZWrite Off


        CGPROGRAM
        #pragma surface surf Unlit alpha:fade addshadow nodirlightmap nolightmap noforwardadd

        half4 LightingUnlit(SurfaceOutput s, half3 lightDir, half atten) {
            half4 c;
            c.a = s.Alpha;
            c.rgb = s.Albedo;
            return c;
        }

        fixed _Alpha;

        struct Input {
            fixed4 color: Color; // Vertex color
        };

        void surf(Input IN, inout SurfaceOutput o) {
            o.Albedo = IN.color.rgb;
            o.Alpha = IN.color.a * _Alpha;
        }
        ENDCG
    }
    FallBack "VertexLit"
}
