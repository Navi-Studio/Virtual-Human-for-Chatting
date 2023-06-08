/**
 * Copyright(c) Live2D Inc. All rights reserved.
 *
 * Use of this source code is governed by the Live2D Open Software license
 * that can be found at https://www.live2d.com/eula/live2d-open-software-license-agreement_en.html.
 */


namespace Live2D.Cubism.Rendering
{
    /// <summary>
    /// Cubism shader variables.
    /// </summary>
    internal static class CubismShaderVariables
    {
        /// <summary>
        /// Main texture shader variable name.
        /// </summary>
        public const string MainTexture = "_MainTex";


        /// <summary>
        /// Model opacity shader variable name.
        /// </summary>
        public const string ModelOpacity = "cubism_ModelOpacity";


        /// <summary>
        /// Diffuse color shader variable name.
        /// </summary>
        public const string MultiplyColor = "cubism_MultiplyColor";


        /// <summary>
        /// Tint color shader variable name.
        /// </summary>
        public const string ScreenColor = "cubism_ScreenColor";


        /// <summary>
        /// Mask texture shader variable name.
        /// </summary>
        public const string MaskTexture = "cubism_MaskTexture";

        /// <summary>
        /// Mask tile shader variable name.
        /// </summary>
        public const string MaskTile = "cubism_MaskTile";

        /// <summary>
        /// Mask transform shader variable name.
        /// </summary>
        public const string MaskTransform = "cubism_MaskTransform";
    }
}
