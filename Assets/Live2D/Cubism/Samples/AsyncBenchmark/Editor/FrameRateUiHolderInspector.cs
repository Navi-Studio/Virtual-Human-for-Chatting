/**
 * Copyright(c) Live2D Inc. All rights reserved.
 *
 * Use of this source code is governed by the Live2D Open Software license
 * that can be found at https://www.live2d.com/eula/live2d-open-software-license-agreement_en.html.
 */


using UnityEditor;

namespace Live2D.Cubism.Samples.AsyncBenchmark.Editor
{
    /// <summary>
    /// Dynamically switch the display on <see cref="FrameRateUiHolder"/>  inspector.
    /// </summary>
    [CanEditMultipleObjects]
    [CustomEditor(typeof(FrameRateUiHolder))]
    public class FrameRateUiHolderInspector : UnityEditor.Editor
    {
        // Components to add inspector enhancements.
        private FrameRateUiHolder Target { get; set; }

        /// <summary>
        /// Called by Unity. Getting target component and Initializing.
        /// </summary>
        private void Awake()
        {
            Target = target as FrameRateUiHolder;
        }

        public override void OnInspectorGUI()
        {
            EditorGUI.BeginChangeCheck();

            // Load a value from the internal cache.
            serializedObject.Update();

            //Whether to enable total uptime.
            Target.HasShownElapsedTime = EditorGUILayout.ToggleLeft("Show Elapsed Time", Target.HasShownElapsedTime);
            // Enable/disable observation.
            Target.HasShownFrameRate = EditorGUILayout.ToggleLeft("Show Frame Rate", Target.HasShownFrameRate);

            // Load properties.
            var maximumFpsUi = serializedObject.FindProperty("HighestFrameRateUi");
            var minimumFpsUi = serializedObject.FindProperty("LowestFrameRateUi");
            var elapsedTimeUi = serializedObject.FindProperty("ElapsedTimeUi");

            // View and change properties.
            EditorGUILayout.PropertyField(maximumFpsUi);
            EditorGUILayout.PropertyField(minimumFpsUi);
            EditorGUILayout.PropertyField(elapsedTimeUi);

            // Apply changes to the serializedProperty.
            serializedObject.ApplyModifiedProperties();

            // Changes the state of objects.
            Target.HighestFrameRateUi.enabled = Target.HasShownFrameRate;
            Target.LowestFrameRateUi.enabled = Target.HasShownFrameRate;
            Target.ElapsedTimeUi.gameObject.SetActive(Target.HasShownElapsedTime);

            if (EditorGUI.EndChangeCheck())
            {
                // Apply changes and set dirty flag.
                EditorUtility.SetDirty(Target);
            }
        }
    }
}
