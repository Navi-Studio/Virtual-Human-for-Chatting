/**
 * Copyright(c) Live2D Inc. All rights reserved.
 *
 * Use of this source code is governed by the Live2D Open Software license
 * that can be found at https://www.live2d.com/eula/live2d-open-software-license-agreement_en.html.
 */


using Live2D.Cubism.Core;
using Live2D.Cubism.Framework;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;


namespace Live2D.Cubism.Rendering.Masking
{
    /// <summary>
    /// Controls rendering of Cubism masks.
    /// </summary>
    [ExecuteInEditMode, CubismDontMoveOnReimport]
    public sealed class CubismMaskController : MonoBehaviour, ICubismMaskTextureCommandSource, ICubismUpdatable
    {
        /// <summary>
        /// <see cref="MaskTexture"/> backing field.
        /// </summary>
        [SerializeField, HideInInspector]
        private CubismMaskTexture _maskTexture;

        /// <summary>
        /// Mask texture.
        /// </summary>
        public CubismMaskTexture MaskTexture
        {
            get
            {
                // Fall back to global mask texture.
                if (_maskTexture == null)
                {
                    _maskTexture = CubismMaskTexture.GlobalMaskTexture;
                }


                return _maskTexture;
            }
            set
            {
                // Return early if same value given.
                if (value == _maskTexture)
                {
                    return;
                }


                _maskTexture = value;


                // Try switch mask textures.
                OnDestroy();
                Start();
            }
        }


        /// <summary>
        /// <see cref="CubismMaskRenderer"/>s.
        /// </summary>
        private CubismMaskMaskedJunction[] Junctions { get; set; }


        /// <summary>
        /// True if controller is revived.
        /// </summary>
        private bool IsRevived
        {
            get { return Junctions != null; }
        }


        /// <summary>
        /// Model has update controller component.
        /// </summary>
        [HideInInspector]
        public bool HasUpdateController { get; set; }

        /// <summary>
        /// Makes sure controller is initialized once.
        /// </summary>
        private void TryRevive()
        {
            if (IsRevived)
            {
                return;
            }


            ForceRevive();
        }

        /// <summary>
        /// Initializes <see cref="Junctions"/>.
        /// </summary>
        private void ForceRevive()
        {
            var drawables = this
                .FindCubismModel()
                .Drawables;


            // Find mask pairs.
            var pairs = new MasksMaskedsPairs();


            for (var i = 0; i < drawables.Length; i++)
            {
                if (!drawables[i].IsMasked)
                {
                    continue;
                }

                // Make sure no leftover null-entries are added as mask.
                var masks = Array.FindAll(drawables[i].Masks, mask => mask != null);

                if (masks.Length == 0)
                {
                    continue;
                }

                pairs.Add(drawables[i], masks);
            }


            // Initialize junctions.
            Junctions = new CubismMaskMaskedJunction[pairs.Entries.Count];


            for (var i = 0; i < Junctions.Length; ++i)
            {
                // Create mask renderers for junction.
                var masks = new CubismMaskRenderer[pairs.Entries[i].Masks.Length];


                for (var j = 0; j < masks.Length; ++j)
                {
                    masks[j] = new CubismMaskRenderer()
                        .SetMainRenderer(pairs.Entries[i].Masks[j]);
                }


                // Create junction.
                Junctions[i] = new CubismMaskMaskedJunction()
                    .SetMasks(masks)
                    .SetMaskeds(pairs.Entries[i].Maskeds.ToArray())
                    .SetMaskTexture(MaskTexture);
            }
        }

        /// <summary>
        /// Called by cubism update controller. Order to invoke OnLateUpdate.
        /// </summary>
        public int ExecutionOrder
        {
            get { return CubismUpdateExecutionOrder.CubismMaskController; }
        }

        /// <summary>
        /// Called by cubism update controller. Needs to invoke OnLateUpdate on Editing.
        /// </summary>
        public bool NeedsUpdateOnEditing
        {
            get { return true; }
        }

