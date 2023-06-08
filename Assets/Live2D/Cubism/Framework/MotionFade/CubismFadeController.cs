/**
 * Copyright(c) Live2D Inc. All rights reserved.
 *
 * Use of this source code is governed by the Live2D Open Software license
 * that can be found at https://www.live2d.com/eula/live2d-open-software-license-agreement_en.html.
 */


using Live2D.Cubism.Core;
using Live2D.Cubism.Framework.Motion;
using UnityEngine;


namespace Live2D.Cubism.Framework.MotionFade
{
    /// <summary>
    /// Cubism fade controller.
    /// </summary>
    [RequireComponent(typeof(Animator))]
    public class CubismFadeController : MonoBehaviour, ICubismUpdatable
    {
        #region Variable

        /// <summary>
        /// Cubism fade motion list.
        /// </summary>
        [SerializeField]
        public CubismFadeMotionList CubismFadeMotionList;

        /// <summary>
        /// Parameters cache.
        /// </summary>
        private CubismParameter[] DestinationParameters { get; set; }

        /// <summary>
        /// Parts cache.
        /// </summary>
        private CubismPart[] DestinationParts { get; set; }

        /// <summary>
        /// Model has motion controller component.
        /// </summary>
        private CubismMotionController _motionController;

        /// <summary>
        /// Model has cubism update controller component.
        /// </summary>
        [HideInInspector]
        public bool HasUpdateController { get; set; }

        /// <summary>
        /// Fade state machine behavior set in the animator.
        /// </summary>
        private ICubismFadeState[] _fadeStates;

        /// <summary>
        /// Model has animator component.
        /// </summary>
        private Animator _animator;

        /// <summary>
        /// Restore parameter value.
        /// </summary>
        private CubismParameterStore _parameterStore;

        /// <summary>
        /// Fading flags for each layer.
        /// </summary>
        private bool[] _isFading;

        #endregion

        #region Function

        /// <summary>
        /// Refreshes the controller. Call this method after adding and/or removing <see cref="CubismFadeParameter"/>s.
        /// </summary>
        public void Refresh()
        {
            _animator = GetComponent<Animator>();

            // Fail silently...
            if (_animator == null)
            {
                return;
            }

            DestinationParameters = this.FindCubismModel().Parameters;
            DestinationParts = this.FindCubismModel().Parts;
            _motionController = GetComponent<CubismMotionController>();
            _parameterStore = GetComponent<CubismParameterStore>();

            // Get cubism update controller.
            HasUpdateController = (GetComponent<CubismUpdateController>() != null);

            _fadeStates = (ICubismFadeState[])_animator.GetBehaviours<CubismFadeStateObserver>();

            if ((_fadeStates == null || _fadeStates.Length == 0) && _motionController != null)
            {
                _fadeStates = _motionController.GetFadeStates();
            }

            if (_fadeStates == null)
            {
                return;
            }
            _isFading = new bool[_fadeStates.Length];
        }

        /// <summary>
        /// Called by cubism update controller. Order to invoke OnLateUpdate.
        /// </summary>
        public int ExecutionOrder
        {
            get { return CubismUpdateExecutionOrder.CubismFadeController; }
        }

        /// <summary>
        /// Called by cubism update controller. Needs to invoke OnLateUpdate on Editing.
        /// </summary>
        public bool NeedsUpdateOnEditing
        {
            get { return false; }
        }

