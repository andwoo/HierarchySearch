using System;
using UnityEditor;
using UnityEngine;

namespace HierarchySearch
{
    public static class EditorPrefsUtils
    {
        public static void SaveBool(string key, bool value)
        {
            EditorPrefs.SetBool(key, value);
        }

        public static bool LoadBool(string key, bool defaultValue)
        {
            return EditorPrefs.GetBool(key, defaultValue);
        }

        public static void SaveString(string key, string value)
        {
            EditorPrefs.SetString(key, value);
        }

        public static string LoadString(string key, string defaultValue)
        {
            return EditorPrefs.GetString(key, defaultValue);
        }

        public static void SaveEnum<TEnum>(string key, TEnum value) where TEnum : struct
        {
            SaveString(key, value.ToString());
        }

        public static TEnum LoadEnum<TEnum>(string key, TEnum defaultValue) where TEnum : struct
        {
            return (TEnum)Enum.Parse(typeof(TEnum), LoadString(key, defaultValue.ToString()));
        }

        public static void SaveColor(string key, Color value)
        {
            EditorPrefs.SetFloat(key + ".red", value.r);
            EditorPrefs.SetFloat(key + ".green", value.g);
            EditorPrefs.SetFloat(key + ".blue", value.b);
            EditorPrefs.SetFloat(key + ".alpha", value.a);
        }

        public static Color LoadColor(string key, Color defaultValue)
        {
            if(!EditorPrefs.HasKey(key + ".red"))
            {
                return defaultValue;
            }

            return new Color(
                EditorPrefs.GetFloat(key + ".red"),
                EditorPrefs.GetFloat(key + ".green"),
                EditorPrefs.GetFloat(key + ".blue"),
                EditorPrefs.GetFloat(key + ".alpha")
            );
        }
    }
}
