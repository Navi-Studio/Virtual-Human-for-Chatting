/**
 * Copyright(c) Live2D Inc. All rights reserved.
 *
 * Use of this source code is governed by the Live2D Open Software license
 * that can be found at https://www.live2d.com/eula/live2d-open-software-license-agreement_en.html.
 */


using UnityEngine;


namespace Live2D.Cubism.Framework.LookAt
{
    /// <summary>
    /// Straight-forward <see cref="ICubismLookTarget"/> <see cref="MonoBehaviour"/>.
    /// </summary>
    public class CubismLookTargetBehaviour : MonoBehaviour, ICubismLookTarget
    {
        #region Implementation of ICubismLookTarget

        /// <summary>
        /// Gets the position of the target.
        /// </summary>
        /// <returns>The position of the target in world space.</returns>
        Vector3 ICubismLookTarget.GetPosition()
        {
            return transform.position;
        }

        /// <summary>
        /// Gets whether the target is active.
        /// </summary>
        /// <returns><see langword="true"/> if the target is active; <see langword="false"/> otherwise.</returns>
        bool ICubismLookTarget.IsActive()
        {
            return isActiveAndEnabled;
        }

        #endregion
    }
}
