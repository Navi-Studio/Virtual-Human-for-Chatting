using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using OpenCvSharp.Detail;
using OpenCvSharp.Util;

namespace OpenCvSharp
{
	/// <summary>
	/// Extremal Region structure
	/// </summary>
	public class ERStat : DisposableCvObject
	{
		/// <summary>
		/// Default C-tor
		/// </summary>
		public ERStat()
		{
			ptr = NativeMethods.text_ERStat_new1();
		}

		/// <summary>
		/// Copy-constructs ERStat with native pointer
		/// </summary>
		/// <param name="refPtr"></param>
		public ERStat(IntPtr refPtr)
		{
			ptr = NativeMethods.text_ERStat_new2(refPtr);
		}

		/// <summary>
		/// Copy-constructs ERStat with ref object
		/// </summary>
		/// <param name="refObj"></param>
		public ERStat(ERStat refObj)
			: this(refObj.CvPtr)
		{}

		/// <summary>
		/// Frees unmanaged object
		/// </summary>
		protected override void DisposeUnmanaged()
		{
			if (ptr != IntPtr.Zero)
			{
				NativeMethods.text_ERStat_delete(ptr);
				ptr = IntPtr.Zero;
			}
		}

		/// <summary>
		/// Rect
		/// </summary>
		public Rect Rect
		{
			get
			{
				Rect result = new Rect();
				NativeMethods.text_ERStat_getRect(ptr, ref result);
				return result;
			}
		}

		/// <summary>
		/// Detection confidence
		/// </summary>
		public double Probability
		{
			get { return NativeMethods.text_ERStat_getProbability(ptr); }
		}
	}

	/// <summary>
	/// Extremal Region Filter
	/// </summary>
	public class ERFilter : OpenCvSharp.Algorithm
	{
		public enum Mode
		{
			NM_RGBLGrad,
			NM_IHSGrad
		};

		/// <summary>
		/// ERFilter::Callback
		/// </summary>
		public class Callback : DisposableCvObject
		{
			internal Callback(IntPtr obj)
			{
				ptr = obj;
			}

			public double Eval(ERStat stat)
			{
				return NativeMethods.text_ERFilter_Callback_eval(ptr, stat.CvPtr);
			}

			protected override void DisposeUnmanaged()
			{
				if (ptr != IntPtr.Zero)
				{
					NativeMethods.text_ERFilter_Callback_delete(ptr);
					ptr = IntPtr.Zero;
				}
			}
		};

		protected object[] preserved = null;

		/// <summary>
		/// Constructs ERFilter with native object
		/// </summary>
		/// <param name="obj"></param>
		internal ERFilter(IntPtr obj, object callback)
		{
			ptr = obj;
			preserved = new object[] { callback };
		}

		/// <summary>
		/// Takes image on input and returns the selected regions in a vector of ERStat only distinctive ERs which correspond to characters are selected by a sequential classifier
		/// </summary>
		/// <param name="image">Single channel image CV_8UC1</param>
		/// <param name="regions">Output for the 1st stage and Input/Output for the 2nd. The selected Extremal Regions are stored here.</param>
		public ERStat[] Run(InputArray image, ERStat[] regions = null)
		{
			using (var vecStats = new VectorOfERStat(regions))
			{
				NativeMethods.text_ERFilter_run(ptr, image.CvPtr, vecStats.CvPtr);
				return vecStats.ToArray();
			}
		}

		public int NumRejected
		{
			get { return NativeMethods.text_ERFilter_getNumRejected(ptr); }
		}
		
		public void SetCallback(Callback cb)
		{
			NativeMethods.text_ERFilter_setCallback(ptr, cb.CvPtr);
		}

		public void SetThresholdDelta(int thresholdDelta)
		{
			NativeMethods.text_ERFilter_setThresholdDelta(ptr, thresholdDelta);
		}

		public void SetMinArea(float minArea)
		{
			NativeMethods.text_ERFilter_setMinArea(ptr, minArea);
		}

		public void SetMaxArea(float maxArea)
		{
			NativeMethods.text_ERFilter_setMaxArea(ptr, maxArea);
		}

		public void SetMinProbability(float minProbability)
		{
			NativeMethods.text_ERFilter_setMinProbability(ptr, minProbability);
		}

		public void SetMinProbabilityDiff(float minProbabilityDiff)
		{
			NativeMethods.text_ERFilter_setMinProbabilityDiff(ptr, minProbabilityDiff);
		}

		public void SetNonMaxSuppression(bool nonMaxSuppression)
		{
			NativeMethods.text_ERFilter_setNonMaxSuppression(ptr, nonMaxSuppression);
		}
	}

