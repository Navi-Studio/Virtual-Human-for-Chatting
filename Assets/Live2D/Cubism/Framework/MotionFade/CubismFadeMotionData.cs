/**
 * Copyright(c) Live2D Inc. All rights reserved.
 *
 * Use of this source code is governed by the Live2D Open Software license
 * that can be found at https://www.live2d.com/eula/live2d-open-software-license-agreement_en.html.
 */


using Live2D.Cubism.Framework.Json;
using UnityEngine;


namespace Live2D.Cubism.Framework.MotionFade
{
    public class CubismFadeMotionData : ScriptableObject
    {
        /// <summary>
        /// Name of motion.
        /// </summary>
        [SerializeField]
        public string MotionName;

        /// <summary>
        /// Time to fade in.
        /// </summary>
        [SerializeField]
        public float FadeInTime;

        /// <summary>
        /// Time to fade out.
        /// </summary>
        [SerializeField]
        public float FadeOutTime;

        /// <summary>
        /// Parameter ids.
        /// </summary>
        [SerializeField]
        public string[] ParameterIds;

        /// <summary>
        /// Parameter curves.
        /// </summary>
        [SerializeField]
        public AnimationCurve[] ParameterCurves;

        /// <summary>
        /// Fade in time parameters.
        /// </summary>
        [SerializeField]
        public float[] ParameterFadeInTimes;

        /// <summary>
        /// Fade out time parameters.
        /// </summary>
        [SerializeField]
        public float[] ParameterFadeOutTimes;

        /// <summary>
        /// Motion length.
        /// </summary>
        [SerializeField]
        public float MotionLength;


        /// <summary>
        /// Create CubismFadeMotionData from CubismMotion3Json.
        /// </summary>
        /// <param name="motion3Json">Motion3json as the creator.</param>
        /// <param name="motionName">Motion name of interest.</param>
        /// <param name="motionLength">Length of target motion.</param>
        /// <param name="shouldImportAsOriginalWorkflow">Whether the original work flow or not.</param>
        /// <param name="isCallFromModelJson">Whether it is a call from the model json.</param>
        /// <returns>Fade data created based on motion3json.</returns>
        public static CubismFadeMotionData CreateInstance(
            CubismMotion3Json motion3Json, string motionName, float motionLength,
             bool shouldImportAsOriginalWorkflow = false, bool isCallFromModelJson = false)
        {
            var fadeMotion = CreateInstance<CubismFadeMotionData>();
            var curveCount = motion3Json.Curves.Length;
            fadeMotion.ParameterIds = new string[curveCount];
            fadeMotion.ParameterFadeInTimes = new float[curveCount];
            fadeMotion.ParameterFadeOutTimes = new float[curveCount];
            fadeMotion.ParameterCurves = new AnimationCurve[curveCount];

            return CreateInstance(fadeMotion, motion3Json, motionName, motionLength, shouldImportAsOriginalWorkflow, isCallFromModelJson);
        }

        /// <summary>
        /// Put motion3json's fade information back into fade motion data.
        /// </summary>
        /// <param name="fadeMotion">Instance containing fade information.</param>
        /// <param name="motion3Json">Target motion3json.</param>
        /// <param name="motionName">Motion name of interest.</param>
        /// <param name="motionLength">Motion length.</param>
        /// <param name="shouldImportAsOriginalWorkflow">Whether the original work flow or not.</param>
        /// <param name="isCallFormModelJson">Whether it is a call from the model json.</param>
        /// <returns>Fade data created based on fademotiondata.</returns>
        public static CubismFadeMotionData CreateInstance(
            CubismFadeMotionData fadeMotion, CubismMotion3Json motion3Json, string motionName, float motionLength,
             bool shouldImportAsOriginalWorkflow = false, bool isCallFormModelJson = false)
        {
            fadeMotion.MotionName = motionName;
            fadeMotion.MotionLength = motionLength;
            fadeMotion.FadeInTime = (motion3Json.Meta.FadeInTime < 0.0f) ? 1.0f : motion3Json.Meta.FadeInTime;
            fadeMotion.FadeOutTime = (motion3Json.Meta.FadeOutTime < 0.0f) ? 1.0f : motion3Json.Meta.FadeOutTime;

            for (var i = 0; i < motion3Json.Curves.Length; ++i)
            {
                var curve = motion3Json.Curves[i];

                // In original workflow mode, skip add part opacity curve when call not from model3.json.
                if (curve.Target == "PartOpacity" && shouldImportAsOriginalWorkflow && !isCallFormModelJson)
                {
                    continue;
                }

                fadeMotion.ParameterIds[i] = curve.Id;
                fadeMotion.ParameterFadeInTimes[i] = (curve.FadeInTime < 0.0f) ? -1.0f : curve.FadeInTime;
                fadeMotion.ParameterFadeOutTimes[i] = (curve.FadeOutTime < 0.0f) ? -1.0f : curve.FadeOutTime;
                fadeMotion.ParameterCurves[i] = new AnimationCurve(CubismMotion3Json.ConvertCurveSegmentsToKeyframes(curve.Segments));
            }

            return fadeMotion;
        }
    }
}
