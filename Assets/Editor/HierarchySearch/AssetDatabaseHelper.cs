using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using UnityEditor;
using UnityEngine;

namespace HierarchySearch
{
    public static class AssetDatabaseHelper
    {
        private const string PREFAB_EXTENSION = ".prefab";
#if SEARCH_UNIT_TESTS
        [MenuItem("HierarchySearch/List All Prefabs", false, 100)]
        private static void OutputAllPrefabs()
        {
            List<string> prefabPaths = GetAllPrefabPaths();
            StringBuilder output = new StringBuilder();
            foreach (string path in prefabPaths)
            {
                output.AppendLine(path);
                List<string> dependencyList = GetDependenciesForAssetPath(path);
                foreach (string dependency in dependencyList)
                {
                    output.AppendLine("-- " + dependency);
                }
            }
            Debug.Log(output);
        }

        [MenuItem("HierarchySearch/List All Prefab Scripts", false, 100)]
        private static void OutputAllPrefabScripts()
        {
            List<string> prefabPaths = GetAllPrefabPaths();
            StringBuilder output = new StringBuilder();
            foreach (string path in prefabPaths)
            {
                output.AppendLine(path);
                List<Type> dependencyList = GetScriptDependenciesForAssetPath(path);
                foreach (Type dependency in dependencyList)
                {
                    output.AppendLine("-- " + dependency.Name);
                }
            }
            Debug.Log(output);
        }

        [MenuItem("HierarchySearch/List All Prefab Materials", false, 100)]
        private static void OutputAllPrefabMaterials()
        {
            List<string> prefabPaths = GetAllPrefabPaths();
            StringBuilder output = new StringBuilder();
            foreach (string path in prefabPaths)
            {
                output.AppendLine(path);
                List<string> dependencyList = GetMaterialDependenciesForAssetPath(path);
                foreach (string dependency in dependencyList)
                {
                    output.AppendLine("-- " + dependency);
                }
            }
            Debug.Log(output);
        }
#endif

        private const string SCRIPT_EXTENSION = ".cs";
        private const string MATERIAL_EXTENSION = ".mat";

        private static List<string> GetDependenciesForAssetPath(string path)
        {
            return AssetDatabase.GetDependencies(path, true).Where(dep => dep != path).ToList();
        }

        private static List<string> GetDependenciesForAssetPath(string path, string fileExtension)
        {
            return AssetDatabase.GetDependencies(path, true).Where(dep => dep != path && Path.GetExtension(dep).Equals(fileExtension)).ToList();
        }

        public static List<string> GetAllPrefabPaths()
        {
            return AssetDatabase.GetAllAssetPaths().Where(path => Path.GetExtension(path) == PREFAB_EXTENSION).ToList();
        }

        public static List<Type> GetScriptDependenciesForAssetPath(string path)
        {
            return GetDependenciesForAssetPath(path, SCRIPT_EXTENSION)
                .Select(dep => ReflectionHelper.GetTypeByName(Path.GetFileNameWithoutExtension(dep), true))
                .ToList();
        }

        public static List<string> GetMaterialDependenciesForAssetPath(string path)
        {
            return GetDependenciesForAssetPath(path, MATERIAL_EXTENSION);
        }
    }
}