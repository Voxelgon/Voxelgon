Shader "Voxelgon/Split" {
    Properties {
        _Hardness ("Hardness", Range(0, 1)) = 0.5
        _BaseColor ("Base Color", Color) = (1,1,1)
        _PlaneNormal("PlaneNormal",Vector) = (0,1,0,0)
		_PlanePosition("PlanePosition",Vector) = (0,0,0,1)
        _StencilMask("Stencil Mask", Range(0, 255)) = 255
    }
    SubShader {
        Tags { "RenderType"="opaque" }

        Stencil {
            Ref [_StencilMask]
            CompBack Always
            PassBack Replace

            CompFront Always
            PassFront Zero
        }


        Cull Front

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

            #include "Voxelgon-SplitCore.cginc"
            ENDCG
        }

                Cull Back

        Pass {
            Name "DEFERRED"
            Tags { "LightMode" = "Deferred" }

            CGPROGRAM
            // compile directives
            #pragma vertex vert
            #pragma fragment frag_deferred
            #pragma exclude_renderers nomrt //exclude platforms without MRT
            #pragma multi_compile_prepassfinal noshadowmask nodynlightmap nodirlightmap nolightmap
            #pragma target 3.0

            #define UNITY_PASS_DEFERRED
            #define SPLIT_FRONT

            #include "Voxelgon-SplitCore.cginc"
            ENDCG
        }

    }
}