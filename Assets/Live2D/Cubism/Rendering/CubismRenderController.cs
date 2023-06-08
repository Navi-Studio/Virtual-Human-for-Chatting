/**
 * Copyright(c) Live2D Inc. All rights reserved.
 *
 * Use of this source code is governed by the Live2D Open Software license
 * that can be found at https://www.live2d.com/eula/live2d-open-software-license-agreement_en.html.
 */


using Live2D.Cubism.Core;
using Live2D.Cubism.Framework;
using System;
using UnityEngine;

using Object = UnityEngine.Object;


namespace Live2D.Cubism.Rendering
{
    /// <summary>
    /// Controls rendering of a <see cref="CubismModel"/>.
    /// </summary>
    [ExecuteInEditMode, CubismDontMoveOnReimport]
    public sealed class CubismRenderController : MonoBehaviour, ICubismUpdatable
    {
        /// <summary>
        /// Model opacity.
        /// </summary>
        /// <remarks>
        /// This is turned into a field to be available to <see cref="AnimationClip"/>s...
        /// </remarks>
        [SerializeField, HideInInspector]
        public float Opacity = 1f;

        /// <summary>
        /// <see cref="LastOpacity"/> backing field.
        /// </summary>
        [SerializeField, HideInInspector]
        private float _lastOpacity;

        /// <summary>
        /// Last model opacity.
        /// </summary>
        private float LastOpacity
        {
            get { return _lastOpacity; }
            set { _lastOpacity = value; }
        }

        /// <summary>
        /// <see cref="OverwriteFlagForModelMultiplyColors"/> backing field.
        /// </summary>
        [SerializeField, HideInInspector]
        private bool _isOverwrittenModelMultiplyColors;

        /// <summary>
        /// Whether to overwrite with multiply color from the model.
        /// </summary>
        public bool OverwriteFlagForModelMultiplyColors
        {
            get { return _isOverwrittenModelMultiplyColors; }
            set { _isOverwrittenModelMultiplyColors = value; }
        }

        /// <summary>
        /// <see cref="OverwriteFlagForModelScreenColors"/> backing field.
        /// </summary>
        [SerializeField, HideInInspector]
        private bool _isOverwrittenModelScreenColors;

        /// <summary>
        /// Whether to overwrite with screen color from the model.
        /// </summary>
        public bool OverwriteFlagForModelScreenColors
        {
            get { return _isOverwrittenModelScreenColors; }
            set { _isOverwrittenModelScreenColors = value; }
        }

        /// <summary>
        /// <see cref="ModelMultiplyColor"/> backing field.
        /// </summary>
        [SerializeField, HideInInspector]
        private Color _modelMultiplyColor;

        /// <summary>
        /// Multiply colors used throughout the model.
        /// </summary>
        public Color ModelMultiplyColor
        {
            get { return _modelMultiplyColor; }
            set { _modelMultiplyColor = value; }
        }

        /// <summary>
        /// <see cref="ModelScreenColor"/> backing field.
        /// </summary>
        [SerializeField, HideInInspector]
        private Color _modelScreenColor;

        /// <summary>
        /// Screen colors used throughout the model.
        /// </summary>
        public Color ModelScreenColor
        {
            get { return _modelScreenColor; }
            set { _modelScreenColor = value; }
        }

        /// <summary>
        /// Sorting layer name.
        /// </summary>
        public string SortingLayer
        {
            get
            {
                return UnityEngine.SortingLayer.IDToName(SortingLayerId);
            }
            set
            {
                SortingLayerId = UnityEngine.SortingLayer.NameToID(value);
            }
        }

        /// <summary>
        /// <see cref="SortingLayerId"/> backing field.
        /// </summary>
        [SerializeField, HideInInspector]
        private int _sortingLayerId;

