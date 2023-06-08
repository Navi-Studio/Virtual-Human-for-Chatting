/**
 * Copyright(c) Live2D Inc. All rights reserved.
 *
 * Use of this source code is governed by the Live2D Open Software license
 * that can be found at https://www.live2d.com/eula/live2d-open-software-license-agreement_en.html.
 */


using Live2D.Cubism.Framework.Pose;
using UnityEditor;

namespace Live2D.Cubism.Editor.Inspectors
{
    /// <summary>
    /// Inspector for <see cref="CubismPosePart"/>s.
    /// </summary>
    [CustomEditor(typeof(CubismPosePart)), CanEditMultipleObjects]
    public class CubismPosePartInspector : UnityEditor.Editor
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

            // Display group index.
            var groupIndex = serializedObject.FindProperty("GroupIndex");
            EditorGUILayout.PropertyField(groupIndex);

            // Display part index.
            var partIndex = serializedObject.FindProperty("PartIndex");
            EditorGUILayout.PropertyField(partIndex);

            // Display linked id.
            var link = serializedObject.FindProperty("Link");
            EditorGUILayout.PropertyField(link);



            // Save any changes.
            if (EditorGUI.EndChangeCheck())
            {
                serializedObject.ApplyModifiedProperties();
            }
        }

        #endregion
    }
}
