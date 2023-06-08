/**
 * Copyright(c) Live2D Inc. All rights reserved.
 *
 * Use of this source code is governed by the Live2D Open Software license
 * that can be found at https://www.live2d.com/eula/live2d-open-software-license-agreement_en.html.
 */


using Live2D.Cubism.Framework.Expression;
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
    public sealed class CubismExpressionAssetDeleter : CubismDeleterBase
    {

        #region Unity Event Handling

        /// <summary>
        /// Registers deleter.
        /// </summary>
        [InitializeOnLoadMethod]
        // ReSharper disable once UnusedMember.Local
        private static void RegisterDeleter()
        {
            CubismDeleter.RegisterDeleter<CubismExpressionAssetDeleter>(".exp3.asset");
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
            var expressionListPath = Path.GetDirectoryName(directoryName).ToString() + "/" + modelName + ".expressionList.asset";
            var expressionList = AssetDatabase.LoadAssetAtPath<CubismExpressionList>(expressionListPath);

            if (expressionList == null)
            {
                return;
            }

            var deleteAssetName = Path.GetFileName(AssetPath).Replace(".asset", "");
            var expressionObjects = new List<CubismExpressionData>();

            for (var i = 0; i < expressionList.CubismExpressionObjects.Length; ++i)
            {
                var expression = expressionList.CubismExpressionObjects[i];

                if (expression == null || expression.name == deleteAssetName)
                {
                    continue;
                }

                expressionObjects.Add(expression);
            }

            expressionList.CubismExpressionObjects = expressionObjects.ToArray();
        }

        #endregion

    }
}
