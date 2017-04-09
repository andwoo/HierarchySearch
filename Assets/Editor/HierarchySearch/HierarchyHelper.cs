using System;
using System.Collections.Generic;
using System.Linq;
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
            return GameObject.FindObjectsOfType(type)
                .Select(component => (component as Component).gameObject)
                .ToList();
        }
    }
}