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
        private static void OutputAllAssemblies()
        {
            List<string> prefabPaths = GetAllPrefabPaths();
            StringBuilder output = new StringBuilder();
            foreach (string path in prefabPaths)
            {
                output.AppendLine(path);
                List<string> dependencyList = GetDependenciesForAssetPath(path);
                foreach (string dependency in dependencyList)
                {
                    output.AppendLine("    " + dependency);
                }
            }
            Debug.Log(output);
        }
#endif

        public static List<string> GetAllPrefabPaths()
        {
            return AssetDatabase.GetAllAssetPaths().Where(path => Path.GetExtension(path) == PREFAB_EXTENSION).ToList();
        }

        public static List<string> GetDependenciesForAssetPath(string path)
        {
            return AssetDatabase.GetDependencies(path, true).ToList();
        }
    }
}

//string[] temp = AssetDatabase.GetAllAssetPaths();
//string[] dependencies = AssetDatabase.GetDependencies( single );