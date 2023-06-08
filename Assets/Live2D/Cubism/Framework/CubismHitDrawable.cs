/**
 * Copyright(c) Live2D Inc. All rights reserved.
 *
 * Use of this source code is governed by the Live2D Open Software license
 * that can be found at https://www.live2d.com/eula/live2d-open-software-license-agreement_en.html.
 */


using UnityEngine;


namespace Live2D.Cubism.Framework
{
    /// <summary>
    /// Tagging component for Drawable used for hit determination.
    /// </summary>
    public sealed class CubismHitDrawable : MonoBehaviour
    {
        /// <summary>
        /// Name set in HitArea.
        /// </summary>
        [SerializeField]
        public string Name;
    }
}
