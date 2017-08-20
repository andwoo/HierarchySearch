﻿using System;
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
            List<GameObject> results = new List<GameObject>();
            if (type.IsSubclassOf(typeof(UnityEngine.Object)))
            {
                UnityEngine.Object[] objects = GameObject.FindObjectsOfType(type);
                if (objects != null && objects.Length > 0)
                {
                    results = objects
                        .Where(component => component is Component)
                        .Select(component => (component as Component).gameObject)
                        .ToList();
                }
            }
            return results;
        }

        public static List<GameObject> GetGameObjectsWithTypes(List<Type> types)
        {
            List<GameObject> results = new List<GameObject>();
            foreach(Type type in types)
            {
                results.AddRange(GetGameObjectsWithType(type));
            }
            return results;
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
                List<FieldInfo> fields = ReflectionHelper.GetFieldsForType(component.GetType());
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
            return GetGameObjectsWithFieldTypes(ReflectionHelper.GetTypesByName(fieldType, caseSensitive));
        }

        public static List<GameObject> GetGameObjectsWithFieldTypes(List<Type> fieldTypes)
        {
            List<GameObject> results = new List<GameObject>();
            Component[] allComponents = GameObject.FindObjectsOfType<Component>();
            foreach (Component component in allComponents)
            {
                if (results.Contains(component.gameObject))
                {
                    continue;
                }
                List<FieldInfo> fields = ReflectionHelper.GetFieldsForType(component.GetType());
                bool found = fields.FirstOrDefault(field => fieldTypes.Contains(field.FieldType)) != null;
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
                List<PropertyInfo> properties = ReflectionHelper.GetPropertiesForType(component.GetType());
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
            return GetGameObjectsWithPropertyTypes(ReflectionHelper.GetTypesByName(propertyType, caseSensitive));
        }

        public static List<GameObject> GetGameObjectsWithPropertyTypes(List<Type> propertyTypes)
        {
            List<GameObject> results = new List<GameObject>();
            Component[] allComponents = GameObject.FindObjectsOfType<Component>();
            foreach (Component component in allComponents)
            {
                if (results.Contains(component.gameObject))
                {
                    continue;
                }
                List<PropertyInfo> properties = ReflectionHelper.GetPropertiesForType(component.GetType());
                bool found = properties.FirstOrDefault(property => propertyTypes.Contains(property.PropertyType)) != null;
                if (found)
                {
                    results.Add(component.gameObject);
                }
            }
            return results;
        }
    }
}