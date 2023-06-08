/**
 * Copyright(c) Live2D Inc. All rights reserved.
 *
 * Use of this source code is governed by the Live2D Open Software license
 * that can be found at https://www.live2d.com/eula/live2d-open-software-license-agreement_en.html.
 */


using Live2D.Cubism.Editor.Deleters;
using Live2D.Cubism.Editor.Importers;
using Live2D.Cubism.Rendering;
using Live2D.Cubism.Rendering.Masking;
using System;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;


namespace Live2D.Cubism.Editor
{
    /// <summary>
    /// Hooks into Unity's asset pipeline allowing custom processing of assets.
    /// </summary>
    public class CubismAssetProcessor : AssetPostprocessor
    {
        #region Unity Event Handling

        /// <summary>
        /// Called by Unity. Makes sure <see langword="unsafe"/> code is allowed.
        /// </summary>
        // ReSharper disable once InconsistentNaming
        public static void OnGeneratedCSProjectFiles()
        {
            AllowUnsafeCode();
        }


        /// <summary>
        /// Called by Unity on asset import. Handles importing of Cubism related assets.
        /// </summary>
        /// <param name="importedAssetPaths">Paths of imported assets.</param>
        /// <param name="deletedAssetPaths">Paths of removed assets.</param>
        /// <param name="movedAssetPaths">Paths of moved assets</param>
        /// <param name="movedFromAssetPaths">Paths of moved assets before moving</param>
        private static void OnPostprocessAllAssets(
            string[] importedAssetPaths,
            string[] deletedAssetPaths,
            string[] movedAssetPaths,
            string[] movedFromAssetPaths)
        {
            // Make sure builtin resources are available.
            GenerateBuiltinResources();

            var assetList = CubismCreatedAssetList.GetInstance();

            // Handle any imported Cubism assets.
            foreach (var assetPath in importedAssetPaths)
            {
                var importer = CubismImporter.GetImporterAtPath(assetPath);


                if (importer == null)
                {
                    continue;
                }

                try
                {
                    importer.Import();
                }
                catch(Exception e)
                {
                    Debug.LogError("CubismAssetProcessor : Following error occurred while importing " + assetPath);
                    Debug.LogError(e);
                }
            }

            assetList.OnPostImport();

            // Handle any deleted Cubism assets.
            foreach (var assetPath in deletedAssetPaths)
            {
                var deleter = CubismDeleter.GetDeleterAsPath(assetPath);

                if (deleter == null)
                {
                    continue;
                }

                deleter.Delete();
            }

        }

        #endregion

        #region C# Project Patching

        /// <summary>
        /// Makes sure <see langword="unsafe"/> code is allowed in the runtime project.
        /// </summary>
        private static void AllowUnsafeCode()
        {
            foreach (var csproj in Directory.GetFiles(Directory.GetCurrentDirectory(), "*.csproj"))
            {
                // Skip Editor assembly.
                if (csproj.EndsWith(".Editor.csproj"))
                {
                    continue;
                }


                var document = XDocument.Load(csproj);
                var project = document.Root;


                // Allow unsafe code.
                for (var propertyGroup = project.FirstNode as XElement; propertyGroup != null; propertyGroup = propertyGroup.NextNode as XElement)
                {
                    // Skip non-relevant groups.
                    if (!propertyGroup.ToString().Contains("PropertyGroup") || !propertyGroup.ToString().Contains("$(Configuration)|$(Platform)"))
                    {
                        continue;
                    }


                    // Add unsafe-block element if necessary.
                    if (!propertyGroup.ToString().Contains("AllowUnsafeBlocks"))
                    {
                        var nameSpace = propertyGroup.GetDefaultNamespace();


                        propertyGroup.Add(new XElement(nameSpace + "AllowUnsafeBlocks", "true"));
                    }


                    // Make sure unsafe-block element is always set to true.
                    for (var allowUnsafeBlocks = propertyGroup.FirstNode as XElement; allowUnsafeBlocks != null; allowUnsafeBlocks = allowUnsafeBlocks.NextNode as XElement)
                    {
                        if (!allowUnsafeBlocks.ToString().Contains("AllowUnsafeBlocks"))
                        {
                            continue;
                        }


                        allowUnsafeBlocks.SetValue("true");
                    }
                }


                // Store changes.
                document.Save(csproj);
            }
        }

        #endregion

        #region Resources Generation

        /// <summary>
        /// Sets Cubism-style normal blending for a material.
        /// </summary>
        /// <param name="material">Material to set up.</param>
        private static void EnableNormalBlending(Material material)
        {
            material.SetInt("_SrcColor", (int)BlendMode.One);
            material.SetInt("_DstColor", (int)BlendMode.OneMinusSrcAlpha);
            material.SetInt("_SrcAlpha", (int)BlendMode.One);
            material.SetInt("_DstAlpha", (int)BlendMode.OneMinusSrcAlpha);
        }

