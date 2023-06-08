/**
 * Copyright(c) Live2D Inc. All rights reserved.
 *
 * Use of this source code is governed by the Live2D Open Software license
 * that can be found at https://www.live2d.com/eula/live2d-open-software-license-agreement_en.html.
 */


using System.Linq;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;

public class CubismWebGLPluginProcessor : IPreprocessBuildWithReport
{
    /// <summary>
    /// Execution order.
    /// </summary>
    public int callbackOrder
    {
        get { return 0; }
    }

    /// <summary>
    /// Enable the appropriate plugins from your Unity version before building.
    /// </summary>
    public void OnPreprocessBuild(BuildReport report)
    {
        // Skip the process if the build is not for WebGL.
        if (report.summary.platform != BuildTarget.WebGL)
        {
            return;
        }


        // Detect the type of WebGL plugin by SDK type and SDK version in the build settings.
        var targetPlugin =
#if UNITY_2021_2_OR_NEWER
            CubismWebGLPlugin.Latest;
#else
            CubismWebGLPlugin.Previous;
#endif


        // Extract the Cubism WebGL plugin from the plugin.
        var pluginImporters = PluginImporter.GetAllImporters()
            .Where(pluginImporter =>
                Regex.IsMatch(
                    pluginImporter.assetPath,
                    @"^.*/Experimental/Emscripten/.*/Live2DCubismCore.bc$"
                )
            )
            .ToArray();


        // Enable only the appropriate plugins.
        foreach (var pluginImporter in pluginImporters)
        {
            pluginImporter.SetCompatibleWithPlatform(
                BuildTarget.WebGL,
                pluginImporter.assetPath.Contains(targetPlugin.DirectoryName)
            );
        }
    }


    /// <summary>
    /// Defines the version of the plugin for WebGL.
    /// </summary>
    private class CubismWebGLPlugin
    {
        public readonly string DirectoryName;

        public static CubismWebGLPlugin Latest
        {
            get { return new CubismWebGLPlugin("latest"); }
        }
        public static CubismWebGLPlugin Previous
        {
            get { return new CubismWebGLPlugin("1_38_48"); }
        }

        private CubismWebGLPlugin(string directoryName)
        {
            DirectoryName = directoryName;
        }
    }
}
