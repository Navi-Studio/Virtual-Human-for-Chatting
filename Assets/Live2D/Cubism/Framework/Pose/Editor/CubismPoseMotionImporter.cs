/**
 * Copyright(c) Live2D Inc. All rights reserved.
 *
 * Use of this source code is governed by the Live2D Open Software license
 * that can be found at https://www.live2d.com/eula/live2d-open-software-license-agreement_en.html.
 */


using Live2D.Cubism.Core;
using Live2D.Cubism.Framework.Json;
using Live2D.Cubism.Framework.MotionFade;
using Live2D.Cubism.Framework.Pose;
using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;


namespace Live2D.Cubism.Editor.Importers
{
    /// <summary>
    /// Cubism pose motion importer.
    /// </summary>
    internal static class CubismPoseMotionImporter
    {
        #region Unity Event Handling

        /// <summary>
        /// Registers processor.
        /// </summary>
        [InitializeOnLoadMethod]
        private static void RegisterModelImporter()
        {
            CubismImporter.OnDidImportModel += OnModelImport;
        }

        #endregion

        #region Cubism Import Event Handling

        /// <summary>
        /// Create pose motions.
        /// </summary>
        /// <param name="sender">Event source.</param>
        /// <param name="model">Imported model.</param>
        private static void OnModelImport(CubismModel3JsonImporter sender, CubismModel model)
        {
            var shouldImportAsOriginalWorkflow = CubismUnityEditorMenu.ShouldImportAsOriginalWorkflow;
            var shouldClearAnimationCurves = CubismUnityEditorMenu.ShouldClearAnimationCurves;
            var pose3Json = sender.Model3Json.Pose3Json;

            // Fail silently...
            if(!shouldImportAsOriginalWorkflow || pose3Json == null)
            {
                return;
            }

            var assetsDirectoryPath = Application.dataPath.Replace("Assets", "");
            var assetPath = sender.AssetPath.Replace(assetsDirectoryPath, "");
            var fileReferences = sender.Model3Json.FileReferences;

            // Create pose animation clip
            var motions = new List<CubismModel3Json.SerializableMotion>();

            if (fileReferences.Motions.GroupNames != null)
            {
                for (var i = 0; i < fileReferences.Motions.GroupNames.Length; i++)
                {
                    motions.AddRange(fileReferences.Motions.Motions[i]);
                }
            }


            for(var i = 0; i < motions.Count; ++i)
            {
                var motionPath = Path.GetDirectoryName(assetPath) + "/" + motions[i].File;
                var jsonString = string.IsNullOrEmpty(motionPath)
                                    ? null
                                    : File.ReadAllText(motionPath);

                if(jsonString == null)
                {
                    continue;
                }

                var directoryPath = Path.GetDirectoryName(assetPath) + "/";
                var motion3Json = CubismMotion3Json.LoadFrom(jsonString);

                var animationClipPath = directoryPath + motions[i].File.Replace(".motion3.json", ".anim");
                animationClipPath = animationClipPath.Replace("\\", "/");

                var animationName = Path.GetFileNameWithoutExtension(motions[i].File.Replace(".motion3.json", ".anim"));
                var assetList = CubismCreatedAssetList.GetInstance();
                var assetListIndex = assetList.AssetPaths.Contains(animationClipPath)
                    ? assetList.AssetPaths.IndexOf(animationClipPath)
                    : -1;

                var oldAnimationClip = (shouldImportAsOriginalWorkflow)
                    ? (assetListIndex >= 0)
                        ? (AnimationClip)assetList.Assets[assetListIndex]
                        : AssetDatabase.LoadAssetAtPath<AnimationClip>(animationClipPath)
                    : null;

                var newAnimationClip = (oldAnimationClip == null)
                    ? motion3Json.ToAnimationClip(shouldImportAsOriginalWorkflow, shouldClearAnimationCurves, true, pose3Json)
                    : motion3Json.ToAnimationClip(oldAnimationClip, shouldImportAsOriginalWorkflow, shouldClearAnimationCurves, true,
                        pose3Json);
                newAnimationClip.name = animationName;

                if (assetListIndex < 0)
                {
                    // Create animation clip.
                    if (oldAnimationClip == null)
                    {
                        AssetDatabase.CreateAsset(newAnimationClip, animationClipPath);
                        oldAnimationClip = newAnimationClip;
                    }

                    assetList.Assets.Add(newAnimationClip);
                    assetList.AssetPaths.Add(animationClipPath);
                    assetList.IsImporterDirties.Add(false);
                }
                // Update animation clip.
                else
                {
                    EditorUtility.CopySerialized(newAnimationClip, oldAnimationClip);
                    EditorUtility.SetDirty(oldAnimationClip);
                    assetList.Assets[assetListIndex] = oldAnimationClip;
                }

                // Add animation event
                {
                    var instanceId = newAnimationClip.GetInstanceID();

                    var sourceAnimationEvents = AnimationUtility.GetAnimationEvents(newAnimationClip);
                    var index = -1;

                    for(var j = 0; j < sourceAnimationEvents.Length; ++j)
                    {
                        if(sourceAnimationEvents[j].functionName != "InstanceId")
                        {
                            continue;
                        }

                        index = j;
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

                    AnimationUtility.SetAnimationEvents(newAnimationClip, sourceAnimationEvents);
                }


                // update fade motion data.
                var fadeMotionPath = directoryPath + motions[i].File.Replace(".motion3.json", ".fade.asset");
                var fadeMotion = AssetDatabase.LoadAssetAtPath<CubismFadeMotionData>(fadeMotionPath);

                if (fadeMotion == null)
                {
                    fadeMotion = CubismFadeMotionData.CreateInstance(
                        motion3Json,
                        Path.GetFileName(motions[i].File),
                        newAnimationClip.length,
                        shouldImportAsOriginalWorkflow,
                        true);

                    AssetDatabase.CreateAsset(fadeMotion, fadeMotionPath);
                }

                // Motion references for Fade added to list.
                var directoryName = Path.GetDirectoryName(fadeMotionPath).ToString();
                var modelDir = Path.GetDirectoryName(directoryName).ToString();
                var modelName = Path.GetFileName(modelDir).ToString();
                var fadeMotionListPath = Path.GetDirectoryName(directoryName).ToString() + "/" + modelName + ".fadeMotionList.asset";

                assetList = CubismCreatedAssetList.GetInstance();
                assetListIndex = assetList.AssetPaths.Contains(fadeMotionListPath)
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

                if (fadeMotions == null)
                {
                    Debug.LogError("CubismPoseMotionImporter : Can not create CubismFadeMotionList.");
                    return;
                }

                var motionIndex = -1;
                var motionName = Path.GetFileName(motions[i].File);

                for (var fadeMotionIndex = 0; fadeMotionIndex < fadeMotions.CubismFadeMotionObjects.Length; fadeMotionIndex++)
                {
                    if (Path.GetFileName(fadeMotions.CubismFadeMotionObjects[fadeMotionIndex].MotionName) != motionName)
                    {
                        continue;
                    }

                    motionIndex = fadeMotionIndex;
                    break;
                }

                // Create fade motion.
                if (motionIndex != -1)
                {
                    var instanceId = 0;
                    var isExistInstanceId = false;
                    var events = newAnimationClip.events;
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
                        instanceId = newAnimationClip.GetInstanceID();
                    }

                    fadeMotions.MotionInstanceIds[motionIndex] = instanceId;
                    fadeMotions.CubismFadeMotionObjects[motionIndex] = fadeMotion;
                }
                else
                {
                    var instanceId = newAnimationClip.GetInstanceID();
                    motionIndex = fadeMotions.MotionInstanceIds.Length;

                    Array.Resize(ref fadeMotions.MotionInstanceIds, motionIndex + 1);
                    fadeMotions.MotionInstanceIds[motionIndex] = instanceId;

                    Array.Resize(ref fadeMotions.CubismFadeMotionObjects, motionIndex + 1);
                    fadeMotions.CubismFadeMotionObjects[motionIndex] = fadeMotion;
                }

                for (var curveIndex = 0; curveIndex < motion3Json.Curves.Length; ++curveIndex)
                {
                    var curve = motion3Json.Curves[curveIndex];

                    if (curve.Target == "PartOpacity")
                    {
                        if(pose3Json.FadeInTime == 0.0f)
                        {
                            fadeMotion.ParameterIds[curveIndex] = curve.Id;
                            fadeMotion.ParameterFadeInTimes[curveIndex] = pose3Json.FadeInTime;
                            fadeMotion.ParameterFadeOutTimes[curveIndex] = (curve.FadeOutTime < 0.0f) ? -1.0f : curve.FadeOutTime;
                            fadeMotion.ParameterCurves[curveIndex] = new AnimationCurve(CubismMotion3Json.ConvertCurveSegmentsToKeyframes(curve.Segments));
                        }
                        else
                        {
                            fadeMotion.ParameterIds[curveIndex] = curve.Id;
                            fadeMotion.ParameterFadeInTimes[curveIndex] = pose3Json.FadeInTime;
                            fadeMotion.ParameterFadeOutTimes[curveIndex] = (curve.FadeOutTime < 0.0f) ? -1.0f : curve.FadeOutTime;
                            fadeMotion.ParameterCurves[curveIndex] = CubismMotion3Json.ConvertSteppedCurveToLinerCurver(curve, pose3Json.FadeInTime);
                        }
                    }
                }

                EditorUtility.SetDirty(fadeMotion);
            }

            InitializePosePart(model.Parts, pose3Json.Groups);
        }

        /// <summary>
        /// Initialize pose part.
        /// </summary>
        /// <param name="parts">Model parts.</param>
        /// <param name="groups">Pose groups.</param>
        private static void InitializePosePart(CubismPart[] parts, CubismPose3Json.SerializablePoseGroup[][] groups)
        {
            // Fail silently...
            if (parts == null || groups == null)
            {
                return;
            }

            for (var groupIndex = 0; groupIndex < groups.Length; ++groupIndex)
            {
                var group = groups[groupIndex];

                // Fail silently...
                if(group == null)
                {
                    continue;
                }

                for (var partIndex = 0; partIndex < group.Length; ++partIndex)
                {
                    var part = parts.FindById(group[partIndex].Id);

                    if(part == null)
                    {
                        continue;
                    }

                    var posePart = part.gameObject.GetComponent<CubismPosePart>();

                    if(posePart == null)
                    {
                        posePart = part.gameObject.AddComponent<CubismPosePart>();
                    }

                    posePart.GroupIndex = groupIndex;
                    posePart.PartIndex = partIndex;
                    posePart.Link = group[partIndex].Link;
                }
            }
         }

        #endregion
    }
}
