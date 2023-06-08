/**
 * Copyright(c) Live2D Inc. All rights reserved.
 *
 * Use of this source code is governed by the Live2D Open Software license
 * that can be found at https://www.live2d.com/eula/live2d-open-software-license-agreement_en.html.
 */


using UnityEditor;

using Object = UnityEngine.Object;


namespace Live2D.Cubism.Editor.Importers
{
    /// <summary>
    /// Provides helper methods for working with Unity assets.
    /// </summary>
    internal static class AssetGuid
    {

        /// <summary>
        /// Loads an asset by Guid.
        /// </summary>
        /// <typeparam name="T">The type of asset to load.</typeparam>
        /// <param name="guid">The guid to query for.</param>
        /// <returns>The loaded asset on </returns>
        public static T LoadAsset<T>(string guid) where T : Object
        {
            return AssetDatabase.LoadAssetAtPath<T>(AssetDatabase.GUIDToAssetPath(guid));
        }

        /// <summary>
        /// Gets the Guid of an asset.
        /// </summary>
        /// <param name="asset"></param>
        /// <returns></returns>
        public static string GetGuid<T>(T asset) where T : Object
        {
            return AssetDatabase.AssetPathToGUID(AssetDatabase.GetAssetPath(asset));
        }
    }
}
