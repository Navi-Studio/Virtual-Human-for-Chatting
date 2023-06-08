using System;
using System.Collections.Generic;
using System.Text;

namespace OpenCvSharp.Face
{
    /// <summary>
    /// 
    /// </summary>
    public class LBPHFaceRecognizer : FaceRecognizer
    {
		/// <summary>
		///
		/// </summary>
		public LBPHFaceRecognizer(IntPtr smartPtr)
			: base(smartPtr)
        {}

		protected override void FreeNativeResources()
		{
			if (smartPointer != IntPtr.Zero)
			{
				NativeMethods.face_Ptr_LBPHFaceRecognizer_delete(smartPointer);
				smartPointer = IntPtr.Zero;
			}
		}

		protected override IntPtr GetPureObjectPtr()
		{
			return NativeMethods.face_Ptr_LBPHFaceRecognizer_get(smartPointer);
		}

		#region Methods

		/// <summary>
		/// 
		/// </summary>
		/// <returns></returns>
		public virtual int GetGridX()
        {
            ThrowIfDisposed();
            return NativeMethods.face_LBPHFaceRecognizer_getGridX(ptr);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="val"></param>
        public virtual void SetGridX(int val)
        {
            ThrowIfDisposed();
            NativeMethods.face_LBPHFaceRecognizer_setGridX(ptr, val);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public virtual int GetGridY()
        {
            ThrowIfDisposed();
            return NativeMethods.face_LBPHFaceRecognizer_getGridY(ptr);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="val"></param>
        public virtual void SetGridY(int val)
        {
            ThrowIfDisposed();
            NativeMethods.face_LBPHFaceRecognizer_setGridY(ptr, val);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public virtual int GetRadius()
        {
            ThrowIfDisposed();
            return NativeMethods.face_LBPHFaceRecognizer_getRadius(ptr);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="val"></param>
        public virtual void SetRadius(int val)
        {
            ThrowIfDisposed();
            NativeMethods.face_LBPHFaceRecognizer_setRadius(ptr, val);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public virtual int GetNeighbors()
        {
            ThrowIfDisposed();
            return NativeMethods.face_LBPHFaceRecognizer_getNeighbors(ptr);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="val"></param>
        public virtual void SetNeighbors(int val)
        {
            ThrowIfDisposed();
            NativeMethods.face_LBPHFaceRecognizer_setNeighbors(ptr, val);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public new virtual double GetThreshold()
        {
            ThrowIfDisposed();
            return NativeMethods.face_LBPHFaceRecognizer_getThreshold(ptr);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="val"></param>
        public new virtual void SetThreshold(double val)
        {
            ThrowIfDisposed();
            NativeMethods.face_LBPHFaceRecognizer_setThreshold(ptr, val);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public virtual Mat[] GetHistograms()
        {
            ThrowIfDisposed();
            using (var resultVector = new VectorOfMat())
            {
                NativeMethods.face_LBPHFaceRecognizer_getHistograms(ptr, resultVector.CvPtr);
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
            NativeMethods.face_LBPHFaceRecognizer_getLabels(ptr, result.CvPtr);
            return result;
        }

		#endregion
    }
}