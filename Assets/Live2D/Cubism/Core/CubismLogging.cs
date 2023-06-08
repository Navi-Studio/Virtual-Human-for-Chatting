/**
 * Copyright(c) Live2D Inc. All rights reserved.
 *
 * Use of this source code is governed by the Live2D Open Software license
 * that can be found at https://www.live2d.com/eula/live2d-open-software-license-agreement_en.html.
 */


using AOT;
using Live2D.Cubism.Core.Unmanaged;
using System;
using System.Runtime.InteropServices;
using UnityEngine;


namespace Live2D.Cubism.Core
{
    /// <summary>
    /// Wrapper for core logs.
    /// </summary>
    internal static class CubismLogging
    {
        #region Delegates

        /// <summary>
        /// Delegate compatible with unmanaged log function.
        /// </summary>
        /// <param name="message">Message to log.</param>
        private unsafe delegate void UnmanagedLogDelegate(char* message);

        #endregion

        /// <summary>
        /// Delegate to pass to native Api.
        /// </summary>
        // ReSharper disable once PrivateFieldCanBeConvertedToLocalVariable
        private static UnmanagedLogDelegate LogDelegate { get; set; }

        #region Initialization

        /// <summary>
        /// Registers delegates.
        /// </summary>
        [RuntimeInitializeOnLoadMethod]
        // ReSharper disable once UnusedMember.Local
        private static unsafe void Initialize()
        {
            LogDelegate = LogUnmanaged;


            var logFunction = Marshal.GetFunctionPointerForDelegate(LogDelegate);


            csmSetLogFunction(logFunction);
        }

        #endregion

        /// <summary>
        /// Prints an unmanaged, null-terminated message.
        /// </summary>
        /// <param name="message">Message to log.</param>
        [MonoPInvokeCallback(typeof(UnmanagedLogDelegate))]
        private static unsafe void LogUnmanaged(char* message)
        {
            // Marshal message and log it.
            var managedMessage = Marshal.PtrToStringAnsi(new IntPtr(message));


            Debug.LogFormat("[Cubism] Core: {0}.", managedMessage);
        }

        #region Extern C

        [DllImport(CubismCoreDll.DllName)]
        private static extern void csmSetLogFunction(IntPtr logFunction);

        #endregion
    }
}
