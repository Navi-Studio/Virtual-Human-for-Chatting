/**
 * Copyright(c) Live2D Inc. All rights reserved.
 *
 * Use of this source code is governed by the Live2D Open Software license
 * that can be found at https://www.live2d.com/eula/live2d-open-software-license-agreement_en.html.
 */

/* THIS FILE WAS AUTO-GENERATED. ALL CHANGES WILL BE LOST UPON RE-GENERATION. */


namespace Live2D.Cubism.Core.Unmanaged
{
    /// <summary>
    /// <see cref="byte"/> extensions.
    /// </summary>
    internal static class ByteExtensionMethods
    {
        /// <summary>
        /// Checks whether flag is set.
        /// </summary>
        /// <param name="self">Bit field.</param>
        /// <returns><see langword="true"/> if bit is set; <see langword="false"/> otherwise.</returns>
        public static bool HasBlendAdditiveFlag(this byte self)
        {
            return (self & (1 << 0)) == (1 << 0);
        }

        /// <summary>
        /// Checks whether flag is set.
        /// </summary>
        /// <param name="self">Bit field.</param>
        /// <returns><see langword="true"/> if bit is set; <see langword="false"/> otherwise.</returns>
        public static bool HasBlendMultiplicativeFlag(this byte self)
        {
            return (self & (1 << 1)) == (1 << 1);
        }

        /// <summary>
        /// Checks whether flag is set.
        /// </summary>
        /// <param name="self">Bit field.</param>
        /// <returns><see langword="true"/> if bit is set; <see langword="false"/> otherwise.</returns>
        public static bool HasIsDoubleSidedFlag(this byte self)
        {
            return (self & (1 << 2)) == (1 << 2);
        }

        /// <summary>
        /// Checks whether flag is set.
        /// </summary>
        /// <param name="self">Bit field.</param>
        /// <returns><see langword="true"/> if bit is set; <see langword="false"/> otherwise.</returns>
        public static bool HasIsInvertedMaskFlag(this byte self)
        {
            return (self & (1 << 3)) == (1 << 3);
        }

        /// <summary>
        /// Checks whether flag is set.
        /// </summary>
        /// <param name="self">Bit field.</param>
        /// <returns><see langword="true"/> if bit is set; <see langword="false"/> otherwise.</returns>
        public static bool HasIsVisibleFlag(this byte self)
        {
            return (self & (1 << 0)) == (1 << 0);
        }

        /// <summary>
        /// Checks whether flag is set.
        /// </summary>
        /// <param name="self">Bit field.</param>
        /// <returns><see langword="true"/> if bit is set; <see langword="false"/> otherwise.</returns>
        public static bool HasVisibilityDidChangeFlag(this byte self)
        {
            return (self & (1 << 1)) == (1 << 1);
        }

        /// <summary>
        /// Checks whether flag is set.
        /// </summary>
        /// <param name="self">Bit field.</param>
        /// <returns><see langword="true"/> if bit is set; <see langword="false"/> otherwise.</returns>
        public static bool HasOpacityDidChangeFlag(this byte self)
        {
            return (self & (1 << 2)) == (1 << 2);
        }

        /// <summary>
        /// Checks whether flag is set.
        /// </summary>
        /// <param name="self">Bit field.</param>
        /// <returns><see langword="true"/> if bit is set; <see langword="false"/> otherwise.</returns>
        public static bool HasDrawOrderDidChangeFlag(this byte self)
        {
            return (self & (1 << 3)) == (1 << 3);
        }

        /// <summary>
        /// Checks whether flag is set.
        /// </summary>
        /// <param name="self">Bit field.</param>
        /// <returns><see langword="true"/> if bit is set; <see langword="false"/> otherwise.</returns>
        public static bool HasRenderOrderDidChangeFlag(this byte self)
        {
            return (self & (1 << 4)) == (1 << 4);
        }

        /// <summary>
        /// Checks whether flag is set.
        /// </summary>
        /// <param name="self">Bit field.</param>
        /// <returns><see langword="true"/> if bit is set; <see langword="false"/> otherwise.</returns>
        public static bool HasVertexPositionsDidChangeFlag(this byte self)
        {
            return (self & (1 << 5)) == (1 << 5);
        }

        /// <summary>
        /// Checks whether flag is set.
        /// </summary>
        /// <param name="self">Bit field.</param>
        /// <returns><see langword="true"/> if bit is set; <see langword="false"/> otherwise.</returns>
        public static bool HasBlendColorDidChangeFlag(this byte self)
        {
            return (self & (1 << 6)) == (1 << 6);
        }

    }
}
