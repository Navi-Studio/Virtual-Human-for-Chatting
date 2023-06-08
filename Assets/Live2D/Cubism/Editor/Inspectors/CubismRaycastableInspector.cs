/**
 * Copyright(c) Live2D Inc. All rights reserved.
 *
 * Use of this source code is governed by the Live2D Open Software license
 * that can be found at https://www.live2d.com/eula/live2d-open-software-license-agreement_en.html.
 */


using Live2D.Cubism.Framework.Raycasting;
using UnityEditor;

namespace Live2D.Cubism.Editor.Inspectors
{
    /// <summary>
    /// Inspector for <see cref="CubismRaycastable"/>s.
    /// </summary>
    [CustomEditor(typeof(CubismRaycastable)), CanEditMultipleObjects]
    public class CubismRaycastableInspector : UnityEditor.Editor
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

            // Display precision.
            var precision = serializedObject.FindProperty("Precision");
            EditorGUILayout.PropertyField(precision);

            // Save any changes.
            if (EditorGUI.EndChangeCheck())
            {
                serializedObject.ApplyModifiedProperties();
            }
        }

        #endregion
    }
}
