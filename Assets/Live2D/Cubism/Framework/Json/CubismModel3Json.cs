/**
 * Copyright(c) Live2D Inc. All rights reserved.
 *
 * Use of this source code is governed by the Live2D Open Software license
 * that can be found at https://www.live2d.com/eula/live2d-open-software-license-agreement_en.html.
 */


using Live2D.Cubism.Core;
using System;
using System.IO;
using Live2D.Cubism.Framework.MouthMovement;
using Live2D.Cubism.Framework.Physics;
using Live2D.Cubism.Framework.UserData;
using Live2D.Cubism.Framework.Pose;
using Live2D.Cubism.Framework.Expression;
using Live2D.Cubism.Framework.MotionFade;
using Live2D.Cubism.Framework.Raycasting;
using Live2D.Cubism.Rendering;
using Live2D.Cubism.Rendering.Masking;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;


namespace Live2D.Cubism.Framework.Json
{
    /// <summary>
    /// Exposes moc3.json asset data.
    /// </summary>
    [Serializable]
    // ReSharper disable once ClassCannotBeInstantiated
    public sealed class CubismModel3Json
    {
        #region Delegates

        /// <summary>
        /// Handles the loading of assets.
        /// </summary>
        /// <param name="assetType">The asset type to load.</param>
        /// <param name="assetPath">The path to the asset.</param>
        /// <returns></returns>
        public delegate object LoadAssetAtPathHandler(Type assetType, string assetPath);


        /// <summary>
        /// Picks a <see cref="Material"/> for a <see cref="CubismDrawable"/>.
        /// </summary>
        /// <param name="sender">Event source.</param>
        /// <param name="drawable">Drawable to pick for.</param>
        /// <returns>Picked material.</returns>
        public delegate Material MaterialPicker(CubismModel3Json sender, CubismDrawable drawable);

        /// <summary>
        /// Picks a <see cref="Texture2D"/> for a <see cref="CubismDrawable"/>.
        /// </summary>
        /// <param name="sender">Event source.</param>
        /// <param name="drawable">Drawable to pick for.</param>
        /// <returns>Picked texture.</returns>
        public delegate Texture2D TexturePicker(CubismModel3Json sender, CubismDrawable drawable);

        #endregion

        #region Load Methods

        /// <summary>
        /// Loads a model.json asset.
        /// </summary>
        /// <param name="assetPath">The path to the asset.</param>
        /// <returns>The <see cref="CubismModel3Json"/> on success; <see langword="null"/> otherwise.</returns>
        public static CubismModel3Json LoadAtPath(string assetPath)
        {
            // Use default asset load handler.
            return LoadAtPath(assetPath, BuiltinLoadAssetAtPath);
        }

        /// <summary>
        /// Loads a model.json asset.
        /// </summary>
        /// <param name="assetPath">The path to the asset.</param>
        /// <param name="loadAssetAtPath">Handler for loading assets.</param>
        /// <returns>The <see cref="CubismModel3Json"/> on success; <see langword="null"/> otherwise.</returns>
        public static CubismModel3Json LoadAtPath(string assetPath, LoadAssetAtPathHandler loadAssetAtPath)
        {
            // Load Json asset.
            var modelJsonAsset = loadAssetAtPath(typeof(string), assetPath) as string;

            // Return early in case Json asset wasn't loaded.
            if (modelJsonAsset == null)
            {
                return null;
            }


            // Deserialize Json.
            var modelJson = JsonUtility.FromJson<CubismModel3Json>(modelJsonAsset);


            // Finalize deserialization.
            modelJson.AssetPath = assetPath;
            modelJson.LoadAssetAtPath = loadAssetAtPath;


            // Set motion references.
            var value = CubismJsonParser.ParseFromString(modelJsonAsset);

            // Return early if there is no references.
            if (!value.Get("FileReferences").GetMap(null).ContainsKey("Motions"))
            {
                return modelJson;
            }


            var motionGroupNames = value.Get("FileReferences").Get("Motions").KeySet().ToArray();
            modelJson.FileReferences.Motions.GroupNames = motionGroupNames;

            var motionGroupNamesCount = motionGroupNames.Length;
            modelJson.FileReferences.Motions.Motions = new SerializableMotion[motionGroupNamesCount][];

            for (var i = 0; i < motionGroupNamesCount; i++)
            {
                var motionGroup = value.Get("FileReferences").Get("Motions").Get(motionGroupNames[i]);
                var motionCount = motionGroup.GetVector(null).ToArray().Length;

                modelJson.FileReferences.Motions.Motions[i] = new SerializableMotion[motionCount];


                for (var j = 0; j < motionCount; j++)
                {
                    if (motionGroup.Get(j).GetMap(null).ContainsKey("File"))
                    {
                        modelJson.FileReferences.Motions.Motions[i][j].File = motionGroup.Get(j).Get("File").toString();
                    }

                    if (motionGroup.Get(j).GetMap(null).ContainsKey("Sound"))
                    {
                        modelJson.FileReferences.Motions.Motions[i][j].Sound = motionGroup.Get(j).Get("Sound").toString();
                    }

                    if (motionGroup.Get(j).GetMap(null).ContainsKey("FadeInTime"))
                    {
                        modelJson.FileReferences.Motions.Motions[i][j].FadeInTime = motionGroup.Get(j).Get("FadeInTime").ToFloat();
                    }

                    if (motionGroup.Get(j).GetMap(null).ContainsKey("FadeOutTime"))
                    {
                        modelJson.FileReferences.Motions.Motions[i][j].FadeOutTime = motionGroup.Get(j).Get("FadeOutTime").ToFloat();
                    }
                }
            }


            return modelJson;
        }

