namespace OpenCvSharp.Demo
{
	using System;
	using System.Collections.Generic;
	using OpenCvSharp;

	/// <summary>
	/// Array utilities
	/// http://stackoverflow.com/questions/1792470/subset-of-array-in-c-sharp
	/// </summary>
	static partial class ArrayUtilities
    {
        // create a subset from a range of indices
        public static T[] RangeSubset<T>(this T[] array, int startIndex, int length)
        {
            T[] subset = new T[length];
            Array.Copy(array, startIndex, subset, 0, length);
            return subset;
        }

        // creates subset with from-to index pair
        public static T[] SubsetFromTo<T>(this T[] array, int fromIndex, int toIndex)
        {
            return array.RangeSubset<T>(fromIndex, toIndex - fromIndex + 1);
        }

        // create a subset from a specific list of indices
        public static T[] Subset<T>(this T[] array, params int[] indices)
        {
            T[] subset = new T[indices.Length];
            for (int i = 0; i < indices.Length; i++)
            {
                subset[i] = array[indices[i]];
            }
            return subset;
        }
    }

    /// <summary>
    /// Holds face processor performance trick parameters
    /// </summary>
    class FaceProcessorPerformanceParams
    {
        /// <summary>
        /// Downscale limit, texture processing will downscale input up to this size
        /// If is less or equals to zero than downscaling is not applied
        /// 
        /// Downscaling is applied with preserved aspect ratio
        /// </summary>
        public int Downscale { get; set; }

        /// <summary>
        /// Processor will skip that much frames before processing anything, 0 means no skip
        /// </summary>
        public int SkipRate { get; set; }

        /// <summary>
        /// Default constructor
        /// </summary>
        public FaceProcessorPerformanceParams()
        {
            Downscale = 0;
            SkipRate = 0;
        }
    }

