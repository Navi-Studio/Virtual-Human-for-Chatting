/**
 * Copyright(c) Live2D Inc. All rights reserved.
 *
 * Use of this source code is governed by the Live2D Open Software license
 * that can be found at https://www.live2d.com/eula/live2d-open-software-license-agreement_en.html.
 */


using Live2D.Cubism.Core;
using System;
using UnityEngine;


namespace Live2D.Cubism.Framework.Physics
{
    /// <summary>
    /// Children of rig.
    /// </summary>
    [Serializable]
    public class CubismPhysicsSubRig
    {
        /// <summary>
        /// Name.
        /// </summary>
        [SerializeField]
        public string Name;

        /// <summary>
        /// Input.
        /// </summary>
        [SerializeField]
        public CubismPhysicsInput[] Input;

        /// <summary>
        /// Original Input.
        /// </summary>
        [NonSerialized]
        public CubismPhysicsInput[] OriginalInput;

        /// <summary>
        /// Output.
        /// </summary>
        [SerializeField]
        public CubismPhysicsOutput[] Output;

        /// <summary>
        /// Original Output.
        /// </summary>
        [NonSerialized]
        public CubismPhysicsOutput[] OriginalOutput;

        /// <summary>
        /// Particles.
        /// </summary>
        [SerializeField]
        public CubismPhysicsParticle[] Particles;

        /// <summary>
        /// Normalization.
        /// </summary>
        [SerializeField]
        public CubismPhysicsNormalization Normalization;

        /// <summary>
        /// Rig.
        /// </summary>
        public CubismPhysicsRig Rig
        {
            get { return _rig; }
            set { _rig = value; }
        }

        [NonSerialized]
        private CubismPhysicsRig _rig;


        /// <summary>
        /// Output result of physics operations before applying to parameters.
        /// </summary>
        private struct SubRigPhysicsOutput
        {
            public float[] Output;
        }

        [NonSerialized]
        private SubRigPhysicsOutput _currentRigOutput; // Results of the latest pendulum calculation.

        [NonSerialized]
        private SubRigPhysicsOutput _previousRigOutput; // Result of previous pendulum calculation.

        /// <summary>
        /// Applies the specified weights from the latest and one previous result of the pendulum operation.
        /// </summary>
        /// <param name="weight">Weight of latest results.</param>
        public void Interpolate(float weight)
        {
            // Load input parameters.
            for (int i = 0; i < Output.Length; ++i)
            {
                if (Output[i].Destination == null)
                {
                    var destination = Rig.Controller.Parameters.FindById(Output[i].DestinationId);
                    if (destination == null)
                    {
                        continue;
                    }

                    Output[i].Destination = destination;
                }

                UpdateOutputParameterValue(
                    Output[i].Destination,
                    ref Output[i].Destination.Value,
                    _previousRigOutput.Output[i] * (1 - weight) + _currentRigOutput.Output[i] * weight,
                    Output[i]
                );
            }
        }

        /// <summary>
        /// Updates parameter from output value.
        /// </summary>
        /// <param name="parameter">Target parameter.</param>
        /// <param name="parameterValue">Target parameter Value.</param>
        /// <param name="translation">Translation.</param>
        /// <param name="output">Output value.</param>
        private void UpdateOutputParameterValue(CubismParameter parameter, ref float parameterValue, float translation, CubismPhysicsOutput output)
        {
            var outputScale = 1.0f;

            outputScale = output.GetScale();

            var value = translation * outputScale;


            if (value < parameter.MinimumValue)
            {
                if (value < output.ValueBelowMinimum)
                {
                    output.ValueBelowMinimum = value;
                }


                value = parameter.MinimumValue;
            }
            else if (value > parameter.MaximumValue)
            {
                if (value > output.ValueExceededMaximum)
                {
                    output.ValueExceededMaximum = value;
                }


                value = parameter.MaximumValue;
            }


            var weight = (output.Weight / CubismPhysics.MaximumWeight);

            if (weight >= 1.0f)
            {
                parameterValue = value;
            }
            else
            {
                value = (parameterValue * (1.0f - weight)) + (value * weight);
                parameterValue = value;
            }
        }


