/**
 * Copyright(c) Live2D Inc. All rights reserved.
 *
 * Use of this source code is governed by the Live2D Open Software license
 * that can be found at https://www.live2d.com/eula/live2d-open-software-license-agreement_en.html.
 */


using System;
using UnityEngine;


namespace Live2D.Cubism.Framework.UserData
{
    /// <summary>
    /// Body of user data.
    /// </summary>
    [Serializable]
    public struct CubismUserDataBody
    {
        /// <summary>
        /// Id.
        /// </summary>
        [SerializeField]
        public string Id;

        /// <summary>
        /// Value.
        /// </summary>
        [SerializeField]
        public string Value;
    }
}
