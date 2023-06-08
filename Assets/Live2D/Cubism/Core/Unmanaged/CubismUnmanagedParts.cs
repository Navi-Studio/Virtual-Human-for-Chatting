/**
 * Copyright(c) Live2D Inc. All rights reserved.
 *
 * Use of this source code is governed by the Live2D Open Software license
 * that can be found at https://www.live2d.com/eula/live2d-open-software-license-agreement_en.html.
 */

/* THIS FILE WAS AUTO-GENERATED. ALL CHANGES WILL BE LOST UPON RE-GENERATION. */


using System;
using System.Runtime.InteropServices;


namespace Live2D.Cubism.Core.Unmanaged
{
    /// <summary>
    /// Unmanaged parts interface.
    /// </sumamry>
    public sealed class CubismUnmanagedParts
    {
        /// <summary>
        /// Part count.
        /// </summary>>
        public int Count { get; private set; }

        /// <summary>
        /// Part IDs.
        /// </summary>>
        public string[] Ids { get; private set; }

        /// <summary>
        /// Opacity values.
        /// </summary>>
        public CubismUnmanagedFloatArrayView Opacities { get; private set; }

        /// <summary>
        /// Part's parent part indices.
        /// </summary>>
        public CubismUnmanagedIntArrayView ParentIndices { get; private set; }



        #region Ctors

        /// <summary>
        /// Initializes instance.
        /// </summary>
        internal unsafe CubismUnmanagedParts(IntPtr modelPtr)
        {
            var length = 0;


            Count = CubismCoreDll.GetPartCount(modelPtr);


            length = CubismCoreDll.GetPartCount(modelPtr);
            Ids = new string[length];
            var _ids = (IntPtr *)(CubismCoreDll.GetPartIds(modelPtr));
            for (var i = 0; i < length; ++i)
            {
                Ids[i] = Marshal.PtrToStringAnsi(_ids[i]);
            }


            length = CubismCoreDll.GetPartCount(modelPtr);
            Opacities = new CubismUnmanagedFloatArrayView(CubismCoreDll.GetPartOpacities(modelPtr), length);

            length = CubismCoreDll.GetPartCount(modelPtr);
            ParentIndices = new CubismUnmanagedIntArrayView(CubismCoreDll.GetPartParentPartIndices(modelPtr), length);

        }

        #endregion
    }
}