        #endregion

        /// <summary>
        /// Path to <see langword="this"/>.
        /// </summary>
        public string AssetPath { get; private set; }


        /// <summary>
        /// Method for loading assets.
        /// </summary>
        private LoadAssetAtPathHandler LoadAssetAtPath { get; set; }

        #region Json Data

        /// <summary>
        /// The motion3.json format version.
        /// </summary>
        [SerializeField]
        public int Version;

        /// <summary>
        /// The file references.
        /// </summary>
        [SerializeField]
        public SerializableFileReferences FileReferences;

        /// <summary>
        /// Groups.
        /// </summary>
        [SerializeField]
        public SerializableGroup[] Groups;

        /// <summary>
        /// Hit areas.
        /// </summary>
        [SerializeField]
        public SerializableHitArea[] HitAreas;

        #endregion

        /// <summary>
        /// The contents of the referenced moc3 asset.
        /// </summary>
        /// <remarks>
        /// The contents isn't cached internally.
        /// </remarks>
        public byte[] Moc3
        {
            get
            {
                return LoadReferencedAsset<byte[]>(FileReferences.Moc);
            }
        }

        /// <summary>
        /// <see cref="CubismPose3Json"/> backing field.
        /// </summary>
        [NonSerialized]
        private CubismPose3Json _pose3Json;

        /// <summary>
        /// The contents of pose3.json asset.
        /// </summary>
        public CubismPose3Json Pose3Json
        {
            get
            {
                if(_pose3Json != null)
                {
                    return _pose3Json;
                }

                var jsonString = string.IsNullOrEmpty(FileReferences.Pose) ? null : LoadReferencedAsset<String>(FileReferences.Pose);
                _pose3Json = CubismPose3Json.LoadFrom(jsonString);
                return _pose3Json;
            }
        }

        /// <summary>
        /// <see cref="Expression3Jsons"/> backing field.
        /// </summary>
        [NonSerialized]
        private CubismExp3Json[] _expression3Jsons;

        /// <summary>
        /// The referenced expression assets.
        /// </summary>
        /// <remarks>
        /// The references aren't cached internally.
        /// </remarks>
        public CubismExp3Json[] Expression3Jsons
        {
            get
            {
                // Fail silently...
                if(FileReferences.Expressions == null)
                {
                    return null;
                }

                // Load expression only if necessary.
                if (_expression3Jsons == null)
                {
                    _expression3Jsons = new CubismExp3Json[FileReferences.Expressions.Length];

                    for (var i = 0; i < _expression3Jsons.Length; ++i)
                    {
                        var expressionJson = (string.IsNullOrEmpty(FileReferences.Expressions[i].File))
                                                ? null
                                                : LoadReferencedAsset<string>(FileReferences.Expressions[i].File);
                        _expression3Jsons[i] = CubismExp3Json.LoadFrom(expressionJson);
                    }
                }

                return _expression3Jsons;
            }
        }

