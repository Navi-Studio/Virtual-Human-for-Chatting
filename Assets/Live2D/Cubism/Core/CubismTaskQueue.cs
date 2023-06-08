/**
 * Copyright(c) Live2D Inc. All rights reserved.
 *
 * Use of this source code is governed by the Live2D Open Software license
 * that can be found at https://www.live2d.com/eula/live2d-open-software-license-agreement_en.html.
 */


namespace Live2D.Cubism.Core
{
    /// <summary>
    /// TOOD Document.
    /// </summary>
    public static class CubismTaskQueue
    {
        #region Delegates

        /// <summary>
        /// Handles <see cref="ICubismTask"/>s.
        /// </summary>
        /// <param name="task"></param>
        public delegate void CubismTaskHandler(ICubismTask task);

        #endregion

        #region Events

        /// <summary>
        /// Event triggered on new <see cref="ICubismTask"/> enqueued.
        /// </summary>
        public static CubismTaskHandler OnTask;

        #endregion

        /// <summary>
        /// Enqeues a <see cref="ICubismTask"/>.
        /// </summary>
        /// <param name="task"></param>
        internal static void Enqueue(ICubismTask task)
        {
            // Execute task idrectly in case enqueueing isn't enabled.
            if (OnTask == null)
            {
                task.Execute();


                return;
            }


            OnTask(task);
        }
    }
}
