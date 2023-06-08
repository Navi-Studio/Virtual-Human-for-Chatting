/**
 * Copyright(c) Live2D Inc. All rights reserved.
 *
 * Use of this source code is governed by the Live2D Open Software license
 * that can be found at https://www.live2d.com/eula/live2d-open-software-license-agreement_en.html.
 */


using Live2D.Cubism.Framework;
using UnityEditor;

namespace Live2D.Cubism.Editor.Inspectors
{
    /// <summary>
    /// Inspector for <see cref="CubismHarmonicMotionParameter"/>s.
    /// </summary>
    [CustomEditor(typeof(CubismDisplayInfoParameterName)), CanEditMultipleObjects]
    public class CubismDisplayInfoParameterNameInspector : UnityEditor.Editor
    {
        #region Editor

        /// <summary>
        /// Draws inspector.
        /// </summary>
        public override void OnInspectorGUI()
        {
            // Fail silently.
            if (serializedObject == null)
            {
                return;
            }


            serializedObject.Update();

            EditorGUI.BeginChangeCheck();

            // Display name.
            var displayName = serializedObject.FindProperty("DisplayName");
            EditorGUILayout.PropertyField(displayName);

            // Save any changes.
            if (EditorGUI.EndChangeCheck())
            {
                serializedObject.ApplyModifiedProperties();
            }
        }
        #endregion
    }
}
