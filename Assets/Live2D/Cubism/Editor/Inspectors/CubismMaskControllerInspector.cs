/**
 * Copyright(c) Live2D Inc. All rights reserved.
 *
 * Use of this source code is governed by the Live2D Open Software license
 * that can be found at https://www.live2d.com/eula/live2d-open-software-license-agreement_en.html.
 */


using Live2D.Cubism.Rendering.Masking;
using UnityEditor;


namespace Live2D.Cubism.Editor.Inspectors
{
    /// <summary>
    /// Inspects <see cref="CubismMaskController"/>s.
    /// </summary>
    [CustomEditor(typeof(CubismMaskController))]
    internal sealed class CubismMaskControllerInspector : UnityEditor.Editor
    {
        #region Editor

        /// <summary>
        /// Draws the inspector.
        /// </summary>
        public override void OnInspectorGUI()
        {
            var controller = target as CubismMaskController;


            // Fail silently.
            if (controller == null)
            {
                return;
            }


            // Draw default inspector.
            base.OnInspectorGUI();


            // Draw mask texture.
            EditorGUI.BeginChangeCheck();


            controller.MaskTexture = EditorGUILayout.ObjectField("Mask Texture", controller.MaskTexture, typeof(CubismMaskTexture), true) as CubismMaskTexture;


            // Apply changes.
            if (EditorGUI.EndChangeCheck())
            {
                EditorUtility.SetDirty(controller);
            }
        }

        #endregion
    }
}
