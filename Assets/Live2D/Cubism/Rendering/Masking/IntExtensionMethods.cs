/**
 * Copyright(c) Live2D Inc. All rights reserved.
 *
 * Use of this source code is governed by the Live2D Open Software license
 * that can be found at https://www.live2d.com/eula/live2d-open-software-license-agreement_en.html.
 */


using UnityEngine;


namespace Live2D.Cubism.Rendering.Masking
{
    /// <summary>
    /// Extensions for <see cref="System.Int32"/>s.
    /// </summary>
    internal static class IntExtensionMethods
    {
        /// <summary>
        /// Checks whether an integer is a power of two.
        /// </summary>
        /// <param name="self">Value to check.</param>
        /// <returns><see langword="true"/> if power of two; <see langword="false"/> otherwise.</returns>
        public static bool IsPowerOfTwo(this int self)
        {
            return Mathf.ClosestPowerOfTwo(self) == self;
        }
    }
}
