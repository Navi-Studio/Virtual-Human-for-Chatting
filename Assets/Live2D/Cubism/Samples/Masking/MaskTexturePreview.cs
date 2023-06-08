/**
 * Copyright(c) Live2D Inc. All rights reserved.
 *
 * Use of this source code is governed by the Live2D Open Software license
 * that can be found at https://www.live2d.com/eula/live2d-open-software-license-agreement_en.html.
 */


using Live2D.Cubism.Rendering.Masking;
using UnityEngine;


namespace Live2D.Cubism.Samples.Masking
{
    /// <summary>
    /// Allows previewing of a <see cref="CubismMaskTexture"/> by assigning it to an attached <see cref="Renderer"/>.
    /// </summary>
    [ExecuteInEditMode, RequireComponent(typeof(Renderer))]
    public class MaskTexturePreview : MonoBehaviour
    {
        /// <summary>
        /// Mask texture to preview.
        /// </summary>
        public CubismMaskTexture MaskTexture;

        #region Unity Event Handling

        /// <summary>
        /// Called by Unity. Applies <see cref="MaskTexture"/> to attached <see cref="Renderer"/>.
        /// </summary>
        private void Start()
        {
            var material = (!Application.isEditor || Application.isPlaying)
                ? GetComponent<Renderer>().material
                : GetComponent<Renderer>().sharedMaterial;


            material.mainTexture = (Texture)MaskTexture;
        }

        #endregion
    }
}
