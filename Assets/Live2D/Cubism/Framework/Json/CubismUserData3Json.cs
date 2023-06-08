/**
 * Copyright(c) Live2D Inc. All rights reserved.
 *
 * Use of this source code is governed by the Live2D Open Software license
 * that can be found at https://www.live2d.com/eula/live2d-open-software-license-agreement_en.html.
 */


using Live2D.Cubism.Framework.UserData;
using System;
using System.Collections.Generic;
using UnityEngine;


namespace Live2D.Cubism.Framework.Json
{
    /// <summary>
    /// Handles user data from cdi3.json.
    /// </summary>
    [Serializable]
    public sealed class CubismUserData3Json
    {
        /// <summary>
        /// Loads a cdi3.json asset.
        /// </summary>
        /// <param name="userData3Json">cdi3.json to deserialize.</param>
        /// <returns>Deserialized cdi3.json on success; <see langword="null"/> otherwise.</returns>
        public static CubismUserData3Json LoadFrom(string userData3Json)
        {
            return (string.IsNullOrEmpty(userData3Json))
                ? null
                : JsonUtility.FromJson<CubismUserData3Json>(userData3Json);
        }

        /// <summary>
        /// Loads a cdi3.json asset.
        /// </summary>
        /// <param name="userData3JsonAsset">cdi3.json to deserialize.</param>
        /// <returns>Deserialized cdi3.json on success; <see langword="null"/> otherwise.</returns>
        public static CubismUserData3Json LoadFrom(TextAsset userData3JsonAsset)
        {
            return (userData3JsonAsset == null)
                ? null
                : LoadFrom(userData3JsonAsset.text);
        }

        /// <summary>
        /// Makes <see cref="CubismUserDataBody"/> array that was selected by <see cref="CubismUserDataTargetType"/>.
        /// </summary>
        /// <param name="targetType">Target object type.</param>
        /// <returns><see cref="CubismUserDataBody"/> array. Selected by <see cref="CubismUserDataTargetType"/>.</returns>
        public CubismUserDataBody[] ToBodyArray(CubismUserDataTargetType targetType)
        {
            var userDataList = new List<CubismUserDataBody>();


            for (var i = 0; i < UserData.Length; ++i)
            {
                var body = new CubismUserDataBody
                {
                    Id = UserData[i].Id,
                    Value = UserData[i].Value
                };

                switch (targetType)
                {
                    case CubismUserDataTargetType.ArtMesh:
                    {
                        // Only drawables.
                        if (UserData[i].Target == "ArtMesh")
                        {
                            userDataList.Add(body);
                        }

                        break;
                    }
                    default:
                    {
                        break;
                    }
                }
            }


            return userDataList.ToArray();
        }


        #region Json Data

        /// <summary>
        /// Json file format version.
        /// </summary>
        [SerializeField]
        public int Version;

        /// <summary>
        /// Additional data describing physics.
        /// </summary>
        [SerializeField]
        public SerializableMeta Meta;

        /// <summary>
        /// Array of user data.
        /// </summary>
        [SerializeField]
        public SerializableUserData[] UserData;

        #endregion


        #region Json Helpers

        /// <summary>
        /// Additional data describing user data.
        /// </summary>
        [Serializable]
        public struct SerializableMeta
        {
            /// <summary>
            /// Number of user data.
            /// </summary>
            [SerializeField]
            public int UserDataCount;

            /// <summary>
            /// Total number of user data.
            /// </summary>
            [SerializeField]
            public int TotalUserDataCount;
        }

        /// <summary>
        /// User data.
        /// </summary>
        [Serializable]
        public struct SerializableUserData
        {
            /// <summary>
            /// Type of target object.
            /// </summary>
            [SerializeField]
            public string Target;

            /// <summary>
            /// Name of target object.
            /// </summary>
            [SerializeField]
            public string Id;

            /// <summary>
            /// Value.
            /// </summary>
            [SerializeField]
            public string Value;
        }

        #endregion
    }
}
