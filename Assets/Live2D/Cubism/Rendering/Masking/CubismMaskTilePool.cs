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
    /// Virtual pool allocator for <see cref="CubismMaskTile"/>s.
    /// </summary>
    internal sealed class CubismMaskTilePool
    {
        /// <summary>
        /// Level of subdivisions.
        /// </summary>
        private int Subdivisions { get; set; }

        /// <summary>
        /// Pool slots.
        /// </summary>
        /// <remarks>
        /// <see langword="true"/> slots are in use, <see langword="false"/> are available slots.
        /// </remarks>
        private bool[] Slots { get; set; }

        #region Ctors

        /// <summary>
        /// Initializes instance.
        /// </summary>
        /// <param name="subdivisions">Number of <see cref="CubismMaskTexture"/> subdivisions.</param>
        /// <param name="channels">Number of <see cref="CubismMaskTexture"/> color channels.</param>
        public CubismMaskTilePool(int subdivisions, int channels)
        {
            Subdivisions = subdivisions;


            Slots = new bool[(int)Mathf.Pow(4, subdivisions) * channels];
        }

        #endregion

        /// <summary>
        /// Acquires tiles.
        /// </summary>
        /// <param name="count">Number of tiles to acquire.</param>
        /// <returns>Acquired tiles on success; <see langword="null"/> otherwise.</returns>
        public CubismMaskTile[] AcquireTiles(int count)
        {
            var result = new CubismMaskTile[count];


            // Populate container.
            for (var i = 0; i < count; ++i)
            {
                var allocationSuccessful = false;


                for (var j = 0; j < Slots.Length; ++j)
                {
                    // Skip occupied slots.
                    if (Slots[j])
                    {
                        continue;
                    }


                    // Generate tile.
                    result[i] = ToTile(j);


                    // Flag slot as occupied.
                    Slots[j] = true;


                    // Flag allocation as successful.
                    allocationSuccessful = true;


                    break;
                }


                // Return as soon as one allocation fails.
                if (!allocationSuccessful)
                {
                    return null;
                }
            }


            // Return on success.
            return result;
        }

        /// <summary>
        /// Releases tiles.
        /// </summary>
        /// <param name="tiles">Tiles to release.</param>
        public void ReturnTiles(CubismMaskTile[] tiles)
        {
            // Flag slots as available.
            for (var i = 0; i < tiles.Length; ++i)
            {
                Slots[ToIndex(tiles[i])] = false;
            }
        }


        /// <summary>
        /// Converts from index to <see cref="CubismMaskTile"/>.
        /// </summary>
        /// <param name="index">Index to convert.</param>
        /// <returns>Mask tile matching index.</returns>
        private CubismMaskTile ToTile(int index)
        {
            var tileCounts = (int)Mathf.Pow(4, Subdivisions - 1);
            var tilesPerRow = (int)Mathf.Pow(2, Subdivisions - 1);
            var tileSize = 1f / (float)tilesPerRow;


            var channel = index / tileCounts;
            var currentTilePosition = index - (channel * tileCounts);
            var column = currentTilePosition / tilesPerRow;
            var rowId = currentTilePosition % tilesPerRow;


            return new CubismMaskTile
            {
                Channel = channel,
                Column = column,
                Row = rowId,
                Size = tileSize
            };
        }

        /// <summary>
        /// Converts from <see cref="CubismMaskTile"/> to index.
        /// </summary>
        /// <param name="tile">Tile to convert.</param>
        /// <returns>Tile index.</returns>
        private int ToIndex(CubismMaskTile tile)
        {
            var tileCounts = (int)Mathf.Pow(4, Subdivisions - 1);
            var tilesPerRow = (int)Mathf.Pow(2, Subdivisions - 1);


            return (int)((tile.Channel * tileCounts) + (tile.Column * tilesPerRow) + tile.Row);
        }
    }
}
