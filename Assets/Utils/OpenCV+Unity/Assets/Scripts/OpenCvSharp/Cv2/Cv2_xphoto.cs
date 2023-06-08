using System;
using System.Collections.Generic;
using OpenCvSharp.Util;

namespace OpenCvSharp
{
    static partial class Cv2
    {
        public static void BalanceWhite(InputArray src, OutputArray dst)
        {
            if (src == null)
                throw new ArgumentNullException("Cv2.BalanceWhite: source Mat is null");
            if (dst == null)
                throw new ArgumentNullException("Cv2.BalanceWhite: destination Mat is null");
            src.ThrowIfDisposed();
            dst.ThrowIfDisposed();

            NativeMethods.xphoto_balanceWhite(src.CvPtr, dst.CvPtr);
        }
    }
}
