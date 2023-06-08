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
    /// Handles pose from pose3.json.
    /// </summary>
    [Serializable]
    public sealed class CubismPose3Json
    {
        /// <summary>
        /// Loads a pose3.json.
        /// </summary>
        /// <param name="pose3Json">pose3.json to deserialize.</param>
        /// <returns>Deserialized pose3.json on success; <see langword="null"/> otherwise.</returns>
        public static CubismPose3Json LoadFrom(string pose3Json)
        {
            if (string.IsNullOrEmpty(pose3Json))
            {
                return null;
            }

            var ret = new CubismPose3Json();
            var value = CubismJsonParser.ParseFromString(pose3Json);

            ret.Type = (value.Get("Type") == null) ? null : value.Get("Type").toString();

            ret.FadeInTime = (value.Get("FadeInTime") == null) ? 0.5f : value.Get("FadeInTime").ToFloat();

            var groups = (value.Get("Groups") == null) ? null : value.Get("Groups").GetVector(null);

            if(groups != null)
            {
                ret.Groups = new SerializablePoseGroup[groups.Count][];

                for (var i = 0; i < groups.Count; ++i)
                {
                    var count = groups[i].GetVector(null).Count;
                    ret.Groups[i] = new SerializablePoseGroup[count];

                    for (var j = 0; j < count; ++j)
                    {
                        ret.Groups[i][j].Id = groups[i].GetVector(null)[j].Get("Id").toString();
                        var link = groups[i].GetVector(null)[j].Get("Link").GetVector(null);

                        if(link.Count == 0)
                        {
                            continue;
                        }

                        ret.Groups[i][j].Link = new string[link.Count];

                        for (var linkCount = 0; linkCount < link.Count; ++ linkCount)
                        {
                            ret.Groups[i][j].Link[linkCount] = link[linkCount].toString();
                        }
                    }
                }
            }

            return ret;
        }

        /// <summary>
        /// Loads a pose3.json asset.
        /// </summary>
        /// <param name="pose3JsonAsset">pose3.json asset to deserialize.</param>
        /// <returns>Deserialized pose3.json asset on success; <see langword="null"/> otherwise.</returns>
        public static CubismPose3Json LoadFrom(TextAsset pose3JsonAsset)
        {
            return (pose3JsonAsset == null)
                ? null
                : LoadFrom(pose3JsonAsset.text);
        }

        #region Json Data

        /// <summary>
        /// The type of cubism pose.
        /// </summary>
        [SerializeField]
        public string Type;

        /// <summary>
        /// [Optional] Time of the Fade-in for easing in seconds..
        /// </summary>
        [SerializeField]
        public float FadeInTime;

        /// <summary>
        /// Array of Groups.
        /// </summary>
        [SerializeField]
        public SerializablePoseGroup[][] Groups;

        #endregion

        #region Json Helpers

        [Serializable]
        public struct SerializablePoseGroup
        {
            /// <summary>
            /// The part id of group.
            /// </summary>
            [SerializeField]
            public string Id;

            /// <summary>
            /// The link part ids.
            /// </summary>
            [SerializeField]
            public string[] Link;
        }

        #endregion

    }

}
