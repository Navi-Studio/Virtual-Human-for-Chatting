/**
 * Copyright(c) Live2D Inc. All rights reserved.
 *
 * Use of this source code is governed by the Live2D Open Software license
 * that can be found at https://www.live2d.com/eula/live2d-open-software-license-agreement_en.html.
 */


using Live2D.Cubism.Framework.MotionFade;
using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;


namespace Live2D.Cubism.Editor.Deleters
{
    /// <summary>
    /// Handles importing of Cubism models.
    /// </summary>
    [Serializable]
    public sealed class CubismFadeAssetDeleter : CubismDeleterBase
    {

        #region Unity Event Handling

        /// <summary>
        /// Registers deleter.
        /// </summary>
        [InitializeOnLoadMethod]
        // ReSharper disable once UnusedMember.Local
        private static void RegisterDeleter()
        {
            CubismDeleter.RegisterDeleter<CubismFadeAssetDeleter>(".fade.asset");
        }

        #endregion

        #region CubismDeleterBase

        /// <summary>
        /// Deleters the corresponding asset.
        /// </summary>
        public override void Delete()
        {
            var directoryName = Path.GetDirectoryName(AssetPath).ToString();
            var modelDir = Path.GetDirectoryName(directoryName).ToString();
            var modelName = Path.GetFileName(modelDir).ToString();
            var fadeMotionListPath = Path.GetDirectoryName(directoryName).ToString() + "/" + modelName + ".fadeMotionList.asset";
            var fadeMotionList = AssetDatabase.LoadAssetAtPath<CubismFadeMotionList>(fadeMotionListPath);

            if (fadeMotionList == null)
            {
                return;
            }

            var deleteAssetName = Path.GetFileName(AssetPath).Replace(".asset", "");
            var instanceIds = new List<int>();
            var fadeMotionObjects = new List<CubismFadeMotionData>();

            for (var i = 0; i < fadeMotionList.CubismFadeMotionObjects.Length; ++i)
            {
                var fadeMotion = fadeMotionList.CubismFadeMotionObjects[i];

                if (fadeMotion == null || fadeMotion.name == deleteAssetName)
                {
                    continue;
                }

                instanceIds.Add(fadeMotionList.MotionInstanceIds[i]);
                fadeMotionObjects.Add(fadeMotion);
            }

            fadeMotionList.MotionInstanceIds = instanceIds.ToArray();
            fadeMotionList.CubismFadeMotionObjects = fadeMotionObjects.ToArray();
        }

        #endregion

    }
}
