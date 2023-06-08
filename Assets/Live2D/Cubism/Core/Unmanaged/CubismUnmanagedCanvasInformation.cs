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
    /// Unmanaged canvas information.
    /// </summary>
    public sealed class CubismUnmanagedCanvasInformation
    {
        /// <summary>
        /// Width of native model canvas.
        /// </summary>
        public float CanvasWidth { get; private set; }

        /// <summary>
        /// Height of native model canvas.
        /// </summary>
        public float CanvasHeight { get; private set; }

        /// <summary>
        /// Coordinate origin of X axis.
        /// </summary>
        public float CanvasOriginX { get; private set; }

        /// <summary>
        /// Coordinate origin of Y axis.
        /// </summary>
        public float CanvasOriginY { get; private set; }

        /// <summary>
        /// Pixels per unit of native model.
        /// </summary>
        public float PixelsPerUnit { get; private set; }

        #region Ctors

        /// <summary>
        /// Initializes instance.
        /// </summary>
        /// <param name="modelPtr"> Native model pointer. </param>
        internal unsafe CubismUnmanagedCanvasInformation(IntPtr modelPtr)
        {
            if (modelPtr == IntPtr.Zero)
            {
                return;
            }

            float[] canvasSize = new float[2];
            float[] canvasOrigin = new float[2];
            float[] pixelsPerUnitBuffer = new float[1];

            fixed( float* canvasSizePtr = canvasSize, canvasOriginPtr = canvasOrigin, pixelsPerUnitPtr = pixelsPerUnitBuffer )
            {
                CubismCoreDll.ReadCanvasInfo(modelPtr, (IntPtr)canvasSizePtr, (IntPtr)canvasOriginPtr, (IntPtr)pixelsPerUnitPtr);

                CanvasWidth = canvasSize[0];
                CanvasHeight = canvasSize[1];
                CanvasOriginX = canvasOrigin[0];
                CanvasOriginY = canvasOrigin[1];
                PixelsPerUnit = pixelsPerUnitBuffer[0];
            }
        }

        #endregion

    }
}
