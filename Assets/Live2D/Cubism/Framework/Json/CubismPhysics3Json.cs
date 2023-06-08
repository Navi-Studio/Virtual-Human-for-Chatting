/**
 * Copyright(c) Live2D Inc. All rights reserved.
 *
 * Use of this source code is governed by the Live2D Open Software license
 * that can be found at https://www.live2d.com/eula/live2d-open-software-license-agreement_en.html.
 */


using Live2D.Cubism.Framework.Physics;
using System;
using UnityEngine;


namespace Live2D.Cubism.Framework.Json
{
    [Serializable]
    public sealed class CubismPhysics3Json
    {
        /// <summary>
        /// Loads a physics3.json asset.
        /// </summary>
        /// <param name="physics3Json">physics3.json to deserialize.</param>
        /// <returns>Deserialized physics3.json on success; <see langword="null"/> otherwise.</returns>
        public static CubismPhysics3Json LoadFrom(string physics3Json)
        {
            return (string.IsNullOrEmpty(physics3Json))
                ? null
                : JsonUtility.FromJson<CubismPhysics3Json>(physics3Json);
        }

        /// <summary>
        /// Loads a physics3.json asset.
        /// </summary>
        /// <param name="physics3JsonAsset">motion3.json to deserialize.</param>
        /// <returns>Deserialized physics3.json on success; <see langword="null"/> otherwise.</returns>
        public static CubismPhysics3Json LoadFrom(TextAsset physics3JsonAsset)
        {
            return (physics3JsonAsset == null)
                ? null
                : LoadFrom(physics3JsonAsset.text);
        }

        public CubismPhysicsRig ToRig()
        {
            var instance = new CubismPhysicsRig();


            instance.Gravity.x = Meta.EffectiveForces.Gravity.X;
            instance.Gravity.y = Meta.EffectiveForces.Gravity.Y;

            instance.Wind.x = Meta.EffectiveForces.Wind.X;
            instance.Wind.y = Meta.EffectiveForces.Wind.Y;

            instance.Fps = Meta.Fps;

            instance.SubRigs = new CubismPhysicsSubRig[Meta.PhysicsSettingCount];

            var idNameTable = Meta.PhysicsDictionary;

            for (var i = 0; i < instance.SubRigs.Length; ++i)
            {
                instance.SubRigs[i] = new CubismPhysicsSubRig
                {
                    Name          = idNameTable[i].Name,
                    Input         = ReadInput(PhysicsSettings[i].Input),
                    Output        = ReadOutput(PhysicsSettings[i].Output),
                    Particles     = ReadParticles(PhysicsSettings[i].Vertices),
                    Normalization = ReadNormalization(PhysicsSettings[i].Normalization)
                };
            }


            return instance;
        }

        private CubismPhysicsInput[] ReadInput(SerializableInput[] source)
        {
            var dataArray = new CubismPhysicsInput[source.Length];


            for (var i = 0; i < dataArray.Length; ++i)
            {
                dataArray[i] = new CubismPhysicsInput
                {
                    SourceId            = source[i].Source.Id,
                    AngleScale          = 0.0f,
                    ScaleOfTranslation  = Vector2.zero,
                    Weight              = source[i].Weight,
                    SourceComponent     = (CubismPhysicsSourceComponent) Enum.Parse(
                        typeof(CubismPhysicsSourceComponent), source[i].Type
                        ),
                    IsInverted          = source[i].Reflect
                };
            }


            return dataArray;
        }

        private CubismPhysicsOutput[] ReadOutput(SerializableOutput[] source)
        {
            var dataArray = new CubismPhysicsOutput[source.Length];


            for (var i = 0; i < dataArray.Length; ++i)
            {
                dataArray[i] = new CubismPhysicsOutput
                {
                    DestinationId        = source[i].Destination.Id,
                    ParticleIndex        = source[i].VertexIndex,
                    TranslationScale     = Vector2.zero,
                    AngleScale           = source[i].Scale,
                    Weight               = source[i].Weight,
                    SourceComponent      = (CubismPhysicsSourceComponent) Enum.Parse(
                        typeof(CubismPhysicsSourceComponent), source[i].Type
                        ),
                    IsInverted           = source[i].Reflect,
                    ValueBelowMinimum    = 0.0f,
                    ValueExceededMaximum = 0.0f
                };
            }


            return dataArray;
        }

