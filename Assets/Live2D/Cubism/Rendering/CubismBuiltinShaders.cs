/**
 * Copyright(c) Live2D Inc. All rights reserved.
 *
 * Use of this source code is governed by the Live2D Open Software license
 * that can be found at https://www.live2d.com/eula/live2d-open-software-license-agreement_en.html.
 */


using UnityEngine;


namespace Live2D.Cubism.Rendering
{
    /// <summary>
    /// Default shader assets.
    /// </summary>
    public static class CubismBuiltinShaders
    {
        /// <summary>
        /// Default unlit shader.
        /// </summary>
        public static Shader Unlit
        {
            get
            {
                return Shader.Find("Live2D Cubism/Unlit");
            }
        }

        /// <summary>
        /// Shader for drawing masks.
        /// </summary>
        public static Shader Mask
        {
            get
            {
                return Shader.Find("Live2D Cubism/Mask");
            }
        }
    }
}
