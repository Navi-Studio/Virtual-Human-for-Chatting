using System;
using System.Collections.Generic;
using System.Text;
using OpenCvSharp.Util;

namespace OpenCvSharp.Face
{
    /// <summary>
    /// Abstract base class for all face recognition models.
    /// All face recognition models in OpenCV are derived from the abstract base class FaceRecognizer, which
    /// provides a unified access to all face recognition algorithms in OpenCV.
    /// </summary>
    public abstract class FaceRecognizer : Algorithm
    {
		protected IntPtr smartPointer;
		protected bool disposed;

        /// <summary>
        ///
        /// </summary>
        protected FaceRecognizer(IntPtr smartPtr)
        {
			smartPointer = smartPtr;
			ptr = GetPureObjectPtr();
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="numComponents"> The number of components (read: Eigenfaces) kept for this Principal Component Analysis. 
        /// As a hint: There's no rule how many components (read: Eigenfaces) should be kept for good reconstruction capabilities. 
        /// It is based on your input data, so experiment with the number. Keeping 80 components should almost always be sufficient.</param>
        /// <param name="threshold">The threshold applied in the prediction.</param>
        /// <returns></returns>
        public static BasicFaceRecognizer CreateEigenFaceRecognizer(int numComponents = 0, double threshold = Double.MaxValue)
        {
            IntPtr p = NativeMethods.face_createEigenFaceRecognizer(numComponents, threshold);
            return new BasicFaceRecognizer(p);
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="numComponents">The number of components (read: Fisherfaces) kept for this Linear Discriminant Analysis 
        /// with the Fisherfaces criterion. It's useful to keep all components, that means the number of your classes c 
        /// (read: subjects, persons you want to recognize). If you leave this at the default (0) or set it 
        /// to a value less-equal 0 or greater (c-1), it will be set to the correct number (c-1) automatically.</param>
        /// <param name="threshold">The threshold applied in the prediction. If the distance to the nearest neighbor 
        /// is larger than the threshold, this method returns -1.</param>
        /// <returns></returns>
        public static BasicFaceRecognizer CreateFisherFaceRecognizer(
            int numComponents = 0, double threshold = Double.MaxValue)
        {
            IntPtr p = NativeMethods.face_createFisherFaceRecognizer(numComponents, threshold);
            return new BasicFaceRecognizer(p);
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="radius">The radius used for building the Circular Local Binary Pattern. The greater the radius, the</param>
        /// <param name="neighbors">The number of sample points to build a Circular Local Binary Pattern from. 
        /// An appropriate value is to use `8` sample points.Keep in mind: the more sample points you include, the higher the computational cost.</param>
        /// <param name="gridX">The number of cells in the horizontal direction, 8 is a common value used in publications. 
        /// The more cells, the finer the grid, the higher the dimensionality of the resulting feature vector.</param>
        /// <param name="gridY">The number of cells in the vertical direction, 8 is a common value used in publications. 
        /// The more cells, the finer the grid, the higher the dimensionality of the resulting feature vector.</param>
        /// <param name="threshold">The threshold applied in the prediction. If the distance to the nearest neighbor 
        /// is larger than the threshold, this method returns -1.</param>
        /// <returns></returns>
        public static LBPHFaceRecognizer CreateLBPHFaceRecognizer(int radius = 1, int neighbors = 8,
            int gridX = 8, int gridY = 8, double threshold = Double.MaxValue)
        {
            IntPtr p = NativeMethods.face_createLBPHFaceRecognizer(radius, neighbors, gridX, gridY, threshold);
            return new LBPHFaceRecognizer(p);
        }

        /// <summary>
        /// Releases managed resources
        /// </summary>
		protected override void Dispose(bool disposing)
		{
			if (!disposed)
			{
				try
				{
					FreeNativeResources();
					ptr = IntPtr.Zero;
					disposed = true;
				}
				finally
				{
					base.Dispose(disposing);
				}
			}
		}

		/// <summary>
		/// Releases all native resources
		/// </summary>
		protected abstract void FreeNativeResources();

		/// <summary>
		/// Gets native objetc pointer from cv::Ptr<>
		/// </summary>
		protected abstract IntPtr GetPureObjectPtr();

		#region Methods

		/// <summary>
		/// Trains a FaceRecognizer with given data and associated labels.
		/// </summary>
		/// <param name="src"></param>
		/// <param name="labels"></param>
		public virtual void Train(IEnumerable<Mat> src, IEnumerable<int> labels)
        {
            ThrowIfDisposed();
            if (src == null)
                throw new ArgumentNullException("src");
            if (labels == null)
                throw new ArgumentNullException("labels");
            IntPtr[] srcArray = EnumerableEx.SelectPtrs(src);
            int[] labelsArray = EnumerableEx.ToArray(labels);
            NativeMethods.face_FaceRecognizer_train(
                ptr, srcArray, srcArray.Length, labelsArray, labelsArray.Length);
        }

        /// <summary>
        /// Updates a FaceRecognizer with given data and associated labels.
        /// </summary>
        /// <param name="src"></param>
        /// <param name="labels"></param>
        public void Update(IEnumerable<Mat> src, IEnumerable<int> labels)
        {
            ThrowIfDisposed();
            if (src == null)
                throw new ArgumentNullException("src");
            if (labels == null)
                throw new ArgumentNullException("labels");
            IntPtr[] srcArray = EnumerableEx.SelectPtrs(src);
            int[] labelsArray = EnumerableEx.ToArray(labels);
            NativeMethods.face_FaceRecognizer_update(
                ptr, srcArray, srcArray.Length, labelsArray, labelsArray.Length);
        }

        /// <summary>
        /// Gets a prediction from a FaceRecognizer.
        /// </summary>
        /// <param name="src"></param>
        /// <returns></returns>
        public virtual int Predict(InputArray src)
        {
            ThrowIfDisposed();
            if (src == null)
                throw new ArgumentNullException("src");
            src.ThrowIfDisposed();
            return NativeMethods.face_FaceRecognizer_predict1(ptr, src.CvPtr);
        }

        /// <summary>
        /// Predicts the label and confidence for a given sample.
        /// </summary>
        /// <param name="src"></param>
        /// <param name="label"></param>
        /// <param name="confidence"></param>
        public virtual void Predict(InputArray src, out int label, out double confidence)
        {
            ThrowIfDisposed();
            if (src == null)
                throw new ArgumentNullException("src");
            src.ThrowIfDisposed();
            NativeMethods.face_FaceRecognizer_predict2(ptr, src.CvPtr, out label, out confidence);
        }

        /// <summary>
        /// Serializes this object to a given filename.
        /// </summary>
        /// <param name="fileName"></param>
        public new virtual void Save(string fileName)
        {
            ThrowIfDisposed();
            if (fileName == null)
                throw new ArgumentNullException("fileName");
            NativeMethods.face_FaceRecognizer_save1(ptr, fileName);
        }

        /// <summary>
        /// Deserializes this object from a given filename.
        /// </summary>
        /// <param name="fileName"></param>
        public virtual void Load(string fileName)
        {
            ThrowIfDisposed();
            if (fileName == null)
                throw new ArgumentNullException("fileName");
            NativeMethods.face_FaceRecognizer_load1(ptr, fileName);
        }

        /// <summary>
        /// Serializes this object to a given cv::FileStorage.
        /// </summary>
        /// <param name="fs"></param>
        public virtual void Save(FileStorage fs)
        {
            ThrowIfDisposed();
            if (fs == null)
                throw new ArgumentNullException("fs");
            NativeMethods.face_FaceRecognizer_save2(ptr, fs.CvPtr);
        }

        /// <summary>
        /// Deserializes this object from a given cv::FileStorage.
        /// </summary>
        /// <param name="fs"></param>
        public virtual void Load(FileStorage fs)
        {
            ThrowIfDisposed();
            if (fs == null)
                throw new ArgumentNullException("fs");
            NativeMethods.face_FaceRecognizer_load2(ptr, fs.CvPtr);
        }

        /// <summary>
        /// Sets string info for the specified model's label.
        /// The string info is replaced by the provided value if it was set before for the specified label.
        /// </summary>
        /// <param name="label"></param>
        /// <param name="strInfo"></param>
        public void SetLabelInfo(int label, string strInfo)
        {
            ThrowIfDisposed();
            if (strInfo == null)
                throw new ArgumentNullException("strInfo");
            NativeMethods.face_FaceRecognizer_setLabelInfo(ptr, label, strInfo);
        }

        /// <summary>
        /// Gets string information by label.
        /// If an unknown label id is provided or there is no label information associated with the specified 
        /// label id the method returns an empty string.
        /// </summary>
        /// <param name="label"></param>
        /// <returns></returns>
        public string GetLabelInfo(int label)
        {
            ThrowIfDisposed();
            using (var resultVector = new VectorOfByte())
            {
                NativeMethods.face_FaceRecognizer_getLabelInfo(ptr, label, resultVector.CvPtr);
                return System.Runtime.InteropServices.Marshal.PtrToStringAnsi(resultVector.ElemPtr);
            }
        }

        /// <summary>
        /// Gets vector of labels by string.
        /// The function searches for the labels containing the specified sub-string in the associated string info.
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public int[] GetLabelsByString(string str)
        {
            ThrowIfDisposed();
            if (str == null)
                throw new ArgumentNullException("str");
            using (var resultVector = new VectorOfInt32())
            {
                NativeMethods.face_FaceRecognizer_getLabelsByString(ptr, str, resultVector.CvPtr);
                return resultVector.ToArray();
            }
        }

        /// <summary>
        /// threshold parameter accessor - required for default BestMinDist collector
        /// </summary>
        /// <returns></returns>
        public double GetThreshold()
        {
            ThrowIfDisposed();
            return NativeMethods.face_FaceRecognizer_getThreshold(ptr);
        }

        /// <summary>
        /// Sets threshold of model
        /// </summary>
        /// <param name="val"></param>
        public void SetThreshold(double val)
        {
            ThrowIfDisposed();
            NativeMethods.face_FaceRecognizer_setThreshold(ptr, val);
        }

        #endregion
	}
}