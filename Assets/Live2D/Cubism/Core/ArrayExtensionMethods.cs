/**
 * Copyright(c) Live2D Inc. All rights reserved.
 *
 * Use of this source code is governed by the Live2D Open Software license
 * that can be found at https://www.live2d.com/eula/live2d-open-software-license-agreement_en.html.
 */


using Live2D.Cubism.Core.Unmanaged;
using System;
using UnityEngine;


namespace Live2D.Cubism.Core
{
    /// <summary>
    /// Extension for Cubism related arrays.
    /// </summary>
    public static class ArrayExtensionMethods
    {
        #region Parameters

        /// <summary>
        /// Finds a <see cref="CubismParameter"/> by its ID.
        /// </summary>
        /// <param name="self">Container.</param>
        /// <param name="id">ID to match.</param>
        /// <returns>Parameter on success; <see langword="null"/> otherwise.</returns>
        public static CubismParameter FindById(this CubismParameter[] self, string id)
        {
            if (self == null)
            {
                return null;
            }

            for (var i = 0; i < self.Length; ++i)
            {
                if (self[i].name != id)
                {
                    continue;
                }

                return self[i];
            }

            return null;
        }


        /// <summary>
        /// Revives (and sorts) <see cref="CubismParameter"/>s.
        /// </summary>
        /// <param name="self">Container.</param>
        /// <param name="model">TaskableModel to unmanaged model.</param>
        internal static void Revive(this CubismParameter[] self, CubismUnmanagedModel model)
        {
            Array.Sort(self, (a, b) => a.UnmanagedIndex - b.UnmanagedIndex);


            for (var i = 0; i < self.Length; ++i)
            {
                self[i].Revive(model);
            }
        }

        /// <summary>
        /// Writes opacities to unmanaged model.
        /// </summary>
        /// <param name="self">Source buffer.</param>
        /// <param name="unmanagedModel"></param>
        internal static void WriteTo(this CubismParameter[] self, CubismUnmanagedModel unmanagedModel)
        {
            // Get address.
            var unmanagedParameters = unmanagedModel.Parameters;
            var values = unmanagedParameters.Values;


            // Push.
            for (var i = 0; i < self.Length; ++i)
            {
                values[self[i].UnmanagedIndex] = self[i].Value;
            }
        }

        /// <summary>
        /// Writes opacities to unmanaged model.
        /// </summary>
        /// <param name="self">Source buffer.</param>
        /// <param name="unmanagedModel"></param>
        internal static void ReadFrom(this CubismParameter[] self, CubismUnmanagedModel unmanagedModel)
        {
            // Get address.
            var unmanagedParameters = unmanagedModel.Parameters;
            var values = unmanagedParameters.Values;


            // Pull.
            for (var i = 0; i < self.Length; ++i)
            {
                self[i].Value = values[self[i].UnmanagedIndex];
            }
        }

        #endregion

        #region Parts

        /// <summary>
        /// Finds a <see cref="CubismPart"/> by its ID.
        /// </summary>
        /// <param name="self"><see langword="this"/>.</param>
        /// <param name="id">ID to match.</param>
        /// <returns>Part if found; <see langword="null"/> otherwise.</returns>
        public static CubismPart FindById(this CubismPart[] self, string id)
        {
            if (self == null)
            {
                return null;
            }

            for (var i = 0; i < self.Length; ++i)
            {
                if (self[i].name != id)
                {
                    continue;
                }

                return self[i];
            }

            return null;
        }


        /// <summary>
        /// Revives (and sorts) <see cref="CubismPart"/>s.
        /// </summary>
        /// <param name="self">Container.</param>
        /// <param name="model">TaskableModel to unmanaged model.</param>
        internal static void Revive(this CubismPart[] self, CubismUnmanagedModel model)
        {
            Array.Sort(self, (a, b) => a.UnmanagedIndex - b.UnmanagedIndex);


            for (var i = 0; i < self.Length; ++i)
            {
                self[i].Revive(model);
            }
        }

