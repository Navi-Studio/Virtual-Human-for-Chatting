/**
 * Copyright(c) Live2D Inc. All rights reserved.
 *
 * Use of this source code is governed by the Live2D Open Software license
 * that can be found at https://www.live2d.com/eula/live2d-open-software-license-agreement_en.html.
 */

/* THIS FILE WAS AUTO-GENERATED. ALL CHANGES WILL BE LOST UPON RE-GENERATION. */


using System;


namespace Live2D.Cubism.Core.Unmanaged
{
    /// <summary>
    /// Float array view.
    /// </summary>
    public sealed class CubismUnmanagedFloatArrayView
    {
        /// <summary>
        /// Array length of unmanaged buffer.
        /// </summary>
        public int Length { get; private set; }

        /// <summary>
        /// Return true if instance is valid.
        /// </summary>
        public unsafe bool IsValid { get { return (UnmanagedFixedAddress != (float*)0) && (Length > 0); } }

        /// <summary>
        /// Gets element at index.
        /// </summary>
        /// <param name="index">Index of array.</param>
        /// <returns>Element of array.</returns>
        public unsafe float this[int index]
        {
            get
            {
                var pointer = UnmanagedFixedAddress;


#if DEVELOPMENT_BUILD || UNITY_EDITOR
                {
                    // Assert instance is valid.
                    if (!IsValid)
                    {
                        throw new InvalidOperationException("Array is empty, or not valid.");
                    }

                    if ((index >= Length) || (index < 0))
                    {
                        throw new IndexOutOfRangeException("Array index is out of range.");
                    }
                }
#endif


                return pointer[index];
            }

            set
            {
                var pointer = UnmanagedFixedAddress;


#if DEVELOPMENT_BUILD || UNITY_EDITOR
                {
                    // Assert instance is valid.
                    if (!IsValid)
                    {
                        throw new InvalidOperationException("Array is empty, or not valid.");
                    }

                    if ((index >= Length) || (index < 0))
                    {
                        throw new IndexOutOfRangeException("Array index is out of range.");
                    }
                }
#endif


                pointer[index] = value;
            }
        }


        /// <summary>
        /// Unmanaged buffer address.
        /// </summary>
        private unsafe float* UnmanagedFixedAddress { get; set; }

        #region Ctors

        /// <summary>
        /// Initializes instance.
        /// </summary>
        /// <param name="address">Unmanaged buffer address.</param>
        /// <param name="length">Length of unmanaged buffer (in types).</param>
        internal unsafe CubismUnmanagedFloatArrayView(float* address, int length)
        {
            UnmanagedFixedAddress = address;
            Length = length;
        }

        /// <summary>
        /// Initializes instance.
        /// </summary>
        /// <param name="address">Unmanaged buffer address.</param>
        /// <param name="length">Length of unmanaged buffer (in types).</param>
        internal unsafe CubismUnmanagedFloatArrayView(IntPtr address, int length)
        {
            UnmanagedFixedAddress = (float*)address.ToPointer();
            Length = length;
        }

        #endregion

        /// <summary>
        /// Reads data.
        /// </summary>
        /// <param name="buffer">Destination managed array.</param>
        public unsafe void Read(float[] buffer)
        {
            var sourceAddress = UnmanagedFixedAddress;
            var destinationLength = buffer.Length;


#if DEVELOPMENT_BUILD || UNITY_EDITOR
            {
                // Assert buffer.Length >= Length
                if (destinationLength < Length)
                {
                    throw new InvalidOperationException("Destination buffer length must be larger than source buffer length.");
                }

                // Assert instance is valid.
                if (!IsValid)
                {
                    throw new InvalidOperationException("Array is empty, or not valid.");
                }
            }
#endif


            // Read data into managed.
            fixed (float* destinationAddress = buffer)
            {
                for (var i = 0; i < Length; ++i)
                {
                    destinationAddress[i] = sourceAddress[i];
                }
            }
        }

        /// <summary>
        /// Writes data.
        /// </summary>
        /// <param name="buffer">Source managed array.</param>
        public unsafe void Write(float[] buffer)
        {
            var sourceLength = buffer.Length;
            var destinationAddress = UnmanagedFixedAddress;


#if DEVELOPMENT_BUILD || UNITY_EDITOR
            {
                // Assert both length.
                if (sourceLength > Length)
                {
                    throw new InvalidOperationException("Destination buffer length must be larger than source buffer length.");
                }

                // Assert instance is valid.
                if (!IsValid)
                {
                    throw new InvalidOperationException("Array is empty, or not valid.");
                }
            }
#endif


            // Write data into unmanaged.
            fixed (float* sourceAddress = buffer)
            {
                for (var i = 0; i < sourceLength; ++i)
                {
                    destinationAddress[i] = sourceAddress[i];
                }
            }
        }
    }

