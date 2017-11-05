using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace HierarchySearch
{
    public static class ReflectionHelper
    {
        private static HashSet<string> IGNORED_ASSEMBLIES = new HashSet<string>()
        {
            "Boo.Lang.Parser",
            "Boo.Lang.Compiler",
            "UnityEditor.iOS.Extensions.Common",
            "UnityEditor.iOS.Extensions.Xcode",
            "UnityEditor.WindowsStandalone.Extensions",
            "UnityEditor.iOS.Extensions",
            "UnityEditor.Android.Extensions",
            "UnityEditor.Graphs",
            "UnityEditor.VR",
            "UnityEditor.Purchasing",
            "UnityEditor.HoloLens",
            "UnityEditor.Analytics",
            "UnityEditor.TreeEditor",
            "UnityEditor.Timeline",
            "nunit.framework",
            "UnityEditor.TestRunner",
            "UnityEditor.Networking",
            "UnityEditor.UI",
            "UnityEditor.Advertisements",
            "Unity.PackageManager",
            "I18N.West",
            "I18N"
        };

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

        public static List<Type> GetTypesByName(string name, bool caseSensitive, bool matchWholeWord)
        {
            name = name.Trim();
            List<Type> foundTypes = new List<Type>();
            if (RESERVED_TYPES.ContainsKey(name))
            {
                foundTypes.Add(RESERVED_TYPES[name]);
                if(matchWholeWord)
                {
                    return foundTypes;
                }
            }

            List<Assembly> trimmedAssemblies = GetAssemblies().Where(assembly => !IGNORED_ASSEMBLIES.Contains(assembly.GetName().Name)).ToList();
            foundTypes.AddRange(GetTypesInAssemblies(trimmedAssemblies).Where(type => StringMatchHelper.DoesNameMatch(type.Name, name, caseSensitive, matchWholeWord)));
            return foundTypes;
        }
    }
}