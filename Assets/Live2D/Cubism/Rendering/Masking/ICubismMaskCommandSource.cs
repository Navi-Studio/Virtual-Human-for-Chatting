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
    /// Common interface for mask command sources.
    /// </summary>
    public interface ICubismMaskCommandSource
    {
        /// <summary>
        /// Called to enqueue source.
        /// </summary>
        /// <param name="buffer">Buffer to enqueue in.</param>
        void AddToCommandBuffer(CommandBuffer buffer);
    }
}
