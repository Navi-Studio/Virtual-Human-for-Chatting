/**
 * Copyright(c) Live2D Inc. All rights reserved.
 *
 * Use of this source code is governed by the Live2D Open Software license
 * that can be found at https://www.live2d.com/eula/live2d-open-software-license-agreement_en.html.
 */


using System;
using System.Collections.Generic;
using Live2D.Cubism.Core;
using Live2D.Cubism.Framework.MouthMovement;
using Live2D.Cubism.Rendering;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;


namespace Live2D.Cubism.Framework.Json
{
    /// <summary>
    /// Contains Cubism motion3.json data.
    /// </summary>
    [Serializable]
    // ReSharper disable once ClassCannotBeInstantiated
    public sealed class CubismMotion3Json
    {
        #region Load Methods

        /// <summary>
        /// Loads a motion3.json asset.
        /// </summary>
        /// <param name="motion3Json">motion3.json to deserialize.</param>
        /// <returns>Deserialized motion3.json on success; <see langword="null"/> otherwise.</returns>
        public static CubismMotion3Json LoadFrom(string motion3Json)
        {
            if (string.IsNullOrEmpty(motion3Json))
            {
                return null;
            }

            var cubismMotion3Json = JsonUtility.FromJson<CubismMotion3Json>(motion3Json);

            cubismMotion3Json.Meta.FadeInTime = -1.0f;
            cubismMotion3Json.Meta.FadeOutTime = -1.0f;
            for (var i = 0; i < cubismMotion3Json.Curves.Length; ++i)
            {
                cubismMotion3Json.Curves[i].FadeInTime = -1.0f;
                cubismMotion3Json.Curves[i].FadeOutTime = -1.0f;
            }
            JsonUtility.FromJsonOverwrite(motion3Json, cubismMotion3Json);

            return cubismMotion3Json;
        }

        /// <summary>
        /// Loads a motion3.json asset.
        /// </summary>
        /// <param name="motion3JsonAsset">motion3.json to deserialize.</param>
        /// <returns>Deserialized motion3.json on success; <see langword="null"/> otherwise.</returns>
        public static CubismMotion3Json LoadFrom(TextAsset motion3JsonAsset)
        {
            return (motion3JsonAsset == null)
                ? null
                : LoadFrom(motion3JsonAsset.text);
        }

        #endregion

        #region Json Data

        /// <summary>
        /// The model3.json format version.
        /// </summary>
        [SerializeField]
        public int Version;

        /// <summary>
        /// Motion meta info.
        /// </summary>
        [SerializeField]
        public SerializableMeta Meta;

        /// <summary>
        /// Curves.
        /// </summary>
        [SerializeField]
        public SerializableCurve[] Curves;

        /// <summary>
        /// User data.
        /// </summary>
        [SerializeField]
        public SerializableUserData[] UserData;

        #endregion

        #region Constructors

        /// <summary>
        /// Makes construction only possible through factories.
        /// </summary>
        private CubismMotion3Json()
        {
        }

        #endregion

        /// <summary>
        /// Converts motion curve segments into <see cref="Keyframe"/>s.
        /// </summary>
        /// <param name="segments">Data to convert.</param>
        /// <returns>Keyframes.</returns>
        public static Keyframe[] ConvertCurveSegmentsToKeyframes(float[] segments)
        {
            // Return early on invalid input.
            if (segments.Length < 1)
            {
                return new Keyframe[0];
            }

            // Initialize container for keyframes.
            var keyframes = new List<Keyframe> { new Keyframe(segments[0], segments[1]) };


            // Parse segments.
            for (var i = 2; i < segments.Length;)
            {
                Parsers[segments[i]](segments, keyframes, ref i);
            }


            // Return result.
            return keyframes.ToArray();
        }

