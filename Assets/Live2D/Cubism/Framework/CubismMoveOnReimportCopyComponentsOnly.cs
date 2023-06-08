/**
 * Copyright(c) Live2D Inc. All rights reserved.
 *
 * Use of this source code is governed by the Live2D Open Software license
 * that can be found at https://www.live2d.com/eula/live2d-open-software-license-agreement_en.html.
 */


using System;
using UnityEngine;


namespace Live2D.Cubism.Framework
{
    /// <summary>
    /// When attached to a <see cref="MonoBehaviour"/>,
    /// prevents the <see cref="MonoBehaviour"/> from getting moved on Cubism model reimport.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public class CubismMoveOnReimportCopyComponentsOnly : Attribute
    {
    }
}
