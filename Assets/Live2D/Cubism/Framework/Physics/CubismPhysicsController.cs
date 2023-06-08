/**
 * Copyright(c) Live2D Inc. All rights reserved.
 *
 * Use of this source code is governed by the Live2D Open Software license
 * that can be found at https://www.live2d.com/eula/live2d-open-software-license-agreement_en.html.
 */


using System;
using Live2D.Cubism.Core;
using Live2D.Cubism.Rendering;
using Live2D.Cubism.Rendering.Masking;
using UnityEngine;


namespace Live2D.Cubism.Framework.Physics
{
    /// <summary>
    /// Physics simulation controller.
    /// </summary>
    [CubismMoveOnReimportCopyComponentsOnly]
    public class CubismPhysicsController : MonoBehaviour, ICubismUpdatable
    {
        /// <summary>
        /// Simulation target rig.
        /// </summary>
        private CubismPhysicsRig Rig
        {
            get { return _rig; }
            set { _rig = value; }
        }

        [SerializeField]
        private CubismPhysicsRig _rig;


        /// <summary>
        /// Cubism model parameters for simulation.
        /// </summary>
        public CubismParameter[] Parameters { get; private set; }


        /// <summary>
        /// Model has update controller component.
        /// </summary>
        [HideInInspector]
        public bool HasUpdateController { get; set; }


        public int ExecutionOrder
        {
            get { return CubismUpdateExecutionOrder.CubismPhysicsController; }
        }

        public bool NeedsUpdateOnEditing
        {
            get { return false; }
        }

        public void OnLateUpdate()
        {
            var deltaTime = Time.deltaTime;


            // Use fixed delta time if required.
            if (CubismPhysics.UseFixedDeltaTime)
            {
                deltaTime = Time.fixedDeltaTime;
            }


            // Evaluate rig.
            Rig.Evaluate(deltaTime);
        }

        /// <summary>
        /// Calculate until the physics is stable and update the model information.
        /// </summary>
        public void Stabilization()
        {
            Rig.Stabilization();

            var renderController = gameObject.GetComponent<CubismRenderController>();
            var maskController = gameObject.GetComponent<CubismMaskController>();

            renderController.OnLateUpdate();

            if (maskController)
            {
                maskController.OnLateUpdate();
            }
        }

        /// <summary>
        /// Sets rig and initializes <see langword="this"/>.
        /// </summary>
        /// <param name="rig"></param>
        public void Initialize(CubismPhysicsRig rig)
        {
            Rig = rig;
            Awake();
        }

        /// <summary>
        /// Set <see cref="CubismPhysicsOutput.AngleScale"/> to the ratio by the argument to the original value.
        /// </summary>
        /// <param name="subRig"></param>
        /// <param name="ratio">Ratio to original value</param>
        public void SetPhysicsSubRigOutputAngleScaleRatio(CubismPhysicsSubRig subRig, float ratio)
        {
            if (subRig == null)
            {
                return;
            }

            for (int i = 0; i < subRig.Output.Length; i++)
            {
                var original = subRig.OriginalOutput[i];

                subRig.Output[i].AngleScale = Math.Max(original.AngleScale * ratio, 0);
                subRig.Output[i].InitializeGetter();
            }
        }

        /// <summary>
        /// Set <see cref="CubismPhysicsOutput.IsInverted"/> whether or not to invert for the original bool value.
        /// </summary>
        /// <param name="subRig"></param>
        /// <param name="isInvert">Invert the original bool value</param>
        public void SetPhysicsSubRigOutputIsInverted(CubismPhysicsSubRig subRig, bool isInvert)
        {
            if (subRig == null)
            {
                return;
            }

            for (int i = 0; i < subRig.Output.Length; i++)
            {
                var original = subRig.OriginalOutput[i];

                subRig.Output[i].IsInverted = isInvert ? !original.IsInverted : original.IsInverted;
                subRig.Output[i].InitializeGetter();
            }
        }

        #region Unity Event Handling

        /// <summary>
        /// Called by Unity or <see cref="Initialize"/>. Initializes <see langword="this"/> if <see cref="Rig"/> exists.
        /// </summary>
        public void Awake()
        {
            // Check rig existence.
            if (Rig == null)
            {
                return;
            }


            // Initialize rig.
            Rig.Controller = this;


            for (var i = 0; i < Rig.SubRigs.Length; ++i)
            {
                Rig.SubRigs[i].Rig = Rig;
            }


            Parameters = this.FindCubismModel().Parameters;

            Rig.Initialize();
        }

        /// <summary>
        /// Called by Unity.
        /// </summary>
        public void Start()
        {
            // Get cubism update controller.
            HasUpdateController = (GetComponent<CubismUpdateController>() != null);
        }

        /// <summary>
        /// Called by Unity. Updates controller.
        /// </summary>
        /// <remarks>Must be call after animation update.</remarks>
        private void LateUpdate()
        {
            if (!HasUpdateController)
            {
                OnLateUpdate();
            }
        }

    #endregion
    }
}
