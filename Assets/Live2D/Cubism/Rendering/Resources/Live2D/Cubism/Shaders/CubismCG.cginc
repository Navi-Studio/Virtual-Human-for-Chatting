/**
 * Copyright(c) Live2D Inc. All rights reserved.
 *
 * Use of this source code is governed by the Live2D Open Software license
 * that can be found at https://www.live2d.com/eula/live2d-open-software-license-agreement_en.html.
 */


#ifndef CUBISM_CG_INCLUDED
#define CUBISM_CG_INCLUDED


#include "UnityCG.cginc"


inline float4 CubismGetMaskChannel(float4 tile)
{
    return tile.xxxx == float4(0, 1, 2, 3);
}

inline float4 CubismGetClippedMaskChannel(float2 coordinates, float4 tile)
{
    float2 bound = tile.yz * tile.w;


    float isInside =
        step(bound.x, coordinates.x)
        * step(bound.y, coordinates.y)
        * step(coordinates.x, bound.x + tile.w)
        * step(coordinates.y, bound.y + tile.w);


    return CubismGetMaskChannel(tile) * isInside;
}


inline float2 CubismToMaskCoordinates(float2 vertex, float4 tile, float4 transform)
{
    float2 result = vertex;


    float  scale = tile.w * transform.z;
    float2 offset = transform.xy;
    float2 origin = (tile.yz + float2(0.5, 0.5)) * tile.ww;


    result.xy -= offset;
    result *= scale;
    result.xy += origin;


    return result;
}

inline float4 CubismToMaskClipPos(float2 vertex, float4 tile, float4 transform)
{
    float2 result = CubismToMaskCoordinates(vertex, tile, transform);


    result *= 2;
    result.xy = float2(result.x, (result.y * _ProjectionParams.x));
    result.xy -= float2(1, _ProjectionParams.x);


    return float4(result.xy, 1, 1);
}


inline float CubismSampleMaskTexture(sampler2D tex, float4 channel, float2 coordinates)
{
    float4 texel = tex2D(tex, coordinates.xy) * channel;


    return texel.r + texel.g + texel.b + texel.a;
}


#if defined (CUBISM_MASK_ON) || defined(CUBISM_INVERT_ON)
#define CUBISM_MASK_SHADER_VARIABLES    \
    sampler2D cubism_MaskTexture;       \
    float4 cubism_MaskTile;             \
    float4 cubism_MaskTransform;


#define CUBISM_TO_MASK_CLIP_POS(IN, OUT) OUT.vertex = CubismToMaskClipPos(IN.vertex, cubism_MaskTile, cubism_MaskTransform);
#define CUBISM_MASK_CHANNEL CubismGetMaskChannel(cubism_MaskTile)


#define CUBISM_VERTEX_OUTPUT float2 cubism_MaskCoordinates : TEXCOORD3;
#define CUBISM_INITIALIZE_VERTEX_OUTPUT(IN, OUT) OUT.cubism_MaskCoordinates = CubismToMaskCoordinates(IN.vertex, cubism_MaskTile, cubism_MaskTransform);


#if defined(CUBISM_INVERT_ON)
#define CUBISM_APPLY_MASK(IN, COLOR)                                                                                        \
    float4 cubism_maskChannel = CubismGetClippedMaskChannel(IN.cubism_MaskCoordinates, cubism_MaskTile);                    \
    float  cubism_maskAlpha = CubismSampleMaskTexture(cubism_MaskTexture, cubism_maskChannel, IN.cubism_MaskCoordinates);   \
    COLOR *= 1.0 - cubism_maskAlpha;
#else
#define CUBISM_APPLY_MASK(IN, COLOR)                                                                                        \
    float4 cubism_maskChannel = CubismGetClippedMaskChannel(IN.cubism_MaskCoordinates, cubism_MaskTile);                    \
    float  cubism_maskAlpha = CubismSampleMaskTexture(cubism_MaskTexture, cubism_maskChannel, IN.cubism_MaskCoordinates);   \
    COLOR *= cubism_maskAlpha;
#endif
#else
#define CUBISM_MASK_SHADER_VARIABLES


#define CUBISM_TO_MASK_CLIP_POS(IN, OUT)
#define CUBISM_MASK_CHANNEL float4(1, 1, 1, 1)


#define CUBISM_VERTEX_OUTPUT
#define CUBISM_INITIALIZE_VERTEX_OUTPUT(IN, OUT)


#define CUBISM_APPLY_MASK(IN, COLOR)
#endif


#define CUBISM_SHADER_VARIABLES     \
    float cubism_ModelOpacity;      \
    CUBISM_MASK_SHADER_VARIABLES

#define CUBISM_APPLY_ALPHA(IN, COLOR)   \
    COLOR.rgb *= COLOR.a;               \
    CUBISM_APPLY_MASK(IN, COLOR);       \
    COLOR *= cubism_ModelOpacity;


#endif