        /// <summary>
        /// Sets Cubism-style additive blending for a material.
        /// </summary>
        /// <param name="material">Material to set up.</param>
        private static void EnableAdditiveBlending(Material material)
        {
            material.SetInt("_SrcColor", (int)BlendMode.One);
            material.SetInt("_DstColor", (int)BlendMode.One);
            material.SetInt("_SrcAlpha", (int)BlendMode.Zero);
            material.SetInt("_DstAlpha", (int)BlendMode.One);
        }

        /// <summary>
        /// Sets Cubism-style multiplicative blending for a material.
        /// </summary>
        /// <param name="material">Material to set up.</param>
        private static void EnableMultiplicativeBlending(Material material)
        {
            material.SetInt("_SrcColor", (int)BlendMode.DstColor);
            material.SetInt("_DstColor", (int)BlendMode.OneMinusSrcAlpha);
            material.SetInt("_SrcAlpha", (int)BlendMode.Zero);
            material.SetInt("_DstAlpha", (int)BlendMode.One);
        }

        /// <summary>
        /// Sets Cubism-style culling for a mask material.
        /// </summary>
        /// <param name="material">Material to set up.</param>
        private static void EnableCulling(Material material)
        {
            material.SetInt("_Cull", (int)CullMode.Front);
        }

        /// <summary>
        /// Enables Cubism-style masking for a material.
        /// </summary>
        /// <param name="material">Material to set up.</param>
        private static void EnableMasking(Material material)
        {
            // Set toggle.
            material.SetInt("cubism_MaskOn", 1);


            // Enable keyword.
            var shaderKeywords = material.shaderKeywords.ToList();


            shaderKeywords.Clear();


            if (!shaderKeywords.Contains("CUBISM_MASK_ON"))
            {
                shaderKeywords.Add("CUBISM_MASK_ON");
            }


            material.shaderKeywords = shaderKeywords.ToArray();
        }

        /// <summary>
        /// Enables Cubism-style inverted mask for a material.
        /// </summary>
        /// <param name="material">Material to set up.</param>
        private static void EnableInvertedMask(Material material)
        {
            // Set toggle.
            material.SetInt("cubism_MaskOn", 1);
            material.SetInt("cubism_InvertOn", 1);


            // Enable keyword.
            var shaderKeywords = material.shaderKeywords.ToList();

            shaderKeywords.Clear();

            if (!shaderKeywords.Contains("CUBISM_INVERT_ON"))
            {
                shaderKeywords.Add("CUBISM_INVERT_ON");
            }


            material.shaderKeywords = shaderKeywords.ToArray();
        }

