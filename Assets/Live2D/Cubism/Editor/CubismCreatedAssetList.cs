using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Live2D.Cubism.Editor.Importers
{
    public class CubismCreatedAssetList
    {
        private List<Object> _assets;
        public List<Object> Assets
        {
            get
            {
                if (_assets == null)
                {
                    _assets = new List<Object>();
                }

                return _assets;
            }
        }

        private List<string> _assetPaths;
        public List<string> AssetPaths
        {
            get
            {
                if(_assetPaths == null)
                {
                    _assetPaths = new List<string>();
                }

                return _assetPaths;
            }
        }

        private List<bool> _isImporterDirties;
        public List<bool> IsImporterDirties
        {
            get
            {
                if (_isImporterDirties == null)
                {
                    _isImporterDirties = new List<bool>();
                }

                return _isImporterDirties;
            }
        }

        private static CubismCreatedAssetList _instance;
        public volatile bool onPostImporting = false;


        public static CubismCreatedAssetList GetInstance()
        {
            if (_instance == null)
            {
                _instance = new CubismCreatedAssetList();
            }

            return _instance;
        }


        public void OnPostImport()
        {
            if (_instance.Assets.Count <= 0)
            {
                return;
            }

            onPostImporting = true;

            for (var i = _instance.Assets.Count - 1; i >= 0; i--)
            {
                var asset = _instance.Assets[i];

                if (!IsImporterDirties[i])
                {
                    continue;
                }

                if (asset != null)
                {
                    EditorUtility.SetDirty(asset);
                }

                Remove(i);
            }

            onPostImporting = false;

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }


        public void Remove(int index)
        {
            if (_instance == null || index < 0)
            {
                return;
            }
            _instance.Assets.RemoveAt(index);
            _instance._assetPaths.RemoveAt(index);
            _instance._isImporterDirties.RemoveAt(index);
        }
    }
}
