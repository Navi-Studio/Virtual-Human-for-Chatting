/**
 * Copyright(c) Live2D Inc. All rights reserved.
 *
 * Use of this source code is governed by the Live2D Open Software license
 * that can be found at https://www.live2d.com/eula/live2d-open-software-license-agreement_en.html.
 */


using Live2D.Cubism.Core;
using Live2D.Cubism.Framework;
using UnityEditor;
using UnityEngine;


namespace Live2D.Cubism.Editor.Inspectors
{
    /// <summary>
    /// Allows inspecting <see cref="CubismPart"/>s.
    /// </summary>
    [CustomEditor(typeof(CubismPartsInspector))]
    internal sealed class CubismPartsInspectorInspector : UnityEditor.Editor
    {
        #region Editor

        /// <summary>
        /// Draws the inspector.
        /// </summary>
        public override void OnInspectorGUI()
        {
            // Lazily initialize.
            if (!IsInitialized)
            {
                Initialize();
            }


            // Show parts.
            var didPartsChange = false;


            for (var i = 0; i < Parts.Length; i++)
            {
                EditorGUI.BeginChangeCheck();

                var name = (string.IsNullOrEmpty(PartsNameFromJson[i]))
                    ? Parts[i].Id
                    : PartsNameFromJson[i];

                Parts[i].Opacity = EditorGUILayout.Slider(
                    name,
                    Parts[i].Opacity,
                    0f,
                    1f
                    );


                if (EditorGUI.EndChangeCheck())
                {
                    EditorUtility.SetDirty(Parts[i]);


                    didPartsChange = true;
                }
            }


            // FIXME Force model update in case parameters have changed.
            if (didPartsChange)
            {
                (target as Component)
                    .FindCubismModel()
                    .ForceUpdateNow();
            }
        }

        #endregion

        /// <summary>
        /// <see cref="CubismPart"/>s cache.
        /// </summary>
        private CubismPart[] Parts { get; set; }

        /// <summary>
        /// Array of <see cref="CubismDisplayInfoPartName.Name"/> obtained from <see cref="CubismDisplayInfoPartName"/>s.
        /// </summary>
        private string[] PartsNameFromJson { get; set; }

        /// <summary>
        /// Gets whether <see langword="this"/> is initialized.
        /// </summary>
        private bool IsInitialized
        {
            get
            {
                return Parts != null;
            }
        }


        /// <summary>
        /// Initializes <see langword="this"/>.
        /// </summary>
        private void Initialize()
        {
            Parts = (target as Component)
                .FindCubismModel(true)
                .Parts;

            //Initializing the property of `PartsNameFromJson `.
            PartsNameFromJson = new string[Parts.Length];

            for (var i = 0; i < Parts.Length; i++)
            {
                var displayInfoParstName = Parts[i].GetComponent<CubismDisplayInfoPartName>();
                PartsNameFromJson[i] = displayInfoParstName != null
                                ? (string.IsNullOrEmpty(displayInfoParstName.DisplayName) ? displayInfoParstName.Name : displayInfoParstName.DisplayName)
                                : string.Empty;
            }
        }
    }
}
