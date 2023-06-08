/**
 * Copyright(c) Live2D Inc. All rights reserved.
 *
 * Use of this source code is governed by the Live2D Open Software license
 * that can be found at https://www.live2d.com/eula/live2d-open-software-license-agreement_en.html.
 */


Shader "Live2D Cubism/Mask"
{
    Properties
    {
        // Culling setting.
        _Cull("Culling", Int) = 0
    }
    SubShader
    {
        Tags
        {
            "Queue" = "Transparent"
            "IgnoreProjector" = "True"
            "RenderType" = "Transparent"
        }


        BindChannels{ Bind "Vertex", vertex Bind "texcoord", texcoord Bind "Color", color }


        LOD      100
        ZWrite   Off
        Lighting Off
        Cull     [_Cull]
        Blend    One One


        Pass
        {
            CGPROGRAM
            #pragma vertex   vert
            #pragma fragment frag

            #define CUBISM_MASK_ON


            #include "UnityCG.cginc"
            #include "CubismCG.cginc"


            struct appdata_t
            {
                float4 vertex   : POSITION;
                fixed4 color    : COLOR;
                float2 texcoord : TEXCOORD0;

            };


            struct v2f
            {
                float4 vertex   : SV_POSITION;
                fixed4 color    : COLOR;
                float2 texcoord : TEXCOORD0;
            };


            CUBISM_SHADER_VARIABLES


            v2f vert(appdata_t IN)
            {
                v2f OUT;


                CUBISM_TO_MASK_CLIP_POS(IN, OUT);


                OUT.color    = IN.color;
                OUT.texcoord = IN.texcoord;


                return OUT;
            }


            sampler2D _MainTex;


            fixed4 frag(v2f IN) : SV_Target
            {
                return CUBISM_MASK_CHANNEL * tex2D(_MainTex, IN.texcoord).a;

            }


            ENDCG
        }
    }
}