	/// <summary>
	/// General text methods 
	/// </summary>
	static public partial class CvText
	{
		/// <summary>
		/// erGrouping modes
		/// </summary>
		public enum GroupingModes
		{
			OrientationHorizontal,
			OrientationAny
		};

		/// <summary>
		/// Converts MSER contours (vector<Point>) to ERStat regions.
		/// </summary>
		/// <param name="image">Source image CV_8UC1 from which the MSERs where extracted.</param>
		/// <param name="contours">Input vector with all the contours</param>
		/// <param name="regions">Output where the ERStat regions are stored</param>
		public static void MSERsToERStats(InputArray image, Point[][] contours, out ERStat[][] regions)
		{
			using (var vecRegions = new VectorOfVectorERStat())
			using (var vecContours = new VectorOfVectorPoint(contours))
			{
				NativeMethods.text_MSERsToERStats(image.CvPtr, vecContours.CvPtr, vecRegions.CvPtr);

				regions = vecRegions.ToArray();
			}
		}

		/// <summary>
		/// Compute the different channels to be processed independently in the N&M algorithm [Neumann12]
		/// </summary>
		/// <param name="src">The source image</param>
		/// <returns>Channels</returns>
		public static Mat[] ComputeNMChannels(Mat src, ERFilter.Mode mode)
		{
			if (src == null)
				throw new ArgumentNullException("nameof(src)");
			src.ThrowIfDisposed();

			IntPtr mvPtr;
			NativeMethods.text_computeNMChannels(src.CvPtr, out mvPtr, (int)mode);

			using (var vec = new VectorOfMat(mvPtr))
			{
				return vec.ToArray();
			}
		}

		/// <summary>
		/// Creates a tailored language model transitions table from a given list of words (lexicon)
		/// </summary>
		/// <param name="vocabulary">The language vocabulary (chars when ASCII English text)</param>
		/// <param name="lexicon">The list of words that are expected to be found in a particular image</param>
		/// <returns>Table with transition probabilities between character pairs</returns>
		public static Mat CreateOCRHMMTransitionsTable(string vocabulary, IList<string> lexicon)
		{
			Mat result = new Mat();
			using (var transition_t = new OutputArray(result))
			using (var vecLex = new VectorOfString(lexicon))
			{
				NativeMethods.text_createOCRHMMTransitionsTable(vocabulary, vecLex.CvPtr, transition_t.CvPtr);
			}
			return result;
		}

		/// <summary>
		/// Extracts text regions from image
		/// </summary>
		/// <param name="image">Source image where text blocks needs to be extracted from. Should be CV_8UC3 (color)</param>
		/// <param name="er_filter1">Extremal Region Filter for the 1st stage classifier of N&M algorithm [Neumann12]</param>
		/// <param name="er_filter2">Extremal Region Filter for the 2nd stage classifier of N&M algorithm [Neumann12]</param>
		/// <param name="regions">Output list of regions with text</param>
		public static void DetectRegions(InputArray image, ERFilter er_filter1, ERFilter er_filter2, out Point[][] regions)
		{
			using (var vecRegions = new VectorOfVectorPoint())
			{
				NativeMethods.text_detectRegions(image.CvPtr, er_filter1.CvPtr, er_filter2.CvPtr, vecRegions.CvPtr);
				regions = vecRegions.ToArray();
			}
		}

		/// <summary>
		/// Find groups of Extremal Regions that are organized as text blocks
		/// </summary>
		/// <param name="image">Original RGB or Greyscale image from wich the regions were extracted</param>
		/// <param name="channels">Vector of single channel images CV_8UC1 from wich the regions were extracted</param>
		/// <param name="regions">Vector of ER's retrieved from the ERFilter algorithm from each channel</param>
		/// <param name="groups">The output of the algorithm is stored in this parameter as set of lists of indexes to provided regions</param>
		/// <param name="groups_rects">The output of the algorithm are stored in this parameter as list of rectangles</param>
		/// <param name="method">Grouping method (see GroupingModes). Can be one of { OrientationHorizontal, OrientationAny }</param>
		/// <param name="filename">The XML or YAML file with the classifier model. Only to use when grouping method is OrientationAny</param>
		/// <param name="minProbablity">The minimum probability for accepting a group. Only to use when grouping method is OrientationAny</param>
		public static void ErGrouping(Mat image, Mat[] channels, ERStat[][] regions, out Vec2i[][] groups, out Rect[] groups_rects, GroupingModes method, string filename = null, float minProbablity = 0.5f)
		{
			using (var vecChannels = new InputArray(channels))
			using (var vecRegions = new VectorOfVectorERStat(regions))
			using (var vecGroups = new VectorOfVectorVec2i())
			using (var vecRects = new VectorOfRect())
			using (var input = new InputArray(image))
			{
				if (null == filename)
					filename = string.Empty;
				NativeMethods.text_erGrouping1(input.CvPtr, vecChannels.CvPtr, vecRegions.CvPtr, vecGroups.CvPtr, vecRects.CvPtr, (int)method, filename, minProbablity);

				groups = vecGroups.ToArray();
				groups_rects = vecRects.ToArray();
			}
		}