        /// <summary>
        /// Updates particles in every frame.
        /// </summary>
        /// <param name="strand">Particles.</param>
        /// <param name="totalTranslation">Total translation.</param>
        /// <param name="totalAngle">Total angle.</param>
        /// <param name="wind">Direction of wind.</param>
        /// <param name="thresholdValue">Value of threshold.</param>
        /// <param name="deltaTime">Time of delta.</param>
        private void UpdateParticles(
            CubismPhysicsParticle[] strand,
            Vector2 totalTranslation,
            float totalAngle,
            Vector2 wind,
            float thresholdValue,
            float deltaTime
            )
        {
            strand[0].Position = totalTranslation;

            var totalRadian = CubismPhysicsMath.DegreesToRadian(totalAngle);
            var currentGravity = CubismPhysicsMath.RadianToDirection(totalRadian);
            currentGravity.Normalize();

            for (var i = 1; i < strand.Length; ++i)
            {
                strand[i].Force = (currentGravity * strand[i].Acceleration) + wind;

                strand[i].LastPosition = strand[i].Position;

                // The Cubism Editor expects 30 FPS so we scale here by 30...
                var delay = strand[i].Delay * deltaTime * 30.0f;

                var direction = strand[i].Position - strand[i - 1].Position;
                var radian = CubismPhysicsMath.DirectionToRadian(strand[i].LastGravity, currentGravity) / CubismPhysics.AirResistance;


                direction.x = ((Mathf.Cos(radian) * direction.x) - (direction.y * Mathf.Sin(radian)));
                direction.y = ((Mathf.Sin(radian) * direction.x) + (direction.y * Mathf.Cos(radian)));


                strand[i].Position = strand[i - 1].Position + direction;


                var velocity = strand[i].Velocity * delay;
                var force = strand[i].Force * delay * delay;


                strand[i].Position = strand[i].Position + velocity + force;


                var newDirection = strand[i].Position - strand[i - 1].Position;

                newDirection.Normalize();


                strand[i].Position = strand[i - 1].Position + newDirection * strand[i].Radius;

                if (Mathf.Abs(strand[i].Position.x) < thresholdValue)
                {
                    strand[i].Position.x = 0.0f;
                }


                if (delay != 0.0f)
                {
                    strand[i].Velocity =
                            ((strand[i].Position - strand[i].LastPosition) / delay) * strand[i].Mobility;
                }


                strand[i].Force = Vector2.zero;
                strand[i].LastGravity = currentGravity;
            }
        }

        /// <summary>
        /// Updates particles in stabilization function.
        /// </summary>
        /// <param name="strand">Particles</param>
        /// <param name="totalTranslation">Total translation.</param>
        /// <param name="totalAngle">Total angle.</param>
        /// <param name="wind">Direction of wind.</param>
        /// <param name="thresholdValue">Value of threshold.</param>
        private void UpdateParticlesForStabilization(
            CubismPhysicsParticle[] strand,
            Vector2 totalTranslation,
            float totalAngle,
            Vector2 wind,
            float thresholdValue
            )
        {
            strand[0].Position = totalTranslation;

            var totalRadian = CubismPhysicsMath.DegreesToRadian(totalAngle);
            var currentGravity = CubismPhysicsMath.RadianToDirection(totalRadian);
            currentGravity.Normalize();

            for (var i = 1; i < strand.Length; ++i)
            {
                strand[i].Force = (currentGravity * strand[i].Acceleration) + wind;

                strand[i].LastPosition = strand[i].Position;

                strand[i].Velocity = Vector2.zero;
                var force = strand[i].Force;
                force.Normalize();

                strand[i].Position = strand[i - 1].Position + force * strand[i].Radius;

                if (Mathf.Abs(strand[i].Position.x) < thresholdValue)
                {
                    strand[i].Position.x = 0.0f;
                }

                strand[i].Force = Vector2.zero;
                strand[i].LastGravity = currentGravity;
            }
        }

        /// <summary>
        /// Initializes <see langword="this"/>.
        /// </summary>
        public void Initialize()
        {
            var strand = Particles;

            // Initialize the top of particle.
            strand[0].InitialPosition = Vector2.zero;
            strand[0].LastPosition = strand[0].InitialPosition;
            strand[0].LastGravity = Rig.Gravity;
            strand[0].LastGravity.y *= -1.0f;


            // Initialize particles.
            for (var i = 1; i < strand.Length; ++i)
            {
                var radius = Vector2.zero;
                radius.y = strand[i].Radius;
                strand[i].InitialPosition = strand[i - 1].InitialPosition + radius;
                strand[i].Position = strand[i].InitialPosition;
                strand[i].LastPosition = strand[i].InitialPosition;
                strand[i].LastGravity = Rig.Gravity;
                strand[i].LastGravity.y *= -1.0f;
            }


            // Initialize inputs.
            OriginalInput = new CubismPhysicsInput[Input.Length];
            for (var i = 0; i < Input.Length; ++i)
            {
                OriginalInput[i] = Input[i];
                Input[i].InitializeGetter();
            }

            _previousRigOutput = new SubRigPhysicsOutput();
            _currentRigOutput = new SubRigPhysicsOutput();

            Array.Resize(ref _previousRigOutput.Output, Output.Length);
            Array.Resize(ref _currentRigOutput.Output, Output.Length);

            // Initialize outputs.
            OriginalOutput = new CubismPhysicsOutput[Output.Length];
            for (var i = 0; i < Output.Length; ++i)
            {
                OriginalOutput[i] = Output[i];
                Output[i].InitializeGetter();
            }
        }


