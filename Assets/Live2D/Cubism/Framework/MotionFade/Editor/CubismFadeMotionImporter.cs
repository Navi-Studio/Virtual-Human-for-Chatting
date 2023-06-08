/**
 * Copyright(c) Live2D Inc. All rights reserved.
 *
 * Use of this source code is governed by the Live2D Open Software license
 * that can be found at https://www.live2d.com/eula/live2d-open-software-license-agreement_en.html.
 */


using Live2D.Cubism.Core;
using Live2D.Cubism.Editor;
using Live2D.Cubism.Editor.Importers;
using System;
using System.IO;
using UnityEditor;
using UnityEditor.Animations;
using UnityEngine;


namespace Live2D.Cubism.Framework.MotionFade
{
    internal static class CubismFadeMotionImporter
    {
        #region Unity Event Handling

        /// <summary>
        /// Register fadeMotion importer.
        /// </summary>
        [InitializeOnLoadMethod]
        private static void RegisterMotionImporter()
        {
            CubismImporter.OnDidImportModel += OnModelImport;
            CubismImporter.OnDidImportMotion += OnFadeMotionImport;
        }

        #endregion

        #region Cubism Import Event Handling

        /// <summary>
        /// Create animator controller for MotionFade.
        /// </summary>
        /// <param name="importer">Event source.</param>
        /// <param name="model">Imported model.</param>
        private static void OnModelImport(CubismModel3JsonImporter importer, CubismModel model)
        {
            var dataPath = Directory.GetParent(Application.dataPath).FullName + "/";
            var assetPath = importer.AssetPath.Replace(".model3.json", ".controller");

            if (!File.Exists(dataPath + assetPath))
            {
                CreateAnimatorController(assetPath);
            }

            var fadeController = model.GetComponent<CubismFadeController>();
            if (importer.Model3Json.FileReferences.Motions.Motions == null || fadeController == null)
            {
                return;
            }

            var modelDir = Path.GetDirectoryName(importer.AssetPath).Replace("\\", "/");
            var modelName = Path.GetFileName(modelDir);
            var fadeMotionListPath = modelDir + "/" + modelName + ".fadeMotionList.asset";

            var fadeMotions = GetFadeMotionList(fadeMotionListPath);

            if (fadeMotions == null)
            {
                return;
            }

            fadeController.CubismFadeMotionList = fadeMotions;
        }

