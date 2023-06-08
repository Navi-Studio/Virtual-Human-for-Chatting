/**
 * Copyright(c) Live2D Inc. All rights reserved.
 *
 * Use of this source code is governed by the Live2D Open Software license
 * that can be found at https://www.live2d.com/eula/live2d-open-software-license-agreement_en.html.
 */


using Live2D.Cubism.Framework;
using System;
using UnityEngine;
#if UNITY_2019_3_OR_NEWER
using UnityEngine.LowLevel;
using UnityEngine.PlayerLoop;
#elif UNITY_2018_1_OR_NEWER
using UnityEngine.Experimental.LowLevel;
using UnityEngine.Experimental.PlayerLoop;
#endif


namespace Live2D.Cubism.Core
{
    /// <summary>
    /// Runtime Cubism model.
    /// </summary>
    [ExecuteInEditMode, CubismDontMoveOnReimport]
    public sealed class CubismModel : MonoBehaviour
    {
        #region Delegates

        /// <summary>
        /// Handler for <see cref="CubismDynamicDrawableData"/>.
        /// </summary>
        /// <param name="sender">Model the dymanic data applies to.</param>
        /// <param name="data">New data.</param>
        public delegate void DynamicDrawableDataHandler(CubismModel sender, CubismDynamicDrawableData[] data);

        #endregion

        #region Events

        /// <summary>
        /// Event triggered if new <see cref="CubismDynamicDrawableData"/> is available for instance.
        /// </summary>
        public event DynamicDrawableDataHandler OnDynamicDrawableData;

        #endregion

        #region Factory Methods

        /// <summary>
        /// Instantiates a <see cref="CubismMoc"/>.
        /// </summary>
        /// <param name="moc">Cubism moc to instantiate.</param>
        /// <returns>Instance.</returns>
        public static CubismModel InstantiateFrom(CubismMoc moc)
        {
            // Return if argument is invalid.
            if (moc == null)
            {
                return null;
            }


            // Create model.
            var model = new GameObject(moc.name)
                .AddComponent<CubismModel>();


            // Initialize it by resetting it.
            model.Reset(moc);


            return model;
        }

        #endregion

        /// <summary>
        /// Resets a <see cref="CubismMoc"/> reference in <see cref="CubismModel"/>.
        /// </summary>
        /// <param name="model">Target Cubism model.</param>
        /// <param name="moc">Cubism moc to reset.</param>
        public static void ResetMocReference(CubismModel model, CubismMoc moc)
        {
            model.Moc = moc;
        }

        /// <summary>
        /// <see cref="Moc"/> backing field.
        /// </summary>
        [SerializeField, HideInInspector]
        private CubismMoc _moc;

        /// <summary>
        /// Moc the instance was instantiated from.
        /// </summary>
        public CubismMoc Moc
        {
            get { return _moc; }
            private set { _moc = value; }
        }


        /// <summary>
        /// TaskableModel for unmanaged backend.
        /// </summary>
        private CubismTaskableModel TaskableModel { get; set; }


        /// <summary>
        /// <see cref="Parameters"/> backing field.
        /// </summary>
        [NonSerialized]
        private CubismParameter[] _parameters;

        /// <summary>
        /// Drawables of model.
        /// </summary>
        public CubismParameter[] Parameters
        {
            get
            {
                if (_parameters == null)
                {
                    Revive();
                }


                return _parameters;
            }
            private set { _parameters = value; }
        }

        /// <summary>
        /// <see cref="Parts"/> backing field.
        /// </summary>
        [NonSerialized]
        private CubismPart[] _parts;

        /// <summary>
        /// Drawables of model.
        /// </summary>
        public CubismPart[] Parts
        {
            get
            {
                if (_parts == null)
                {
                    Revive();
                }


                return _parts;
            }
            private set { _parts = value; }
        }

        /// <summary>
        /// <see cref="Drawables"/> backing field.
        /// </summary>
        [NonSerialized]
        private CubismDrawable[] _drawables;

        /// <summary>
        /// Drawables of model.
        /// </summary>
        public CubismDrawable[] Drawables
        {
            get
            {
                if (_drawables == null)
                {
                    Revive();
                }


                return _drawables;
            }
            private set { _drawables = value; }
        }

        /// <summary>
        /// <see cref="CanvasInformation"/> backing field.
        /// </summary>
        [NonSerialized]
        private CubismCanvasInformation _canvasInformation;

