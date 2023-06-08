/**
 * Copyright(c) Live2D Inc. All rights reserved.
 *
 * Use of this source code is governed by the Live2D Open Software license
 * that can be found at https://www.live2d.com/eula/live2d-open-software-license-agreement_en.html.
 */


using UnityEngine;


namespace Live2D.Cubism.Samples.LookAt
{
    /// <summary>
    /// Forces a <see cref="GameObject"/> to face a camera.
    /// </summary>
    public class Billboarder : MonoBehaviour
    {
        /// <summary>
        /// Camera to face.
        /// </summary>
        [SerializeField] public Camera CameraToFace;

        #region Unity Event Handling

        /// <summary>
        /// Called by Unity. Updates facing.
        /// </summary>
        private void Update()
        {
            if (CameraToFace.orthographic)
            {
                transform.LookAt(transform.position - CameraToFace.transform.forward, CameraToFace.transform.up);
            }
            else
            {
                transform.LookAt(CameraToFace.transform.position, CameraToFace.transform.up);
            }
        }

        #endregion
    }
}
