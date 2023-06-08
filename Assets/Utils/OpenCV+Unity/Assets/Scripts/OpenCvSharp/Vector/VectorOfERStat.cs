using System;
using System.Collections.Generic;
using OpenCvSharp.Util;

namespace OpenCvSharp
{
	/// <summary>
	/// This class DOES NOT implement IStdVector(ERStat) as it's native stuct needs copy-constructor
	/// and we can't give direct memory access to it
	/// </summary>
	internal class VectorOfERStat : DisposableCvObject
	{
		/// <summary>
		/// Track whether Dispose has been called
		/// </summary>
		private bool disposed = false;

		/// <summary>
		/// 
		/// </summary>
		public VectorOfERStat()
		{
			ptr = NativeMethods.vector_ERStat_new1();
		}

		/// <summary>
		/// 
		/// </summary>
		public VectorOfERStat(ERStat[] source)
		{
			if (source == null)
				ptr = NativeMethods.vector_ERStat_new1();
			else
			{
				IntPtr[] ptrs = new IntPtr[source.Length];
				for (int i = 0; i < source.Length; ++i)
					ptrs[i] = source[i].CvPtr;
				ptr = NativeMethods.vector_ERStat_new2(ptrs, new IntPtr(source.Length));
			}
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
					if (IsEnabledDispose)
					{
						NativeMethods.vector_ERStat_delete(ptr);
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
		/// vector.size()
		/// </summary>
		public int Size
		{
			get { return NativeMethods.vector_ERStat_getSize(ptr).ToInt32(); }
		}

		/// <summary>
		/// Reads single vector element
		/// </summary>
		/// <param name="index">Index of the element to read</param>
		/// <param name="output">Output struct</param>
		public ERStat GetElement(int index)
		{
			ERStat output = new ERStat();
			NativeMethods.vector_ERStat_getElement(ptr, index, output.CvPtr);
			return output;
		}

		/// <summary>
		/// Converts std::vector to managed array
		/// </summary>
		/// <returns></returns>
		public ERStat[] ToArray()
		{
			int size = Size;
			if (size == 0)
				return new ERStat[0];

			ERStat[] dst = new ERStat[size];
			for (int i = 0; i < size; ++i)
				dst[i] = GetElement(i);
			return dst;
		}
	}
}