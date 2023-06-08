/**
 * Copyright(c) Live2D Inc. All rights reserved.
 *
 * Use of this source code is governed by the Live2D Open Software license
 * that can be found at https://www.live2d.com/eula/live2d-open-software-license-agreement_en.html.
 */


using UnityEngine;


namespace Live2D.Cubism.Framework.MouthMovement
{
    /// <summary>
    /// Real-time <see cref="CubismMouthController"/> input from <see cref="AudioSource"/>s.
    /// </summary>
    [RequireComponent(typeof(CubismMouthController))]
    public sealed class CubismAudioMouthInput : MonoBehaviour
    {
        /// <summary>
        /// Audio source to sample.
        /// </summary>
        [SerializeField]
        public AudioSource AudioInput;


        /// <summary>
        /// Sampling quality.
        /// </summary>
        [SerializeField]
        public CubismAudioSamplingQuality SamplingQuality;


        /// <summary>
        /// Audio gain.
        /// </summary>
        [Range(1.0f, 10.0f)]
        public float Gain = 1.0f;

        /// <summary>
        /// Smoothing.
        /// </summary>
        [Range(0.0f, 1.0f)]
        public float Smoothing;


        /// <summary>
        /// Current samples.
        /// </summary>
        private float[] Samples { get; set; }

        /// <summary>
        /// Last root mean square.
        /// </summary>
        private float LastRms { get; set; }

        /// <summary>
        /// Buffer for <see cref="Mathf.SmoothDamp(float, float, ref float, float)"/> velocity.
        /// </summary>
        // ReSharper disable once InconsistentNaming
        private float VelocityBuffer;

        /// <summary>
        /// Targeted <see cref="CubismMouthController"/>.
        /// </summary>
        private CubismMouthController Target { get; set; }


        /// <summary>
        /// True if instance is initialized.
        /// </summary>
        private bool IsInitialized
        {
            get { return Samples != null; }
        }


        /// <summary>
        /// Makes sure instance is initialized.
        /// </summary>
        private void TryInitialize()
        {
            // Return early if already initialized.
            if (IsInitialized)
            {
                return;
            }


            // Initialize samples buffer.
            switch (SamplingQuality)
            {
                case (CubismAudioSamplingQuality.VeryHigh):
                {
                        Samples = new float[256];


                        break;
                    }
                case (CubismAudioSamplingQuality.Maximum):
                {
                    Samples = new float[512];


                    break;
                }
                default:
                {
                    Samples = new float[256];


                    break;
                }
            }


            // Cache target.
            Target = GetComponent<CubismMouthController>();
        }

        #region Unity Event Handling

        /// <summary>
        /// Samples audio input and applies it to mouth controller.
        /// </summary>
        private void Update()
        {
            // 'Fail' silently.
            if (AudioInput == null)
            {
                return;
            }


            // Sample audio.
            var total = 0f;


            AudioInput.GetOutputData(Samples, 0);


            for (var i = 0; i < Samples.Length; ++i)
            {
                var sample = Samples[i];


                total += (sample * sample);
            }


            // Compute root mean square over samples.
            var rms = Mathf.Sqrt(total / Samples.Length) * Gain;


            // Clamp root mean square.
            rms = Mathf.Clamp(rms, 0.0f, 1.0f);


            // Smooth rms.
            rms = Mathf.SmoothDamp(LastRms, rms, ref VelocityBuffer, Smoothing * 0.1f);


            // Set rms as mouth opening and store it for next evaluation.
            Target.MouthOpening = rms;


            LastRms = rms;
        }


        /// <summary>
        /// Initializes instance.
        /// </summary>
        private void OnEnable()
        {
            TryInitialize();
        }

        #endregion
    }
}
