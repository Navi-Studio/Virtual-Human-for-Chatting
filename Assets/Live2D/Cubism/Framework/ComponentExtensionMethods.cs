/**
 * Copyright(c) Live2D Inc. All rights reserved.
 *
 * Use of this source code is governed by the Live2D Open Software license
 * that can be found at https://www.live2d.com/eula/live2d-open-software-license-agreement_en.html.
 */


using System.Collections.Generic;
using UnityEngine;


namespace Live2D.Cubism.Framework
{
    /// <summary>
    /// Extensions for <see cref="Component"/>s.
    /// </summary>
    public static class ComponentExtensionMethods
    {
        /// <summary>
        /// Gets components for each item of a sequence and flattens the resulting sequences into one sequence.
        /// </summary>
        /// <typeparam name="T">Component type to find.</typeparam>
        /// <param name="self">Array to query.</param>
        /// <returns>Matches.</returns>
        public static T[] GetComponentsMany<T>(this Component[] self) where T : Component
        {
            if (self == null)
            {
                return null;
            }

            var components = new List<T>();


            for (var i = 0; i < self.Length; ++i)
            {
                var range = self[i].GetComponents<T>();


                // Skip empty ranges.
                if (range == null || range.Length == 0)
                {
                    continue;
                }


                components.AddRange(range);
            }


            return components.ToArray();
        }


        /// <summary>
        /// Adds a component to multiple objects.
        /// </summary>
        /// <typeparam name="T">Component type to add.</typeparam>
        /// <param name="self">Array of objects.</param>
        /// <returns>Added components.</returns>
        public static T[] AddComponentEach<T>(this Component[] self) where T : Component
        {
            if (self == null)
            {
                return null;
            }

            var components = new T[self.Length];


            for (var i = 0; i < self.Length; ++i)
            {
                components[i] = self[i].gameObject.AddComponent<T>();
            }


            return components;
        }
    }
}
