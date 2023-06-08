/**
 * Copyright(c) Live2D Inc. All rights reserved.
 *
 * Use of this source code is governed by the Live2D Open Software license
 * that can be found at https://www.live2d.com/eula/live2d-open-software-license-agreement_en.html.
 */


using UnityEngine;
using UnityEngine.UI;

namespace Live2D.Cubism.Samples.AsyncBenchmark
{
    /// <summary>
    /// Record when the frame rate falls below the set target frame rate.
    /// </summary>
    public class FrameRateUiHolder : MonoBehaviour
    {
        /// <summary>
        /// Enable/disable observation.
        /// </summary>
        [SerializeField]
        public bool HasShownFrameRate;

        /// <summary>
        /// Whether to enable total uptime.
        /// </summary>
        [SerializeField]
        public bool HasShownElapsedTime;

        /// <summary>
        /// Displays the frame rate and observation time when the maximum frame rate is observed.
        /// </summary>
        [SerializeField]
        public Text HighestFrameRateUi = null;

        /// <summary>
        /// Displays the frame rate and observation time when the minimum frame rate is observed.
        /// </summary>
        [SerializeField]
        public Text LowestFrameRateUi = null;

        /// <summary>
        /// UI to display total benchmark uptime.
        /// </summary>
        [SerializeField]
        public Text ElapsedTimeUi = null;
    }
}
