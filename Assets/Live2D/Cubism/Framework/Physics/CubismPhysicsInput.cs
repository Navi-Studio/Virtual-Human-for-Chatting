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
    /// Input data of physics.
    /// </summary>
    [Serializable]
    public struct CubismPhysicsInput
    {
        /// <summary>
        /// Delegation of function of getting normalized parameter value.
        /// </summary>
        /// <param name="targetTranslation">Result of translation.</param>
        /// <param name="targetAngle">Result of rotation.</param>
        /// <param name="parameter">Parameter.</param>
        /// <param name="parameterValue">Parameter value.</param>
        /// <param name="normalization">Normalized components.</param>
        /// <param name="weight">Weight.</param>
        public delegate void NormalizedParameterValueGetter(
            ref Vector2 targetTranslation,
            ref float targetAngle,
            CubismParameter parameter,
            ref float parameterValue,
            CubismPhysicsNormalization normalization,
            float weight
        );

        /// <summary>
        /// Gets Normalized parameter value from input translation X-axis.
        /// </summary>
        /// <param name="targetTranslation">Result of translation.</param>
        /// <param name="targetAngle">Result of rotation.</param>
        /// <param name="parameter">Parameter.</param>
        /// <param name="parameterValue">Parameter value.</param>
        /// <param name="normalization">Normalized components.</param>
        /// <param name="weight">Weight.</param>
        private void GetInputTranslationXFromNormalizedParameterValue(
             ref Vector2 targetTranslation,
             ref float targetAngle,
             CubismParameter parameter,
             ref float parameterValue,
             CubismPhysicsNormalization normalization,
             float weight
        )
        {
            targetTranslation.x += CubismPhysicsMath.Normalize(
                                    parameter,
                                    ref parameterValue,
                                    normalization.Position.Minimum,
                                    normalization.Position.Maximum,
                                    normalization.Position.Default,
                                    IsInverted
                                    ) * weight;
        }

        /// <summary>
        /// Gets Normalized parameter value from input translation Y-axis.
        /// </summary>
        /// <param name="targetTranslation">Result of translation.</param>
        /// <param name="targetAngle">Result of rotation.</param>
        /// <param name="parameter">Parameter.</param>
        /// <param name="parameterValue">Parameter value.</param>
        /// <param name="normalization">Normalized components.</param>
        /// <param name="weight">Weight.</param>
        private void GetInputTranslationYFromNormalizedParameterValue(
            ref Vector2 targetTranslation,
            ref float targetAngle,
            CubismParameter parameter,
            ref float parameterValue,
            CubismPhysicsNormalization normalization,
            float weight
        )
        {
            targetTranslation.y += CubismPhysicsMath.Normalize(
                        parameter,
                        ref parameterValue,
                        normalization.Position.Minimum,
                        normalization.Position.Maximum,
                        normalization.Position.Default,
                        IsInverted
                        ) * weight;
        }

        /// <summary>
        /// Gets Normalized parameter value from input rotation.
        /// </summary>
        /// <param name="targetTranslation">Result of translation.</param>
        /// <param name="targetAngle">Result of rotation.</param>
        /// <param name="parameter">Parameter.</param>
        /// <param name="parameterValue">Parameter value.</param>
        /// <param name="normalization">Normalized components.</param>
        /// <param name="weight">Weight.</param>
        private void GetInputAngleFromNormalizedParameterValue(
            ref Vector2 targetTranslation,
            ref float targetAngle,
            CubismParameter parameter,
            ref float parameterValue,
            CubismPhysicsNormalization normalization,
            float weight
        )
        {
            targetAngle += CubismPhysicsMath.Normalize(
                              parameter,
                              ref parameterValue,
                              normalization.Angle.Minimum,
                              normalization.Angle.Maximum,
                              normalization.Angle.Default,
                              IsInverted
                              ) * weight;
        }

        public void InitializeGetter()
        {
            switch (SourceComponent)
            {
                case CubismPhysicsSourceComponent.X:
                    {
                        GetNormalizedParameterValue =
                            GetInputTranslationXFromNormalizedParameterValue;
                    }
                    break;
                case CubismPhysicsSourceComponent.Y:
                    {
                        GetNormalizedParameterValue =
                            GetInputTranslationYFromNormalizedParameterValue;
                    }
                    break;
                case CubismPhysicsSourceComponent.Angle:
                    {
                        GetNormalizedParameterValue =
                            GetInputAngleFromNormalizedParameterValue;
                    }
                    break;
            }
        }

        /// <summary>
        /// Parameter ID of source.
        /// </summary>
        [SerializeField]
        public string SourceId;

        /// <summary>
        /// Scale of translation.
        /// </summary>
        [SerializeField]
        public Vector2 ScaleOfTranslation;

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
        /// Source data from parameter.
        /// </summary>
        [NonSerialized]
        public CubismParameter Source;

        /// <summary>
        /// Function of getting normalized parameter value.
        /// </summary>
        [NonSerialized]
        public NormalizedParameterValueGetter GetNormalizedParameterValue;
    }
}
