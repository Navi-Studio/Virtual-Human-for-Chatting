/**
 * Copyright(c) Live2D Inc. All rights reserved.
 *
 * Use of this source code is governed by the Live2D Open Software license
 * that can be found at https://www.live2d.com/eula/live2d-open-software-license-agreement_en.html.
 */


namespace Live2D.Cubism.Framework.Raycasting
{
    /// <summary>
    /// Precision for casting rays against a <see cref="CubismRaycastable"/>.
    /// </summary>
    public enum CubismRaycastablePrecision
    {
        /// <summary>
        /// Cast against bounding box.
        /// </summary>
        BoundingBox,

        /// <summary>
        /// Cast against triangles.
        /// </summary>
        Triangles
    }
}
