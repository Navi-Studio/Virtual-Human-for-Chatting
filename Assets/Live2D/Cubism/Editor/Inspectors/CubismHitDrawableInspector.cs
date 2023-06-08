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
    [CustomEditor(typeof(CubismHitDrawable)), CanEditMultipleObjects]
    public class CubismHitDrawableInspector : UnityEditor.Editor
    {
        #region Editor

        /// <summary>
        /// Draws inspector.
        /// </summary>
        public override void OnInspectorGUI()
        {
            var hitDrawable = target as CubismHitDrawable;


            // Fail silently.
            if (hitDrawable == null)
            {
                return;
            }


            // Display user data.
            using (var scope = new EditorGUI.ChangeCheckScope())
            {
                var name = EditorGUILayout.TextField("Precision", hitDrawable.Name);

                if (!scope.changed)
                {
                    return;
                }


                // Apply to all selected HitDrawable.
                foreach (CubismHitDrawable cubismRaycastable in targets)
                {
                    cubismRaycastable.Name = name;

                    // Save any changes.
                    EditorUtility.SetDirty(cubismRaycastable);
                }
            }
        }

        #endregion
    }
}
