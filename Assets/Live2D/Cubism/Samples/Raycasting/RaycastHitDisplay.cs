/**
 * Copyright(c) Live2D Inc. All rights reserved.
 *
 * Use of this source code is governed by the Live2D Open Software license
 * that can be found at https://www.live2d.com/eula/live2d-open-software-license-agreement_en.html.
 */


using UnityEngine;
using Live2D.Cubism.Core;
using Live2D.Cubism.Framework.Raycasting;


namespace Live2D.Cubism.Samples.Raycasting
{
    /// <summary>
    /// Casts rays against a <see cref="Model"/> and displays results.
    /// </summary>
    public sealed class RaycastHitDisplay : MonoBehaviour
    {
        /// <summary>
        /// <see cref="CubismModel"/> to cast rays against.
        /// </summary>
        [SerializeField]
        public CubismModel Model;


        /// <summary>
        /// UI element to display results in.
        /// </summary>
        [SerializeField]
        public UnityEngine.UI.Text ResultsText;


        /// <summary>
        /// <see cref="CubismRaycaster"/> attached to <see cref="Model"/>.
        /// </summary>
        private CubismRaycaster Raycaster { get; set; }

        /// <summary>
        /// Buffer for raycast results.
        /// </summary>
        private CubismRaycastHit[] Results { get; set; }


        /// <summary>
        /// Hit test.
        /// </summary>
        private void DoRaycast()
        {
            // Cast ray from pointer position.
            var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            var hitCount = Raycaster.Raycast(ray, Results);


            // Return early if nothing was hit.
            if (hitCount == 0)
            {
                ResultsText.text = "0";


                return;
            }


            // Show results.
            ResultsText.text = hitCount + "\n";


            for (var i = 0; i < hitCount; i++)
            {
                ResultsText.text += Results[i].Drawable.name + "\n";
            }
        }

        #region Unity Event Handling

        /// <summary>
        /// Called by Unity. Initializes instance.
        /// </summary>
        private void Start()
        {
            Raycaster = Model.GetComponent<CubismRaycaster>();
            Results = new CubismRaycastHit[4];
        }

        /// <summary>
        /// Called by Unity. Triggers raycasting.
        /// </summary>
        private void Update()
        {
            // Return early in case of no user interaction.
            if (!Input.GetMouseButtonDown(0))
            {
                return;
            }


            DoRaycast();
        }

        #endregion
    }
}