        /// <summary>
        /// Sorting layer Id.
        /// </summary>
        public int SortingLayerId
        {
            get
            {
                return _sortingLayerId;
            }
            set
            {
                if (value == _sortingLayerId)
                {
                    return;
                }


                _sortingLayerId = value;


                // Apply sorting layer.
                var renderers = Renderers;


                for (var i = 0; i < renderers.Length; ++i)
                {
                    renderers[i].OnControllerSortingLayerDidChange(_sortingLayerId);
                }
            }
        }


        /// <summary>
        /// <see cref="SortingMode"/> backing field.
        /// </summary>
        [SerializeField, HideInInspector]
        private CubismSortingMode _sortingMode;

        /// <summary>
        /// <see cref="CubismDrawable"/> sorting.
        /// </summary>
        public CubismSortingMode SortingMode
        {
            get
            {
                return _sortingMode;
            }
            set
            {
                // Return early if same value given.
                if (value == _sortingMode)
                {
                    return;
                }


                _sortingMode = value;


                // Flip sorting.
                var renderers = Renderers;


                for (var i = 0; i < renderers.Length; ++i)
                {
                    renderers[i].OnControllerSortingModeDidChange(_sortingMode);
                }
            }
        }


        /// <summary>
        /// Order in sorting layer.
        /// </summary>
        [SerializeField, HideInInspector]
        private int _sortingOrder;

        /// <summary>
        /// Order in sorting layer.
        /// </summary>
        public int SortingOrder
        {
            get
            {
                return _sortingOrder;
            }
            set
            {
                // Return early in case same value given.
                if (value == _sortingOrder)
                {
                    return;
                }


                _sortingOrder = value;


                // Apply new sorting order.
                var renderers = Renderers;


                for (var i = 0; i < renderers.Length; ++i)
                {
                    renderers[i].OnControllerSortingOrderDidChange(SortingOrder);
                }
            }
        }


        /// <summary>
        /// [Optional] Camera to face.
        /// </summary>
        [SerializeField]
        public Camera CameraToFace;



        /// <summary>
        /// <see cref="DrawOrderHandler"/> backing field.
        /// </summary>
        [SerializeField, HideInInspector]
        private Object _drawOrderHandler;

        /// <summary>
        /// Draw order handler proxy object.
        /// </summary>
        public Object DrawOrderHandler
        {
            get { return _drawOrderHandler; }
            set { _drawOrderHandler = value.ToNullUnlessImplementsInterface<ICubismDrawOrderHandler>(); }
        }


        /// <summary>
        /// <see cref="DrawOrderHandlerInterface"/> backing field.
        /// </summary>
        [NonSerialized]
        private ICubismDrawOrderHandler _drawOrderHandlerInterface;

        /// <summary>
        /// Listener for draw order changes.
        /// </summary>
        private ICubismDrawOrderHandler DrawOrderHandlerInterface
        {
            get
            {
                if (_drawOrderHandlerInterface == null)
                {
                    _drawOrderHandlerInterface = DrawOrderHandler.GetInterface<ICubismDrawOrderHandler>();
                }


                return _drawOrderHandlerInterface;
            }
        }


        /// <summary>
        /// <see cref="OpacityHandler"/> backing field.
        /// </summary>
        [SerializeField, HideInInspector]
        private Object _opacityHandler;

        /// <summary>
        /// Opacity handler proxy object.
        /// </summary>
        public Object OpacityHandler
        {
            get { return _opacityHandler; }
            set { _opacityHandler = value.ToNullUnlessImplementsInterface<ICubismOpacityHandler>(); }
        }


        /// <summary>
        /// <see cref="OpacityHandler"/> backing field.
        /// </summary>
        private ICubismOpacityHandler _opacityHandlerInterface;

        /// <summary>
        /// Listener for opacity changes.
        /// </summary>
        private ICubismOpacityHandler OpacityHandlerInterface
        {
            get
            {
                if (_opacityHandlerInterface == null)
                {
                    _opacityHandlerInterface = OpacityHandler.GetInterface<ICubismOpacityHandler>();
                }


                return _opacityHandlerInterface;
            }
        }


