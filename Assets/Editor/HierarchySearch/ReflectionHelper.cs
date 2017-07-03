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
        [MenuItem("HierarchySearch/List All Assemblies", false, 0)]
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

        [MenuItem("HierarchySearch/List All Types", false, 0)]
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

        [MenuItem("HierarchySearch/List All Field for Image", false, 0)]
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

        private static Dictionary<string, Type> RESERVED_TYPES = new Dictionary<string, Type>
        {
            { "short", typeof(Int16) },
            { "int", typeof(Int32) },
            { "bool", typeof(Boolean) }
        };

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

        public static PropertyInfo[] GetPropertiesForType(Type type)
        {
            return type.GetProperties(
                BindingFlags.Instance |
                BindingFlags.Static |
                BindingFlags.FlattenHierarchy |
                BindingFlags.NonPublic |
                BindingFlags.Public);
        }

        public static Type GetTypeByName(string name, bool caseSensitive)
        {
            name = name.Trim();

            if(RESERVED_TYPES.ContainsKey(name))
            {
                return RESERVED_TYPES[name];
            }

            Assembly[] trimmedAssemblies = GetAssemblies().Where(assembly => assembly.GetName().Name != "UnityEditor").ToArray();
            return GetTypesInAssemblies(trimmedAssemblies).FirstOrDefault(field => caseSensitive ? field.Name == name : field.Name.ToLowerInvariant() == name.ToLowerInvariant());
        }
    }
}