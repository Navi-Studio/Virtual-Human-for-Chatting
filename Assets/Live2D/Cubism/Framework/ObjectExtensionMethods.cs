/**
 * Copyright(c) Live2D Inc. All rights reserved.
 *
 * Use of this source code is governed by the Live2D Open Software license
 * that can be found at https://www.live2d.com/eula/live2d-open-software-license-agreement_en.html.
 */


using UnityEngine;


namespace Live2D.Cubism.Framework
{
    /// <summary>
    /// Extensions for <see cref="Object"/>s.
    /// </summary>
    internal static class ObjectExtensionMethods
    {
        /// <summary>
        /// Extracts an interface from an <see cref="Object"/>.
        /// </summary>
        /// <typeparam name="T">Interface type to extract.</typeparam>
        /// <param name="self"><see langword="this"/>.</param>
        /// <returns>Valid reference on success; <see langword="null"/> otherwise.</returns>
        public static T GetInterface<T>(this Object self) where T : class
        {
            var result = self as T;


            if (result != null)
            {
                return result;
            }


            // Deal with GameObjects.
            var gameObject = self as GameObject;


            if (gameObject != null)
            {
                result = gameObject.GetComponent<T>();
            }


            // Warn on error.
            if (self != null && result == null)
            {
                Debug.LogWarning(self + " doesn't expose requested interface of type \"" + typeof(T) + "\".");
            }


            return result;
        }


        /// <summary>
        /// Nulls reference in case an <see cref="Object"/> doesn't expose an interface requested.
        /// </summary>
        /// <typeparam name="T">Type of interface to check for.</typeparam>
        /// <param name="self"><see langword="this"/>.</param>
        /// <returns><paramref name="self"/> if object exposes interface; <see langword="null"/> otherwise.</returns>
        public static Object ToNullUnlessImplementsInterface<T>(this Object self) where T : class
        {
            var exposesInterface = self.ImplementsInterface<T>();


            // Warn on error.
            if (self != null && !exposesInterface)
            {
                Debug.LogWarning(self + " doesn't expose requested interface of type \"" + typeof(T) + "\".");
            }


            return (exposesInterface)
                ? self
                : null;
        }


        /// <summary>
        /// Checks whether a <see cref="Object"/> implements an interface.
        /// </summary>
        /// <typeparam name="T">Interface type to check against.</typeparam>
        /// <param name="self"><see langword="this"/>.</param>
        /// <returns><see langword="true"/> if interface is exposed; <see langword="false"/> otherwise.</returns>
        public static bool ImplementsInterface<T>(this Object self)
        {
            // Return early in case argument matches type.
            if (self is T)
            {
                return true;
            }


            // Search in components in case object is a GameObject.
            var gameObject = self as GameObject;


            if (gameObject != null)
            {
                var components = gameObject.GetComponents<T>();


                return components.Length > 0;
            }


            // Return on fail.
            return false;
        }
    }
}