        /// <summary>
        /// <see cref="MultiplyColorHandler"/> backing field.
        /// </summary>
        [SerializeField, HideInInspector]
        private Object _multiplyColorHandler;

        /// <summary>
        /// Opacity handler proxy object.
        /// </summary>
        public Object MultiplyColorHandler
        {
            get { return _multiplyColorHandler; }
            set { _multiplyColorHandler = value.ToNullUnlessImplementsInterface<ICubismBlendColorHandler>(); }
        }


        /// <summary>
        /// <see cref="MultiplyColorHandler"/> backing field.
        /// </summary>
        private ICubismBlendColorHandler _multiplyColorHandlerInterface;

        /// <summary>
        /// Listener for blend color changes.
        /// </summary>
        private ICubismBlendColorHandler MultiplyColorHandlerInterface
        {
            get
            {
                if (_multiplyColorHandlerInterface == null)
                {
                    _multiplyColorHandlerInterface = MultiplyColorHandler?.GetInterface<ICubismBlendColorHandler>();
                }


                return _multiplyColorHandlerInterface;
            }
        }

        /// <summary>
        /// <see cref="ScreenColorHandler"/> backing field.
        /// </summary>
        [SerializeField, HideInInspector]
        private Object _screenColorHandler;

        /// <summary>
        /// Opacity handler proxy object.
        /// </summary>
        public Object ScreenColorHandler
        {
            get { return _screenColorHandler; }
            set { _screenColorHandler = value.ToNullUnlessImplementsInterface<ICubismBlendColorHandler>(); }
        }


        /// <summary>
        /// <see cref="MultiplyColorHandler"/> backing field.
        /// </summary>
        private ICubismBlendColorHandler _screenColorHandlerInterface;

        /// <summary>
        /// Listener for blend color changes.
        /// </summary>
        private ICubismBlendColorHandler ScreenColorHandlerInterface
        {
            get
            {
                if (_screenColorHandlerInterface == null)
                {
                    _screenColorHandlerInterface = ScreenColorHandler?.GetInterface<ICubismBlendColorHandler>();
                }


                return _screenColorHandlerInterface;
            }
        }

        /// <summary>
        /// The value to offset the <see cref="CubismDrawable"/>s by.
        /// </summary>
        /// <remarks>
        /// You only need to adjust this value when using perspective cameras.
        /// </remarks>
        [SerializeField, HideInInspector]
        public float _depthOffset = 0.00001f;

        /// <summary>
        /// Depth offset used when sorting by depth.
        /// </summary>
        public float DepthOffset
        {
            get { return _depthOffset; }
            set
            {
                // Return if same value given.
                if (Mathf.Abs(value - _depthOffset) < Mathf.Epsilon)
                {
                    return;
                }


                // Store value.
                _depthOffset = value;


                // Apply it.
                var renderers = Renderers;


                for (var i = 0; i < renderers.Length; ++i)
                {
                    renderers[i].OnControllerDepthOffsetDidChange(_depthOffset);
                }
            }
        }


        /// <summary>
        /// Model the controller belongs to.
        /// </summary>
        private CubismModel Model
        {
            get { return this.FindCubismModel(); }
        }


        /// <summary>
        /// <see cref="DrawablesRootTransform"/> backing field.
        /// </summary>
        private Transform _drawablesRootTransform;

        /// <summary>
        /// Root transform of all <see cref="CubismDrawable"/>s of the model.
        /// </summary>
        private Transform DrawablesRootTransform
        {
            get
            {
                if (_drawablesRootTransform == null)
                {
                    _drawablesRootTransform = Model.Drawables[0].transform.parent;
                }


                return _drawablesRootTransform;
            }
        }


        /// <summary>
        /// <see cref="Renderers"/>s backing field.
        /// </summary>
        [NonSerialized]
        private CubismRenderer[] _renderers;