        /// <summary>
        /// Create oldFadeMotion.
        /// </summary>
        /// <param name="importer">Event source.</param>
        /// <param name="animationClip">Imported motion.</param>
        private static void OnFadeMotionImport(CubismMotion3JsonImporter importer, AnimationClip animationClip)
        {
            // Add reference of motion for Fade to list.
            var directoryName = Path.GetDirectoryName(importer.AssetPath);
            var modelDir = Path.GetDirectoryName(directoryName);
            var modelName = Path.GetFileName(modelDir);
            var fadeMotionListPath = modelDir + "/" + modelName + ".fadeMotionList.asset";

            var fadeMotions = GetFadeMotionList(fadeMotionListPath);

            if (fadeMotions == null)
            {
                Debug.LogError("CubismFadeMotionImporter : Can not create CubismFadeMotionList.");
                return;
            }

            var instanceId = 0;
            var isExistInstanceId = false;
            var events = animationClip.events;
            for (var k = 0; k < events.Length; ++k)
            {
                if (events[k].functionName != "InstanceId")
                {
                    continue;
                }

                instanceId = events[k].intParameter;
                isExistInstanceId = true;
                break;
            }

            if (!isExistInstanceId)
            {
                instanceId = animationClip.GetInstanceID();
            }


            var motionName = Path.GetFileName(importer.AssetPath);
            var motionIndex = -1;

            for (var i = 0; i < fadeMotions.CubismFadeMotionObjects.Length; i++)
            {
                if (Path.GetFileName(fadeMotions.CubismFadeMotionObjects[i].MotionName) != motionName)
                {
                    continue;
                }

                motionIndex = i;
                break;
            }

            // Create fade motion.
            CubismFadeMotionData fadeMotion;
            if (motionIndex != -1)
            {
                var oldFadeMotion = fadeMotions.CubismFadeMotionObjects[motionIndex];

                fadeMotion = CubismFadeMotionData.CreateInstance(
                    oldFadeMotion,
                    importer.Motion3Json,
                    importer.AssetPath,
                    animationClip.length,
                    CubismUnityEditorMenu.ShouldImportAsOriginalWorkflow,
                    CubismUnityEditorMenu.ShouldClearAnimationCurves);

                EditorUtility.CopySerialized(fadeMotion, oldFadeMotion);

                fadeMotions.MotionInstanceIds[motionIndex] = instanceId;
                fadeMotions.CubismFadeMotionObjects[motionIndex] = fadeMotion;
            }
            else
            {
                // Create fade motion instance.
                fadeMotion = CubismFadeMotionData.CreateInstance(
                    importer.Motion3Json,
                    importer.AssetPath,
                    animationClip.length,
                    CubismUnityEditorMenu.ShouldImportAsOriginalWorkflow,
                    CubismUnityEditorMenu.ShouldClearAnimationCurves);

                AssetDatabase.CreateAsset(
                    fadeMotion,
                    importer.AssetPath.Replace(".motion3.json", ".fade.asset"));

                motionIndex = fadeMotions.MotionInstanceIds.Length;

                Array.Resize(ref fadeMotions.MotionInstanceIds, motionIndex + 1);
                fadeMotions.MotionInstanceIds[motionIndex] = instanceId;

                Array.Resize(ref fadeMotions.CubismFadeMotionObjects, motionIndex + 1);
                fadeMotions.CubismFadeMotionObjects[motionIndex] = fadeMotion;
            }

            EditorUtility.SetDirty(fadeMotion);

            // Add animation event
            {
                var sourceAnimationEvents = AnimationUtility.GetAnimationEvents(animationClip);
                var index = -1;

                for(var i = 0; i < sourceAnimationEvents.Length; ++i)
                {
                    if(sourceAnimationEvents[i].functionName != "InstanceId")
                    {
                        continue;
                    }

                    index = i;
                    break;
                }

                if(index == -1)
                {
                    index = sourceAnimationEvents.Length;
                    Array.Resize(ref sourceAnimationEvents, sourceAnimationEvents.Length + 1);
                    sourceAnimationEvents[sourceAnimationEvents.Length - 1] = new AnimationEvent();
                }

                sourceAnimationEvents[index].time = 0;
                sourceAnimationEvents[index].functionName = "InstanceId";
                sourceAnimationEvents[index].intParameter = instanceId;
                sourceAnimationEvents[index].messageOptions = SendMessageOptions.DontRequireReceiver;

                AnimationUtility.SetAnimationEvents(animationClip, sourceAnimationEvents);
            }
        }

        #endregion


        #region Functions

        /// <summary>
        /// Create animator controller for MotionFade.
        /// </summary>
        /// <param name="assetPath"></param>
        /// <returns>Animator controller attached CubismFadeStateObserver.</returns>
        public static AnimatorController CreateAnimatorController(string assetPath)
        {
            var animatorController = AnimatorController.CreateAnimatorControllerAtPath(assetPath);
            animatorController.layers[0].stateMachine.AddStateMachineBehaviour<CubismFadeStateObserver>();

            return animatorController;
        }

        /// <summary>
        /// Load the .fadeMotionList.
        /// If it does not exist, create a new one.
        /// </summary>
        /// <param name="fadeMotionListPath">The path of the .fadeMotionList.asset relative to the project.</param>
        /// <returns>.fadeMotionList.asset.</returns>
        private static CubismFadeMotionList GetFadeMotionList(string fadeMotionListPath)
        {
            var assetList = CubismCreatedAssetList.GetInstance();
            var assetListIndex = assetList.AssetPaths.Contains(fadeMotionListPath)
                ? assetList.AssetPaths.IndexOf(fadeMotionListPath)
                : -1;

            CubismFadeMotionList fadeMotions = null;

            if (assetListIndex < 0)
            {
                fadeMotions = AssetDatabase.LoadAssetAtPath<CubismFadeMotionList>(fadeMotionListPath);

                if (fadeMotions == null)
                {
                    // Create reference list.
                    fadeMotions = ScriptableObject.CreateInstance<CubismFadeMotionList>();
                    fadeMotions.MotionInstanceIds = new int[0];
                    fadeMotions.CubismFadeMotionObjects = new CubismFadeMotionData[0];
                    AssetDatabase.CreateAsset(fadeMotions, fadeMotionListPath);
                }

                assetList.Assets.Add(fadeMotions);
                assetList.AssetPaths.Add(fadeMotionListPath);
                assetList.IsImporterDirties.Add(true);
            }
            else
            {
                fadeMotions = (CubismFadeMotionList)assetList.Assets[assetListIndex];
            }

            return fadeMotions;
        }

        #endregion
    }
}
