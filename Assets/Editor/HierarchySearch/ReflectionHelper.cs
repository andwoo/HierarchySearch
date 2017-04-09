#define ENABLE_UNIT_TESTS

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

namespace HierarchySearch
{
    public static class ReflectionHelper
    {
        #if ENABLE_UNIT_TESTS
        [MenuItem("HierarchySearch/List All Assemblies")]
        private static void OutputAllAssemblies()
        {
            Assembly[] assemblies = GetAssemblies();
            StringBuilder logOutput = new StringBuilder();
            for (int i = 0; i < assemblies.Length; ++i)
            {
                logOutput.AppendLine(assemblies[i].GetName().Name);
            }
            Debug.Log(logOutput.ToString());
        }

        [MenuItem("HierarchySearch/List All Types")]
        private static void OutputAllTypes()
        {
            Type[] allTypes = GetTypesInAssemblies(GetAssemblies());
            StringBuilder logOutput = new StringBuilder();
            for (int i = 0; i < allTypes.Length; ++i)
            {
                logOutput.AppendLine(allTypes[i].Namespace + "." + allTypes[i].Name);
            }
            Debug.Log(logOutput.ToString());
        }

        [MenuItem("HierarchySearch/List All Field for Image")]
        private static void GetFieldsForType()
        {
            FieldInfo[] fields = GetFieldsForType(typeof(Image));
            StringBuilder logOutput = new StringBuilder();
            for (int i = 0; i < fields.Length; ++i)
            {
                logOutput.AppendLine(fields[i].Name);
            }
            Debug.Log(logOutput.ToString());
        }
        #endif

        public static Assembly[] GetAssemblies()
        {
            return AppDomain.CurrentDomain.GetAssemblies();
        }

        public static Type[] GetTypesInAssemblies(params Assembly[] assemblies)
        {
            return assemblies.SelectMany(assembly => assembly.GetTypes()).ToArray();
        }

        public static FieldInfo[] GetFieldsForType(Type type)
        {
            return type.GetFields(
                BindingFlags.Instance |
                BindingFlags.Static |
                BindingFlags.FlattenHierarchy |
                BindingFlags.NonPublic |
                BindingFlags.Public);
        }

        public static Type GetTypeByName(string name, bool caseSensitive)
        {
            return GetTypesInAssemblies(GetAssemblies()).FirstOrDefault(field => caseSensitive ? field.Name == name : field.Name.ToLowerInvariant() == name.ToLowerInvariant());
        }
    }
}