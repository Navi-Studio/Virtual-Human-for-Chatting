/**
 * Copyright(c) Live2D Inc. All rights reserved.
 *
 * Use of this source code is governed by the Live2D Open Software license
 * that can be found at https://www.live2d.com/eula/live2d-open-software-license-agreement_en.html.
 */


using System.IO;
using UnityEditor;
using UnityEngine;


namespace Live2D.Cubism.Editor.OriginalWorkflow
{
    /// <summary>
    /// ScriptableObject to save cubism original workflow setting.
    /// </summary>
    public class CubismOriginalWorkflowSettings: ScriptableObject
    {
        /// <summary>
        /// Should import as original workflow.
        /// </summary>
        [SerializeField, HideInInspector]
        public bool ShouldImportAsOriginalWorkflow = true;

        /// <summary>
        /// Should clear animation clip curves.
        /// </summary>
        [SerializeField, HideInInspector]
        public bool ShouldClearAnimationCurves = false;

        /// <summary>
        /// The cubism original workflow settings.
        /// </summary>
        /// <returns></returns>
        public static CubismOriginalWorkflowSettings OriginalWorkflowSettings
        {
            get
            {
                var setting = Resources.Load<CubismOriginalWorkflowSettings>("Live2D/Cubism/OriginalWorkflowSettings");

                if(setting == null)
                {
                    setting = CreateInstance<CubismOriginalWorkflowSettings>();

                    var directory = "Assets/Live2D/Cubism/Editor/Resources/Live2D/Cubism/";
                    if(!Directory.Exists(directory))
                    {
                        Directory.CreateDirectory(directory);
                    }

                    AssetDatabase.CreateAsset(setting, "Assets/Live2D/Cubism/Editor/Resources/Live2D/Cubism/OriginalWorkflowSettings.asset");
                }

               return setting;
            }
        }
    }
}
