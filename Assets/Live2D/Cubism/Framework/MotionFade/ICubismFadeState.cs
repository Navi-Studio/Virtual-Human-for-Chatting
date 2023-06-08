/**
 * Copyright(c) Live2D Inc. All rights reserved.
 *
 * Use of this source code is governed by the Live2D Open Software license
 * that can be found at https://www.live2d.com/eula/live2d-open-software-license-agreement_en.html.
 */

using System.Collections.Generic;

namespace Live2D.Cubism.Framework.MotionFade
{
    /// <summary>
    /// Cubism fade state interface.
    /// </summary>
    public interface ICubismFadeState
    {
        /// <summary>
        /// Get cubism playing motion list.
        /// </summary>
        /// <returns>Cubism playing motion list.</returns>
        List<CubismFadePlayingMotion> GetPlayingMotions();

        /// <summary>
        /// Is default state.
        /// </summary>
        /// <returns><see langword="true"/> State is default; <see langword="false"/> otherwise.</returns>
        bool IsDefaultState();

        /// <summary>
        /// Get layer weight.
        /// </summary>
        /// <returns>Layer weight.</returns>
        float GetLayerWeight();

        /// <summary>
        /// Get state transition finished.
        /// </summary>
        /// <returns><see langword="true"/> State transition is finished; <see langword="false"/> otherwise.</returns>
        bool GetStateTransitionFinished();

        /// <summary>
        /// Set state transition finished.
        /// </summary>
        /// <param name="isFinished">State is finished.</param>
        void SetStateTransitionFinished(bool isFinished);

        /// <summary>
        /// Stop animation.
        /// </summary>
        /// <param name="index">Playing motion index.</param>
        void StopAnimation(int index);
    }
}
