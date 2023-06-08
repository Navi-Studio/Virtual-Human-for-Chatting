/**
 * Copyright(c) Live2D Inc. All rights reserved.
 *
 * Use of this source code is governed by the Live2D Open Software license
 * that can be found at https://www.live2d.com/eula/live2d-open-software-license-agreement_en.html.
 */


using System;
using UnityEngine;


namespace Live2D.Cubism.Framework.Physics
{
    /// <summary>
    /// Normalization tuplet.
    /// </summary>
    [Serializable]
    public struct CubismPhysicsNormalizationTuplet
    {
        /// <summary>
        /// Normalized maximum value.
        /// </summary>
        [SerializeField]
        public float Maximum;

        /// <summary>
        /// Normalized minimum value.
        /// </summary>
        [SerializeField]
        public float Minimum;

        /// <summary>
        /// Normalized default value.
        /// </summary>
        [SerializeField]
        public float Default;
    }

    /// <summary>
    /// Normalization parameters of physics.
    /// </summary>
    [Serializable]
    public struct CubismPhysicsNormalization
    {
        /// <summary>
        /// Normalized position.
        /// </summary>
        [SerializeField]
        public CubismPhysicsNormalizationTuplet Position;

        /// <summary>
        /// Normalized angle.
        /// </summary>
        [SerializeField]
        public CubismPhysicsNormalizationTuplet Angle;
    }
}
