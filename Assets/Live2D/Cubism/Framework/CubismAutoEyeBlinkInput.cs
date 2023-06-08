/**
 * Copyright(c) Live2D Inc. All rights reserved.
 *
 * Use of this source code is governed by the Live2D Open Software license
 * that can be found at https://www.live2d.com/eula/live2d-open-software-license-agreement_en.html.
 */


using UnityEngine;


namespace Live2D.Cubism.Framework
{
    /// <summary>
    /// Automatic mouth movement.
    /// </summary>
    public sealed class CubismAutoEyeBlinkInput : MonoBehaviour
    {
        /// <summary>
        /// Mean time between eye blinks in seconds.
        /// </summary>
        [SerializeField, Range(1f, 10f)]
        public float Mean = 2.5f;

        /// <summary>
        /// Maximum deviation from <see cref="Mean"/> in seconds.
        /// </summary>
        [SerializeField, Range(0.5f, 5f)]
        public float MaximumDeviation = 2f;

        /// <summary>
        /// Timescale.
        /// </summary>
        [SerializeField, Range(1f, 20f)]
        public float Timescale = 10f;


        /// <summary>
        /// Target controller.
        /// </summary>
        private CubismEyeBlinkController Controller { get; set; }

        /// <summary>
        /// Time until next eye blink.
        /// </summary>
        private float T { get; set; }

        /// <summary>
        /// Control over whether output should be evaluated.
        /// </summary>
        private Phase CurrentPhase { get; set; }

        /// <summary>
        /// Used for switching from <see cref="Phase.ClosingEyes"/> to <see cref="Phase.OpeningEyes"/> and back to <see cref="Phase.Idling"/>.
        /// </summary>
        private float LastValue { get; set; }


        /// <summary>
        /// Resets the input.
        /// </summary>
        public void Reset()
        {
            T = 0f;
        }

        #region Unity Event Handling

        /// <summary>
        /// Called by Unity. Initializes input.
        /// </summary>
        private void Start()
        {
            Controller = GetComponent<CubismEyeBlinkController>();
        }


        /// <summary>
        /// Called by Unity. Updates controller.
        /// </summary>
        /// <remarks>
        /// Make sure this method is called after any animations are evaluated.
        /// </remarks>
        private void LateUpdate()
        {
            // Fail silently.
            if (Controller == null)
            {
                return;
            }


            // Wait for time until blink.
            if (CurrentPhase == Phase.Idling)
            {
                T -= Time.deltaTime;


                if (T < 0f)
                {
                    T = (Mathf.PI * -0.5f);
                    LastValue = 1f;
                    CurrentPhase = Phase.ClosingEyes;
                }
                else
                {
                    return;
                }
            }


            // Evaluate eye blinking.
            T += (Time.deltaTime * Timescale);
            var value = Mathf.Abs(Mathf.Sin(T));


            if (CurrentPhase == Phase.ClosingEyes && value > LastValue)
            {
                CurrentPhase = Phase.OpeningEyes;
            }
            else if (CurrentPhase == Phase.OpeningEyes && value < LastValue)
            {
                value = 1f;
                CurrentPhase = Phase.Idling;
                T = Mean + Random.Range(-MaximumDeviation, MaximumDeviation);
            }


            Controller.EyeOpening = value;
            LastValue = value;
        }

        #endregion

        /// <summary>
        /// Internal states.
        /// </summary>
        private enum Phase
        {
            /// <summary>
            /// Idle state.
            /// </summary>
            Idling,

            /// <summary>
            /// State when closing eyes.
            /// </summary>
            ClosingEyes,

            /// <summary>
            /// State when opening eyes.
            /// </summary>
            OpeningEyes
        }
    }
}
