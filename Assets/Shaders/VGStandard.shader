Shader "Voxelgon/Standard" {
    Properties {
        _Hardness ("Hardness", Range(0, 1)) = 0.5
        _BaseColor ("Base Color", Color) = (1,1,1)
    }
    SubShader {
        Tags { "RenderType"="opaque" }
        LOD 200

        CGPROGRAM
        #pragma surface surf WrapLambert addshadow nodirlightmap nolightmap

        fixed _Hardness;
        fixed3 _BaseColor;

        struct Input {
            fixed4 color: Color; // Vertex color
        };


        half4 LightingWrapLambert(SurfaceOutput s, half3 lightDir, half atten) {
            half NdotL = dot (s.Normal, lightDir);
            NdotL = (NdotL * _Hardness) + 1.0 - _Hardness; //wrap surface normal
            half4 c;
            c.rgb = s.Albedo * _LightColor0.rgb * (NdotL * atten);
            return c;
        }

        void surf(Input IN, inout SurfaceOutput o) {
            o.Albedo = lerp(_BaseColor.rgb, IN.color.rgb, IN.color.a);
        }
        ENDCG
    }
    FallBack "VertexLit"
}
