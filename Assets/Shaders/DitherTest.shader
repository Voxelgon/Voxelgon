Shader "Voxelgon/DitherTest"
{
    Properties
    {
    }
 
    SubShader
    {
        Tags { "RenderType"="Opaque" "PreviewType"="Plane" }
        LOD 200
        //ZTest LEqual
        Cull Off Lighting Off 

 
        CGPROGRAM
        #pragma surface surf Unlit nodynlightmap nodirlightmap nolightmap noforwardadd
 
        #pragma target 3.0
 
        #include "UnityCG.cginc"
 
        sampler2D _TBlueNoise;

 
         float nrand( float2 n ) {
            return frac(sin(dot(n.xy, float2(12.9898, 78.233)))* 43758.5453);
        }

        float n2rand( float2 n ) {
            float t = 2.1442;//frac( _Time );
            float nrnd0 = nrand( n + 0.07*t );

            // Convert uniform distribution into triangle-shaped distribution.
            float orig = nrnd0*2.0-1.0;
            nrnd0 = orig*rsqrt(abs(orig));
            nrnd0 = max(-1.0,nrnd0); // Nerf the NaN generated by 0*rsqrt(0). Thanks @FioraAeterna!
            nrnd0 = nrnd0-sign(orig);
            
            return nrnd0;
        }


        struct Input {
            float2 texcoord : TEXCOORD0;
        };
 
        half4 LightingUnlit(SurfaceOutput s, half3 lightDir, half atten) {
            half4 c;
            c.rgb  = s.Albedo;
            return c;
        }

        void surf (Input IN, inout SurfaceOutput o) {
            float3 c = float3(1,1,1) * IN.texcoord.y / 2;
            float2 seed =IN.texcoord * 21.9 + _Time; 
            c += ((tex2D(_TBlueNoise, seed).a * 2) - 1) / 10;
            //c += n2rand(seed) / 10;
            //c += ((nrand(seed) * 2) - 1) / 10;
            o.Albedo = round(c * 10) / 10;
        }
        ENDCG
    }
}