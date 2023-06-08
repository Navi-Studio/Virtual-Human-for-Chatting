using System;
using System.Runtime.InteropServices;

#pragma warning disable 1591

namespace OpenCvSharp
{
	static partial class NativeMethods
	{
		[DllImport(DllExtern, CallingConvention = CallingConvention.Cdecl)]
		public static extern void text_computeNMChannels(IntPtr img, out IntPtr channels, int mode);

		[DllImport(DllExtern, CallingConvention = CallingConvention.Cdecl)]
		public static extern void text_MSERsToERStats(IntPtr image, IntPtr contours, IntPtr region);

		[DllImport(DllExtern, CallingConvention = CallingConvention.Cdecl, ThrowOnUnmappableChar = true)]
		public static extern void text_createOCRHMMTransitionsTable([MarshalAs(UnmanagedType.LPStr)] string vocabulary, IntPtr lexicon, IntPtr transition_probabilities_table);

		[DllImport(DllExtern, CallingConvention = CallingConvention.Cdecl)]
		public static extern void text_detectRegions(IntPtr image, IntPtr er_filter1, IntPtr er_filter2, IntPtr regions);
		
		[DllImport(DllExtern, CallingConvention = CallingConvention.Cdecl, ThrowOnUnmappableChar = true)]
		public static extern void text_erGrouping1(IntPtr img, IntPtr channels, IntPtr regions, IntPtr groups, IntPtr groups_rects, int method, [MarshalAs(UnmanagedType.LPStr)] string filename, float minProbablity);

		[DllImport(DllExtern, CallingConvention = CallingConvention.Cdecl, ThrowOnUnmappableChar = true)]
		public static extern void text_erGrouping2(IntPtr image, IntPtr channel, IntPtr regions, IntPtr groups_rects, int method, [MarshalAs(UnmanagedType.LPStr)] string filename, float minProbablity);

		//
		// ERStat
		//
		[DllImport(DllExtern, CallingConvention = CallingConvention.Cdecl)]
		public static extern IntPtr text_ERStat_new1();

		[DllImport(DllExtern, CallingConvention = CallingConvention.Cdecl)]
		public static extern IntPtr text_ERStat_new2(IntPtr obj);

		[DllImport(DllExtern, CallingConvention = CallingConvention.Cdecl)]
		public static extern void text_ERStat_getRect(IntPtr obj, ref Rect output);

		[DllImport(DllExtern, CallingConvention = CallingConvention.Cdecl)]
		public static extern double text_ERStat_getProbability(IntPtr obj);

		[DllImport(DllExtern, CallingConvention = CallingConvention.Cdecl)]
		public static extern void text_ERStat_delete(IntPtr obj);

		// 
		// OCRHMMDecoder::ClassifierCallback
		//
		[DllImport(DllExtern, CallingConvention = CallingConvention.Cdecl, ThrowOnUnmappableChar = true)]
		public static extern IntPtr text_loadOCRHMMClassifierCNN([MarshalAs(UnmanagedType.LPStr)] string filename);

		[DllImport(DllExtern, CallingConvention = CallingConvention.Cdecl, ThrowOnUnmappableChar = true)]
		public static extern IntPtr text_loadOCRHMMClassifierNM([MarshalAs(UnmanagedType.LPStr)] string filename);

		[DllImport(DllExtern, CallingConvention = CallingConvention.Cdecl)]
		public static extern void text_OCRHMMDecoder_ClassifierCallback_delete(IntPtr obj);

		[DllImport(DllExtern, CallingConvention = CallingConvention.Cdecl)]
		public static extern void text_OCRHMMDecoder_ClassifierCallback_eval(IntPtr obj, IntPtr image, IntPtr vecIntClass, IntPtr vecDoubleConfidence);

		//
		// OCRHMMDecoder
		//
		[DllImport(DllExtern, CallingConvention = CallingConvention.Cdecl, ThrowOnUnmappableChar = true)]
		public static extern IntPtr text_OCRHMMDecoder_create(IntPtr classifier, [MarshalAs(UnmanagedType.LPStr)] string vocabulary, IntPtr transition_probabilities, IntPtr emission_probabilities, int mode);

		[DllImport(DllExtern, CallingConvention = CallingConvention.Cdecl)]
		public static extern void text_OCRHMMDecoder_delete(IntPtr obj);

		[DllImport(DllExtern, CallingConvention = CallingConvention.Cdecl)]
		public static extern void text_OCRHMMDecoder_run(IntPtr decoder, IntPtr image, IntPtr rects, IntPtr texts, IntPtr confidences, int component_level);

		//
		// OCRBeamSearchDecoder::ClassifierCallback
		//
		[DllImport(DllExtern, CallingConvention = CallingConvention.Cdecl, ThrowOnUnmappableChar = true)]
		public static extern IntPtr text_loadOCRBeamSearchClassifierCNN([MarshalAs(UnmanagedType.LPStr)] string fileName);

