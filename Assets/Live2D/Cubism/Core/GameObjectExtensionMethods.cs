/**
 * Copyright(c) Live2D Inc. All rights reserved.
 *
 * Use of this source code is governed by the Live2D Open Software license
 * that can be found at https://www.live2d.com/eula/live2d-open-software-license-agreement_en.html.
 */


using UnityEngine;


namespace Live2D.Cubism.Core
{
    /// <summary>
    /// Extends <see cref="GameObject"/>s.
    /// </summary>
    public static class GameObjectExtensionMethods
    {
        /// <summary>
        /// Finds a <see cref="CubismModel"/> relative to a <see cref="GameObject"/>.
        /// </summary>
        /// <param name="self"><see langword="this"/>.</param>
        /// <param name="includeParents">Condition for including parents in search.</param>
        /// <returns>The relative <see cref="CubismModel"/> if found; <see langword="null"/> otherwise.</returns>
        public static CubismModel FindCubismModel(this GameObject self, bool includeParents = false)
        {
            // Validate arguments.
            if (self == null)
            {
                return null;
            }


            return self.transform.FindCubismModel(includeParents);
        }
    }
}
