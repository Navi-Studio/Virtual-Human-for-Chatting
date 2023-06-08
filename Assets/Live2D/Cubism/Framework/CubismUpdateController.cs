/**
 * Copyright(c) Live2D Inc. All rights reserved.
 *
 * Use of this source code is governed by the Live2D Open Software license
 * that can be found at https://www.live2d.com/eula/live2d-open-software-license-agreement_en.html.
 */

using Live2D.Cubism.Core;
using System.Collections.Generic;
using UnityEngine;


namespace Live2D.Cubism.Framework
{
    [ExecuteInEditMode]
    public class CubismUpdateController : MonoBehaviour
    {
        /// <summary>
        /// The action of cubism component late update.
        /// </summary>
        private System.Action _onLateUpdate;

        /// <summary>
        /// Refresh delegate manager.
        /// </summary>
        public void Refresh()
        {
            var model = this.FindCubismModel();

            // Fail silently...
            if (model == null)
            {
                return;
            }

            // Set the null value when refreshed UpdateController to avoid duplicated registering.
            _onLateUpdate = null;

            // Set delegate.
            var components = model.GetComponents<ICubismUpdatable>();
            var sortedComponents = new List<ICubismUpdatable>(components);
            CubismUpdateExecutionOrder.SortByExecutionOrder(sortedComponents);

            foreach(var component in sortedComponents)
            {
#if UNITY_EDITOR
                if (!Application.isPlaying && !component.NeedsUpdateOnEditing)
                {
                    continue;
                }
#endif

                _onLateUpdate += component.OnLateUpdate;
            }
        }

        #region Unity Event Handling

        /// <summary>
        /// Called by Unity.
        /// </summary>
        private void Start()
        {
            Refresh();
        }

        /// <summary>
        /// Called by Unity.
        /// </summary>
        private void LateUpdate()
        {
            // Cubism late update.
            if(_onLateUpdate != null)
            {
                _onLateUpdate();
            }
        }

        #endregion
    }
}
