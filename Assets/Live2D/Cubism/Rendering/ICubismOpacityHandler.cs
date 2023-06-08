/**
 * Copyright(c) Live2D Inc. All rights reserved.
 *
 * Use of this source code is governed by the Live2D Open Software license
 * that can be found at https://www.live2d.com/eula/live2d-open-software-license-agreement_en.html.
 */


using Live2D.Cubism.Core;


namespace Live2D.Cubism.Rendering
{
    /// <summary>
    /// Allows listening to <see cref="CubismDrawable"/> draw order changes.
    /// </summary>
    public interface ICubismOpacityHandler
    {
        /// <summary>
        /// Called when opacity did change.
        /// </summary>
        /// <param name="controller">The <see cref="CubismRenderController"/>.</param>
        /// <param name="newOpacity">New opacity.</param>
        void OnOpacityDidChange(CubismRenderController controller, float newOpacity);
    }
}
