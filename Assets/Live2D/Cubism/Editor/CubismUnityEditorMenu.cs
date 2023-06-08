/**
 * Copyright(c) Live2D Inc. All rights reserved.
 *
 * Use of this source code is governed by the Live2D Open Software license
 * that can be found at https://www.live2d.com/eula/live2d-open-software-license-agreement_en.html.
 */

using System;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using Live2D.Cubism.Editor.OriginalWorkflow;
using Live2D.Cubism.Framework.MotionFade;
using UnityEditor;
using UnityEngine;


namespace Live2D.Cubism.Editor
{
    /// <summary>
    /// Cubism unity editor menu.
    /// </summary>
    public class CubismUnityEditorMenu
    {
        /// <summary>
        /// Should import as original workflow.
        /// </summary>
        public static bool ShouldImportAsOriginalWorkflow
        {
            get
            {
                return CubismOriginalWorkflowSettings.OriginalWorkflowSettings.ShouldImportAsOriginalWorkflow;
            }
            set
            {
                CubismOriginalWorkflowSettings.OriginalWorkflowSettings.ShouldImportAsOriginalWorkflow = value;
                EditorUtility.SetDirty(CubismOriginalWorkflowSettings.OriginalWorkflowSettings);
            }
        }

        /// <summary>
        /// Should clear animation clip curves.
        /// </summary>
        public static bool ShouldClearAnimationCurves
        {
            get
            {
                return CubismOriginalWorkflowSettings.OriginalWorkflowSettings.ShouldClearAnimationCurves;
            }
            set
            {
                CubismOriginalWorkflowSettings.OriginalWorkflowSettings.ShouldClearAnimationCurves = value;
                EditorUtility.SetDirty(CubismOriginalWorkflowSettings.OriginalWorkflowSettings);
            }
        }


        /// <summary>
        /// Unity editor menu should import as original workflow.
        /// </summary>
        [MenuItem ("Live2D/Cubism/OriginalWorkflow/Should Import As Original Workflow")]
        private static void ImportAsOriginalWorkflow()
        {
            SetImportAsOriginalWorkflow(!ShouldImportAsOriginalWorkflow);

            // Disable clear animation curves.
            if(!ShouldImportAsOriginalWorkflow)
            {
                SetClearAnimationCurves(false);
            }
        }

        /// <summary>
        /// Unity editor menu clear animation curves.
        /// </summary>
        [MenuItem ("Live2D/Cubism/OriginalWorkflow/Should Clear Animation Curves")]
        private static void ClearAnimationCurves()
        {
            SetClearAnimationCurves(!ShouldClearAnimationCurves);
        }


        /// <summary>
        /// Unity editor context menu create an animator controller for cubism.
        /// </summary>
        [MenuItem("Assets/Create/Live2D Cubism/Animator Controller for Cubism")]
        private static void CreateAnimatorController()
        {
            var dataPath = Directory.GetParent(Application.dataPath).FullName + "/";
            var assetPath = CubismUnityEditorUtility.GetCurrentDirectoryPath();

            var assetName = "";

            if (!File.Exists(dataPath + assetPath + "/New Cubism Animator Controller.controller"))
            {
                assetName = "New Cubism Animator Controller.controller";
            }
            else
            {
                var regex = new Regex(@"new cubism animator controller [0-9]+.controller");
                var files = Directory.GetFiles(dataPath + assetPath, "*.controller")
                    .Where(path=> regex.IsMatch(Path.GetFileName(path).ToLower()))
                    .OrderBy(f => f, StringComparer.OrdinalIgnoreCase)
                    .ToArray();

                for (var i = 0; i < files.Length; i++)
                {
                    var name = $"New Cubism Animator Controller {(i + 1)}.controller";

                    if (files[i].ToLower().EndsWith(name, StringComparison.OrdinalIgnoreCase))
                    {
                        continue;
                    }

                    assetName = name;
                    break;
                }

                if (string.IsNullOrEmpty(assetName))
                {
                    assetName = $"New Cubism Animator Controller {(files.Length + 1)}.controller";
                }
            }

            assetPath = Path.Combine(assetPath, assetName);

            CubismFadeMotionImporter.CreateAnimatorController(assetPath);
            AssetDatabase.Refresh();
        }

        /// <summary>
        /// Set import as original workflow.
        /// </summary>
        public static void SetImportAsOriginalWorkflow(bool isEnable)
        {
            ShouldImportAsOriginalWorkflow= isEnable;
            Menu.SetChecked ("Live2D/Cubism/OriginalWorkflow/Should Import As Original Workflow", ShouldImportAsOriginalWorkflow);
        }

        /// <summary>
        /// Set clear animation curves.
        /// </summary>
        public static void SetClearAnimationCurves(bool isEnable)
        {
            ShouldClearAnimationCurves= (ShouldImportAsOriginalWorkflow && isEnable);
            Menu.SetChecked ("Live2D/Cubism/OriginalWorkflow/Should Clear Animation Curves", ShouldClearAnimationCurves);
        }

        /// <summary>
        /// Initialize cubism menu.
        /// </summary>
        [InitializeOnLoadMethod]
        private static void Initialize()
        {
            EditorApplication.delayCall += () => Menu.SetChecked ("Live2D/Cubism/OriginalWorkflow/Should Import As Original Workflow", ShouldImportAsOriginalWorkflow);
            EditorApplication.delayCall += () => Menu.SetChecked ("Live2D/Cubism/OriginalWorkflow/Should Clear Animation Curves", ShouldClearAnimationCurves);
        }

    }
}
