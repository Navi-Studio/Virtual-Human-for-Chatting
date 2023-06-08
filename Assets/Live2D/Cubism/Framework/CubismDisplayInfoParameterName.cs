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
    /// Get the parameter name from cdi3.json and save the display name.
    /// </summary>
    public class CubismDisplayInfoParameterName : MonoBehaviour
    {
        /// <summary>
        /// Original name of the parameter from cdi3.json.
        /// </summary>
        [SerializeField,HideInInspector]
        public string Name;

        /// <summary>
        /// Name for display that can be changed by the user.
        /// </summary>
        [SerializeField]
        public string DisplayName;
    }
}
