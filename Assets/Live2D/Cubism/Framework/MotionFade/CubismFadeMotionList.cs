/**
 * Copyright(c) Live2D Inc. All rights reserved.
 *
 * Use of this source code is governed by the Live2D Open Software license
 * that can be found at https://www.live2d.com/eula/live2d-open-software-license-agreement_en.html.
 */


using UnityEngine;


namespace Live2D.Cubism.Framework.MotionFade
{
    [CreateAssetMenu(menuName = "Live2D Cubism/Fade Motion List")]
    public class CubismFadeMotionList : ScriptableObject
    {
        /// <summary>
        /// Cubism fade motion instance ids.
        /// </summary>
        [SerializeField]
        public int[] MotionInstanceIds;

        /// <summary>
        /// Cubism fade motion objects.
        /// </summary>
        [SerializeField]
        public CubismFadeMotionData[] CubismFadeMotionObjects;
    }
}
