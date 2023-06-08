/**
 * Copyright(c) Live2D Inc. All rights reserved.
 *
 * Use of this source code is governed by the Live2D Open Software license
 * that can be found at https://www.live2d.com/eula/live2d-open-software-license-agreement_en.html.
 */


using Live2D.Cubism.Core;
using Live2D.Cubism.Framework.MotionFade;
using System;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.Playables;


namespace Live2D.Cubism.Framework.Motion
{
    /// <summary>
    /// Cubism motion controller.
    /// </summary>
    [RequireComponent(typeof(CubismFadeController))]
    public class CubismMotionController : MonoBehaviour
    {
        #region Action

        /// <summary>
        /// Action animation end handler.
        /// </summary>
        [SerializeField]
        public Action<float> AnimationEndHandler;

        /// <summary>
        /// Action OnAnimationEnd.
        /// </summary>
        private void OnAnimationEnd(int layerIndex, float instanceId)
        {
            _motionPriorities[layerIndex] = CubismMotionPriority.PriorityNone;

            if (AnimationEndHandler != null)
            {
                AnimationEndHandler(instanceId);
            }
        }

        #endregion

        #region Variable

        /// <summary>
        /// Layer count.
        /// </summary>
        public int LayerCount = 1;

        /// <summary>
        /// List of cubism fade motion.
        /// </summary>
        private CubismFadeMotionList _cubismFadeMotionList;

        /// <summary>
        /// Motion controller is active.
        /// </summary>
        private bool _isActive = false;

        /// <summary>
        /// Playable graph controller.
        /// </summary>
        private PlayableGraph _playableGrap;

        /// <summary>
        /// Playable output.
        /// </summary>
        private AnimationPlayableOutput _playableOutput;

        /// <summary>
        /// Animation layer mixer.
        /// </summary>
        private AnimationLayerMixerPlayable _layerMixer;

        /// <summary>
        /// Cubism motion layers.
        /// </summary>
        private CubismMotionLayer[] _motionLayers;

        /// <summary>
        /// Cubism motion priorities.
        /// </summary>
        private int[] _motionPriorities;

        #endregion Variable

        #region Function

        /// <summary>
        /// Play animations.
        /// </summary>
        /// <param name="clip">Animator clip.</param>
        /// <param name="layerIndex">layer index.</param>
        /// <param name="priority">Animation priority</param>
        /// <param name="isLoop">Animation is loop.</param>
        /// <param name="speed">Animation speed.</param>
        public void PlayAnimation(AnimationClip clip, int layerIndex = 0, int priority = CubismMotionPriority.PriorityNormal, bool isLoop = true, float speed = 1.0f)
        {
            // Fail silently...
            if(!enabled || !_isActive || _cubismFadeMotionList == null || clip == null
               || layerIndex < 0 || layerIndex >= LayerCount ||
               ((_motionPriorities[layerIndex] >= priority) && (priority != CubismMotionPriority.PriorityForce)))
            {
                Debug.Log("can't start motion.");
                return;
            }

            _motionPriorities[layerIndex] = priority;
            _motionLayers[layerIndex].PlayAnimation(clip, isLoop, speed);

            // Play Playable Graph
            if(!_playableGrap.IsPlaying())
            {
                _playableGrap.Play();
            }
        }

        /// <summary>
        /// Stop animation.
        /// </summary>
        /// <param name="animationIndex">Animator index.</param>
        /// <param name="layerIndex">layer index.</param>
        public void StopAnimation(int animationIndex, int layerIndex = 0)
        {
            // Fail silently...
            if(layerIndex < 0 || layerIndex >= LayerCount)
            {
                return;
            }

            _motionLayers[layerIndex].StopAnimationClip();
        }

        /// <summary>
        /// Stop all animation.
        /// </summary>
        public void StopAllAnimation()
        {
            for(var i = 0; i < LayerCount; ++i)
            {
                _motionLayers[i].StopAnimationClip();
            }
        }

        /// <summary>
        /// Is playing animation.
        /// </summary>
        /// <returns>True if the animation is playing.</returns>
        public bool IsPlayingAnimation(int layerIndex = 0)
        {
            // Fail silently...
            if(layerIndex < 0 || layerIndex >= LayerCount)
            {
                return false;
            }

            return !_motionLayers[layerIndex].IsFinished;
        }

        /// <summary>
        /// Set layer weight.
        /// </summary>
        /// <param name="layerIndex">layer index.</param>
        /// <param name="weight">Layer weight.</param>
        public void SetLayerWeight(int layerIndex, float weight)
        {
            // Fail silently...
            if(layerIndex <= 0 || layerIndex >= LayerCount)
            {
                return;
            }

            _motionLayers[layerIndex].SetLayerWeight(weight);
            _layerMixer.SetInputWeight(layerIndex, weight);
        }

