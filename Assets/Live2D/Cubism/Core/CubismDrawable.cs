/**
 * Copyright(c) Live2D Inc. All rights reserved.
 *
 * Use of this source code is governed by the Live2D Open Software license
 * that can be found at https://www.live2d.com/eula/live2d-open-software-license-agreement_en.html.
 */


using Live2D.Cubism.Core.Unmanaged;
using Live2D.Cubism.Framework;
using UnityEngine;


namespace Live2D.Cubism.Core
{
    /// <summary>
    /// Single <see cref="CubismModel"/> drawable.
    /// </summary>
    [CubismDontMoveOnReimport]
    public sealed class CubismDrawable : MonoBehaviour
    {
        #region Factory Methods

        /// <summary>
        /// Creates drawables for a <see cref="CubismModel"/>.
        /// </summary>
        /// <param name="unmanagedModel">Handle to unmanaged model.</param>
        /// <returns>Drawables root.</returns>
        internal static GameObject CreateDrawables(CubismUnmanagedModel unmanagedModel)
        {
            var root = new GameObject("Drawables");


            // Create drawables.
            var unmanagedDrawables = unmanagedModel.Drawables;
            var buffer = new CubismDrawable[unmanagedDrawables.Count];


            for (var i = 0; i < buffer.Length; ++i)
            {
                var proxy = new GameObject();


                buffer[i] = proxy.AddComponent<CubismDrawable>();


                buffer[i].transform.SetParent(root.transform);
                buffer[i].Reset(unmanagedModel, i);
            }


            return root;
        }

        #endregion


        /// <summary>
        /// Unmanaged drawables from unmanaged model.
        /// </summary>
        private CubismUnmanagedDrawables UnmanagedDrawables { get; set; }


        /// <summary>
        /// <see cref="UnmanagedIndex"/> backing field.
        /// </summary>
        [SerializeField, HideInInspector]
        private int _unmanagedIndex = -1;

        /// <summary>
        /// Position in unmanaged arrays.
        /// </summary>
        internal int UnmanagedIndex
        {
            get { return _unmanagedIndex; }
            private set { _unmanagedIndex = value; }
        }


        /// <summary>
        /// Copy of Id.
        /// </summary>
        public string Id
        {
            get
            {
                // Pull data.
                return UnmanagedDrawables.Ids[UnmanagedIndex];
            }
        }

        /// <summary>
        /// Texture UnmanagedIndex.
        /// </summary>
        public int TextureIndex
        {
            get
            {
                // Pull data.
                return UnmanagedDrawables.TextureIndices[UnmanagedIndex];
            }
        }

        /// <summary>
        /// Copy of MultiplyColor.
        /// </summary>
        public Color MultiplyColor
        {
            get
            {
                var index = UnmanagedIndex * 4;
                // Pull data.
                return new Color(UnmanagedDrawables.MultiplyColors[index],
                    UnmanagedDrawables.MultiplyColors[index + 1],
                    UnmanagedDrawables.MultiplyColors[index + 2],
                    UnmanagedDrawables.MultiplyColors[index + 3]);
            }
        }

        /// <summary>
        /// Copy of ScreenColor.
        /// </summary>
        public Color ScreenColor
        {
            get
            {
                var index = UnmanagedIndex * 4;
                // Pull data.
                return new Color(UnmanagedDrawables.ScreenColors[index],
                    UnmanagedDrawables.ScreenColors[index + 1],
                    UnmanagedDrawables.ScreenColors[index + 2],
                    UnmanagedDrawables.ScreenColors[index + 3]);
            }
        }

        /// <summary>
        /// Index of Parent Part.
        /// </summary>
        public int ParentPartIndex
        {
            get
            {
                // Pull data.
                return UnmanagedDrawables.ParentPartIndices[UnmanagedIndex];
            }
        }

        /// <summary>
        /// Copy of the masks.
        /// </summary>
        public CubismDrawable[] Masks
        {
            get
            {
                var drawables = this
                    .FindCubismModel(true)
                    .Drawables;


                // Get addresses.
                var counts = UnmanagedDrawables.MaskCounts;
                var indices = UnmanagedDrawables.Masks;


                // Pull data.
                var buffer = new CubismDrawable[counts[UnmanagedIndex]];


                for (var i = 0; i < buffer.Length; ++i)
                {
                    for (var j = 0; j < drawables.Length; ++j)
                    {
                        if (drawables[j].UnmanagedIndex != indices[UnmanagedIndex][i])
                        {
                            continue;
                        }


                        buffer[i] = drawables[j];


                        break;
                    }
                }


                return buffer;
            }
        }

