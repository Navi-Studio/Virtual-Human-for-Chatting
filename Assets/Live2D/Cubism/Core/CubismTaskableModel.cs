/**
 * Copyright(c) Live2D Inc. All rights reserved.
 *
 * Use of this source code is governed by the Live2D Open Software license
 * that can be found at https://www.live2d.com/eula/live2d-open-software-license-agreement_en.html.
 */


using Live2D.Cubism.Core.Unmanaged;
using System.Threading;
using UnityEngine;


namespace Live2D.Cubism.Core
{
    /// <summary>
    /// 'Atomic' <see cref="CubismModel"/> update task.
    /// </summary>
    internal sealed class CubismTaskableModel : ICubismTask
    {
        #region Factory Methods

        /// <summary>
        /// Creates a <see cref="CubismTaskableModel"/> from a <see cref="CubismMoc"/>.
        /// </summary>
        /// <param name="moc">Moc source.</param>
        /// <returns>Instance.</returns>
        public static CubismTaskableModel CreateTaskableModel(CubismMoc moc)
        {
            return new CubismTaskableModel(moc);
        }

        #endregion

        /// <summary>
        /// Handle to unmanaged model.
        /// </summary>
        public CubismUnmanagedModel UnmanagedModel { get; private set; }

        /// <summary>
        /// <see cref="CubismMoc"/> the model was instantiated from.
        /// </summary>
        public CubismMoc Moc { get; private set; }


        private CubismDynamicDrawableData[] _dynamicDrawableData;

        /// <summary>
        /// Buffer to write dynamic data to.
        /// </summary>
        public CubismDynamicDrawableData[] DynamicDrawableData
        {
            get
            {
                CubismDynamicDrawableData[] dynamicDrawableData = null;

                if (Monitor.TryEnter(Lock))
                {
                    dynamicDrawableData = _dynamicDrawableData;

                    Monitor.Exit(Lock);
                }


                return dynamicDrawableData;
            }


            private set
            {
                _dynamicDrawableData = value;
            }
        }


        /// <summary>
        /// True if task is currently executing.
        /// </summary>
        public bool IsExecuting
        {
            get
            {
                var isExecuting = false;


                if (Monitor.TryEnter(Lock))
                {
                    isExecuting = (State == TaskState.Enqueued || State == TaskState.Executing);


                    Monitor.Exit(Lock);
                }


                return isExecuting;
            }
        }

        /// <summary>
        /// True if did run to completion at least once.
        /// </summary>
        public bool DidExecute
        {
            get
            {
                var didExecute = false;


                if (Monitor.TryEnter(Lock))
                {
                    didExecute = (State == TaskState.Executed);

                    Monitor.Exit(Lock);
                }


                return didExecute;
            }
        }

        /// <summary>
        /// True if unmanaged model and moc should be released.
        /// </summary>
        private bool ShouldReleaseUnmanaged { get; set; }

        #region Constructor

        /// <summary>
        /// Initializes instance.
        /// </summary>
        /// <param name="moc">Moc unmanaged model was instantiated from.</param>
        public CubismTaskableModel(CubismMoc moc)
        {
            Moc = moc;


            // Instantiate unmanaged model.
            var unmanagedMoc = moc.AcquireUnmanagedMoc();


            UnmanagedModel = CubismUnmanagedModel.FromMoc(unmanagedMoc);

            if (UnmanagedModel == null)
            {
                Debug.LogError("This .moc3 file version is \"Unknown\"!!\n" +
                               "It may be broken or you are trying to use a higher version of Moc than Cubism Core.\n" +
                               "Check the supported versions at CubismMoc.LatestVersion.\n" +
                               "The \"CoreDll\" constants indicate which Moc version the numbers are assigned to.");
                return;
            }

            Lock = new object();
            State = TaskState.Idle;
            DynamicDrawableData = CubismDynamicDrawableData.CreateData(UnmanagedModel);
            ShouldReleaseUnmanaged = false;
        }

        #endregion

