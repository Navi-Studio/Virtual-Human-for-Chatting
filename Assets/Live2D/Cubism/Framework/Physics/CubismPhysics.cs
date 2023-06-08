/**
 * Copyright(c) Live2D Inc. All rights reserved.
 *
 * Use of this source code is governed by the Live2D Open Software license
 * that can be found at https://www.live2d.com/eula/live2d-open-software-license-agreement_en.html.
 */


using UnityEngine;


namespace Live2D.Cubism.Framework.Physics
{
    /// <summary>
    /// Global variables of physics.
    /// </summary>
    public static class CubismPhysics
    {
        /// <summary>
        /// Default gravity.
        /// </summary>
        public static Vector2 Gravity = Vector2.down;

        /// <summary>
        /// Default direction of wind.
        /// </summary>
        public static Vector2 Wind = Vector2.zero;

        /// <summary>
        /// Air resistance.
        /// </summary>
        public static float AirResistance = 5.0f;

        /// <summary>
        /// Physical maximum weight.
        /// </summary>
        public static float MaximumWeight = 100.0f;

        /// <summary>
        /// Use fixed delta time.
        /// </summary>
        public static bool UseFixedDeltaTime = false;

        /// <summary>
        /// Use angle correction.
        /// </summary>
        public static bool UseAngleCorrection = true;

        /// <summary>
        /// Threshold of moving.
        /// </summary>
        public const float MovementThreshold = 0.001f;

        /// Constant of maximum allowed delta time
        public const float MaxDeltaTime = 5.0f;
    }
}
