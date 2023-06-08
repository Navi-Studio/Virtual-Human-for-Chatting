/**
 * Copyright(c) Live2D Inc. All rights reserved.
 *
 * Use of this source code is governed by the Live2D Open Software license
 * that can be found at https://www.live2d.com/eula/live2d-open-software-license-agreement_en.html.
 */


using Live2D.Cubism.Framework.HarmonicMotion;
using UnityEditor;

namespace Live2D.Cubism.Editor.Inspectors
{
    /// <summary>
    /// Inspector for <see cref="CubismHarmonicMotionParameter"/>s.
    /// </summary>
    [CustomEditor(typeof(CubismHarmonicMotionParameter)), CanEditMultipleObjects]
    public sealed class CubismHarmonicMotionParameterInspector : UnityEditor.Editor
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


            // Display channel.
            var channel = serializedObject.FindProperty("Channel");
            EditorGUILayout.PropertyField(channel);

            // Display direction.
            var direction = serializedObject.FindProperty("Direction");
            EditorGUILayout.PropertyField(direction);

            // Display normalized origin.
            var normalizedOrigin = serializedObject.FindProperty("NormalizedOrigin");
            EditorGUILayout.PropertyField(normalizedOrigin);

            // Display normalized range.
            var normalizedRange = serializedObject.FindProperty("NormalizedRange");
            EditorGUILayout.PropertyField(normalizedRange);

            // Display duration.
            var duration = serializedObject.FindProperty("Duration");
            EditorGUILayout.PropertyField(duration);


            // Save any changes.
            if (EditorGUI.EndChangeCheck())
            {
                serializedObject.ApplyModifiedProperties();
            }
        }

        #endregion
    }
}
