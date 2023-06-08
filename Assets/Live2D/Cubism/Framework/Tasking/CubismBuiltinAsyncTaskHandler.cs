/**
 * Copyright(c) Live2D Inc. All rights reserved.
 *
 * Use of this source code is governed by the Live2D Open Software license
 * that can be found at https://www.live2d.com/eula/live2d-open-software-license-agreement_en.html.
 */


using Live2D.Cubism.Core;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;


namespace Live2D.Cubism.Framework.Tasking
{
    /// <summary>
    /// Built-in task handler, works async.
    /// </summary>
    public static class CubismBuiltinAsyncTaskHandler
    {
        #region Workers

        /// <summary>
        /// <see cref="ICubismTask"/>s waiting for execution.
        /// </summary>
        private static Queue<ICubismTask> Tasks { get; set; }

        /// <summary>
        /// Background worker threads.
        /// </summary>
        private static Thread Worker { get; set; }

        /// <summary>
        /// Lock for syncing access to <see cref="Tasks"/> and <see cref="CallItADay"/>.
        /// </summary>
        private static object Lock { get; set; }

        /// <summary>
        /// Signal for waking up workers.
        /// </summary>
        private static ManualResetEvent Signal { get; set; }


        /// <summary>
        /// <see cref="CallItADay"/> backing field. ALWAYS ACCESS THROUGH PROPERTY!
        /// </summary>
        private static bool _callItADay;

        /// <summary>
        /// True if workers should exit.
        /// </summary>
        private static bool CallItADay
        {
            get
            {
                lock (Lock)
                {
                    return _callItADay;
                }
            }
            set
            {
                lock (Lock)
                {
                    _callItADay = value;
                }
            }
        }


        /// <summary>
        /// Initializes async task handling.
        /// </summary>
        public static void Activate()
        {
            // Check if it is already set.
            if (CubismTaskQueue.OnTask != null && CubismTaskQueue.OnTask != EnqueueTask)
            {
                Debug.LogWarning("\"CubismTaskQueue.OnTask\" already set.");


                return;
            }


            // Initialize fields.
            Tasks = new Queue<ICubismTask>();
            Worker = new Thread(Work);
            Lock = new object();
            Signal = new ManualResetEvent(false);
            CallItADay = false;


            // Become handler.
            CubismTaskQueue.OnTask = EnqueueTask;


            // Start worker.
            Worker.Start();
        }


        /// <summary>
        /// Cleanup workers.
        /// </summary>
        public static void Deactivate()
        {
            // Return early if self isn' handler.
            if (CubismTaskQueue.OnTask != EnqueueTask)
            {
                return;
            }


            // Unbecome handler.
            CubismTaskQueue.OnTask = null;


            // Stop worker.
            CallItADay = true;


            if (Worker != null)
            {
                Signal.Set();
                Worker.Join();
            }


            // Reset fields
            Tasks = null;
            Worker = null;
            Lock = null;
            Signal = null;
        }


        /// <summary>
        /// Enqueues a new task.
        /// </summary>
        /// <param name="task">Task to enqueue.</param>
        private static void EnqueueTask(ICubismTask task)
        {
            lock (Lock)
            {
                Tasks.Enqueue(task);
                Signal.Set();
            }
        }

        /// <summary>
        /// Dequeues a task.
        /// </summary>
        /// <returns>A valid task on success; <see langword="null"/> otherwise.</returns>
        private static ICubismTask DequeueTask()
        {
            lock (Lock)
            {
                return (Tasks.Count > 0)
                    ? Tasks.Dequeue()
                    : null;
            }
        }


        /// <summary>
        /// Entry point for workers.
        /// </summary>
        private static void Work()
        {
            while (!CallItADay)
            {
                // Try to dequeue a task.
                var task = DequeueTask();


                // Execute task if available.
                if (task != null)
                {
                    task.Execute();
                }


                // Wait for a task to become available.
                else
                {
                    Signal.WaitOne();
                    Signal.Reset();
                }
            }
        }

        #endregion
    }
}
