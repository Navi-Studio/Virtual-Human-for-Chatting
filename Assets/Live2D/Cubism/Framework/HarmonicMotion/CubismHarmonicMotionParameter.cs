/**
 * Copyright(c) Live2D Inc. All rights reserved.
 *
 * Use of this source code is governed by the Live2D Open Software license
 * that can be found at https://www.live2d.com/eula/live2d-open-software-license-agreement_en.html.
 */


using Live2D.Cubism.Core;
using UnityEngine;


namespace Live2D.Cubism.Framework.HarmonicMotion
{
    /// <summary>
    /// Holds data for controlling the output of simple harmonic motions.
    /// </summary>
    /// <remarks>
    /// This type of motion can be very useful for faking breathing, for example.
    /// </remarks>
    public sealed class CubismHarmonicMotionParameter : MonoBehaviour
    {
        /// <summary>
        /// Timescale channel.
        /// </summary>
        [SerializeField]
        public int Channel;

        /// <summary>
        /// Motion direction.
        /// </summary>
        [SerializeField]
        public CubismHarmonicMotionDirection Direction;

        /// <summary>
        /// Normalized origin of motion.
        /// </summary>
        /// <remarks>
        /// The actual origin used for evaluating the motion depends limits of the <see cref="CubismParameter"/>.
        /// </remarks>
        [SerializeField, Range(0f, 1f)]
        public float NormalizedOrigin = 0.5f;

        /// <summary>
        /// Normalized range of motion.
        /// </summary>
        /// <remarks>
        /// The actual origin used for evaluating the motion depends limits of the <see cref="CubismParameter"/>.
        /// </remarks>
        [SerializeField, Range(0f, 1f)]
        public float NormalizedRange = 0.5f;

        /// <summary>
        /// Duration of one motion cycle in seconds.
        /// </summary>
        [SerializeField, Range(0.01f, 10f)]
        public float Duration = 3f;


        /// <summary>
        /// <see langword="true"/> if <see langword="this"/> is initialized.
        /// </summary>
        private bool IsInitialized
        {
            get { return Mathf.Abs(ValueRange) >= Mathf.Epsilon; }
        }


        /// <summary>
        /// Initializes instance.
        /// </summary>
        private void Initialize()
        {
            // Initialize value fields.
            var parameter = GetComponent<CubismParameter>();


            MaximumValue = parameter.MaximumValue;
            MinimumValue = parameter.MinimumValue;
            ValueRange = MaximumValue - MinimumValue;
        }

        #region Interface for Controller

        /// <summary>
        /// Cached <see cref="CubismParameter.MaximumValue"/>.
        /// </summary>
        private float MaximumValue { get; set; }

        /// <summary>
        /// Cached <see cref="CubismParameter.MinimumValue"/>.
        /// </summary>
        private float MinimumValue { get; set; }

        /// <summary>
        /// Range of <see cref="MaximumValue"/> and <see cref="MinimumValue"/>.
        /// </summary>
        private float ValueRange { get; set; }


        /// <summary>
        /// Current time.
        /// </summary>
        private float T { get; set; }


        /// <summary>
        /// Proceeds time.
        /// </summary>
        /// <param name="channelTimescales"></param>
        internal void Play(float[] channelTimescales)
        {
            T += (Time.deltaTime * channelTimescales[Channel]);


            // Make sure time stays within duration.
            while (T > Duration)
            {
                T -= Duration;
            }
        }

        /// <summary>
        /// Evaluates the parameter.
        /// </summary>
        /// <returns>Parameter value.</returns>
        internal float Evaluate()
        {
            // Lazily initialize.
            if (!IsInitialized)
            {
                Initialize();
            }


            // Restore origin and range.
            var origin = MinimumValue + (NormalizedOrigin * ValueRange);
            var range  = NormalizedRange * ValueRange;


            // Clamp the range so that it stays within the limits.
            Clamp(ref origin, ref range);


            // Return result.
            return origin + (range * Mathf.Sin(T * (2 * Mathf.PI) / Duration));
        }

        #endregion

        #region Helper Methods

        /// <summary>
        /// Clamp origin and range based on <see cref="Direction"/>.
        /// </summary>
        /// <param name="origin">Origin to clamp.</param>
        /// <param name="range">Range to clamp.</param>
        private void Clamp(ref float origin, ref float range)
        {
            switch (Direction)
            {
                case CubismHarmonicMotionDirection.Left:
                {
                    if ((origin - range) >= MinimumValue)
                    {
                        range /= 2;
                        origin -= range;
                    }
                    else
                    {
                        range           = (origin - MinimumValue) / 2f;
                        origin          = MinimumValue + range;
                        NormalizedRange = (range * 2f)/ValueRange;
                    }


                    break;
                }
                case CubismHarmonicMotionDirection.Right:
                {
                    if ((origin + range) <= MaximumValue)
                    {
                        range  /= 2f;
                        origin += range;
                    }
                    else
                    {
                        range           = (MaximumValue - origin) / 2f;
                        origin          = MaximumValue - range;
                        NormalizedRange = (range * 2f)/ValueRange;
                    }


                    break;
                }
                default:
                {
                    break;
                }
            }


            // Clamp both range and NormalizedRange.
            if ((origin - range) < MinimumValue)
            {
                range           = origin - MinimumValue;
                NormalizedRange = range / ValueRange;
            }
            else if ((origin + range) > MaximumValue)
            {
                range           = MaximumValue - origin;
                NormalizedRange = range / ValueRange;
            }
        }

        #endregion
    }
}