		/// <summary>
		/// This one has no documentation in OpenCV sources or knowledge base
		/// </summary>
		/// <param name="image">Original RGB or Greyscale image from wich the regions were extracted</param>
		/// <param name="channel">Vector of single channel images CV_8UC1 from wich the regions were extracted</param>
		/// <param name="regions">Regions extracted by DetectRegions function</param>
		/// <param name="groups_rects">The output of the algorithm are stored in this parameter as list of rectangles</param>
		/// <param name="method">Grouping method (see GroupingModes). Can be one of { OrientationHorizontal, OrientationAny }</param>
		/// <param name="filename">The XML or YAML file with the classifier model. Only to use when grouping method is OrientationAny</param>
		/// <param name="minProbablity">The minimum probability for accepting a group. Only to use when grouping method is OrientationAny</param>
		public static void ErGrouping(Mat image, Mat[] channels, Point[][] regions, out Rect[] groups_rects, GroupingModes method, string filename = null, float minProbablity = 0.5f)
		{
			using (var vecChannels = new InputArray(channels))
			using (var vecRegions = new VectorOfVectorPoint(regions))
			using (var vecRects = new VectorOfRect())
			using (var input = new InputArray(image))
			{
				if (null == filename)
					filename = string.Empty;
				NativeMethods.text_erGrouping2(input.CvPtr, vecChannels.CvPtr, vecRegions.CvPtr, vecRects.CvPtr, (int)method, filename, minProbablity);

				groups_rects = vecRects.ToArray();
			}
		}

		/// <summary>
		/// Loads ERFilter NM1 classifier
		/// </summary>
		/// <param name="fileName">Path to the classifier</param>
		/// <returns>Loaded classifier</returns>
		public static ERFilter.Callback LoadClassifierNM1(string fileName)
		{
			return new ERFilter.Callback(NativeMethods.text_loadClassifierNM1(fileName));
		}

		/// <summary>
		/// Loads ERFilter NM2 classifier
		/// </summary>
		/// <param name="fileName">Path to the classifier</param>
		/// <returns>Loaded classifier</returns>
		public static ERFilter.Callback LoadClassifierNM2(string fileName)
		{
			return new ERFilter.Callback(NativeMethods.text_loadClassifierNM2(fileName));
		}

		/// <summary>
		/// Creates an Extremal Region Filter for the 1st stage classifier of N&M algorithm [Neumann12].
		/// </summary>
		/// <param name="cb">Callback with the classifier. Default classifier can be implicitly load with function loadClassifierNM1()</param>
		/// <param name="thresholdDelta">Threshold step in subsequent thresholds when extracting the component tree</param>
		/// <param name="minArea">The minimum area (% of image size) allowed for retreived ER’s</param>
		/// <param name="maxArea">The maximum area (% of image size) allowed for retreived ER’s</param>
		/// <param name="minProbability">The minimum probability P(er|character) allowed for retreived ER’s</param>
		/// <param name="nonMaxSuppression">Whenever non-maximum suppression is done over the branch probabilities</param>
		/// <param name="minProbabilityDiff">The minimum probability difference between local maxima and local minima ERs</param>
		/// <returns></returns>
		public static ERFilter CreateERFilterNM1(ERFilter.Callback cb, int thresholdDelta = 1, float minArea = 0.00025f, float maxArea = 0.13f, float minProbability = 0.4f, bool nonMaxSuppression = true, float minProbabilityDiff = 0.1f)
		{
			IntPtr obj = NativeMethods.text_createERFilterNM1(cb.CvPtr, thresholdDelta, minArea, maxArea, minProbability, nonMaxSuppression, minProbabilityDiff);
			return new ERFilter(obj, cb);
		}

		/// <summary>
		/// Create an Extremal Region Filter for the 2nd stage classifier of N&M algorithm
		/// </summary>
		/// <param name="cb">Callback with the classifier. Default classifier can be implicitly load with function loadClassifierNM2()</param>
		/// <param name="minProbability">The minimum probability per character allowed for retreived ER’s</param>
		/// <returns></returns>
		public static ERFilter CreateERFilterNM2(ERFilter.Callback cb, float minProbability = 0.3f)
		{
			IntPtr obj = NativeMethods.text_createERFilterNM2(cb.CvPtr, minProbability);
			return new ERFilter(obj, cb);
		}
	}
}