        private CubismPhysicsParticle[] ReadParticles(SerializableVertex[] source)
        {
            var dataArray = new CubismPhysicsParticle[source.Length];


            for (var i = 0; i < dataArray.Length; ++i)
            {
                dataArray[i] = new CubismPhysicsParticle
                {
                    InitialPosition =
                    {
                        x = source[i].Position.X,
                        y = source[i].Position.Y
                    },
                    Mobility          = source[i].Mobility,
                    Delay             = source[i].Delay,
                    Acceleration      = source[i].Acceleration,
                    Radius            = source[i].Radius,
                    Position          = Vector2.zero,
                    LastPosition      = Vector2.zero,
                    LastGravity       = Vector2.down,
                    Force             = Vector2.zero,
                    Velocity          = Vector2.zero
                };
            }


            return dataArray;
        }

        private CubismPhysicsNormalization ReadNormalization(SerializableNormalization source)
        {
            return new CubismPhysicsNormalization
            {
                Position =
                {
                    Maximum = source.Position.Maximum,
                    Minimum = source.Position.Minimum,
                    Default = source.Position.Default
                },

                Angle =
                {
                    Maximum = source.Angle.Maximum,
                    Minimum = source.Angle.Minimum,
                    Default = source.Angle.Default
                }
            };
        }

    #region Json Data

        /// <summary>
        /// Json file format version.
        /// </summary>
        [SerializeField]
        public int Version;

        /// <summary>
        /// Additional data describing physics.
        /// </summary>
        [SerializeField]
        public SerializableMeta Meta;

        /// <summary>
        /// TODO Document.
        /// </summary>
        [SerializeField]
        public SerializablePhysicsSettings[] PhysicsSettings;


        #endregion

        #region Json Helpers

        /// <summary>
        /// 2-component vector.
        /// </summary>
        [Serializable]
        public struct SerializableVector2
        {
            [SerializeField]
            public float X;

            [SerializeField]
            public float Y;
        }


        /// <summary>
        /// TODO Document.
        /// </summary>
        [Serializable]
        public struct SerializableNormalizationValue
        {
            /// <summary>
            /// Minimum of normalization.
            /// </summary>
            [SerializeField]
            public float Minimum;

            /// <summary>
            /// Center of normalization range.
            /// </summary>
            [SerializeField]
            public float Default;

            /// <summary>
            /// Maximum of normalization.
            /// </summary>
            [SerializeField]
            public float Maximum;
        }


        /// <summary>
        /// Target parameter of model.
        /// </summary>
        [Serializable]
        public struct SerializableParameter
        {
            /// <summary>
            /// Target type.
            /// </summary>
            [SerializeField]
            public string Target;

            /// <summary>
            /// Parameter ID.
            /// </summary>
            [SerializeField]
            public string Id;
        }


        /// <summary>
        /// TODO Document.
        /// </summary>
        [Serializable]
        public struct SerializableInput
        {
            /// <summary>
            /// Target parameter.
            /// </summary>
            [SerializeField]
            public SerializableParameter Source;

            /// <summary>
            /// Influence ratio of each kind.
            /// </summary>
            [SerializeField]
            public float Weight;

            /// <summary>
            /// Type of source.
            /// </summary>
            [SerializeField]
            public string Type;

            /// <summary>
            /// TODO Document.
            /// </summary>
            [SerializeField]
            public bool Reflect;
        }


        /// <summary>
        /// TODO Document.
        /// </summary>
        [Serializable]
        public struct SerializableOutput
        {
            /// <summary>
            /// Target parameter.
            /// </summary>
            [SerializeField]
            public SerializableParameter Destination;

            /// <summary>
            /// Index of referenced vertex.
            /// </summary>
            [SerializeField]
            public int VertexIndex;

            /// <summary>
            /// Scale.
            /// </summary>
            [SerializeField]
            public float Scale;