        /// <summary>
        /// Canvas information of model.
        /// </summary>
        public CubismCanvasInformation CanvasInformation
        {
            get
            {
                if (_canvasInformation == null)
                {
                    Revive();
                }


                return _canvasInformation;
            }
            private set { _canvasInformation = value; }
        }

        /// <summary>
        /// Parameter store cache.
        /// </summary>
        CubismParameterStore _parameterStore;

        /// <summary>
        /// True if instance is revived.
        /// </summary>
        public bool IsRevived
        {
            get { return TaskableModel != null; }
        }

        /// <summary>
        /// True if instance can revive.
        /// </summary>
        private bool CanRevive
        {
            get { return Moc != null; }
        }

#if UNITY_2018_1_OR_NEWER
        /// <summary>
        /// Model update functions for player loop.
        /// </summary>
        [NonSerialized]
        private static Action _modelUpdateFunctions;

        private bool WasAttachedModelUpdateFunction { get; set; }
#endif


        /// <summary>
        /// True on the frame the instance was enabled.
        /// </summary>
        private bool WasJustEnabled { get; set; }

        /// <summary>
        /// Frame number last update was done.
        /// </summary>
        private int LastTick { get; set; }


        /// <summary>
        /// Revives instance.
        /// </summary>
        private void Revive()
        {
            // Return if already revive.
            if (IsRevived)
            {
                return;
            }


            // Return if revive isn't possible.
            if (!CanRevive)
            {
                return;
            }


            // Revive unmanaged model.
            TaskableModel = new CubismTaskableModel(Moc);

            if (TaskableModel == null || TaskableModel.UnmanagedModel == null)
            {
                return;
            }

            // Revive proxies.
            Parameters = GetComponentsInChildren<CubismParameter>();
            Parts = GetComponentsInChildren<CubismPart>();
            Drawables = GetComponentsInChildren<CubismDrawable>();

            Parameters.Revive(TaskableModel.UnmanagedModel);
            Parts.Revive(TaskableModel.UnmanagedModel);
            Drawables.Revive(TaskableModel.UnmanagedModel);

            CanvasInformation = new CubismCanvasInformation(TaskableModel.UnmanagedModel);

            _parameterStore = GetComponent<CubismParameterStore>();
        }

        /// <summary>
        /// Initializes instance for first use.
        /// </summary>
        /// <param name="moc">Moc to instantiate from.</param>
        private void Reset(CubismMoc moc)
        {
            Moc = moc;
            name = moc.name;
            TaskableModel = new CubismTaskableModel(moc);

            if (TaskableModel == null || TaskableModel.UnmanagedModel == null)
            {
                return;
            }

            // Create and initialize proxies.
            var parameters = CubismParameter.CreateParameters(TaskableModel.UnmanagedModel);
            var parts = CubismPart.CreateParts(TaskableModel.UnmanagedModel);
            var drawables = CubismDrawable.CreateDrawables(TaskableModel.UnmanagedModel);


            parameters.transform.SetParent(transform);
            parts.transform.SetParent(transform);
            drawables.transform.SetParent(transform);


            Parameters = parameters.GetComponentsInChildren<CubismParameter>();
            Parts = parts.GetComponentsInChildren<CubismPart>();
            Drawables = drawables.GetComponentsInChildren<CubismDrawable>();

            CanvasInformation = new CubismCanvasInformation(TaskableModel.UnmanagedModel);
        }

        /// <summary>
        /// Forces update.
        /// </summary>
        public void ForceUpdateNow()
        {
            WasJustEnabled = true;
            LastTick = -1;


            Revive();

#if UNITY_2018_1_OR_NEWER
            OnModelUpdate();
#else
            OnRenderObject();
#endif
        }


#if UNITY_2018_1_OR_NEWER
        /// <summary>
        /// Calls model update functions for player loop.
        /// </summary>
        private static void OnModelsUpdate()
        {
            if (_modelUpdateFunctions != null)
            {
                _modelUpdateFunctions.Invoke();
            }
        }


