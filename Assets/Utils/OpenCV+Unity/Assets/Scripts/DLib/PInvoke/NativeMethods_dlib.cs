using System;
using System.Runtime.InteropServices;

namespace OpenCvSharp
{
	static partial class NativeMethods
	{
		[DllImport(DllExtern, CallingConvention = CallingConvention.Cdecl)]
		public static extern IntPtr dlib_shapePredictor_new();

		[DllImport(DllExtern, CallingConvention = CallingConvention.Cdecl)]
		public static extern void dlib_shapePredictor_loadData(IntPtr predictor, Byte[] data, int dataSize);

		[DllImport(DllExtern, CallingConvention = CallingConvention.Cdecl)]
		[return: MarshalAs(UnmanagedType.I1)]
		public static extern bool dlib_shapePredictor_detectLandmarks(IntPtr predictor, IntPtr image, Rect roi, ref IntPtr landmarks);

		[DllImport(DllExtern, CallingConvention = CallingConvention.Cdecl)]
		public static extern void dlib_shapePredictor_delete(IntPtr instance);
	}
}