        /// <summary>
        /// <see cref="CubismRenderer"/>s.
        /// </summary>
        public CubismRenderer[] Renderers
        {
            get
            {
                if (_renderers== null)
                {
                    _renderers = Model.Drawables.GetComponentsMany<CubismRenderer>();
                }


                return _renderers;
            }
            private set { _renderers = value; }
        }


        /// <summary>
        /// Model has update controller component.
        /// </summary>
        [HideInInspector]
        public bool HasUpdateController { get; set; }


        /// <summary>
        /// Makes sure all <see cref="CubismDrawable"/>s have <see cref="CubismRenderer"/>s attached to them.
        /// </summary>
        private void TryInitializeRenderers()
        {
            // Try get renderers.
            var renderers = Renderers;

            // Create renderers if necesssary.
            if (renderers == null || renderers.Length == 0)
            {
                // Create renders and apply it to backing field...
                var drawables = this
                .FindCubismModel()
                .Drawables;


                renderers = drawables.AddComponentEach<CubismRenderer>();

                // Store renderers.
                Renderers = renderers;

            }

            if (renderers == null)
            {
                return;
            }

            // Make sure renderers are initialized.
            for (var i = 0; i < renderers.Length; ++i)
            {
                renderers[i].TryInitialize(this);
            }


            // Initialize sorting layer.
            // We set the backing field here directly because we pull the sorting layer directly from the renderer.
            _sortingLayerId = renderers[0]
                .MeshRenderer
                .sortingLayerID;
        }


        /// <summary>
        /// Updates opacity if necessary.
        /// </summary>
        private void UpdateOpacity()
        {
            // Return if same value given.
            if (Mathf.Abs(Opacity - LastOpacity) < Mathf.Epsilon)
            {
                return;
            }


            // Store value.
            Opacity = Mathf.Clamp(Opacity, 0f, 1f);
            LastOpacity = Opacity;


            // Apply opacity.
            var applyOpacityToRenderers = (OpacityHandlerInterface == null || Opacity > (1f - Mathf.Epsilon));


            if (applyOpacityToRenderers && Renderers != null)
            {
                var renderers = Renderers;


                for (var i = 0; i < renderers.Length; ++i)
                {
                    renderers[i].OnModelOpacityDidChange(Opacity);
                }
            }


            // Call handler.
            if (OpacityHandlerInterface != null)
            {
                OpacityHandlerInterface.OnOpacityDidChange(this, Opacity);
            }
        }

