/**
 * Copyright(c) Live2D Inc. All rights reserved.
 *
 * Use of this source code is governed by the Live2D Open Software license
 * that can be found at https://www.live2d.com/eula/live2d-open-software-license-agreement_en.html.
 */


using Live2D.Cubism.Core;
using UnityEngine;


namespace Live2D.Cubism.Framework.Physics
{
    /// <summary>
    /// Math utilities for physics.
    /// </summary>
    internal static class CubismPhysicsMath
    {
        /// <summary>
        /// Gets radian from degrees.
        /// </summary>
        /// <param name="degrees">Degrees.</param>
        /// <returns>Radian.</returns>
        public static float DegreesToRadian(float degrees)
        {
            return (degrees / 180.0f) * Mathf.PI;
        }

        /// <summary>
        /// Gets degrees from radian.
        /// </summary>
        /// <param name="radian">Radian.</param>
        /// <returns>Degrees.</returns>
        public static float RadianToDegrees(float radian)
        {
            return (radian * 180.0f) / Mathf.PI;
        }


        /// <summary>
        /// Gets angle from both vector direction.
        /// </summary>
        /// <param name="from">From vector.</param>
        /// <param name="to">To vector.</param>
        /// <returns>Angle of radian.</returns>
        public static float DirectionToRadian(Vector2 from, Vector2 to)
        {
            var q1 = Mathf.Atan2(to.y, to.x);
            var q2 = Mathf.Atan2(from.y, from.x);

            return GetAngleDiff(q1, q2);
        }


        /// <summary>
        /// Gets difference of angle.
        /// </summary>
        /// <param name="q1"></param>
        /// <param name="q2"></param>
        /// <returns></returns>
        public static float GetAngleDiff(float q1, float q2)
        {
            var ret = q1 - q2;


            while (ret < -Mathf.PI)
            {
                ret += (Mathf.PI * 2.0f);
            }

            while (ret > Mathf.PI)
            {
                ret -= (Mathf.PI * 2.0f);
            }


            return ret;
        }


        /// <summary>
    /// Gets angle from both vector direction.
    /// </summary>
    /// <param name="from">From vector.</param>
    /// <param name="to">To vector.</param>
    /// <returns>Angle of degrees.</returns>
    public static float DirectionToDegrees(Vector2 from, Vector2 to)
        {
            var radian = DirectionToRadian(from, to);
            var degree = (float)RadianToDegrees(radian);


            if ((to.x - from.x) > 0.0f)
            {
                degree = -degree;
            }


            return degree;
        }

        /// <summary>
        /// Gets vector direction from angle.
        /// </summary>
        /// <param name="totalAngle">Radian.</param>
        /// <returns>Direction of vector.</returns>
        public static Vector2 RadianToDirection(float totalAngle)
        {
            var ret = Vector2.zero;


            ret.x = Mathf.Sin(totalAngle);
            ret.y = (float)Mathf.Cos(totalAngle);


            return ret;
        }


        /// <summary>
        /// Gets range of value.
        /// </summary>
        /// <param name="min">Minimum value.</param>
        /// <param name="max">Maximum value.</param>
        /// <returns></returns>
        private static float GetRangeValue(float min, float max)
        {
            var maxValue = Mathf.Max(min, max);
            var minValue = Mathf.Min(min, max);
            return Mathf.Abs(maxValue - minValue);
        }

        /// <summary>
        /// Gets middle value.
        /// </summary>
        /// <param name="min">Minimum value.</param>
        /// <param name="max">Maximum value.</param>
        /// <returns></returns>
        private static float GetDefaultValue(float min, float max)
        {
            var minValue = Mathf.Min(min, max);
            return minValue + (GetRangeValue(min, max) / 2.0f);
        }

        /// <summary>
        /// Normalize parameter value.
        /// </summary>
        /// <param name="parameter">Target parameter.</param>
        /// <param name="parameterValue">Target parameter Value.</param>
        /// <param name="normalizedMinimum">Value of normalized minimum.</param>
        /// <param name="normalizedMaximum">Value of normalized maximum.</param>
        /// <param name="normalizedDefault">Value of normalized default.</param>
        /// <param name="isInverted">True if input is inverted; otherwise.</param>
        /// <returns></returns>
        public static float Normalize(CubismParameter parameter,
            ref float parameterValue,
            float normalizedMinimum,
            float normalizedMaximum,
            float normalizedDefault,
            bool isInverted = false)
        {
            var result = 0.0f;

            var maxValue = Mathf.Max(parameter.MaximumValue, parameter.MinimumValue);

            if (maxValue < parameterValue)
            {
                parameterValue = maxValue;
            }

            var minValue = Mathf.Min(parameter.MaximumValue, parameter.MinimumValue);
            if (minValue > parameterValue)
            {
                parameterValue = minValue;
            }

            var minNormValue = Mathf.Min(normalizedMinimum, normalizedMaximum);
            var maxNormValue = Mathf.Max(normalizedMinimum, normalizedMaximum);
            var middleNormValue = normalizedDefault;

            var middleValue = GetDefaultValue(minValue, maxValue);
            var paramValue = parameterValue - middleValue;

            switch ((int)Mathf.Sign(paramValue))
            {
                case 1:
                    {
                        var nLength = maxNormValue - middleNormValue;
                        var pLength = maxValue - middleValue;
                        if (pLength != 0.0f)
                        {
                            result = paramValue * (nLength / pLength);
                            result += middleNormValue;
                        }


                        break;
                    }
                case -1:
                    {
                        var nLength = minNormValue - middleNormValue;
                        var pLength = minValue - middleValue;
                        if (pLength != 0.0f)
                        {
                            result = paramValue * (nLength / pLength);
                            result += middleNormValue;
                        }


                        break;
                    }
                case 0:
                    {
                        result = middleNormValue;


                        break;
                    }
            }

            return (isInverted) ? result : (result * -1.0f);
        }
    }
}


