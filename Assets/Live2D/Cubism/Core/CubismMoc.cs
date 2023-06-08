/**
 * Copyright(c) Live2D Inc. All rights reserved.
 *
 * Use of this source code is governed by the Live2D Open Software license
 * that can be found at https://www.live2d.com/eula/live2d-open-software-license-agreement_en.html.
 */


using System;
using Live2D.Cubism.Core.Unmanaged;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif


namespace Live2D.Cubism.Core
{
    /// <summary>
    /// Cubism moc asset.
    /// </summary>
    public sealed class CubismMoc : ScriptableObject
    {
        #region Factory Methods

        /// <summary>
        /// Checks consistency of a moc.
        /// </summary>
        public static bool HasMocConsistency(byte[] moc3)
        {
            return CubismUnmanagedMoc.HasMocConsistency(moc3);
        }

        /// <summary>
        /// Creates a <see cref="CubismMoc"/> asset from raw bytes.
        /// </summary>
        /// <param name="moc3">Source.</param>
        /// <returns>Instance.</returns>
        public static CubismMoc CreateFrom(byte[] moc3)
        {
            var moc = CreateInstance<CubismMoc>();


            moc.Bytes = moc3;


            return moc;
        }

        #endregion

        /// <summary>
        /// Resets native handle.
        /// </summary>
        /// <param name="moc"></param>
        public static void ResetUnmanagedMoc(CubismMoc moc)
        {
            moc.UnmanagedMoc = null;


            moc.Revive();
        }

        /// <summary>
        /// Gets the latest .moc3 file version.
        /// </summary>
        public static uint LatestVersion
        {
            get
            {
                return CubismCoreDll.GetLatestMocVersion();
            }
        }

        /// <summary>
        /// <see cref="Bytes"/> backing field.
        /// </summary>
        [SerializeField]
        private byte[] _bytes;

        /// <summary>
        /// Raw moc bytes.
        /// </summary>
        private byte[] Bytes
        {
            get { return _bytes; }
            set { _bytes = value; }
        }


        private CubismUnmanagedMoc UnmanagedMoc { get; set; }

        private int ReferenceCount { get; set; }

#if UNITY_EDITOR
        private static int CoreNotFoundCallCount { get; set; }
#endif


        /// <summary>
        /// True if instance is revived.
        /// </summary>
        public bool IsRevived
        {
            get
            {
                return UnmanagedMoc != null;
            }
        }


        /// <summary>
        /// Acquires native handle.
        /// </summary>
        /// <returns>Valid handle on success; <see cref="IntPtr.Zero"/> otherwise.</returns>
        public CubismUnmanagedMoc AcquireUnmanagedMoc()
        {
            ++ReferenceCount;


#if UNITY_EDITOR
            try
            {
#endif
                Revive();
#if UNITY_EDITOR
            }
            catch (DllNotFoundException)
            {
                if (CoreNotFoundCallCount == 0)
                {
                    EditorUtility.DisplayDialog("Live2D CubismCore is not loaded", "Please reboot this Unity project if it is just after import of the SDK. If it's not, please check if platform settings of dll is correct. dll cannot be used on platform which is different from its own build settings.", "ok", "cancel");
                }
                ++CoreNotFoundCallCount;
            }
#endif


            return UnmanagedMoc;
        }

        /// <summary>
        /// Releases native handle.
        /// </summary>
        public void ReleaseUnmanagedMoc()
        {
            -- ReferenceCount;


            // Release instance of unmanaged moc in case the instance isn't referenced any longer.
            if (ReferenceCount == 0)
            {
                UnmanagedMoc.Release();
                UnmanagedMoc = null;
            }


            // Deal with invalid reference counts.
            else if (ReferenceCount < 0)
            {
                ReferenceCount = 0;
            }
        }


        /// <summary>
        /// Revives instance without acquiring it.
        /// </summary>
        private void Revive()
        {
            // Return if already revived.
            if (IsRevived)
            {
                return;
            }


            // Return if no bytes are available.
            if (Bytes == null)
            {
                return;
            }


            // Try revive.
            UnmanagedMoc = CubismUnmanagedMoc.FromBytes(Bytes);
        }

        public uint Version
        {
            get
            {
                return UnmanagedMoc.MocVersion;
            }
        }
    }
}