        /// <summary>
        /// Called by cubism update controller. Updates controller.
        /// </summary>
        /// <remarks>
        /// Make sure this method is called after any animations are evaluated.
        /// </remarks>
        public void OnLateUpdate()
        {
            // Fail silently.
            if (!enabled || _fadeStates == null || _parameterStore == null
               || DestinationParameters == null || DestinationParts == null)
            {
                return;
            }

            var time = Time.time;
            for (var i = 0; i < _fadeStates.Length; ++i)
            {
                _isFading[i] = false;

                var playingMotions = _fadeStates[i].GetPlayingMotions();
                if (playingMotions == null || playingMotions.Count <= 1)
                {
                    continue;
                }

                var latestPlayingMotion = playingMotions[playingMotions.Count - 1];

                var playingMotionData = latestPlayingMotion.Motion;
                var elapsedTime = time - latestPlayingMotion.StartTime;
                for (var j = 0; j < playingMotionData.ParameterFadeInTimes.Length; j++)
                {
                    if ((elapsedTime <= playingMotionData.FadeInTime) ||
                        ((0 <= playingMotionData.ParameterFadeInTimes[j]) &&
                          (elapsedTime <= playingMotionData.ParameterFadeInTimes[j])))
                    {
                        _isFading[i] = true;
                        break;
                    }
                }
            }

            var isFadingAllFinished = true;
            for (var i = 0; i < _fadeStates.Length; ++i)
            {
                var playingMotions = _fadeStates[i].GetPlayingMotions();
                var playingMotionCount = playingMotions.Count - 1;
                if (_isFading[i])
                {
                    isFadingAllFinished = false;
                    continue;
                }
                for (var j = playingMotionCount; j >= 0; --j)
                {
                    if (playingMotions.Count <= 1)
                    {
                        break;
                    }

                    var playingMotion = playingMotions[j];
                    if (time <= playingMotion.EndTime)
                    {
                        continue;
                    }

                    // If fade-in has been completed, delete the motion that has been played back.
                    _fadeStates[i].StopAnimation(j);
                }
            }

            if (isFadingAllFinished)
            {
                return;
            }

            _parameterStore.RestoreParameters();


            // Update sources and destinations.
            for (var i = 0; i < _fadeStates.Length; ++i)
            {
                if (!_isFading[i])
                {
                    continue;
                }
                UpdateFade(_fadeStates[i]);
            }
        }

        /// <summary>
        /// Update motion fade.
        /// </summary>
        /// <param name="fadeState">Fade state observer.</param>
        private void UpdateFade(ICubismFadeState fadeState)
        {
            var playingMotions = fadeState.GetPlayingMotions();

            if (playingMotions == null)
            {
                // Do not process if there is only one motion, if it does not switch.
                return;
            }

            // Weight set for the layer being processed.
            // (In the case of the layer located at the top, it is forced to 1.)
            var layerWeight = fadeState.GetLayerWeight();

            var time = Time.time;

            // Set playing motions end time.
            if ((playingMotions.Count > 0) && (playingMotions[playingMotions.Count - 1].Motion != null) && (playingMotions[playingMotions.Count - 1].IsLooping))
            {
                var motion = playingMotions[playingMotions.Count - 1];

                var newEndTime = time + motion.Motion.FadeOutTime;

                motion.EndTime = newEndTime;


                while (true)
                {
                    if ((motion.StartTime + motion.Motion.MotionLength) >= time)
                    {
                        break;
                    }

                    motion.StartTime += motion.Motion.MotionLength;
                }


                playingMotions[playingMotions.Count - 1] = motion;
            }


            // Calculate MotionFade.
            for (var i = 0; i < playingMotions.Count; i++)
            {
                var playingMotion = playingMotions[i];

                var fadeMotion = playingMotion.Motion;
                if (fadeMotion == null)
                {
                    continue;
                }

                var elapsedTime = time - playingMotion.StartTime;
                var endTime = playingMotion.EndTime - elapsedTime;

                var fadeInTime = fadeMotion.FadeInTime;
                var fadeOutTime = fadeMotion.FadeOutTime;


                var fadeInWeight = (fadeInTime <= 0.0f)
                    ? 1.0f
                    : CubismFadeMath.GetEasingSine(elapsedTime / fadeInTime);
                var fadeOutWeight = (fadeOutTime <= 0.0f)
                    ? 1.0f
                    : CubismFadeMath.GetEasingSine((playingMotion.EndTime - Time.time) / fadeOutTime);


                playingMotions[i] = playingMotion;

                var motionWeight = (i == 0)
                    ? (fadeInWeight * fadeOutWeight)
                    : (fadeInWeight * fadeOutWeight * layerWeight);

                // Apply to parameter values
                for (var j = 0; j < DestinationParameters.Length; ++j)
                {
                    var index = -1;
                    for (var k = 0; k < fadeMotion.ParameterIds.Length; ++k)
                    {
                        if (fadeMotion.ParameterIds[k] != DestinationParameters[j].Id)
                        {
                            continue;
                        }

                        index = k;
                        break;
                    }

                    if (index < 0)
                    {
                        // There is not target ID curve in motion.
                        continue;
                    }

                    DestinationParameters[j].Value = Evaluate(
                            fadeMotion.ParameterCurves[index], elapsedTime, endTime,
                            fadeInWeight, fadeOutWeight,
                            fadeMotion.ParameterFadeInTimes[index], fadeMotion.ParameterFadeOutTimes[index],
                            motionWeight, DestinationParameters[j].Value);
                }

                // Apply to part opacities
                for (var j = 0; j < DestinationParts.Length; ++j)
                {
                    var index = -1;
                    for (var k = 0; k < fadeMotion.ParameterIds.Length; ++k)
                    {
                        if (fadeMotion.ParameterIds[k] != DestinationParts[j].Id)
                        {
                            continue;
                        }

                        index = k;
                        break;
                    }

                    if (index < 0)
                    {
                        // There is not target ID curve in motion.
                        continue;
                    }

                    DestinationParts[j].Opacity = Evaluate(
                            fadeMotion.ParameterCurves[index], elapsedTime, endTime,
                            fadeInWeight, fadeOutWeight,
                            fadeMotion.ParameterFadeInTimes[index], fadeMotion.ParameterFadeOutTimes[index],
                            motionWeight, DestinationParts[j].Opacity);
                }

            }
        }

