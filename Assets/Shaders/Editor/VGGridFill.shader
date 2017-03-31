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
        Tags { "Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent" "PreviewType"="Plane" }
        LOD 200
        //ZTest LEqual
        Blend SrcAlpha One
        Cull Off Lighting Off ZWrite Off

 
        CGPROGRAM
        #pragma surface surf Unlit vertex:vert alpha:fade 
        #pragma nodynlightmap nodirlightmap nolightmap noforwardadd
        #pragma multi_compile _ SOFTPARTICLES_ON
        #pragma multi_compile_fog
 
        #pragma target 3.0
 
        #include "UnityCG.cginc"
 
        fixed3 _Color;
        fixed _Alpha;
        half2 _Offset;
        fixed _Radius;
        fixed _RadiusOffset;
        float _FadeCutoff;
        float _InvFade;
        sampler2D_float _CameraDepthTexture;

        sampler2D _TBlueNoise;

 
        struct Input {
            float3 viewDir;
            float2 uv_MainTex;
            UNITY_FOG_COORDS(1)
            float2 texcoord : TEXCOORD0;
            #ifdef SOFTPARTICLES_ON
            float4 projPos : TEXCOORD2;
            #endif
        };
 

        half4 LightingUnlit(SurfaceOutput s, half3 lightDir, half atten) {
            half4 c;
            c.rgb  = s.Albedo;
            c.a = s.Alpha;
            return c;
        }

        void vert(inout appdata_full v, out Input o) {
            //set custom value
            o.uv_MainTex = v.texcoord;
            o.texcoord = v.texcoord;
            o.viewDir = WorldSpaceViewDir( v.vertex );
            #ifdef SOFTPARTICLES_ON
            float4 hpos = mul(UNITY_MATRIX_MVP, v.vertex);
            o.projPos = ComputeGrabScreenPos(hpos);
            #endif
        }
 
        void surf (Input IN, inout SurfaceOutput o) {
            half4 c;
            c.rgb = _Color;
            c.a =  _Alpha;

            float2 delta = (IN.texcoord - 0.5 - (_Offset / 10));
            half radius = _Radius + _SinTime.w * _RadiusOffset;
            half dist =(delta.x * delta.x) + (delta.y * delta.y); 
            clip(radius - dist);
            c.a *= (smoothstep(radius, 0, dist)- 0.05);
            c.a *= saturate(dot(IN.viewDir, o.Normal));

            #ifdef SOFTPARTICLES_ON
            half sceneZ = LinearEyeDepth(SAMPLE_DEPTH_TEXTURE_PROJ(_CameraDepthTexture, UNITY_PROJ_COORD(IN.projPos)));
            half partZ = IN.projPos.z;
            half fade = saturate(_InvFade * (sceneZ - partZ - _FadeCutoff));
            c.a *= fade;
            #endif

            o.Albedo = c.rgb;
            o.Alpha = c.a + ((tex2D(_TBlueNoise, IN.texcoord * 21.9 + _Time).a * 2) - 1) / 256;
            UNITY_APPLY_FOG(IN.fogCoord, c);
        }
        ENDCG
    }
}