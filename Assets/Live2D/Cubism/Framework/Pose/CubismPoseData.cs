/**
 * Copyright(c) Live2D Inc. All rights reserved.
 *
 * Use of this source code is governed by the Live2D Open Software license
 * that can be found at https://www.live2d.com/eula/live2d-open-software-license-agreement_en.html.
 */


using Live2D.Cubism.Core;

namespace Live2D.Cubism.Framework.Pose
{
    public struct CubismPoseData
    {
        /// <summary>
        /// Cubism pose part.
        /// </summary>
        public CubismPosePart PosePart;

        /// <summary>
        /// Cubism part cache.
        /// </summary>
        public CubismPart Part;

        /// <summary>
        /// Link parts cache.
        /// </summary>
        public CubismPart[] LinkParts;

        /// <summary>
        /// Cubism part opacity.
        /// </summary>
        public float Opacity;
    }
}
