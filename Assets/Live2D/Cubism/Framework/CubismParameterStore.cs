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
    /// Cubism parameter store.
    /// </summary>
    public class CubismParameterStore : MonoBehaviour, ICubismUpdatable
    {
        /// <summary>
        /// Parameters cache.
        /// </summary>
        private CubismParameter[] DestinationParameters { get; set; }

        /// <summary>
        /// Parts cache.
        /// </summary>
        private CubismPart[] DestinationParts { get; set; }

        /// <summary>
        /// For storage parameters value.
        /// </summary>
        private float[] _parameterValues;

        /// <summary>
        /// For storage parts opacity.
        /// </summary>
        private float[] _partOpacities;

        /// <summary>
        /// Model has cubism update controller component.
        /// </summary>
        [HideInInspector]
        public bool HasUpdateController { get; set; }



        /// <summary>
        /// Called by cubism update controller. Order to invoke OnLateUpdate.
        /// </summary>
        public int ExecutionOrder
        {
            get { return CubismUpdateExecutionOrder.CubismParameterStoreSaveParameters; }
        }

        /// <summary>
        /// Called by cubism update controller. Needs to invoke OnLateUpdate on Editing.
        /// </summary>
        public bool NeedsUpdateOnEditing
        {
            get { return false; }
        }


        public void Refresh()
        {
            if (DestinationParameters == null)
            {
                DestinationParameters = this.FindCubismModel().Parameters;
            }

            if (DestinationParts == null)
            {
                DestinationParts = this.FindCubismModel().Parts;
            }

            // Get cubism update controller.
            HasUpdateController = (GetComponent<CubismUpdateController>() != null);

            SaveParameters();
        }

        /// <summary>
        /// Called by cubism update controller. Updates controller.
        /// </summary>
        /// <remarks>
        /// Make sure this method is called after any animations are evaluated.
        /// </remarks>
        public void OnLateUpdate()
        {
            if (!HasUpdateController)
            {
                return;
            }

            SaveParameters();
        }



        /// <summary>
        /// Save model parameters value and parts opacity.
        /// </summary>
        public void SaveParameters()
        {
            // Fail silently...
            if(!enabled)
            {
                return;
            }

            // save parameters value
            if(DestinationParameters != null && _parameterValues == null)
            {
                _parameterValues = new float[DestinationParameters.Length];
            }

            if(_parameterValues != null)
            {
                for(var i = 0; i < _parameterValues.Length; ++i)
                {
                    _parameterValues[i] = DestinationParameters[i].Value;
                }
            }

            // save parts opacity
            if(DestinationParts != null && _partOpacities == null)
            {
                _partOpacities = new float[DestinationParts.Length];
            }

            if(_partOpacities != null)
            {
                for(var i = 0; i < _partOpacities.Length; ++i)
                {
                    _partOpacities[i] = DestinationParts[i].Opacity;
                }
            }
        }

        /// <summary>
        /// Restore model parameters value and parts opacity.
        /// </summary>
        public void RestoreParameters()
        {
            // Fail silently...
            if(!enabled)
            {
                return;
            }

            // restore parameters value
            if(_parameterValues != null)
            {
                for(var i = 0; i < _parameterValues.Length; ++i)
                {
                    DestinationParameters[i].Value = _parameterValues[i];
                }
            }

            // restore parts opacity
            if(_partOpacities != null)
            {
                for(var i = 0; i < _partOpacities.Length; ++i)
                {
                    DestinationParts[i].Opacity = _partOpacities[i];
                }
            }
        }

        /// <summary>
        /// Called by Unity.
        /// </summary>
        private void OnEnable()
        {
            // Initialize cache.
            Refresh();
        }

    }
}
