using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using OpenCvSharp.Detail;
using OpenCvSharp.Util;

namespace OpenCvSharp
{
	static public partial class CvText
	{
		public enum OCRClassifierType
		{
			KNN,
			CNN
		}

		public enum OCRLevel
		{
			Word = 0,
			TextLine
		}

		public enum OCRDecoderMode
		{
			Viterbi = 0
		}
	}
	
	/// <summary>
	/// Base OCR class
	/// </summary>
	public abstract class BaseOCR : DisposableCvObject
	{
		protected object[] preserved = null;

		/// <summary>
		/// Recognize text using Beam Search
		/// Optionally provides also the Rects for individual text elements found (e.g. words), and the list of those text elements with their confidence values.
		/// </summary>
		/// <param name="image">Input image CV_8UC1 with a single text line (or word)</param>
		/// <param name="rects">Method will output a list of Rects for the individual text elements found (e.g. words)</param>
		/// <param name="texts">Method will output a list of text strings for the recognition of individual text elements found (e.g. words)</param>
		/// <param name="confidences">Method will output a list of confidence values for the recognition of individual text elements found (e.g. words)</param>
		/// <param name="component_level"></param>
		/// <returns>Best match</returns>
		public abstract string Run(Mat image, out Rect[] rects, out string[] texts, out float[] confidences, CvText.OCRLevel component_level);

		/// <summary>
		/// Recognize text using Beam Search
		/// Optionally provides also the Rects for individual text elements found (e.g. words), and the list of those text elements with their confidence values.
		/// </summary>
		/// <param name="image">Input image CV_8UC1 with a single text line (or word)</param>
		/// <param name="mask">Text mask CV_8UC1 image</param>
		/// <param name="rects">Method will output a list of Rects for the individual text elements found (e.g. words)</param>
		/// <param name="texts">Method will output a list of text strings for the recognition of individual text elements found (e.g. words)</param>
		/// <param name="confidences">Method will output a list of confidence values for the recognition of individual text elements found (e.g. words)</param>
		/// <param name="component_level"></param>
		/// <returns>Best match</returns>
		public abstract string Run(Mat image, Mat mask, out Rect[] rects, out string[] texts, out float[] confidences, CvText.OCRLevel component_level);
	}

	/// <summary>
	/// OCRHMMDecoder wrapper
	/// </summary>
	public sealed class OCRHMMDecoder : BaseOCR
	{
		public class ClassifierCallback : DisposableCvObject
		{
			/// <summary>
			/// Unparameterized construction is prohibited
			/// </summary>
			private ClassifierCallback()
			{ }

			/// <summary>
			/// Constructs callback
			/// </summary>
			/// <param name="fileName"></param>
			public ClassifierCallback(string fileName, CvText.OCRClassifierType type)
			{
				switch (type)
				{
					case CvText.OCRClassifierType.CNN:
						ptr = NativeMethods.text_loadOCRHMMClassifierCNN(fileName);
						break;

					case CvText.OCRClassifierType.KNN:
						ptr = NativeMethods.text_loadOCRHMMClassifierNM(fileName);
						break;

					default:
						throw new ArgumentException("Incorrect OCRHMMDecoder classifier type specified");
				}
			}

			/// <summary>
			/// Free native resources
			/// </summary>
			protected override void DisposeUnmanaged()
			{
				// releases unmanaged resources
				if (ptr != IntPtr.Zero)
				{
					NativeMethods.text_OCRHMMDecoder_ClassifierCallback_delete(ptr);
					ptr = IntPtr.Zero;
				}
			}

			/// <summary>
			/// 
			/// </summary>
			/// <param name="image">Input image CV_8UC1 or CV_8UC3 with a single letter</param>
			/// <param name="classes">The classifier returns the character class categorical label, or list of class labels, to which the input image corresponds</param>
			/// <param name="confidences">The classifier returns the probability of the input image corresponding to each classes in out_class</param>
			public void Eval(InputArray image, out int[] classes, out double[] confidences)
			{
				if (image == null)
					throw new ArgumentNullException("nameof(image)");
				image.ThrowIfDisposed();

				using (var vecClass = new VectorOfInt32())
				using (var vecConfd = new VectorOfDouble())
				{
					NativeMethods.text_OCRHMMDecoder_ClassifierCallback_eval(ptr, image.CvPtr, vecClass.CvPtr, vecConfd.CvPtr);

					classes = vecClass.ToArray();
					confidences = vecConfd.ToArray();
				}

				GC.KeepAlive(image);
			}
		}

