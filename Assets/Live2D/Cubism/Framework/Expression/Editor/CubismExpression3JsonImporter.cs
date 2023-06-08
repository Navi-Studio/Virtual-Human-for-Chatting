/**
 * Copyright(c) Live2D Inc. All rights reserved.
 *
 * Use of this source code is governed by the Live2D Open Software license
 * that can be found at https://www.live2d.com/eula/live2d-open-software-license-agreement_en.html.
 */


using Live2D.Cubism.Framework.Expression;
using Live2D.Cubism.Framework.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Live2D.Cubism.Core;
using UnityEditor;
using UnityEngine;

namespace Live2D.Cubism.Editor.Importers
{
    public sealed class CubismExpression3JsonImporter : CubismImporterBase
    {
        /// <summary>
        /// <see cref="CubismPose3Json"/> backing field.
        /// </summary>
        [NonSerialized]
        private CubismExp3Json _expressionJson;

        private CubismExp3Json ExpressionJson
        {
            get
            {
                if(_expressionJson != null)
                {
                    return _expressionJson;
                }

                if(string.IsNullOrEmpty(AssetPath))
                {
                    return null;
                }

                var expressionJson = AssetDatabase.LoadAssetAtPath<TextAsset>((AssetPath));
                _expressionJson = CubismExp3Json.LoadFrom(expressionJson);

                return _expressionJson;
            }
        }

        #region Unity Event Handling

        /// <summary>
        /// Registers importer.
        /// </summary>
        [InitializeOnLoadMethod]
        // ReSharper disable once UnusedMember.Local
        private static void RegisterImporter()
        {
            CubismImporter.RegisterImporter<CubismExpression3JsonImporter>(".exp3.json");
            CubismImporter.OnDidImportModel += OnModelImport;
        }

        #endregion

        #region Cubism Import Event Handling

        /// <summary>
        /// Imports the corresponding asset.
        /// </summary>
        public override void Import()
        {
            var oldExpressionData = AssetDatabase.LoadAssetAtPath<CubismExpressionData>(AssetPath.Replace(".exp3.json", ".exp3.asset"));

            // Create expression data.
            CubismExpressionData expressionData;

            if(oldExpressionData == null)
            {
                expressionData = CubismExpressionData.CreateInstance(ExpressionJson);
                AssetDatabase.CreateAsset(expressionData, AssetPath.Replace(".exp3.json", ".exp3.asset"));
            }
            else
            {
                expressionData = CubismExpressionData.CreateInstance(oldExpressionData, ExpressionJson);
                EditorUtility.CopySerialized(expressionData, oldExpressionData);
                expressionData = oldExpressionData;
            }

            EditorUtility.SetDirty(expressionData);

            // Add expression data to expression list.
            var directoryName = Path.GetDirectoryName(AssetPath);
            var modelDir = Path.GetDirectoryName(directoryName);
            var modelName = Path.GetFileName(modelDir);
            var expressionListPath = Path.GetDirectoryName(directoryName).Replace("\\", "/") + "/" + modelName + ".expressionList.asset";

            var assetList = CubismCreatedAssetList.GetInstance();
            var assetListIndex = assetList.AssetPaths.Contains(expressionListPath)
                ? assetList.AssetPaths.IndexOf(expressionListPath)
                : -1;

            var expressionList = GetExpressionList(expressionListPath);

            if (expressionList == null)
            {
                Debug.LogError("CubismExpression3JsonImporter : Can not create CubismExpressionList.");
                return;
            }

            // Rebuild the array if any element is null.
            if (expressionList.CubismExpressionObjects.Any(element => element == null))
            {
                var cubismExpressionObjectsToList = expressionList.CubismExpressionObjects.ToList();
                cubismExpressionObjectsToList.RemoveAll(element => element == null);

                var expressionEqualityComparer = new ExpressionEqualityComparer();
                var expressionDistinctObjectsArray = cubismExpressionObjectsToList.Distinct(expressionEqualityComparer).ToArray();

                expressionList.CubismExpressionObjects = new CubismExpressionData[0];
                Array.Resize(ref expressionList.CubismExpressionObjects, expressionDistinctObjectsArray.Length);

                expressionList.CubismExpressionObjects = expressionDistinctObjectsArray;
            }

            // Find index.
            var expressionName = Path.GetFileName(AssetPath).Replace(".json", "");
            var expressionIndex = -1;
            for (var i = 0; i < expressionList.CubismExpressionObjects.Length; ++i)
            {
                var expression = expressionList.CubismExpressionObjects[i];

                if (expression.name != expressionName)
                {
                    continue;
                }

                expressionIndex = i;
                break;
            }

            // Set expression data.
            if (expressionIndex != -1)
            {
                expressionList.CubismExpressionObjects[expressionIndex] = expressionData;
            }
            else
            {
                expressionIndex = expressionList.CubismExpressionObjects.Length;
                Array.Resize(ref expressionList.CubismExpressionObjects, expressionIndex + 1);
                expressionList.CubismExpressionObjects[expressionIndex] = expressionData;
            }

            EditorUtility.SetDirty(expressionList);

        }


        /// <summary>
        /// Set expression list.
        /// </summary>
        /// <param name="importer">Event source.</param>
        /// <param name="model">Imported model.</param>
        private static void OnModelImport(CubismModel3JsonImporter importer, CubismModel model)
        {
            var expressionController = model.GetComponent<CubismExpressionController>();
            if (expressionController == null || importer.Model3Json.FileReferences.Expressions == null)
            {
                return;
            }

            var modelDir = Path.GetDirectoryName(importer.AssetPath).Replace("\\","/");
            var modelName = Path.GetFileName(modelDir);
            var expressionListPath = modelDir + "/" + modelName + ".expressionList.asset";

            var expressionList = GetExpressionList(expressionListPath);

            if (expressionList == null)
            {
                return;
            }

            expressionController.ExpressionsList = expressionList;
        }

        #endregion

        /// <summary>
        /// Load the .expressionList.
        /// If it does not exist, create a new one.
        /// </summary>
        /// <param name="expressionListPath">The path of the .expressionList.asset relative to the project.</param>
        /// <returns>.expressionList.asset</returns>
        private static CubismExpressionList GetExpressionList(string expressionListPath)
        {
            var assetList = CubismCreatedAssetList.GetInstance();
            var assetListIndex = assetList.AssetPaths.Contains(expressionListPath)
                ? assetList.AssetPaths.IndexOf(expressionListPath)
                : -1;

            CubismExpressionList expressionList = null;

            if (assetListIndex < 0)
            {
                expressionList = AssetDatabase.LoadAssetAtPath<CubismExpressionList>(expressionListPath);

                if (expressionList == null)
                {
                    expressionList = ScriptableObject.CreateInstance<CubismExpressionList>();
                    expressionList.CubismExpressionObjects = new CubismExpressionData[0];
                    AssetDatabase.CreateAsset(expressionList, expressionListPath);
                }

                assetList.Assets.Add(expressionList);
                assetList.AssetPaths.Add(expressionListPath);
                assetList.IsImporterDirties.Add(true);
            }
            else
            {
                expressionList = (CubismExpressionList)assetList.Assets[assetListIndex];
            }

            return expressionList;
        }

        private class ExpressionEqualityComparer : IEqualityComparer<CubismExpressionData>
        {
            public bool Equals(CubismExpressionData x, CubismExpressionData y)
            {
                return x.name == y.name;
            }

            public int GetHashCode(CubismExpressionData obj)
            {
                return obj.GetHashCode();
            }
        }
    }
}
