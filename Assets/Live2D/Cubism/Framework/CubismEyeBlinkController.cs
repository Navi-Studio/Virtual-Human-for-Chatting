/**
 * Copyright(c) Live2D Inc. All rights reserved.
 *
 * Use of this source code is governed by the Live2D Open Software license
 * that can be found at https://www.live2d.com/eula/live2d-open-software-license-agreement_en.html.
 */


using Live2D.Cubism.Core;
using UnityEngine;


namespace Live2D.Cubism.Framework
{
    /// <summary>
    /// <see cref="CubismEyeBlinkParameter"/> controller.
    /// </summary>
    public sealed class CubismEyeBlinkController : MonoBehaviour, ICubismUpdatable
    {
        /// <summary>
        /// Blend mode.
        /// </summary>
        [SerializeField]
        public CubismParameterBlendMode BlendMode = CubismParameterBlendMode.Multiply;


        /// <summary>
        /// Opening of the eyes.
        /// </summary>
        [SerializeField, Range(0f, 1f)]
        public float EyeOpening = 1f;


        /// <summary>
        /// Eye blink parameters cache.
        /// </summary>
        private CubismParameter[] Destinations { get; set; }


        /// <summary>
        /// Model has update controller component.
        /// </summary>
        [HideInInspector]
        public bool HasUpdateController { get; set; }

        /// <summary>
        /// Refreshes controller. Call this method after adding and/or removing <see cref="CubismEyeBlinkParameter"/>s.
        /// </summary>
        public void Refresh()
        {
            var model = this.FindCubismModel();


            // Fail silently...
            if (model == null)
            {
                return;
            }


            // Cache destinations.
            var tags = model
                .Parameters
                .GetComponentsMany<CubismEyeBlinkParameter>();


            Destinations = new CubismParameter[tags.Length];


            for (var i = 0; i < tags.Length; ++i)
            {
                Destinations[i] = tags[i].GetComponent<CubismParameter>();
            }

            // Get cubism update controller.
            HasUpdateController = (GetComponent<CubismUpdateController>() != null);
        }

        /// <summary>
        /// Called by cubism update controller. Order to invoke OnLateUpdate.
        /// </summary>
        public int ExecutionOrder
        {
            get { return CubismUpdateExecutionOrder.CubismEyeBlinkController; }
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
            // Fail silently.
            if (!enabled || Destinations == null)
            {
                return;
            }


            // Apply value.
            Destinations.BlendToValue(BlendMode, EyeOpening);
        }

        #region Unity Event Handling


        /// <summary>
        /// Called by Unity. Makes sure cache is initialized.
        /// </summary>
        private void Start()
        {
            // Initialize cache.
            Refresh();
        }

        /// <summary>
        /// Called by Unity.
        /// </summary>
        private void LateUpdate()
        {
            if(!HasUpdateController)
            {
                OnLateUpdate();
            }
        }

        #endregion
    }
}
