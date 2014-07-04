Shader "Voxelgon/Colormap-VertexLit" {
    Properties {
    }

    Subshader {
        pass {
            Cull Back
            Lighting On

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            struct a2v {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
                float4 color : COLOR;
            };

            struct v2f {
                float4 pos : POSITION;
                float3 color : COLOR;
            };

            v2f vert (a2v v) {
                v2f o;
                o.pos = mul(UNITY_MATRIX_MVP, v.vertex);
                o.color = ShadeVertexLights(v.vertex, v.normal) * v.color.rgb;

                return o;
            }

            float4 frag (v2f i) : COLOR {
                float4 c;
                c.rgb = i.color * 2;

                return c;
            }


            ENDCG

        }
    }
}
