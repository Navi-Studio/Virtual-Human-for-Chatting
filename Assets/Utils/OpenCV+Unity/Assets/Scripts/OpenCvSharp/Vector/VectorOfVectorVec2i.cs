using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using OpenCvSharp.Util;

namespace OpenCvSharp
{
	class VectorOfVectorVec2i : DisposableCvObject, IStdVector<Vec2i[]>
	{
		/// <summary>
		/// Track whether Dispose has been called
		/// </summary>
		private bool disposed = false;
		
		/// <summary>
		/// 
		/// </summary>
		public VectorOfVectorVec2i()
		{
			ptr = NativeMethods.vector_vector_Vec2i_new1();
		}

		/// <summary>
		/// 
		/// </summary>
		public VectorOfVectorVec2i(Vec2i[][] source)
		{
			using (var srcPtr = new ArrayAddress2<Vec2i>(source))
			{
				IntPtr[] sizes = new IntPtr[source.Length];
				for (int i = 0; i < source.Length; ++i)
					sizes[i] = new IntPtr(source[i].Length);

				ptr = NativeMethods.vector_vector_Vec2i_new3(new IntPtr(source.Length), sizes, srcPtr);
			}
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="ptr"></param>
		public VectorOfVectorVec2i(IntPtr ptr)
		{
			this.ptr = ptr;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="size"></param>
		public VectorOfVectorVec2i(int size)
		{
			if (size < 0)
				throw new ArgumentOutOfRangeException("nameof(size)");
			ptr = NativeMethods.vector_vector_Vec2i_new2(new IntPtr(size));
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
						NativeMethods.vector_vector_Vec2i_delete(ptr);
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
			get { return NativeMethods.vector_vector_Vec2i_getSize1(ptr).ToInt32(); }
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
				NativeMethods.vector_vector_Vec2i_getSize2(ptr, size2Org);
				long[] size2 = new long[size1];
				for (int i = 0; i < size1; i++)
				{
					size2[i] = size2Org[i].ToInt64();
				}
				return size2;
			}
		}

		/// <summary>
		/// &amp;vector[0]
		/// </summary>
		public IntPtr ElemPtr
		{
			get { return NativeMethods.vector_vector_Vec2i_getPointer(ptr); }
		}
		
		/// <summary>
		/// Converts std::vector to managed array
		/// </summary>
		/// <returns></returns>
		public Vec2i[][] ToArray()
		{
			int size1 = Size1;
			if (size1 == 0)
				return new Vec2i[0][];
			long[] size2 = Size2;

			Vec2i[][] ret = new Vec2i[size1][];
			for (int i = 0; i < size1; i++)
			{
				ret[i] = new Vec2i[size2[i]];
			}
			using (var retPtr = new ArrayAddress2<Vec2i>(ret))
			{
				NativeMethods.vector_vector_Vec2i_copy(ptr, retPtr);
			}
			return ret;
		}
	}
}