    /// <summary>
    /// Int array view.
    /// </summary>
    public sealed class CubismUnmanagedIntArrayView
    {
        /// <summary>
        /// Array length of unmanaged buffer.
        /// </summary>
        public int Length { get; private set; }

        /// <summary>
        /// Return true if instance is valid.
        /// </summary>
        public unsafe bool IsValid { get { return (UnmanagedFixedAddress != (int*)0) && (Length > 0); } }

        /// <summary>
        /// Gets element at index.
        /// </summary>
        /// <param name="index">Index of array.</param>
        /// <returns>Element of array.</returns>
        public unsafe int this[int index]
        {
            get
            {
                var pointer = UnmanagedFixedAddress;


#if DEVELOPMENT_BUILD || UNITY_EDITOR
                {
                    // Assert instance is valid.
                    if (!IsValid)
                    {
                        throw new InvalidOperationException("Array is empty, or not valid.");
                    }

                    if ((index >= Length) || (index < 0))
                    {
                        throw new IndexOutOfRangeException("Array index is out of range.");
                    }
                }
#endif


                return pointer[index];
            }

            set
            {
                var pointer = UnmanagedFixedAddress;


#if DEVELOPMENT_BUILD || UNITY_EDITOR
                {
                    // Assert instance is valid.
                    if (!IsValid)
                    {
                        throw new InvalidOperationException("Array is empty, or not valid.");
                    }

                    if ((index >= Length) || (index < 0))
                    {
                        throw new IndexOutOfRangeException("Array index is out of range.");
                    }
                }
#endif


                pointer[index] = value;
            }
        }


        /// <summary>
        /// Unmanaged buffer address.
        /// </summary>
        private unsafe int* UnmanagedFixedAddress { get; set; }

        #region Ctors

        /// <summary>
        /// Initializes instance.
        /// </summary>
        /// <param name="address">Unmanaged buffer address.</param>
        /// <param name="length">Length of unmanaged buffer (in types).</param>
        internal unsafe CubismUnmanagedIntArrayView(int* address, int length)
        {
            UnmanagedFixedAddress = address;
            Length = length;
        }

        /// <summary>
        /// Initializes instance.
        /// </summary>
        /// <param name="address">Unmanaged buffer address.</param>
        /// <param name="length">Length of unmanaged buffer (in types).</param>
        internal unsafe CubismUnmanagedIntArrayView(IntPtr address, int length)
        {
            UnmanagedFixedAddress = (int*)address.ToPointer();
            Length = length;
        }

        #endregion

        /// <summary>
        /// Reads data.
        /// </summary>
        /// <param name="buffer">Destination managed array.</param>
        public unsafe void Read(int[] buffer)
        {
            var sourceAddress = UnmanagedFixedAddress;
            var destinationLength = buffer.Length;


#if DEVELOPMENT_BUILD || UNITY_EDITOR
            {
                // Assert buffer.Length >= Length
                if (destinationLength < Length)
                {
                    throw new InvalidOperationException("Destination buffer length must be larger than source buffer length.");
                }

                // Assert instance is valid.
                if (!IsValid)
                {
                    throw new InvalidOperationException("Array is empty, or not valid.");
                }
            }
#endif


            // Read data into managed.
            fixed (int* destinationAddress = buffer)
            {
                for (var i = 0; i < Length; ++i)
                {
                    destinationAddress[i] = sourceAddress[i];
                }
            }
        }

        /// <summary>
        /// Writes data.
        /// </summary>
        /// <param name="buffer">Source managed array.</param>
        public unsafe void Write(int[] buffer)
        {
            var sourceLength = buffer.Length;
            var destinationAddress = UnmanagedFixedAddress;


#if DEVELOPMENT_BUILD || UNITY_EDITOR
            {
                // Assert both length.
                if (sourceLength > Length)
                {
                    throw new InvalidOperationException("Destination buffer length must be larger than source buffer length.");
                }

                // Assert instance is valid.
                if (!IsValid)
                {
                    throw new InvalidOperationException("Array is empty, or not valid.");
                }
            }
#endif


            // Write data into unmanaged.
            fixed (int* sourceAddress = buffer)
            {
                for (var i = 0; i < sourceLength; ++i)
                {
                    destinationAddress[i] = sourceAddress[i];
                }
            }
        }
    }