        /// <summary>
        /// Evaluate rig in every frame.
        /// </summary>
        /// <param name="deltaTime"></param>
        public void Evaluate(float deltaTime)
        {
            var totalAngle = 0.0f;
            var totalTranslation = Vector2.zero;

            for (var i = 0; i < Input.Length; ++i)
            {
                var weight = Input[i].Weight / CubismPhysics.MaximumWeight;

                if (Input[i].Source == null)
                {
                    Input[i].Source = Rig.Controller.Parameters.FindById(Input[i].SourceId);
                }
                var index = Array.IndexOf(Rig.Controller.Parameters, Input[i].Source);

                var parameter = Input[i].Source;
                Input[i].GetNormalizedParameterValue(
                    ref totalTranslation,
                    ref totalAngle,
                    parameter,
                    ref Rig.ParametersCache[index],
                    Normalization,
                    weight
                    );
            }


            var radAngle = CubismPhysicsMath.DegreesToRadian(-totalAngle);


            totalTranslation.x = (totalTranslation.x * Mathf.Cos(radAngle) - totalTranslation.y * Mathf.Sin(radAngle));
            totalTranslation.y = (totalTranslation.x * Mathf.Sin(radAngle) + totalTranslation.y * Mathf.Cos(radAngle));


            UpdateParticles(
                Particles,
                totalTranslation,
                totalAngle,
                Rig.Wind,
                CubismPhysics.MovementThreshold * Normalization.Position.Maximum,
                deltaTime
                );


            for (var i = 0; i < Output.Length; ++i)
            {
                _previousRigOutput.Output[i] = _currentRigOutput.Output[i];

                if (Output[i].Destination == null)
                {
                    var destination = Rig.Controller.Parameters.FindById(Output[i].DestinationId);
                    if (destination == null)
                    {
                        continue;
                    }

                    Output[i].Destination = destination;
                }

                var particleIndex = Output[i].ParticleIndex;

                if (particleIndex < 1 || particleIndex >= Particles.Length)
                {
                    continue;
                }

                var index = Array.IndexOf(Rig.Controller.Parameters, Output[i].Destination);

                var translation = Particles[particleIndex].Position -
                                        Particles[particleIndex - 1].Position;

                var parameter = Output[i].Destination;
                var outputValue = Output[i].GetValue(
                    translation,
                    Particles,
                    particleIndex,
                    Rig.Gravity
                    );

                _currentRigOutput.Output[i] = outputValue;

                UpdateOutputParameterValue(parameter, ref Rig.ParametersCache[index], outputValue, Output[i]);
            }
        }

        /// <summary>
        /// Calculate the state in which the physics operation stabilizes at the current parameter values.
        /// </summary>
        public void Stabilization()
        {
            var totalAngle = 0.0f;
            var totalTranslation = Vector2.zero;

            for (var i = 0; i < Input.Length; ++i)
            {
                var weight = Input[i].Weight / CubismPhysics.MaximumWeight;

                if (Input[i].Source == null)
                {
                    Input[i].Source = Rig.Controller.Parameters.FindById(Input[i].SourceId);
                }
                var index = Array.IndexOf(Rig.Controller.Parameters, Input[i].Source);

                var parameter = Input[i].Source;
                Input[i].GetNormalizedParameterValue(
                    ref totalTranslation,
                    ref totalAngle,
                    parameter,
                    ref Input[i].Source.Value,
                    Normalization,
                    weight
                    );
                Rig.ParametersCache[index] = Input[i].Source.Value;
            }


            var radAngle = CubismPhysicsMath.DegreesToRadian(-totalAngle);


            totalTranslation.x = (totalTranslation.x * Mathf.Cos(radAngle) - totalTranslation.y * Mathf.Sin(radAngle));
            totalTranslation.y = (totalTranslation.x * Mathf.Sin(radAngle) + totalTranslation.y * Mathf.Cos(radAngle));


            UpdateParticlesForStabilization(
                Particles,
                totalTranslation,
                totalAngle,
                Rig.Wind,
                CubismPhysics.MovementThreshold * Normalization.Position.Maximum
                );


            for (var i = 0; i < Output.Length; ++i)
            {
                _previousRigOutput.Output[i] = _currentRigOutput.Output[i];

                if (Output[i].Destination == null)
                {
                    var destination = Rig.Controller.Parameters.FindById(Output[i].DestinationId);
                    if (destination == null)
                    {
                        continue;
                    }

                    Output[i].Destination = destination;
                }

                var particleIndex = Output[i].ParticleIndex;

                if (particleIndex < 1 || particleIndex >= Particles.Length)
                {
                    continue;
                }

                var index = Array.IndexOf(Rig.Controller.Parameters, Output[i].Destination);

                var translation = Particles[particleIndex].Position -
                                        Particles[particleIndex - 1].Position;

                var parameter = Output[i].Destination;
                var outputValue = Output[i].GetValue(
                    translation,
                    Particles,
                    particleIndex,
                    Rig.Gravity
                    );

                _currentRigOutput.Output[i] = outputValue;
                _previousRigOutput.Output[i] = outputValue;
                UpdateOutputParameterValue(parameter, ref Output[i].Destination.Value, outputValue, Output[i]);

                Rig.ParametersCache[index] = Output[i].Destination.Value;
            }
        }
    }
}
