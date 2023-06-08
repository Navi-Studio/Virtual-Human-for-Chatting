/**
 * Copyright(c) Live2D Inc. All rights reserved.
 *
 * Use of this source code is governed by the Live2D Open Software license
 * that can be found at https://www.live2d.com/eula/live2d-open-software-license-agreement_en.html.
 */


using Live2D.Cubism.Core;
using Live2D.Cubism.Rendering;
using Live2D.Cubism.Editor.Importers;
using System.Linq;
using UnityEngine;
using UnityEditor;


namespace Live2D.Cubism.Samples.Editor
{
    /// <summary>
    /// Shows how Cubism model importing can be customized.
    /// </summary>
    internal static class ImportCustomization
    {
        #region Unity Event Handling

        /// <summary>
        /// Registers processor.
        /// </summary>
        /// <remarks>
        /// BE CAREFUL! Uncomment the following line to enable the sample.
        //  DON'T DO THIS IN REAL PROJECTS!
        /// </remarks>
        // [InitializeOnLoadMethod]
        private static void RegisterModelImporter()
        {
            CubismImporter.OnDidImportModel += OnModelImport;
        }

        #endregion

        #region Cubism Import Event Handling

        /// <summary>
        /// Customizes model importing.
        /// </summary>
        /// <param name="sender">Event source.</param>
        /// <param name="model">Imported model.</param>
        private static void OnModelImport(CubismModel3JsonImporter sender, CubismModel model)
        {
            // Lets pretend we want to change the vertex colors of all drawables to green...
            foreach (var renderer in model.Drawables.Select(d => d.GetComponent<CubismRenderer>()))
            {
              renderer.Color = Color.green;
            }
        }

        #endregion
    }
}