    /// <summary>
    /// Byte array view.
    /// </summary>
    public sealed class CubismUnmanagedByteArrayView
    {
        /// <summary>
        /// Array length of unmanaged buffer.
        /// </summary>
        public int Length { get; private set; }

        /// <summary>
        /// Return true if instance is valid.
        /// </summary>
        public unsafe bool IsValid { get { return (UnmanagedFixedAddress != (Byte*)0) && (Length > 0); } }

        /// <summary>
        /// Gets element at index.
        /// </summary>
        /// <param name="index">Index of array.</param>
        /// <returns>Element of array.</returns>
        public unsafe Byte this[int index]
        {
            get
            {
                var pointer = UnmanagedFixedAddress;


#if DEVELOPMENT_BUILD || UNITY_EDITOR
                {
                    // Assert instance is valid.
                    if (!IsValid)
                    {
                        throw new InvalidOperationException("Array is empty, or not valid.");
                    }

                    if ((index >= Length) || (index < 0))
                    {
                        throw new IndexOutOfRangeException("Array index is out of range.");
                    }
                }
#endif


                return pointer[index];
            }

            set
            {
                var pointer = UnmanagedFixedAddress;


#if DEVELOPMENT_BUILD || UNITY_EDITOR
                {
                    // Assert instance is valid.
                    if (!IsValid)
                    {
                        throw new InvalidOperationException("Array is empty, or not valid.");
                    }

                    if ((index >= Length) || (index < 0))
                    {
                        throw new IndexOutOfRangeException("Array index is out of range.");
                    }
                }
#endif


                pointer[index] = value;
            }
        }


        /// <summary>
        /// Unmanaged buffer address.
        /// </summary>
        private unsafe Byte* UnmanagedFixedAddress { get; set; }

        #region Ctors

        /// <summary>
        /// Initializes instance.
        /// </summary>
        /// <param name="address">Unmanaged buffer address.</param>
        /// <param name="length">Length of unmanaged buffer (in types).</param>
        internal unsafe CubismUnmanagedByteArrayView(Byte* address, int length)
        {
            UnmanagedFixedAddress = address;
            Length = length;
        }

        /// <summary>
        /// Initializes instance.
        /// </summary>
        /// <param name="address">Unmanaged buffer address.</param>
        /// <param name="length">Length of unmanaged buffer (in types).</param>
        internal unsafe CubismUnmanagedByteArrayView(IntPtr address, int length)
        {
            UnmanagedFixedAddress = (Byte*)address.ToPointer();
            Length = length;
        }

        #endregion

        /// <summary>
        /// Reads data.
        /// </summary>
        /// <param name="buffer">Destination managed array.</param>
        public unsafe void Read(Byte[] buffer)
        {
            var sourceAddress = UnmanagedFixedAddress;
            var destinationLength = buffer.Length;


#if DEVELOPMENT_BUILD || UNITY_EDITOR
            {
                // Assert buffer.Length >= Length
                if (destinationLength < Length)
                {
                    throw new InvalidOperationException("Destination buffer length must be larger than source buffer length.");
                }

                // Assert instance is valid.
                if (!IsValid)
                {
                    throw new InvalidOperationException("Array is empty, or not valid.");
                }
            }
#endif


            // Read data into managed.
            fixed (Byte* destinationAddress = buffer)
            {
                for (var i = 0; i < Length; ++i)
                {
                    destinationAddress[i] = sourceAddress[i];
                }
            }
        }

        /// <summary>
        /// Writes data.
        /// </summary>
        /// <param name="buffer">Source managed array.</param>
        public unsafe void Write(Byte[] buffer)
        {
            var sourceLength = buffer.Length;
            var destinationAddress = UnmanagedFixedAddress;


#if DEVELOPMENT_BUILD || UNITY_EDITOR
            {
                // Assert both length.
                if (sourceLength > Length)
                {
                    throw new InvalidOperationException("Destination buffer length must be larger than source buffer length.");
                }

                // Assert instance is valid.
                if (!IsValid)
                {
                    throw new InvalidOperationException("Array is empty, or not valid.");
                }
            }
#endif


            // Write data into unmanaged.
            fixed (Byte* sourceAddress = buffer)
            {
                for (var i = 0; i < sourceLength; ++i)
                {
                    destinationAddress[i] = sourceAddress[i];
                }
            }
        }
    }

