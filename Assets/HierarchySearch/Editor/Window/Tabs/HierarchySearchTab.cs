using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace HierarchySearch
{
    internal class SearchHelpBoxPrompt
    {
        public string message;
        public MessageType type;
    }

    public class HierarchySearchTab : IWindowTab
    {
        delegate void SearchHandler(string searchTerm, bool caseSensitive, bool includeInactive, bool matchWholeWord, HashSet<int> searchResults);

        private Action m_OnRepaintWindow;
        private Dictionary<HierarchySearchType, SearchHandler> m_SearchHandlers;
        private HashSet<int> m_SearchResults;
        private SearchWidget<HierarchySearchType> m_SearchWidget;
        private SearchHelpBoxPrompt m_SearchPrompt;
        private Texture2D m_FoundIcon;

        public HierarchySearchTab(Action onRepaintWindow)
        {
            m_OnRepaintWindow = onRepaintWindow;
            m_SearchResults = new HashSet<int>();
            m_SearchPrompt = new SearchHelpBoxPrompt();

            m_SearchHandlers = new Dictionary<HierarchySearchType, SearchHandler>();
            m_SearchHandlers.Add(HierarchySearchType.Component, SearchComponentType);
            m_SearchHandlers.Add(HierarchySearchType.FieldName, SearchFieldName);
            m_SearchHandlers.Add(HierarchySearchType.FieldType, SearchFieldType);
            m_SearchHandlers.Add(HierarchySearchType.PropertyName, SearchPropertyName);
            m_SearchHandlers.Add(HierarchySearchType.PropertyType, SearchPropertyType);
            m_SearchHandlers.Add(HierarchySearchType.GameObjectName, SearchGameObjectName);
        }

        public void OnDestroy()
        {
            m_OnRepaintWindow = null;
            m_FoundIcon = null;
            m_SearchWidget.OnDestroy();
            m_SearchWidget = null;
            m_SearchHandlers.Clear();
        }

        public void OnEnable()
        {
            m_FoundIcon = Resources.Load<Texture2D>(string.Format("{0}/{1}", EditorStyles.ThemeFolder, EditorStyles.ICON_NOTIFICATION));
            m_SearchWidget = new SearchWidget<HierarchySearchType>(HierarchySearchType.Component, OnSearch, OnClear);

            EditorApplication.hierarchyWindowItemOnGUI += HierarchyHighlightItem;
        }

        public void OnDisable()
        {
            EditorApplication.hierarchyWindowItemOnGUI -= HierarchyHighlightItem;
        }

        public void OnGUI()
        {
            m_SearchWidget.OnGUI();

            if (!string.IsNullOrEmpty(m_SearchPrompt.message))
            {
                EditorGUILayout.HelpBox(m_SearchPrompt.message, m_SearchPrompt.type);
            }

            if (Event.current.type == EventType.keyUp && Event.current.keyCode == KeyCode.Escape)
            {
                OnClear();
            }
        }

        public void OnGUIEnd()
        {
            m_SearchWidget.OnGUIEnd();
        }

        private void OnSearch(HierarchySearchType type, string term)
        {
            if (m_OnRepaintWindow != null)
            {
                m_OnRepaintWindow();
            }

            m_SearchResults.Clear();
            EditorApplication.RepaintHierarchyWindow();
            if (string.IsNullOrEmpty(term))
            {
                m_SearchPrompt.message = "Search term cannot be empty.";
                m_SearchPrompt.type = MessageType.Error;
                return;
            }

            m_SearchHandlers[type](term, m_SearchWidget.CaseSensitive, m_SearchWidget.IncludeInactive, m_SearchWidget.MatchWholeWord, m_SearchResults);
            if (m_SearchResults.Count == 0)
            {
                //repaint hierarchy
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

            if (m_OnRepaintWindow != null)
            {
                m_OnRepaintWindow();
            }
        }

        private void HierarchyHighlightItem(int instanceId, Rect selectionRect)
        {
            if (m_SearchResults.Contains(instanceId))
            {
                GameObject go = (GameObject)EditorUtility.InstanceIDToObject(instanceId);
                if (go == null)
                {
                    m_SearchResults.Remove(instanceId);
                    return;
                }
                EditorGUI.DrawRect(selectionRect, HierarchySearchSettings.Instance.searchResultBackground);
                EditorGUI.LabelField(selectionRect, go.name, EditorStyles.SearchResult);

                Rect iconRect = selectionRect;
                iconRect.width = 18;
                iconRect.x = 0;
                GUI.Label(iconRect, m_FoundIcon);
            }
        }

        #region Search Methods
        private static void SearchComponentType(string searchTerm, bool caseSensitive, bool includeInactive, bool matchWholeWord, HashSet<int> searchResults)
        {
            List<Type> results = ReflectionHelper.GetTypesByName(searchTerm, caseSensitive, matchWholeWord);
            if (results != null)
            {
                HierarchyHelper.GetGameObjectsWithTypes(results, includeInactive).ForEach(
                go =>
                {
                    int instanceId = go.GetInstanceID();
                    searchResults.Add(instanceId);
                    EditorGUIUtility.PingObject(instanceId);
                });
            }
        }

        private static void SearchFieldName(string searchTerm, bool caseSensitive, bool includeInactive, bool matchWholeWord, HashSet<int> searchResults)
        {
            HierarchyHelper.GetGameObjectsWithFieldName(searchTerm, caseSensitive, includeInactive, matchWholeWord).ForEach(
            go =>
            {
                int instanceId = go.GetInstanceID();
                searchResults.Add(instanceId);
                EditorGUIUtility.PingObject(instanceId);
            });
        }

        private static void SearchFieldType(string searchTerm, bool caseSensitive, bool includeInactive, bool matchWholeWord, HashSet<int> searchResults)
        {
            HierarchyHelper.GetGameObjectsWithFieldType(searchTerm, caseSensitive, includeInactive, matchWholeWord).ForEach(
            go =>
            {
                int instanceId = go.GetInstanceID();
                searchResults.Add(instanceId);
                EditorGUIUtility.PingObject(instanceId);
            });
        }

        private static void SearchPropertyName(string searchTerm, bool caseSensitive, bool includeInactive, bool matchWholeWord, HashSet<int> searchResults)
        {
            HierarchyHelper.GetGameObjectsWithPropertyName(searchTerm, caseSensitive, includeInactive, matchWholeWord).ForEach(
            go =>
            {
                int instanceId = go.GetInstanceID();
                searchResults.Add(instanceId);
                EditorGUIUtility.PingObject(instanceId);
            });
        }

        private static void SearchPropertyType(string searchTerm, bool caseSensitive, bool includeInactive, bool matchWholeWord, HashSet<int> searchResults)
        {
            HierarchyHelper.GetGameObjectsWithPropertyType(searchTerm, caseSensitive, includeInactive, matchWholeWord).ForEach(
            go =>
            {
                int instanceId = go.GetInstanceID();
                searchResults.Add(instanceId);
                EditorGUIUtility.PingObject(instanceId);
            });
        }

        private static void SearchGameObjectName(string searchTerm, bool caseSensitive, bool includeInactive, bool matchWholeWord, HashSet<int> searchResults)
        {
            HierarchyHelper.FindObjectsByName(searchTerm, caseSensitive, includeInactive, matchWholeWord).ForEach(
            go =>
            {
                int instanceId = go.GetInstanceID();
                searchResults.Add(instanceId);
                EditorGUIUtility.PingObject(instanceId);
            });
        }
        #endregion
    }
}
