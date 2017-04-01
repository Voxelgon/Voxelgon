Shader "Voxelgon/Universe/Galaxy Backdrop" {
    Properties {
        _Alpha ("Alpha", Range(0, 1)) = 0.1 
        _BaseColor ("Base Color", Color) = (1, 1, 1, 1)
    }
    SubShader {
        Tags { "Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent" }
        LOD 200
        Blend SrcAlpha One
        Cull Off Lighting Off ZWrite Off


        CGPROGRAM
        #pragma surface surf Unlit alpha:fade vertex:vert nodirlightmap nolightmap noforwardadd

        #include "../Dither.cginc"

        fixed _Alpha;
        fixed4 _BaseColor;

        SETUP_DITHER

        struct Input {
            fixed4 color: Color; // Vertex color
            fixed2 texcoord;
        };

        void vert(inout appdata_full v, out Input o) {
            o.texcoord = v.texcoord;
            o.color = v.color;
        }

        half4 LightingUnlit(SurfaceOutput s, half3 lightDir, half atten) {
            half4 c;
            c.a = s.Alpha;
            c.rgb = s.Albedo;
            return c;
        }

        void surf(Input IN, inout SurfaceOutput o) {
            half4 c;
            c.a =  smoothstep(0.5, 0, abs(0.5 - IN.texcoord.y));

            c.rgb = _BaseColor.rgb * _BaseColor.a * c.a;

            c -= DITHER(IN.texcoord);

            o.Albedo = c.rgb;
            o.Alpha = c.a;
        }
        ENDCG
    }
    FallBack "VertexLit"
}
