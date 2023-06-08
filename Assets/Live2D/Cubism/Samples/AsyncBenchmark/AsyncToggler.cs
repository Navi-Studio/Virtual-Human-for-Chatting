/**
 * Copyright(c) Live2D Inc. All rights reserved.
 *
 * Use of this source code is governed by the Live2D Open Software license
 * that can be found at https://www.live2d.com/eula/live2d-open-software-license-agreement_en.html.
 */


using Live2D.Cubism.Framework.Tasking;
using UnityEngine;


namespace Live2D.Cubism.Samples.AsyncBenchmark
{
    /// <summary>
    /// Shows how to enable the <see cref="CubismBuiltinAsyncTaskHandler"/> from script.
    /// </summary>
    public sealed class AsyncToggler : MonoBehaviour
    {
        /// <summary>
        /// Controls async task handling.
        /// </summary>
        public bool EnableAsync = true;

        /// <summary>
        /// Last <see cref="EnableAsync"/> state.
        /// </summary>
        private bool LastEnableSync { get; set; }

       #region Unity Event Handling

        /// <summary>
        /// Called by Unity. Enables/Disables async task handler.
        /// </summary>
        private void Update()
        {
            if (EnableAsync == LastEnableSync)
            {
                return;
            }


            if (EnableAsync)
            {
                CubismBuiltinAsyncTaskHandler.Activate();
            }
            else
            {
                CubismBuiltinAsyncTaskHandler.Deactivate();
            }


            LastEnableSync = EnableAsync;
        }


        /// <summary>
        /// Called by Unity. Disables async task handler.
        /// </summary>
        private void OnDestroy()
        {
            EnableAsync = false;


            Update();
        }

        #endregion
    }
}
