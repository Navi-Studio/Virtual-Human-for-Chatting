/**
 * Copyright(c) Live2D Inc. All rights reserved.
 *
 * Use of this source code is governed by the Live2D Open Software license
 * that can be found at https://www.live2d.com/eula/live2d-open-software-license-agreement_en.html.
 */


using System;
using UnityEditor;
using UnityEngine;


namespace Live2D.Cubism.Editor.Importers
{
    /// <summary>
    /// Base class for Cubism asset importers.
    /// </summary>
    [Serializable]
    public abstract class CubismImporterBase : ICubismImporter
    {
        /// <summary>
        /// Gets the path to the imported asset.
        /// </summary>
        public string AssetPath { get; private set; }


        /// <summary>
        /// Imports the corresponding asset.
        /// </summary>
        public abstract void Import();


        /// <summary>
        /// Saves the importer state and reimports the asset.
        /// </summary>
        public void Save()
        {
            var assetImporter = AssetImporter.GetAtPath(AssetPath);


            assetImporter.userData = JsonUtility.ToJson(this);


            assetImporter.SaveAndReimport();
        }

#region ICubismImporter

        /// <summary>
        /// Sets the asset path.
        /// </summary>
        void ICubismImporter.SetAssetPath(string value)
        {
            AssetPath = value;
        }

        /// <summary>
        /// Imports the corresponding asset.
        /// </summary>
        void ICubismImporter.Import()
        {
            Import();
        }

        /// <summary>
        /// Saves the importer state and reimports the asset.
        /// </summary>
        void ICubismImporter.Save()
        {
            Save();
        }

#endregion
    }
}
