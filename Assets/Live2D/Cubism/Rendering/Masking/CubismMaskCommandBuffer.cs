/**
 * Copyright(c) Live2D Inc. All rights reserved.
 *
 * Use of this source code is governed by the Live2D Open Software license
 * that can be found at https://www.live2d.com/eula/live2d-open-software-license-agreement_en.html.
 */


using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;


namespace Live2D.Cubism.Rendering.Masking
{
    /// <summary>
    /// Singleton buffer for Cubism mask related draw commands.
    /// </summary>
    [ExecuteInEditMode]
    public sealed class CubismMaskCommandBuffer : MonoBehaviour
    {
        /// <summary>
        /// Draw command sources.
        /// </summary>
        private static List<ICubismMaskCommandSource> Sources { get; set; }

        /// <summary>
        /// Command buffer.
        /// </summary>
        private static CommandBuffer Buffer { get; set; }


        /// <summary>
        /// True if <see cref="Sources"/> are empty.
        /// </summary>
        private static bool ContainsSources
        {
            get { return Sources != null && Sources.Count > 0; }
        }


        /// <summary>
        /// Makes sure class is initialized for static usage.
        /// </summary>
        private static void Initialize()
        {
            // Initialize containers.
            if (Sources == null)
            {
                Sources = new List<ICubismMaskCommandSource>();
            }


            if (Buffer == null)
            {
                Buffer = new CommandBuffer
                {
                    name = "cubism_MaskCommandBuffer"
                };
            }


            // Spawn update proxy.
            const string proxyName = "cubism_MaskCommandBuffer";


            var proxy = GameObject.Find(proxyName);


            if (proxy == null)
            {
                proxy = new GameObject(proxyName)
                {
                     hideFlags = HideFlags.HideAndDontSave
                };


                if (!Application.isEditor || Application.isPlaying)
                {
                    DontDestroyOnLoad(proxy);
                }


                proxy.AddComponent<CubismMaskCommandBuffer>();
            }
        }


        /// <summary>
        /// Registers a new draw command source.
        /// </summary>
        /// <param name="source">Source to add.</param>
        internal static void AddSource(ICubismMaskCommandSource source)
        {
            // Make sure singleton is initialized.
            Initialize();


            // Prevent same source from being added twice.
            if (Sources.Contains(source))
            {
                return;
            }


            // Add source and force refresh.
            Sources.Add(source);
        }

        /// <summary>
        /// Deregisters a draw command source.
        /// </summary>
        /// <param name="source">Source to remove.</param>
        internal static void RemoveSource(ICubismMaskCommandSource source)
        {
            // Make sure singleton is initialized.
            Initialize();


            // Remove source and force refresh.
            Sources.RemoveAll(s => s == source);
        }


        /// <summary>
        /// Forces the command buffer to be refreshed.
        /// </summary>
        private static void RefreshCommandBuffer()
        {
            // Clear buffer.
            Buffer.Clear();


            // Enqueue sources.
            for (var i = 0; i < Sources.Count; ++i)
            {
                Sources[i].AddToCommandBuffer(Buffer);
            }
        }

        #region Unity Event Handling

        /// <summary>
        /// Executes <see cref="Buffer"/>.
        /// </summary>
        private void LateUpdate()
        {
            if (!ContainsSources)
            {
                return;
            }


            // Refresh and execute buffer.
            RefreshCommandBuffer();
            Graphics.ExecuteCommandBuffer(Buffer);
        }

        #endregion
    }
}
