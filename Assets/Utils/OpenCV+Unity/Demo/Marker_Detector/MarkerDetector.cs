namespace OpenCvSharp.Demo {

	using UnityEngine;
	using System.Collections;
	using UnityEngine.UI;
	using Aruco;

	public class MarkerDetector : MonoBehaviour {

		public Texture2D texture;

		void Start () {
			// Create default parameres for detection
			DetectorParameters detectorParameters = DetectorParameters.Create();

			// Dictionary holds set of all available markers
			Dictionary dictionary = CvAruco.GetPredefinedDictionary (PredefinedDictionaryName.Dict6X6_250);

			// Variables to hold results
			Point2f[][] corners;
			int[] ids;
			Point2f[][] rejectedImgPoints;

			// Create Opencv image from unity texture
			Mat mat = Unity.TextureToMat (this.texture);

			// Convert image to grasyscale
			Mat grayMat = new Mat ();
			Cv2.CvtColor (mat, grayMat, ColorConversionCodes.BGR2GRAY); 

			// Detect and draw markers
			CvAruco.DetectMarkers (grayMat, dictionary, out corners, out ids, detectorParameters, out rejectedImgPoints);
			CvAruco.DrawDetectedMarkers (mat, corners, ids);

			// Create Unity output texture with detected markers
			Texture2D outputTexture = Unity.MatToTexture (mat);

			// Set texture to see the result
			RawImage rawImage = gameObject.GetComponent<RawImage> ();
			rawImage.texture = outputTexture;
		}
		
	}
}