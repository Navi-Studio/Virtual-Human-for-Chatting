using System;

namespace OpenCvSharp
{
    /// <summary>
    /// DisposableObject + ICvPtrHolder
    /// </summary>
    abstract public class DisposableCvObject : DisposableObject, ICvPtrHolder
    {
        /// <summary>
        /// Data pointer
        /// </summary>
        protected IntPtr ptr;

        /// <summary>
        /// Track whether Dispose has been called
        /// </summary>
        private bool disposed;

        #region Init and Dispose
        /// <summary>
        /// Default constructor
        /// </summary>
        protected DisposableCvObject()
            : this(true)
        {
        }
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="ptr"></param>
        protected DisposableCvObject(IntPtr ptr)
            : this(ptr, true)
        {
        }
        
        /// <summary>
        ///  
        /// </summary>
        /// <param name="isEnabledDispose"></param>
        protected DisposableCvObject(bool isEnabledDispose)
            : this(IntPtr.Zero, isEnabledDispose)
        {
        }
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="ptr"></param>
        /// <param name="isEnabledDispose"></param>
        protected DisposableCvObject(IntPtr ptr, bool isEnabledDispose)
            : base(isEnabledDispose)
        {
            this.ptr = ptr;
        }
        
        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">
        /// If disposing equals true, the method has been called directly or indirectly by a user's code. Managed and unmanaged resources can be disposed.
        /// If false, the method has been called by the runtime from inside the finalizer and you should not reference other objects. Only unmanaged resources can be disposed.
        /// </param>
        protected override void Dispose(bool disposing)
        {
            if (!disposed)
            {
                try
                {
					DisposeUnmanaged();
                    
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
		/// Default finalizator for unmanaged resources
		/// </summary>
		protected virtual void DisposeUnmanaged()
		{}
		#endregion

		/// <summary>
		/// Native pointer of OpenCV structure
		/// </summary>
		public IntPtr CvPtr
        {
            get
            {
                ThrowIfDisposed();
                return ptr;
            }
        }
    }
}