        /// <summary>
        /// Register the model update function into the player loop.
        /// </summary>
        [RuntimeInitializeOnLoadMethod]
        private static void RegisterCallbackFunction()
        {
            // Prepare the function for using player loop.
            var myPlayerLoopSystem = new PlayerLoopSystem()
            {
                type = typeof(CubismModel),     // Identifier for Profiler Hierarchy view.
                updateDelegate = OnModelsUpdate    // Register the function.
            };


            // Get the default player loop.
            var playerLoopSystem =
#if UNITY_2019_3_OR_NEWER
                PlayerLoop.GetCurrentPlayerLoop();
#else
                PlayerLoop.GetDefaultPlayerLoop();
#endif

            var playerLoopIndex = -1;
            for (var i = 0; i < playerLoopSystem.subSystemList.Length; i++)
            {
                if (playerLoopSystem.subSystemList[i].type != typeof(PreLateUpdate))
                {
                    continue;
                }

                playerLoopIndex = i;
                break;
            }

            if (playerLoopIndex < 0)
            {
                Debug.LogError("CubismModel : Failed to add processing to PlayerLoop.");
                return;
            }

            // Get the "PreLateUpdate" system.
            var playerLoopSubSystem = playerLoopSystem.subSystemList[playerLoopIndex];
            var subSystemList = playerLoopSubSystem.subSystemList;


            // Register the model update function after "PreLateUpdate" system.
            Array.Resize(ref subSystemList, subSystemList.Length + 1);
            subSystemList[subSystemList.Length - 1] = myPlayerLoopSystem;


            // Restore the "PreLateUpdate" sytem.
            playerLoopSubSystem.subSystemList = subSystemList;
            playerLoopSystem.subSystemList[playerLoopIndex] = playerLoopSubSystem;
            PlayerLoop.SetPlayerLoop(playerLoopSystem);
        }
#endif

#region Unity Event Handling

        /// <summary>
        /// Called by Unity. Triggers <see langword="this"/> to update.
        /// </summary>
        private void Update()
        {
#if UNITY_2018_1_OR_NEWER
            if (!WasAttachedModelUpdateFunction)
            {
                _modelUpdateFunctions += OnModelUpdate;


                WasAttachedModelUpdateFunction = true;
            }
#endif


            // Return on first frame enabled.
            if (WasJustEnabled)
            {
                return;
            }


            // Return unless revived.
            if (!IsRevived)
            {
                return;
            }


            // Return if backend is ticking.
            if (!TaskableModel.DidExecute)
            {
                return;
            }


            // Sync parameters back.
            TaskableModel.TryReadParameters(Parameters);

            // restore last frame parameters value and parts opacity.
            if(_parameterStore != null)
            {
                _parameterStore.RestoreParameters();
            }

            // Trigger event.
            if (OnDynamicDrawableData == null)
            {
                return;
            }


            OnDynamicDrawableData(this, TaskableModel.DynamicDrawableData);
        }


        /// <summary>
        /// Called by Unity. Blockingly updates <see langword="this"/> on first frame enabled; otherwise tries async update.
        /// </summary>
        private void OnRenderObject()
        {
#if !UNITY_2018_1_OR_NEWER
            OnModelUpdate();
#endif
        }

        /// <summary>
        /// Update model states.
        /// </summary>
        private void OnModelUpdate()
        {
            // Return unless revived.
            if (!IsRevived)
            {
                return;
            }


            // Return if already ticked this frame.
            if (LastTick == Time.frameCount && Application.isPlaying)
            {
                return;
            }


            LastTick = Time.frameCount;


            // Try to sync parameters and parts (without caring whether task is executing or not).
            TaskableModel.TryWriteParametersAndParts(Parameters, Parts);


            // Return if task is executing.
            if (TaskableModel.IsExecuting)
            {
                return;
            }


            // Force blocking update on first frame enabled.
            if (WasJustEnabled)
            {
                // Force sync update.
                TaskableModel.UpdateNow();


                // Unset condition.
                WasJustEnabled = false;


                // Fetch results by calling own 'Update()'.
                Update();


                return;
            }


            // Enqueue update task.
            TaskableModel.Update();
        }

        /// <summary>
        /// Called by Unity. Revives instance.
        /// </summary>
        private void OnEnable()
        {
            WasJustEnabled = true;


            Revive();
        }

        private void OnDisable()
        {
#if UNITY_2018_1_OR_NEWER
            if (WasAttachedModelUpdateFunction)
            {
                _modelUpdateFunctions -= OnModelUpdate;


                WasAttachedModelUpdateFunction = false;
            }
#endif
        }

        /// <summary>
        /// Called by Unity. Releases unmanaged memory.
        /// </summary>
        private void OnDestroy()
        {
            if (!IsRevived)
            {
                return;
            }


            TaskableModel.ReleaseUnmanaged();


            TaskableModel = null;
        }

        /// <summary>
        /// Called by Unity. Triggers <see cref="OnEnable"/>.
        /// </summary>
        private void OnValidate()
        {
            OnEnable();
        }

#endregion
    }
}