        /// <summary>
        /// Set layer blend type is additive.
        /// </summary>
        /// <param name="layerIndex">layer index.</param>
        /// <param name="isAdditive">Blend type is additive.</param>
        public void SetLayerAdditive(int layerIndex, bool isAdditive)
        {
            // Fail silently...
            if(layerIndex <= 0 || layerIndex >= LayerCount)
            {
                return;
            }

            _layerMixer.SetLayerAdditive((uint)layerIndex, isAdditive);
        }

        /// <summary>
        /// Set animation speed.
        /// </summary>
        /// <param name="layerIndex">layer index.</param>
        /// <param name="index">index of playing motion list.</param>
        /// <param name="speed">Animation speed.</param>
        public void SetAnimationSpeed(int layerIndex, int index, float speed)
        {
            // Fail silently...
            if(layerIndex < 0 || layerIndex >= LayerCount)
            {
                return;
            }

            _motionLayers[layerIndex].SetStateSpeed(index, speed);
        }

        /// <summary>
        /// Set animation is loop.
        /// </summary>
        /// <param name="layerIndex">layer index.</param>
        /// <param name="index">Index of playing motion list.</param>
        /// <param name="isLoop">State is loop.</param>
        public void SetAnimationIsLoop(int layerIndex, int index, bool isLoop)
        {
            // Fail silently...
            if(layerIndex < 0 || layerIndex >= LayerCount)
            {
                return;
            }

            _motionLayers[layerIndex].SetStateIsLoop(index, isLoop);
        }

        /// <summary>
        /// Get cubism fade states.
        /// </summary>
        public ICubismFadeState[] GetFadeStates()
        {
            if(_motionLayers == null)
            {
                LayerCount = (LayerCount < 1) ? 1 : LayerCount;
                _motionLayers = new CubismMotionLayer[LayerCount];
                _motionPriorities = new int[LayerCount];
            }

            return _motionLayers;
        }

        #endregion Function

        #region Unity Events Handling

        /// <summary>
        /// Called by Unity.
        /// </summary>
        private void OnEnable()
        {
            _cubismFadeMotionList = GetComponent<CubismFadeController>().CubismFadeMotionList;

            // Fail silently...
            if(_cubismFadeMotionList == null)
            {
                Debug.LogError("CubismMotionController : CubismFadeMotionList doesn't set in CubismFadeController.");
                return;
            }

            // Get Animator.
            var animator = GetComponent<Animator>();

            if (animator.runtimeAnimatorController != null)
            {
                Debug.LogWarning("Animator Controller was set in Animator component.");
                return;
            }

            _isActive = true;

            // Disable animator's playablegrap.
            var graph = animator.playableGraph;

            if(graph.IsValid())
            {
                graph.GetOutput(0).SetWeight(0);
            }

            // Create Playable Graph.
#if UNITY_2018_1_OR_NEWER
            _playableGrap = PlayableGraph.Create("Playable Graph : " + this.FindCubismModel().name);
#else
            _playableGrap = PlayableGraph.Create();
#endif
            _playableGrap.SetTimeUpdateMode(DirectorUpdateMode.GameTime);

            // Create Playable Output.
            _playableOutput = AnimationPlayableOutput.Create(_playableGrap, "Animation", animator);
            _playableOutput.SetWeight(1);

            // Create animation layer mixer.
            _layerMixer = AnimationLayerMixerPlayable.Create(_playableGrap, LayerCount);

            // Create cubism motion layers.
            if(_motionLayers == null)
            {
                LayerCount = (LayerCount < 1) ? 1 : LayerCount;
                _motionLayers = new CubismMotionLayer[LayerCount];
                _motionPriorities = new int[LayerCount];
            }

            for(var i = 0; i < LayerCount; ++i)
            {
                _motionLayers[i] = CubismMotionLayer.CreateCubismMotionLayer(_playableGrap, _cubismFadeMotionList, i);
                _motionLayers[i].AnimationEndHandler += OnAnimationEnd;
                _layerMixer.ConnectInput(i, _motionLayers[i].PlayableOutput, 0);
                _layerMixer.SetInputWeight(i, 1.0f);
            }

            // Set Playable Output.
            _playableOutput.SetSourcePlayable(_layerMixer);

        }

        /// <summary>
        /// Called by Unity.
        /// </summary>
        private void OnDisable()
        {
            // Destroy _playableGrap.
            if(_playableGrap.IsValid())
            {
                _playableGrap.Destroy();
            }
        }

        /// <summary>
        /// Called by Unity.
        /// </summary>
        private void Update()
        {
            // Fail silently...
            if(!_isActive)
            {
                return;
            }

            for( var i = 0; i < _motionLayers.Length; ++i)
            {
                _motionLayers[i].Update();

                if (_motionLayers[i].IsFinished)
                {
                    _motionPriorities[i] = 0;
                }
            }
        }

        #endregion Unity Events Handling
    }
}
