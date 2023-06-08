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
    /// Unmanaged drawables interface.
    /// </summary>
    public sealed class CubismUnmanagedDrawables
    {
        /// <summary>
        /// Drawable count.
        /// </summary>>
        public int Count { get; private set; }

        /// <summary>
        /// Drawable IDs.
        /// </summary>>
        public string[] Ids { get; private set; }

        /// <summary>
        /// Constant drawable flags.
        /// </summary>>
        public CubismUnmanagedByteArrayView ConstantFlags { get; private set; }

        /// <summary>
        /// Dynamic drawable flags.
        /// </summary>>
        public CubismUnmanagedByteArrayView DynamicFlags { get; private set; }

        /// <summary>
        /// Drawable texture indices.
        /// </summary>>
        public CubismUnmanagedIntArrayView TextureIndices { get; private set; }

        /// <summary>
        /// Drawable draw orders.
        /// </summary>>
        public CubismUnmanagedIntArrayView DrawOrders { get; private set; }

        /// <summary>
        /// Drawable render orders.
        /// </summary>>
        public CubismUnmanagedIntArrayView RenderOrders { get; private set; }

        /// <summary>
        /// Drawable opacities.
        /// </summary>>
        public CubismUnmanagedFloatArrayView Opacities { get; private set; }

        /// <summary>
        /// Mask count for each drawable.
        /// </summary>>
        public CubismUnmanagedIntArrayView MaskCounts { get; private set; }

        /// <summary>
        /// Masks for each drawable.
        /// </summary>>
        public CubismUnmanagedIntArrayView[] Masks { get; private set; }

        /// <summary>
        /// Number of vertices of each drawable.
        /// </summary>>
        public CubismUnmanagedIntArrayView VertexCounts { get; private set; }

        /// <summary>
        /// 2D vertex position data of each drawable.
        /// </summary>>
        public CubismUnmanagedFloatArrayView[] VertexPositions { get; private set; }

        /// <summary>
        /// 2D texture coordinate data of each drawables.
        /// </summary>>
        public CubismUnmanagedFloatArrayView[] VertexUvs { get; private set; }

        /// <summary>
        /// Number of triangle indices for each drawable.
        /// </summary>>
        public CubismUnmanagedIntArrayView IndexCounts { get; private set; }

        /// <summary>
        /// Triangle index data for each drawable.
        /// </summary>>
        public CubismUnmanagedUshortArrayView[] Indices { get; private set; }

        /// <summary>
        /// Information multiply color.
        /// </summary>>
        public CubismUnmanagedFloatArrayView MultiplyColors { get; private set; }

        /// <summary>
        /// Information Screen color.
        /// </summary>>
        public CubismUnmanagedFloatArrayView ScreenColors { get; private set; }

        /// <summary>
        /// Indices of drawables parent part.
        /// </summary>>
        public CubismUnmanagedIntArrayView ParentPartIndices { get; private set; }


        /// <summary>
        /// Resets all dynamic drawable flags.
        /// </summary>
        public void ResetDynamicFlags()
        {
            CubismCoreDll.ResetDrawableDynamicFlags(ModelPtr);
        }


        /// <summary>
        /// Native model pointer.
        /// </summary>
        private IntPtr ModelPtr {get; set;}


        #region Ctors

        /// <summary>
        /// Initializes instance.
        /// </summary>
        internal unsafe CubismUnmanagedDrawables(IntPtr modelPtr)
        {
            ModelPtr = modelPtr;


            var length = 0;
            CubismUnmanagedIntArrayView length2;


            Count = CubismCoreDll.GetDrawableCount(modelPtr);


            length = CubismCoreDll.GetDrawableCount(modelPtr);
            Ids = new string[length];
            var _ids = (IntPtr *)(CubismCoreDll.GetDrawableIds(modelPtr));
            for (var i = 0; i < length; ++i)
            {
                Ids[i] = Marshal.PtrToStringAnsi(_ids[i]);
            }


            length = CubismCoreDll.GetDrawableCount(modelPtr);
            ConstantFlags = new CubismUnmanagedByteArrayView(CubismCoreDll.GetDrawableConstantFlags(modelPtr), length);

            length = CubismCoreDll.GetDrawableCount(modelPtr);
            DynamicFlags = new CubismUnmanagedByteArrayView(CubismCoreDll.GetDrawableDynamicFlags(modelPtr), length);

            length = CubismCoreDll.GetDrawableCount(modelPtr);
            TextureIndices = new CubismUnmanagedIntArrayView(CubismCoreDll.GetDrawableTextureIndices(modelPtr), length);

            length = CubismCoreDll.GetDrawableCount(modelPtr);
            DrawOrders = new CubismUnmanagedIntArrayView(CubismCoreDll.GetDrawableDrawOrders(modelPtr), length);

            length = CubismCoreDll.GetDrawableCount(modelPtr);
            RenderOrders = new CubismUnmanagedIntArrayView(CubismCoreDll.GetDrawableRenderOrders(modelPtr), length);

            length = CubismCoreDll.GetDrawableCount(modelPtr);
            Opacities = new CubismUnmanagedFloatArrayView(CubismCoreDll.GetDrawableOpacities(modelPtr), length);

            length = CubismCoreDll.GetDrawableCount(modelPtr);
            MaskCounts = new CubismUnmanagedIntArrayView(CubismCoreDll.GetDrawableMaskCounts(modelPtr), length);

            length = CubismCoreDll.GetDrawableCount(modelPtr);
            VertexCounts = new CubismUnmanagedIntArrayView(CubismCoreDll.GetDrawableVertexCounts(modelPtr), length);

            length = CubismCoreDll.GetDrawableCount(modelPtr);
            IndexCounts = new CubismUnmanagedIntArrayView(CubismCoreDll.GetDrawableIndexCounts(modelPtr), length);

            length = CubismCoreDll.GetDrawableCount(modelPtr);
            MultiplyColors = new CubismUnmanagedFloatArrayView(CubismCoreDll.GetDrawableMultiplyColors(modelPtr), length * 4);

            length = CubismCoreDll.GetDrawableCount(modelPtr);
            ScreenColors = new CubismUnmanagedFloatArrayView(CubismCoreDll.GetDrawableScreenColors(modelPtr), length * 4);

            length = CubismCoreDll.GetDrawableCount(modelPtr);
            ParentPartIndices = new CubismUnmanagedIntArrayView(CubismCoreDll.GetDrawableParentPartIndices(modelPtr), length);


            length = CubismCoreDll.GetDrawableCount(modelPtr);
            length2 = new CubismUnmanagedIntArrayView(CubismCoreDll.GetDrawableMaskCounts(modelPtr), length);
            Masks = new CubismUnmanagedIntArrayView[length];
            var _masks = (IntPtr *)(CubismCoreDll.GetDrawableMasks(modelPtr));
            for (var i = 0; i < length; ++i)
            {
                Masks[i] = new CubismUnmanagedIntArrayView(_masks[i], length2[i]);
            }

            length = CubismCoreDll.GetDrawableCount(modelPtr);
            length2 = new CubismUnmanagedIntArrayView(CubismCoreDll.GetDrawableVertexCounts(modelPtr), length);
            VertexPositions = new CubismUnmanagedFloatArrayView[length];
            var _vertexPositions = (IntPtr *)(CubismCoreDll.GetDrawableVertexPositions(modelPtr));
            for (var i = 0; i < length; ++i)
            {
                VertexPositions[i] = new CubismUnmanagedFloatArrayView(_vertexPositions[i], length2[i] * 2);
            }

            length = CubismCoreDll.GetDrawableCount(modelPtr);
            length2 = new CubismUnmanagedIntArrayView(CubismCoreDll.GetDrawableVertexCounts(modelPtr), length);
            VertexUvs = new CubismUnmanagedFloatArrayView[length];
            var _vertexUvs = (IntPtr *)(CubismCoreDll.GetDrawableVertexUvs(modelPtr));
            for (var i = 0; i < length; ++i)
            {
                VertexUvs[i] = new CubismUnmanagedFloatArrayView(_vertexUvs[i], length2[i] * 2);
            }

            length = CubismCoreDll.GetDrawableCount(modelPtr);
            length2 = new CubismUnmanagedIntArrayView(CubismCoreDll.GetDrawableIndexCounts(modelPtr), length);
            Indices = new CubismUnmanagedUshortArrayView[length];
            var _indices = (IntPtr *)(CubismCoreDll.GetDrawableIndices(modelPtr));
            for (var i = 0; i < length; ++i)
            {
                Indices[i] = new CubismUnmanagedUshortArrayView(_indices[i], length2[i]);
            }
        }

        #endregion
    }
}