    /// <summary>
    /// High-level wrapper around OpenCV and DLib functionality that simplifies face detection tasks
    /// </summary>
    class FaceProcessor<T>
        where T: UnityEngine.Texture
    {
        protected CascadeClassifier cascadeFaces = null;
        protected CascadeClassifier cascadeEyes = null;
        protected ShapePredictor shapeFaces = null;

        protected Mat processingImage = null;
        protected Double appliedFactor = 1.0;
		protected bool cutFalsePositivesWithEyesSearch = false;

        /// <summary>
        /// Performance options
        /// </summary>
        public FaceProcessorPerformanceParams Performance { get; private set; }

        /// <summary>
        /// Data stabilizer parameters (face rect, face landmarks etc.)
        /// </summary>
        public DataStabilizerParams DataStabilizer { get; private set; }

        /// <summary>
        /// Processed texture
        /// </summary>
        public Mat Image { get; private set; }

        /// <summary>
        /// Detected objects
        /// </summary>
        public List<DetectedFace> Faces { get; private set; }

        /// <summary>
        /// Default constructor
        /// </summary>
        public FaceProcessor()
        {
            Faces = new List<DetectedFace>();
            DataStabilizer = new DataStabilizerParams();
            Performance = new FaceProcessorPerformanceParams();
        }

        /// <summary>
        /// Processor initializer
        /// <param name="facesCascadeData">String with cascade XML for face detection, must be defined</param>
        /// <param name="eyesCascadeData">String with cascade XML for eyes detection, can be null</param>
        /// <param name="shapeData">Binary data with trained shape predictor for 68-point face landmarks recognition, can be empty or null</param>
        /// </summary>
        public virtual void Initialize(string facesCascadeData, string eyesCascadeData, byte[] shapeData = null)
        {
            // face detector - the key thing here
            if (null == facesCascadeData || facesCascadeData.Length == 0)
                throw new Exception("FaceProcessor.Initialize: No face detector cascade passed, with parameter is required");

            FileStorage storageFaces = new FileStorage(facesCascadeData, FileStorage.Mode.Read | FileStorage.Mode.Memory);
            cascadeFaces = new CascadeClassifier();
            if (!cascadeFaces.Read(storageFaces.GetFirstTopLevelNode()))
                throw new System.Exception("FaceProcessor.Initialize: Failed to load faces cascade classifier");

            // eyes detector
            if (null != eyesCascadeData)
            {
                FileStorage storageEyes = new FileStorage(eyesCascadeData, FileStorage.Mode.Read | FileStorage.Mode.Memory);
                cascadeEyes = new CascadeClassifier();
                if (!cascadeEyes.Read(storageEyes.GetFirstTopLevelNode()))
                    throw new System.Exception("FaceProcessor.Initialize: Failed to load eyes cascade classifier");
            }

            // shape detector
            if (null != shapeData && shapeData.Length > 0)
            {
                shapeFaces = new ShapePredictor();
                shapeFaces.LoadData(shapeData);
            }
        }

        /// <summary>
        /// Creates OpenCV Mat from Unity texture
        /// </summary>
        /// <param name="texture">Texture instance, must be either Texture2D or WbCamTexture</param>
        /// <returns>Newely created Mat object, ready to use with OpenCV</returns>
        /// <param name="texParams">Texture parameters (flipped, rotated etc.)</param>
        protected virtual Mat MatFromTexture(T texture, Unity.TextureConversionParams texParams)
        {
            if (texture is UnityEngine.Texture2D)
                return Unity.TextureToMat(texture as UnityEngine.Texture2D, texParams);
            else if (texture is UnityEngine.WebCamTexture)
                return Unity.TextureToMat(texture as UnityEngine.WebCamTexture, texParams);
            else
                throw new Exception("FaceProcessor: incorrect input texture type, must be Texture2D or WebCamTexture");
        }

        /// <summary>
        /// Imports Unity texture to the FaceProcessor, can pre-process object (white balance, resize etc.)
        /// Fill few properties and fields: Image, downscaledImage, appliedScaleFactor
        /// </summary>
        /// <param name="texture">Input texture</param>
        /// <param name="texParams">Texture parameters (flipped, rotated etc.)</param>
        protected virtual void ImportTexture(T texture, Unity.TextureConversionParams texParams)
        {
            // free currently used textures
            if (null != processingImage)
                processingImage.Dispose();
            if (null != Image)
                Image.Dispose();

            // convert and prepare
            Image = MatFromTexture(texture, texParams);
            if (Performance.Downscale > 0 && (Performance.Downscale < Image.Width || Performance.Downscale < Image.Height))
            {
                // compute aspect-respective scaling factor
                int w = Image.Width;
                int h = Image.Height;

                // scale by max side
                if (w >= h)
                {
                    appliedFactor = (double)Performance.Downscale / (double)w;
                    w = Performance.Downscale;
                    h = (int)(h * appliedFactor + 0.5);
                }
                else
                {
                    appliedFactor = (double)Performance.Downscale / (double)h;
                    h = Performance.Downscale;
                    w = (int)(w * appliedFactor + 0.5);
                }

                // resize
                processingImage = new Mat();
                Cv2.Resize(Image, processingImage, new Size(w, h));
            }
            else
            {
                appliedFactor = 1.0;
                processingImage = Image;
            }
        }

        /// <summary>
        /// Detector
        /// </summary>
        /// <param name="inputTexture">Input Unity texture</param>
        /// <param name="texParams">Texture parameters (flipped, rotated etc.)</param>
        /// <param name="detect">Flag signalling whether we need detection on this frame</param>
        public virtual void ProcessTexture(T texture, Unity.TextureConversionParams texParams, bool detect = true)
        {
            // convert Unity texture to OpenCv::Mat
            ImportTexture(texture, texParams);

            // detect
            if (detect)
            {
                double invF = 1.0 / appliedFactor;
                DataStabilizer.ThresholdFactor = invF;

                // convert to grayscale and normalize
                Mat gray = new Mat();
                Cv2.CvtColor(processingImage, gray, ColorConversionCodes.BGR2GRAY);

                // fix shadows
                Cv2.EqualizeHist(gray, gray);

                /*Mat normalized = new Mat();
                CLAHE clahe = CLAHE.Create();
                clahe.TilesGridSize = new Size(8, 8);
                clahe.Apply(gray, normalized);
                gray = normalized;*/

                // detect matching regions (faces bounding)
                Rect[] rawFaces = cascadeFaces.DetectMultiScale(gray, 1.2, 6);
				if (Faces.Count != rawFaces.Length)
					Faces.Clear();

                // now per each detected face draw a marker and detect eyes inside the face rect
                int facesCount = 0;
                for (int i = 0; i < rawFaces.Length; ++i)
                {
                    Rect faceRect = rawFaces[i];
                    Rect faceRectScaled = faceRect * invF;
                    using (Mat grayFace = new Mat(gray, faceRect))
                    {
                        // another trick: confirm the face with eye detector, will cut some false positives
                        if (cutFalsePositivesWithEyesSearch && null != cascadeEyes)
                        {
                            Rect[] eyes = cascadeEyes.DetectMultiScale(grayFace);
                            if (eyes.Length == 0 || eyes.Length > 2)
                                continue;
                        }

                        // get face object
                        DetectedFace face = null;
                        if (Faces.Count < i + 1)
                        {
                            face = new DetectedFace(DataStabilizer, faceRectScaled);
                            Faces.Add(face);
                        }
                        else
                        {
                            face = Faces[i];
                            face.SetRegion(faceRectScaled);
                        }

                        // shape
                        facesCount++;
                        if (null != shapeFaces)
                        {
                            Point[] marks = shapeFaces.DetectLandmarks(gray, faceRect);

                            // we have 68-point predictor
                            if (marks.Length == 68)
                            {
                                // transform landmarks to the original image space
                                List<Point> converted = new List<Point>();
                                foreach (Point pt in marks)
                                    converted.Add(pt * invF);

                                // save and parse landmarks
                                face.SetLandmarks(converted.ToArray());
                            }
                        }
                    }
                }

                // log
                //UnityEngine.Debug.Log(String.Format("Found {0} faces", Faces.Count));
            }
        }

        /// <summary>
        /// Marks detected objects on the texture
        /// </summary>
        public void MarkDetected(bool drawSubItems = true)
        {
            // mark each found eye
            foreach (DetectedFace face in Faces)
            {
                // face rect
                Cv2.Rectangle((InputOutputArray)Image, face.Region, Scalar.FromRgb(255, 0, 0), 2);

                // convex hull
                //Cv2.Polylines(Image, new IEnumerable<Point>[] { face.Info.ConvexHull }, true, Scalar.FromRgb(255, 0, 0), 2);

                // render face triangulation (should we have one)
                if (face.Info != null)
                {
                    foreach (DetectedFace.Triangle tr in face.Info.DelaunayTriangles)
                        Cv2.Polylines(Image, new IEnumerable<Point>[] { tr.ToArray() }, true, Scalar.FromRgb(0, 0, 255), 1);
                }

                // Sub-items
                if (drawSubItems)
                {
                    List<string> closedItems = new List<string>(new string[] { "Nose", "Eye", "Lip" });
                    foreach (DetectedObject sub in face.Elements)
                        if (sub.Marks != null)
                            Cv2.Polylines(Image, new IEnumerable<Point>[] { sub.Marks }, closedItems.Contains(sub.Name), Scalar.FromRgb(0, 255, 0), 1);
                }
            }
        }
    }

    /// <summary>
    /// FaceProcessor subclass designed for live (web camera or stream) processing
    /// </summary>
    class FaceProcessorLive<T> : FaceProcessor<T>
        where T : UnityEngine.Texture
    {
        private int frameCounter = 0;

        /// <summary>
        /// Constructs face processor
        /// </summary>
        public FaceProcessorLive()
            : base()
        {}

        /// <summary>
        /// Detector
        /// </summary>
        /// <param name="inputTexture">Input Unity texture</param>
        /// <param name="texParams">Texture parameters (flipped, rotated etc.)</param>
        /// <param name="detect">Flag signalling whether we need detection on this frame</param>
        public override void ProcessTexture(T texture, Unity.TextureConversionParams texParams,  bool detect = true)
        {
            bool acceptedFrame = (0 == Performance.SkipRate || 0 == frameCounter++ % Performance.SkipRate);
            base.ProcessTexture(texture, texParams, detect && acceptedFrame);
        }
    }
}