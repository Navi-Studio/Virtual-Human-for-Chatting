/**
 * Copyright(c) Live2D Inc. All rights reserved.
 *
 * Use of this source code is governed by the Live2D Open Software license
 * that can be found at https://www.live2d.com/eula/live2d-open-software-license-agreement_en.html.
 */


namespace Live2D.Cubism.Framework.HarmonicMotion
{
    /// <summary>
    /// Determines the direction of a harmonic motion from its origin.
    /// </summary>
    public enum CubismHarmonicMotionDirection
    {
        /// <summary>
        /// Motion to the left of the origin.
        /// </summary>
        Left,

        /// <summary>
        /// Motion to the right of the origin.
        /// </summary>
        Right,

        /// <summary>
        /// Centric left-right motion around the origin.
        /// </summary>
        Centric,
    }
}