        /// <summary>
        /// Evaluate fade curve.
        /// </summary>
        /// <param name="curve">Curves to be evaluated.</param>
        /// <param name="elapsedTime">Elapsed Time.</param>
        /// <param name="endTime">Fading end time.</param>
        /// <param name="fadeInTime">Fade in time.</param>
        /// <param name="fadeOutTime">Fade out time.</param>
        /// <param name="parameterFadeInTime">Fade in time parameter.</param>
        /// <param name="parameterFadeOutTime">Fade out time parameter.</param>
        /// <param name="motionWeight">Motion weight.</param>
        /// <param name="currentValue">Current value with weight applied.</param>
        public float Evaluate(
            AnimationCurve curve, float elapsedTime, float endTime,
            float fadeInTime, float fadeOutTime,
            float parameterFadeInTime, float parameterFadeOutTime,
            float motionWeight, float currentValue)
        {
            if (curve.length <= 0)
            {
                return currentValue;
            }

            // Motion fade.
            if (parameterFadeInTime < 0.0f &&
                parameterFadeOutTime < 0.0f)
            {
                return currentValue + (curve.Evaluate(elapsedTime) - currentValue) * motionWeight;
            }

            // Parameter fade.
            float fadeInWeight, fadeOutWeight;
            if (parameterFadeInTime < 0.0f)
            {
                fadeInWeight = fadeInTime;
            }
            else
            {
                fadeInWeight = (parameterFadeInTime < float.Epsilon)
                    ? 1.0f
                    : CubismFadeMath.GetEasingSine(elapsedTime / parameterFadeInTime);
            }

            if (parameterFadeOutTime < 0.0f)
            {
                fadeOutWeight = fadeOutTime;
            }
            else
            {
                fadeOutWeight = (parameterFadeOutTime < float.Epsilon)
                    ? 1.0f
                    : CubismFadeMath.GetEasingSine(endTime / parameterFadeOutTime);
            }

            var parameterWeight = fadeInWeight * fadeOutWeight;

            return currentValue + (curve.Evaluate(elapsedTime) - currentValue) * parameterWeight;
        }

        #endregion

        #region Unity Events Handling

        /// <summary>
        /// Initializes instance.
        /// </summary>
        private void OnEnable()
        {
            // Initialize cache.
            Refresh();
        }

        /// <summary>
        /// Called by Unity.
        /// </summary>
        private void LateUpdate()
        {
            if (!HasUpdateController)
            {
                OnLateUpdate();
            }
        }

        #endregion
    }
}