        /// <summary>
        /// The contents of physics3.json asset.
        /// </summary>
        public string Physics3Json
        {
            get
            {
                return string.IsNullOrEmpty(FileReferences.Physics) ? null : LoadReferencedAsset<string>(FileReferences.Physics);
            }
        }

        public string UserData3Json
        {
            get
            {
                return string.IsNullOrEmpty(FileReferences.UserData) ? null : LoadReferencedAsset<string>(FileReferences.UserData);
            }
        }

        /// <summary>
        /// The contents of cdi3.json asset.
        /// </summary>
        public string DisplayInfo3Json
        {
            get
            {
                return string.IsNullOrEmpty(FileReferences.DisplayInfo) ? null : LoadReferencedAsset<string>(FileReferences.DisplayInfo);
            }
        }

        /// <summary>
        /// <see cref="Textures"/> backing field.
        /// </summary>
        [NonSerialized]
        private Texture2D[] _textures;

        /// <summary>
        /// The referenced texture assets.
        /// </summary>
        /// <remarks>
        /// The references aren't cached internally.
        /// </remarks>
        public Texture2D[] Textures
        {
            get
            {
                // Load textures only if necessary.
                if (_textures == null)
                {
                    _textures = new Texture2D[FileReferences.Textures.Length];


                    for (var i = 0; i < _textures.Length; ++i)
                    {
                        _textures[i] = LoadReferencedAsset<Texture2D>(FileReferences.Textures[i]);
                    }
                }


                return _textures;
            }
        }

        #region Constructors

        /// <summary>
        /// Makes construction only possible through factories.
        /// </summary>
        private CubismModel3Json()
        {
        }

        #endregion

        /// <summary>
        /// Instantiates a <see cref="CubismMoc">model source</see> and a <see cref="CubismModel">model</see> with the default texture set.
        /// </summary>
        /// <param name="shouldImportAsOriginalWorkflow">Should import as original workflow.</param>
        /// <returns>The instantiated <see cref="CubismModel">model</see> on success; <see langword="null"/> otherwise.</returns>
        public CubismModel ToModel(bool shouldImportAsOriginalWorkflow = false)
        {
            return ToModel(CubismBuiltinPickers.MaterialPicker, CubismBuiltinPickers.TexturePicker, shouldImportAsOriginalWorkflow);
        }

