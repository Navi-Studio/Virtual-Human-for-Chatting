/**
 * Copyright(c) Live2D Inc. All rights reserved.
 *
 * Use of this source code is governed by the Live2D Open Software license
 * that can be found at https://www.live2d.com/eula/live2d-open-software-license-agreement_en.html.
 */


using Live2D.Cubism.Core.Unmanaged;
using Live2D.Cubism.Framework;
using UnityEngine;


namespace Live2D.Cubism.Core
{
    /// <summary>
    /// Single <see cref="CubismModel"/> canvas information.
    /// </summary>
    [CubismDontMoveOnReimport]
    public sealed class CubismCanvasInformation
    {
        /// <summary>
        /// Initializes instance.
        /// </summary>
        /// <param name="unmanagedModel">Handle to unmanaged model.</param>
        public CubismCanvasInformation(CubismUnmanagedModel unmanagedModel)
        {
            Reset(unmanagedModel);
        }


        /// <summary>
        /// Unmanaged canvas information from unmanaged model.
        /// </summary>
        private CubismUnmanagedCanvasInformation UnmanagedCanvasInformation { get; set; }


        /// <summary>
        /// Width of native model canvas.
        /// </summary>
        public float CanvasWidth
        {
            get
            {
                // Pull data.
                return UnmanagedCanvasInformation.CanvasWidth;
            }
        }


        /// <summary>
        /// Height of native model canvas.
        /// </summary>
        public float CanvasHeight
        {
            get
            {
                // Pull data.
                return UnmanagedCanvasInformation.CanvasHeight;
            }
        }


        /// <summary>
        /// Coordinate origin of X axis.
        /// </summary>
        public float CanvasOriginX
        {
            get
            {
                // Pull data.
                return UnmanagedCanvasInformation.CanvasOriginX;
            }
        }


        /// <summary>
        /// Coordinate origin of Y axis.
        /// </summary>
        public float CanvasOriginY
        {
            get
            {
                // Pull data.
                return UnmanagedCanvasInformation.CanvasOriginY;
            }
        }


        /// <summary>
        /// Pixels per unit of native model.
        /// </summary>
        public float PixelsPerUnit
        {
            get
            {
                // Pull data.
                return UnmanagedCanvasInformation.PixelsPerUnit;
            }
        }


        /// <summary>
        /// Revives the instance.
        /// </summary>
        /// <param name="unmanagedModel">Handle to unmanaged model.</param>
        internal void Revive(CubismUnmanagedModel unmanagedModel)
        {
            UnmanagedCanvasInformation = unmanagedModel.CanvasInformation;
        }

        /// <summary>
        /// Restores instance to initial state.
        /// </summary>
        /// <param name="unmanagedModel">Handle to unmanaged model.</param>
        private void Reset(CubismUnmanagedModel unmanagedModel)
        {
            Revive(unmanagedModel);
        }
    }
}