    /// <summary>
    /// Ushort array view.
    /// </summary>
    public sealed class CubismUnmanagedUshortArrayView
    {
        /// <summary>
        /// Array length of unmanaged buffer.
        /// </summary>
        public int Length { get; private set; }

        /// <summary>
        /// Return true if instance is valid.
        /// </summary>
        public unsafe bool IsValid { get { return (UnmanagedFixedAddress != (ushort*)0) && (Length > 0); } }

        /// <summary>
        /// Gets element at index.
        /// </summary>
        /// <param name="index">Index of array.</param>
        /// <returns>Element of array.</returns>
        public unsafe ushort this[int index]
        {
            get
            {
                var pointer = UnmanagedFixedAddress;


#if DEVELOPMENT_BUILD || UNITY_EDITOR
                {
                    // Assert instance is valid.
                    if (!IsValid)
                    {
                        throw new InvalidOperationException("Array is empty, or not valid.");
                    }

                    if ((index >= Length) || (index < 0))
                    {
                        throw new IndexOutOfRangeException("Array index is out of range.");
                    }
                }
#endif


                return pointer[index];
            }

            set
            {
                var pointer = UnmanagedFixedAddress;


#if DEVELOPMENT_BUILD || UNITY_EDITOR
                {
                    // Assert instance is valid.
                    if (!IsValid)
                    {
                        throw new InvalidOperationException("Array is empty, or not valid.");
                    }

                    if ((index >= Length) || (index < 0))
                    {
                        throw new IndexOutOfRangeException("Array index is out of range.");
                    }
                }
#endif


                pointer[index] = value;
            }
        }


        /// <summary>
        /// Unmanaged buffer address.
        /// </summary>
        private unsafe ushort* UnmanagedFixedAddress { get; set; }

        #region Ctors

        /// <summary>
        /// Initializes instance.
        /// </summary>
        /// <param name="address">Unmanaged buffer address.</param>
        /// <param name="length">Length of unmanaged buffer (in types).</param>
        internal unsafe CubismUnmanagedUshortArrayView(ushort* address, int length)
        {
            UnmanagedFixedAddress = address;
            Length = length;
        }

        /// <summary>
        /// Initializes instance.
        /// </summary>
        /// <param name="address">Unmanaged buffer address.</param>
        /// <param name="length">Length of unmanaged buffer (in types).</param>
        internal unsafe CubismUnmanagedUshortArrayView(IntPtr address, int length)
        {
            UnmanagedFixedAddress = (ushort*)address.ToPointer();
            Length = length;
        }

        #endregion

        /// <summary>
        /// Reads data.
        /// </summary>
        /// <param name="buffer">Destination managed array.</param>
        public unsafe void Read(ushort[] buffer)
        {
            var sourceAddress = UnmanagedFixedAddress;
            var destinationLength = buffer.Length;


#if DEVELOPMENT_BUILD || UNITY_EDITOR
            {
                // Assert buffer.Length >= Length
                if (destinationLength < Length)
                {
                    throw new InvalidOperationException("Destination buffer length must be larger than source buffer length.");
                }

                // Assert instance is valid.
                if (!IsValid)
                {
                    throw new InvalidOperationException("Array is empty, or not valid.");
                }
            }
#endif


            // Read data into managed.
            fixed (ushort* destinationAddress = buffer)
            {
                for (var i = 0; i < Length; ++i)
                {
                    destinationAddress[i] = sourceAddress[i];
                }
            }
        }

        /// <summary>
        /// Writes data.
        /// </summary>
        /// <param name="buffer">Source managed array.</param>
        public unsafe void Write(ushort[] buffer)
        {
            var sourceLength = buffer.Length;
            var destinationAddress = UnmanagedFixedAddress;


#if DEVELOPMENT_BUILD || UNITY_EDITOR
            {
                // Assert both length.
                if (sourceLength > Length)
                {
                    throw new InvalidOperationException("Destination buffer length must be larger than source buffer length.");
                }

                // Assert instance is valid.
                if (!IsValid)
                {
                    throw new InvalidOperationException("Array is empty, or not valid.");
                }
            }
#endif


            // Write data into unmanaged.
            fixed (ushort* sourceAddress = buffer)
            {
                for (var i = 0; i < sourceLength; ++i)
                {
                    destinationAddress[i] = sourceAddress[i];
                }
            }
        }
    }

}
