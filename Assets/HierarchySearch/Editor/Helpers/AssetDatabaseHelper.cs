using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;

namespace HierarchySearch
{
    public static class AssetDatabaseHelper
    {
        private const string PREFAB_EXTENSION = ".prefab";
        private const string SCRIPT_EXTENSION = ".cs";
        private const string MATERIAL_EXTENSION = ".mat";

        private static List<string> GetDependenciesForAssetPath(string path)
        {
            return AssetDatabase.GetDependencies(path, true).Where(dep => dep != path)
                .ToList();
        }

        private static List<string> GetDependenciesForAssetPath(string path, string fileExtension)
        {
            return AssetDatabase.GetDependencies(path, true).Where(dep => dep != path && Path.GetExtension(dep)
                .Equals(fileExtension))
                .ToList();
        }

        public static List<string> GetAllPrefabPaths()
        {
            return AssetDatabase.GetAllAssetPaths().Where(path => Path.GetExtension(path) == PREFAB_EXTENSION)
                .ToList();
        }

        public static List<Type> GetScriptDependenciesForAssetPath(string path)
        {
            return GetDependenciesForAssetPath(path, SCRIPT_EXTENSION)
                .Select(dep => ReflectionHelper.GetTypesByName(Path.GetFileNameWithoutExtension(dep), true, true).FirstOrDefault())
                .ToList();
        }

        public static List<string> GetScriptPathDependenciesForAssetPath(string path)
        {
            return GetDependenciesForAssetPath(path, SCRIPT_EXTENSION)
                .ToList();
        }

        public static List<string> GetMaterialDependenciesForAssetPath(string path)
        {
            return GetDependenciesForAssetPath(path, MATERIAL_EXTENSION);
        }
    }
}