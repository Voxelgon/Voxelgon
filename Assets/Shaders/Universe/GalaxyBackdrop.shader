Shader "Voxelgon/Galaxy Backdrop" {
    Properties {
        _Alpha ("Alpha", Range(0, 1)) = 0.1 
    }
    SubShader {
        Tags { "Queue"="Transparent"  "RenderType"="Transparent" }
        LOD 200
        Blend SrcAlpha One
        Cull Off Lighting Off ZWrite Off ZTest Less


        CGPROGRAM
        #pragma surface surf Unlit alpha:fade vertex:vert noforwardadd

        #include "../Voxelgon-Dither.cginc"

        half4 LightingUnlit(SurfaceOutput s, half3 lightDir, half atten) {
            half4 c;
            c.a = s.Alpha;
            c.rgb = s.Albedo;
            return c;
        }

        fixed _Alpha;
        half _Clip;

        struct Input {
            fixed4 color: Color; // Vertex color
            fixed2 texcoord;
            float4 projPos : TEXCOORD2;
        };

        void vert(inout appdata_full v, out Input i) {
            float4 hpos = UnityObjectToClipPos(v.vertex);
            i.projPos = ComputeGrabScreenPos(hpos);

            i.texcoord = v.texcoord;
            i.color = v.color;
        }

        void surf(Input i, inout SurfaceOutput o) {
            half4 c = i.color + DITHER(i.texcoord);
            o.Albedo = c.rgb;
            o.Alpha = c.a * _Alpha;
        }
        ENDCG
    }
    FallBack "VertexLit"
}
