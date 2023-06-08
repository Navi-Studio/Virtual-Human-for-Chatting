/**
 * Copyright(c) Live2D Inc. All rights reserved.
 *
 * Use of this source code is governed by the Live2D Open Software license
 * that can be found at https://www.live2d.com/eula/live2d-open-software-license-agreement_en.html.
 */


using Live2D.Cubism.Framework;
using System;
using System.Linq;
using UnityEngine;


namespace Live2D.Cubism.Editor.Importers
{
    /// <summary>
    /// Extensions for <see cref="MonoBehaviour"/>s.
    /// </summary>
    internal static class ComponentExtensionMethods
    {
        public static Component GetOrAddComponent(this Component self, Type type)
        {
            var component = self.GetComponent(type);


            if (component != null)
            {
                return component;
            }


            return self.gameObject.AddComponent(type);
        }


        /// <summary>
        /// Checks whether a component should be moved on reimport.
        /// </summary>
        /// <param name="self">Component to check against.</param>
        /// <returns>True if component should be moved; false otherwise.</returns>
        public static bool MoveOnCubismReimport(this Component self, bool componentsOnly)
        {
            return self
                .GetType()
                .GetCustomAttributes(false)
                .FirstOrDefault(a => (a.GetType() == typeof(CubismDontMoveOnReimportAttribute)) || (a.GetType() == typeof(CubismMoveOnReimportCopyComponentsOnly) && !componentsOnly)) == null;
        }
    }
}