        /// <summary>
        /// Instantiates a <see cref="CubismMoc">model source</see> and a <see cref="CubismModel">model</see>.
        /// </summary>
        /// <param name="pickMaterial">The material mapper to use.</param>
        /// <param name="pickTexture">The texture mapper to use.</param>
        /// <param name="shouldImportAsOriginalWorkflow">Should import as original workflow.</param>
        /// <returns>The instantiated <see cref="CubismModel">model</see> on success; <see langword="null"/> otherwise.</returns>
        public CubismModel ToModel(MaterialPicker pickMaterial, TexturePicker pickTexture, bool shouldImportAsOriginalWorkflow = false)
        {
            // Initialize model source and instantiate it.
            var mocAsBytes = Moc3;


            if (mocAsBytes == null)
            {
                return null;
            }


            var moc = CubismMoc.CreateFrom(mocAsBytes);


            var model = CubismModel.InstantiateFrom(moc);

            if (model == null)
            {
                return null;
            }

            model.name = Path.GetFileNameWithoutExtension(FileReferences.Moc);


#if UNITY_EDITOR
            // Add parameters and parts inspectors.
            model.gameObject.AddComponent<CubismParametersInspector>();
            model.gameObject.AddComponent<CubismPartsInspector>();
#endif

            // Create renderers.
            var rendererController = model.gameObject.AddComponent<CubismRenderController>();
            var renderers = rendererController.Renderers;

            var drawables = model.Drawables;

            if (renderers == null || drawables  == null)
            {
                return null;
            }

            // Initialize materials.
            for (var i = 0; i < renderers.Length; ++i)
            {
                renderers[i].Material = pickMaterial(this, drawables[i]);
            }


            // Initialize textures.
            for (var i = 0; i < renderers.Length; ++i)
            {
                renderers[i].MainTexture = pickTexture(this, drawables[i]);
            }


            // Initialize drawables.
            if(HitAreas != null)
            {
                for (var i = 0; i < HitAreas.Length; i++)
                {
                    for (var j = 0; j < drawables.Length; j++)
                    {
                        if (drawables[j].Id == HitAreas[i].Id)
                        {
                            // Add components for hit judgement to HitArea target Drawables.
                            var hitDrawable = drawables[j].gameObject.AddComponent<CubismHitDrawable>();
                            hitDrawable.Name = HitAreas[i].Name;

                            drawables[j].gameObject.AddComponent<CubismRaycastable>();
                            break;
                        }
                    }
                }
            }

            //Load from cdi3.json
            var DisplayInfo3JsonAsString = DisplayInfo3Json;
            var cdi3Json = CubismDisplayInfo3Json.LoadFrom(DisplayInfo3JsonAsString);

            // Initialize groups.
            var parameters = model.Parameters;


            for (var i = 0; i < parameters.Length; ++i)
            {
                if (IsParameterInGroup(parameters[i], "EyeBlink"))
                {
                    if (model.gameObject.GetComponent<CubismEyeBlinkController>() == null)
                    {
                        model.gameObject.AddComponent<CubismEyeBlinkController>();
                    }


                    parameters[i].gameObject.AddComponent<CubismEyeBlinkParameter>();
                }


                // Set up mouth parameters.
                if (IsParameterInGroup(parameters[i], "LipSync"))
                {
                    if (model.gameObject.GetComponent<CubismMouthController>() == null)
                    {
                        model.gameObject.AddComponent<CubismMouthController>();
                    }


                    parameters[i].gameObject.AddComponent<CubismMouthParameter>();
                }


                // Setting up the parameter name for display.
                if (cdi3Json != null)
                {
                    var cubismDisplayInfoParameterName = parameters[i].gameObject.AddComponent<CubismDisplayInfoParameterName>();
                    cubismDisplayInfoParameterName.Name = cdi3Json.Parameters[i].Name;
                    cubismDisplayInfoParameterName.DisplayName = string.Empty;
                }

            }

            // Setting up the part name for display.
            if (cdi3Json != null)
            {
                // Initialize groups.
                var parts = model.Parts;

                for (var i = 0; i < parts.Length; i++)
                {
                    var cubismDisplayInfoPartNames = parts[i].gameObject.AddComponent<CubismDisplayInfoPartName>();
                    cubismDisplayInfoPartNames.Name = cdi3Json.Parts[i].Name;
                    cubismDisplayInfoPartNames.DisplayName = string.Empty;
                }
            }

            // Add mask controller if required.
            for (var i = 0; i < drawables.Length; ++i)
            {
                if (!drawables[i].IsMasked)
                {
                    continue;
                }


                // Add controller exactly once...
                model.gameObject.AddComponent<CubismMaskController>();


                break;
            }

            // Add original workflow component if is original workflow.
            if(shouldImportAsOriginalWorkflow)
            {
                // Add cubism update manager.
                var updateManager = model.gameObject.GetComponent<CubismUpdateController>();

                if(updateManager == null)
                {
                    model.gameObject.AddComponent<CubismUpdateController>();
                }

                // Add parameter store.
                var parameterStore = model.gameObject.GetComponent<CubismParameterStore>();

                if(parameterStore == null)
                {
                    parameterStore = model.gameObject.AddComponent<CubismParameterStore>();
                }

                // Add pose controller.
                var poseController = model.gameObject.GetComponent<CubismPoseController>();

                if(poseController == null)
                {
                    poseController = model.gameObject.AddComponent<CubismPoseController>();
                }

                // Add expression controller.
                var expressionController = model.gameObject.GetComponent<CubismExpressionController>();

                if(expressionController == null)
                {
                    expressionController = model.gameObject.AddComponent<CubismExpressionController>();
                }


                // Add fade controller.
                var motionFadeController = model.gameObject.GetComponent<CubismFadeController>();

                if(motionFadeController == null)
                {
                    motionFadeController = model.gameObject.AddComponent<CubismFadeController>();
                }

            }


            // Initialize physics if JSON exists.
            var physics3JsonAsString = Physics3Json;


            if (!string.IsNullOrEmpty(physics3JsonAsString))
            {
                var physics3Json = CubismPhysics3Json.LoadFrom(physics3JsonAsString);
                var physicsController = model.gameObject.GetComponent<CubismPhysicsController>();

                if (physicsController == null)
                {
                    physicsController = model.gameObject.AddComponent<CubismPhysicsController>();

                }

                physicsController.Initialize(physics3Json.ToRig());
            }


            var userData3JsonAsString = UserData3Json;


            if (!string.IsNullOrEmpty(userData3JsonAsString))
            {
                var userData3Json = CubismUserData3Json.LoadFrom(userData3JsonAsString);


                var drawableBodies = userData3Json.ToBodyArray(CubismUserDataTargetType.ArtMesh);

                for (var i = 0; i < drawables.Length; ++i)
                {
                    var index = GetBodyIndexById(drawableBodies, drawables[i].Id);

                    if (index >= 0)
                    {
                        var tag = drawables[i].gameObject.GetComponent<CubismUserDataTag>();


                        if (tag == null)
                        {
                            tag = drawables[i].gameObject.AddComponent<CubismUserDataTag>();
                        }


                        tag.Initialize(drawableBodies[index]);
                    }
                }
            }

            if (model.gameObject.GetComponent<Animator>() == null)
            {
                model.gameObject.AddComponent<Animator>();
            }

            // Make sure model is 'fresh'
            model.ForceUpdateNow();


            return model;
        }