		/// <summary>
		/// OCRHMMDecoder constructor
		/// </summary>
		/// <param name="classifier">The character classifier with built in feature extractor</param>
		/// <param name="vocabulary">The language vocabulary (chars when ascii english text). vocabulary.size() must be equal to the number of classes of the classifier</param>
		/// <param name="transition_probabilities_table">Table with transition probabilities between character pairs. cols == rows == vocabulary.size()</param>
		/// <param name="emission_probabilities_table">Table with observation emission probabilities. cols == rows == vocabulary.size()</param>
		public OCRHMMDecoder(ClassifierCallback classifier, string vocabulary, InputArray transition_probabilities_table, InputArray emission_probabilities_table, CvText.OCRDecoderMode mode)
		{
			preserved = new object[] { classifier, transition_probabilities_table, emission_probabilities_table };
			ptr = NativeMethods.text_OCRHMMDecoder_create(classifier.CvPtr, vocabulary, transition_probabilities_table.CvPtr, emission_probabilities_table.CvPtr, (int)mode);
		}

		/// <summary>
		/// Free native resources
		/// </summary>
		protected override void DisposeUnmanaged()
		{
			// releases unmanaged resources
			if (ptr != IntPtr.Zero)
			{
				NativeMethods.text_OCRHMMDecoder_delete(ptr);
				ptr = IntPtr.Zero;
			}
		}

		/// <summary>
		/// Recognize text using HMM. Takes image on input and returns recognized text in the output_text parameter.
		/// Optionally provides also the Rects for individual text elements found (e.g. words), and the list of those text elements with their confidence values.
		/// </summary>
		/// <param name="image">Input image CV_8UC1 with a single text line (or word)</param>
		/// <param name="rects">Method will output a list of Rects for the individual text elements found (e.g. words)</param>
		/// <param name="texts">Method will output a list of text strings for the recognition of individual text elements found (e.g. words)</param>
		/// <param name="confidences">Method will output a list of confidence values for the recognition of individual text elements found (e.g. words)</param>
		/// <param name="component_level"></param>
		/// <returns></returns>
		public override string Run(Mat image, out Rect[] rects, out string[] texts, out float[] confidences, CvText.OCRLevel component_level)
		{
			using (VectorOfRect vecRects = new VectorOfRect())
			using (VectorOfString vecTexts = new VectorOfString())
			using (VectorOfFloat vecConfidences = new VectorOfFloat())
			{
				NativeMethods.text_OCRHMMDecoder_run(ptr, image.CvPtr, vecRects.CvPtr, vecTexts.CvPtr, vecConfidences.CvPtr, (int)component_level);

				rects = vecRects.ToArray();
				texts = vecTexts.ToArray();
				confidences = vecConfidences.ToArray();
			}

			return texts.Length > 0 ? texts[0] : String.Empty;
		}

		public override string Run(Mat image, Mat mask, out Rect[] rects, out string[] texts, out float[] confidences, CvText.OCRLevel component_level)
		{
			throw new Exception("Masked processing is not implemented in OCRHMMDecoder");
		}
	}

	/// <summary>
	/// OCRBeamSearchDecoder wrapper
	/// </summary>
	public sealed class OCRBeamSearchDecoder : BaseOCR
	{
		public class ClassifierCallback : DisposableCvObject
		{
			/// <summary>
			/// Unparameterized construction is prohibited
			/// </summary>
			private ClassifierCallback()
			{ }

			/// <summary>
			/// Constructs callback
			/// </summary>
			/// <param name="fileName"></param>
			public ClassifierCallback(string fileName)
			{
				ptr = NativeMethods.text_loadOCRBeamSearchClassifierCNN(fileName);
			}

			/// <summary>
			/// Free native resources
			/// </summary>
			protected override void DisposeUnmanaged()
			{
				// releases unmanaged resources
				if (ptr != IntPtr.Zero)
				{
					NativeMethods.text_OCRBeamSearchDecoder_ClassifierCallback_delete(ptr);
					ptr = IntPtr.Zero;
				}
			}

			/// <summary>
			/// 
			/// </summary>
			/// <param name="image">Input image CV_8UC1 or CV_8UC3 with a single letter</param>
			/// <param name="classes">The classifier returns the character class categorical label, or list of class labels, to which the input image corresponds</param>
			/// <param name="confidences">The classifier returns the probability of the input image corresponding to each classes in out_class</param>
			public void Eval(InputArray image, out int[] classes, out double[] confidences)
			{
				if (image == null)
					throw new ArgumentNullException("nameof(image)");
				image.ThrowIfDisposed();

				using (var vecClass = new VectorOfInt32())
				using (var vecConfd = new VectorOfDouble())
				{
					NativeMethods.text_OCRHMMDecoder_ClassifierCallback_eval(ptr, image.CvPtr, vecClass.CvPtr, vecConfd.CvPtr);

					classes = vecClass.ToArray();
					confidences = vecConfd.ToArray();
				}

				GC.KeepAlive(image);
			}
		}

