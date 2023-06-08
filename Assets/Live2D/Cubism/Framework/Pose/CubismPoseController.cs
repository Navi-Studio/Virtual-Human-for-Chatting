/**
 * Copyright(c) Live2D Inc. All rights reserved.
 *
 * Use of this source code is governed by the Live2D Open Software license
 * that can be found at https://www.live2d.com/eula/live2d-open-software-license-agreement_en.html.
 */


using Live2D.Cubism.Core;
using System;
using UnityEngine;

namespace Live2D.Cubism.Framework.Pose
{
    /// <summary>
    /// Cubism pose controller.
    /// </summary>
    public sealed class CubismPoseController : MonoBehaviour, ICubismUpdatable
    {
        #region variable

        /// <summary>
        /// Default visible pose index.
        /// </summary>
        [SerializeField]
        public int defaultPoseIndex = 0;

        /// <summary>
        /// Back opacity threshold.
        /// </summary>
        private const float BackOpacityThreshold = 0.15f;

        /// <summary>
        /// Cubism model cache.
        /// </summary>
        private CubismModel _model;

        /// <summary>
        /// Model has update controller component.
        /// </summary>
        [HideInInspector]
        public bool HasUpdateController { get; set; }

        /// <summary>
        /// Pose data.
        /// </summary>
        private CubismPoseData[][] _poseData;

        #endregion

        #region Function

        /// <summary>
        /// update hidden part opacity.
        /// </summary>
        public void Refresh()
        {
            _model = this.FindCubismModel();

            // Fail silently...
            if (_model == null)
            {
                return;
            }

            var tags = _model
                .Parts
                .GetComponentsMany<CubismPosePart>();

            for(var i = 0; i < tags.Length; ++i)
            {
                var groupIndex = tags[i].GroupIndex;
                var partIndex = tags[i].PartIndex;

                if(_poseData == null || _poseData.Length <= groupIndex)
                {
                    Array.Resize(ref _poseData, groupIndex + 1);
                }

                if(_poseData[groupIndex] == null || _poseData[groupIndex].Length <= partIndex)
                {
                    Array.Resize(ref _poseData[groupIndex], partIndex + 1);
                }

                _poseData[groupIndex][partIndex].PosePart = tags[i];
                _poseData[groupIndex][partIndex].Part= tags[i].GetComponent<CubismPart>();

                defaultPoseIndex = (defaultPoseIndex < 0) ? 0 : defaultPoseIndex;
                if (partIndex != defaultPoseIndex)
                {
                    _poseData[groupIndex][partIndex].Part.Opacity = 0.0f;
                }

                _poseData[groupIndex][partIndex].Opacity = _poseData[groupIndex][partIndex].Part.Opacity;

                if(tags[i].Link == null || tags[i].Link.Length == 0)
                {
                    continue;
                }

                _poseData[groupIndex][partIndex].LinkParts = new CubismPart[tags[i].Link.Length];

                for(var j = 0; j < tags[i].Link.Length; ++j)
                {
                    var linkId = tags[i].Link[j];
                    _poseData[groupIndex][partIndex].LinkParts[j] = _model.Parts.FindById(linkId);
                }
            }

            // Get cubism update controller.
            HasUpdateController = (GetComponent<CubismUpdateController>() != null);
        }

        /// <summary>
        /// update hidden part opacity.
        /// </summary>
        private void DoFade()
        {
            for(var groupIndex = 0; groupIndex < _poseData.Length; ++groupIndex)
            {
                var appearPartsGroupIndex = -1;
                var appearPartsGroupOpacity = 1.0f;

                // Find appear parts group index and opacity.
                for (var i = 0; i < _poseData[groupIndex].Length; ++i)
                {
                    var part = _poseData[groupIndex][i].Part;

                    if(part.Opacity > _poseData[groupIndex][i].Opacity)
                    {
                        appearPartsGroupIndex = i;
                        appearPartsGroupOpacity = part.Opacity;
                        break;
                    }
                }

                // Fail silently...
                if(appearPartsGroupIndex < 0)
                {
                    return;
                }

                // Delay disappearing parts groups disappear.
                for (var i = 0; i < _poseData[groupIndex].Length; ++i)
                {
                    // Fail silently...
                    if(i == appearPartsGroupIndex)
                    {
                        continue;
                    }

                    var part = _poseData[groupIndex][i].Part;
                    var delayedOpacity = part.Opacity;
                    var backOpacity = (1.0f - delayedOpacity) * (1.0f - appearPartsGroupOpacity);

                    // When restricting the visible proportion of the background
                    if (backOpacity > BackOpacityThreshold)
                    {
                        delayedOpacity = 1.0f - BackOpacityThreshold / (1.0f - appearPartsGroupOpacity);
                    }

                    // Overwrite the opacity if it's greater than the delayed opacity.
                    if (part.Opacity > delayedOpacity)
                    {
                        part.Opacity = delayedOpacity;
                    }
                }
            }
        }

        /// <summary>
        /// Copy opacity to linked parts.
        /// </summary>
        private void CopyPartOpacities()
        {
            for(var groupIndex = 0; groupIndex < _poseData.Length; ++groupIndex)
            {
                for (var partIndex = 0; partIndex < _poseData[groupIndex].Length; ++partIndex)
                {
                    var linkParts = _poseData[groupIndex][partIndex].LinkParts;

                    if(linkParts == null)
                    {
                        continue;
                    }

                    var opacity = _poseData[groupIndex][partIndex].Part.Opacity;

                    for (var linkIndex = 0; linkIndex < linkParts.Length; ++linkIndex)
                    {
                        var linkPart = linkParts[linkIndex];

                        if(linkPart != null)
                        {
                            linkPart.Opacity = opacity;
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Save parts opacity.
        /// </summary>
        private void SavePartOpacities()
        {
            for(var groupIndex = 0; groupIndex < _poseData.Length; ++groupIndex)
            {
                for (var partIndex = 0; partIndex < _poseData[groupIndex].Length; ++partIndex)
                {
                    _poseData[groupIndex][partIndex].Opacity = _poseData[groupIndex][partIndex].Part.Opacity;
                }
            }
        }

        /// <summary>
        /// Called by cubism update controller. Order to invoke OnLateUpdate.
        /// </summary>
        public int ExecutionOrder
        {
            get { return CubismUpdateExecutionOrder.CubismPoseController; }
        }

        /// <summary>
        /// Called by cubism update controller. Needs to invoke OnLateUpdate on Editing.
        /// </summary>
        public bool NeedsUpdateOnEditing
        {
            get { return false; }
        }

        /// <summary>
        /// Called by cubism update manager. Updates controller.
        /// </summary>
        public void OnLateUpdate()
        {
            // Fail silently...
            if (!enabled || _model == null || _poseData == null)
            {
               return;
            }

            DoFade();
            CopyPartOpacities();
            SavePartOpacities();
        }

        #endregion

        #region Unity Event Handling

        /// <summary>
        /// Called by Unity. Makes sure cache is initialized.
        /// </summary>
        private void OnEnable()
        {
            Refresh();
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

        #endregion
    }

}