        #region Helper Methods

        /// <summary>
        /// Type-safely loads an asset.
        /// </summary>
        /// <typeparam name="T">Asset type.</typeparam>
        /// <param name="referencedFile">Path to asset.</param>
        /// <returns>The asset on success; <see langword="null"/> otherwise.</returns>
        private T LoadReferencedAsset<T>(string referencedFile) where T : class
        {
            var assetPath = Path.GetDirectoryName(AssetPath) + "/" + referencedFile;


            return LoadAssetAtPath(typeof(T), assetPath) as T;
        }


        /// <summary>
        /// Builtin method for loading assets.
        /// </summary>
        /// <param name="assetType">Asset type.</param>
        /// <param name="assetPath">Path to asset.</param>
        /// <returns>The asset on success; <see langword="null"/> otherwise.</returns>
        private static object BuiltinLoadAssetAtPath(Type assetType, string assetPath)
        {
            // Explicitly deal with byte arrays.
            if (assetType == typeof(byte[]))
            {
#if UNITY_EDITOR
                return File.ReadAllBytes(assetPath);
#else
                var textAsset = Resources.Load(assetPath, typeof(TextAsset)) as TextAsset;


                return (textAsset != null)
                    ? textAsset.bytes
                    : null;
#endif
            }
            else if (assetType == typeof(string))
            {
#if UNITY_EDITOR
                return File.ReadAllText(assetPath);
#else
                var textAsset = Resources.Load(assetPath, typeof(TextAsset)) as TextAsset;


                return (textAsset != null)
                    ? textAsset.text
                    : null;
#endif
            }


#if UNITY_EDITOR
            return AssetDatabase.LoadAssetAtPath(assetPath, assetType);
#else
            return Resources.Load(assetPath, assetType);
#endif
        }


        /// <summary>
        /// Checks whether the parameter is an eye blink parameter.
        /// </summary>
        /// <param name="parameter">Parameter to check.</param>
        /// <param name="groupName">Name of group to query for.</param>
        /// <returns><see langword="true"/> if parameter is an eye blink parameter; <see langword="false"/> otherwise.</returns>
        private bool IsParameterInGroup(CubismParameter parameter, string groupName)
        {
            // Return early if groups aren't available...
            if (Groups == null || Groups.Length == 0)
            {
                return false;
            }


            for (var i = 0; i < Groups.Length; ++i)
            {
                if (Groups[i].Name != groupName)
                {
                    continue;
                }

                if(Groups[i].Ids != null)
                {
                    for (var j = 0; j < Groups[i].Ids.Length; ++j)
                    {
                        if (Groups[i].Ids[j] == parameter.name)
                        {
                            return true;
                        }
                    }
                }
            }


            return false;
        }


