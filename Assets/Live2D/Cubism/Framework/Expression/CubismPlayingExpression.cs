/**
 * Copyright(c) Live2D Inc. All rights reserved.
 *
 * Use of this source code is governed by the Live2D Open Software license
 * that can be found at https://www.live2d.com/eula/live2d-open-software-license-agreement_en.html.
 */


using Live2D.Cubism.Core;
using UnityEngine;

namespace Live2D.Cubism.Framework.Expression
{
    /// <summary>
    /// The cubism expression data.
    /// </summary>
    [System.Serializable]
    public class CubismPlayingExpression
    {
        #region variable

        /// <summary>
        /// Expression type.
        /// </summary>
        [SerializeField]
        public string Type;

        /// <summary>
        /// Expression fade in time.
        /// </summary>
        [SerializeField]
        public float FadeInTime;

        /// <summary>
        /// Expression fade out time.
        /// </summary>
        [SerializeField]
        public float FadeOutTime;

        /// <summary>
        /// Expression Weight.
        /// </summary>
        [SerializeField, Range(0.0f, 1.0f)]
        public float Weight;

        /// <summary>
        /// Expression user time.
        /// </summary>
        [SerializeField]
        public float ExpressionUserTime;

        /// <summary>
        /// Expression end time.
        /// </summary>
        [SerializeField]
        public float ExpressionEndTime;

        /// <summary>
        /// Expression parameters cache.
        /// </summary>
        [SerializeField]
        public CubismParameter[] Destinations;

        /// <summary>
        /// Expression parameter value.
        /// </summary>
        [SerializeField]
        public float[] Value;

        /// <summary>
        /// Expression parameter blend mode.
        /// </summary>
        [SerializeField]
        public CubismParameterBlendMode[] Blend;

        #endregion

        /// <summary>
        /// Initialize expression data from <see cref="CubismExpressionData"/>.
        /// </summary>
        /// <param name="model">model.</param>
        /// <param name="expressionData">Source.</param>
        public static CubismPlayingExpression Create(CubismModel model, CubismExpressionData expressionData)
        {
            // Fail silently...
            if(model == null || expressionData == null)
            {
                return null;
            }

            var ret = new CubismPlayingExpression();

            ret.Type = expressionData.Type;

            ret.FadeInTime = (expressionData.FadeInTime < 0.0f)
                                ? 1.0f
                                : expressionData.FadeInTime;

            ret.FadeOutTime = (expressionData.FadeOutTime < 0.0f)
                                ? 1.0f
                                : expressionData.FadeOutTime;

            ret.Weight = 0.0f;
            ret.ExpressionUserTime = 0.0f;
            ret.ExpressionEndTime = 0.0f;

            var parameterCount = expressionData.Parameters.Length;
            ret.Destinations = new CubismParameter[parameterCount];
            ret.Value = new float[parameterCount];
            ret.Blend = new CubismParameterBlendMode[parameterCount];

            for(var i = 0; i < parameterCount; ++i)
            {
                ret.Destinations[i] = model.Parameters.FindById(expressionData.Parameters[i].Id);
                ret.Value[i] = expressionData.Parameters[i].Value;
                ret.Blend[i] = expressionData.Parameters[i].Blend;
            }

            return ret;
        }
    }
}
