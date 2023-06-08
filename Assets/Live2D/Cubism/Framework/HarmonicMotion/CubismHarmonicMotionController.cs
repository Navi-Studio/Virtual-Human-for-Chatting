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
    /// Controller for <see cref="CubismHarmonicMotionParameter"/>s.
    /// </summary>
    public sealed class CubismHarmonicMotionController : MonoBehaviour, ICubismUpdatable
    {
        /// <summary>
        /// Default number of channels.
        /// </summary>
        private const int DefaultChannelCount = 1;


        /// <summary>
        /// Blend mode.
        /// </summary>
        [SerializeField]
        public CubismParameterBlendMode BlendMode = CubismParameterBlendMode.Additive;


        /// <summary>
        /// The timescales for each channel.
        /// </summary>
        [SerializeField]
        public float[] ChannelTimescales;


        /// <summary>
        /// Sources.
        /// </summary>
        private CubismHarmonicMotionParameter[] Sources { get; set; }

        /// <summary>
        /// Destinations.
        /// </summary>
        private CubismParameter[] Destinations { get; set; }

        /// <summary>
        /// Model has update controller component.
        /// </summary>
        [HideInInspector]
        public bool HasUpdateController { get; set; }


        /// <summary>
        /// Refreshes the controller. Call this method after adding and/or removing <see cref="CubismHarmonicMotionParameter"/>.
        /// </summary>
        public void Refresh()
        {
            var model = this.FindCubismModel();


            // Catch sources and destinations.
            Sources = model
                .Parameters
                .GetComponentsMany<CubismHarmonicMotionParameter>();
            Destinations = new CubismParameter[Sources.Length];


            for (var i = 0; i < Sources.Length; ++i)
            {
                Destinations[i] = Sources[i].GetComponent<CubismParameter>();
            }

            // Get cubism update controller.
            HasUpdateController = (GetComponent<CubismUpdateController>() != null);
        }

        /// <summary>
        /// Called by cubism update controller. Order to invoke OnLateUpdate.
        /// </summary>
        public int ExecutionOrder
        {
            get { return CubismUpdateExecutionOrder.CubismHarmonicMotionController; }
        }

        /// <summary>
        /// Called by cubism update controller. Needs to invoke OnLateUpdate on Editing.
        /// </summary>
        public bool NeedsUpdateOnEditing
        {
            get { return false; }
        }

        /// <summary>
        /// Called by cubism update controller. Updates controller.
        /// </summary>
        public void OnLateUpdate()
        {
            // Return if it is not valid or there's nothing to update.
            if (!enabled || Sources == null)
            {
                return;
            }


            // Update sources and destinations.
            for (var i = 0; i < Sources.Length; ++i)
            {
                Sources[i].Play(ChannelTimescales);


                Destinations[i].BlendToValue(BlendMode, Sources[i].Evaluate());
            }
        }

        #region Unity Events Handling

        /// <summary>
        /// Called by Unity. Makes sure cache is initialized.
        /// </summary>
        private void Start()
        {
            // Initialize cache.
            Refresh();
        }


        /// <summary>
        /// Called by Unity. Updates controller.
        /// </summary>
        private void LateUpdate()
        {
            if (!HasUpdateController)
            {
                OnLateUpdate();
            }
        }


        /// <summary>
        /// Called by Unity. Resets channels.
        /// </summary>
        private void Reset()
        {
            // Reset/Initialize channel timescales.
            ChannelTimescales = new float[DefaultChannelCount];


            for (var s = 0; s < DefaultChannelCount; ++s)
            {
                ChannelTimescales[s] = 1f;
            }
        }

        #endregion
    }
}