        /// <summary>
        /// Tries to read parameters into a buffer.
        /// </summary>
        /// <param name="parameters">Buffer to write to.</param>
        /// <returns><see langword="true"/> on success; <see langword="false"/> otherwise.</returns>
        public bool TryReadParameters(CubismParameter[] parameters)
        {
            var didRead = false;


            if (Monitor.TryEnter(Lock))
            {
                try
                {
                    if (State == TaskState.Executed)
                    {
                        parameters.ReadFrom(UnmanagedModel);


                        didRead = true;
                    }
                }
                finally
                {
                    Monitor.Exit(Lock);
                }
            }


            return didRead;
        }

        /// <summary>
        /// Tries to write parameters to a buffer.
        /// </summary>
        /// <param name="parameters">Buffer to read from.</param>
        /// <param name="parts">Buffer to read from.</param>
        /// <returns><see langword="true"/> on success; <see langword="false"/> otherwise.</returns>
        public bool TryWriteParametersAndParts(CubismParameter[] parameters, CubismPart[] parts)
        {
            var didWrite = false;


            if (Monitor.TryEnter(Lock))
            {
                try
                {
                    if (State != TaskState.Executing)
                    {
                        parameters.WriteTo(UnmanagedModel);
                        parts.WriteTo(UnmanagedModel);


                        didWrite = true;
                    }
                }
                finally
                {
                    Monitor.Exit(Lock);
                }
            }


            return didWrite;
        }


        /// <summary>
        /// Dispatches the task for (maybe async) execution.
        /// </summary>
        public void Update()
        {
            // Validate state.
            lock (Lock)
            {
                if (State == TaskState.Enqueued || State == TaskState.Executing)
                {
                    return;
                }


                // Update state.
                State = TaskState.Enqueued;
            }


            CubismTaskQueue.Enqueue(this);
        }

        /// <summary>
        /// Forces the task to run now to completion.
        /// </summary>
        public bool UpdateNow()
        {
            // Validate state.
            lock (Lock)
            {
                if (State == TaskState.Enqueued || State == TaskState.Executing)
                {
                    return false;
                }


                // Update state.
                State = TaskState.Enqueued;
            }


            // Run execution directly.
            Execute();


            return true;
        }


        /// <summary>
        /// Releases unmanaged resource.
        /// </summary>
        public void ReleaseUnmanaged()
        {
            ShouldReleaseUnmanaged = true;


            // Return if task is ongoing.
            lock (Lock)
            {
                if (State == TaskState.Enqueued || State == TaskState.Executing)
                {
                    return;
                }
            }


            OnReleaseUnmanaged();


            ShouldReleaseUnmanaged = false;
        }


        /// <summary>
        /// Runs the task.
        /// </summary>
        private void Execute()
        {
            // Validate state.
            lock (Lock)
            {
                State = TaskState.Executing;
            }


            // Update native backend.
            UnmanagedModel.Update();


            // Get results.
            DynamicDrawableData.ReadFrom(UnmanagedModel);


            // Update state.
            lock (Lock)
            {
                State = TaskState.Executed;


                // Release native if requested.
                if (ShouldReleaseUnmanaged)
                {
                    OnReleaseUnmanaged();
                }
            }
        }


        /// <summary>
        /// Actually releases native resource(s).
        /// </summary>
        private void OnReleaseUnmanaged()
        {
            UnmanagedModel.Release();
            Moc.ReleaseUnmanagedMoc();


            UnmanagedModel = null;
        }

        #region Implementation of ICubismTask

        void ICubismTask.Execute()
        {
            Execute();
        }

        #endregion

        #region Threading

        /// <summary>
        /// Task states.
        /// </summary>
        private enum TaskState
        {
            /// <summary>
            /// Idle state.
            /// </summary>
            Idle,

            /// <summary>
            /// Waiting-for-execution state.
            /// </summary>
            Enqueued,

            /// <summary>
            /// Executing state.
            /// </summary>
            Executing,

            /// <summary>
            /// Executed state.
            /// </summary>
            Executed
        }


        /// <summary>
        /// Lock.
        /// </summary>
        private object Lock { get; set; }

        /// <summary>
        /// Internal state.
        /// </summary>
        private TaskState State { get; set; }

        #endregion
    }
}
