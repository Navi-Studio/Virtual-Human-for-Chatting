/**
 * Copyright(c) Live2D Inc. All rights reserved.
 *
 * Use of this source code is governed by the Live2D Open Software license
 * that can be found at https://www.live2d.com/eula/live2d-open-software-license-agreement_en.html.
 */


using Live2D.Cubism.Core;
using UnityEngine;


namespace Live2D.Cubism.Framework.LookAt
{
    /// <summary>
    /// Look at parameter.
    /// </summary>
    public sealed class CubismLookParameter : MonoBehaviour
    {
        /// <summary>
        /// Look axis.
        /// </summary>
        [SerializeField]
        public CubismLookAxis Axis;


        /// <summary>
        /// Factor.
        /// </summary>
        [SerializeField]
        public float Factor;

        #region Unity Event Handling

        /// <summary>
        /// Called by Unity. Guesses best settings.
        /// </summary>
        private void Reset()
        {
            var parameter = GetComponent<CubismParameter>();


            // Fail silently.
            if (parameter == null)
            {
                return;
            }


            // Guess axis.
            if (parameter.name.EndsWith("Y"))
            {
                Axis = CubismLookAxis.Y;
            }
            else if (parameter.name.EndsWith("Z"))
            {
                Axis = CubismLookAxis.Z;
            }
            else
            {
                Axis = CubismLookAxis.X;
            }


            // Guess factor.
            Factor = parameter.MaximumValue;
        }

        #endregion

        #region Interface for Controller

        /// <summary>
        /// Updates and evaluates the instance.
        /// </summary>
        /// <param name="targetOffset">Delta to target.</param>
        /// <returns>Evaluation result.</returns>
        internal float TickAndEvaluate(Vector3 targetOffset)
        {
            var result = (Axis == CubismLookAxis.X)
                ? targetOffset.x
                : targetOffset.y;


            if (Axis == CubismLookAxis.Z)
            {
                result = targetOffset.z;
            }


            return result * Factor;
        }

        #endregion
    }
}
