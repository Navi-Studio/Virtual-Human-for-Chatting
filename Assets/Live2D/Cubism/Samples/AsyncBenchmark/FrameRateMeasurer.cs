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
    /// <summary>
    /// Measure the frame rate.
    /// </summary>
    public class FrameRateMeasurer : MonoBehaviour
    {
        /// <summary>
        /// Target frame rate value.
        /// </summary>
        [SerializeField]
        public int TargetFrameRate = 60;

        /// <summary>
        /// Whether the model is spawnable or not.
        /// </summary>
        private bool LessThanTargetFrameRate { get; set; }

        /// <summary>
        /// The highest frame rate on running the application.
        /// </summary>
        private float HighestFrameRate { get; set; }

        /// <summary>
        /// Save the maximum frame rate.
        /// </summary>
        private int CurrentHighestFrameRate { get; set; }

        /// <summary>
        /// Save Previous Frame <see cref="CurrentHighestFrameRate"/>.
        /// </summary>
        private int PreviousHighestFrameRate { get; set; }

        /// <summary>
        /// The lowest frame rate on running the application.
        /// </summary>
        private float LowestFrameRate { get; set; }

        /// <summary>
        /// Save the minimum frame rate.
        /// </summary>
        private int CurrentLowestFrameRate { get; set; }

        /// <summary>
        /// Save Previous Frame <see cref="CurrentLowestFrameRate"/>.
        /// </summary>
        private int PreviousLowestFrameRate { get; set; }

        /// <summary>
        /// Get Model Instances Count.
        /// </summary>
        private int InstancesCount { get; set; }

        /// <summary>
        /// <see cref="AsyncBenchmark.FpsCounter"/> Component.
        /// </summary>
        private FpsCounter FpsCounter { get; set; }

        /// <summary>
        /// <see cref="AsyncBenchmark.BenchmarkController"/> Component.
        /// </summary>
        private BenchmarkController BenchmarkController { get; set; }

        /// <summary>
        /// <see cref="AsyncBenchmark.ModelSpawner"/> Conponent.
        /// </summary>
        private ModelSpawner ModelSpawner { get; set; }

        /// <summary>
        /// <see cref="AsyncBenchmark.TotalElapsedTime"/> Component.
        /// </summary>
        private TotalElapsedTime TotalElapsedTime { get; set; }

        /// <summary>
        /// <see cref="AsyncBenchmark.FrameRateUiHolder"/> Component.
        /// </summary>
        private FrameRateUiHolder FrameRateUiHolder { get; set; }

        /// <summary>
        /// Displays the frame rate and observation time when the maximum frame rate is observed.
        /// </summary>
        private Text HighestFrameRateUi { get; set; }

        /// <summary>
        /// Displays the frame rate and observation time when the minimum frame rate is observed.
        /// </summary>
        private Text LowestFrameRateUi { get; set; }

        /// <summary>
        /// Called by Unity. Getting target component and Initializing.
        /// </summary>
        private void Start()
        {
            // Getting components and initializing.
            FpsCounter = GetComponent<FpsCounter>();
            BenchmarkController = GetComponent<BenchmarkController>();
            ModelSpawner = GetComponent<ModelSpawner>();
            TotalElapsedTime = GetComponent<TotalElapsedTime>();
            FrameRateUiHolder = GetComponent<FrameRateUiHolder>();

            HighestFrameRateUi = FrameRateUiHolder.HighestFrameRateUi;
            LowestFrameRateUi = FrameRateUiHolder.LowestFrameRateUi;

            /// If <see cref="BenchmarkController"/> is present, get <see cref="BenchmarkController.TargetFrameRate"/>.
            TargetFrameRate = BenchmarkController != null
                ? BenchmarkController.TargetFrameRate
                : TargetFrameRate;
        }

        // Update is called once per frame
        private void Update()
        {
            /// Get value from <see cref="FpsCounter"/> Component.
            var fps = Mathf.FloorToInt(FpsCounter.Fps);

            // Compare the measured value with the maximum value so far.
            CurrentHighestFrameRate = fps > CurrentHighestFrameRate
                ? fps
                : CurrentHighestFrameRate;

            // Compare the measured value with the maximum value so far.
            CurrentLowestFrameRate = fps < CurrentLowestFrameRate
                ? fps
                : CurrentLowestFrameRate;

            // Whether the recorded frame rate has reached the target frame rate.
            LessThanTargetFrameRate = TargetFrameRate > CurrentHighestFrameRate;

            // When the observed value is lower than the set value.
            if (LessThanTargetFrameRate)
            {
                // Assign the maximum and minimum values.
                HighestFrameRate = HighestFrameRate < CurrentHighestFrameRate
                    ? CurrentHighestFrameRate
                    : HighestFrameRate;

                // Assign the maximum and minimum values.
                LowestFrameRate = LowestFrameRate > CurrentLowestFrameRate
                    ? CurrentLowestFrameRate
                    : LowestFrameRate;

                // Has the values been changed?
                var isMaximumFrameRateChange = (HighestFrameRate == CurrentHighestFrameRate) && (PreviousHighestFrameRate != CurrentHighestFrameRate);
                var isMinimumFrameRateChange = (LowestFrameRate == CurrentLowestFrameRate) && (PreviousLowestFrameRate != CurrentLowestFrameRate);

                var timeConversion = TimeConversion(TotalElapsedTime.ElapsedTime);

                // Update ui.
                if (isMaximumFrameRateChange)
                {
                    var maximumObservationFrameRateText = string.Format("max ({0} fps)\n", HighestFrameRate);
                    HighestFrameRateUi.text = string.Concat(maximumObservationFrameRateText, timeConversion);
                    PreviousHighestFrameRate = CurrentHighestFrameRate;
                }

                if (isMinimumFrameRateChange)
                {
                    var minimumObservationFrameRateText = string.Format("min ({0} fps)\n", LowestFrameRate);
                    LowestFrameRateUi.text = string.Concat(minimumObservationFrameRateText, timeConversion);
                    PreviousLowestFrameRate = CurrentLowestFrameRate;
                }
            }

            // When the observed value is higher than the set value.
            else
            {
                // Reset variables.
                CurrentHighestFrameRate = 0;
                PreviousHighestFrameRate = CurrentHighestFrameRate;
                CurrentLowestFrameRate = TargetFrameRate;
                PreviousLowestFrameRate = CurrentLowestFrameRate;
            }

            var storeInstancesCount = InstancesCount;

            /// Get Instances Count from <see cref="ModelSpawner"/> Component
            InstancesCount = ModelSpawner.InstancesCount;

            if (storeInstancesCount != InstancesCount || InstancesCount == 0)
            {
                var timeConversion = TimeConversion(0);

                // Reset ui.
                var highestFrameRateText = string.Format("max (0 fps)\n");
                HighestFrameRateUi.text = string.Concat(highestFrameRateText, timeConversion);

                var lowesttFrameRateText = string.Format("min (0 fps)\n");
                LowestFrameRateUi.text = string.Concat(lowesttFrameRateText, timeConversion);

                // Reset variables.
                CurrentHighestFrameRate = 0;
                PreviousHighestFrameRate = CurrentHighestFrameRate;
                HighestFrameRate = 0;
                CurrentLowestFrameRate = TargetFrameRate;
                PreviousLowestFrameRate = CurrentLowestFrameRate;
                LowestFrameRate = TargetFrameRate;
            }
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
