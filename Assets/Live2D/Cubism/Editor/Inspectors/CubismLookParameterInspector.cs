/**
 * Copyright(c) Live2D Inc. All rights reserved.
 *
 * Use of this source code is governed by the Live2D Open Software license
 * that can be found at https://www.live2d.com/eula/live2d-open-software-license-agreement_en.html.
 */


using Live2D.Cubism.Framework.LookAt;
using UnityEditor;

namespace Live2D.Cubism.Editor.Inspectors
{
    /// <summary>
    /// Inspector for <see cref="CubismLookParameter"/>s.
    /// </summary>
    [CustomEditor(typeof(CubismLookParameter)), CanEditMultipleObjects]
    public class CubismLookParameterInspector : UnityEditor.Editor
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


            // Display axis.
            var axis = serializedObject.FindProperty("Axis");
            EditorGUILayout.PropertyField(axis);


            // Display factor.
            var factor = serializedObject.FindProperty("Factor");
            EditorGUILayout.PropertyField(factor);


            // Save any changes.
            if (EditorGUI.EndChangeCheck())
            {
                serializedObject.ApplyModifiedProperties();
            }
        }
        #endregion
    }
}
