/**
 * Copyright(c) Live2D Inc. All rights reserved.
 *
 * Use of this source code is governed by the Live2D Open Software license
 * that can be found at https://www.live2d.com/eula/live2d-open-software-license-agreement_en.html.
 */


using Live2D.Cubism.Core;
using Live2D.Cubism.Framework.Expression;
using UnityEngine;


namespace Live2D.Cubism.Samples.OriginalWorkflow.Expression
{
    public class CubismExpressionPreview : MonoBehaviour
    {
        /// <summary>
        /// ExpressionController to be operated.
        /// </summary>
        CubismExpressionController _expressionController;


        /// <summary>
        /// Get expression controller.
        /// </summary>
        private void Start()
        {
            var model = this.FindCubismModel();

            _expressionController = model.GetComponent<CubismExpressionController>();
        }

        /// <summary>
        /// Change facial expression.
        /// </summary>
        /// <param name="expressionIndex">index of facial expression to set.</param>
        public void ChangeExpression(int expressionIndex)
        {
            if (_expressionController != null)
            {
                _expressionController.CurrentExpressionIndex = expressionIndex;
            }
        }
    }
}
