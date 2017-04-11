Shader "Voxelgon/Editor Grid Fill"
{
    Properties
    {
        _Color ("Color", Color) = (1,1,1)
        _Alpha ("Alpha", Range(0, 1)) = 0.1 
        _Offset ("Offset", Vector) = (0, 0, 0, 0)
        _Radius ("Circle Radius", Range(0, 1)) = 0.2 
        _RadiusOffset ("Offset Radius", Range(0, 1)) = 0.05 
        _FadeCutoff ("Soft Particles Cutoff", Range(0, 1)) = 0.5
        _InvFade("Soft Particles Factor", Range(0.01,3.0)) = 1.0
    }
 
    SubShader
    {
        Pass {
            Tags { "Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent" "PreviewType"="Plane" }
            LOD 200
            Blend One OneMinusSrcAlpha
            Cull Off Lighting Off ZWrite Off
            CGPROGRAM

            // Upgrade NOTE: excluded shader from DX11; has structs without semantics (struct appdata_t members viewDir)
            #pragma exclude_renderers d3d11

            #pragma vertex vert
            #pragma fragment frag
            #pragma target 2.0
            #pragma multi_compile_particles
    
            #include "UnityCG.cginc"
            #include "../Voxelgon-Dither.cginc"

            fixed3 _Color;
            fixed _Alpha;
            half2 _Offset;
            fixed _Radius;
            fixed _RadiusOffset;
            float _FadeCutoff;
            float _InvFade;
            sampler2D_float _CameraDepthTexture;

            struct appdata_t {
                float4 vertex : POSITION;
                fixed4 color : COLOR;
                float2 texcoord : TEXCOORD0;
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            struct v2f {
                float4 vertex : SV_POSITION;
                fixed4 color : COLOR;
                float2 texcoord : TEXCOORD0;
                #ifdef SOFTPARTICLES_ON
                float4 projPos : TEXCOORD1;
                #endif
                UNITY_VERTEX_OUTPUT_STEREO
            };
            
            v2f vert (appdata_t v) {
                v2f o;
                UNITY_SETUP_INSTANCE_ID(v);
                UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);
                o.vertex = UnityObjectToClipPos(v.vertex);

                #ifdef SOFTPARTICLES_ON
                o.projPos = ComputeScreenPos (o.vertex);
                COMPUTE_EYEDEPTH(o.projPos.z);
                #endif

                o.color = v.color;
                o.texcoord = v.texcoord;
                return o;
            }

            fixed4 frag (v2f i) : SV_Target {
                half4 c;
                c.rgb = _Color;
                c.a =  _Alpha;

                float2 delta = (i.texcoord - 0.5 - (_Offset / 10));
                half radius = _Radius + _SinTime.w * _RadiusOffset;
                half dist =(delta.x * delta.x) + (delta.y * delta.y); 

                clip(radius - dist);

                c.a *= (smoothstep(radius, 0, dist)- 0.05);
                c.a *= 0.9 - (_SinTime.w * -0.2);

                #ifdef SOFTPARTICLES_ON
                float sceneZ = LinearEyeDepth (SAMPLE_DEPTH_TEXTURE_PROJ(_CameraDepthTexture, UNITY_PROJ_COORD(i.projPos)));
                half partZ = i.projPos.z;
                half fade = saturate(_InvFade * (sceneZ - partZ - _FadeCutoff));
                c.a *= fade;
                #endif

                c += DITHER(i.texcoord);
                return saturate(c * c.a);
            }
            ENDCG
        }
    }
}