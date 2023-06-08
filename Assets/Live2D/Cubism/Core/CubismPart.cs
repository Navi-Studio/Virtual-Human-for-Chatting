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
    /// Single <see cref="CubismModel"/> part.
    /// </summary>
    [CubismDontMoveOnReimport]
    public sealed class CubismPart : MonoBehaviour
    {
        #region Factory Methods

        /// <summary>
        /// Creates parts for a <see cref="CubismModel"/>.
        /// </summary>
        /// <param name="unmanagedModel">Handle to unmanaged model.</param>
        /// <returns>Parts root.</returns>
        internal static GameObject CreateParts(CubismUnmanagedModel unmanagedModel)
        {
            var root = new GameObject("Parts");


            // Create parts.
            var unmanagedParts = unmanagedModel.Parts;
            var buffer = new CubismPart[unmanagedParts.Count];


            for (var i = 0; i < buffer.Length; ++i)
            {
                var proxy = new GameObject();


                buffer[i] = proxy.AddComponent<CubismPart>();


                buffer[i].transform.SetParent(root.transform);
                buffer[i].Reset(unmanagedModel, i);
            }


            return root;
        }

        #endregion


        /// <summary>
        /// Unmanaged parts from unmanaged model.
        /// </summary>
        private CubismUnmanagedParts UnmanagedParts { get; set; }


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
                return UnmanagedParts.Ids[UnmanagedIndex];
            }
        }

        /// <summary>
        /// Current opacity.
        /// </summary>
        [SerializeField, HideInInspector]
        public float Opacity;


        /// <summary>
        /// Revives instance.
        /// </summary>
        /// <param name="unmanagedModel">TaskableModel to unmanaged unmanagedModel.</param>
        internal void Revive(CubismUnmanagedModel unmanagedModel)
        {
            UnmanagedParts = unmanagedModel.Parts;
        }

        /// <summary>
        /// Restores instance to initial state.
        /// </summary>
        /// <param name="unmanagedModel">TaskableModel to unmanaged unmanagedModel.</param>
        /// <param name="unmanagedIndex">Position in unmanaged arrays.</param>
        private void Reset(CubismUnmanagedModel unmanagedModel, int unmanagedIndex)
        {
            Revive(unmanagedModel);


            UnmanagedIndex = unmanagedIndex;
            name = Id;
            Opacity = UnmanagedParts.Opacities[unmanagedIndex];
        }
    }
}
