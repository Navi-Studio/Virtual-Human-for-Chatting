/**
 * Copyright(c) Live2D Inc. All rights reserved.
 *
 * Use of this source code is governed by the Live2D Open Software license
 * that can be found at https://www.live2d.com/eula/live2d-open-software-license-agreement_en.html.
 */


using Live2D.Cubism.Core;
using Live2D.Cubism.Framework;
using Live2D.Cubism.Framework.Expression;
using Live2D.Cubism.Framework.Motion;
using Live2D.Cubism.Framework.Raycasting;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Live2D.Cubism.Samples.OriginalWorkflow.Demo
{
    [RequireComponent(typeof(CubismMotionController))]
    [RequireComponent(typeof(CubismRaycaster))]
    public class CubismSampleController : MonoBehaviour
    {
        /// <summary>
        /// MotionController to be operated.
        /// </summary>
        private CubismMotionController _motionController;

        /// <summary>
        /// ExpressionController to be operated.
        /// </summary>
        private CubismExpressionController _expressionController;

        /// <summary>
        /// Operation animation clip from the inspector.
        /// </summary>
        [SerializeField]
        private AnimationClip _bodyAnimation;

        /// <summary>
        /// Array of motion set in tapbody.
        /// </summary>
        [SerializeField]
        private AnimationClip[] _tapBodyMotions;

        /// <summary>
        /// Motion set in loop motion.
        /// </summary>
        private AnimationClip _loopMotion;

        /// <summary>
        /// List of Drawables info.
        /// </summary>
        private List<HitDrawableInfomation> _hasHitDrawables;

        /// <summary>
        /// Component that performs ray judgment on Drawables of model.
        /// </summary>
        private CubismRaycaster _raycaster;

        /// <summary>
        /// Raycast Hit Results.
        /// </summary>
        private CubismRaycastHit[] _raycastResults;

        /// <summary>
        /// Enumeration type for hit area discrimination.
        /// </summary>
        private enum HitArea
        {
            Head,
            Body
        }

        /// <summary>
        /// Structure that stores Drawable information for which hit area is specified.
        /// </summary>
        private struct HitDrawableInfomation
        {
            /// <summary>
            /// Drawable with component set.
            /// </summary>
            public CubismDrawable drawable;

            /// <summary>
            /// HitArea.
            /// </summary>
            public HitArea hitArea;
        }


        /// <summary>
        /// Load model.
        /// </summary>
        private void Start()
        {
            var model = this.FindCubismModel();

            // Get components.
            _motionController = model.GetComponent<CubismMotionController>();
            _expressionController = model.GetComponent<CubismExpressionController>();
            _raycaster = model.GetComponent<CubismRaycaster>();


            // Set behavior at the end of animation.
            _motionController.AnimationEndHandler = AnimationEnded;


            // Get up to 4 results of collision detection.
            _raycastResults = new CubismRaycastHit[4];


            // Cache the drawable in which the component is set.
            {
                _hasHitDrawables = new List<HitDrawableInfomation>();

                var hitAreas = Enum.GetValues(typeof(HitArea));
                var drawables = model.Drawables;


                for (var i = 0; i < hitAreas.Length; i++)
                {
                    for (var j = 0; j < drawables.Length; j++)
                    {
                        var cubismHitDrawable = drawables[j].GetComponent<CubismHitDrawable>();

                        if (cubismHitDrawable)
                        {
                            if (cubismHitDrawable.Name == hitAreas.GetValue(i).ToString())
                            {
                                var hitDrawable = new HitDrawableInfomation();
                                hitDrawable.drawable = drawables[j];
                                hitDrawable.hitArea = (HitArea)i;

                                _hasHitDrawables.Add(hitDrawable);
                                break;
                            }
                        }
                    }
                }
            }
        }


        /// <summary>
        /// Update.
        /// </summary>
        private void Update()
        {
            // Play if animation is specified.
            SpecifiedAnimationCheck();


            if(!Input.GetMouseButtonDown(0))
            {
                return;
            }


            // Cast ray from pointer position.
            var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            var hitCount = _raycaster.Raycast(ray, _raycastResults);


            // Motion playback according to the hit location.
            for (var i = 0; i < hitCount; i++)
            {
                var hitDrawable = _raycastResults[i].Drawable;

                for (var j = 0; j < _hasHitDrawables.Count; j++)
                {
                    if (hitDrawable == _hasHitDrawables[j].drawable)
                    {
                        var hitArea = _hasHitDrawables[j].hitArea;


                        // Tap body.
                        if (hitArea == HitArea.Body)
                        {
                            // Decide motion to play at random.
                            var motionIndex = UnityEngine.Random.Range(0, _tapBodyMotions.Length);

                            Debug.Log("Tap body : Play : " + _tapBodyMotions[motionIndex].name);

                            _motionController.PlayAnimation(_tapBodyMotions[motionIndex], isLoop: false, priority:CubismMotionPriority.PriorityNormal);
                        }
                        // Tap head.
                        else if (hitArea == HitArea.Head)
                        {
                            // Decide expression motion to play at random.
                            var expressionNum = _expressionController.ExpressionsList.CubismExpressionObjects.Length;
                            var expressionIndex = UnityEngine.Random.Range(0, expressionNum);

                            _expressionController.CurrentExpressionIndex = expressionIndex;

                            Debug.Log("Tap head : Play : " + _expressionController.ExpressionsList.CubismExpressionObjects[expressionIndex].name);
                        }

                        break;
                    }
                }
            }
        }


        /// <summary>
        /// Check the specified animation and play it.
        /// </summary>
        private void SpecifiedAnimationCheck()
        {
            if(_bodyAnimation != _loopMotion)
            {
                _loopMotion = _bodyAnimation;

                Debug.Log("Body animation : Play : " + _loopMotion.name);

                _motionController.PlayAnimation(_loopMotion, priority:CubismMotionPriority.PriorityIdle);
            }
        }


        /// <summary>
        /// Called at the end of the animation.
        /// </summary>
        /// <param name="instanceId"></param>
        private void AnimationEnded(float instanceId)
        {
            // Play loop motion.
            _motionController.PlayAnimation(_loopMotion, priority:CubismMotionPriority.PriorityIdle);

            Debug.Log("Body animation : Play : " + _loopMotion.name);
        }
    }
}