            /// <summary>
            /// Influence ratio of each kind.
            /// </summary>
            [SerializeField]
            public float Weight;

            /// <summary>
            /// Type of destination.
            /// </summary>
            [SerializeField]
            public string Type;

            /// <summary>
            /// TODO Document.
            /// </summary>
            [SerializeField]
            public bool Reflect;
        }


        /// <summary>
        /// Single vertex.
        /// </summary>
        [Serializable]
        public struct SerializableVertex
        {
            /// <summary>
            ///  Default position.
            /// </summary>
            [SerializeField]
            public SerializableVector2 Position;

            /// <summary>
            /// Mobility.
            /// </summary>
            [SerializeField]
            public float Mobility;

            /// <summary>
            /// Delay ratio.
            /// </summary>
            [SerializeField]
            public float Delay;

            /// <summary>
            /// Acceleration.
            /// </summary>
            [SerializeField]
            public float Acceleration;

            /// <summary>
            /// Length.
            /// </summary>
            [SerializeField]
            public float Radius;
        }


        /// <summary>
        /// TODO Document.
        /// </summary>
        [Serializable]
        public struct SerializableNormalization
        {
            /// <summary>
            /// Normalization value of position.
            /// </summary>
            [SerializeField]
            public SerializableNormalizationValue Position;

            /// <summary>
            /// Normalization value of angle.
            /// </summary>
            [SerializeField]
            public SerializableNormalizationValue Angle;
        }


        /// <summary>
        /// Physics Id - Name Table Item.
        /// </summary>
        [Serializable]
        public struct PhysicsDictionaryItem
        {
            /// <summary>
            /// Id for internal management.
            /// </summary>
            [SerializeField]
            public string Id;

            /// <summary>
            /// Physics Setting Name.
            /// </summary>
            [SerializeField]
            public string Name;
        }


        /// <summary>
        /// Setting of physics calculation.
        /// </summary>
        [Serializable]
        public struct SerializablePhysicsSettings
        {
            /// <summary>
            /// Id for internal management.
            /// </summary>
            [SerializeField]
            public string Id;

            /// <summary>
            /// Input array.
            /// </summary>
            [SerializeField]
            public SerializableInput[] Input;

            /// <summary>
            /// Output array.
            /// </summary>
            [SerializeField]
            public SerializableOutput[] Output;

            /// <summary>
            /// Vertices.
            /// </summary>
            [SerializeField]
            public SerializableVertex[] Vertices;

            /// <summary>
            /// Normalization parameter of using input.
            /// </summary>
            [SerializeField]
            public SerializableNormalization Normalization;
        }


        /// <summary>
        /// Additional data describing physics.
        /// </summary>
        [Serializable]
        public struct SerializableMeta
        {
            /// <summary>
            /// Number of physics settings.
            /// </summary>
            [SerializeField]
            public int PhysicsSettingCount;

            /// <summary>
            /// Total number of input parameters.
            /// </summary>
            [SerializeField]
            public int TotalInputCount;

            /// <summary>
            /// Total number of output parameters.
            /// </summary>
            [SerializeField]
            public int TotalOutputCount;

            /// <summary>
            /// Total number of vertices.
            /// </summary>
            [SerializeField]
            public int TotalVertexCount;

            /// <summary>
            /// TODO Document.
            /// </summary>
            [SerializeField]
            public SerializableEffectiveForces EffectiveForces;

            /// <summary>
            /// [Optional] Fps of physics operations.
            /// If the value is not set to Json, it will change according to the application's operating FPS.
            /// </summary>
            [SerializeField]
            public float Fps;

            /// <summary>
            /// Physics Id - Name Table
            /// </summary>
            [SerializeField]
            public PhysicsDictionaryItem[] PhysicsDictionary;
        }


        /// <summary>
        /// TODO Document.
        /// </summary>
        [Serializable]
        public struct SerializableEffectiveForces
        {
            /// <summary>
            /// Gravity.
            /// </summary>
            [SerializeField]
            public SerializableVector2 Gravity;

            /// <summary>
            /// Wind. (Not in use)
            /// </summary>
            [SerializeField]
            public SerializableVector2 Wind;
        }

        #endregion
    }
}
