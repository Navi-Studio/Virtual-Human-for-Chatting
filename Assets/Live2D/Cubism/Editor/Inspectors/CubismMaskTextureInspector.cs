/**
 * Copyright(c) Live2D Inc. All rights reserved.
 *
 * Use of this source code is governed by the Live2D Open Software license
 * that can be found at https://www.live2d.com/eula/live2d-open-software-license-agreement_en.html.
 */


using Live2D.Cubism.Rendering.Masking;
using UnityEditor;
using UnityEngine;


namespace Live2D.Cubism.Editor.Inspectors
{
    /// <summary>
    /// Inspector for <see cref="CubismMaskTexture"/>s.
    /// </summary>
    [CustomEditor(typeof(CubismMaskTexture))]
    internal sealed class CubismMaskTextureInspector : UnityEditor.Editor
    {
        #region Editor

        /// <summary>
        /// Draws inspector.
        /// </summary>
        public override void OnInspectorGUI()
        {
            var texture = target as CubismMaskTexture;


            // Fail silently.
            if (texture == null)
            {
                return;
            }


            // Show settings.
            EditorGUI.BeginChangeCheck();


            texture.Size = EditorGUILayout.IntField("Size (In Pixels)", texture.Size);
            texture.Subdivisions = EditorGUILayout.IntSlider("Subdivisions", texture.Subdivisions, 1, 5);
            EditorGUILayout.ObjectField("Render Texture (Read-only)", (RenderTexture) texture, typeof(RenderTexture), false);


            // Save any changes.
            if (EditorGUI.EndChangeCheck())
            {
                EditorUtility.SetDirty(texture);
            }
        }

        #endregion
    }
}
