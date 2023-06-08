/**
 * Copyright(c) Live2D Inc. All rights reserved.
 *
 * Use of this source code is governed by the Live2D Open Software license
 * that can be found at https://www.live2d.com/eula/live2d-open-software-license-agreement_en.html.
 */


using System;
using System.Collections.Generic;
using UnityEngine;


namespace Live2D.Cubism.Editor.Deleters
{
    /// <summary>
    /// Helper functionality for <see cref="ICubismDeleter"/>s.
    /// </summary>
    public static class CubismDeleter
    {
        /// <summary>
        /// Tries to get an deleter for a Cubism asset.
        /// </summary>
        /// <typeparam name="T">Deleter type.</typeparam>
        /// <param name="assetPath">Path to the asset.</param>
        /// <returns>The deleter on success; <see langword="null"/> otherwise.</returns>
        public static T GetDeleterAsPath<T>(string assetPath) where T : class, ICubismDeleter
        {
            return GetDeleterAsPath(assetPath) as T;
        }

        /// <summary>
        /// Tries to deserialize an deleter from <see cref="AssetDeleter.userData"/>.
        /// </summary>
        /// <param name="assetPath">Path to the asset.</param>
        /// <returns>The deleter on success; <see langword="null"/> otherwise.</returns>
        public static ICubismDeleter GetDeleterAsPath(string assetPath)
        {
            var deleterEntry = _registry.Find(e => assetPath.EndsWith(e.FileExtension));


            // Return early in case no valid deleter is registered.
            if (deleterEntry.DeleterType == null)
            {
                return null;
            }


            var deleter = Activator.CreateInstance(deleterEntry.DeleterType) as ICubismDeleter;

            // Finalize deleter initialization.
            if (deleter != null)
            {
                deleter.SetAssetPath(assetPath);
            }


            return deleter;
        }


        #region Registry

        /// <summary>
        /// Registry entry.
        /// </summary>
        private struct DeleterEntry
        {
            /// <summary>
            /// Deleter type.
            /// </summary>
            public Type DeleterType;

            /// <summary>
            /// File extension valid for the deleter.
            /// </summary>
            public string FileExtension;
        }


        /// <summary>
        /// List of registered <see cref="ICubismDeleter"/>s.
        /// </summary>
        private static List<DeleterEntry> _registry = new List<DeleterEntry>();


        /// <summary>
        /// Registers an deleter type.
        /// </summary>
        /// <typeparam name="T">The type of deleter to register.</typeparam>
        /// <param name="fileExtension">The file extension the deleter supports.</param>
        internal static void RegisterDeleter<T>(string fileExtension) where T : ICubismDeleter
        {
            _registry.Add(new DeleterEntry
            {
                DeleterType = typeof(T),
                FileExtension = fileExtension
            });
        }

        #endregion
    }
}
