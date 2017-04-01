Shader "Voxelgon/Editor Grid" {
    Properties {
        _Color ("Color", Color) = (1,1,1)
        _Offset ("Offset", Vector) = (0, 0, 0, 0)
        _Radius ("Circle Radius", Range(0, 5)) = 4
        _UnitRadius ("Unit Radius", Range(0, 2)) = 0.15
        _MinRadius ("Min Radius", Range(0, 1)) = 0.5
    }
    SubShader {
        Tags { "RenderType"="opaque" }
        Lighting Off
        LOD 200

        CGPROGRAM
        #pragma surface surf Unlit nodirlightmap nolightmap vertex:vert 
        #include "Tessellation.cginc"

        fixed3 _Color;
        half2 _Offset;
        fixed _Radius;
        fixed _UnitRadius;
        fixed _MinRadius;

        half4 LightingUnlit(SurfaceOutput s, half3 lightDir, half atten) {
            half4 c;
            c.rgb = s.Albedo;
            return c;
        }

        struct Input {
            float2 uv_MainTex;
        };
 
        void surf(Input IN, inout SurfaceOutput o) {
            o.Albedo = _Color;

        }

        void vert (inout appdata_full v) {
            half2 delta = (v.texcoord - 0.5 - (_Offset / 10));
            half distance = (delta.x * delta.x) + (delta.y * delta.y);
            if (distance < _Radius) {
                v.vertex.xyz -= v.normal * _MinRadius * lerp((1 - _UnitRadius), 1,  distance / _Radius);
            } else {
                v.vertex.xyz = float3(0,0,0);
            }
        }
        ENDCG
    }
    FallBack "Unlit"
}
