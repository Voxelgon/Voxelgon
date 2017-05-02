Shader "Voxelgon/Effects/Split" {
    Properties {
        _Hardness ("Hardness", Range(0, 1)) = 0.5
        _BaseColor ("Base Color", Color) = (1,1,1)
        _PlaneNormal("PlaneNormal",Vector) = (0,1,0,0)
        _PlanePosition("PlanePosition",Vector) = (0,0,0,1)
        _StencilVal("Stencil Value", int) = 255
    }
    SubShader {
        Tags { "RenderType"="opaque" }

        Pass {
            Name "DEFERRED"
            Tags { "LightMode" = "Deferred" }
            Cull Back

            CGPROGRAM
            // compile directives
            #pragma vertex vert
            #pragma fragment frag_deferred
            #pragma exclude_renderers nomrt //exclude platforms without MRT
            #pragma multi_compile_prepassfinal noshadowmask nodynlightmap nodirlightmap nolightmap

            #define UNITY_PASS_DEFERRED
            #define SPLIT_DRAW

            #include "Voxelgon-SplitCore.cginc"
            ENDCG
        }

        Pass {
            Stencil {
                ref [_StencilVal]
                WriteMask 31
                Comp Always
                Pass Replace
            }
            Tags { "LightMode" = "Deferred" }
            Cull Front

            CGPROGRAM
            // compile directives
            #pragma vertex vert
            #pragma fragment frag_deferred
            #pragma exclude_renderers nomrt //exclude platforms without MRT
            #pragma multi_compile_prepassfinal noshadowmask nodynlightmap nodirlightmap nolightmap

            #define UNITY_PASS_DEFERRED

            #include "Voxelgon-SplitCore.cginc"
            ENDCG
        }
    } 
}