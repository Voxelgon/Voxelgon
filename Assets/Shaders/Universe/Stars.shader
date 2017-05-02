Shader "Voxelgon/Universe/Stars" {
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
            float4 projPos : TEXCOORD2;
        };

        void surf(Input i, inout SurfaceOutput o) {
            o.Albedo = i.color.rgb;
            o.Alpha = i.color.a * _Alpha;
        }

        void vert(inout appdata_full v, out Input i) {
            i.color = v.color;

            float4 hpos = UnityObjectToClipPos(v.vertex);
            i.projPos = ComputeGrabScreenPos(hpos);
        }

        ENDCG
    }
    FallBack "VertexLit"
}