        /// <summary>
        /// Copy of vertex positions.
        /// </summary>
        public Vector3[] VertexPositions
        {
            get
            {
                // Get addresses.
                var counts = UnmanagedDrawables.VertexCounts;
                var positions = UnmanagedDrawables.VertexPositions;


                // Pull data.
                var buffer = new Vector3[counts[UnmanagedIndex]];


                for (var i = 0; i < buffer.Length; ++i)
                {
                    buffer[i] = new Vector3(
                        positions[UnmanagedIndex][(i * 2) + 0],
                        positions[UnmanagedIndex][(i * 2) + 1]
                    );
                }


                return buffer;
            }
        }

        /// <summary>
        /// Copy of vertex texture coordinates.
        /// </summary>
        public Vector2[] VertexUvs
        {
            get
            {
                // Get addresses.
                var counts = UnmanagedDrawables.VertexCounts;
                var uvs = UnmanagedDrawables.VertexUvs;


                // Pull data.
                var buffer = new Vector2[counts[UnmanagedIndex]];


                for (var i = 0; i < buffer.Length; ++i)
                {
                    buffer[i] = new Vector2(
                        uvs[UnmanagedIndex][(i * 2) + 0],
                        uvs[UnmanagedIndex][(i * 2) + 1]
                    );
                }


                return buffer;
            }
        }

        /// <summary>
        /// Copy of triangle indices.
        /// </summary>
        public int[] Indices
        {
            get
            {
                // Get addresses.
                var counts = UnmanagedDrawables.IndexCounts;
                var indices = UnmanagedDrawables.Indices;


                // Pull data.
                var buffer = new int[counts[UnmanagedIndex]];


                for (var i = 0; i < buffer.Length; ++i)
                {
                    buffer[i] = indices[UnmanagedIndex][i];
                }


                return buffer;
            }
        }


        /// <summary>
        /// True if double-sided.
        /// </summary>
        public bool IsDoubleSided
        {
            get
            {
                // Get address.
                var flags = UnmanagedDrawables.ConstantFlags;


                // Pull data.
                return flags[UnmanagedIndex].HasIsDoubleSidedFlag();
            }
        }

        /// <summary>
        /// True if masking is requested.
        /// </summary>
        public bool IsMasked
        {
            get
            {
                // Get address.
                var counts = UnmanagedDrawables.MaskCounts;


                // Pull data.
                return counts[UnmanagedIndex] > 0;
            }
        }

        /// <summary>
        /// True if inverted mask.
        /// </summary>
        public bool IsInverted
        {
            get
            {
                // Get address.
                var flags = UnmanagedDrawables.ConstantFlags;


                // Pull data.
                return flags[UnmanagedIndex].HasIsInvertedMaskFlag();
            }
        }

        /// <summary>
        /// True if additive blending is requested.
        /// </summary>
        public bool BlendAdditive
        {
            get
            {
                // Get address.
                var flags = UnmanagedDrawables.ConstantFlags;


                // Pull data.
                return flags[UnmanagedIndex].HasBlendAdditiveFlag();
            }
        }

        /// <summary>
        /// True if multiply blending is setd.
        /// </summary>
        public bool MultiplyBlend
        {
            get
            {
                // Get address.
                var flags = UnmanagedDrawables.ConstantFlags;


                // Pull data.
                return flags[UnmanagedIndex].HasBlendMultiplicativeFlag();
            }
        }


        /// <summary>
        /// Revives instance.
        /// </summary>
        /// <param name="unmanagedModel">Handle to unmanaged model.</param>
        internal void Revive(CubismUnmanagedModel unmanagedModel)
        {
            UnmanagedDrawables = unmanagedModel.Drawables;
        }

        /// <summary>
        /// Restores instance to initial state.
        /// </summary>
        /// <param name="unmanagedModel">Handle to unmanaged model.</param>
        /// <param name="unmanagedIndex">Position in unmanaged arrays.</param>
        private void Reset(CubismUnmanagedModel unmanagedModel, int unmanagedIndex)
        {
            Revive(unmanagedModel);

            UnmanagedIndex = unmanagedIndex;
            name = Id;
        }
    }
}
