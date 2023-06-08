/**
 * Copyright(c) Live2D Inc. All rights reserved.
 *
 * Use of this source code is governed by the Live2D Open Software license
 * that can be found at https://www.live2d.com/eula/live2d-open-software-license-agreement_en.html.
 */


namespace Live2D.Cubism.Rendering
{
    /// <summary>
    /// Extensions for <see cref="CubismSortingMode"/>.
    /// </summary>
    public static class CubismSortingModeExtensionMethods
    {
        /// <summary>
        /// Checks whether a mode sorts by depth.
        /// </summary>
        /// <param name="self">Mode to query.</param>
        /// <returns><see langword="true"/> if mode sorts by depth; <see langword="false"/> otherwise.</returns>
        public static bool SortByDepth(this CubismSortingMode self)
        {
            return self == CubismSortingMode.BackToFrontZ || self == CubismSortingMode.FrontToBackZ;
        }

        /// <summary>
        /// Checks whether a mode sorts by order.
        /// </summary>
        /// <param name="self">Mode to query.</param>
        /// <returns><see langword="true"/> if mode sorts by order; <see langword="false"/> otherwise.</returns>
        public static bool SortByOrder(this CubismSortingMode self)
        {
            return !self.SortByDepth();
        }
    }
}
