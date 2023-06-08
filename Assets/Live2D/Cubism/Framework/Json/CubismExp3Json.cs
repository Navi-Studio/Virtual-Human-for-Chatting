/**
 * Copyright(c) Live2D Inc. All rights reserved.
 *
 * Use of this source code is governed by the Live2D Open Software license
 * that can be found at https://www.live2d.com/eula/live2d-open-software-license-agreement_en.html.
 */


using System;
using UnityEngine;


namespace Live2D.Cubism.Framework.Json
{
    /// <summary>
    /// Cubism exp3.json data.
    /// </summary>
    [Serializable]
    public sealed class CubismExp3Json
    {
        #region Load Methods

        /// <summary>
        /// Loads a exp3.json asset.
        /// </summary>
        /// <param name="exp3Json">exp3.json to deserialize.</param>
        /// <returns>Deserialized exp3.json on success; <see langword="null"/> otherwise.</returns>
        public static CubismExp3Json LoadFrom(string exp3Json)
        {
            return (string.IsNullOrEmpty(exp3Json))
                ? null
                : JsonUtility.FromJson<CubismExp3Json>(exp3Json);
        }

        /// <summary>
        /// Loads a exp3.json asset.
        /// </summary>
        /// <param name="exp3JsonAsset">exp3.json to deserialize.</param>
        /// <returns>Deserialized exp3.json on success; <see langword="null"/> otherwise.</returns>
        public static CubismExp3Json LoadFrom(TextAsset exp3JsonAsset)
        {
            return (exp3JsonAsset == null)
                ? null
                : LoadFrom(exp3JsonAsset.text);
        }

        #endregion

        #region Json Data

        /// <summary>
        /// Expression Type
        /// </summary>
        [SerializeField]
        public string Type;

        /// <summary>
        /// Expression FadeInTime
        /// </summary>
        [SerializeField]
        public float FadeInTime = 1.0f;

        /// <summary>
        /// Expression FadeOutTime
        /// </summary>
        [SerializeField]
        public float FadeOutTime = 1.0f;

        /// <summary>
        /// Expression Parameters
        /// </summary>
        [SerializeField]
        public SerializableExpressionParameter[] Parameters;

        #endregion

        #region Json Helpers

        /// <summary>
        /// Expression Parameter
        /// </summary>
        [Serializable]
        public struct SerializableExpressionParameter
        {
            /// <summary>
            /// Expression Parameter Id
            /// </summary>
            [SerializeField]
            public string Id;

            /// <summary>
            /// Expression Parameter Value
            /// </summary>
            [SerializeField]
            public float Value;

            /// <summary>
            /// Expression Parameter Blend Mode
            /// </summary>
            [SerializeField]
            public string Blend;
        }

        #endregion

    }
}
