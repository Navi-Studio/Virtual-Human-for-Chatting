/**
 * Copyright(c) Live2D Inc. All rights reserved.
 *
 * Use of this source code is governed by the Live2D Open Software license
 * that can be found at https://www.live2d.com/eula/live2d-open-software-license-agreement_en.html.
 */


using UnityEngine;


namespace Live2D.Cubism.Framework.UserData
{
    /// <summary>
    /// Tag of user data.
    /// </summary>
    [CubismDontMoveOnReimport]
    public class CubismUserDataTag : MonoBehaviour
    {
        /// <summary>
        /// Value.
        /// </summary>
        public string Value
        {
            get
            {
                if (string.IsNullOrEmpty(_value) &&
                    !string.IsNullOrEmpty(Body.Value))
                {
                    _value = Body.Value;
                }

                return _value;
            }
            set { _value = value; }
        }

        /// <summary>
        /// Value backing field.
        /// </summary>
        [SerializeField, HideInInspector]
        private string _value;

        /// <summary>
        /// Body backing field.
        /// </summary>
        [SerializeField, HideInInspector]
        private CubismUserDataBody _body;

        /// <summary>
        /// Body.
        /// </summary>
        private CubismUserDataBody Body
        {
            get { return _body; }
            set { _body = value; }
        }

        /// <summary>
        /// Initializes tag.
        /// </summary>
        /// <param name="body">Body for initialization.</param>
        public void Initialize(CubismUserDataBody body)
        {
            Body = body;
        }
    }
}