        /// <summary>
        /// Updates Blend Colors if necessary.
        /// </summary>
        private void UpdateBlendColors()
        {
            if (Renderers == null)
            {
                return;
            }

            var isMultiplyColorUpdated = false;
            var isScreenColorUpdated = false;
            var newMultiplyColors = new Color[Renderers.Length];
            var newScreenColors = new Color[Renderers.Length];

            for (int i = 0; i < Renderers.Length; i++)
            {
                var isUseUserMultiplyColor = (Renderers[i].OverwriteFlagForDrawableMultiplyColors ||
                                        OverwriteFlagForModelMultiplyColors);

                if (isUseUserMultiplyColor)
                {
                    // If you switch from a setting that uses the color of the model, revert to the color that was retained.
                    if (!Renderers[i].LastIsUseUserMultiplyColor)
                    {
                        Renderers[i].MultiplyColor = Renderers[i].LastMultiplyColor;
                        Renderers[i].ApplyMultiplyColor();
                        isMultiplyColorUpdated = true;
                    }
                    else if (Renderers[i].LastMultiplyColor != Renderers[i].MultiplyColor)
                    {
                        Renderers[i].ApplyMultiplyColor();
                        isMultiplyColorUpdated = true;
                    }

                    Renderers[i].LastMultiplyColor = Renderers[i].MultiplyColor;
                }
                else if (Renderers[i].LastIsUseUserMultiplyColor)
                {
                    Renderers[i].MultiplyColor = Renderers[i].LastMultiplyColor;
                    Renderers[i].ApplyMultiplyColor();
                    isMultiplyColorUpdated = true;
                }

                newMultiplyColors[i] = Renderers[i].MultiplyColor;
                Renderers[i].LastIsUseUserMultiplyColor = isUseUserMultiplyColor;

                var isUseUserScreenColor = (Renderers[i].OverwriteFlagForDrawableScreenColors ||
                                             OverwriteFlagForModelScreenColors);

                if (isUseUserScreenColor)
                {
                    // If you switch from a setting that uses the color of the model, revert to the color that was retained.
                    if (!Renderers[i].LastIsUseUserScreenColors)
                    {
                        Renderers[i].ScreenColor = Renderers[i].LastScreenColor;
                        Renderers[i].ApplyScreenColor();
                        isScreenColorUpdated = true;
                    }
                    else if (Renderers[i].LastScreenColor != Renderers[i].ScreenColor)
                    {
                        Renderers[i].ApplyScreenColor();
                        isScreenColorUpdated = true;
                    }

                    Renderers[i].LastScreenColor = Renderers[i].ScreenColor;
                }
                else if (Renderers[i].LastIsUseUserScreenColors)
                {
                    Renderers[i].ScreenColor = Renderers[i].LastScreenColor;
                    Renderers[i].ApplyScreenColor();
                    isScreenColorUpdated = true;
                }

                newScreenColors[i] = Renderers[i].ScreenColor;
                Renderers[i].LastIsUseUserScreenColors = isUseUserScreenColor;
            }

            if (MultiplyColorHandler != null && isMultiplyColorUpdated)
            {
                MultiplyColorHandlerInterface.OnBlendColorDidChange(this, newMultiplyColors);
            }

            if (ScreenColorHandler != null && isScreenColorUpdated)
            {
                ScreenColorHandlerInterface.OnBlendColorDidChange(this, newScreenColors);
            }
        }

        /// <summary>
        /// Called by cubism update controller. Order to invoke OnLateUpdate.
        /// </summary>
        public int ExecutionOrder
        {
            get { return CubismUpdateExecutionOrder.CubismRenderController; }
        }

        /// <summary>
        /// Called by cubism update controller. Needs to invoke OnLateUpdate on Editing.
        /// </summary>
        public bool NeedsUpdateOnEditing
        {
            get { return true; }
        }

        /// <summary>
        /// Called by cubism update controller. Applies billboarding.
        /// </summary>
        public void OnLateUpdate()
        {
            // Fail silently...
            if(!enabled)
            {
                return;
            }

            // Update opacity if necessary.
            UpdateOpacity();

            // Updates Blend Colors if necessary.
            UpdateBlendColors();

            // Return early in case no camera is to be faced.
            if (CameraToFace == null)
            {
                return;
            }


            // Face camera.
            DrawablesRootTransform.rotation = (Quaternion.LookRotation(CameraToFace.transform.forward, Vector3.up));
        }

        #region Unity Event Handling

        /// <summary>
        /// Called by Unity.
        /// </summary>
        private void Start()
        {
            // Get cubism update controller.
            HasUpdateController = (GetComponent<CubismUpdateController>() != null);
        }

        /// <summary>
        /// Called by Unity. Enables listening to render data updates.
        /// </summary>
        private void OnEnable()
        {
            // Fail silently.
            if (Model == null)
            {
                return;
            }


            // Make sure renderers are available.
            TryInitializeRenderers();


            // Register listener.
            Model.OnDynamicDrawableData += OnDynamicDrawableData;
        }

        /// <summary>
        /// Called by Unity. Disables listening to render data updates.
        /// </summary>
        private void OnDisable()
        {
            // Fail silently.
            if (Model == null)
            {
                return;
            }


            // Deregister listener.
            Model.OnDynamicDrawableData -= OnDynamicDrawableData;
        }

        #endregion

        #region Cubism Event Handling

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

