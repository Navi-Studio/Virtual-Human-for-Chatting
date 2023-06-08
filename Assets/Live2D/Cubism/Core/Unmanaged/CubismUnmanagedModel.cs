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
    /// Unmanaged model.
    /// </summary>
    public sealed class CubismUnmanagedModel
    {
        #region Factory Methods

        /// <summary>
        /// Instantiates <see cref="CubismUnmanagedMoc"/>.
        /// </summary>
        /// <param name="moc">Moc.</param>
        /// <returns>Instance on success; <see langword="null"/> otherwise.</returns>
        public static CubismUnmanagedModel FromMoc(CubismUnmanagedMoc moc)
        {
            if (moc == null)
            {
                return null;
            }


            var model = new CubismUnmanagedModel(moc);


            return (model.Ptr != IntPtr.Zero)
                ? model
                : null;
        }

        #endregion

        /// <summary>
        /// Unmanaged parameters.
        /// </summary>
        public CubismUnmanagedParameters Parameters { get; private set; }

        /// <summary>
        /// Unmanaged parts.
        /// </summary>
        public CubismUnmanagedParts Parts { get; private set; }

        /// <summary>
        /// Unmanaged drawables.
        /// </summary>
        public CubismUnmanagedDrawables Drawables { get; private set; }

        /// <summary>
        /// Unmanaged canvas information(size, origin, ppu).
        /// </summary>
        public CubismUnmanagedCanvasInformation CanvasInformation { get; private set; }

        /// <summary>
        /// Native model pointer.
        /// </summary>
        public IntPtr Ptr { get; private set; }


        /// <summary>
        /// Updates instance.
        /// </summary>
        public void Update()
        {
            if (Ptr == IntPtr.Zero)
            {
                return;
            }


            CubismCoreDll.UpdateModel(Ptr);
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
        /// <param name="moc">Moc.</param>
        private CubismUnmanagedModel(CubismUnmanagedMoc moc)
        {
            // Allocate and initialize memory (returning on fail).
            var size = CubismCoreDll.GetSizeofModel(moc.Ptr);
            var memory = CubismUnmanagedMemory.Allocate((int)size, CubismCoreDll.AlignofModel);


            if (memory == IntPtr.Zero)
            {
                return;
            }


            // Initialize native model (cleaning up and returning on fail).
            Ptr = CubismCoreDll.InitializeModelInPlace(moc.Ptr, memory, size);


            if (Ptr == IntPtr.Zero)
            {
                CubismUnmanagedMemory.Deallocate(memory);


                return;
            }


            // Initialize 'components'.
            Parameters = new CubismUnmanagedParameters(Ptr);
            Parts = new CubismUnmanagedParts(Ptr);
            Drawables = new CubismUnmanagedDrawables(Ptr);
            CanvasInformation = new CubismUnmanagedCanvasInformation(Ptr);
        }

        #endregion
    }
}