        /// <summary>
        /// Called by cubism update controller. Updates <see cref="Junktions"/>.
        /// </summary>
        public void OnLateUpdate()
        {
            if (!enabled || !IsRevived)
            {
                return;
            }


            for (var i = 0; i < Junctions.Length; ++i)
            {
                Junctions[i].Update();
            }
        }

        #region Unity Event Handling

        /// <summary>
        /// Initializes instance.
        /// </summary>
        private void Start()
        {
            // Fail silently.
            if (MaskTexture == null)
            {
                return;
            }


            MaskTexture.AddSource(this);

            // Get cubism update controller.
            HasUpdateController = (GetComponent<CubismUpdateController>() != null);
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


        /// <summary>
        /// Finalizes instance.
        /// </summary>
        private void OnDestroy()
        {
            if (MaskTexture == null)
            {
                return;
            }


            MaskTexture.RemoveSource(this);
        }

        #endregion

        #region ICubismMaskDrawSource

        /// <summary>
        /// Queries the number of tiles needed by the source.
        /// </summary>
        /// <returns>The necessary number of tiles needed.</returns>
        int ICubismMaskTextureCommandSource.GetNecessaryTileCount()
        {
            TryRevive();


            return Junctions.Length;
        }


        /// <summary>
        /// Assigns the tiles.
        /// </summary>
        /// <param name="value">Tiles to assign.</param>
        void ICubismMaskTextureCommandSource.SetTiles(CubismMaskTile[] value)
        {
            for (var i = 0; i < Junctions.Length; ++i)
            {
                Junctions[i].SetMaskTile(value[i]);
            }
        }


        /// <summary>
        /// Called when source should instantly draw.
        /// </summary>
        void ICubismMaskCommandSource.AddToCommandBuffer(CommandBuffer buffer)
        {
            for (var i = 0; i < Junctions.Length; ++i)
            {
                Junctions[i].AddToCommandBuffer(buffer);
            }
        }

        #endregion

        #region Mask-Masked Pair

        /// <summary>
        /// Pair of masks and masked drawables.
        /// </summary>
        private struct MasksMaskedsPair
        {
            /// <summary>
            /// Mask drawables.
            /// </summary>
            public CubismRenderer[] Masks;

            /// <summary>
            /// Masked drawables.
            /// </summary>
            public List<CubismRenderer> Maskeds;
        }


        private class MasksMaskedsPairs
        {
            /// <summary>
            /// List of <see cref="MasksMaskedsPair"/>
            /// </summary>
            public List<MasksMaskedsPair> Entries = new List<MasksMaskedsPair>();


            /// <summary>
            /// Add <see cref="MasksMaskedsPair"/> to the list.
            /// </summary>
            public void Add(CubismDrawable masked, CubismDrawable[] masks)
            {
                // Try to add masked to existing mask compound.
                for (var i = 0; i < Entries.Count; ++i)
                {
                    var match = (Entries[i].Masks.Length == masks.Length);


                    if (!match)
                    {
                        continue;
                    }


                    for (var j = 0; j < Entries[i].Masks.Length; ++j)
                    {
                        if (Entries[i].Masks[j] != masks[j].GetComponent<CubismRenderer>())
                        {
                            match = false;


                            break;
                        }
                    }


                    if (!match)
                    {
                        continue;
                    }


                    Entries[i].Maskeds.Add(masked.GetComponent<CubismRenderer>());


                    return;
                }


                // Create new pair.
                var renderers = new CubismRenderer[masks.Length];


                for (var i = 0; i < masks.Length; ++i)
                {
                    renderers[i] = masks[i].GetComponent<CubismRenderer>();
                }


                Entries.Add(new MasksMaskedsPair
                {
                    Masks = renderers,
                    Maskeds = new List<CubismRenderer>() { masked.GetComponent<CubismRenderer>() }
                });
            }
        }

        #endregion
    }
}
