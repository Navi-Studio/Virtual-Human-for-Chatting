/**
 * Copyright(c) Live2D Inc. All rights reserved.
 *
 * Use of this source code is governed by the Live2D Open Software license
 * that can be found at https://www.live2d.com/eula/live2d-open-software-license-agreement_en.html.
 */


using Live2D.Cubism.Core;
using Live2D.Cubism.Framework.Motion;
using UnityEngine;

namespace Live2D.Cubism.Samples.OriginalWorkflow.Motion
{
    [RequireComponent(typeof(CubismMotionController))]
    public class CubismMotionPreview : MonoBehaviour
    {
        /// <summary>
        ///
        /// </summary>
        public AnimationClip Animation;

        /// <summary>
        /// MotionController to be operated.
        /// </summary>
        CubismMotionController _motionController;

        /// <summary>
        /// Get motion controller.
        /// </summary>
        private void Start()
        {
            var model = this.FindCubismModel();

            _motionController = model.GetComponent<CubismMotionController>();

            _motionController.AnimationEndHandler += PlayIdleAnimation;

            if (Animation == null)
            {
                return;
            }

            PlayIdleAnimation();
        }



        private void PlayIdleAnimation(float index = 0.0f)
        {
            _motionController.PlayAnimation(Animation, isLoop: false, priority: CubismMotionPriority.PriorityIdle);
        }



        /// <summary>
        /// Play specified animation.
        /// </summary>
        /// <param name="animation">Animation clip to play.</param>
        public void PlayAnimation(AnimationClip animation)
        {
            _motionController.PlayAnimation(animation, isLoop: false, priority: CubismMotionPriority.PriorityForce);
        }
    }
}
