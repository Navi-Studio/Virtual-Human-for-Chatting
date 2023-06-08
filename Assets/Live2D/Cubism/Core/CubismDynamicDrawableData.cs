/**
 * Copyright(c) Live2D Inc. All rights reserved.
 *
 * Use of this source code is governed by the Live2D Open Software license
 * that can be found at https://www.live2d.com/eula/live2d-open-software-license-agreement_en.html.
 */


using Live2D.Cubism.Core.Unmanaged;
using UnityEngine;


namespace Live2D.Cubism.Core
{
    /// <summary>
    /// Dynamic <see cref="CubismDrawable"/> data.
    /// </summary>
    public sealed class CubismDynamicDrawableData
    {
        #region Factory Methods

        /// <summary>
        /// Creates buffer for dynamic <see cref="CubismDrawable"/> data.
        /// </summary>
        /// <param name="unmanagedModel">Unmanaged model to create buffer for.</param>
        /// <returns>Buffer.</returns>
        internal static CubismDynamicDrawableData[] CreateData(CubismUnmanagedModel unmanagedModel)
        {
            var unmanagedDrawables = unmanagedModel.Drawables;
            var buffer = new CubismDynamicDrawableData[unmanagedDrawables.Count];


            // Initialize buffers.
            var vertexCounts = unmanagedDrawables.VertexCounts;


            for (var i = 0; i < buffer.Length; ++i)
            {
                buffer[i] = new CubismDynamicDrawableData
                {
                    VertexPositions = new Vector3[vertexCounts[i]]
                };
            }


            return buffer;
        }

        #endregion

        /// <summary>
        /// Dirty flags.
        /// </summary>
        internal byte Flags { private get; set; }


        /// <summary>
        /// Current opacity.
        /// </summary>
        public float Opacity { get; internal set; }

        /// <summary>
        /// Current draw order.
        /// </summary>
        public int DrawOrder { get; internal set; }

        /// <summary>
        /// Current render order.
        /// </summary>
        public int RenderOrder { get; internal set; }

        /// <summary>
        /// Current vertex position.
        /// </summary>
        public Vector3[] VertexPositions { get; internal set; }

        /// <summary>
        /// Current multiply color.
        /// </summary>
        public Color MultiplyColor{ get; internal set; }

        /// <summary>
        /// Current screen color.
        /// </summary>
        public Color ScreenColor { get; internal set; }


        /// <summary>
        /// True if currently visible.
        /// </summary>
        public bool IsVisible
        {
            get { return Flags.HasIsVisibleFlag(); }
        }


        /// <summary>
        /// True if <see cref="IsVisible"/> did change.
        /// </summary>
        public bool IsVisibilityDirty
        {
            get { return Flags.HasVisibilityDidChangeFlag(); }
        }

        /// <summary>
        /// True if <see cref="Opacity"/> did change.
        /// </summary>
        public bool IsOpacityDirty
        {
            get { return Flags.HasOpacityDidChangeFlag(); }
        }

        /// <summary>
        /// True if <see cref="DrawOrder"/> did change.
        /// </summary>
        public bool IsDrawOrderDirty
        {
            get { return Flags.HasDrawOrderDidChangeFlag(); }
        }

        /// <summary>
        /// True if <see cref="RenderOrder"/> did change.
        /// </summary>
        public bool IsRenderOrderDirty
        {
            get { return Flags.HasRenderOrderDidChangeFlag(); }
        }

        /// <summary>
        /// True if <see cref="VertexPositions"/> did change.
        /// </summary>
        public bool AreVertexPositionsDirty
        {
            get { return Flags.HasVertexPositionsDidChangeFlag(); }
        }

        /// <summary>
        /// True if <see cref="MultiplyColor"/> and <see cref="ScreenColor"/> did change.
        /// </summary>
        public bool IsBlendColorDirty
        {
            get { return Flags.HasBlendColorDidChangeFlag(); }
        }

        /// <summary>
        /// True if any data did change.
        /// </summary>
        public bool IsAnyDirty
        {
            get { return Flags != 0; }
        }
    }
}
