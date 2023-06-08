/**
 * Copyright(c) Live2D Inc. All rights reserved.
 *
 * Use of this source code is governed by the Live2D Open Software license
 * that can be found at https://www.live2d.com/eula/live2d-open-software-license-agreement_en.html.
 */


using UnityEngine;


namespace Live2D.Cubism.Rendering.Masking
{
    /// <summary>
    /// Single mask tile.
    /// </summary>
    public struct CubismMaskTile
    {
        #region Conversion

        /// <summary>
        /// Converts a <see cref="CubismMaskTile"/> to a <see cref="Vector4"/>.
        /// </summary>
        /// <param name="value">Value to convert.</param>
        public static implicit operator Vector4(CubismMaskTile value)
        {
            return new Vector4
            {
                x = value.Channel,
                y = value.Column,
                z = value.Row,
                w = value.Size
            };
        }

        #endregion

        /// <summary>
        /// Color channel of the tile.
        /// </summary>
        /// <remarks>
        /// Valid values are 0f, 1f, 2, and 3f.
        /// </remarks>
        public float Channel;

        /// <summary>
        /// Column index of the tile in subdivided texture.
        /// </summary>
        public float Column;

        /// <summary>
        /// Row index of the tile in subdivided texture.
        /// </summary>
        public float Row;

        /// <summary>
        /// Size of the tile in texture coordinates.
        /// </summary>
        public float Size;
    }
}
