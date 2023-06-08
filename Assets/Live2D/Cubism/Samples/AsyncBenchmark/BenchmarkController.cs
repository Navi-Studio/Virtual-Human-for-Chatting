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
    /// Automatically adjust the fps to the set value.
    /// </summary>
    public sealed class BenchmarkController : MonoBehaviour
    {
        /// <summary>
        /// Interval time before the model can spawn.
        /// </summary>
        public readonly float SpawnIntervalTimeSecond = 1.0f;

        /// <summary>
        /// Target frame rate value.
        /// </summary>
        [SerializeField]
        public int TargetFrameRate = 60;

        /// <summary>
        /// UI for displaying <see cref="ElapsedTime"/> values.
        /// </summary>
        [SerializeField]
        public Text ReachedElapsedTimeUi = null;

        /// <summary>
        /// UI to display the number of instances of the model when the target frame rate is finally reached.
        /// </summary>
        [SerializeField]
        public Text InstancesCountUi = null;

        /// <summary>
        /// Save the maximum frame rate.
        /// </summary>
        private float HighestRecordedFrameRate { get; set; }

        /// <summary>
        /// Whether the model is spawnable or not.
        /// </summary>
        private bool CanModelSpawn { get; set; }

        /// <summary>
        /// Add delta time.
        /// </summary>
        private float SpawnTimeCount { get; set; }

        /// <summary>
        /// Time elapsed since <see cref="CanModelSpawn"/> was set to false.
        /// </summary>
        private float ElapsedTime { get; set; }

        /// <summary>
        /// <see cref="AsyncBenchmark.FpsCounter"/> Component.
        /// </summary>
        private FpsCounter FpsCounter { get; set; }

        /// <summary>
        /// <see cref="AsyncBenchmark.ModelSpawner"/> Conponent.
        /// </summary>
        private ModelSpawner ModelSpawner { get; set; }

        /// <summary>
        /// Called by Unity. Setting vsync and target frame rate.
        /// </summary>
        private void Awake()
        {
            // Setting vsync and targetFrameRate.
            QualitySettings.vSyncCount = 0;
            Application.targetFrameRate = TargetFrameRate + 1;

            // Getting the component.
            FpsCounter = GetComponent<FpsCounter>();
            ModelSpawner = GetComponent<ModelSpawner>();
        }

        /// <summary>
        /// Called by Unity. Record the maximum frame rate and manage model spawning.
        /// </summary>
        private void Update()
        {
            RecordFrameRate();
            ManageSpawn();
        }

        /// <summary>
        /// Records the maximum frame rate within a given time period.
        /// </summary>
        private void RecordFrameRate()
        {
            /// Get value from <see cref="FpsCounter"/> Component.
            var fps = FpsCounter.Fps;

            // Compare the measured value with the maximum value so far.
            HighestRecordedFrameRate = fps >= HighestRecordedFrameRate
                ? fps
                : HighestRecordedFrameRate;

            // Whether you have time to make a decision.
            if (SpawnTimeCount < SpawnIntervalTimeSecond)
            {
                SpawnTimeCount += Time.deltaTime;
                return;
            }

            // If the model is not ready to spawn, add the elapsed time.
            if (!CanModelSpawn && (ModelSpawner.InstancesCount != 0))
            {
                ElapsedTime += SpawnIntervalTimeSecond;

                // Combine strings and display them in UI.
                var elapsedTimeString = TimeConversion(Mathf.FloorToInt(ElapsedTime));
                ReachedElapsedTimeUi.text = string.Format(" Reached Time:{0}", elapsedTimeString);
            }

            // Whether the recorded frame rate has reached the target frame rate.
            CanModelSpawn = TargetFrameRate <= HighestRecordedFrameRate;

            // Reset variables and properties.
            SpawnTimeCount = 0.0f;
            HighestRecordedFrameRate = 0.0f;
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

        /// <summary>
        /// Managing model spawn.
        /// </summary>
        private void ManageSpawn()
        {
            if (SpawnTimeCount < SpawnIntervalTimeSecond)
            {
                return;
            }

            // When the model can spawn
            if (CanModelSpawn)
            {
                // Spawn the model.
                ModelSpawner.IncreaseInstances();

                // Reset variable.
                ElapsedTime = 0;
            }

            // When the model can't spawn
            else
            {
                /// Get Instances Count from <see cref="ModelSpawner"/> Component and update UI.
                var instancesCount = ModelSpawner.InstancesCount;
                InstancesCountUi.text = string.Format(" Reached Model Count:{0}", instancesCount.ToString());
            }
        }
    }
}
