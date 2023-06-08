/**
 * Copyright(c) Live2D Inc. All rights reserved.
 *
 * Use of this source code is governed by the Live2D Open Software license
 * that can be found at https://www.live2d.com/eula/live2d-open-software-license-agreement_en.html.
 */


using Live2D.Cubism.Core;
using System;
using UnityEngine;


namespace Live2D.Cubism.Framework.Physics
{
    /// <summary>
    /// Output data of physics.
    /// </summary>
    [Serializable]
    public struct CubismPhysicsOutput
    {
        /// <summary>
        /// Delegation of function of getting output value.
        /// </summary>
        /// <param name="translation">Translation.</param>
        /// <param name="particles">Particles.</param>
        /// <param name="particleIndex">Index of particle.</param>
        /// <param name="gravity">Gravity.</param>
        /// <returns>Output value.</returns>
        public delegate float ValueGetter(
            Vector2 translation,
            CubismPhysicsParticle[] particles,
            int particleIndex,
            Vector2 gravity
        );

        /// <summary>
        /// Delegation of function of getting output scale.
        /// </summary>
        /// <returns>Output scale.</returns>
        public delegate float ScaleGetter();

        /// <summary>
        /// Gets output for translation X-axis.
        /// </summary>
        /// <param name="translation">Translation.</param>
        /// <param name="particles">Particles.</param>
        /// <param name="particleIndex">Index of particle.</param>
        /// <param name="gravity">Gravity.</param>
        /// <returns>Output value.</returns>
        private float GetOutputTranslationX(
            Vector2 translation,
            CubismPhysicsParticle[] particles,
            int particleIndex,
            Vector2 gravity
        )
        {
            var outputValue = translation.x;

            if (IsInverted)
            {
                outputValue *= -1.0f;
            }

            return outputValue;
        }

        /// <summary>
        /// Gets output for translation Y-axis.
        /// </summary>
        /// <param name="translation">Translation.</param>
        /// <param name="particles">Particles.</param>
        /// <param name="particleIndex">Index of particle.</param>
        /// <param name="gravity">Gravity.</param>
        /// <returns>Output value.</returns>
        private float GetOutputTranslationY(
            Vector2 translation,
            CubismPhysicsParticle[] particles,
            int particleIndex,
            Vector2 gravity
        )
        {
            var outputValue = translation.y;

            if (IsInverted)
            {
                outputValue *= -1.0f;
            }

            return outputValue;
        }

        /// <summary>
        /// Gets output for angle.
        /// </summary>
        /// <param name="translation">Translation.</param>
        /// <param name="particles">Particles.</param>
        /// <param name="particleIndex">Index of particle.</param>
        /// <param name="gravity">Gravity.</param>
        /// <returns>Output value.</returns>
        private float GetOutputAngle(
            Vector2 translation,
            CubismPhysicsParticle[] particles,
            int particleIndex,
            Vector2 gravity
        )
        {
            var parentGravity = Vector2.zero;


            if (CubismPhysics.UseAngleCorrection)
            {
                if (particleIndex < 2)
                {
                    parentGravity = gravity;
                    parentGravity.y *= -1.0f;
                }
                else
                {
                    parentGravity = particles[particleIndex - 1].Position -
                        particles[particleIndex - 2].Position;
                }
            }
            else
            {
                parentGravity = gravity;
                parentGravity.y *= -1.0f;
            }


            var outputValue = CubismPhysicsMath.DirectionToRadian(parentGravity, translation);


            if (IsInverted)
            {
                outputValue *= -1.0f;
            }

            return outputValue;
        }

        /// <summary>
        /// Gets output scale for translation X-axis.
        /// </summary>
        /// <returns>Output scale.</returns>
        private float GetOutputScaleTranslationX()
        {
            return TranslationScale.x;
        }

        /// <summary>
        /// Gets output scale for translation Y-axis.
        /// </summary>
        /// <returns>Output scale.</returns>
        private float GetOutputScaleTranslationY()
        {
            return TranslationScale.y;
        }

        /// <summary>
        /// Gets output scale for angle.
        /// </summary>
        /// <returns>Output scale.</returns>
        private float GetOutputScaleAngle()
        {
            return AngleScale;
        }

        public void InitializeGetter()
        {
            switch (SourceComponent)
            {
                case CubismPhysicsSourceComponent.X:
                    {
                        GetScale =
                            GetOutputScaleTranslationX;

                        GetValue =
                            GetOutputTranslationX;
                    }
                    break;
                case CubismPhysicsSourceComponent.Y:
                    {
                        GetScale =
                            GetOutputScaleTranslationY;

                        GetValue =
                            GetOutputTranslationY;
                    }
                    break;
                case CubismPhysicsSourceComponent.Angle:
                    {
                        GetScale =
                            GetOutputScaleAngle;

                        GetValue =
                            GetOutputAngle;
                    }
                    break;
            }
        }

        /// <summary>
        /// Parameter ID of destination.
        /// </summary>
        [SerializeField]
        public string DestinationId;

        /// <summary>
        /// Index of particle.
        /// </summary>
        [SerializeField]
        public int ParticleIndex;

        /// <summary>
        /// Scale of transition.
        /// </summary>
        [SerializeField]
        public Vector2 TranslationScale;

        /// <summary>
        /// Scale of angle.
        /// </summary>
        [SerializeField]
        public float AngleScale;

        /// <summary>
        /// Weight.
        /// </summary>
        [SerializeField]
        public float Weight;

        /// <summary>
        /// Component of source.
        /// </summary>
        [SerializeField]
        public CubismPhysicsSourceComponent SourceComponent;

        /// <summary>
        /// True if value is inverted; otherwise.
        /// </summary>
        [SerializeField]
        public bool IsInverted;

        /// <summary>
        /// The value that below minimum.
        /// </summary>
        [NonSerialized]
        public float ValueBelowMinimum;

        /// <summary>
        /// The value that exceeds maximum.
        /// </summary>
        [NonSerialized]
        public float ValueExceededMaximum;

        /// <summary>
        /// Destination data from parameter.
        /// </summary>
        [NonSerialized]
        public CubismParameter Destination;

        /// <summary>
        /// Function of getting output value.
        /// </summary>
        [NonSerialized]
        public ValueGetter GetValue;

        /// <summary>
        /// Function of getting output scale.
        /// </summary>
        [NonSerialized]
        public ScaleGetter GetScale;
    }
}
