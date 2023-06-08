using System;
using System.Collections.Generic;

namespace OpenCvSharp
{
	/// <summary>
	/// dlib::shape_predictor wrapper with an extra method
	/// </summary>
	public class ShapePredictor : DisposableCvObject
	{
		/// <summary>
		/// Separate flag from the superclass as we might have our own branch of de-initialization
		/// </summary>
		private bool disposed;

		/// <summary>
		/// Creates new shape predictor
		/// </summary>
		public ShapePredictor()
			: base()
		{
			ptr = NativeMethods.dlib_shapePredictor_new();
		}

		/// <summary>
		/// Releases the resources
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
					// releases managed resources
					if (disposing)
					{
						// releases unmanaged resources
						if (ptr != IntPtr.Zero)
						{
							NativeMethods.dlib_shapePredictor_delete(ptr);
							ptr = IntPtr.Zero;
						}
					}
					disposed = true;
				}
				finally
				{
					base.Dispose(disposing);
				}
			}
		}

		/// <summary>
		/// Loads shape predictor with data
		/// </summary>
		/// <param name="array">Input data stream</param>
		public void LoadData(Byte[] data)
		{
			NativeMethods.dlib_shapePredictor_loadData(ptr, data, data.Length);
		}

		/// <summary>
		/// Detects landmarks on the image
		/// </summary>
		/// <param name="image">Input image</param>
		/// <param name="roi">Region of interest</param>
		/// <returns>Landmark points</returns>
		public Point[] DetectLandmarks(Mat image, Rect roi)
		{
			IntPtr stdvec = IntPtr.Zero;
			if (NativeMethods.dlib_shapePredictor_detectLandmarks(ptr, image.CvPtr, roi, ref stdvec))
			{
				using (VectorOfPoint vec = new VectorOfPoint(stdvec))
				{
					return vec.ToArray();
				}
			}

			return new Point[0];
		}
	}
}
