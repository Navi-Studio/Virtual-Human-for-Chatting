namespace OpenCvSharp.Demo
{
	using UnityEngine;
	using UnityEngine.UI;

	public class DocumentScannerScript : MonoBehaviour
	{
		private PaperScanner scanner = new PaperScanner();

		#region Boring code that combines output image with OpenCV

		/// <summary>
		/// Combines original and processed images into a new twice wide image
		/// </summary>
		/// <param name="original">Source image</param>
		/// <param name="processed">Processed image</param>
		/// <param name="detectedContour">Contour to draw over original image to show detected shape</param>
		/// <returns>OpenCV::Mat image with images combined</returns>
		private Mat CombineMats(Mat original, Mat processed, Point[] detectedContour)
		{
			Size inputSize = new Size(original.Width, original.Height);

			// combine fancy output image:
			// - create new texture twice as wide as input
			// - copy input into the left half
			// - draw detected paper contour over original input
			// - put "scanned", un-warped and cleared paper to the right, centered in the right half
			var matCombined = new Mat(new Size(inputSize.Width * 2, inputSize.Height), original.Type(), Scalar.FromRgb(64, 64, 64));

			// copy original image with detected shape drawn over
			original.CopyTo(matCombined.SubMat(0, inputSize.Height, 0, inputSize.Width));
			if (null != detectedContour && detectedContour.Length > 2)
				matCombined.DrawContours(new Point[][] { detectedContour }, 0, Scalar.FromRgb(255, 255, 0), 3);

			// copy scanned paper without extra scaling, as is
			if (null != processed)
			{
				double hw = processed.Width * 0.5, hh = processed.Height * 0.5;
				Point2d center = new Point2d(inputSize.Width + inputSize.Width * 0.5, inputSize.Height * 0.5);
				Mat roi = matCombined.SubMat(
					(int)(center.Y - hh), (int)(center.Y + hh),
					(int)(center.X - hw), (int)(center.X + hw)
				);
				processed.CopyTo(roi);
			}

			return matCombined;
		}

		#endregion

		// Use this for initialization
		public void Process(string name)
		{	
			var rawImage = gameObject.GetComponent<RawImage>();
			rawImage.texture = null;

			Texture2D inputTexture = (Texture2D)Resources.Load("DocumentScanner/" + name);

			// first of all, we set up scan parameters
			// 
			// scanner.Settings has more values than we use
			// (like Settings.Decolorization that defines
			// whether b&w filter should be applied), but
			// default values are quite fine and some of
			// them are by default in "smart" mode that
			// uses heuristic to find best choice. so,
			// we change only those that matter for us
			scanner.Settings.NoiseReduction = 0.7;											// real-world images are quite noisy, this value proved to be reasonable
			scanner.Settings.EdgesTight = 0.9;												// higher value cuts off "noise" as well, this time smaller and weaker edges
			scanner.Settings.ExpectedArea = 0.2;											// we expect document to be at least 20% of the total image area
			scanner.Settings.GrayMode = PaperScanner.ScannerSettings.ColorMode.Grayscale;	// color -> grayscale conversion mode

			// process input with PaperScanner
			Mat result = null;
			scanner.Input = Unity.TextureToMat(inputTexture);

			// should we fail, there is second try - HSV might help to detect paper by color difference
			if (!scanner.Success)
				// this will drop current result and re-fetch it next time we query for 'Success' flag or actual data
				scanner.Settings.GrayMode = PaperScanner.ScannerSettings.ColorMode.HueGrayscale;

			// now can combine Original/Scanner image
			result = CombineMats(scanner.Input, scanner.Output, scanner.PaperShape);

			// apply result or source (late for a failed scan)
			rawImage.texture = Unity.MatToTexture(result);

			var transform = gameObject.GetComponent<RectTransform>();
			transform.sizeDelta = new Vector2(result.Width, result.Height);
		}

		void Start() {
			Process ("Brochure");
		}

			
	}
}