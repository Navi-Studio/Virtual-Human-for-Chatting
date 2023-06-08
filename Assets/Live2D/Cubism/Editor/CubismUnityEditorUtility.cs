using System.IO;
using UnityEditor;

namespace Live2D.Cubism.Editor
{
    public class CubismUnityEditorUtility
    {
        /// <summary>
        /// Projectウィンドウで現在選択しているディレクトリのパスを取得。
        /// Projectウィンドウ以外が選択されていたり、何も選択されていない場合、返す値はAssets直下。
        /// </summary>
        /// <returns>Projectウィンドウで現在のディレクトリのパス</returns>
        public static string GetCurrentDirectoryPath()
        {
            var activeObject = Selection.activeObject;
            var currentDirectoryPath = ((activeObject == null)
                ? "Assets"
                : AssetDatabase.GetAssetPath(activeObject.GetInstanceID()));

            if (string.IsNullOrEmpty(currentDirectoryPath))
            {
                currentDirectoryPath = "Assets";
            }
            else if (!Directory.Exists(currentDirectoryPath))
            {
                currentDirectoryPath = currentDirectoryPath.Replace("/" + Path.GetFileName(currentDirectoryPath), "");
            }

            return currentDirectoryPath;
        }
    }
}
