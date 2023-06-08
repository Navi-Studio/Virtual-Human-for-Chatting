/**
 * Copyright(c) Live2D Inc. All rights reserved.
 *
 * Use of this source code is governed by the Live2D Open Software license
 * that can be found at https://www.live2d.com/eula/live2d-open-software-license-agreement_en.html.
 */


using System;
using UnityEngine;
using UnityEngine.UI;

namespace Live2D.Cubism.Samples.AsyncBenchmark
{
    public class TotalElapsedTime : MonoBehaviour
    {
        /// <summary>
        /// Interval time before the model can spawn.
        /// </summary>
        public readonly int UpdateInterval = 1;

        /// <summary>
        /// Total benchmark uptime.
        /// </summary>
        [SerializeField, HideInInspector]
        public int ElapsedTime = 0;

        /// <summary>
        /// Add delta time.
        /// </summary>
        private float UpdateIntervalCount { get; set; }

        /// <summary>
        /// UI to display total benchmark uptime.
        /// </summary>
        private Text TotalElapsedTimeText { get; set; }

        /// <summary>
        /// <see cref="AsyncBenchmark.FrameRateUiHolder"/> Component.
        /// </summary>
        private FrameRateUiHolder FrameRateUiHolder { get; set; }

        /// <summary>
        /// Called by Unity. Getting FpsObservation Component and Getting Component from FpsObservation.
        /// </summary>
        private void Start()
        {
            FrameRateUiHolder = GetComponent<FrameRateUiHolder>();
            TotalElapsedTimeText = FrameRateUiHolder.ElapsedTimeUi;
        }

        /// <summary>
        /// Called by Unity. Update Total Operating Time.
        /// </summary>
        private void Update()
        {
            // Whether you have time to make a decision.
            if (UpdateIntervalCount < UpdateInterval)
            {
                UpdateIntervalCount += Time.deltaTime;
                return;
            }

            // Update total benchmark uptime.
            ElapsedTime += UpdateInterval;

            if (TotalElapsedTimeText != null)
            {
                TotalElapsedTimeText.text = TimeConversion(ElapsedTime);
            }

            // Reset variable.
            UpdateIntervalCount = 0.0f;
        }

        /// <summary>
        /// Convert seconds to "hours:minutes:seconds".
        /// </summary>
        /// <param name="second">Number of seconds it conversion source.</param>
        /// <returns>String type converted to "hours:minutes:seconds" notation.</returns>
        private string TimeConversion(int second)
        {
            // Generate TimeSpan structure type.
            var timeSpan = new TimeSpan(0, 0, second);

            return timeSpan.ToString();
        }
    }
}
