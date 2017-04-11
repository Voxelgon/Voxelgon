Shader "Voxelgon/Standard" {
    Properties {
        _Hardness ("Hardness", Range(0, 1)) = 0.5
        _BaseColor ("Base Color", Color) = (1,1,1)
    }
    SubShader {
        Tags { "RenderType"="opaque" }
        LOD 200

        // ---- forward rendering base pass:
        Pass {
            Name "FORWARD"
            Tags { "LightMode" = "ForwardBase" }

            CGPROGRAM
            // compile directives
            #pragma vertex vert
            #pragma fragment frag_forward
            #pragma multi_compile_fwdbase noshadowmask nodynlightmap nodirlightmap nolightmap

            #define UNITY_PASS_FORWARDBASE

            #include "Voxelgon-StandardCore.cginc"
            ENDCG
        }

        // ---- forward rendering additive lights pass:
        Pass {
            Name "FORWARD"
            Tags { "LightMode" = "ForwardAdd" }
            ZWrite Off Blend One One

            CGPROGRAM
            // compile directives
            #pragma vertex vert
            #pragma fragment frag_forward
            #pragma multi_compile_fwdadd_fullshadows noshadowmask nodynlightmap nodirlightmap nolightmap

            #define UNITY_PASS_FORWARDADD

            #include "Voxelgon-StandardCore.cginc"
            ENDCG
        }
        // ---- deferred shading pass:
        Pass {
            Name "DEFERRED"
            Tags { "LightMode" = "Deferred" }

            CGPROGRAM
            // compile directives
            #pragma vertex vert
            #pragma fragment frag_deferred
            #pragma exclude_renderers nomrt //exclude platforms without MRT
            #pragma multi_compile_prepassfinal noshadowmask nodynlightmap nodirlightmap nolightmap

            #define UNITY_PASS_DEFERRED

            #include "Voxelgon-StandardCore.cginc"
            ENDCG
        }
    }
    FallBack "VertexLit"
}