        /// <summary>
        /// Generates the builtin resources as necessary.
        /// </summary>
        private static void GenerateBuiltinResources()
        {
            var resourcesRoot = AssetDatabase
                .GetAssetPath(CubismBuiltinShaders.Unlit)
                .Replace("/Shaders/Unlit.shader", "");


            // Create materials.
            if (CubismBuiltinMaterials.Mask == null)
            {
                var materialsRoot = resourcesRoot + "/Materials";


                // Make sure materials folder exists.
                if (!Directory.Exists(Path.Combine(Directory.GetCurrentDirectory(), materialsRoot)))
                {
                    Directory.CreateDirectory(Path.Combine(Directory.GetCurrentDirectory(), materialsRoot));
                }


                // Create mask material.
                var material = new Material (CubismBuiltinShaders.Mask)
                {
                    name = "Mask"
                };


                AssetDatabase.CreateAsset(material, string.Format("{0}/{1}.mat", materialsRoot, material.name));


                // Create mask material.
                material = new Material (CubismBuiltinShaders.Mask)
                {
                    name = "MaskCulling"
                };

                EnableCulling(material);
                AssetDatabase.CreateAsset(material, string.Format("{0}/{1}.mat", materialsRoot, material.name));


                // Create non-masked materials.
                material = new Material (CubismBuiltinShaders.Unlit)
                {
                    name = "Unlit"
                };

                EnableNormalBlending(material);
                AssetDatabase.CreateAsset(material, string.Format("{0}/{1}.mat", materialsRoot, material.name));


                material = new Material (CubismBuiltinShaders.Unlit)
                {
                    name = "UnlitAdditive"
                };

                EnableAdditiveBlending(material);
                AssetDatabase.CreateAsset(material, string.Format("{0}/{1}.mat", materialsRoot, material.name));


                material = new Material (CubismBuiltinShaders.Unlit)
                {
                    name = "UnlitMultiply"
                };

                EnableMultiplicativeBlending(material);
                AssetDatabase.CreateAsset(material, string.Format("{0}/{1}.mat", materialsRoot, material.name));


                // Create masked materials.
                material = new Material (CubismBuiltinShaders.Unlit)
                {
                    name = "UnlitMasked"
                };

                EnableNormalBlending(material);
                EnableMasking(material);
                AssetDatabase.CreateAsset(material, string.Format("{0}/{1}.mat", materialsRoot, material.name));


                material = new Material (CubismBuiltinShaders.Unlit)
                {
                    name = "UnlitAdditiveMasked"
                };

                EnableAdditiveBlending(material);
                EnableMasking(material);
                AssetDatabase.CreateAsset(material, string.Format("{0}/{1}.mat", materialsRoot, material.name));


                material = new Material (CubismBuiltinShaders.Unlit)
                {
                    name = "UnlitMultiplyMasked"
                };

                EnableMultiplicativeBlending(material);
                EnableMasking(material);
                AssetDatabase.CreateAsset(material, string.Format("{0}/{1}.mat", materialsRoot, material.name));


                // Create inverted mask materials.
                material = new Material (CubismBuiltinShaders.Unlit)
                {
                    name = "UnlitMaskedInverted"
                };

                EnableNormalBlending(material);
                EnableInvertedMask(material);
                AssetDatabase.CreateAsset(material, string.Format("{0}/{1}.mat", materialsRoot, material.name));


                material = new Material (CubismBuiltinShaders.Unlit)
                {
                    name = "UnlitAdditiveMaskedInverted"
                };

                EnableAdditiveBlending(material);
                EnableInvertedMask(material);
                AssetDatabase.CreateAsset(material, string.Format("{0}/{1}.mat", materialsRoot, material.name));


                material = new Material (CubismBuiltinShaders.Unlit)
                {
                    name = "UnlitMultiplyMaskedInverted"
                };

                EnableMultiplicativeBlending(material);
                EnableInvertedMask(material);
                AssetDatabase.CreateAsset(material, string.Format("{0}/{1}.mat", materialsRoot, material.name));


                // Create non-masked materials.
                material = new Material(CubismBuiltinShaders.Unlit)
                {
                    name = "UnlitCulling"
                };

                EnableNormalBlending(material);
                EnableCulling(material);
                AssetDatabase.CreateAsset(material, string.Format("{0}/{1}.mat", materialsRoot, material.name));


                material = new Material(CubismBuiltinShaders.Unlit)
                {
                    name = "UnlitAdditiveCulling"
                };

                EnableAdditiveBlending(material);
                EnableCulling(material);
                AssetDatabase.CreateAsset(material, string.Format("{0}/{1}.mat", materialsRoot, material.name));


                material = new Material(CubismBuiltinShaders.Unlit)
                {
                    name = "UnlitMultiplyCulling"
                };

                EnableMultiplicativeBlending(material);
                EnableCulling(material);
                AssetDatabase.CreateAsset(material, string.Format("{0}/{1}.mat", materialsRoot, material.name));


                // Create masked materials.
                material = new Material(CubismBuiltinShaders.Unlit)
                {
                    name = "UnlitMaskedCulling"
                };

                EnableNormalBlending(material);
                EnableMasking(material);
                EnableCulling(material);
                AssetDatabase.CreateAsset(material, string.Format("{0}/{1}.mat", materialsRoot, material.name));


                material = new Material(CubismBuiltinShaders.Unlit)
                {
                    name = "UnlitAdditiveMaskedCulling"
                };

                EnableAdditiveBlending(material);
                EnableMasking(material);
                EnableCulling(material);
                AssetDatabase.CreateAsset(material, string.Format("{0}/{1}.mat", materialsRoot, material.name));


                material = new Material(CubismBuiltinShaders.Unlit)
                {
                    name = "UnlitMultiplyMaskedCulling"
                };

                EnableMultiplicativeBlending(material);
                EnableMasking(material);
                EnableCulling(material);
                AssetDatabase.CreateAsset(material, string.Format("{0}/{1}.mat", materialsRoot, material.name));


                // Create inverted mask materials.
                material = new Material(CubismBuiltinShaders.Unlit)
                {
                    name = "UnlitMaskedInvertedCulling"
                };

                EnableNormalBlending(material);
                EnableInvertedMask(material);
                EnableCulling(material);
                AssetDatabase.CreateAsset(material, string.Format("{0}/{1}.mat", materialsRoot, material.name));


                material = new Material(CubismBuiltinShaders.Unlit)
                {
                    name = "UnlitAdditiveMaskedInvertedCulling"
                };

                EnableAdditiveBlending(material);
                EnableInvertedMask(material);
                EnableCulling(material);
                AssetDatabase.CreateAsset(material, string.Format("{0}/{1}.mat", materialsRoot, material.name));


                material = new Material(CubismBuiltinShaders.Unlit)
                {
                    name = "UnlitMultiplyMaskedInvertedCulling"
                };

                EnableMultiplicativeBlending(material);
                EnableInvertedMask(material);
                EnableCulling(material);
                AssetDatabase.CreateAsset(material, string.Format("{0}/{1}.mat", materialsRoot, material.name));


                EditorUtility.SetDirty(CubismBuiltinShaders.Unlit);
                AssetDatabase.SaveAssets();
            }


            // Create global mask texture.
            if (CubismMaskTexture.GlobalMaskTexture == null)
            {
                var globalMaskTexture = ScriptableObject.CreateInstance<CubismMaskTexture>();

                globalMaskTexture.name = "GlobalMaskTexture";


                AssetDatabase.CreateAsset(globalMaskTexture, string.Format("{0}/{1}.asset", resourcesRoot, globalMaskTexture.name));
            }
        }

        #endregion
    }
}