        /// <summary>
        /// Converts stepped curves to liner curves.
        /// </summary>
        /// <param name="curve">Data to convert.</param>
        /// <returns>Animation curve.</returns>
        public static AnimationCurve ConvertSteppedCurveToLinerCurver(CubismMotion3Json.SerializableCurve curve, float poseFadeInTime)
        {
            poseFadeInTime = (poseFadeInTime < 0) ? 0.5f : poseFadeInTime;

            var segments = curve.Segments;
            var segmentsCount = 2;

            for(var index = 2; index < curve.Segments.Length; index += 3)
            {
                // if current segment type is stepped and
                // next segment type is stepped or next segment is last segment
                // then convert segment type to liner.
                var currentSegmentTypeIsStepped = (curve.Segments[index] == 2);
                var currentSegmentIsLast = (index == (curve.Segments.Length - 3));
                var nextSegmentTypeIsStepped = (currentSegmentIsLast) ? false : (curve.Segments[index + 3] == 2);
                var nextSegmentIsLast = (currentSegmentIsLast) ? false : ((index + 3) == (curve.Segments.Length - 3));
                if ( currentSegmentTypeIsStepped && (nextSegmentTypeIsStepped || nextSegmentIsLast) )
                {
                    Array.Resize(ref segments, segments.Length + 3);
                    segments[segmentsCount + 0] = 0;
                    segments[segmentsCount + 1] = curve.Segments[index + 1];
                    segments[segmentsCount + 2] = curve.Segments[index - 1];
                    segments[segmentsCount + 3] = 0;
                    segments[segmentsCount + 4] = curve.Segments[index + 1] + poseFadeInTime;
                    segments[segmentsCount + 5] = curve.Segments[index + 2];
                    segmentsCount += 6;
                }
                else if(curve.Segments[index] == 1)
                {
                    segments[segmentsCount + 0] = curve.Segments[index + 0];
                    segments[segmentsCount + 1] = curve.Segments[index + 1];
                    segments[segmentsCount + 2] = curve.Segments[index + 2];
                    segments[segmentsCount + 3] = curve.Segments[index + 3];
                    segments[segmentsCount + 4] = curve.Segments[index + 4];
                    segments[segmentsCount + 5] = curve.Segments[index + 5];
                    segments[segmentsCount + 6] = curve.Segments[index + 6];
                    index += 4;
                    segmentsCount += 7;
                }
                else
                {
                    segments[segmentsCount + 0] = curve.Segments[index + 0];
                    segments[segmentsCount + 1] = curve.Segments[index + 1];
                    segments[segmentsCount + 2] = curve.Segments[index + 2];
                    segmentsCount += 3;
                }
            }

            return new AnimationCurve(ConvertCurveSegmentsToKeyframes(segments));
        }


        /// <summary>
        /// Instantiates an <see cref="AnimationClip"/>.
        /// </summary>
        /// <param name="shouldImportAsOriginalWorkflow">Should import as original workflow.</param>
        /// <param name="shouldClearAnimationCurves">Should clear animation clip curves.</param>
        /// <param name="isCallFormModelJson">Is function call form <see cref="CubismModel3Json"/>.</param>
        /// <param name="poseJson">pose3.json asset.</param>
        /// <returns>The instantiated clip on success; <see langword="null"/> otherwise.</returns>
        /// <remarks>
        /// Note this method generates <see cref="AnimationClip.legacy"/> clips when called at runtime.
        /// </remarks>
        public AnimationClip ToAnimationClip(bool shouldImportAsOriginalWorkflow = false, bool shouldClearAnimationCurves = false,
                                             bool isCallFormModelJson = false, CubismPose3Json poseJson = null)
        {
            // Check béziers restriction flag.
            if (!Meta.AreBeziersRestricted)
            {
                Debug.LogWarning("Béziers are not restricted and curves might be off. Please export motions from Cubism in restricted mode for perfect match.");
            }


            // Create animation clip.
            var animationClip = new AnimationClip
            {
#if UNITY_EDITOR
                frameRate = Meta.Fps
#else
                frameRate = Meta.Fps,
                legacy = true,
                wrapMode = (Meta.Loop)
                  ? WrapMode.Loop
                  : WrapMode.Default
#endif
            };

            return ToAnimationClip(animationClip, shouldImportAsOriginalWorkflow, shouldClearAnimationCurves, isCallFormModelJson, poseJson);
        }

