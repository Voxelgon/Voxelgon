Shader "Hidden/Voxelgon-DeferredShading" {
    Properties {
        _LightTexture0 ("", any) = "" {}
        _LightTextureB0 ("", 2D) = "" {}
        _ShadowMapTexture ("", any) = "" {}
        _SrcBlend ("", Float) = 1
        _DstBlend ("", Float) = 1
    }
    SubShader {

        // Pass 1: Lighting pass
        //  LDR case - Lighting encoded into a subtractive ARGB8 buffer
        //  HDR case - Lighting additively blended into floating point buffer
        Pass {
            ZWrite Off
            Blend [_SrcBlend] [_DstBlend]

            CGPROGRAM
            #pragma target 3.0
            #pragma vertex vert
            #pragma fragment frag
            #pragma multi_compile_lightpass
            #pragma multi_compile ___ UNITY_HDR_ON

            #pragma exclude_renderers nomrt

            #define DEFERRED

            #include "UnityCG.cginc"
            //#include "Voxelgon-Lighting.cginc"
            #include "Voxelgon-LightingDeferred.cginc"
            #include "Voxelgon-LightingCommon.cginc"
            #include "Voxelgon-GBuffer.cginc"

            #include "UnityStandardUtils.cginc"

            sampler2D _CameraGBufferTexture0;
            sampler2D _CameraGBufferTexture1;
            sampler2D _CameraGBufferTexture2;

            float _LightAsQuad;

            voxelgon_v2f_deferred vert(float4 vertex : POSITION, float3 normal : NORMAL) {
                voxelgon_v2f_deferred o;
                o.pos = UnityObjectToClipPos(vertex);
                o.uv = ComputeScreenPos(o.pos);
                o.ray = UnityObjectToViewPos(vertex) * float3(-1,-1,1);
                
                // normal contains a ray pointing from the camera to one of near plane's
                // corners in camera space when we are drawing a full screen quad.
                // Otherwise, when rendering 3D shapes, use the ray calculated here.
                o.ray = lerp(o.ray, normal, _LightAsQuad);
                
                return o;
            }

            #ifdef UNITY_HDR_ON
            half4
            #else
            fixed4
            #endif
            frag (voxelgon_v2f_deferred i) : SV_Target {
                float3 lightDir;
                float3 wpos;
                float2 uv;
                float atten, shadow;
                //UnityLight light;
                //UNITY_INITIALIZE_OUTPUT(UnityLight, light);
                VoxelgonDeferredCalculateLightParams (i, uv, lightDir, atten, shadow);

                // unpack Gbuffer
                half4 gbuffer0 = tex2D (_CameraGBufferTexture0, uv);
                half4 gbuffer1 = tex2D (_CameraGBufferTexture1, uv);
                half4 gbuffer2 = tex2D (_CameraGBufferTexture2, uv);
                VoxelgonStandardData data = VoxelgonStandardDataFromGbuffer(gbuffer0, gbuffer1, gbuffer2);

                half3 c = VoxelgonWrapLighting(data.diffuseColor, data.normalWorld, lightDir, _LightColor, atten, shadow, data.hardness);
                #ifdef UNITY_HDR_ON
                return half4(c, 1);
                #else
                return exp2(-half4(c, 1));
                #endif
            }
            ENDCG
        }


        // Pass 2: Final decode pass.
        // Used only with HDR off, to decode the logarithmic buffer into the main RT
        Pass {
            ZTest Always Cull Off ZWrite Off
            Stencil {
                ref [_StencilNonBackground]
                readmask [_StencilNonBackground]
                // Normally just comp would be sufficient, but there's a bug and only front face stencil state is set (case 583207)
                compback equal
                compfront equal
            }

            CGPROGRAM
            #pragma target 3.0
            #pragma vertex vert
            #pragma fragment frag
            #pragma exclude_renderers nomrt

            #include "UnityCG.cginc"

            sampler2D _LightBuffer;
            struct v2f {
                float4 vertex : SV_POSITION;
                float2 texcoord : TEXCOORD0;
            };

            v2f vert (float4 vertex : POSITION, float2 texcoord : TEXCOORD0)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(vertex);
                o.texcoord = texcoord.xy;
            #ifdef UNITY_SINGLE_PASS_STEREO
                o.texcoord = TransformStereoScreenSpaceTex(o.texcoord, 1.0f);
            #endif
                return o;
            }

            fixed4 frag (v2f i) : SV_Target {
                return -log2(tex2D(_LightBuffer, i.texcoord));
            }
            ENDCG 
        }

    }
    Fallback Off
}
