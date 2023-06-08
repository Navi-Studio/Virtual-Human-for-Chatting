namespace OpenCvSharp.Demo
{
	using System;
	using System.IO;
	using System.Collections.Generic;
	using OpenCvSharp;
	using OpenCvSharp.Face;

	public class FaceRecognizerScene : UnityEngine.MonoBehaviour
	{
		public UnityEngine.Texture2D sample;
		public UnityEngine.TextAsset faces;
		public UnityEngine.TextAsset recognizerXml;

		private CascadeClassifier cascadeFaces;
		private FaceRecognizer recognizer;
		private string[] names;

		private readonly Size requiredSize = new Size(128, 128);

		#region Face recognizer training
		/// <summary>
		/// Routine to train face recognizer with sample images
		/// </summary>
		/*private void TrainRecognizer(string root)
		{
			// This one was actually used to train the recognizer. I didn't push much effort and satisfied once it
			// distinguished all detected faces on the sample image, for the real-world application you might want to
			// refer to the following documentation:
			// OpenCV documentation and samples: http://docs.opencv.org/3.0-beta/modules/face/doc/facerec/tutorial/facerec_video_recognition.html
			// Training sets overview: https://www.kairos.com/blog/60-facial-recognition-databases
			// Another OpenCV doc: http://docs.opencv.org/2.4/modules/contrib/doc/facerec/facerec_tutorial.html#face-database

			int id = 0;
			var ids = new List<int>();
			var mats = new List<Mat>();
			var namesList = new List<string>();
			
			foreach (string dir in Directory.GetDirectories(root))
			{
				string name = System.IO.Path.GetFileNameWithoutExtension(dir);
				if (name.StartsWith("-"))
					continue;

				namesList.Add(name);
				UnityEngine.Debug.LogFormat("{0} = {1}", id, name);

				foreach (string file in Directory.GetFiles(dir))
				{
					var bytes = File.ReadAllBytes(file);
					var texture = new UnityEngine.Texture2D(2, 2);
					texture.LoadImage(bytes); // <--- this one has changed in Unity 2017 API and on that version must be changed

					ids.Add(id);

					// each loaded texture is converted to OpenCV Mat, turned to grayscale (assuming we have RGB source) and resized
					var mat = Unity.TextureToMat(texture);
					mat = mat.CvtColor(ColorConversionCodes.BGR2GRAY);
					if (requiredSize.Width > 0 && requiredSize.Height > 0)
						mat = mat.Resize(requiredSize);
					mats.Add(mat);
				}
				id++;
			}

			names = namesList.ToArray();

			// train recognizer and save result for the future re-use, while this isn't quite necessary on small training sets, on a bigger set it should
			// give serious performance boost
			recognizer.Train(mats, ids);
			recognizer.Save(root + "/face-recognizer.xml");
		}*/
		#endregion

		/// <summary>
		/// Initializes scene
		/// </summary>
		protected virtual void Awake()
		{
			// classifier
			FileStorage storageFaces = new FileStorage(faces.text, FileStorage.Mode.Read | FileStorage.Mode.Memory);
			cascadeFaces = new CascadeClassifier();
			if (!cascadeFaces.Read(storageFaces.GetFirstTopLevelNode()))
				throw new System.Exception("FaceProcessor.Initialize: Failed to load faces cascade classifier");

			// recognizer
			// There are three available face recognition algorithms in current version of the OpenCV library (please, refer to the OpenCV documentation for details)
			// Our particular training set was trained and saved with FisherFaceRecognizer() and shuld not work with others, however, you can refer to the "TrainRecognizer"
			// method defined above to instructions and sample code regarding training your own recognizer from the scratch
			//recognizer = FaceRecognizer.CreateLBPHFaceRecognizer();
			//recognizer = FaceRecognizer.CreateEigenFaceRecognizer();
			recognizer = FaceRecognizer.CreateFisherFaceRecognizer();

			// This pre-trained set was quite tiny and contained only those 5 persons that are detected and recognized on the image. We took 5 photos for each person from
			// public images on Google, for a real-world application you will need much more sample data for each persona, for more info refer to the OpenCV documentation
			// (there are some links in the "TrainRecognizer" sample function
			recognizer.Load(new FileStorage(recognizerXml.text, FileStorage.Mode.Read | FileStorage.Mode.Memory));

			// label names
			names = new string[] { "Cooper", "DeGeneres", "Nyongo", "Pitt", "Roberts", "Spacey"	};
		}

		/// <summary>
		/// Process the sample
		/// </summary>
		void Start()
		{
			// convert texture to cv image
			Mat image = Unity.TextureToMat(this.sample);
			
			// Detect faces
			var gray = image.CvtColor(ColorConversionCodes.BGR2GRAY);
			Cv2.EqualizeHist(gray, gray);

			// detect matching regions (faces bounding)
			Rect[] rawFaces = cascadeFaces.DetectMultiScale(gray, 1.1, 6);

			// now per each detected face draw a marker and detect eyes inside the face rect
			foreach (var faceRect in rawFaces)
			{
				var grayFace = new Mat(gray, faceRect);
				if (requiredSize.Width > 0 && requiredSize.Height > 0)
					grayFace = grayFace.Resize(requiredSize);

				// now try to recognize the face:
				// "confidence" here is actually a misguide. in fact, it's "distance from the sample to the closest known face" where
				// exact metric is not disclosed in the docs, but checking returned values I found "confidence" to be like 70-100 for
				// positive match with LBPH algo and more like 700-1200 for positive match with EigenFaces/FisherFaces. Unfortunately,
				// all that data isn't much helpful for real life as you don't get adequate % of the confidence, the only thing you
				// actually know is "less is better" with 0 being some "ideal match"
				int label = -1;
				double confidence = 0.0;
				recognizer.Predict(grayFace, out label, out confidence);

				bool found = confidence < 1200;
				Scalar frameColor = found ? Scalar.LightGreen : Scalar.Red;
				Cv2.Rectangle((InputOutputArray)image, faceRect, frameColor, 2);

				int line = 0;
				const int textPadding = 2;
				const double textScale = 2.0;
				string messge = String.Format("{0}", names[label], (int)confidence);
				var textSize = Cv2.GetTextSize(messge, HersheyFonts.HersheyPlain, textScale, 1, out line);
				var textBox = new Rect(
					faceRect.X + (faceRect.Width - textSize.Width) / 2 - textPadding,
					faceRect.Bottom,
					textSize.Width + textPadding * 2,
					textSize.Height + textPadding * 2
				);

				Cv2.Rectangle((InputOutputArray)image, textBox, frameColor, -1);
				image.PutText(messge, textBox.TopLeft + new Point(textPadding, textPadding + textSize.Height), HersheyFonts.HersheyPlain, textScale, Scalar.Black, 2);
			}

			// Render texture
			var texture = Unity.MatToTexture(image);
			var rawImage = gameObject.GetComponent<UnityEngine.UI.RawImage>();
			rawImage.texture = texture;

			var transform = gameObject.GetComponent<UnityEngine.RectTransform>();
			transform.sizeDelta = new UnityEngine.Vector2(image.Width, image.Height);
		}

		// Update is called once per frame
		void Update()
		{}
	}
}