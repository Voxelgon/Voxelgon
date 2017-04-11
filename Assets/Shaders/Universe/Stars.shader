Shader "Voxelgon/Stars" {
    Properties {
        _Alpha ("Alpha", Range(0, 1)) = 0.1 
        _Clip ("Clip Threshold", Range(0, 0.1)) = 0.001
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
        sampler2D_float _CameraDepthTexture;


        struct Input {
            fixed4 color: Color; // Vertex color
            float4 projPos : TEXCOORD2;
        };

        void surf(Input i, inout SurfaceOutput o) {
            float depth = _Clip + Linear01Depth(SAMPLE_DEPTH_TEXTURE_PROJ(_CameraDepthTexture, UNITY_PROJ_COORD(i.projPos)));
            clip(depth - 1);
            o.Albedo = i.color.rgb * depth;
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
