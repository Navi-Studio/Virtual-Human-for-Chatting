/**
 * Copyright(c) Live2D Inc. All rights reserved.
 *
 * Use of this source code is governed by the Live2D Open Software license
 * that can be found at https://www.live2d.com/eula/live2d-open-software-license-agreement_en.html.
 */


namespace Live2D.Cubism.Editor.Deleters
{
    /// <summary>
    /// Common interface for Cubism asset deleter.
    /// </summary>
    public interface ICubismDeleter
    {
        /// <summary>
        /// Sets the asset path.
        /// </summary>
        void SetAssetPath(string value);


        /// <summary>
        /// Delete the corresponding asset.
        /// </summary>
        void Delete();
    }
}
