Shader "Voxelgon/Standard-Unlit"
{
    Properties
    {
        _BaseColor ("Base Color", Color) = (1,1,1)
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 100

        Pass {
            Name "FORWARD"
            Tags { "LightMode" = "ForwardBase" }

            CGPROGRAM
            // compile directives
            #pragma vertex vert
            #pragma fragment frag
            #pragma multi_compile_fwdbase noshadowmask nodynlightmap nodirlightmap nolightmap
            #pragma multi_compile_instancing

            #define UNITY_PASS_FORWARDBASE

            #include "UnityCG.cginc"

            fixed4 _BaseColor;

            struct appdata {
                float4 vertex : POSITION;
                fixed4 color : COLOR0;

                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            struct v2f {
                float4 pos : SV_POSITION;
                fixed3 color : COLOR0;

                UNITY_VERTEX_INPUT_INSTANCE_ID
                UNITY_VERTEX_OUTPUT_STEREO
            };

            v2f vert(appdata v) {
                UNITY_SETUP_INSTANCE_ID(v);
                v2f o;
                UNITY_INITIALIZE_OUTPUT(v2f,o);
                UNITY_TRANSFER_INSTANCE_ID(v,o);
                UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);


                o.pos = UnityObjectToClipPos(v.vertex);
                o.color = lerp(_BaseColor.rgb, v.color.rgb, v.color.a);

                return o;
            }

            float4 frag(v2f i) : SV_Target {
                UNITY_SETUP_INSTANCE_ID(i);
                return fixed4(i.color, 1);
            }

            ENDCG
        }

        Pass {
            Name "DEFERRED"
            Tags { "LightMode" = "Deferred" }

            CGPROGRAM
            // compile directives
            #pragma vertex vert
            #pragma fragment frag
            #pragma exclude_renderers nomrt //exclude platforms without MRT
            #pragma multi_compile_prepassfinal noshadowmask nodynlightmap nodirlightmap nolightmap
            #pragma multi_compile_instancing

            #define UNITY_PASS_FORWARDBASE

            #include "UnityCG.cginc"
            #include "Voxelgon-GBuffer.cginc"

            fixed4 _BaseColor;

            struct appdata {
                float4 vertex : POSITION;
                fixed4 color : COLOR0;

                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            struct v2f {
                float4 pos : SV_POSITION;
                fixed3 color : COLOR0;

                UNITY_VERTEX_INPUT_INSTANCE_ID
                UNITY_VERTEX_OUTPUT_STEREO
            };

            v2f vert(appdata v) {
                UNITY_SETUP_INSTANCE_ID(v);
                v2f o;
                UNITY_INITIALIZE_OUTPUT(v2f,o);
                UNITY_TRANSFER_INSTANCE_ID(v,o);
                UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);


                o.pos = UnityObjectToClipPos(v.vertex);
                o.color = lerp(_BaseColor.rgb, v.color.rgb, v.color.a);

                return o;
            }

            void frag (v2f i,
                out half4 outGBuffer0 : SV_Target0,
                out half4 outGBuffer1 : SV_Target1,
                out half4 outGBuffer2 : SV_Target2,
                out half4 outEmission : SV_Target3){

                UNITY_SETUP_INSTANCE_ID(i);

                VoxelgonFillGBufferUnlit(i.color, outGBuffer0, outGBuffer1, outGBuffer2, outEmission);
            }
            ENDCG
        }
    }
}
