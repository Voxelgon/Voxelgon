Shader "Voxelgon/Effects/Split-Cap-Unlit"
{
    Properties
    {
        _StripeFreq("Stripe Frequency", Range(0, 1)) = 0.1
        _StripeRatio("Stripe Ratio", Range(0, 1)) = 0.5
        _Color1 ("Stripe Color 1", Color) = (1,1,1)
        _Color2 ("Stripe Color 2", Color) = (0.5,0.5,0.5)
        _StencilVal ("Stencil Value", int) = 2
    }
    SubShader
    {
        Tags {"Queue" = "AlphaTest"}
        LOD 100

        Pass {
            Name "SPLIT_CAP_UNLIT"
            Tags { "LightMode" = "ForwardBase" }
            Stencil {
                ref [_StencilVal]
                ReadMask 31
                Comp Equal
                Pass Keep
            }

            CGPROGRAM
            
            // compile directives
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            float _StripeFreq;
            float _StripeRatio;

            fixed3 _Color1;
            fixed3 _Color2;

            // vertex-to-fragment interpolation data
            struct v2f {
                float4 pos : SV_POSITION;
                float2 uv : TEXCOORD0;
            };

            // common vertex shader
            v2f vert (appdata_base v) {
                UNITY_SETUP_INSTANCE_ID(v);
                v2f o;
                UNITY_INITIALIZE_OUTPUT(v2f,o);
                UNITY_TRANSFER_INSTANCE_ID(v,o);
                UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);

                o.pos = UnityObjectToClipPos(v.vertex);
                o.uv = v.texcoord;

                return o;
            }

            fixed4 frag (v2f i) : SV_Target {
                UNITY_SETUP_INSTANCE_ID(i);

                fixed val = (i.uv.x + i.uv.y) / _StripeFreq;
                return fixed4(lerp(_Color1, _Color2, frac(val) > _StripeRatio), 1);
            }
            ENDCG
        }

        Pass {
            Tags { "LightMode" = "MotionVectors" }
            Stencil {
                ref [_StencilVal]
                ReadMask 31
                Comp Equal
                Pass Zero
            }
            ZWrite Off

            CGPROGRAM
            // compile directives
            #pragma vertex VertMotionVectors
			#pragma fragment FragMotionVectors

            #include "UnityCG.cginc"
            #include "Voxelgon-SplitCore.cginc"

            // Object rendering things
            float4x4 _NonJitteredVP;
            float4x4 _PreviousVP;
            float4x4 _PreviousM;
            bool _HasLastPositionData;
            bool _ForceNoMotion;
            float _MotionVectorDepthBias;

            struct MotionVectorData {
                float4 transferPos : TEXCOORD0;
                float4 transferPosOld : TEXCOORD1;
                float4 pos : SV_POSITION;
                half3 worldPos : TEXCOORD2;

                UNITY_VERTEX_OUTPUT_STEREO
            };

            struct MotionVertexInput {
                float4 vertex : POSITION;
                float3 oldPos : NORMAL;
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            MotionVectorData VertMotionVectors(MotionVertexInput v) {
                MotionVectorData o;
                UNITY_SETUP_INSTANCE_ID(v);
                UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);
                o.pos = UnityObjectToClipPos(v.vertex);
                o.worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;

                // this works around an issue with dynamic batching
                // potentially remove in 5.4 when we use instancing
                #if defined(UNITY_REVERSED_Z)
                    o.pos.z -= _MotionVectorDepthBias * o.pos.w;
                #else
                    o.pos.z += _MotionVectorDepthBias * o.pos.w;
                #endif

                o.transferPos = mul(_NonJitteredVP, mul(unity_ObjectToWorld, v.vertex));
                o.transferPosOld = mul(_PreviousVP, mul(_PreviousM, _HasLastPositionData ? float4(v.oldPos, 1) : v.vertex));
                return o;
            }

            half4 FragMotionVectors(MotionVectorData i) : SV_Target {
                float3 hPos = (i.transferPos.xyz / i.transferPos.w);
                float3 hPosOld = (i.transferPosOld.xyz / i.transferPosOld.w);

                // V is the viewport position at this pixel in the range 0 to 1.
                float2 vPos = (hPos.xy + 1.0f) / 2.0f;
                float2 vPosOld = (hPosOld.xy + 1.0f) / 2.0f;

                #if UNITY_UV_STARTS_AT_TOP
                    vPos.y = 1.0 - vPos.y;
                    vPosOld.y = 1.0 - vPosOld.y;
                #endif

                half2 uvDiff = vPos - vPosOld;
                return lerp(half4(uvDiff, 0, 1), 0, (half)_ForceNoMotion * Visible(i.worldPos));
            }

            ENDCG
        }
    }
}