        /// <summary>
        /// Called whenever new render data is available.
        /// </summary>
        /// <param name="sender">Model with new render data.</param>
        /// <param name="data">New render data.</param>
        private void OnDynamicDrawableData(CubismModel sender, CubismDynamicDrawableData[] data)
        {
            // Get drawables.
            var drawables = sender.Drawables;
            var renderers = Renderers;


            // Handle render data changes.
            for (var i = 0; i < data.Length; ++i)
            {
                // Controls whether mesh buffers are to be swapped.
                var swapMeshes = false;


                // Update visibility if last SwapInfo flag is true.
                renderers[i].UpdateVisibility();


                // Update render order if last SwapInfo flags is true.
                renderers[i].UpdateRenderOrder();


                // Skip completely non-dirty data.
                if (!data[i].IsAnyDirty)
                {
                    continue;
                }


                // Update visibility.
                if (data[i].IsVisibilityDirty)
                {
                    renderers[i].OnDrawableVisiblityDidChange(data[i].IsVisible);

                    swapMeshes = true;
                }


                // Update render order.
                if (data[i].IsRenderOrderDirty)
                {
                    renderers[i].OnDrawableRenderOrderDidChange(data[i].RenderOrder);


                    swapMeshes = true;
                }


                // Update opacity.
                if (data[i].IsOpacityDirty)
                {
                    renderers[i].OnDrawableOpacityDidChange(data[i].Opacity);


                    swapMeshes = true;
                }


                // Update vertex positions.
                if (data[i].AreVertexPositionsDirty)
                {
                    renderers[i].OnDrawableVertexPositionsDidChange(data[i].VertexPositions);


                    swapMeshes = true;
                }


                // Swap buffers if necessary.
                // [INV] Swapping only half of the meshes might improve performance even. Would that be visually feasible?
                if (swapMeshes)
                {
                    renderers[i].SwapMeshes();
                }
            }


            // Pass draw order changes to handler (if available).
            var drawOrderHandler = DrawOrderHandlerInterface;


            if (drawOrderHandler != null)
            {
                for (var i = 0; i < data.Length; ++i)
                {
                    if (data[i].IsDrawOrderDirty)
                    {
                        drawOrderHandler.OnDrawOrderDidChange(this, drawables[i], data[i].DrawOrder);
                    }
                }
            }

            var isMultiplyColorUpdated = false;
            var isScreenColorUpdated = false;
            var newMultiplyColors = new Color[Renderers.Length];
            var newScreenColors = new Color[Renderers.Length];

            for (var i = 0; i < data.Length; ++i)
            {
                var isUseModelMultiplyColor = !(renderers[i].OverwriteFlagForDrawableMultiplyColors ||
                                                OverwriteFlagForModelMultiplyColors);

                // Skip processing when not using model colors.
                if (data[i].IsBlendColorDirty && isUseModelMultiplyColor)
                {
                    renderers[i].ApplyMultiplyColor();
                    isMultiplyColorUpdated = true;
                }

                newMultiplyColors[i] = renderers[i].MultiplyColor;
            }

            for (var i = 0; i < data.Length; ++i)
            {
                var isUseModelScreenColor = !(renderers[i].OverwriteFlagForDrawableScreenColors ||
                                              OverwriteFlagForModelScreenColors);

                // Skip processing when not using model colors.
                if (data[i].IsBlendColorDirty && isUseModelScreenColor)
                {
                    renderers[i].ApplyScreenColor();
                    isScreenColorUpdated = true;
                }

                newScreenColors[i] = renderers[i].ScreenColor;
            }

            // Pass blend color changes to handler (if available).
            var multiplyColorHandlerInterface = MultiplyColorHandlerInterface;
            var screenColorHandlerInterface = ScreenColorHandlerInterface;

            if (MultiplyColorHandler != null && isMultiplyColorUpdated)
            {
                multiplyColorHandlerInterface.OnBlendColorDidChange(this, newMultiplyColors);
            }

            if (ScreenColorHandler != null && isScreenColorUpdated)
            {
                screenColorHandlerInterface.OnBlendColorDidChange(this, newScreenColors);
            }
        }

        #endregion
    }
}
