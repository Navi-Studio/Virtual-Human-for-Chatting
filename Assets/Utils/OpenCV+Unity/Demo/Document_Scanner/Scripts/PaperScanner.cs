namespace OpenCvSharp.Demo
{
	using System;
	using System.Collections.Generic;

	/// <summary>
	/// Array utilities
	/// </summary>
	static partial class ArrayUtilities
	{
		/// <summary>
		/// Checks whether array contains element
		/// </summary>
		/// <param name="array">Input array to check</param>
		/// <param name="obj">Element we're looking for</param>
		public static bool Contains<T>(this T[] array, T obj)
		{
			return ((IList<T>)array).Contains(obj);
		}

		/// <summary>
		/// Swaps two elements of the array
		/// </summary>
		/// <param name="array">Input array</param>
		/// <param name="i1">Firts element index</param>
		/// <param name="i2">Second element index</param>
		public static void Swap<T>(this T[] array, int i1, int i2)
		{
			T v = array[i2];
			array[i2] = array[i1];
			array[i1] = v;
		}

		/// <summary>
		/// Finds object closest to the given reference object in an array
		/// </summary>
		/// <param name="array">Array to search</param>
		/// <param name="referenceObject">Object to find closes element for</param>
		/// <param name="distanceMeasurer">Distance calculation function</param>
		/// <returns></returns>
		public static T ClosestElement<T>(this T[] array, T referenceObject, System.Func<T, T, double> distanceMeasurer)
		{
			if (array.Length == 0)
				throw new OpenCvSharpException("Array can not be empty.");

			// LINQ would work best here, but I read about issues with sorting (OrderBy) on Unity/iOS, so I'll pass and
			// make my own sorting
			T[] copy = (T[])array.Clone();
			System.Array.Sort(copy, (a, b) => distanceMeasurer(a, referenceObject).CompareTo(distanceMeasurer(b, referenceObject)));
			return copy[0];
		}
	}

	/// <summary>
	/// OpenCV Mat class extension with some handy methods
	/// </summary>
	static partial class MatUtilities
	{
		/// <summary>
		/// Computes median value for GRAY input image
		/// </summary>
		/// <param name="matGray">Mat with gray image</param>
		/// <param name="max">Max channel value, 256 by default</param>
		/// <returns>Image median</returns>
		public static int Median(this Mat matGray, int max = 256)
		{
			Mat hist = new Mat();
			int[] hdims = { max };
			Rangef[] ranges = { new Rangef(0, max), };
			Cv2.CalcHist(
				new Mat[] { matGray },
				new int[] { 0 },
				null,
				hist,
				1,
				hdims,
				ranges);

			int median = 0, sum = 0;
			int thresh = (int)matGray.Total() / 2;
			while (sum < thresh && median < max)
			{
				sum += (int)(hist.Get<float>(median));
				median++;
			}
			return median;
		}

		/// <summary>
		/// Calculates upper and lower threshold bound for a GRAY image
		/// </summary>
		/// <param name="matGray">Input gray image</param>
		/// <param name="lower">Output lower bound</param>
		/// <param name="upper">Output upper bound</param>
		/// <param name="sigma">Threshold strength param where [0, 1] means [tightest, widest] threshold</param>
		private static void CalculateThresholdBounds(Mat matGray, out int lower, out int upper, double sigma)
		{
			// prepare
			int median = matGray.Median();
			lower = (int)Math.Max(0.0, (1.0 - sigma) * median);
			upper = (int)Math.Min(255.0, (1.0 + sigma) * median);
		}

		/// <summary>
		/// Automatic edge detector, defines bounds on it's own
		/// Presumes the input is GRAY image
		/// </summary>
		/// <param name="matGray">Mat to detect edges on</param>
		/// <param name="sigma">Threshold strength param where [0, 1] means [tightest, widest] threshold</param>
		/// <returns>Mat with edges filtered</returns>
		public static Mat AdaptiveEdges(this Mat matGray, double sigma = 0.33)
		{
			int upper, lower;
			CalculateThresholdBounds(matGray, out lower, out upper, sigma);
			return matGray.Canny(lower, upper, 3, true);
		}

		/// <summary>
		/// Special threshold algorithm based on https://stackoverflow.com/questions/13391073/adaptive-threshold-of-blurry-image
		/// </summary>
		/// <param name="otsuRegions">Amount of splits for the image to perform Otsu threshold</param>
		/// <returns></returns>
		public static Mat MorphThreshold(this Mat matGray, int otsuRegions = 3)
		{
			Mat image = matGray.Clone();

			// Divide the image by its morphologically closed counterpart
			Mat kernel = Cv2.GetStructuringElement(MorphShapes.Ellipse, new Size(19, 19));
			Mat closed = new Mat();
			Cv2.MorphologyEx(image, closed, MorphTypes.Close, kernel);

			//image.ConvertTo(image, MatType.CV_32F);
			Cv2.Divide(image, closed, image);
			Cv2.Normalize(image, image, 0, 255, NormTypes.MinMax);
			//image.ConvertTo(image, MatType.CV_8UC1);

			// Threshold each block (3x3 grid) of the image separately to
			// correct for minor differences in contrast across the image
			int tw = image.Width / otsuRegions, th = image.Height / otsuRegions;
			for (int i = 0; i < otsuRegions; i++)
			{
				for (int j = 0; j < otsuRegions; j++)
				{
					Mat block = image.SubMat(i * th, (i + 1) * th, j * tw, (j + 1) * tw);
					Cv2.Threshold(block, block, -1, 255, ThresholdTypes.Binary | ThresholdTypes.Otsu);
				}
			}

			return image;
		}

		/// <summary>
		/// "Posterizes" image and returns new Mat with result. Implementation is color
		/// quantization with K-Means algorithm
		/// 
		/// Note: it's slow, don't use on big images
		/// </summary>
		/// <param name="colors">Desired output color count</param>
		/// <returns>New Mat with requested colors count</returns>
		public static Mat PosterizedImage(this Mat img, int colors)
		{
			// basics
			int attempts = 5;
			double eps = 0.01;
			TermCriteria criteria = new TermCriteria(CriteriaType.Eps | CriteriaType.MaxIter, attempts, eps);

			// prepare
			Mat labels = new Mat(), centers = new Mat();
			Mat samples = new Mat(img.Rows * img.Cols, 3, MatType.CV_32F);
			for (int y = 0; y < img.Rows; y++)
			{
				for (int x = 0; x < img.Cols; x++)
				{
					Vec3b color = img.At<Vec3b>(y, x);
					for (int z = 0; z < 3; z++)
						samples.Set<float>(y + x * img.Rows, z, color[z]);
				}
			}

			// run k-means
			Cv2.Kmeans(samples, colors, labels, criteria, attempts, KMeansFlags.PpCenters, centers);

			// restore original image
			Mat new_image = new Mat(img.Size(), img.Type());
			for (int y = 0; y < img.Rows; y++)
			{
				for (int x = 0; x < img.Cols; x++)
				{
					int cluster_idx = labels.At<int>(y + x * img.Rows, 0);
					Vec3b color = new Vec3b(
						(byte)centers.At<float>(cluster_idx, 0),
						(byte)centers.At<float>(cluster_idx, 1),
						(byte)centers.At<float>(cluster_idx, 2)
					);

					new_image.Set(y, x, color);
				}
			}

			return new_image;
		}

		/// <summary>
		/// Reduces color count in the image
		/// </summary>
		/// <param name="div">Output colors count</param>
		/// <returns></returns>
		public static unsafe Mat ColorReduced(this Mat source, int div = 64)
		{
			// prepare
			Mat image = source.Clone();
			
			// "manual" implementation
			for (int j = 0; j < image.Rows; j++)
			{
				// get the address of row j
				byte* b = (byte*)image.Ptr(j).ToPointer();

				for (int i = 0; i < image.Cols; i++)
				{
					for (int c = 0; c < image.Channels(); c++, b++)
						*b = (byte)(*b / div * div + div / 2);
				}
			}

			// this should run in parallel when possible
			//image.ForEachAsByte((byte* b, int* p) => *b = (byte)(*b / div * div + div / 2));

			// return
			return image;
		}

		/// <summary>
		/// Counts colors in the BGR image
		/// </summary>
		/// <param name="maxCount">Maximum colors to count, the lower this value is the better for performance</param>
		/// <returns>Colors count</returns>
		public static int CountUniqueColors(this Mat img, int maxCount)
		{
			var matGray = img.CvtColor(ColorConversionCodes.BGR2GRAY);

			// calculate histogram
			Mat hist = new Mat();
			int[] hdims = { maxCount };
			Rangef[] ranges = { new Rangef(0, 256) };
			Cv2.CalcHist( new Mat[] { matGray }, new int[] { 0 }, null, hist, 1, hdims, ranges);

			// scales and draws histogram
			return hist.CountNonZero();
		}

		/// <summary>
		/// Gets per-channel histograms for image and renders with given params
		/// Hint: It doesn't render anything fancy, the method is designed for debugging purposes and as an example
		/// </summary>
		/// <param name="bins">Desired histogram bins</param>
		/// <param name="outputSize">Rendered image size</param>
		/// <param name="background">Background color (bgr)</param>
		/// <param name="colors">Histograam color (bgr), one per source image channel</param>
		/// <param name="thickness">Histogram line thickness</param>
		/// <returns>Rendered histogram image</returns>
		public static Mat RenderedHistogram(this Mat img, int bins, Size outputSize, Scalar background, Scalar[] colors, int thickness)
		{
			Mat[] channels = img.Split();
			if (channels.Length != colors.Length)
				throw new OpenCvSharpException("colors array must have a color for each channel of the source image");

			// calculate histograms and render
			Mat hist = new Mat();
			int[] hdims = { bins };
			Rangef[] ranges = { new Rangef(0, 256) };
			Point[][] charts = new Point[channels.Length][];
			for (int c = 0; c < channels.Length; ++c)
			{
				// calculate histogram
				Cv2.CalcHist(new Mat[] { channels[c] }, new int[] { 0 }, null, hist, 1, hdims, ranges, true, false);

				// normalize
				Cv2.Normalize(hist, hist, 0, outputSize.Height, NormTypes.MinMax);

				// save
				List<Point> chart = new List<Point>();
				for (int j = 0; j < bins; ++j)
					chart.Add(new Point(j, outputSize.Height - hist.At<float>(j, 0)));
				charts[c] = chart.ToArray();
			}

			// render
			Mat output = new Mat(outputSize, MatType.CV_8UC3);
			output.SetTo(InputArray.Create(background));
			for (int c = 0; c < channels.Length; ++c)
				output.Polylines(new Point[][] { charts[c] }, false, colors[c], thickness);

			return output;
		}

		/// <summary>
		/// Extracts shape inside the given RotatedRect and un-warps it
		/// </summary>
		/// <param name="input">Input image</param>
		/// <param name="points">Sorted corners of the shape</param>
		/// <param name="corners">Downscaling option, biggest size with be re-scaled to this value. 0 means no downscaling (default value)</param>
		/// <returns>New OpenCV Mat with un-warped object</returns>
		public static Mat UnwrapShape(this Mat img, Point2f[] corners, int maxSize = 0)
		{
			if (corners.Length != 4)
				throw new OpenCvSharpException("argument 'points' must be of length = 4");

			// grab bounds corners, sort them in correct order and
			// get width/height of the shape
			float width = (float)Point2f.Distance(corners[0], corners[1]);  // lt -> rt
			float height = (float)Point2f.Distance(corners[0], corners[3]); // lt -> lb

			// downscaling
			if (maxSize > 0 && (width > maxSize || height > maxSize))
			{
				if (width > height)
				{
					var s = maxSize / width;
					width = maxSize;
					height = height * s;
				}
				else
				{
					var s = maxSize / height;
					height = maxSize;
					width = width * s;
				}
			}

			// compute transform
			Point2f[] destination = new Point2f[]
			{
				new Point2f(0,     0),
				new Point2f(width, 0),
				new Point2f(width, height),
				new Point2f(0,     height)
			};
			var transform = Cv2.GetPerspectiveTransform(corners, destination);

			// un-warp
			return img.WarpPerspective(transform, new Size(width, height), InterpolationFlags.Cubic);
		}
	}

	/// <summary>
	/// This class takes care about paper "scanning". It presumes the input is a picture
	/// of some document, i.e. there is one rectangular-shaped object on the image
	/// </summary>
	public class PaperScanner
	{
		private bool dirty_ = true;
		private Mat matInput_ = null;
		private Mat matOutput_ = null;
		private Point[] shape_ = null;

		#region Scanner settings
		/// <summary>
		/// Scanner settings
		/// </summary>
		public class ScannerSettings
		{
			/// <summary>
			/// Grayscaling algorithm
			/// </summary>
			public enum ColorMode
			{
				// Direct grayscale
				Grayscale,
				// Grayscale through Hue
				HueGrayscale
			}

			/// <summary>
			/// De-colorizer thresholding type
			/// </summary>
			public enum ScanType
			{
				Adaptive,
				Otsu
			}

			/// <summary>
			/// Document images look best in B&W with threshold applying, it makes them de-noised and sharp
			/// This enum defines how to process color for scanned paper
			/// </summary>
			public enum DecolorizationMode
			{
				Always,		// Always convert to B&W
				Never,		// Don't touch, live color as is
				Automatic,	// Scanner will guess whether we've scanned a document or a color object and apply/leave b&w based on it's guess
			}

			public delegate void NotifyDirty();

			private ColorMode colorMode_ = ColorMode.Grayscale;
			private double expectedArea_ = 0.33;
			private int scale_ = 512;
			private DecolorizationMode decolorization_ = DecolorizationMode.Automatic;
			private ScanType colorThreshold_ = ScanType.Adaptive;
			private double noiseReduction_ = 0.33;
			private double edgesTight_ = 0.75;
			private bool dropBadGuess_ = true;
			private NotifyDirty notifyDirty_ = null;

			/// <summary>
			/// Constructor
			/// </summary>
			/// <param name="observer">Delegate to notify about settings changes</param>
			public ScannerSettings(NotifyDirty notify)
			{
				notifyDirty_ = notify;
			}

			/// <summary>
			/// Color conversion mode, either Grayscale (standard RGB -> Gray conversion) or HueGray (RGB -> HSV and extract H channel) conversion
			/// HSV one might work really good for the cases where background and foreground colors are similar in lightness (i.e. will not be much different in gray)
			/// An example would be a white paper or light-yellow background
			/// Default value is regular Grayscale
			/// </summary>
			public ColorMode GrayMode
			{
				get { return colorMode_; }
				set
				{
					if (value != colorMode_)
					{
						colorMode_ = value;
						notifyDirty_();
					}
				}
			}

			/// <summary>
			/// Expected document area % from the whole picture. Detected shapes of a smaller size
			/// will be ignored unless no acceptable shape found, in that case smaller objects will
			/// be used to make a best guess for the whole structure
			/// By default is 33%
			/// </summary>
			public double ExpectedArea
			{
				get { return expectedArea_; }
				set
				{
					if (value != expectedArea_)
					{
						expectedArea_ = value;
						notifyDirty_();
					}
				}
			}

			/// <summary>
			/// Preprocessing scale, increases processing speed.
			/// 512 by default, put 0 to turn off
			/// </summary>
			public int Scale
			{
				get { return scale_; }
				set
				{
					if (value != scale_)
					{
						scale_ = value;
						notifyDirty_();
					}
				}
			}

			/// <summary>
			/// Scanned paper color transformation, see DecolorizationMode enum for more info
			/// </summary>
			public DecolorizationMode Decolorization
			{
				get { return decolorization_; }
				set
				{
					if (value != decolorization_)
					{
						decolorization_ = value;
						notifyDirty_();
					}
				}
			}

			/// <summary>
			/// Converter to B&W will use this algorithm for thresholding, it can be smart Adaptive or Otsu
			/// Adaptive is default one
			/// </summary>
			public ScanType ColorThreshold
			{
				get { return colorThreshold_; }
				set
				{
					if (value != colorThreshold_)
					{
						colorThreshold_ = value;
						notifyDirty_();
					}
				}
			}

			/// <summary>
			/// Noise reduction, 0 to turn off, 1 to apply max adaptive reduction
			/// Default value is 0.33
			/// </summary>
			public double NoiseReduction
			{
				get { return noiseReduction_; }
				set
				{
					if (value != noiseReduction_)
					{
						noiseReduction_ = value;
						notifyDirty_();
					}
				}
			}

			/// <summary>
			/// Value [0, 1] to define how wide -> tight should the edges be
			/// Default is 0.75
			/// </summary>
			public double EdgesTight
			{
				get { return edgesTight_; }
				set
				{
					if (value != edgesTight_)
					{
						edgesTight_ = value;
						notifyDirty_();
					}
				}
			}

			/// <summary>
			/// True to discard heuristically-guessed shape when it's area is far from one expected, False to return at least something in any case
			/// True is default
			/// </summary>
			public bool DropBadGuess
			{
				get { return dropBadGuess_; }
				set
				{
					if (value != dropBadGuess_)
					{
						dropBadGuess_ = true;
						notifyDirty_();
					}
				}
			}
		}
		#endregion

		#region Properties
		/// <summary>
		/// Input texture with presumable a pictured document, for simplicity it's
		/// assumed to be in BGR format
		/// </summary>
		public Mat Input
		{
			get { return matInput_; }
			set
			{
				if (matInput_ != value)
				{
					matInput_ = value;
					dirty_ = true;
				}
			}
		}

		/// <summary>
		/// Returns detected paper contour
		/// </summary>
		public Point[] PaperShape
		{
			get
			{
				if (dirty_)
					CalculateOutput();
				return shape_;
			}
		}

		/// <summary>
		/// Returns success code, true if a document was scanned, false otherwise
		/// </summary>
		public bool Success
		{
			get
			{
				if (dirty_)
					CalculateOutput();
				return PaperShape == null? false : PaperShape.Length == 4;
			}
		}

		/// <summary>
		/// Gets output texture with processed document
		/// </summary>
		public Mat Output
		{
			get
			{
				if (dirty_)
					CalculateOutput();
				return matOutput_;
			}
		}

		/// <summary>
		/// Scanner settings
		/// </summary>
		public ScannerSettings Settings
		{
			get; private set;
		}
		#endregion

		/// <summary>
		/// Constructor
		/// </summary>
		public PaperScanner()
		{
			Settings = new ScannerSettings(() => dirty_ = true);
		}

		/// <summary>
		/// Sorts corners as { left-top, right-top, right-bottom, left-bottom }
		/// </summary>
		/// <param name="corners">Input points</param>
		/// <returns>Sorted corners</returns>
		private Point[] SortCorners(Point[] corners)
		{
			if (corners.Length != 4)
				throw new OpenCvSharpException("\"corners\" must be an array of 4 elements");

			// divide vertically
			System.Array.Sort<Point>(corners, (a, b) => a.Y.CompareTo(b.Y));
			Point[] tops = new Point[] { corners[0], corners[1] }, bottoms = new Point[] { corners[2], corners[3] };

			// divide horizontally
			System.Array.Sort<Point>(corners, (a, b) => a.X.CompareTo(b.X));
			Point[] lefts = new Point[] { corners[0], corners[1] }, rights = new Point[] { corners[2], corners[3] };

			// fetch final array
			Point[] output = new Point[] {
				tops[0],
				tops[1],
				bottoms[0],
				bottoms[1]
			};
			if (!lefts.Contains(tops[0]))
				output.Swap(0, 1);
			if (!rights.Contains(bottoms[0]))
				output.Swap(2, 3);

			// done
			return output;
		}

		/// <summary>
		/// Takes candidate shape and combined hull and returns best match
		/// </summary>
		/// <param name="areaSize">Area size</param>
		/// <param name="candidates">Candidates</param>
		/// <param name="hull">Hull</param>
		/// <returns></returns>
		private Point[] GetBestMatchingContour(double areaSize, List<Point[]> candidates, Point[] hull)
		{
			Point[] result = hull;
			if (candidates.Count == 1)
				result = candidates[0];
			else if (candidates.Count > 1)
			{
				List<Point> keys = new List<Point>();
				foreach (var c in candidates)
					keys.AddRange(c);

				Point[] joinedCandidates = Cv2.ConvexHull(keys);
				Point[] joinedHull = Cv2.ApproxPolyDP(joinedCandidates, Cv2.ArcLength(joinedCandidates, true) * 0.01, true);
				result = joinedHull;
			}

			// check further
			if (Settings.DropBadGuess)
			{
				double area = Cv2.ContourArea(result);
				if (area / areaSize < Settings.ExpectedArea * 0.75)
					result = null;
			}

			return result;
		}

		/// <summary>
		/// Decides whether given image is "colored" or potentially can be fine represented by 2 colors
		/// </summary>
		/// <param name="image">Image to test</param>
		/// <param name="minColorsToPass">Minimum color count image might have to pass the test</param>
		/// <returns></returns>
		private bool IsColored(Mat image, int minColorsToPass = 12)
		{
			var matDownscaled = image.Resize(new Size(64, 64), 64.0 / image.Width, 64.0 / image.Height);

			// this one uses color quantization to reduce color count to 64, than it counts
			// actual color count and should it be too few - we probably have a document (homogeneous background + few colors in printing)
			int bins = 30;
			matDownscaled = matDownscaled.MedianBlur(9);
			var matPosterized = matDownscaled.ColorReduced(bins);// matDownscaled.PosterizedImage(bins);
			int colors = matPosterized.CountUniqueColors(bins);

			return (colors >= minColorsToPass);
		}

		/// <summary>
		/// The magic is here
		/// </summary>
		private void CalculateOutput()
		{
			Mat matGray = null;
			// instead of regular Grayscale, we use BGR -> HSV and take Hue channel as
			// source
			if (Settings.GrayMode == ScannerSettings.ColorMode.HueGrayscale)
			{
				var matHSV = matInput_.CvtColor(ColorConversionCodes.RGB2HSV);
				Mat[] hsvChannels = matHSV.Split();
				matGray = hsvChannels[0];
			}
			// Alternative: just plain BGR -> Grayscale
			else
			{
				matGray = matInput_.CvtColor(ColorConversionCodes.BGR2GRAY);
			}

			// scale down if necessary
			var matScaled = matGray;
			float sx = 1, sy = 1;
			if (Settings.Scale != 0)
			{
				if (matGray.Width > Settings.Scale)
					sx = (float)Settings.Scale / matGray.Width;
				if (matGray.Height > Settings.Scale)
					sy = (float)Settings.Scale / matGray.Height;

				matScaled = matGray.Resize(new Size(Math.Min(matGray.Width, Settings.Scale), Math.Min(matGray.Height, Settings.Scale)));
			}

			// reduce noise
			var matBlur = matScaled;
			if (Settings.NoiseReduction != 0)
			{
				int medianKernel = 11;

				// calculate kernel scale
				double kernelScale = Settings.NoiseReduction;
				if (0 == Settings.Scale)
					kernelScale *= Math.Max(matInput_.Width, matInput_.Height) / 512.0;

				// apply scale
				medianKernel = (int)(medianKernel * kernelScale + 0.5);
				medianKernel = medianKernel - (medianKernel % 2) + 1;

				if (medianKernel > 1)
					matBlur = matScaled.MedianBlur(medianKernel);
			}

			// detect edges with our 'adaptive' algorithm that computes bounds automatically with
			// image's mean value
			var matEdges = matBlur.AdaptiveEdges(Settings.EdgesTight);

			// now find contours
			Point[][] contours;
			HierarchyIndex[] hierarchy;
			Cv2.FindContours(matEdges, out contours, out hierarchy, RetrievalModes.List, ContourApproximationModes.ApproxNone, null);

			// check contours and drop those we consider "noise", all others put into a single huge "key points" map
			// also, detect all almost-rectangular contours with big area and try to determine whether they're exact match
			List<Point> keyPoints = new List<Point>();
			List<Point[]> goodCandidates = new List<Point[]>();
			double referenceArea = matScaled.Width * matScaled.Height;
			foreach (Point[] contour in contours)
			{
				double length = Cv2.ArcLength(contour, true);

				// drop mini-contours
				if (length >= 25.0)
				{
					Point[] approx = Cv2.ApproxPolyDP(contour, length * 0.01, true);
					keyPoints.AddRange(approx);

					if (approx.Length >= 4 && approx.Length <= 6)
					{
						double area = Cv2.ContourArea(approx);
						if (area / referenceArea >= Settings.ExpectedArea)
							goodCandidates.Add(approx);
					}
				}
			}

			// compute convex hull, considering we presume having an image of a document on more or less
			// homogeneous background, this accumulated convex hull should be the document bounding contour
			Point[] hull = Cv2.ConvexHull(keyPoints);
			Point[] hullContour = Cv2.ApproxPolyDP(hull, Cv2.ArcLength(hull, true) * 0.01, true);

			// find best guess for our contour
			Point[] paperContour = GetBestMatchingContour(matScaled.Width * matScaled.Height, goodCandidates, hullContour);
			if (null == paperContour)
			{
				shape_ = null;
				dirty_ = false;
				matOutput_ = matInput_;
				return;
			}

			// exact hit - we have 4 corners
			if (paperContour.Length == 4)
			{
				paperContour = SortCorners(paperContour);
			}
			// some hit: we either have 3 points or > 4 which we can try to make a 4-corner shape
			else if (paperContour.Length > 2)
			{
				// yet contour might contain too much points: along with calculation inaccuracies we might face a
				// bended piece of paper, missing corner etc.
				// the solution is to use bounding box
				RotatedRect bounds = Cv2.MinAreaRect(paperContour);
				Point2f[] points = bounds.Points();
				Point[] intPoints = Array.ConvertAll(points, p => new Point(Math.Round(p.X), Math.Round(p.Y)));
				Point[] fourCorners = SortCorners(intPoints);

				// array.ClosestElement is not efficient but we can live with it since it's quite few
				// elements to search for
				System.Func<Point, Point, double> distance = (Point x, Point y) => Point.Distance(x, y);
				Point[] closest = new Point[4];
				for (int i = 0; i < fourCorners.Length; ++i)
					closest[i] = paperContour.ClosestElement(fourCorners[i], distance);

				paperContour = closest;
			}

			// scale contour back to input image coordinate space - if necessary
			if (sx != 1 || sy != 1)
			{
				for (int i = 0; i < paperContour.Length; ++i)
				{
					Point2f pt = paperContour[i];
					paperContour[i] = new Point2f(pt.X / sx, pt.Y / sy);
				}
			}

			// un-wrap
			var matUnwrapped = matInput_;
			bool needConvertionToBGR = true;
			if (paperContour.Length == 4)
			{
				matUnwrapped = matInput_.UnwrapShape(Array.ConvertAll(paperContour, p => new Point2f(p.X, p.Y)));

				// automatic color converter
				bool convertColor = (ScannerSettings.DecolorizationMode.Always == Settings.Decolorization);
				if (ScannerSettings.DecolorizationMode.Automatic == Settings.Decolorization)
					convertColor = !IsColored(matUnwrapped);

				// perform color conversion to b&w
				if (convertColor)
				{
					matUnwrapped = matUnwrapped.CvtColor(ColorConversionCodes.BGR2GRAY);

					// we have some constants for Adaptive, but this can be improved with some 'educated guess' for the constants depending on input image
					if (ScannerSettings.ScanType.Adaptive == Settings.ColorThreshold)
						matUnwrapped = matUnwrapped.AdaptiveThreshold(255, AdaptiveThresholdTypes.MeanC, ThresholdTypes.Binary, 47, 25);
					// Otsu doesn't need our help, decent on it's own
					else
						matUnwrapped = matUnwrapped.Threshold(0, 255, ThresholdTypes.Binary | ThresholdTypes.Otsu);
				}
				else
				{
					needConvertionToBGR = false;
				}
			}

			// assign result
			shape_ = paperContour;

			matOutput_ = matUnwrapped;
			if (needConvertionToBGR)
				matOutput_ = matOutput_.CvtColor(ColorConversionCodes.GRAY2BGR);    // to make it compatible with input texture

			// mark we're good
			dirty_ = false;
		}
	}
}