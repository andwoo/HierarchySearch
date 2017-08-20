using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace HierarchySearch
{
    public class PrefabSearchTab : IWindowTab
    {
        delegate void SearchHandler(string searchTerm, bool caseSensitive, HashSet<string> searchResults);

        private Dictionary<PrefabSearchType, SearchHandler> m_SearchHandlers;
        private HashSet<string> m_SearchResults;
        private SearchWidget<PrefabSearchType> m_SearchWidget;
        private SearchHelpBoxPrompt m_SearchPrompt;
        private Vector2 m_ScrollPosition;

        public PrefabSearchTab()
        {
            m_SearchPrompt = new SearchHelpBoxPrompt();
            m_SearchResults = new HashSet<string>();

            m_SearchHandlers = new Dictionary<PrefabSearchType, SearchHandler>();
            m_SearchHandlers.Add(PrefabSearchType.Component, SearchComponentType);
        }

        public void OnEnable()
        {
            m_SearchWidget = new SearchWidget<PrefabSearchType>(PrefabSearchType.Component, OnSearch, OnClear);
        }

        public void OnDisable()
        {

        }

        public void OnDestroy()
        {
            m_SearchWidget.OnDestroy();
            m_SearchWidget = null;
            m_SearchHandlers.Clear();
        }

        public void OnGUI()
        {
            m_SearchWidget.OnGUI();

            if (m_SearchResults.Count > 0)
            {
                m_ScrollPosition = EditorGUILayout.BeginScrollView(m_ScrollPosition);//, GUI.skin.box);
                foreach (string prefabPath in m_SearchResults)
                {
                    DrawPrefabUI(prefabPath);
                }
                EditorGUILayout.EndScrollView();
            }

            if (!string.IsNullOrEmpty(m_SearchPrompt.message))
            {
                EditorGUILayout.HelpBox(m_SearchPrompt.message, m_SearchPrompt.type);
            }
        }

        private void OnSearch(PrefabSearchType type, string term)
        {
            m_SearchResults.Clear();
            if (string.IsNullOrEmpty(term))
            {
                m_SearchPrompt.message = "Search term cannot be empty.";
                m_SearchPrompt.type = MessageType.Error;
                return;
            }

            m_SearchHandlers[type](term, m_SearchWidget.CaseSensitive, m_SearchResults);
            if (m_SearchResults.Count == 0)
            {
                m_SearchPrompt.message = string.Format("Could not find match for \"{0}\"", term);
                m_SearchPrompt.type = MessageType.Info;
            }
            else
            {
                m_SearchPrompt.message = string.Empty;
            }
        }

        private void OnClear()
        {
            m_SearchResults.Clear();
            m_SearchPrompt.message = string.Empty;
        }

        private void DrawPrefabUI(string prefabPath)
        {
            EditorGUILayout.BeginVertical(GUI.skin.box);
            if(GUILayout.Button(Path.GetFileNameWithoutExtension(prefabPath), GUIStyle.none))
            {
                EditorGUIUtility.PingObject(AssetDatabase.LoadAssetAtPath<UnityEngine.Object>(prefabPath));
            }
            EditorGUILayout.EndVertical();
        }

        #region Search Methods
        private static void SearchComponentType(string searchTerm, bool caseSensitive, HashSet<string> searchResults)
        {
            List<string> prefabPaths = AssetDatabaseHelper.GetAllPrefabPaths();
            foreach(string prefabPath in prefabPaths)
            {
                List<string> scriptPaths = AssetDatabaseHelper.GetScriptPathDependenciesForAssetPath(prefabPath);
                foreach (string scriptPath in scriptPaths)
                {
                    bool found = caseSensitive ? searchTerm == Path.GetFileNameWithoutExtension(scriptPath) : searchTerm.ToLowerInvariant() == Path.GetFileNameWithoutExtension(scriptPath).ToLowerInvariant();
                    if (found)
                    {
                        searchResults.Add(prefabPath);
                    }
                }
            }
        }
        #endregion

    }
}
