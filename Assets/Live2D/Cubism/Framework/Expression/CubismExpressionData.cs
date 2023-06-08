/**
 * Copyright(c) Live2D Inc. All rights reserved.
 *
 * Use of this source code is governed by the Live2D Open Software license
 * that can be found at https://www.live2d.com/eula/live2d-open-software-license-agreement_en.html.
 */


using Live2D.Cubism.Framework.Json;
using System;
using UnityEngine;

namespace Live2D.Cubism.Framework.Expression
{
    public class CubismExpressionData : ScriptableObject
    {
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
        /// Expression Parameters
        /// </summary>
        [SerializeField]
        public SerializableExpressionParameter[] Parameters;

        /// <summary>
        /// ExpressionParameter
        /// </summary>
        [Serializable]
        public struct SerializableExpressionParameter
        {
            /// <summary>
            /// Expression Parameter Id
            /// </summary>
            [SerializeField]
            public string Id;

            /// <summary>
            /// Expression Parameter Value
            /// </summary>
            [SerializeField]
            public float Value;

            /// <summary>
            /// Expression Parameter Blend Mode
            /// </summary>
            [SerializeField]
            public CubismParameterBlendMode Blend;
        }

        public static CubismExpressionData CreateInstance(CubismExp3Json json)
        {
            var expressionData = CreateInstance<CubismExpressionData>();
            return CreateInstance(expressionData, json);
        }

        public static CubismExpressionData CreateInstance(CubismExpressionData expressionData, CubismExp3Json json)
        {
            expressionData.Type = json.Type;
            expressionData.FadeInTime = json.FadeInTime;
            expressionData.FadeOutTime = json.FadeOutTime;
            expressionData.Parameters = new SerializableExpressionParameter[json.Parameters.Length];

            for(var i = 0; i < json.Parameters.Length; ++i)
            {
                expressionData.Parameters[i].Id = json.Parameters[i].Id;
                expressionData.Parameters[i].Value = json.Parameters[i].Value;

                switch(json.Parameters[i].Blend)
                {
                    case "Add":
                        expressionData.Parameters[i].Blend = CubismParameterBlendMode.Additive;
                        break;
                    case "Multiply":
                        expressionData.Parameters[i].Blend = CubismParameterBlendMode.Multiply;
                        break;
                    case "Overwrite":
                        expressionData.Parameters[i].Blend = CubismParameterBlendMode.Override;
                        break;
                    default:
                        expressionData.Parameters[i].Blend = CubismParameterBlendMode.Additive;
                        break;

                }
            }

            return expressionData;
        }

    }

}
