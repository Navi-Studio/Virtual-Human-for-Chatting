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
    /// Raw Core DLL bindings.
    /// </summary>
    public static class CubismCoreDll
    {
        /// <summary>
        /// Name of native DLL file.
        /// </summary>
#if (UNITY_IOS || UNITY_WEBGL || UNITY_SWITCH) && !UNITY_EDITOR
        public const string DllName = "__Internal";
#else
        public const string DllName = "Live2DCubismCore";
#endif


        /// <sumamry>
        /// Necessary alignment for mocs (in bytes).
        /// </sumamry>
        public const int AlignofMoc = 64;
        /// <sumamry>
        /// Necessary alignment for models (in bytes).
        /// </sumamry>
        public const int AlignofModel = 16;


        /// <sumamry>
        /// .moc3 file version Unknown
        /// </sumamry>
        public const int MocVersion_Unknown = 0;
        /// <sumamry>
        /// .moc3 file version 3.0.00 - 3.2.07
        /// </sumamry>
        public const int MocVersion_30 = 1;
        /// <sumamry>
        /// .moc3 file version 3.3.00 - 3.3.03
        /// </sumamry>
        public const int MocVersion_33 = 2;
        /// <sumamry>
        /// .moc3 file version 4.0.00 - 4.1.05
        /// </sumamry>
        public const int MocVersion_40 = 3;
        /// <sumamry>
        /// .moc3 file version 4.2.00 -
        /// </sumamry>
        public const int MocVersion_42 = 4;


        /// <sumamry>
        /// Normal Parameter.
        /// </sumamry>
        public const int ParameterType_Normal = 0;
        /// <sumamry>
        /// Parameter for blend shape.
        /// </sumamry>
        public const int ParameterType_BlendShape = 1;


        /// <sumamry>
        /// Additive blend mode bit.
        /// </sumamry>
        public const Byte BlendAdditive = 1 << 0;
        /// <sumamry>
        /// Multiplicative blend mode bit.
        /// </sumamry>
        public const Byte BlendMultiplicative = 1 << 1;
        /// <sumamry>
        /// Double-sidedness bit.
        /// </sumamry>
        public const Byte IsDoubleSided = 1 << 2;
        /// <sumamry>
        /// Clipping mask inversion mode mask.
        /// </sumamry>
        public const Byte IsInvertedMask = 1 << 3;


        /// <sumamry>
        /// Bit set when visible.
        /// </sumamry>
        public const Byte IsVisible = 1 << 0;
        /// <sumamry>
        /// Bit set when visibility did change.
        /// </sumamry>
        public const Byte VisibilityDidChange = 1 << 1;
        /// <sumamry>
        /// Bit set when opacity did change.
        /// </sumamry>
        public const Byte OpacityDidChange = 1 << 2;
        /// <sumamry>
        /// Bit set when draw order did change.
        /// </sumamry>
        public const Byte DrawOrderDidChange = 1 << 3;
        /// <sumamry>
        /// Bit set when render order did change.
        /// </sumamry>
        public const Byte RenderOrderDidChange = 1 << 4;
        /// <sumamry>
        /// Flag set when vertex positions did change.
        /// </sumamry>
        public const Byte VertexPositionsDidChange = 1 << 5;
        /// <sumamry>
        /// Flag set when blend color did change.
        /// </sumamry>
        public const Byte BlendColorDidChange = 1 << 6;


        /// <summary>
        /// Queries Core version.
        /// </summary>
        [DllImport(DllName, EntryPoint = "csmGetVersion")]
        public static extern uint GetVersion();
        /// <summary>
        /// Gets Moc file supported latest version.
        /// </summary>
        [DllImport(DllName, EntryPoint = "csmGetLatestMocVersion")]
        public static extern uint GetLatestMocVersion();
        /// <summary>
        /// Moc file format version.
        /// </summary>
        [DllImport(DllName, EntryPoint = "csmGetMocVersion")]
        public static extern uint GetMocVersion(IntPtr moc, uint mocSize);
        /// <summary>
        /// Sets log handler.
        /// </summary>
        [DllImport(DllName, EntryPoint = "csmSetLogFunction")]
        public static extern void SetLogFunction(uint handler);
        /// <summary>
        /// Gets Size of model instance (in bytes).
        /// </summary>
        [DllImport(DllName, EntryPoint = "csmGetSizeofModel")]
        public static extern uint GetSizeofModel(IntPtr moc);
        /// <summary>
        /// Revives moc in place.
        /// </summary>
        [DllImport(DllName, EntryPoint = "csmReviveMocInPlace")]
        public static extern IntPtr ReviveMocInPlace(IntPtr memory, uint mocSize);
        /// <summary>
        /// Instantiates moc in place.
        /// </summary>
        [DllImport(DllName, EntryPoint = "csmInitializeModelInPlace")]
        public static extern IntPtr InitializeModelInPlace(IntPtr moc, IntPtr memory, uint modelSize);
        /// <summary>
        /// Checks consistency of a moc.
        /// </summary>
        [DllImport(DllName, EntryPoint = "csmHasMocConsistency")]
        public static extern int HasMocConsistency(IntPtr memory, uint mocSize);
        /// <summary>
        /// Updates model.
        /// </summary>
        [DllImport(DllName, EntryPoint = "csmUpdateModel")]
        public static extern void UpdateModel(IntPtr model);
        /// <summary>
        /// Reads info on a model canvas.
        /// </summary>
        [DllImport(DllName, EntryPoint = "csmReadCanvasInfo")]
        public static extern void ReadCanvasInfo(IntPtr model, IntPtr outSizeInPixels, IntPtr outOriginInPixels, IntPtr outPixelsPerUnit);
        /// <summary>
        /// Gets parameter count.
        /// </summary>
        [DllImport(DllName, EntryPoint = "csmGetParameterCount")]
        public static extern int GetParameterCount(IntPtr model);
        /// <summary>
        /// Gets parameter IDs.
        /// </summary>
        [DllImport(DllName, EntryPoint = "csmGetParameterIds")]
        public static extern unsafe char ** GetParameterIds(IntPtr model);
        /// <summary>
        /// Gets minimum parameter values.
        /// </summary>
        [DllImport(DllName, EntryPoint = "csmGetParameterMinimumValues")]
        public static extern unsafe float* GetParameterMinimumValues(IntPtr model);
        /// <summary>
        /// Gets Parameter types.
        /// </summary>
        [DllImport(DllName, EntryPoint = "csmGetParameterTypes")]
        public static extern unsafe int* GetParameterTypes(IntPtr model);
        /// <summary>
        /// Gets maximum parameter values.
        /// </summary>
        [DllImport(DllName, EntryPoint = "csmGetParameterMaximumValues")]
        public static extern unsafe float* GetParameterMaximumValues(IntPtr model);
        /// <summary>
        /// Gets default parameter values.
        /// </summary>
        [DllImport(DllName, EntryPoint = "csmGetParameterDefaultValues")]
        public static extern unsafe float* GetParameterDefaultValues(IntPtr model);
        /// <summary>
        /// Gets parameter values.
        /// </summary>
        [DllImport(DllName, EntryPoint = "csmGetParameterValues")]
        public static extern unsafe float* GetParameterValues(IntPtr model);
        /// <summary>
        /// Gets number of key values of each parameter.
        /// </summary>
        [DllImport(DllName, EntryPoint = "csmGetParameterKeyCounts")]
        public static extern unsafe int* GetParameterKeyCounts(IntPtr model);
        /// <summary>
        /// Gets key values of each parameter.
        /// </summary>
        [DllImport(DllName, EntryPoint = "csmGetParameterKeyValues")]
        public static extern unsafe float** GetParameterKeyValues(IntPtr model);
        /// <summary>
        /// Gets part count.
        /// </summary>
        [DllImport(DllName, EntryPoint = "csmGetPartCount")]
        public static extern int GetPartCount(IntPtr model);
        /// <summary>
        /// Gets part IDs.
        /// </summary>
        [DllImport(DllName, EntryPoint = "csmGetPartIds")]
        public static extern unsafe char ** GetPartIds(IntPtr model);
        /// <summary>
        /// Gets opacity values.
        /// </summary>
        [DllImport(DllName, EntryPoint = "csmGetPartOpacities")]
        public static extern unsafe float* GetPartOpacities(IntPtr model);
        /// <summary>
        /// Gets part's parent part indices.
        /// </summary>
        [DllImport(DllName, EntryPoint = "csmGetPartParentPartIndices")]
        public static extern unsafe int* GetPartParentPartIndices(IntPtr model);
        /// <summary>
        /// Gets drawable count.
        /// </summary>
        [DllImport(DllName, EntryPoint = "csmGetDrawableCount")]
        public static extern int GetDrawableCount(IntPtr model);
        /// <summary>
        /// Gets drawable IDs.
        /// </summary>
        [DllImport(DllName, EntryPoint = "csmGetDrawableIds")]
        public static extern unsafe char ** GetDrawableIds(IntPtr model);
        /// <summary>
        /// Gets constant drawable flags.
        /// </summary>
        [DllImport(DllName, EntryPoint = "csmGetDrawableConstantFlags")]
        public static extern unsafe Byte* GetDrawableConstantFlags(IntPtr model);
        /// <summary>
        /// Gets dynamic drawable flags.
        /// </summary>
        [DllImport(DllName, EntryPoint = "csmGetDrawableDynamicFlags")]
        public static extern unsafe Byte* GetDrawableDynamicFlags(IntPtr model);
        /// <summary>
        /// Gets drawable texture indices.
        /// </summary>
        [DllImport(DllName, EntryPoint = "csmGetDrawableTextureIndices")]
        public static extern unsafe int* GetDrawableTextureIndices(IntPtr model);
        /// <summary>
        /// Gets drawable draw orders.
        /// </summary>
        [DllImport(DllName, EntryPoint = "csmGetDrawableDrawOrders")]
        public static extern unsafe int* GetDrawableDrawOrders(IntPtr model);
        /// <summary>
        /// Gets drawable render orders.
        /// </summary>
        [DllImport(DllName, EntryPoint = "csmGetDrawableRenderOrders")]
        public static extern unsafe int* GetDrawableRenderOrders(IntPtr model);
        /// <summary>
        /// Gets drawable opacities.
        /// </summary>
        [DllImport(DllName, EntryPoint = "csmGetDrawableOpacities")]
        public static extern unsafe float* GetDrawableOpacities(IntPtr model);
        /// <summary>
        /// Gets mask count for each drawable.
        /// </summary>
        [DllImport(DllName, EntryPoint = "csmGetDrawableMaskCounts")]
        public static extern unsafe int* GetDrawableMaskCounts(IntPtr model);
        /// <summary>
        /// Gets masks for each drawable.
        /// </summary>
        [DllImport(DllName, EntryPoint = "csmGetDrawableMasks")]
        public static extern unsafe int** GetDrawableMasks(IntPtr model);
        /// <summary>
        /// Gets number of vertices of each drawable.
        /// </summary>
        [DllImport(DllName, EntryPoint = "csmGetDrawableVertexCounts")]
        public static extern unsafe int* GetDrawableVertexCounts(IntPtr model);
        /// <summary>
        /// Gets 2D vertex position data of each drawable.
        /// </summary>
        [DllImport(DllName, EntryPoint = "csmGetDrawableVertexPositions")]
        public static extern unsafe float** GetDrawableVertexPositions(IntPtr model);
        /// <summary>
        /// Gets 2D texture coordinate data of each drawables.
        /// </summary>
        [DllImport(DllName, EntryPoint = "csmGetDrawableVertexUvs")]
        public static extern unsafe float** GetDrawableVertexUvs(IntPtr model);
        /// <summary>
        /// Gets number of triangle indices for each drawable.
        /// </summary>
        [DllImport(DllName, EntryPoint = "csmGetDrawableIndexCounts")]
        public static extern unsafe int* GetDrawableIndexCounts(IntPtr model);
        /// <summary>
        /// Gets triangle index data for each drawable.
        /// </summary>
        [DllImport(DllName, EntryPoint = "csmGetDrawableIndices")]
        public static extern unsafe ushort** GetDrawableIndices(IntPtr model);
        /// <summary>
        /// Resets all dynamic drawable flags.
        /// </summary>
        [DllImport(DllName, EntryPoint = "csmResetDrawableDynamicFlags")]
        public static extern void ResetDrawableDynamicFlags(IntPtr model);
        /// <summary>
        /// Gets information multiply color.
        /// </summary>
        [DllImport(DllName, EntryPoint = "csmGetDrawableMultiplyColors")]
        public static extern unsafe float* GetDrawableMultiplyColors(IntPtr model);
        /// <summary>
        /// Gets information Screen color.
        /// </summary>
        [DllImport(DllName, EntryPoint = "csmGetDrawableScreenColors")]
        public static extern unsafe float* GetDrawableScreenColors(IntPtr model);
        /// <summary>
        /// Gets Indices of drawables parent part.
        /// </summary>
        [DllImport(DllName, EntryPoint = "csmGetDrawableParentPartIndices")]
        public static extern unsafe int* GetDrawableParentPartIndices(IntPtr model);
    }
}