		[DllImport(DllExtern, CallingConvention = CallingConvention.Cdecl)]
		public static extern void text_OCRBeamSearchDecoder_ClassifierCallback_delete(IntPtr obj);

		[DllImport(DllExtern, CallingConvention = CallingConvention.Cdecl)]
		public static extern void text_OCRBeamSearchDecoder_ClassifierCallback_eval(IntPtr obj, IntPtr input, IntPtr recognition_probabilities, IntPtr oversegmentation);

		//
		// OCRBeamSearchDecoder
		//
		[DllImport(DllExtern, CallingConvention = CallingConvention.Cdecl, ThrowOnUnmappableChar = true)]
		public static extern IntPtr text_OCRBeamSearchDecoder_create(IntPtr classifier, [MarshalAs(UnmanagedType.LPStr)] string vocabulary, IntPtr transition_probabilities_table, IntPtr emission_probabilities_table, int mode, int beam_size);

		[DllImport(DllExtern, CallingConvention = CallingConvention.Cdecl)]
		public static extern void text_OCRBeamSearchDecoder_delete(IntPtr obj);

		[DllImport(DllExtern, CallingConvention = CallingConvention.Cdecl)]
		public static extern void text_OCRBeamSearchDecoder_run(IntPtr decoder, IntPtr image, IntPtr rects, IntPtr texts, IntPtr confidences, int component_level);

		[DllImport(DllExtern, CallingConvention = CallingConvention.Cdecl)]
		public static extern void text_OCRBeamSearchDecoder_run2(IntPtr decoder, IntPtr image, IntPtr mask, IntPtr rects, IntPtr texts, IntPtr confidences, int component_level);

		//
		// ERFilter::Callback
		//
		[DllImport(DllExtern, CallingConvention = CallingConvention.Cdecl)]
		public static extern double text_ERFilter_Callback_eval(IntPtr obj, IntPtr stat);

		[DllImport(DllExtern, CallingConvention = CallingConvention.Cdecl)]
		public static extern void text_ERFilter_Callback_delete(IntPtr obj);

		[DllImport(DllExtern, CallingConvention = CallingConvention.Cdecl, ThrowOnUnmappableChar = true)]
		public static extern IntPtr text_loadClassifierNM1([MarshalAs(UnmanagedType.LPStr)] string filename);

		[DllImport(DllExtern, CallingConvention = CallingConvention.Cdecl, ThrowOnUnmappableChar = true)]
		public static extern IntPtr text_loadClassifierNM2([MarshalAs(UnmanagedType.LPStr)] string filename);

		//
		// ERFilter
		//
		[DllImport(DllExtern, CallingConvention = CallingConvention.Cdecl)]
		public static extern IntPtr text_createERFilterNM1(IntPtr cb, int thresholdDelta, float minArea, float maxArea, float minProbability, [MarshalAs(UnmanagedType.U1)] bool nonMaxSuppression, float minProbabilityDiff);

		[DllImport(DllExtern, CallingConvention = CallingConvention.Cdecl)]
		public static extern IntPtr text_createERFilterNM2(IntPtr cb, float minProbability);

		[DllImport(DllExtern, CallingConvention = CallingConvention.Cdecl)]
		public static extern void text_ERFilter_run(IntPtr obj, IntPtr image, IntPtr regions);

		[DllImport(DllExtern, CallingConvention = CallingConvention.Cdecl)]
		public static extern void text_ERFilter_setCallback(IntPtr obj, IntPtr cb);

		[DllImport(DllExtern, CallingConvention = CallingConvention.Cdecl)]
		public static extern void text_ERFilter_setThresholdDelta(IntPtr obj, int thresholdDelta);

		[DllImport(DllExtern, CallingConvention = CallingConvention.Cdecl)]
		public static extern void text_ERFilter_setMinArea(IntPtr obj, float minArea);

		[DllImport(DllExtern, CallingConvention = CallingConvention.Cdecl)]
		public static extern void text_ERFilter_setMaxArea(IntPtr obj, float maxArea);

		[DllImport(DllExtern, CallingConvention = CallingConvention.Cdecl)]
		public static extern void text_ERFilter_setMinProbability(IntPtr obj, float minProbability);

		[DllImport(DllExtern, CallingConvention = CallingConvention.Cdecl)]
		public static extern void text_ERFilter_setMinProbabilityDiff(IntPtr obj, float minProbabilityDiff);

		[DllImport(DllExtern, CallingConvention = CallingConvention.Cdecl)]
		public static extern void text_ERFilter_setNonMaxSuppression(IntPtr obj, bool nonMaxSuppression);

		[DllImport(DllExtern, CallingConvention = CallingConvention.Cdecl)]
		public static extern int text_ERFilter_getNumRejected(IntPtr obj);
	}
}