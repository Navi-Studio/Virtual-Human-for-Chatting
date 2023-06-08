/**
 * Copyright(c) Live2D Inc. All rights reserved.
 *
 * Use of this source code is governed by the Live2D Open Software license
 * that can be found at https://www.live2d.com/eula/live2d-open-software-license-agreement_en.html.
 */


using UnityEngine;


namespace Live2D.Cubism.Framework.Pose
{
    /// <summary>
    /// Tagging component for pose part.
    /// </summary>
    public sealed class CubismPosePart : MonoBehaviour
    {
        [SerializeField]
        public int GroupIndex;

        [SerializeField]
        public int PartIndex;

        [SerializeField]
        public string[] Link;
    }
}