        /// <summary>
        /// Get body index from body array by Id.
        /// </summary>
        /// <param name="bodies">Target body array.</param>
        /// <param name="id">Id for find.</param>
        /// <returns>Array index if Id found; -1 otherwise.</returns>
        private int GetBodyIndexById(CubismUserDataBody[] bodies, string id)
        {
            for (var i = 0; i < bodies.Length; ++i)
            {
                if (bodies[i].Id == id)
                {
                    return i;
                }
            }

            return -1;
        }


        #endregion

        #region Json Helpers

        /// <summary>
        /// File references data.
        /// </summary>
        [Serializable]
        public struct SerializableFileReferences
        {
            /// <summary>
            /// Relative path to the moc3 asset.
            /// </summary>
            [SerializeField]
            public string Moc;

            /// <summary>
            /// Relative paths to texture assets.
            /// </summary>
            [SerializeField]
            public string[] Textures;

            /// <summary>
            /// Relative path to the pose3.json.
            /// </summary>
            [SerializeField]
            public string Pose;

            /// <summary>
            /// Relative path to the expression asset.
            /// </summary>
            [SerializeField]
            public SerializableExpression[] Expressions;

            /// <summary>
            /// Relative path to the pose motion3.json.
            /// </summary>
            [SerializeField]
            public SerializableMotions Motions;

            /// <summary>
            /// Relative path to the physics asset.
            /// </summary>
            [SerializeField]
            public string Physics;

            /// <summary>
            /// Relative path to the user data asset.
            /// </summary>
            [SerializeField]
            public string UserData;

            /// <summary>
            /// Relative path to the cdi3.json.
            /// </summary>
            [SerializeField]
            public string DisplayInfo;
        }

        /// <summary>
        /// Group data.
        /// </summary>
        [Serializable]
        public struct SerializableGroup
        {
            /// <summary>
            /// Target type.
            /// </summary>
            [SerializeField]
            public string Target;

            /// <summary>
            /// Group name.
            /// </summary>
            [SerializeField]
            public string Name;

            /// <summary>
            /// Referenced IDs.
            /// </summary>
            [SerializeField]
            public string[] Ids;
        }

        /// <summary>
        /// Expression data.
        /// </summary>
        [Serializable]
        public struct SerializableExpression
        {
            /// <summary>
            /// Expression Name.
            /// </summary>
            [SerializeField]
            public string Name;

            /// <summary>
            /// Expression File.
            /// </summary>
            [SerializeField]
            public string File;

            /// <summary>
            /// Expression FadeInTime.
            /// </summary>
            [SerializeField]
            public float FadeInTime;

            /// <summary>
            /// Expression FadeOutTime.
            /// </summary>
            [SerializeField]
            public float FadeOutTime;
        }

        /// <summary>
        /// Motion data.
        /// </summary>
        [Serializable]
        public struct SerializableMotions
        {
            /// <summary>
            /// Motion group names.
            /// </summary>
            [SerializeField]
            public string[] GroupNames;

            /// <summary>
            /// Motion groups.
            /// </summary>
            [SerializeField]
            public SerializableMotion[][] Motions;
        }

        /// <summary>
        /// Motion data.
        /// </summary>
        [Serializable]
        public struct SerializableMotion
        {
            /// <summary>
            /// File path.
            /// </summary>
            [SerializeField]
            public string File;

            /// <summary>
            /// Sound path.
            /// </summary>
            [SerializeField]
            public string Sound;

            /// <summary>
            /// Fade in time.
            /// </summary>
            [SerializeField]
            public float FadeInTime;

            /// <summary>
            /// Fade out time.
            /// </summary>
            [SerializeField]
            public float FadeOutTime;
        }

        /// <summary>
        /// Hit Area.
        /// </summary>
        [Serializable]
        public struct SerializableHitArea
        {
            /// <summary>
            /// Hit area name.
            /// </summary>
            [SerializeField]
            public string Name;

            /// <summary>
            /// Hit area id.
            /// </summary>
            [SerializeField]
            public string Id;
        }

        #endregion
    }
}