        /// <summary>
        /// Instantiates an <see cref="AnimationClip"/>.
        /// </summary>
        /// <param name="animationClip">Previous animation clip.</param>
        /// <param name="shouldImportAsOriginalWorkflow">Should import as original workflow.</param>
        /// <param name="shouldClearAnimationCurves">Should clear animation clip curves.</param>
        /// <param name="isCallFormModelJson">Is function call form <see cref="CubismModel3Json"/>.</param>
        /// <param name="poseJson">pose3.json asset.</param>
        /// <returns>The instantiated clip on success; <see langword="null"/> otherwise.</returns>
        /// <remarks>
        /// Note this method generates <see cref="AnimationClip.legacy"/> clips when called at runtime.
        /// </remarks>
        public AnimationClip ToAnimationClip(AnimationClip animationClip, bool shouldImportAsOriginalWorkflow = false, bool shouldClearAnimationCurves = false
                                                                        , bool isCallFormModelJson = false, CubismPose3Json poseJson = null)
        {
            // Clear curves.
            if (shouldClearAnimationCurves && (!shouldImportAsOriginalWorkflow || (isCallFormModelJson && shouldImportAsOriginalWorkflow)))
            {
                animationClip.ClearCurves();
            }

            // Convert curves.
            for (var i = 0; i < Curves.Length; ++i)
            {
                var curve = Curves[i];

                // If should import as original workflow mode, skip add part opacity curve when call not from model3.json.
                if (curve.Target == "PartOpacity" && shouldImportAsOriginalWorkflow && !isCallFormModelJson)
                {
                    continue;
                }

                var relativePath = string.Empty;
                var type = default(Type);
                var propertyName = string.Empty;
                var animationCurve = new AnimationCurve(ConvertCurveSegmentsToKeyframes(curve.Segments));


                // Create model binding.
                if (curve.Target == "Model")
                {
                    // Bind opacity.
                    if (curve.Id == "Opacity")
                    {
                        relativePath = string.Empty;
                        propertyName = "Opacity";
                        type = typeof(CubismRenderController);
                    }

                    // Bind eye-blink.
                    else if (curve.Id == "EyeBlink")
                    {
                        relativePath = string.Empty;
                        propertyName = "EyeOpening";
                        type = typeof(CubismEyeBlinkController);
                    }

                    // Bind lip-sync.
                    else if (curve.Id == "LipSync")
                    {
                        relativePath = string.Empty;
                        propertyName = "MouthOpening";
                        type = typeof(CubismMouthController);
                    }
                }

                // Create parameter binding.
                else if (curve.Target == "Parameter")
                {
                    relativePath = "Parameters/" + curve.Id;
                    propertyName = "Value";
                    type = typeof(CubismParameter);
                }

                // Create part opacity binding.
                else if (curve.Target == "PartOpacity")
                {
                    relativePath = "Parts/" + curve.Id;
                    propertyName = "Opacity";
                    type = typeof(CubismPart);

                    // original workflow.
                    if (shouldImportAsOriginalWorkflow && poseJson != null && poseJson.FadeInTime != 0.0f)
                    {
                        animationCurve = ConvertSteppedCurveToLinerCurver(curve, poseJson.FadeInTime);
                    }
                }


#if UNITY_EDITOR
                var curveBinding = new EditorCurveBinding
                {
                    path = relativePath,
                    propertyName = propertyName,
                    type = type
                };


                AnimationUtility.SetEditorCurve(animationClip, curveBinding, animationCurve);
#else
                animationClip.SetCurve(relativePath, type, propertyName, animationCurve);
#endif
            }


#if UNITY_EDITOR
            // Apply settings.
            var animationClipSettings = new AnimationClipSettings
            {
                loopTime = Meta.Loop,
                stopTime = Meta.Duration
            };


            AnimationUtility.SetAnimationClipSettings(animationClip, animationClipSettings);
#endif


#if UNITY_EDITOR
            // Add animation events from user data.
            if (UserData != null)
            {
                var animationEvents = new List<AnimationEvent>();


                for (var i = 0; i < UserData.Length; ++i)
                {
                    var animationEvent = new AnimationEvent
                    {
                        time = UserData[i].Time,
                        stringParameter = UserData[i].Value,
                    };


                    animationEvents.Add(animationEvent);
                }


                if (animationEvents.Count > 0)
                {
                    AnimationUtility.SetAnimationEvents(animationClip, animationEvents.ToArray());
                }
            }
#endif

            return animationClip;
        }

