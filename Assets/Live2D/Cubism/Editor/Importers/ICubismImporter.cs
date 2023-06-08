/**
 * Copyright(c) Live2D Inc. All rights reserved.
 *
 * Use of this source code is governed by the Live2D Open Software license
 * that can be found at https://www.live2d.com/eula/live2d-open-software-license-agreement_en.html.
 */


namespace Live2D.Cubism.Editor.Importers
{
    /// <summary>
    /// Common interface for Cubism asset importers.
    /// </summary>
    public interface ICubismImporter
    {
        /// <summary>
        /// Sets the asset path.
        /// </summary>
        void SetAssetPath(string value);


        /// <summary>
        /// Imports the corresponding asset.
        /// </summary>
        void Import();


        /// <summary>
        /// Saves the importer.
        /// </summary>
        void Save();
    }
}
