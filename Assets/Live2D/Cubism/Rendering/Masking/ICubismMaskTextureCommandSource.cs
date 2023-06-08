/**
 * Copyright(c) Live2D Inc. All rights reserved.
 *
 * Use of this source code is governed by the Live2D Open Software license
 * that can be found at https://www.live2d.com/eula/live2d-open-software-license-agreement_en.html.
 */


namespace Live2D.Cubism.Rendering.Masking
{
    /// <summary>
    /// Common interface for mask draw sources.
    /// </summary>
    public interface ICubismMaskTextureCommandSource : ICubismMaskCommandSource
    {
        /// <summary>
        /// Queries the number of tiles needed by the source.
        /// </summary>
        /// <returns>The necessary number of tiles needed.</returns>
        int GetNecessaryTileCount();


        /// <summary>
        /// Assigns the tiles.
        /// </summary>
        /// <param name="value">Tiles to assign.</param>
        void SetTiles(CubismMaskTile[] value);
    }
}
