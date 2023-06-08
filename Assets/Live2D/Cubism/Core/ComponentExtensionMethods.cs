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
    /// Extensions for <see cref="Component"/>s.
    /// </summary>
    public static class ComponentExtensionMethods
    {
        /// <summary>
        /// Finds a <see cref="CubismModel"/> relative to a <see cref="Component"/>.
        /// </summary>
        /// <param name="self">Component to base search on.</param>
        /// <param name="includeParents">Condition for including parents in search.</param>
        /// <returns>The relative <see cref="CubismModel"/> if found; <see langword="null"/> otherwise.</returns>
        public static CubismModel FindCubismModel(this Component self, bool includeParents = false)
        {
            // Validate arguments.
            if (self == null)
            {
                return null;
            }


            var model = self.GetComponent<CubismModel>();


            // Return model if found.
            if (model != null)
            {
                return model;
            }


            // Recursively search in parents if requested.
            if (includeParents)
            {
                for (var parent = self.transform.parent; parent != null; parent = parent.parent)
                {
                    model = parent.GetComponent<CubismModel>();


                    if (model)
                    {
                        return model;
                    }
                }
            }


            // Signal not found.
            return null;
        }
    }
}