        #region Segment Parsing

        /// <summary>
        /// Offset to use for setting of keyframes.
        /// </summary>
        private const float OffsetGranularity = 0.01f;

        /// <summary>
        /// Handles parsing of a single segment.
        /// </summary>
        /// <param name="segments">Curve segments.</param>
        /// <param name="result">Buffer to append result to.</param>
        /// <param name="position">Offset of segment.</param>
        private delegate void SegmentParser(float[] segments, List<Keyframe> result, ref int position);


        /// <summary>
        /// Available segment parsers.
        /// </summary>
        // ReSharper disable once InconsistentNaming
        private static Dictionary<float, SegmentParser> Parsers = new Dictionary<float, SegmentParser>
        {
            {0f, ParseLinearSegment},
            {1f, ParseBezierSegment},
            {2f, ParseSteppedSegment},
            {3f, ParseInverseSteppedSegment}
        };


        /// <summary>
        /// Parses a linear segment.
        /// </summary>
        /// <param name="segments">Curve segments.</param>
        /// <param name="result">Buffer to append result to.</param>
        /// <param name="position">Offset of segment.</param>
        private static void ParseLinearSegment(float[] segments, List<Keyframe> result, ref int position)
        {
            // Compute slope.
            var length = (segments[position + 1] - result[result.Count - 1].time);
            var slope = (segments[position + 2] - result[result.Count - 1].value) / length;


            // Determine tangents.
            var outTangent = slope;
            var inTangent = outTangent;


            // Create keyframes.
            var keyframe = new Keyframe(
                result[result.Count - 1].time,
                result[result.Count - 1].value,
                result[result.Count - 1].inTangent,
                outTangent);

            result[result.Count - 1] = keyframe;


            keyframe = new Keyframe(
                segments[position + 1],
                segments[position + 2],
                inTangent,
                0);

            result.Add(keyframe);


            // Update position.
            position += 3;
        }

        /// <summary>
        /// Parses a bezier segment.
        /// </summary>
        /// <param name="segments">Curve segments.</param>
        /// <param name="result">Buffer to append result to.</param>
        /// <param name="position">Offset of segment.</param>
        private static void ParseBezierSegment(float[] segments, List<Keyframe> result, ref int position)
        {
            // Compute tangents.
            var tangentLength = Mathf.Abs(result[result.Count - 1].time - segments[position + 5]) * 0.333333f;


            var outTangent = (segments[position + 2] - result[result.Count - 1].value) / tangentLength;
            var inTangent = (segments[position + 6] - segments[position + 4]) / tangentLength;


            // Create keyframes.
            var keyframe = new Keyframe(
                result[result.Count - 1].time,
                result[result.Count - 1].value,
                result[result.Count - 1].inTangent,
                outTangent);

            result[result.Count - 1] = keyframe;


            keyframe = new Keyframe(
                segments[position + 5],
                segments[position + 6],
                inTangent,
                0);

            result.Add(keyframe);


            // Update position.
            position += 7;
        }

        /// <summary>
        /// Parses a stepped segment.
        /// </summary>
        /// <param name="segments">Curve segments.</param>
        /// <param name="result">Buffer to append result to.</param>
        /// <param name="position">Offset of segment.</param>
        private static void ParseSteppedSegment(float[] segments, List<Keyframe> result, ref int position)
        {
            // Create keyframe.
            result.Add(
                new Keyframe(segments[position + 1], segments[position + 2])
                {
                    inTangent = float.PositiveInfinity
                });


            // Update position.
            position += 3;
        }

