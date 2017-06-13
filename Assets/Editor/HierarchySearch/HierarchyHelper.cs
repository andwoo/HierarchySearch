using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace HierarchySearch
{
    public static class HierarchyHelper
    {
        public static GameObject GetSelectedGameObject()
        {
            return Selection.activeGameObject;
        }

        public static List<GameObject> GetRootGameObjects()
        {
            return UnityEngine.SceneManagement.SceneManager.GetActiveScene().GetRootGameObjects().ToList();
        }

        public static List<GameObject> GetGameObjectsWithType(Type type)
        {
            UnityEngine.Object[] objects = GameObject.FindObjectsOfType(type);
            if(objects != null && objects.Length > 0)
            {
                return objects
                    .Where(component => component is Component)
                    .Select(component => (component as Component).gameObject)
                    .ToList();
            }
            return null;
        }

        public static List<GameObject> GetGameObjectsWithFieldName(string fieldName, bool caseSensitive)
        {
            fieldName = fieldName.Trim();

            List<GameObject> results = new List<GameObject>();
            Component[] allComponents = GameObject.FindObjectsOfType<Component>();
            foreach (Component component in allComponents)
            {
                if (results.Contains(component.gameObject))
                {
                    continue;
                }
                FieldInfo[] fields = ReflectionHelper.GetFieldsForType(component.GetType());
                bool found = fields.FirstOrDefault(field =>
                {
                    if(caseSensitive)
                    {
                        return field.Name == fieldName;
                    }
                    else
                    {
                        return field.Name.ToLowerInvariant() == fieldName.ToLowerInvariant();
                    }
                }) != null;
                if (found)
                {
                    results.Add(component.gameObject);
                }
            }
            return results;
        }

        public static List<GameObject> GetGameObjectsWithFieldType(string fieldType, bool caseSensitive)
        {
            return GetGameObjectsWithFieldType(ReflectionHelper.GetTypeByName(fieldType, caseSensitive));
        }

        public static List<GameObject> GetGameObjectsWithFieldType(Type fieldType)
        {
            List<GameObject> results = new List<GameObject>();
            Component[] allComponents = GameObject.FindObjectsOfType<Component>();
            foreach (Component component in allComponents)
            {
                if (results.Contains(component.gameObject))
                {
                    continue;
                }
                FieldInfo[] fields = ReflectionHelper.GetFieldsForType(component.GetType());
                bool found = fields.FirstOrDefault(field => field.FieldType.Equals(fieldType)) != null;
                if (found)
                {
                    results.Add(component.gameObject);
                }
            }
            return results;
        }

        public static List<GameObject> GetGameObjectsWithPropertyName(string propertyName, bool caseSensitive)
        {
            propertyName = propertyName.Trim();

            List<GameObject> results = new List<GameObject>();
            Component[] allComponents = GameObject.FindObjectsOfType<Component>();
            foreach (Component component in allComponents)
            {
                if (results.Contains(component.gameObject))
                {
                    continue;
                }
                PropertyInfo[] properties = ReflectionHelper.GetPropertiesForType(component.GetType());
                bool found = properties.FirstOrDefault(property =>
                {
                    if(caseSensitive)
                    {
                        return property.Name == propertyName;
                    }
                    else
                    {
                        return property.Name.ToLowerInvariant() == propertyName.ToLowerInvariant();
                    }
                }) != null;
                if (found)
                {
                    results.Add(component.gameObject);
                }
            }
            return results;
        }

        public static List<GameObject> GetGameObjectsWithPropertyType(string propertyType, bool caseSensitive)
        {
            return GetGameObjectsWithPropertyType(ReflectionHelper.GetTypeByName(propertyType, caseSensitive));
        }

        public static List<GameObject> GetGameObjectsWithPropertyType(Type propertyType)
        {
            List<GameObject> results = new List<GameObject>();
            Component[] allComponents = GameObject.FindObjectsOfType<Component>();
            foreach (Component component in allComponents)
            {
                if (results.Contains(component.gameObject))
                {
                    continue;
                }
                PropertyInfo[] properties = ReflectionHelper.GetPropertiesForType(component.GetType());
                bool found = properties.FirstOrDefault(property => property.PropertyType.Equals(propertyType)) != null;
                if (found)
                {
                    results.Add(component.gameObject);
                }
            }
            return results;
        }
    }
}