        /// <summary>
        /// Writes opacities to unmanaged model.
        /// </summary>
        /// <param name="self">Source buffer.</param>
        /// <param name="unmanagedModel"></param>
        internal static void WriteTo(this CubismPart[] self, CubismUnmanagedModel unmanagedModel)
        {
            // Get address.
            var unmanagedParts = unmanagedModel.Parts;
            var opacities = unmanagedParts.Opacities;


            // Push.
            for (var i = 0; i < self.Length; ++i)
            {
                opacities[self[i].UnmanagedIndex] = self[i].Opacity;
            }
        }

        #endregion

        #region Drawables

        /// <summary>
        /// Finds a <see cref="CubismParameter"/> by its ID.
        /// </summary>
        /// <param name="self"><see langword="this"/>.</param>
        /// <param name="id">ID to match.</param>
        /// <returns>Part if found; <see langword="null"/> otherwise.</returns>
        public static CubismDrawable FindById(this CubismDrawable[] self, string id)
        {
            if (self == null)
            {
                return null;
            }

            for (var i = 0; i < self.Length; ++i)
            {
                if (self[i].name != id)
                {
                    continue;
                }

                return self[i];
            }

            return null;
        }


        /// <summary>
        /// Revives (and sorts) <see cref="CubismDrawable"/>s.
        /// </summary>
        /// <param name="self">Container.</param>
        /// <param name="model">TaskableModel to unmanaged model.</param>
        internal static void Revive(this CubismDrawable[] self, CubismUnmanagedModel model)
        {
            Array.Sort(self, (a, b) => a.UnmanagedIndex - b.UnmanagedIndex);


            for (var i = 0; i < self.Length; ++i)
            {
                self[i].Revive(model);
            }
        }


        /// <summary>
        /// Reads new data from a model.
        /// </summary>
        /// <param name="self">Buffer to write to.</param>
        /// <param name="unmanagedModel">Unmanaged model to read from.</param>
        internal static unsafe void ReadFrom(this CubismDynamicDrawableData[] self, CubismUnmanagedModel unmanagedModel)
        {
            // Get addresses.
            var drawables = unmanagedModel.Drawables;
            var flags = drawables.DynamicFlags;
            var opacities = drawables.Opacities;
            var drawOrders = drawables.DrawOrders;
            var renderOrders = drawables.RenderOrders;
            var vertexPositions = drawables.VertexPositions;
            var multiplyColors = drawables.MultiplyColors;
            var screenColors = drawables.ScreenColors;

            // Pull data.
            for (var i = 0; i < self.Length; ++i)
            {
                var data = self[i];


                data.Flags = flags[i];
                data.Opacity = opacities[i];
                data.DrawOrder = drawOrders[i];
                data.RenderOrder = renderOrders[i];


                // Read vertex positions only if necessary.
                if (!data.AreVertexPositionsDirty)
                {
                    continue;
                }


                // Copy vertex positions.
                fixed (Vector3* dataVertexPositions = data.VertexPositions)
                {
                    for (var v = 0; v < data.VertexPositions.Length; ++v)
                    {
                        dataVertexPositions[v].x = vertexPositions[i][(v * 2) + 0];
                        dataVertexPositions[v].y = vertexPositions[i][(v * 2) + 1];
                    }
                }

                if (!data.IsBlendColorDirty)
                {
                    continue;
                }

                var rgbaIndex = i * 4;
                data.MultiplyColor = new Color(multiplyColors[rgbaIndex], multiplyColors[rgbaIndex + 1], multiplyColors[rgbaIndex + 2], multiplyColors[rgbaIndex + 3]);
                data.ScreenColor = new Color(screenColors[rgbaIndex], screenColors[rgbaIndex + 1], screenColors[rgbaIndex + 2], screenColors[rgbaIndex + 3]);
            }


            // Clear dynamic flags.
            drawables.ResetDynamicFlags();
        }

        #endregion
    }
}
