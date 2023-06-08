/**
 * Copyright(c) Live2D Inc. All rights reserved.
 *
 * Use of this source code is governed by the Live2D Open Software license
 * that can be found at https://www.live2d.com/eula/live2d-open-software-license-agreement_en.html.
 */


using System;


namespace Live2D.Cubism.Framework.MotionFade
{
    public static class CubismFadeMath
    {
        /// <summary>
        /// Calculate the easing processed signaure.
        /// </summary>
        /// <param name="value">Value to be subjected to easing.</param>
        /// <returns>Eased sign value.</returns>
        public static float GetEasingSine(float value)
        {
            if (value < 0.0f) return 0.0f;
            if (value > 1.0f) return 1.0f;

            return (float)(0.5f - 0.5f * Math.Cos(value * (float)Math.PI));
        }
    }
}
