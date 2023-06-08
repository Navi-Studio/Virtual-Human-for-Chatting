using System;

namespace OpenCvSharp.Aruco
{
    /// <summary>
    /// Dictionary/Set of markers. It contains the inner codification
    /// </summary>
    public class Dictionary : DisposableCvObject
    {
        /// <summary>
        /// cv::Ptr&lt;T&gt;
        /// </summary>
		//internal Ptr<Dictionary> ObjectPtr { get; }
		internal Ptr<Dictionary> ptrObj;

        #region Init & Disposal

        /// <summary>
        /// 
        /// </summary>
        internal Dictionary(IntPtr p)
        {
			ptrObj = new Ptr<Dictionary>(p);
			ptr = ptrObj.Get();
        }

        #endregion

        #region Properties

        /// <summary>
        /// Marker code information
        /// </summary>
        public Mat BytesList
        {
            get
            {
                ThrowIfDisposed();
                IntPtr ret = NativeMethods.aruco_Dictionary_getBytesList(ptr);
                return new Mat(ret);
            }
        }

        /// <summary>
        /// Number of bits per dimension.
        /// </summary>
        public int MarkerSize
        {
            get
            {
                ThrowIfDisposed();
                return NativeMethods.aruco_Dictionary_getMarkerSize(ptr);
            }
            set
            {
                ThrowIfDisposed();
                NativeMethods.aruco_Dictionary_setMarkerSize(ptr, value);
            }
        }

        /// <summary>
        /// Maximum number of bits that can be corrected.
        /// </summary>
        public int MaxCorrectionBits
        {
            get
            {
                ThrowIfDisposed();
                return NativeMethods.aruco_Dictionary_getMaxCorrectionBits(ptr);
            }
            set
            {
                ThrowIfDisposed();
                NativeMethods.aruco_Dictionary_setMaxCorrectionBits(ptr, value);
            }
        }

        #endregion


    }
}
