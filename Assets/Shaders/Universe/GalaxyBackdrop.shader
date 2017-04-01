Shader "Voxelgon/Universe/Galaxy Backdrop" {
    Properties {
        _Alpha ("Alpha", Range(0, 1)) = 0.1 
        _Color1 ("Color 1", Color) = (1, 1, 1, 1)
        _Color2 ("Color 2", Color) = (1, 1, 1, 1)
        _Color3 ("Color 3", Color) = (1, 1, 1, 1)
        _Color4 ("Color 4", Color) = (1, 1, 1, 1)

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
        fixed4 _Color1;
        fixed4 _Color2;
        fixed4 _Color3;
        fixed4 _Color4;

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
            half v1 = saturate(smoothstep(0.5, 0, abs(0.5 - IN.texcoord.y)) -  IN.color.r);
            half v2 = saturate(smoothstep(0.5, 0, abs(0.5 - IN.texcoord.y)) -  IN.color.g);
            half v3 = saturate(smoothstep(0.5, 0, abs(0.5 - IN.texcoord.y)) -  IN.color.b);
            half v4 = saturate(smoothstep(0.5, 0, abs(0.5 - IN.texcoord.y)) -  IN.color.a);

            c.rgb = v1 * _Color1.rgb * _Color1.a;
            c.rgb += v2 * _Color2.rgb * _Color2.a; 
            c.rgb += v3 * _Color3.rgb * _Color3.a; 
            c.rgb += v4 * _Color4.rgb * _Color4.a; 

            c.a = v1 + v2 + v3 + v4;

            c -= DITHER(IN.texcoord);

            o.Albedo = c.rgb;
            o.Alpha = c.a * _Alpha;
        }
        ENDCG
    }
    FallBack "VertexLit"
}