		/// <summary>
		/// OCRBeamSearchDecoder constructor
		/// </summary>
		/// <param name="classifier">The character classifier with built in feature extractor</param>
		/// <param name="vocabulary">The language vocabulary (chars when ascii english text). vocabulary.size() must be equal to the number of classes of the classifier</param>
		/// <param name="transition_probabilities_table">Table with transition probabilities between character pairs. cols == rows == vocabulary.size()</param>
		/// <param name="emission_probabilities_table">Table with observation emission probabilities. cols == rows == vocabulary.size()</param>
		/// <param name="beamSize">Size of the beam in Beam Search algorithm</param>
		public OCRBeamSearchDecoder(ClassifierCallback classifier, string vocabulary, InputArray transition_probabilities_table, InputArray emission_probabilities_table, CvText.OCRDecoderMode mode, int beamSize = 500)
		{
			preserved = new object[] { classifier, transition_probabilities_table, emission_probabilities_table };
			ptr = NativeMethods.text_OCRBeamSearchDecoder_create(classifier.CvPtr, vocabulary, transition_probabilities_table.CvPtr, emission_probabilities_table.CvPtr, (int)mode, beamSize);
		}

		/// <summary>
		/// Free native resources
		/// </summary>
		protected override void DisposeUnmanaged()
		{
			// releases unmanaged resources
			if (ptr != IntPtr.Zero)
			{
				NativeMethods.text_OCRBeamSearchDecoder_delete(ptr);
				ptr = IntPtr.Zero;
			}
		}

		/// <summary>
		/// Recognize text using Beam Search
		/// Optionally provides also the Rects for individual text elements found (e.g. words), and the list of those text elements with their confidence values.
		/// </summary>
		/// <param name="image">Input image CV_8UC1 with a single text line (or word)</param>
		/// <param name="rects">Method will output a list of Rects for the individual text elements found (e.g. words)</param>
		/// <param name="texts">Method will output a list of text strings for the recognition of individual text elements found (e.g. words)</param>
		/// <param name="confidences">Method will output a list of confidence values for the recognition of individual text elements found (e.g. words)</param>
		/// <param name="component_level"></param>
		/// <returns>Best match</returns>
		public override string Run(Mat image, out Rect[] rects, out string[] texts, out float[] confidences, CvText.OCRLevel component_level)
		{
			using (VectorOfRect vecRects = new VectorOfRect())
			using (VectorOfString vecTexts = new VectorOfString())
			using (VectorOfFloat vecConfidences = new VectorOfFloat())
			{
				NativeMethods.text_OCRBeamSearchDecoder_run(ptr, image.CvPtr, vecRects.CvPtr, vecTexts.CvPtr, vecConfidences.CvPtr, (int)component_level);

				rects = vecRects.ToArray();
				texts = vecTexts.ToArray();
				confidences = vecConfidences.ToArray();
			}

			return texts.Length > 0 ? texts[0] : String.Empty;
		}

		/// <summary>
		/// Recognize text using Beam Search
		/// Optionally provides also the Rects for individual text elements found (e.g. words), and the list of those text elements with their confidence values.
		/// </summary>
		/// <param name="image">Input image CV_8UC1 with a single text line (or word)</param>
		/// <param name="mask">Text mask CV_8UC1 image</param>
		/// <param name="rects">Method will output a list of Rects for the individual text elements found (e.g. words)</param>
		/// <param name="texts">Method will output a list of text strings for the recognition of individual text elements found (e.g. words)</param>
		/// <param name="confidences">Method will output a list of confidence values for the recognition of individual text elements found (e.g. words)</param>
		/// <param name="component_level"></param>
		/// <returns></returns>
		public override string Run(Mat image, Mat mask, out Rect[] rects, out string[] texts, out float[] confidences, CvText.OCRLevel component_level)
		{
			using (VectorOfRect vecRects = new VectorOfRect())
			using (VectorOfString vecTexts = new VectorOfString())
			using (VectorOfFloat vecConfidences = new VectorOfFloat())
			{
				NativeMethods.text_OCRBeamSearchDecoder_run2(ptr, image.CvPtr, mask.CvPtr, vecRects.CvPtr, vecTexts.CvPtr, vecConfidences.CvPtr, (int)component_level);

				rects = vecRects.ToArray();
				texts = vecTexts.ToArray();
				confidences = vecConfidences.ToArray();
			}

			return texts.Length > 0 ? texts[0] : String.Empty;
		}
	}
}