using System;
using OpenCvSharp.Util;

namespace OpenCvSharp
{
	/// <summary>
	/// This class DOES NOT implement IStdVector(ERStat) as it's native stuct needs copy-constructor
	/// and we can't give direct memory access to it
	/// </summary>
	internal class VectorOfVectorERStat : DisposableCvObject
	{
		/// <summary>
		/// Track whether Dispose has been called
		/// </summary>
		private bool disposed = false;
		
		/// <summary>
		/// 
		/// </summary>
		public VectorOfVectorERStat()
		{
			ptr = NativeMethods.vector_vector_ERStat_new1();
		}

		/// <summary>
		/// 
		/// </summary>
		public VectorOfVectorERStat(ERStat[][] source)
		{
			var data = new System.Collections.Generic.List<IntPtr>();
			IntPtr[] sizes = new IntPtr[source.Length];
			for (int i = 0; i < source.Length; ++i)
			{
				sizes[i] = new IntPtr(source[i].Length);
				for (int j = 0; j < sizes[i].ToInt32(); ++j)
					data.Add(source[i][j].CvPtr);
			}

			ptr = NativeMethods.vector_vector_ERStat_new2(new IntPtr(source.Length), sizes, data.ToArray());
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="ptr"></param>
		public VectorOfVectorERStat(IntPtr ptr)
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
					if (IsEnabledDispose)
					{
						NativeMethods.vector_vector_Point_delete(ptr);
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
		public int Size1
		{
			get { return NativeMethods.vector_vector_ERStat_getSize1(ptr).ToInt32(); }
		}

		public int Size
		{
			get { return Size1; }
		}

		/// <summary>
		/// vector.size()
		/// </summary>
		public long[] Size2
		{
			get
			{
				int size1 = Size1;
				IntPtr[] size2Org = new IntPtr[size1];
				NativeMethods.vector_vector_ERStat_getSize2(ptr, size2Org);
				long[] size2 = new long[size1];
				for (int i = 0; i < size1; i++)
				{
					size2[i] = size2Org[i].ToInt64();
				}
				return size2;
			}
		}

		/// <summary>
		/// Gets element
		/// </summary>
		/// <param name="i">First coordinate</param>
		/// <param name="j">Second coordinate</param>
		/// <returns>Object at vector[i][j] location</returns>
		public ERStat GetElement(int i, int j)
		{
			ERStat output = new ERStat();
			NativeMethods.vector_vector_ERStat_getElement(ptr, i, j, output.CvPtr);
			return output;
		}

		/// <summary>
		/// Converts std::vector to managed array
		/// </summary>
		/// <returns></returns>
		public ERStat[][] ToArray()
		{
			int size1 = Size1;
			if (size1 == 0)
				return new ERStat[0][];
			long[] size2 = Size2;

			ERStat[][] ret = new ERStat[size1][];
			for (int i = 0; i < size1; i++)
			{
				ret[i] = new ERStat[size2[i]];
				for (int j = 0; j < size2[i]; ++j)
					ret[i][j] = GetElement(i, j);
			}

			return ret;
		}
	}
}
