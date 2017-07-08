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
#if SEARCH_UNIT_TESTS
        [MenuItem("HierarchySearch/List All Assemblies", false, 0)]
        private static void OutputAllAssemblies()
        {
            List<Assembly> assemblies = GetAssemblies();
            StringBuilder logOutput = new StringBuilder();
            foreach (Assembly assembly in assemblies)
            {
                logOutput.AppendLine(assembly.GetName().Name);
            }
            Debug.Log(logOutput.ToString());
        }

        [MenuItem("HierarchySearch/List All Types", false, 0)]
        private static void OutputAllTypes()
        {
            List<Type> allTypes = GetTypesInAssemblies(GetAssemblies());
            StringBuilder logOutput = new StringBuilder();
            foreach (Type type in allTypes)
            {
                logOutput.AppendLine(type.Namespace + "." + type.Name);
            }
            Debug.Log(logOutput.ToString());
        }

        [MenuItem("HierarchySearch/List All Field for Image", false, 0)]
        private static void GetFieldsForType()
        {
            List<FieldInfo> fields = GetFieldsForType(typeof(Image));
            StringBuilder logOutput = new StringBuilder();
            foreach (FieldInfo field in fields)
            {
                logOutput.AppendLine(field.Name);
            }
            Debug.Log(logOutput.ToString());
        }
#endif

        private static Dictionary<string, Type> RESERVED_TYPES = new Dictionary<string, Type>
        {
            { "short", typeof(short) },
            { "int", typeof(int) },
            { "long", typeof(long) },
            
            { "ushort", typeof(ushort) },
            { "uint", typeof(uint) },
            { "ulong", typeof(ulong) },
            
            { "float", typeof(float) },
            { "double", typeof(double) },
            
            { "bool", typeof(bool) }
        };

        public static List<Assembly> GetAssemblies()
        {
            return AppDomain.CurrentDomain.GetAssemblies().ToList();
        }

        public static List<Type> GetTypesInAssemblies(List<Assembly> assemblies)
        {
            return assemblies.SelectMany(assembly => assembly.GetTypes()).ToList();
        }

        public static List<FieldInfo> GetFieldsForType(Type type)
        {
            return type.GetFields(
                BindingFlags.Instance |
                BindingFlags.Static |
                BindingFlags.FlattenHierarchy |
                BindingFlags.NonPublic |
                BindingFlags.Public).ToList();
        }

        public static List<PropertyInfo> GetPropertiesForType(Type type)
        {
            return type.GetProperties(
                BindingFlags.Instance |
                BindingFlags.Static |
                BindingFlags.FlattenHierarchy |
                BindingFlags.NonPublic |
                BindingFlags.Public).ToList();
        }

        public static Type GetTypeByName(string name, bool caseSensitive)
        {
            name = name.Trim();

            if(RESERVED_TYPES.ContainsKey(name))
            {
                return RESERVED_TYPES[name];
            }

            List<Assembly> trimmedAssemblies = GetAssemblies().Where(assembly => assembly.GetName().Name.ToLower().Contains("unityeditor") == false).ToList();
            //prioritize unity assemblies first
            trimmedAssemblies.Sort((as1, as2) => 
            {
                string as1Name = as1.GetName().Name.ToLower();
                string as2Name = as2.GetName().Name.ToLower();
                bool as1ContainsUnity = as1Name.Contains("unityengine");
                bool as2ContainsUnity = as2Name.Contains("unityengine");
                if (as1ContainsUnity != as2ContainsUnity)
                {
                    if(as1ContainsUnity)
                    {
                        return -1;
                    }
                    else
                    {
                        return 1;
                    }
                }
                return as1Name.CompareTo(as2Name);
            });

            return GetTypesInAssemblies(trimmedAssemblies).FirstOrDefault(field => caseSensitive ? field.Name == name : field.Name.ToLowerInvariant() == name.ToLowerInvariant());
        }
    }
}