/**
 * Copyright(c) Live2D Inc. All rights reserved.
 *
 * Use of this source code is governed by the Live2D Open Software license
 * that can be found at https://www.live2d.com/eula/live2d-open-software-license-agreement_en.html.
 */


using Live2D.Cubism.Core;
using Live2D.Cubism.Framework.MotionFade;
using System.Collections.Generic;
using UnityEngine;


namespace Live2D.Cubism.Framework.Expression
{
    /// <summary>
    /// Expression controller.
    /// </summary>
    public class CubismExpressionController : MonoBehaviour, ICubismUpdatable
    {
        #region variable

        /// <summary>
        /// Expressions data list.
        /// </summary>
        [SerializeField]
        public CubismExpressionList ExpressionsList;

        /// <summary>
        /// CubismModel cache.
        /// </summary>
        private CubismModel _model = null;

        /// <summary>
        /// Playing expressions.
        /// </summary>
        private List<CubismPlayingExpression> _playingExpressions = new List<CubismPlayingExpression>();

        /// <summary>
        /// Playing expressions index.
        /// </summary>
        [SerializeField]
        public int CurrentExpressionIndex = -1;

        /// <summary>
        /// Last playing expressions index.
        /// </summary>
        private int _lastExpressionIndex = -1;

        /// <summary>
        /// Model has update controller component.
        /// </summary>
        [HideInInspector]
        public bool HasUpdateController { get; set; }

        #endregion

        /// <summary>
        /// Add new expression to playing expressions.
        /// </summary>
        private void StartExpression()
        {
            // Fail silently...
            if(ExpressionsList == null || ExpressionsList.CubismExpressionObjects == null)
            {
                return;
            }

            // Backup expression.
            _lastExpressionIndex = CurrentExpressionIndex;

            // Set last expression end time
            if(_playingExpressions.Count > 0)
            {
                var playingExpression = _playingExpressions[_playingExpressions.Count - 1];
                playingExpression.ExpressionEndTime = playingExpression.ExpressionUserTime + playingExpression.FadeOutTime;
                _playingExpressions[_playingExpressions.Count - 1] = playingExpression;
            }

            // Fail silently...
            if(CurrentExpressionIndex < 0 || CurrentExpressionIndex >= ExpressionsList.CubismExpressionObjects.Length)
            {
                return;
            }

            var palyingExpression = CubismPlayingExpression.Create(_model, ExpressionsList.CubismExpressionObjects[CurrentExpressionIndex]);

            if(palyingExpression == null)
            {
                return;
            }

            // Add to PlayingExList.
            _playingExpressions.Add(palyingExpression);
        }

        /// <summary>
        /// Called by cubism update controller. Order to invoke OnLateUpdate.
        /// </summary>
        public int ExecutionOrder
        {
            get { return CubismUpdateExecutionOrder.CubismExpressionController; }
        }

        /// <summary>
        /// Called by cubism update controller. Needs to invoke OnLateUpdate on Editing.
        /// </summary>
        public bool NeedsUpdateOnEditing
        {
            get { return false; }
        }

        /// <summary>
        /// Called by cubism update manager.
        /// </summary>
        public void OnLateUpdate()
        {
            // Fail silently...
            if(!enabled || _model == null)
            {
                return;
            }

            // Start expression when current expression changed.
            if(CurrentExpressionIndex != _lastExpressionIndex)
            {
                StartExpression();
            }

            // Update expression
            for(var expressionIndex = 0; expressionIndex < _playingExpressions.Count; ++expressionIndex)
            {
                var playingExpression = _playingExpressions[expressionIndex];

                // Update expression user time.
                playingExpression.ExpressionUserTime += Time.deltaTime;

                // Update weight
                var fadeIn = (Mathf.Abs(playingExpression.FadeInTime) < float.Epsilon)
                              ? 1.0f
                              : CubismFadeMath.GetEasingSine(playingExpression.ExpressionUserTime / playingExpression.FadeInTime);

                var fadeOut = ((Mathf.Abs(playingExpression.ExpressionEndTime) < float.Epsilon) || (playingExpression.ExpressionEndTime < 0.0f))
                              ? 1.0f
                              : CubismFadeMath.GetEasingSine(
                                  (playingExpression.ExpressionEndTime - playingExpression.ExpressionUserTime) / playingExpression.FadeOutTime);

                playingExpression.Weight = fadeIn * fadeOut;

                // Apply value.
                for(var i = 0; i < playingExpression.Destinations.Length; ++i)
                {
                    // Fail silently...
                    if(playingExpression.Destinations[i] == null)
                    {
                        continue;
                    }

                    switch(playingExpression.Blend[i])
                    {
                        case CubismParameterBlendMode.Additive:
                            playingExpression.Destinations[i].AddToValue(playingExpression.Value[i], playingExpression.Weight);
                            break;
                        case CubismParameterBlendMode.Multiply:
                            playingExpression.Destinations[i].MultiplyValueBy(playingExpression.Value[i], playingExpression.Weight);
                            break;
                        case CubismParameterBlendMode.Override:
                            playingExpression.Destinations[i].Value = playingExpression.Destinations[i].Value * (1 - playingExpression.Weight) + (playingExpression.Value[i] * playingExpression.Weight);
                            break;
                        default:
                            // When an unspecified value is set, it is already in addition mode.
                            break;
                    }
                }

                // Apply update value
                _playingExpressions[expressionIndex] = playingExpression;
            }

            // Remove expression from playing expressions
            for(var expressionIndex = _playingExpressions.Count - 1; expressionIndex >= 0; --expressionIndex)
            {
                if(_playingExpressions[expressionIndex].Weight > 0.0f)
                {
                    continue;
                }

                _playingExpressions.RemoveAt(expressionIndex);
            }
        }


        #region Unity Event Handling

        /// <summary>
        /// Called by Unity.
        /// </summary>
        private void OnEnable()
        {
            _model = this.FindCubismModel();

            // Get cubism update controller.
            HasUpdateController = (GetComponent<CubismUpdateController>() != null);
        }

        /// <summary>
        /// Called by Unity.
        /// </summary>
        private void LateUpdate()
        {
            if(!HasUpdateController)
            {
                OnLateUpdate();
            }
        }

        #endregion

    }
}
