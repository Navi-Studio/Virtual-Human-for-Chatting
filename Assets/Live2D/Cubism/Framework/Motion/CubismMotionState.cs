/**
 * Copyright(c) Live2D Inc. All rights reserved.
 *
 * Use of this source code is governed by the Live2D Open Software license
 * that can be found at https://www.live2d.com/eula/live2d-open-software-license-agreement_en.html.
 */

using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.Playables;

namespace Live2D.Cubism.Framework.Motion
{
    /// <summary>
    /// Cubism motion state.
    /// </summary>
    public class CubismMotionState
    {
        #region Variable

        /// <summary>
        /// Cubism motion state clip.
        /// </summary>
        public AnimationClip Clip { get; private set; }

        /// <summary>
        /// Animation clip mixer.
        /// </summary>
        public AnimationMixerPlayable ClipMixer { get; private set; }

        /// <summary>
        /// Animation clip playable.
        /// </summary>
        public AnimationClipPlayable ClipPlayable { get; private set; }

        #endregion

        /// <summary>
        /// Create motion state.
        /// </summary>
        /// <param name="playableGraph">Playable graph.</param>
        /// <param name="clip">Animation clip.</param>
        /// <param name="isLoop">Animation is loop.</param>
        /// <param name="speed">Animation speed.</param>
        public static CubismMotionState CreateCubismMotionState(PlayableGraph playableGraph, AnimationClip clip, bool isLoop = true, float speed = 1.0f)
        {
            var ret = new CubismMotionState();

            ret.Clip = clip;

            // Create animation clip mixer.
            ret.ClipMixer = AnimationMixerPlayable.Create(playableGraph, 2);
            ret.ClipMixer.SetSpeed(speed);

            // Connect AnimationClip Playable
            ret.ClipPlayable = AnimationClipPlayable.Create(playableGraph, ret.Clip);

            if(!isLoop)
            {
                ret.ClipPlayable.SetDuration(clip.length - 0.0001f);
            }

            ret.ClipMixer.ConnectInput(0, ret.ClipPlayable, 0);
            ret.ClipMixer.SetInputWeight(0, 1.0f);

            return ret;
        }

        /// <summary>
        /// Connect motion state clip mixer.
        /// </summary>
        /// <param name="clipMixer">.</param>
        public void ConnectClipMixer(AnimationMixerPlayable clipMixer)
        {
            var lastInput = ClipMixer.GetInputCount() - 1;
#if UNITY_2018_2_OR_NEWER
            ClipMixer.DisconnectInput(lastInput);
#else
            ClipMixer.GetGraph().Disconnect(ClipMixer, lastInput);
#endif
            ClipMixer.ConnectInput(lastInput, clipMixer, 0);
            ClipMixer.SetInputWeight(lastInput, 1.0f);
        }
    }
}
