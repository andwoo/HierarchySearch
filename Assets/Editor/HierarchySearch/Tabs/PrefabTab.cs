using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace HierarchySearch
{
    public class PrefabTab : IWindowTab
    {
        private List<string> m_Prefabs;
        private HashSet<string> m_SearchResults;
        private SearchWidget m_SearchWidget;
        private Vector2 m_ScrollPosition;

        public void OnEnable()
        {
            m_Prefabs = AssetDatabaseHelper.GetAllPrefabPaths();
            m_SearchResults = new HashSet<string>(m_Prefabs);
            m_SearchWidget = new SearchWidget(SearchType.Component, OnSearch, OnClear);
        }

        public void OnDisable()
        {

        }

        public void OnDestroy()
        {
            m_SearchWidget.OnDestroy();
            m_SearchWidget = null;
        }

        public void OnGUI()
        {
            m_SearchWidget.OnGUI();
            m_ScrollPosition = EditorGUILayout.BeginScrollView(m_ScrollPosition);
            foreach(string prefabPath in m_Prefabs)
            {
                if(m_SearchResults.Contains(prefabPath))
                {
                    DrawPrefabUI(prefabPath);
                }
            }
            EditorGUILayout.EndScrollView();
        }

        private void OnSearch(SearchType type, string term)
        {
            if (string.IsNullOrEmpty(term))
            {
                return;
            }

            Type termType = ReflectionHelper.GetTypeByName(term, m_SearchWidget.CaseSensitive);
            if (termType != null)
            {
                foreach (string prefabPath in m_Prefabs)
                {
                    List<Type> prefabTypes = AssetDatabaseHelper.GetScriptDependenciesForAssetPath(prefabPath);
                    if(!prefabTypes.Contains(termType))
                    {
                        m_SearchResults.Remove(prefabPath);
                    }
                }
            }
        }

        private void OnClear()
        {
            m_SearchResults = new HashSet<string>(m_Prefabs);
        }

        private void DrawPrefabUI(string prefabPath)
        {
            if (GUILayout.Button(Path.GetFileNameWithoutExtension(prefabPath)))
            {
                EditorGUIUtility.PingObject(AssetDatabase.LoadAssetAtPath<UnityEngine.Object>(prefabPath));
            }
        }
    }
}
