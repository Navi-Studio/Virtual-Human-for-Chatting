/**
 * Copyright(c) Live2D Inc. All rights reserved.
 *
 * Use of this source code is governed by the Live2D Open Software license
 * that can be found at https://www.live2d.com/eula/live2d-open-software-license-agreement_en.html.
 */


using Live2D.Cubism.Framework.MotionFade;
using System;
using UnityEditor;


namespace Live2D.Cubism.Editor.Deleters
{
    /// <summary>
    /// Handles importing of Cubism models.
    /// </summary>
    [Serializable]
    public sealed class CubismAnimationClipDeleter : CubismDeleterBase
    {

        #region Unity Event Handling

        /// <summary>
        /// Registers deleter.
        /// </summary>
        [InitializeOnLoadMethod]
        // ReSharper disable once UnusedMember.Local
        private static void RegisterDeleter()
        {
            CubismDeleter.RegisterDeleter<CubismAnimationClipDeleter>(".anim");
        }

        #endregion

        #region CubismDeleterBase

        /// <summary>
        /// Deleters the corresponding asset.
        /// </summary>
        public override void Delete()
        {
            var fadeAssetPath = AssetPath.Replace(".anim", ".fade.asset");
            var fadeAsset = AssetDatabase.LoadAssetAtPath<CubismFadeMotionData>(fadeAssetPath);

            // Fail silently...
            if (fadeAsset == null)
            {
                return;
            }

            // Delete fade motion asset.
            AssetDatabase.DeleteAsset(fadeAssetPath);

            // Get fade motion asset deleter.
            var fadeMotionDeleter = CubismDeleter.GetDeleterAsPath(fadeAssetPath);

            // Fail silently...
            if (fadeMotionDeleter == null)
            {
                return;
            }

            fadeMotionDeleter.Delete();
        }

        #endregion

    }
}
