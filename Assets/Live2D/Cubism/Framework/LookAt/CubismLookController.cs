/**
 * Copyright(c) Live2D Inc. All rights reserved.
 *
 * Use of this source code is governed by the Live2D Open Software license
 * that can be found at https://www.live2d.com/eula/live2d-open-software-license-agreement_en.html.
 */


using Live2D.Cubism.Core;
using UnityEngine;

using Object = UnityEngine.Object;


namespace Live2D.Cubism.Framework.LookAt
{
    /// <summary>
    /// Controls <see cref="CubismLookParameter"/>s.
    /// </summary>
    public sealed class CubismLookController : MonoBehaviour, ICubismUpdatable
    {
        /// <summary>
        /// Blend mode.
        /// </summary>
        [SerializeField]
        public CubismParameterBlendMode BlendMode = CubismParameterBlendMode.Additive;


        /// <summary>
        /// <see cref="Target"/> backing field.
        /// </summary>
        [SerializeField, HideInInspector] private Object _target;

        /// <summary>
        /// Target.
        /// </summary>
        public Object Target
        {
            get { return _target; }
            set { _target = value.ToNullUnlessImplementsInterface<ICubismLookTarget>(); }
        }


        /// <summary>
        /// <see cref="TargetInterface"/> backing field.
        /// </summary>
        private ICubismLookTarget _targetInterface;

        /// <summary>
        /// Interface of target.
        /// </summary>
        private ICubismLookTarget TargetInterface
        {
            get
            {
                if (_targetInterface == null)
                {
                    _targetInterface = Target.GetInterface<ICubismLookTarget>();
                }


                return _targetInterface;
            }
        }


        /// <summary>
        /// Local center position.
        /// </summary>
        public Transform Center;

        /// <summary>
        /// Damping to apply.
        /// </summary>
        public float Damping = 0.15f;


        /// <summary>
        /// Source parameters.
        /// </summary>
        private CubismLookParameter[] Sources { get; set; }

        /// <summary>
        /// The actual parameters to apply the source values to.
        /// </summary>
        private CubismParameter[] Destinations { get; set; }


        /// <summary>
        /// Position at last frame.
        /// </summary>
        private Vector3 LastPosition { get; set; }

        /// <summary>
        /// Goal position.
        /// </summary>
        private Vector3 GoalPosition { get; set; }

        /// <summary>
        /// Buffer for <see cref="Mathf.SmoothDamp(float, float, ref float, float)"/> velocity.
        /// </summary>
        // ReSharper disable once InconsistentNaming
        private Vector3 VelocityBuffer;

        /// <summary>
        /// Model has update controller component.
        /// </summary>
        [HideInInspector]
        public bool HasUpdateController { get; set; }


        /// <summary>
        /// Refreshes the controller. Call this method after adding and/or removing <see cref="CubismLookParameter"/>s.
        /// </summary>
        public void Refresh()
        {
            var model = this.FindCubismModel();


            // Catch sources and destinations.
            Sources = model
                .Parameters
                .GetComponentsMany<CubismLookParameter>();
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
            get { return CubismUpdateExecutionOrder.CubismLookController; }
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
            if (!enabled || Destinations == null)
            {
                return;
            }


            // Return early if no target is available or if target is inactive.
            var target = TargetInterface;


            if (target == null || !target.IsActive())
            {
                return;
            }


            // Update position.
            var position = LastPosition;
            GoalPosition = transform.InverseTransformPoint(target.GetPosition()) - Center.localPosition;


            if (position != GoalPosition)
            {
                position = Vector3.SmoothDamp(
                    position,
                    GoalPosition,
                    ref VelocityBuffer,
                    Damping);
            }


            // Update sources and destinations.
            for (var i = 0; i < Destinations.Length; ++i)
            {
                Destinations[i].BlendToValue(BlendMode, Sources[i].TickAndEvaluate(position));
            }


            // Store position.
            LastPosition = position;
        }

        #region Unity Events Handling

        /// <summary>
        /// Called by Unity. Makes sure cache is initialized.
        /// </summary>
        private void Start()
        {
            // Default center if necessary.
            if (Center == null)
            {
                Center = transform;
            }


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

        #endregion
    }
}
