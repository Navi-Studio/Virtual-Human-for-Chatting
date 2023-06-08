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
    /// Vertex data of physics.
    /// </summary>
    [Serializable]
    public struct CubismPhysicsParticle
    {
        /// <summary>
        /// Initial position.
        /// </summary>
        [SerializeField]
        public Vector2 InitialPosition;

        /// <summary>
        /// Mobility ratio.
        /// </summary>
        [SerializeField]
        public float Mobility;

        /// <summary>
        /// Delay ratio.
        /// </summary>
        [SerializeField]
        public float Delay;

        /// <summary>
        /// Current acceleration.
        /// </summary>
        [SerializeField]
        public float Acceleration;

        /// <summary>
        /// Length of radius.
        /// </summary>
        [SerializeField]
        public float Radius;

        /// <summary>
        /// Current position.
        /// </summary>
        [NonSerialized]
        public Vector2 Position;

        /// <summary>
        /// Last position.
        /// </summary>
        [NonSerialized]
        public Vector2 LastPosition;

        /// <summary>
        /// Last gravity.
        /// </summary>
        [NonSerialized]
        public Vector2 LastGravity;

        /// <summary>
        /// Current force.
        /// </summary>
        [NonSerialized]
        public Vector2 Force;

        /// <summary>
        /// Current velocity.
        /// </summary>
        [NonSerialized]
        public Vector2 Velocity;
    }
}
