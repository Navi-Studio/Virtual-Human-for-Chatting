/**
 * Copyright(c) Live2D Inc. All rights reserved.
 *
 * Use of this source code is governed by the Live2D Open Software license
 * that can be found at https://www.live2d.com/eula/live2d-open-software-license-agreement_en.html.
 */


using Live2D.Cubism.Core;
using Live2D.Cubism.Rendering;
using System.Collections.Generic;
using Live2D.Cubism.Framework.Tasking;
using UnityEngine;
using UnityEngine.UI;
using Random = System.Random;


namespace Live2D.Cubism.Samples.AsyncBenchmark
{
    /// <summary>
    /// Spawns models for benchmarking.
    /// </summary>
    public sealed class ModelSpawner : MonoBehaviour
    {
        /// <summary>
        /// <see cref="CubismModel"/> prefab to spawn.
        /// </summary>
        [SerializeField]
        public GameObject ModelPrefab;


        /// <summary>
        /// UI component representing current model count.
        /// </summary>
        [SerializeField]
        public Text ModelCountUi;

        /// <summary>
        /// Holds the number of instances of the model.
        /// </summary>
        public int InstancesCount { get; private set; }

        /// <summary>
        /// Model instances.
        /// </summary>
        private List<GameObject> Instances { get; set; }

        /// <summary>
        /// <see cref="AsyncBenchmark.BenchmarkController"/> Component.
        /// </summary>
        private BenchmarkController BenchmarkController { get; set; }

        #region Interface for UI Elements

        /// <summary>
        /// Adds a new instance.
        /// </summary>
        public void IncreaseInstances()
        {
            if (ModelPrefab == null)
            {
                return;
            }

            // Spawn new instance.
            var instance = Instantiate(ModelPrefab);


            var random = new Random();
            var offsetX = (float)random.Next(-1000, 1000) / 1000f;
            var offsetY = (float)random.Next(-1000, 1000) / 1000f;


            var screenToWorld = Camera.main.ScreenToWorldPoint(
                new Vector3(
                    Screen.width,
                    Screen.height,
                    Camera.main.nearClipPlane));


            instance.transform.position = new Vector3(
                screenToWorld.x * offsetX,
                screenToWorld.y * offsetY,
                instance.transform.position.z);


            // Register instance and update UI.
            Instances.Add(instance);


            // Make sure to assign a unique sorting order to the instance.
            instance.GetComponent<CubismRenderController>().SortingOrder = Instances.Count;

            // Update propertie.
            InstancesCount = Instances.Count;

            // Update UI.
            ModelCountUi.text = BenchmarkController == null
                ? Instances.Count.ToString()
                : string.Concat("Current Model Count:", Instances.Count.ToString());
        }

        /// <summary>
        /// Removes an instance.
        /// </summary>
        public void DecreaseInstances()
        {
            // Return early if there's nothing to decrease.
            if (Instances.Count == 0)
            {
                return;
            }


            // Remove last instance and update UI.
            DestroyImmediate(Instances[Instances.Count - 1]);
            Instances.RemoveAt(Instances.Count - 1);


            ModelCountUi.text = Instances.Count.ToString();
        }

        #endregion

        #region Unity Event Handling

        /// <summary>
        /// Called by Unity. Initializes fields.
        /// </summary>
        private void Start()
        {
            Instances = new List<GameObject>();
            BenchmarkController = GetComponent<BenchmarkController>();
        }

        #endregion
    }
}
