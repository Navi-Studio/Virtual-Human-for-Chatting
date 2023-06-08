namespace OpenCvSharp.Demo
{
	using System;
	using System.Collections.Generic;
	using OpenCvSharp;

	/// <summary>
	/// OpenCV Point struct(s) extension
	/// </summary>
	static partial class PointUtilities
	{
		/// <summary>
		/// Normalizes vector
		/// </summary>
		/// <returns>Normalized vector</returns>
		public static Point2d Normalize(this Point vec)
		{
			double len = 1.0 / vec.Length();
			return new Point2d(vec.X * len, vec.Y * len);
		}
	}

	/// <summary>
	/// AlphabetOCR auxiliary class
	/// </summary>
	public class AlphabetOCR
	{
		/// <summary>
		/// This must exactly match vocabulary from trained model we're using
		/// </summary>
		const string vocabulary = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";

		/// <summary>
		/// We use it to recognize single character
		/// </summary>
		OCRHMMDecoder.ClassifierCallback ocr = null;

		/// <summary>
		/// Recognition result
		/// </summary>
		public class RecognizedLetter
		{
			/// <summary>
			/// Recognized text data, a single char in form of string
			/// </summary>
			public string Data { get; set; }

			/// <summary>
			/// Detected region, a "rotated box" as 4 points
			/// </summary>
			public Point[] Rect { get; set; }

			/// <summary>
			/// Recognition confidence, i.e. probability of the correct output
			/// </summary>
			public double Confidence { get; set; }

			/// <summary>
			/// Source char rotation angle
			/// </summary>
			public double Angle { get; set; }
		}

		/// <summary>
		/// Default c-tor
		/// </summary>
		/// <param name="model">HMM model, it's "OCRHMM_knn_model_data.xml.gz" from original OpenCV package for instance</param>
		public AlphabetOCR(byte[] model)
		{
			// use temporary file as OpenCV::Contrib::Text doesn not have in-memory reading functions
			string folder = UnityEngine.Application.persistentDataPath;
			string filename = null;
			while (null == filename || System.IO.File.Exists(System.IO.Path.Combine(folder, filename)))
				filename = System.IO.Path.GetRandomFileName() + ".xml";

			// flush
			var tempFile = System.IO.Path.Combine(folder, filename);
			System.IO.File.WriteAllBytes(tempFile, model);

			// read classifier callback and clean file
			ocr = new OCRHMMDecoder.ClassifierCallback(tempFile, CvText.OCRClassifierType.KNN);
			System.IO.File.Delete(tempFile);
		}

		/// <summary>
		/// Detects letters on given image
		/// </summary>
		/// <param name="clean">B&W thresholded image with black background and white letters</param>
		/// <returns>ROI as a list of RotatedRect's, each around a single letter</returns>
		IList<RotatedRect> DetectLetters(Mat clean)
		{
			var output = new List<RotatedRect>();

			// find letters
			Point[][] contours;
			HierarchyIndex[] hierarchy;
			clean.FindContours(out contours, out hierarchy, RetrievalModes.Tree, ContourApproximationModes.ApproxSimple);

			// save
			for (int index = 0; index != -1; index = hierarchy[index].Next)
			{
				if (hierarchy[index].Parent == -1)
					output.Add(Cv2.MinAreaRect(contours[index]));
			}

			return output;
		}

		/// <summary>
		/// Recognized single char
		/// </summary>
		/// <param name="cleanLetter">B&W thresholded image with single char as white on black background</param>
		/// <returns>Best match recognition data</returns>
		RecognizedLetter ProcessSingleLetter(Mat cleanLetter)
		{
			// ocr
			int[] classes;
			double[] confidences;
			ocr.Eval(cleanLetter, out classes, out confidences);

			// process
			if (confidences.Length > 0)
			{
				var unit = new RecognizedLetter();
				unit.Data = vocabulary[classes[0]].ToString();
				unit.Confidence = confidences[0];
				return unit;
			}

			return null;
		}

		/// <summary>
		/// Recognizes letter on the image
		/// </summary>
		/// <param name="clean">B&W thresholded image with black background and white letters</param>
		/// <param name="possibilities">ROI as a list of RotatedRect's with letters marked</param>
		/// <returns>Recognized letters</returns>
		IList<RecognizedLetter> RecognizeLetters(Mat clean, IList<RotatedRect> possibilities)
		{
			var result = new List<RecognizedLetter>();
			foreach (RotatedRect potentialLetter in possibilities)
			{
				var corners = Cv2.BoxPoints(potentialLetter);
				var chunk = clean.UnwrapShape(corners, 64);

				// check aspect as scanner might have issues with some "tall" chars like { I, J, L, etc. } and we can
				// help the detection here: as there are no that much wide chars, we presume all strongly disproportional chars
				// to be tall and reduce rotation options
				Mat[] letters = null;
				var size = potentialLetter.Size;
				var aspect = Math.Max(size.Width, size.Height) / Math.Min(size.Width, size.Height);
				if (aspect >= 2.0)
				{
					if (chunk.Cols > chunk.Rows) // it lays on "broadside", let's make it tall again
						chunk = chunk.Rotate(RotateFlags.Rotate90Clockwise);

					letters = new Mat[]
					{
						chunk,
						chunk.Rotate(RotateFlags.Rotate180)
					};
				}
				else
				{
					// as we're not sure how exactly is letter rotated, we recognize it 4 times,
					// with a 90 degrees step, than the best match (best "confidence" from OCR) is
					// used
					letters = new Mat[]
					{
						chunk,
						chunk.Rotate(RotateFlags.Rotate90Clockwise),
						chunk.Rotate(RotateFlags.Rotate180),
						chunk.Rotate(RotateFlags.Rotate90CounterClockwise)
					};
				}

				// ocr
				int selectedIndex = -1;
				RecognizedLetter unit = null;
				var lowerCases = new List<KeyValuePair<int, RecognizedLetter>>();
				for (int i = 0; i < letters.Length; ++i)
				{
					var local = ProcessSingleLetter(letters[i]);
					if (null != local && (null == unit || (null != unit && null != local && local.Confidence > unit.Confidence)))
					{
						if (char.IsUpper(local.Data[0]))
						{
							unit = local;
							selectedIndex = i;
						}
						else
						{
							lowerCases.Add(new KeyValuePair<int, RecognizedLetter>(i, local));
						}
					}

					//Cv2.ImShow(string.Format("#{0}: {1} - {2:00.0}%", i, local.Data, local.Confidence * 100), letters[i]);
				}

				if (null == unit && lowerCases.Count > 0)
				{
					lowerCases.Sort((x, y) => x.Value.Confidence.CompareTo(y.Value.Confidence));
					unit = lowerCases[0].Value;
					selectedIndex = lowerCases[0].Key;
				}

				// process
				if (null != unit)
				{
					// this will show each separated letter in it's own window, un-wrapped and rotated according
					// to the best found match
					//Cv2.ImShow(unit.Data, letters[selectedIndex]);

					unit.Rect = Array.ConvertAll(corners, p => new Point(Math.Round(p.X), Math.Round(p.Y)));
					unit.Angle = (potentialLetter.Angle + selectedIndex * 90) % 360;
					result.Add(unit);
				}
			}

			return result;
		}

		/// <summary>
		/// Detects all single characters on image
		/// </summary>
		/// <param name="input">3-channel 8-bit image, i.e. CV_8UC3 mat</param>
		/// <returns>A list of recognized letters with corresponding data</returns>
		public IList<RecognizedLetter> ProcessImage(Mat input)
		{
			// quick pre-process
			var clean = input.CvtColor(ColorConversionCodes.BGR2GRAY).MedianBlur(5).Threshold(127, 255, ThresholdTypes.BinaryInv | ThresholdTypes.Otsu);
			var ROIs = DetectLetters(clean);
			return RecognizeLetters(clean, ROIs);
		}
	}
}