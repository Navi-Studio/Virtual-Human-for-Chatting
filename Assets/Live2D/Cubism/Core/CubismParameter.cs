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
    /// Single <see cref="CubismModel"/> parameter.
    /// </summary>
    [CubismDontMoveOnReimport]
    public sealed class CubismParameter : MonoBehaviour
    {
        #region Factory Methods

        /// <summary>
        /// Creates drawables for a <see cref="CubismModel"/>.
        /// </summary>
        /// <param name="unmanagedModel">Handle to unmanaged model.</param>
        /// <returns>Drawables root.</returns>
        internal static GameObject CreateParameters(CubismUnmanagedModel unmanagedModel)
        {
            var root = new GameObject("Parameters");


            // Create parameters.
            var unmanagedParameters = unmanagedModel.Parameters;
            var buffer = new CubismParameter[unmanagedParameters.Count];


            for (var i = 0; i < buffer.Length; ++i)
            {
                var proxy = new GameObject();


                buffer[i] = proxy.AddComponent<CubismParameter>();


                buffer[i].transform.SetParent(root.transform);
                buffer[i].Reset(unmanagedModel, i);
            }


            return root;
        }

        #endregion

        /// <summary>
        /// Unmanaged parameters from unmanaged model.
        /// </summary>
        private CubismUnmanagedParameters UnmanagedParameters { get; set; }


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
                return UnmanagedParameters.Ids[UnmanagedIndex];
            }
        }

        /// <summary>
        /// Get Parameter Type.
        /// </summary>
        public int Type
        {
            get
            {
                // Pull data.
                return UnmanagedParameters.Types[UnmanagedIndex];
            }
        }

        /// <summary>
        /// Minimum value.
        /// </summary>
        public float MinimumValue
        {
            get
            {
                // Pull data.
                return UnmanagedParameters.MinimumValues[UnmanagedIndex];
            }
        }

        /// <summary>
        /// Maximum value.
        /// </summary>
        public float MaximumValue
        {
            get
            {
                // Pull data.
                return UnmanagedParameters.MaximumValues[UnmanagedIndex];
            }
        }

        /// <summary>
        /// Default value.
        /// </summary>
        public float DefaultValue
        {
            get
            {
                // Pull data.
                return UnmanagedParameters.DefaultValues[UnmanagedIndex];
            }
        }

        /// <summary>
        /// Current value.
        /// </summary>
        [SerializeField, HideInInspector]
        public float Value;


        /// <summary>
        /// Revives the instance.
        /// </summary>
        /// <param name="unmanagedModel">Handle to unmanaged model.</param>
        internal void Revive(CubismUnmanagedModel unmanagedModel)
        {
            UnmanagedParameters = unmanagedModel.Parameters;
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
            Value = DefaultValue;
        }
    }
}
