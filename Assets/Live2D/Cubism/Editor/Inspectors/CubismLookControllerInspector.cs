/**
 * Copyright(c) Live2D Inc. All rights reserved.
 *
 * Use of this source code is governed by the Live2D Open Software license
 * that can be found at https://www.live2d.com/eula/live2d-open-software-license-agreement_en.html.
 */


using Live2D.Cubism.Framework.LookAt;
using UnityEditor;

using Object = UnityEngine.Object;


namespace Live2D.Cubism.Editor.Inspectors
{
    /// <summary>
    /// Inspects <see cref="CubismLookController"/>s.
    /// </summary>
    [CustomEditor(typeof(CubismLookController))]
    internal sealed class CubismLookControllerInspector : UnityEditor.Editor
    {
        #region Editor

        /// <summary>
        /// Draws the inspector.
        /// </summary>
        public override void OnInspectorGUI()
        {
            var controller = target as CubismLookController;


            // Fail silently.
            if (controller == null)
            {
                return;
            }


            EditorGUI.BeginChangeCheck();


            // Draw default inspector.
            base.OnInspectorGUI();


            // Draw target.
            controller.Target = EditorGUILayout.ObjectField("Target", controller.Target, typeof(Object), true);


            // Apply changes.
            if (EditorGUI.EndChangeCheck())
            {
                EditorUtility.SetDirty(controller);
            }
        }

        #endregion
    }
}
