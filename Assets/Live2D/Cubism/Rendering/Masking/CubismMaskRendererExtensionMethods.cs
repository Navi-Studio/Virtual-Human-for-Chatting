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
    /// Extensions for <see cref="CubismMaskRenderer"/>.
    /// </summary>
    internal static class CubismMaskRendererExtensionMethods
    {
        /// <summary>
        /// Combines bounds of multiple <see cref="CubismMaskRenderer"/>s.
        /// </summary>
        /// <param name="self">Renderers.</param>
        /// <returns>Combined bounds.</returns>
        public static Bounds GetBounds(this CubismMaskRenderer[] self)
        {
            var min = self[0].MeshBounds.min;
            var max = self[0].MeshBounds.max;


            for (var i = 1; i < self.Length; ++i)
            {
                var boundsI = self[i].MeshBounds;


                if (boundsI.min.x < min.x)
                {
                    min.x = boundsI.min.x;
                }

                if (boundsI.max.x > max.x)
                {
                    max.x = boundsI.max.x;
                }


                if (boundsI.min.y < min.y)
                {
                    min.y = boundsI.min.y;
                }

                if (boundsI.max.y > max.y)
                {
                    max.y = boundsI.max.y;
                }
            }


            return new Bounds
            {
                min = min,
                max = max
            };
        }
    }
}
