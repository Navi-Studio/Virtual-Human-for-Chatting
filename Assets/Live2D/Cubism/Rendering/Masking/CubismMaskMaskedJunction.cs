/**
 * Copyright(c) Live2D Inc. All rights reserved.
 *
 * Use of this source code is governed by the Live2D Open Software license
 * that can be found at https://www.live2d.com/eula/live2d-open-software-license-agreement_en.html.
 */


using UnityEngine.Rendering;


namespace Live2D.Cubism.Rendering.Masking
{
    /// <summary>
    /// Many-to-many <see cref="CubismRenderer"/>-<see cref="CubismMaskRenderer"/> map.
    /// </summary>
    internal sealed class CubismMaskMaskedJunction
    {
        /// <summary>
        /// Shared buffer for <see cref="CubismMaskProperties"/>s.
        /// </summary>
        private static CubismMaskProperties SharedMaskProperties { get; set; }


        /// <summary>
        /// Masks.
        /// </summary>
        private CubismMaskRenderer[] Masks { get; set; }

        /// <summary>
        /// Masked drawables.
        /// </summary>
        private CubismRenderer[] Maskeds { get; set; }


        /// <summary>
        /// Mask texture to be referenced by <see cref="Maskeds"/>.
        /// </summary>
        private CubismMaskTexture MaskTexture { get; set; }

        /// <summary>
        /// Mask tile to write to and read from.
        /// </summary>
        private CubismMaskTile MaskTile { get; set; }

        /// <summary>
        /// Mask transform
        /// </summary>
        private CubismMaskTransform MaskTransform { get; set; }

        #region Ctors

        /// <summary>
        /// Makes sure statics are initialized.
        /// </summary>
        public CubismMaskMaskedJunction()
        {
            if (SharedMaskProperties != null)
            {
                return;
            }


            SharedMaskProperties = new CubismMaskProperties();
        }

        #endregion

        #region Interface For CubismMaskController

        /// <summary>
        /// Sets the masks.
        /// </summary>
        /// <param name="value">Value to set.</param>
        /// <returns>Instance.</returns>
        public CubismMaskMaskedJunction SetMasks(CubismMaskRenderer[] value)
        {
            Masks = value;


            return this;
        }

        /// <summary>
        /// Sets the masked drawables.
        /// </summary>
        /// <param name="value">Value to set.</param>
        /// <returns>Instance.</returns>
        public CubismMaskMaskedJunction SetMaskeds(CubismRenderer[] value)
        {
            Maskeds = value;


            return this;
        }

        /// <summary>
        /// Sets the mask texture to read from.
        /// </summary>
        /// <param name="value">Value to set.</param>
        /// <returns>Instance.</returns>
        public CubismMaskMaskedJunction SetMaskTexture(CubismMaskTexture value)
        {
            MaskTexture = value;


            return this;
        }

        /// <summary>
        /// Sets the mask tile to write to and read from.
        /// </summary>
        /// <param name="value">Value to set.</param>
        /// <returns>Instance.</returns>
        public CubismMaskMaskedJunction SetMaskTile(CubismMaskTile value)
        {
            MaskTile = value;


            return this;
        }


        /// <summary>
        /// Appends junction draw commands to a buffer.
        /// </summary>
        /// <param name="buffer">Buffer to append commands to.</param>
        public void AddToCommandBuffer(CommandBuffer buffer)
        {
            // Make sure mask transform is initialized.
            RecalculateMaskTransform();


            // Initialize and enqueue masks.
            for (var i = 0; i < Masks.Length; ++i)
            {
                Masks[i]
                    .SetMaskTile(MaskTile)
                    .SetMaskTransform(MaskTransform)
                    .AddToCommandBuffer(buffer);
            }
        }


        /// <summary>
        /// Updates the junction and all related data.
        /// </summary>
        internal void Update()
        {
            // Update mask transform.
            RecalculateMaskTransform();


            // Apply transform to masks.
            for (var i = 0; i < Masks.Length; ++i)
            {
                Masks[i].SetMaskTransform(MaskTransform);
            }


            // Apply transform and other properties to maskeds.
            var maskProperties = SharedMaskProperties;


            maskProperties.Texture = MaskTexture;
            maskProperties.Tile = MaskTile;
            maskProperties.Transform = MaskTransform;


            for (var i = 0; i < Maskeds.Length; ++i)
            {
                Maskeds[i].OnMaskPropertiesDidChange(maskProperties);
            }
        }


        #endregion


        /// <summary>
        /// Updates <see cref="MaskTransform"/> and <see cref="Maskeds"/>.
        /// </summary>
        private void RecalculateMaskTransform()
        {
            // Compute bounds and scale.
            var bounds = Masks.GetBounds();
            var scale = (bounds.size.x > bounds.size.y)
                ? bounds.size.x
                : bounds.size.y;


            // Compute mask transform.
            MaskTransform = new CubismMaskTransform
            {
                Offset = bounds.center,
                Scale = 1f / scale
            };
        }
    }
}
