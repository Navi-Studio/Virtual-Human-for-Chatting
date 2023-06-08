/**
 * Copyright(c) Live2D Inc. All rights reserved.
 *
 * Use of this source code is governed by the Live2D Open Software license
 * that can be found at https://www.live2d.com/eula/live2d-open-software-license-agreement_en.html.
 */


using Live2D.Cubism.Core;
using Live2D.Cubism.Rendering.Masking;
using System;
using UnityEngine;
using UnityEngine.Rendering;


namespace Live2D.Cubism.Rendering
{
    /// <summary>
    /// Wrapper for drawing <see cref="CubismDrawable"/>s.
    /// </summary>
    [ExecuteInEditMode, RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
    public sealed class CubismRenderer : MonoBehaviour
    {
        /// <summary>
        /// <see cref="LocalSortingOrder"/> backing field.
        /// </summary>
        [SerializeField, HideInInspector]
        private int _localSortingOrder;

        /// <summary>
        /// Local sorting order.
        /// </summary>
        public int LocalSortingOrder
        {
            get
            {
                return _localSortingOrder;
            }
            set
            {
                // Return early if same value given.
                if (value == _localSortingOrder)
                {
                    return;
                }


                // Store value.
                _localSortingOrder = value;


                // Apply it.
                ApplySorting();
            }
        }


        /// <summary>
        /// <see cref="Color"/> backing field.
        /// </summary>
        [SerializeField, HideInInspector]
        private Color _color = Color.white;

        /// <summary>
        /// Color.
        /// </summary>
        public Color Color
        {
            get { return _color; }
            set
            {
                // Return early if same value given.
                if (value == _color)
                {
                    return;
                }


                // Store value.
                _color = value;

                // Apply color.
                ApplyVertexColors();
            }
        }

        /// <summary>
        /// <see cref="OverwriteFlagForDrawableMultiplyColors"/> backing field.
        /// </summary>
        [SerializeField, HideInInspector]
        private bool _isOverwrittenDrawableMultiplyColors;

        /// <summary>
        /// Whether to overwrite with multiply color from the model.
        /// </summary>
        public bool OverwriteFlagForDrawableMultiplyColors
        {
            get { return _isOverwrittenDrawableMultiplyColors; }
            set { _isOverwrittenDrawableMultiplyColors = value; }
        }

        /// <summary>
        /// Last <see cref="OverwriteFlagForDrawableMultiplyColors"/>.
        /// </summary>
        public bool LastIsUseUserMultiplyColor { get; set; }

        /// <summary>
        /// <see cref="OverwriteFlagForDrawableScreenColors"/> backing field.
        /// </summary>
        [SerializeField, HideInInspector]
        private bool _isOverwrittenDrawableScreenColors;

        /// <summary>
        /// Whether to overwrite with screen color from the model.
        /// </summary>
        public bool OverwriteFlagForDrawableScreenColors
        {
            get { return _isOverwrittenDrawableScreenColors; }
            set { _isOverwrittenDrawableScreenColors = value; }
        }

        /// <summary>
        /// Last <see cref="OverwriteFlagForDrawableScreenColors"/>.
        /// </summary>
        public bool LastIsUseUserScreenColors { get; set; }

        /// <summary>
        /// <see cref="MultiplyColor"/> backing field.
        /// </summary>
        [SerializeField, HideInInspector]
        private Color _multiplyColor = Color.white;

        /// <summary>
        /// Drawable Multiply Color.
        /// </summary>
        public Color MultiplyColor
        {
            get
            {
                if (OverwriteFlagForDrawableMultiplyColors || RenderController.OverwriteFlagForModelMultiplyColors)
                {
                    return _multiplyColor;
                }

                return Drawable.MultiplyColor;
            }
            set
            {
                // Return early if same value given.
                if (value == _multiplyColor)
                {
                    return;
                }


                // Store value.
                _multiplyColor = (value != null)
                    ? value
                    : Color.white;
            }
        }

        /// <summary>
        /// Last Drawable Multiply Color.
        /// </summary>
        public Color LastMultiplyColor { get; set; }

        /// <summary>
        /// <see cref="ScreenColor"/> backing field.
        /// </summary>
        [SerializeField, HideInInspector]
        private Color _screenColor = Color.clear;

        /// <summary>
        /// Drawable Screen Color.
        /// </summary>
        public Color ScreenColor
        {
            get
            {
                if (OverwriteFlagForDrawableScreenColors || RenderController.OverwriteFlagForModelScreenColors)
                {
                    return _screenColor;
                }

                return Drawable.ScreenColor;
            }
            set
            {
                // Return early if same value given.
                if (value == _screenColor)
                {
                    return;
                }


                // Store value.
                _screenColor = (value != null)
                    ? value
                    : Color.black;
            }
        }

        /// <summary>
        /// Last Drawable Screen Color.
        /// </summary>
        public Color LastScreenColor { get; set; }


        /// <summary>
        /// <see cref="UnityEngine.Material"/>.
        /// </summary>
        public Material Material
        {
            get
            {
                #if UNITY_EDITOR
                if (!Application.isPlaying)
                {
                    return MeshRenderer.sharedMaterial;
                }
                #endif


                return MeshRenderer.material;
            }
            set
            {
                #if UNITY_EDITOR
                if (!Application.isPlaying)
                {
                    MeshRenderer.sharedMaterial = value;


                    return;
                }
                #endif


                MeshRenderer.material = value;
            }
        }


        /// <summary>
        /// <see cref="MainTexture"/> backing field.
        /// </summary>
        [SerializeField, HideInInspector]
        private Texture2D _mainTexture;

        /// <summary>
        /// <see cref="MeshRenderer"/>'s main texture.
        /// </summary>
        public Texture2D MainTexture
        {
            get { return _mainTexture; }
            set
            {
                // Return early if same value given and main texture is valid.
                if (value == _mainTexture && _mainTexture != null)
                {
                    return;
                }


                // Store value.
                _mainTexture = (value != null)
                    ? value
                    : Texture2D.whiteTexture;


                // Apply it.
                ApplyMainTexture();
            }
        }


        /// <summary>
        /// Meshes.
        /// </summary>
        /// <remarks>
        /// Double buffering dynamic meshes increases performance on mobile, so we double-buffer them here.
        /// </remarks>

        private Mesh[] Meshes { get; set; }

        /// <summary>
        /// Index of front buffer mesh.
        /// </summary>
        private int FrontMesh { get; set; }

        /// <summary>
        /// Index of back buffer mesh..
        /// </summary>
        private int BackMesh { get; set; }

        /// <summary>
        /// <see cref="UnityEngine.Mesh"/>.
        /// </summary>
        public Mesh Mesh
        {
            get { return Meshes[FrontMesh]; }
        }


        /// <summary>
        /// <see cref="MeshFilter"/> backing field.
        /// </summary>
        [NonSerialized]
        private MeshFilter _meshFilter;

        /// <summary>
        /// <see cref="UnityEngine.MeshFilter"/>.
        /// </summary>
        public MeshFilter MeshFilter
        {
            get
            {
                return _meshFilter;
            }
        }


        /// <summary>
        /// <see cref="MeshRenderer"/> backing field.
        /// </summary>
        [NonSerialized]
        private MeshRenderer _meshRenderer;

        /// <summary>
        /// <see cref="UnityEngine.MeshRenderer"/>.
        /// </summary>
        public MeshRenderer MeshRenderer
        {
            get
            {
                TryInitializeMeshRenderer();

                return _meshRenderer;
            }
        }

        /// <summary>
        /// <see cref="CubismDrawable"/>.
        /// </summary>
        private CubismDrawable Drawable { get; set; }

        /// <summary>
        /// <see cref="CubismRenderController"/>.
        /// </summary>
        private CubismRenderController RenderController { get; set; }


        #region Interface For CubismRenderController

        /// <summary>
        /// <see cref="SortingMode"/> backing field.
        /// </summary>
        [SerializeField, HideInInspector]
        private CubismSortingMode _sortingMode;

        /// <summary>
        /// Sorting mode.
        /// </summary>
        private CubismSortingMode SortingMode
        {
            get { return _sortingMode; }
            set { _sortingMode = value; }
        }


        /// <summary>
        /// <see cref="SortingOrder"/> backing field.
        /// </summary>
        [SerializeField, HideInInspector]
        private int _sortingOrder;

        /// <summary>
        /// Sorting mode.
        /// </summary>
        private int SortingOrder
        {
            get { return _sortingOrder; }
            set { _sortingOrder = value; }
        }


        /// <summary>
        /// <see cref="RenderOrder"/> backing field.
        /// </summary>
        [SerializeField, HideInInspector]
        private int _renderOrder;

        /// <summary>
        /// Sorting mode.
        /// </summary>
        private int RenderOrder
        {
            get { return _renderOrder; }
            set { _renderOrder = value; }
        }


        /// <summary>
        /// <see cref="DepthOffset"/> backing field.
        /// </summary>
        [SerializeField, HideInInspector]
        private float _depthOffset = 0.00001f;

        /// <summary>
        /// Offset to apply in case of depth sorting.
        /// </summary>
        private float DepthOffset
        {
            get { return _depthOffset; }
            set { _depthOffset = value; }
        }


        /// <summary>
        /// <see cref="Opacity"/> backing field.
        /// </summary>
        [SerializeField, HideInInspector]
        private float _opacity;

        /// <summary>
        /// Opacity.
        /// </summary>
        private float Opacity
        {
            get { return _opacity; }
            set { _opacity = value; }
        }


        /// <summary>
        /// Buffer for vertex colors.
        /// </summary>
        private Color[] VertexColors { get; set; }


        /// <summary>
        /// Allows tracking of what vertex data was updated last swap.
        /// </summary>
        private SwapInfo LastSwap { get; set; }

        /// <summary>
        /// Allows tracking of what vertex data will be swapped.
        /// </summary>
        private SwapInfo ThisSwap { get; set; }


        /// <summary>
        /// Swaps mesh buffers.
        /// </summary>
        /// <remarks>
        /// Make sure to manually call this method in case you changed the <see cref="Color"/>.
        /// </remarks>
        public void SwapMeshes()
        {
            // Perform internal swap.
            BackMesh = FrontMesh;
            FrontMesh = (FrontMesh == 0) ? 1 : 0;


            var mesh = Meshes[FrontMesh];


            // Update colors.
            Meshes[BackMesh].colors = VertexColors;


            // Update swap info.
            LastSwap = ThisSwap;


            ResetSwapInfoFlags();


            // Apply swap.
#if UNITY_EDITOR
            if (!Application.isPlaying)
            {
                MeshFilter.mesh = mesh;


                return;
            }
#endif


            MeshFilter.mesh = mesh;
        }


        /// <summary>
        /// Updates visibility.
        /// </summary>
        public void UpdateVisibility()
        {
            if (LastSwap.DidBecomeVisible)
            {
                MeshRenderer.enabled = true;
            }
            else if (LastSwap.DidBecomeInvisible)
            {
                MeshRenderer.enabled = false;
            }


            ResetVisibilityFlags();
        }


        /// <summary>
        /// Updates render order.
        /// </summary>
        public void UpdateRenderOrder()
        {
            if (LastSwap.NewRenderOrder)
            {
                ApplySorting();
            }


            ResetRenderOrderFlag();
        }

        /// <summary>
        /// Updates sorting layer.
        /// </summary>
        /// <param name="newSortingLayer">New sorting layer.</param>
        internal void OnControllerSortingLayerDidChange(int newSortingLayer)
        {
            MeshRenderer.sortingLayerID = newSortingLayer;
        }

        /// <summary>
        /// Updates sorting mode.
        /// </summary>
        /// <param name="newSortingMode">New sorting mode.</param>
        internal void OnControllerSortingModeDidChange(CubismSortingMode newSortingMode)
        {
            SortingMode = newSortingMode;


            ApplySorting();
        }

        /// <summary>
        /// Updates sorting order.
        /// </summary>
        /// <param name="newSortingOrder">New sorting order.</param>
        internal void OnControllerSortingOrderDidChange(int newSortingOrder)
        {
            SortingOrder = newSortingOrder;


            ApplySorting();
        }

        /// <summary>
        /// Updates depth offset.
        /// </summary>
        /// <param name="newDepthOffset"></param>
        internal void OnControllerDepthOffsetDidChange(float newDepthOffset)
        {
            DepthOffset = newDepthOffset;


            ApplySorting();
        }


        /// <summary>
        /// Sets the opacity.
        /// </summary>
        /// <param name="newOpacity">New opacity.</param>
        internal void OnDrawableOpacityDidChange(float newOpacity)
        {
            Opacity = newOpacity;


            ApplyVertexColors();
        }

        /// <summary>
        /// Updates render order.
        /// </summary>
        /// <param name="newRenderOrder">New render order.</param>
        internal void OnDrawableRenderOrderDidChange(int newRenderOrder)
        {
            RenderOrder = newRenderOrder;


            SetNewRenderOrder();
        }

        /// <summary>
        /// Sets the <see cref="UnityEngine.Mesh.vertices"/>.
        /// </summary>
        /// <param name="newVertexPositions">Vertex positions to set.</param>
        internal void OnDrawableVertexPositionsDidChange(Vector3[] newVertexPositions)
        {
            var mesh = Mesh;


            // Apply positions and update bounds.
            mesh.vertices = newVertexPositions;


            mesh.RecalculateBounds();


            // Set swap flag.
            SetNewVertexPositions();
        }

        /// <summary>
        /// Sets visibility.
        /// </summary>
        /// <param name="newVisibility">New visibility.</param>
        internal void OnDrawableVisiblityDidChange(bool newVisibility)
        {
            // Set swap flag if visible.
            if (newVisibility)
            {
                BecomeVisible();
            }
            else
            {
                BecomeInvisible();
            }
        }


        /// <summary>
        /// Sets mask properties.
        /// </summary>
        /// <param name="newMaskProperties">Value to set.</param>
        internal void OnMaskPropertiesDidChange(CubismMaskProperties newMaskProperties)
        {
            MeshRenderer.GetPropertyBlock(SharedPropertyBlock);


            // Write properties.
            SharedPropertyBlock.SetTexture(CubismShaderVariables.MaskTexture, newMaskProperties.Texture);
            SharedPropertyBlock.SetVector(CubismShaderVariables.MaskTile, newMaskProperties.Tile);
            SharedPropertyBlock.SetVector(CubismShaderVariables.MaskTransform, newMaskProperties.Transform);

            MeshRenderer.SetPropertyBlock(SharedPropertyBlock);
        }


        /// <summary>
        /// Sets model opacity.
        /// </summary>
        /// <param name="newModelOpacity">Opacity to set.</param>
        internal void OnModelOpacityDidChange(float newModelOpacity)
        {
            _meshRenderer.GetPropertyBlock(SharedPropertyBlock);


            // Write property.
            SharedPropertyBlock.SetFloat(CubismShaderVariables.ModelOpacity, newModelOpacity);

            MeshRenderer.SetPropertyBlock(SharedPropertyBlock);
        }

        #endregion

        /// <summary>
        /// <see cref="SharedPropertyBlock"/> backing field.
        /// </summary>
        private static MaterialPropertyBlock _sharedPropertyBlock;

        /// <summary>
        /// <see cref="MaterialPropertyBlock"/> that can be shared on the main script thread.
        /// </summary>
        private static MaterialPropertyBlock SharedPropertyBlock
        {
            get
            {
                // Lazily initialize.
                if (_sharedPropertyBlock == null)
                {
                    _sharedPropertyBlock = new MaterialPropertyBlock();
                }


                return _sharedPropertyBlock;
            }
        }


        /// <summary>
        /// Applies main texture for rendering.
        /// </summary>
        private void ApplyMainTexture()
        {
            MeshRenderer.GetPropertyBlock(SharedPropertyBlock);


            // Write property.
            SharedPropertyBlock.SetTexture(CubismShaderVariables.MainTexture, MainTexture);

            MeshRenderer.SetPropertyBlock(SharedPropertyBlock);
        }

        /// <summary>
        /// Applies sorting.
        /// </summary>
        private void ApplySorting()
        {
            // Sort by order.
            if (SortingMode.SortByOrder())
            {
                MeshRenderer.sortingOrder = SortingOrder + ((SortingMode == CubismSortingMode.BackToFrontOrder)
                    ? (RenderOrder + LocalSortingOrder)
                    : -(RenderOrder + LocalSortingOrder));


                transform.localPosition = Vector3.zero;


                return;
            }


            // Sort by depth.
            var offset = (SortingMode == CubismSortingMode.BackToFrontZ)
                    ? -DepthOffset
                    : DepthOffset;


            MeshRenderer.sortingOrder = SortingOrder + LocalSortingOrder;

            transform.localPosition = new Vector3(0f, 0f, RenderOrder * offset);
        }

        /// <summary>
        /// Uploads mesh vertex colors.
        /// </summary>
        public void ApplyVertexColors()
        {
            var vertexColors = VertexColors;
            var color = Color;


            color.a *= Opacity;


            for (var i = 0; i < vertexColors.Length; ++i)
            {
                vertexColors[i] = color;
            }


            // Set swap flag.
            SetNewVertexColors();
        }

        /// <summary>
        /// Uploads diffuse colors.
        /// </summary>
        public void ApplyMultiplyColor()
        {
            MeshRenderer.GetPropertyBlock(SharedPropertyBlock);


            // Write property.
            SharedPropertyBlock.SetColor(CubismShaderVariables.MultiplyColor, MultiplyColor);

            MeshRenderer.SetPropertyBlock(SharedPropertyBlock);
        }

        /// <summary>
        /// Initializes the main texture if possible.
        /// </summary>
        private void TryInitializeMultiplyColor()
        {
            LastIsUseUserMultiplyColor = false;

            LastMultiplyColor = MultiplyColor;

            ApplyMultiplyColor();
        }

        /// <summary>
        /// Uploads tint colors.
        /// </summary>
        public void ApplyScreenColor()
        {
            MeshRenderer.GetPropertyBlock(SharedPropertyBlock);


            // Write property.
            SharedPropertyBlock.SetColor(CubismShaderVariables.ScreenColor, ScreenColor);

            MeshRenderer.SetPropertyBlock(SharedPropertyBlock);
        }

        /// <summary>
        /// Initializes the main texture if possible.
        /// </summary>
        private void TryInitializeScreenColor()
        {
            LastIsUseUserScreenColors = false;

            LastScreenColor = ScreenColor;

            ApplyScreenColor();
        }

        /// <summary>
        /// Initializes the mesh renderer.
        /// </summary>
        private void TryInitializeMeshRenderer()
        {
            if (_meshRenderer == null)
            {
                _meshRenderer = GetComponent<MeshRenderer>();


                // Lazily add component.
                if (_meshRenderer == null)
                {
                    _meshRenderer = gameObject.AddComponent<MeshRenderer>();
                    _meshRenderer.hideFlags = HideFlags.HideInInspector;
                    _meshRenderer.receiveShadows = false;
                    _meshRenderer.shadowCastingMode = ShadowCastingMode.Off;
                    _meshRenderer.lightProbeUsage = LightProbeUsage.BlendProbes;
                }
            }
        }


        /// <summary>
        /// Initializes the mesh filter.
        /// </summary>
        private void TryInitializeMeshFilter()
        {
            if (_meshFilter == null)
            {
                _meshFilter = GetComponent<MeshFilter>();


                // Lazily add component.
                if (_meshFilter == null)
                {
                    _meshFilter = gameObject.AddComponent<MeshFilter>();
                    _meshFilter.hideFlags = HideFlags.HideInInspector;
                }
            }
        }


        /// <summary>
        /// Initializes the mesh if necessary.
        /// </summary>
        private void TryInitializeMesh()
        {
            // Only create mesh if necessary.
            // HACK 'Mesh.vertex > 0' makes sure mesh is recreated in case of runtime instantiation.
            if (Meshes != null && Mesh.vertexCount > 0)
            {
                return;
            }


            if (Meshes == null)
            {
                Meshes = new Mesh[2];
            }


            for (var i = 0; i < 2; ++i)
            {
                var mesh = new Mesh
                {
                    name = Drawable.name,
                    vertices = Drawable.VertexPositions,
                    uv = Drawable.VertexUvs,
                    triangles = Drawable.Indices
                };


                mesh.MarkDynamic();
                mesh.RecalculateBounds();


                // Store mesh.
                Meshes[i] = mesh;
            }
        }

        /// <summary>
        /// Initializes vertex colors.
        /// </summary>
        private void TryInitializeVertexColor()
        {
            var mesh = Mesh;


            VertexColors = new Color[mesh.vertexCount];


            for (var i = 0; i < VertexColors.Length; ++i)
            {
                VertexColors[i] = Color;
                VertexColors[i].a *= Opacity;
            }
        }

        /// <summary>
        /// Initializes the main texture if possible.
        /// </summary>
        private void TryInitializeMainTexture()
        {
            if (MainTexture == null)
            {
                MainTexture = null;
            }


            ApplyMainTexture();
        }


        /// <summary>
        /// Initializes components if possible.
        /// </summary>
        public void TryInitialize(CubismRenderController renderController)
        {
            Drawable = GetComponent<CubismDrawable>();
            RenderController = renderController;

            TryInitializeMeshRenderer();
            TryInitializeMeshFilter();

            TryInitializeMesh();
            TryInitializeVertexColor();
            TryInitializeMainTexture();
            TryInitializeMultiplyColor();
            TryInitializeScreenColor();

            ApplySorting();
        }

        #region Swap Info

        /// <summary>
        /// Sets <see cref="NewVertexPositions"/>.
        /// </summary>
        private void SetNewVertexPositions()
        {
            var swapInfo = ThisSwap;
            swapInfo.NewVertexPositions = true;
            ThisSwap = swapInfo;
        }


        /// <summary>
        /// Sets <see cref="NewVertexColors"/>.
        /// </summary>
        private void SetNewVertexColors()
        {
            var swapInfo = ThisSwap;
            swapInfo.NewVertexColors = true;
            ThisSwap = swapInfo;
        }


        /// <summary>
        /// Sets <see cref="DidBecomeVisible"/> on visible.
        /// </summary>
        private void BecomeVisible()
        {
            var swapInfo = ThisSwap;
            swapInfo.DidBecomeVisible = true;
            ThisSwap = swapInfo;
        }


        /// <summary>
        /// Sets <see cref="DidBecomeInvisible"/> on invisible.
        /// </summary>
        private void BecomeInvisible()
        {
            var swapInfo = ThisSwap;
            swapInfo.DidBecomeInvisible = true;
            ThisSwap = swapInfo;
        }


        /// <summary>
        /// Sets <see cref="SetNewRenderOrder"/>.
        /// </summary>
        private void SetNewRenderOrder()
        {
            var swapInfo = ThisSwap;
            swapInfo.NewRenderOrder = true;
            ThisSwap = swapInfo;
        }


        /// <summary>
        /// Resets flags.
        /// </summary>
        private void ResetSwapInfoFlags()
        {
            var swapInfo = ThisSwap;
            swapInfo.NewVertexColors = false;
            swapInfo.NewVertexPositions = false;
            swapInfo.DidBecomeVisible = false;
            swapInfo.DidBecomeInvisible = false;
            ThisSwap = swapInfo;
        }


        /// <summary>
        /// Reset visibility flags.
        /// </summary>
        private void ResetVisibilityFlags()
        {
            var swapInfo = LastSwap;
            swapInfo.DidBecomeVisible = false;
            swapInfo.DidBecomeInvisible = false;
            LastSwap = swapInfo;
        }


        /// <summary>
        /// Reset render order flag.
        /// </summary>
        private void ResetRenderOrderFlag()
        {
            var swapInfo = LastSwap;
            swapInfo.NewRenderOrder = false;
            LastSwap = swapInfo;
        }


        /// <summary>
        /// Allows tracking of <see cref="Mesh"/> data changed on a swap.
        /// </summary>
        private struct SwapInfo
        {
            /// <summary>
            /// Vertex positions were changed.
            /// </summary>
            public bool NewVertexPositions { get; set; }

            /// <summary>
            /// Vertex colors were changed.
            /// </summary>
            public bool NewVertexColors { get; set; }

            /// <summary>
            /// Visibility were changed to visible.
            /// </summary>
            public bool DidBecomeVisible { get; set; }

            /// <summary>
            /// Visibility were changed to invisible.
            /// </summary>
            public bool DidBecomeInvisible { get; set; }

            /// <summary>
            /// Render order were changed.
            /// </summary>
            public bool NewRenderOrder { get; set; }
        }

        #endregion



        #region Unity Events Handling

        /// <summary>
        /// Finalizes instance.
        /// </summary>
        private void OnDestroy()
        {
            if (Meshes == null)
            {
                return;
            }


            for (var i = 0; i < Meshes.Length; i++)
            {
                DestroyImmediate(Meshes[i]);
            }
        }

        #endregion
    }
}
