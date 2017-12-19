using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

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
            List<GameObject> rootObjects = new List<GameObject>();
            for (int i = 0; i < SceneManager.sceneCount; ++i)
            {
                GameObject[] arr = SceneManager.GetSceneAt(i).GetRootGameObjects();
                rootObjects.AddRange(arr);
            }

            if (Application.isPlaying)
            {
                GameObject temp = new GameObject();
                UnityEngine.Object.DontDestroyOnLoad(temp);
                rootObjects.AddRange(temp.scene.GetRootGameObjects());
                rootObjects.Remove(temp);
                GameObject.DestroyImmediate(temp);
            }

            return rootObjects;
        }

        public static List<GameObject> FindObjectsByName(string name, bool caseSensitive, bool includeInactive, bool matchWholeWord)
        {
            return FindObjectsOfType<Transform>(includeInactive)
                .Where(obj => StringMatchHelper.DoesNameMatch(obj.name, name, caseSensitive, matchWholeWord))
                .Select(obj => obj.gameObject)
                .ToList();
        }

        public static List<UnityEngine.Object> FindObjectsOfType(Type type, bool includeInactive)
        {
            List <UnityEngine.Object> results = new List<UnityEngine.Object>();
            if(!type.IsSubclassOf(typeof(Component)))
            {
                return results;
            }
            List<GameObject> rootObjects = GetRootGameObjects();
            foreach(GameObject root in rootObjects)
            {
                if(!includeInactive && !root.activeSelf)
                {
                    continue;
                }
                results.AddRange(root.GetComponentsInChildren(type, includeInactive));
            }
            
            return results;
        }

        public static List<TType> FindObjectsOfType<TType>(bool includeInactive) where TType : UnityEngine.Object
        {
            List<TType> results = new List<TType>();
            List<GameObject> rootObjects = GetRootGameObjects();
            foreach (GameObject root in rootObjects)
            {
                if (!includeInactive && !root.activeSelf)
                {
                    continue;
                }
                results.AddRange(root.GetComponentsInChildren<TType>(includeInactive));
            }

            return results;
        }

        public static List<GameObject> GetGameObjectsWithType(Type type, bool includeInactive)
        {
            List<GameObject> results = new List<GameObject>();
            if (type.IsSubclassOf(typeof(UnityEngine.Object)))
            {
                List<UnityEngine.Object> objects = FindObjectsOfType(type, includeInactive);
                if (objects != null && objects.Count > 0)
                {
                    results = objects
                        .Where(component => component is Component)
                        .Select(component => (component as Component).gameObject)
                        .ToList();
                }
            }
            return results;
        }

        public static List<GameObject> GetGameObjectsWithTypes(List<Type> types, bool includeInactive)
        {
            List<GameObject> results = new List<GameObject>();
            foreach(Type type in types)
            {
                results.AddRange(GetGameObjectsWithType(type, includeInactive));
            }
            return results;
        }

        public static List<GameObject> GetGameObjectsWithFieldName(string fieldName, bool caseSensitive, bool includeInactive, bool matchWholeWord)
        {
            fieldName = fieldName.Trim();

            List<GameObject> results = new List<GameObject>();
            List<Component> allComponents = FindObjectsOfType<Component>(includeInactive);
            foreach (Component component in allComponents)
            {
                if (results.Contains(component.gameObject))
                {
                    continue;
                }
                List<FieldInfo> fields = ReflectionHelper.GetFieldsForType(component.GetType());
                bool found = fields.FirstOrDefault(field => StringMatchHelper.DoesNameMatch(field.Name, fieldName, caseSensitive, matchWholeWord)) != null;
                if (found)
                {
                    results.Add(component.gameObject);
                }
            }
            return results;
        }

        public static List<GameObject> GetGameObjectsWithFieldType(string fieldType, bool caseSensitive, bool includeInactive, bool matchWholeWord)
        {
            return GetGameObjectsWithFieldTypes(ReflectionHelper.GetTypesByName(fieldType, caseSensitive, matchWholeWord), includeInactive);
        }

        public static List<GameObject> GetGameObjectsWithFieldTypes(List<Type> fieldTypes, bool includeInactive)
        {
            List<GameObject> results = new List<GameObject>();
            List<Component> allComponents = FindObjectsOfType<Component>(includeInactive);
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

        public static List<GameObject> GetGameObjectsWithPropertyName(string propertyName, bool caseSensitive, bool includeInactive, bool matchWholeWord)
        {
            propertyName = propertyName.Trim();

            List<GameObject> results = new List<GameObject>();
            List<Component> allComponents = FindObjectsOfType<Component>(includeInactive);
            foreach (Component component in allComponents)
            {
                if (results.Contains(component.gameObject))
                {
                    continue;
                }
                List<PropertyInfo> properties = ReflectionHelper.GetPropertiesForType(component.GetType());
                bool found = properties.FirstOrDefault(property => StringMatchHelper.DoesNameMatch(property.Name, propertyName, caseSensitive, matchWholeWord)) != null;
                if (found)
                {
                    results.Add(component.gameObject);
                }
            }
            return results;
        }

        public static List<GameObject> GetGameObjectsWithPropertyType(string propertyType, bool caseSensitive, bool includeInactive, bool matchWholeWord)
        {
            return GetGameObjectsWithPropertyTypes(ReflectionHelper.GetTypesByName(propertyType, caseSensitive, matchWholeWord), includeInactive);
        }

        public static List<GameObject> GetGameObjectsWithPropertyTypes(List<Type> propertyTypes, bool includeInactive)
        {
            List<GameObject> results = new List<GameObject>();
            List<Component> allComponents = FindObjectsOfType<Component>(includeInactive);
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