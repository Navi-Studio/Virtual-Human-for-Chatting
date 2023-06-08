/**
 * Copyright(c) Live2D Inc. All rights reserved.
 *
 * Use of this source code is governed by the Live2D Open Software license
 * that can be found at https://www.live2d.com/eula/live2d-open-software-license-agreement_en.html.
 */


namespace Live2D.Cubism.Rendering.Masking
{
    /// <summary>
    /// Holds mask properties.
    /// </summary>
    public sealed class CubismMaskProperties
    {
        /// <summary>
        /// RenderTexture to draw masks
        /// </summary>
        public CubismMaskTexture Texture;

        /// <summary>
        /// Tile where masks are drawn on Texture
        /// </summary>
        public CubismMaskTile Tile;

        /// <summary>
        /// Transform info to draw masks on Texture
        /// </summary>
        public CubismMaskTransform Transform;
    }
}
