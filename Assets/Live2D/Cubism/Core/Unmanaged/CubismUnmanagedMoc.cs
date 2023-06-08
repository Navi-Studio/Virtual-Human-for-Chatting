/**
 * Copyright(c) Live2D Inc. All rights reserved.
 *
 * Use of this source code is governed by the Live2D Open Software license
 * that can be found at https://www.live2d.com/eula/live2d-open-software-license-agreement_en.html.
 */

/* THIS FILE WAS AUTO-GENERATED. ALL CHANGES WILL BE LOST UPON RE-GENERATION. */


using System;


namespace Live2D.Cubism.Core.Unmanaged
{
    /// <summary>
    /// Unmanaged moc.
    /// </summary>
    public sealed class CubismUnmanagedMoc
    {
        #region Factory Methods

        /// <summary>
        /// Creates <see cref="CubismUnmanagedMoc"/> from bytes.
        /// </summary>
        /// <param name="bytes">Moc bytes.</param>
        /// <returns>Instance on success; <see langword="null"/> otherwise.</returns>
        public static CubismUnmanagedMoc FromBytes(byte[] bytes)
        {
            if (bytes == null)
            {
                return null;
            }


            var moc = new CubismUnmanagedMoc(bytes);


            return (moc.Ptr != IntPtr.Zero)
                ? moc
                : null;
        }

        #endregion

        /// <summary>
        /// Native moc pointer.
        /// </summary>
        public IntPtr Ptr { get; private set; }

        /// <summary>
        /// .moc3 version.
        /// </summary>
        public uint MocVersion { get; private set; }

        /// <summary>
        /// Checks consistency of a moc.
        /// </summary>
        public static bool HasMocConsistency(byte[] bytes)
        {
            // Allocate and initialize memory (returning on fail).
            var memory = CubismUnmanagedMemory.Allocate(bytes.Length, CubismCoreDll.AlignofMoc);

            CubismUnmanagedMemory.Write(bytes, memory);

            // '1' if Moc is valid; '0' otherwise.
            var mocConsistencyNum = CubismCoreDll.HasMocConsistency(memory, (uint)bytes.Length);
            var hasMocConsistency = (mocConsistencyNum == 1);

            CubismUnmanagedMemory.Deallocate(memory);

            return hasMocConsistency;
        }

        /// <summary>
        /// Releases instance.
        /// </summary>
        public void Release()
        {
            if (Ptr == IntPtr.Zero)
            {
                return;
            }


            CubismUnmanagedMemory.Deallocate(Ptr);


            Ptr = IntPtr.Zero;
        }

        #region Ctors

        /// <summary>
        /// Initializes instance.
        /// </summary>
        /// <param name="bytes">Moc bytes.</param>
        private CubismUnmanagedMoc(byte[] bytes)
        {
            // Allocate and initialize memory (returning on fail).
            var memory = CubismUnmanagedMemory.Allocate(bytes.Length, CubismCoreDll.AlignofMoc);


            if (memory == IntPtr.Zero)
            {
                return;
            }


            CubismUnmanagedMemory.Write(bytes, memory);


            // Revive native moc (cleaning up on fail).
            Ptr = CubismCoreDll.ReviveMocInPlace(memory, (uint)bytes.Length);


            if (Ptr == IntPtr.Zero)
            {
                CubismUnmanagedMemory.Deallocate(memory);
                // Fail silently.
                return;
            }

            MocVersion = CubismCoreDll.GetMocVersion(Ptr, (uint)bytes.Length);
        }

        #endregion
    }
}