        /// <summary>
        /// Parses a inverse-stepped segment.
        /// </summary>
        /// <param name="segments">Curve segments.</param>
        /// <param name="result">Buffer to append result to.</param>
        /// <param name="position">Offset of segment.</param>
        private static void ParseInverseSteppedSegment(float[] segments, List<Keyframe> result, ref int position)
        {
            // Compute tangents.
            var keyframe = result[result.Count - 1];

            var tangent = (float)Math.Atan2(
                (segments[position + 2] - keyframe.value),
                (segments[position + 1] - keyframe.time));


            keyframe.outTangent = tangent;
            result[result.Count - 1] = keyframe;


            result.Add(
                new Keyframe(keyframe.time + OffsetGranularity, segments[position + 2])
                {
                    inTangent = tangent,
                    outTangent = 0
                });

            result.Add(
                new Keyframe(segments[position + 1], segments[position + 2])
                {
                    inTangent = 0
                });


            // Update position.
            position += 3;
        }

        #endregion

        #region Json Object Types

        /// <summary>
        /// Motion meta info.
        /// </summary>
        [Serializable]
        public struct SerializableMeta
        {
            /// <summary>
            /// Duration in seconds.
            /// </summary>
            [SerializeField]
            public float Duration;

            /// <summary>
            /// Framerate in seconds.
            /// </summary>
            [SerializeField]
            public float Fps;

            /// <summary>
            /// True if motion is looping.
            /// </summary>
            [SerializeField]
            public bool Loop;

            /// <summary>
            /// Number of curves.
            /// </summary>
            [SerializeField]
            public int CurveCount;

            /// <summary>
            /// Total number of curve segments.
            /// </summary>
            [SerializeField]
            public int TotalSegmentCount;

            /// <summary>
            /// Total number of curve points.
            /// </summary>
            [SerializeField]
            public int TotalPointCount;

            /// <summary>
            /// True if beziers are restricted.
            /// </summary>
            [SerializeField]
            public bool AreBeziersRestricted;

            /// <summary>
            /// Total number of UserData.
            /// </summary>
            [SerializeField]
            public int UserDataCount;

            /// <summary>
            /// Total size of UserData in bytes.
            /// </summary>
            [SerializeField]
            public int TotalUserDataSize;

            /// <summary>
            /// [Optional] Time of the Fade-In for easing in seconds.
            /// </summary>
            [SerializeField]
            public float FadeInTime;

            /// <summary>
            /// [Optional] Time of the Fade-Out for easing in seconds.
            /// </summary>
            [SerializeField]
            public float FadeOutTime;
        };

        /// <summary>
        /// Single motion curve.
        /// </summary>
        [Serializable]
        public struct SerializableCurve
        {
            /// <summary>
            /// Target type.
            /// </summary>
            [SerializeField]
            public string Target;

            /// <summary>
            /// Id within target.
            /// </summary>
            [SerializeField]
            public string Id;

            /// <summary>
            /// Flattened curve segments.
            /// </summary>
            [SerializeField]
            public float[] Segments;

            /// <summary>
            /// [Optional] Time of the overall Fade-In for easing in seconds.
            /// </summary>
            [SerializeField]
            public float FadeInTime;

            /// <summary>
            /// [Optional] Time of the overall Fade-Out for easing in seconds.
            /// </summary>
            [SerializeField]
            public float FadeOutTime;
        };

        /// <summary>
        /// User data.
        /// </summary>
        [Serializable]
        public struct SerializableUserData
        {
            /// <summary>
            /// Time in seconds.
            /// </summary>
            [SerializeField]
            public float Time;

            /// <summary>
            /// Content of user data.
            /// </summary>
            [SerializeField]
            public string Value;
        }

        #endregion
    }
}
