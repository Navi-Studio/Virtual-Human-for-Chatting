/**
 * Copyright(c) Live2D Inc. All rights reserved.
 *
 * Use of this source code is governed by the Live2D Open Software license
 * that can be found at https://www.live2d.com/eula/live2d-open-software-license-agreement_en.html.
 */


using Live2D.Cubism.Core;
using UnityEngine;
using UnityEngine.Rendering;


namespace Live2D.Cubism.Rendering.Masking
{
    /// <summary>
    /// Renders out a single Cubism mask.
    /// </summary>
    /// <remarks>
    /// Note that - depending on the model - multiple <see cref="CubismMaskRenderer"/> might be assigned to a single <see cref="CubismDrawable"/>.
    /// </remarks>
    internal sealed class CubismMaskRenderer
    {
        /// <summary>
        /// Mask properties.
        /// </summary>
        private MaterialPropertyBlock MaskProperties { get; set; }


        /// <summary>
        /// Main renderer.
        /// </summary>
        private CubismRenderer MainRenderer { get; set; }


        /// <summary>
        /// Mask material.
        /// </summary>
        private Material MaskMaterial { get; set; }

        /// <summary>
        /// Mask culling material.
        /// </summary>
        private Material MaskCullingMaterial { get; set; }

        /// <summary>
        /// Culling setting.
        /// </summary>
        private bool IsCulling { get; set; }

        /// <summary>
        /// Bounds of <see cref="CubismRenderer.Mesh"/>.
        /// </summary>
        internal Bounds MeshBounds
        {
            get { return MainRenderer.Mesh.bounds; }
        }

        #region Ctors

        /// <summary>
        /// Initializes fields.
        /// </summary>
        public CubismMaskRenderer()
        {
            MaskProperties = new MaterialPropertyBlock();
            MaskMaterial = CubismBuiltinMaterials.Mask;
            MaskCullingMaterial = CubismBuiltinMaterials.MaskCulling;
        }

        #endregion

        #region Interface For CubismMaskMaskedJunction

        /// <summary>
        /// Sets the <see cref="CubismRenderer"/> to reference.
        /// </summary>
        /// <param name="value">Value to set.</param>
        /// <returns>Instance.</returns>
        internal CubismMaskRenderer SetMainRenderer(CubismRenderer value)
        {
            MainRenderer = value;

            IsCulling = !(MainRenderer.gameObject.GetComponent<CubismDrawable>().IsDoubleSided);

            return this;
        }

        /// <summary>
        /// Sets <see cref="CubismMaskTile"/>.
        /// </summary>
        /// <param name="value">Value to set.</param>
        /// <returns>Instance.</returns>
        internal CubismMaskRenderer SetMaskTile(CubismMaskTile value)
        {
            MaskProperties.SetVector(CubismShaderVariables.MaskTile, value);


            return this;
        }

        /// <summary>
        /// Sets <see cref="CubismMaskTransform"/>.
        /// </summary>
        /// <param name="value">Value to set.</param>
        /// <returns>Instance.</returns>
        internal CubismMaskRenderer SetMaskTransform(CubismMaskTransform value)
        {
            MaskProperties.SetVector(CubismShaderVariables.MaskTransform, value);


            return this;
        }


        /// <summary>
        /// Enqueues
        /// </summary>
        /// <param name="buffer">Buffer to enqueue in.</param>
        internal void AddToCommandBuffer(CommandBuffer buffer)
        {
            // Lazily fetch drawable texture and mesh.
            var mainTexture = MainRenderer.MainTexture;
            var mesh = MainRenderer.Mesh;


            MaskProperties.SetTexture(CubismShaderVariables.MainTexture, mainTexture);


            // Add command.
            buffer.DrawMesh(mesh, Matrix4x4.identity,
                IsCulling
                    ? MaskCullingMaterial
                    : MaskMaterial,
                0, 0, MaskProperties);
        }

        #endregion
    }
}
