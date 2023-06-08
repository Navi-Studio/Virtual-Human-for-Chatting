/**
 * Copyright(c) Live2D Inc. All rights reserved.
 *
 * Use of this source code is governed by the Live2D Open Software license
 * that can be found at https://www.live2d.com/eula/live2d-open-software-license-agreement_en.html.
 */

using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations;


namespace Live2D.Cubism.Framework.MotionFade
{
    public class CubismFadeStateObserver : StateMachineBehaviour, ICubismFadeState
    {
        #region variable

        /// <summary>
        /// Cubism fade motion list.
        /// </summary>
        private CubismFadeMotionList _cubismFadeMotionList;

        /// <summary>
        /// Cubism playing motion list.
        /// </summary>
        private List<CubismFadePlayingMotion> _playingMotions;

        /// <summary>
        /// State that attached this is default.
        /// </summary>
        private bool _isDefaulState;

        /// <summary>
        /// Layer index that attached this.
        /// </summary>
        private int _layerIndex;

        /// <summary>
        /// Weight of layer that attached this.
        /// </summary>
        private float _layerWeight;

        /// <summary>
        /// State that attached this is transition finished.
        /// </summary>
        private bool _isStateTransitionFinished;

        #endregion


        #region Fade State Interface

        /// <summary>
        /// Get cubism playing motion list.
        /// </summary>
        /// <returns>Cubism playing motion list.</returns>
        public List<CubismFadePlayingMotion> GetPlayingMotions()
        {
            return _playingMotions;
        }

        /// <summary>
        /// Is default state.
        /// </summary>
        /// <returns><see langword="true"/> State is default; <see langword="false"/> otherwise.</returns>
        public bool IsDefaultState()
        {
            return _isDefaulState;
        }

        /// <summary>
        /// Get layer weight.
        /// </summary>
        /// <returns>Layer weight.</returns>
        public float GetLayerWeight()
        {
            return _layerWeight;
        }

        /// <summary>
        /// Get state transition finished.
        /// </summary>
        /// <returns><see langword="true"/> State transition is finished; <see langword="false"/> otherwise.</returns>
        public bool GetStateTransitionFinished()
        {
            return _isStateTransitionFinished;
        }

        /// <summary>
        /// Set state transition finished.
        /// </summary>
        /// <param name="isFinished">State is finished.</param>
        public void SetStateTransitionFinished(bool isFinished)
        {
            _isStateTransitionFinished = isFinished;
        }

        /// <summary>
        /// Stop animation.
        /// </summary>
        /// <param name="index">Playing motion index.</param>
        public void StopAnimation(int index)
        {
            _playingMotions.RemoveAt(index);
        }

        #endregion


        #region Unity Event Handling

        /// <summary>
        /// Called by Unity.
        /// </summary>
        private void OnEnable()
        {
            _isStateTransitionFinished = false;

            if (_playingMotions == null)
            {
                _playingMotions = new List<CubismFadePlayingMotion>();
            }
        }

        /// <summary>
        /// Called by Unity.
        /// </summary>
        /// <param name="animator">Animator.</param>
        /// <param name="stateInfo">Animator state info.</param>
        /// <param name="layerIndex">Index of the layer.</param>
        /// <param name="controller">Animation controller playable.</param>
        public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex, AnimatorControllerPlayable controller)
        {
            var fadeController = animator.gameObject.GetComponent<CubismFadeController>();

            // Fail silently...
            if (fadeController == null)
            {
                return;
            }

            _cubismFadeMotionList = fadeController.CubismFadeMotionList;

            _layerIndex = layerIndex;
            _layerWeight = (_layerIndex == 0)
                ? 1.0f
                : animator.GetLayerWeight(_layerIndex);

            var animatorClipInfo = controller.GetNextAnimatorClipInfo(layerIndex);

            _isDefaulState = (animatorClipInfo.Length == 0);

            if (_isDefaulState)
            {
                // Get the motion of Default State only for the first time.
                animatorClipInfo = controller.GetCurrentAnimatorClipInfo(layerIndex);
            }

            // Set playing motions end time.
            if ((_playingMotions.Count > 0) && (_playingMotions[_playingMotions.Count - 1].Motion != null))
            {
                var motion = _playingMotions[_playingMotions.Count - 1];

                var time = Time.time;

                var newEndTime = time + motion.Motion.FadeOutTime;

                motion.EndTime = newEndTime;


                while (motion.IsLooping)
                {
                    if ((motion.StartTime + motion.Motion.MotionLength) >= time)
                    {
                        break;
                    }

                    motion.StartTime += motion.Motion.MotionLength;
                }


                _playingMotions[_playingMotions.Count - 1] = motion;
            }

            for (var i = 0; i < animatorClipInfo.Length; ++i)
            {
                CubismFadePlayingMotion playingMotion;


                var instanceId = -1;
                var events = animatorClipInfo[i].clip.events;
                for(var k = 0; k < events.Length; ++k)
                {
                    if(events[k].functionName != "InstanceId")
                    {
                        continue;
                    }

                    instanceId = events[k].intParameter;
                    break;
                }

                var motionIndex = -1;
                for (var j = 0; j < _cubismFadeMotionList.MotionInstanceIds.Length; ++j)
                {
                    if (_cubismFadeMotionList.MotionInstanceIds[j] != instanceId)
                    {
                        continue;
                    }

                    motionIndex = j;
                    break;
                }

                playingMotion.Motion = (motionIndex == -1)
                    ? null
                    : _cubismFadeMotionList.CubismFadeMotionObjects[motionIndex];

                playingMotion.Speed = 1.0f;
                playingMotion.StartTime = Time.time;
                playingMotion.FadeInStartTime = Time.time;
                playingMotion.EndTime = (playingMotion.Motion.MotionLength <= 0)
                                        ? -1
                                        : playingMotion.StartTime + playingMotion.Motion.MotionLength;
                playingMotion.IsLooping = animatorClipInfo[i].clip.isLooping;
                playingMotion.Weight = 0.0f;

                _playingMotions.Add(playingMotion);
            }
        }

        /// <summary>
        /// Called by Unity.
        /// </summary>
        /// <param name="animator">Animator.</param>
        /// <param name="stateInfo">Animator state info.</param>
        /// <param name="layerIndex">Index of the layer.</param>
        public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            _isStateTransitionFinished = true;
        }

        #endregion
    }
}
