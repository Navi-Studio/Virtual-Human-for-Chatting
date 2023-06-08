/**
 * Copyright(c) Live2D Inc. All rights reserved.
 *
 * Use of this source code is governed by the Live2D Open Software license
 * that can be found at https://www.live2d.com/eula/live2d-open-software-license-agreement_en.html.
 */


using System;
using Live2D.Cubism.Core;
using UnityEngine;


namespace Live2D.Cubism.Framework.Physics
{
    /// <summary>
    /// Physics rig.
    /// </summary>
    [Serializable]
    public class CubismPhysicsRig
    {
        /// <summary>
        /// Children of rig.
        /// </summary>
        [SerializeField]
        public CubismPhysicsSubRig[] SubRigs;


        [SerializeField]
        public Vector2 Gravity = CubismPhysics.Gravity;


        [SerializeField]
        public Vector2 Wind = CubismPhysics.Wind;

        [SerializeField]
        public float Fps = 0.0f;


        private float _currentRemainTime; // Time not processed by physics.

        public float[] ParametersCache
        {
            get { return _parametersCache; }
            set { _parametersCache = value; }
        }

        [NonSerialized]
        private float[] _parametersCache; // Cache parameters used by Evaluate.

        [NonSerialized]
        private float[] _parametersInputCache; // Cache input when UpdateParticles runs.

        /// <summary>
        /// Reference of controller to refer from children rig.
        /// </summary>
        public CubismPhysicsController Controller { get; set; }

        /// <summary>
        /// Get <see cref="CubismPhysicsSubRig"/> by name
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public CubismPhysicsSubRig GetSubRig(string name)
        {
            for (int i = 0; i < SubRigs.Length; i++)
            {
                if (SubRigs[i].Name == name)
                {
                    return SubRigs[i];
                }
            }

            return null;
        }

        /// <summary>
        /// Initializes rigs.
        /// </summary>
        public void Initialize()
        {
            _currentRemainTime = 0.0f;

            Controller.gameObject.FindCubismModel();

            _parametersCache = new float[Controller.Parameters.Length];
            _parametersInputCache = new float[Controller.Parameters.Length];

            for (var i = 0; i < SubRigs.Length; ++i)
            {
                SubRigs[i].Initialize();
            }
        }

        /// <summary>
        /// Calculations are performed until the physics are stable.
        /// </summary>
        public void Stabilization()
        {
            if (Controller == null)
            {
                return;
            }

            // Initialize.
            if (_parametersCache == null)
            {
                _parametersCache = new float[Controller.Parameters.Length];
            }

            if (_parametersCache.Length < Controller.Parameters.Length)
            {
                Array.Resize(ref _parametersCache, Controller.Parameters.Length);
            }

            if (_parametersInputCache == null)
            {
                _parametersInputCache = new float[Controller.Parameters.Length];
            }

            if (_parametersInputCache.Length < Controller.Parameters.Length)
            {
                Array.Resize(ref _parametersInputCache, Controller.Parameters.Length);
            }

            // Obtain and cache the current parameter posture.
            for (var i = 0; i < Controller.Parameters.Length; i++)
            {
                _parametersCache[i] = Controller.Parameters[i].Value;
                _parametersInputCache[i] = _parametersCache[i];
            }

            // Evaluate.
            for (var i = 0; i < SubRigs.Length; ++i)
            {
                SubRigs[i].Stabilization();
            }

            var model = Controller.gameObject.FindCubismModel();
            model.ForceUpdateNow();
        }

        /// <summary>
        /// Evaluate rigs.
        ///
        /// Pendulum interpolation weights
        ///
        /// The result of the pendulum calculation is saved and
        /// the output to the parameters is interpolated with the saved previous result of the pendulum calculation.
        ///
        /// The figure shows the interpolation between [1] and [2].
        ///
        /// The weight of the interpolation are determined by the current time seen between
        /// the latest pendulum calculation timing and the next timing.
        ///
        /// Figure shows the weight of position (3) as seen between [2] and [4].
        ///
        /// As an interpretation, the pendulum calculation and weights are misaligned.
        ///
        /// If there is no FPS information in physics3.json, it is always set in the previous pendulum state.
        ///
        /// The purpose of this specification is to avoid the quivering appearance caused by deviations from the interpolation range.
        ///
        /// ------------ time -------------->
        ///
        ///    　　　　　　　　|+++++|------| <- weight
        /// ==[1]====#=====[2]---(3)----(4)
        ///          ^ output contents
        ///
        /// 1: _previousRigOutput
        /// 2: _currentRigOutput
        /// 3: _currentRemainTime (now rendering)
        /// 4: next particles timing
        /// </summary>
        /// <param name="deltaTime"></param>
        public void Evaluate(float deltaTime)
        {
            if (0.0f >= deltaTime)
            {
                return;
            }

            _currentRemainTime += deltaTime;
            if (_currentRemainTime > CubismPhysics.MaxDeltaTime)
            {
                _currentRemainTime = 0.0f;
            }

            var physicsDeltaTime = 0.0f;

            if (Fps > 0.0f)
            {
                physicsDeltaTime = 1.0f / Fps;
            }
            else
            {
                physicsDeltaTime = deltaTime;
            }

            if (_parametersCache == null)
            {
                _parametersCache = new float[Controller.Parameters.Length];
            }

            if (_parametersCache.Length < Controller.Parameters.Length)
            {
                Array.Resize(ref _parametersCache, Controller.Parameters.Length);
            }

            if (_parametersInputCache == null)
            {
                _parametersInputCache = new float[Controller.Parameters.Length];
            }

            if (_parametersInputCache.Length < Controller.Parameters.Length)
            {
                Array.Resize(ref _parametersInputCache, Controller.Parameters.Length);

                for (var i = 0; i < _parametersInputCache.Length; i++)
                {
                    _parametersInputCache[i] = _parametersCache[i];
                }
            }

            while (_currentRemainTime >= physicsDeltaTime)
            {
                var inputWeight = physicsDeltaTime / _currentRemainTime;

                // Calculate the input at the timing to UpdateParticles by linear interpolation with the _parameterInputCache and parameterValue.
                // _parameterCache needs to be separated from _parameterInputCache because of its role in propagating values between groups.
                for (var i = 0; i < Controller.Parameters.Length; i++)
                {
                    _parametersCache[i] = _parametersInputCache[i] * (1.0f - inputWeight) + Controller.Parameters[i].Value * inputWeight;
                    _parametersInputCache[i] = _parametersCache[i];
                }

                for (var i = 0; i < SubRigs.Length; ++i)
                {
                    SubRigs[i].Evaluate(physicsDeltaTime);
                }

                _currentRemainTime -= physicsDeltaTime;
            }

            float alpha = _currentRemainTime / physicsDeltaTime;
            for (var i = 0; i < SubRigs.Length; ++i)
            {
                SubRigs[i].Interpolate(alpha);
            }
        }
    }
}
