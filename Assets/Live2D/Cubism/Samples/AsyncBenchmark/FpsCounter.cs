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
    /// Measures Fps for on-screen display.
    /// </summary>
    public sealed class FpsCounter : MonoBehaviour
    {
        /// <summary>
        /// UI component representing current model count.
        /// </summary>
        [SerializeField]
        public Text FpsUi;

        /// <summary>
        /// Frame rate propertie to get from external sources.
        /// </summary>
        public float Fps { get; private set; }

        /// <summary>
        /// Time for FPS calculation.
        /// </summary>
        private float DeltaTime { get; set; }

        #region Unity Event Handling

        /// <summary>
        /// Called by Unity. Initializes fields.
        /// </summary>
        private void Update()
        {
            // Update delta time.
            DeltaTime += (Time.deltaTime - DeltaTime) * 0.1f;


            // Compute FPS and update UI.
            var fps = 1.0f / DeltaTime;

            // Save the value to the property.
            Fps = fps;

            FpsUi.text = string.Format("({0:0.} fps)", fps);
        }

        #endregion
    }
}
