/**
 * Copyright(c) Live2D Inc. All rights reserved.
 *
 * Use of this source code is governed by the Live2D Open Software license
 * that can be found at https://www.live2d.com/eula/live2d-open-software-license-agreement_en.html.
 */


using System;

namespace Live2D.Cubism.Editor.Deleters
{
    /// <summary>
    /// Base class for Cubism asset deleters.
    /// </summary>
    [Serializable]
    public abstract class CubismDeleterBase : ICubismDeleter
    {
        /// <summary>
        /// Gets the path to the imported asset.
        /// </summary>
        public string AssetPath { get; private set; }

        /// <summary>
        /// Imports the corresponding asset.
        /// </summary>
        public abstract void Delete();

        #region ICubismDeleter

        /// <summary>
        /// Sets the asset path.
        /// </summary>
        void ICubismDeleter.SetAssetPath(string value)
        {
            AssetPath = value;
        }

        /// <summary>
        /// Imports the corresponding asset.
        /// </summary>
        void ICubismDeleter.Delete()
        {
            Delete();
        }

        #endregion
    }
}
