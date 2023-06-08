using System;
using System.Collections.Generic;
using System.Text;

namespace OpenCvSharp.Face
{
    /// <summary>
    /// base for two FaceRecognizer classes
    /// </summary>
    public class BasicFaceRecognizer : FaceRecognizer
    {
        /// <summary>
        ///
        /// </summary>
        public BasicFaceRecognizer(IntPtr smartPtr)
			: base(smartPtr)
        {}

		protected override void FreeNativeResources()
		{
			if (smartPointer != IntPtr.Zero)
			{
				NativeMethods.face_Ptr_BasicFaceRecognizer_get(smartPointer);
				smartPointer = IntPtr.Zero;
			}
		}

		protected override IntPtr GetPureObjectPtr()
		{
			return NativeMethods.face_Ptr_BasicFaceRecognizer_get(smartPointer);
		}

		#region Methods
		/// <summary>
		/// 
		/// </summary>
		/// <returns></returns>
		public virtual int GetNumComponents()
        {
            ThrowIfDisposed();
            return NativeMethods.face_BasicFaceRecognizer_getNumComponents(ptr);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="val"></param>
        public virtual void SetNumComponents(int val)
        {
            ThrowIfDisposed();
            NativeMethods.face_BasicFaceRecognizer_setNumComponents(ptr, val);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public new virtual double GetThreshold()
        {
            ThrowIfDisposed();
            return NativeMethods.face_BasicFaceRecognizer_getThreshold(ptr);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="val"></param>
        public new virtual void SetThreshold(double val)
        {
            ThrowIfDisposed();
            NativeMethods.face_BasicFaceRecognizer_setThreshold(ptr, val);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public virtual Mat[] GetProjections()
        {
            ThrowIfDisposed();
            using (var resultVector = new VectorOfMat())
            {
                NativeMethods.face_BasicFaceRecognizer_getProjections(ptr, resultVector.CvPtr);
                return resultVector.ToArray();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public virtual Mat GetLabels()
        {
            ThrowIfDisposed();
            Mat result = new Mat();
            NativeMethods.face_BasicFaceRecognizer_getLabels(ptr, result.CvPtr);
            return result;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public virtual Mat GetEigenValues()
        {
            ThrowIfDisposed();
            Mat result = new Mat();
            NativeMethods.face_BasicFaceRecognizer_getEigenValues(ptr, result.CvPtr);
            return result;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public virtual Mat GetEigenVectors()
        {
            ThrowIfDisposed();
            Mat result = new Mat();
            NativeMethods.face_BasicFaceRecognizer_getEigenVectors(ptr, result.CvPtr);
            return result;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public virtual Mat GetMean()
        {
            ThrowIfDisposed();
            Mat result = new Mat();
            NativeMethods.face_BasicFaceRecognizer_getMean(ptr, result.CvPtr);
            return result;
        }

        #endregion
	}
}