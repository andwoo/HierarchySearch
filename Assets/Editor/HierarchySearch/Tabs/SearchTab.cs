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

    delegate void SearchHandler(string searchTerm, bool caseSensitive, HashSet<int> searchResults);
    
    public class SearchTab : AbstractWindowTab
    {
        private const string ICON_SEARCH = "ic_search";
        private const string ICON_CLOSE = "ic_close";
        private const string ICON_NOTIFICATION = "ic_priority_high";

        private Dictionary<SearchType, SearchHandler> m_SearchHandlers;

        private HashSet<int> m_SearchResults;
        private SearchHelpBoxPrompt m_SearchPrompt;
        private SearchType m_SearchType;
        private string m_SearchTerm;
        private bool m_CaseSensitive;

        private Vector2 m_ScrollPosition;
        private Texture2D m_SearchIcon;
        private Texture2D m_FoundIcon;
        private Texture2D m_ClearIcon;

        public SearchTab()
        {
            m_SearchResults = new HashSet<int>();
            m_SearchPrompt = new SearchHelpBoxPrompt();
            m_SearchType = SearchType.Component;

            m_SearchHandlers = new Dictionary<SearchType, SearchHandler>();
            m_SearchHandlers.Add(SearchType.Component, SearchComponentType);
            m_SearchHandlers.Add(SearchType.FieldName, SearchFieldName);
            m_SearchHandlers.Add(SearchType.FieldType, SearchFieldType);
            m_SearchHandlers.Add(SearchType.PropertyName, SearchPropertyName);
            m_SearchHandlers.Add(SearchType.PropertyType, SearchPropertyType);
        }

        public override void OnDestroy()
        {
            m_SearchIcon = null;
            m_FoundIcon = null;
            m_ClearIcon = null;
        }

        public override void OnEnable()
        {
            string themeFolder = EditorGUIUtility.isProSkin ? "ProTheme" : "DefaultTheme";
            m_SearchIcon = Resources.Load<Texture2D>(string.Format("{0}/{1}", themeFolder, SearchConstants.ICON_SEARCH));
            m_ClearIcon = Resources.Load<Texture2D>(string.Format("{0}/{1}", themeFolder, SearchConstants.ICON_CLOSE));
            m_FoundIcon = Resources.Load<Texture2D>(string.Format("{0}/{1}", themeFolder, SearchConstants.ICON_NOTIFICATION));
            EditorApplication.hierarchyWindowItemOnGUI += HierarchyHighlightItem;
        }

        public override void OnDisable()
        {
            EditorApplication.hierarchyWindowItemOnGUI -= HierarchyHighlightItem;
            m_SearchIcon = null;
            m_FoundIcon = null;
            m_ClearIcon = null;
        }

        public override void OnGUI()
        {
            m_ScrollPosition = EditorGUILayout.BeginScrollView(m_ScrollPosition);

            EditorGUILayout.BeginHorizontal();
            {
                m_SearchType = (SearchType)EditorGUILayout.EnumPopup(m_SearchType, GUILayout.Width(100f));
                m_SearchTerm = EditorGUILayout.TextField(m_SearchTerm);

                bool isReturnPressed = Event.current.type == EventType.keyDown && Event.current.keyCode == KeyCode.Return;
                if (EditorStyles.IconButton(m_SearchIcon) || isReturnPressed)
                {
                    m_SearchResults.Clear();
                    if(string.IsNullOrEmpty(m_SearchTerm))
                    {
                        m_SearchPrompt.message = "Search term cannot be empty.";
                        m_SearchPrompt.type = MessageType.Error;
                    }
                    else
                    {
                        m_SearchHandlers[m_SearchType](m_SearchTerm, m_CaseSensitive, m_SearchResults);
                        if (m_SearchResults.Count == 0)
                        {
                            m_SearchPrompt.message = string.Format("Could not find match for \"{0}\"", m_SearchTerm);
                            m_SearchPrompt.type = MessageType.Info;
                        }
                        else
                        {
                            m_SearchPrompt.message = string.Empty;
                        }
                    }
                }
                else if (EditorStyles.IconButton(m_ClearIcon))
                {
                    m_SearchResults.Clear();
                    m_SearchTerm = string.Empty;
                    m_SearchPrompt.message = string.Empty;
                    GUI.FocusControl(null);
                    EditorApplication.RepaintHierarchyWindow();
                }
            }
            EditorGUILayout.EndHorizontal();
            m_CaseSensitive = EditorGUILayout.Toggle("Match case", m_CaseSensitive);

            EditorGUILayout.EndScrollView();

            if(!string.IsNullOrEmpty(m_SearchPrompt.message))
            {
                EditorGUILayout.HelpBox(m_SearchPrompt.message, m_SearchPrompt.type);
            }
        }

        private void HierarchyHighlightItem(int instanceId, Rect selectionRect)
        {
            if (m_SearchResults.Contains(instanceId))
            {
                GameObject go = (GameObject)EditorUtility.InstanceIDToObject(instanceId);
                EditorGUI.DrawRect(selectionRect, HierarchySearchSettings.Instance.searchResultBackground);
                EditorGUI.LabelField(selectionRect, go.name, EditorStyles.SearchResult);

                Rect iconRect = selectionRect;
                iconRect.width = 18;
                iconRect.x = 0;
                GUI.Label(iconRect, m_FoundIcon);
            }
        }

#region Search Methods
        private static void SearchComponentType(string searchTerm, bool caseSensitive, HashSet<int> searchResults)
        {
            Type result = ReflectionHelper.GetTypeByName(searchTerm, caseSensitive);
            if (result != null)
            {
                HierarchyHelper.GetGameObjectsWithType(result).ForEach(
                go => {
                    int instanceId = go.GetInstanceID();
                    searchResults.Add(instanceId);
                    EditorGUIUtility.PingObject(instanceId);
                });
            }
        }

        private static void SearchFieldName(string searchTerm, bool caseSensitive, HashSet<int> searchResults)
        {
            HierarchyHelper.GetGameObjectsWithFieldName(searchTerm, caseSensitive).ForEach(
            go =>
            {
                int instanceId = go.GetInstanceID();
                searchResults.Add(instanceId);
                EditorGUIUtility.PingObject(instanceId);
            });
        }

        private static void SearchFieldType(string searchTerm, bool caseSensitive, HashSet<int> searchResults)
        {
            HierarchyHelper.GetGameObjectsWithFieldType(searchTerm, caseSensitive).ForEach(
            go =>
            {
                int instanceId = go.GetInstanceID();
                searchResults.Add(instanceId);
                EditorGUIUtility.PingObject(instanceId);
            });
        }

        private static void SearchPropertyName(string searchTerm, bool caseSensitive, HashSet<int> searchResults)
        {
            HierarchyHelper.GetGameObjectsWithPropertyName(searchTerm, caseSensitive).ForEach(
            go =>
            {
                int instanceId = go.GetInstanceID();
                searchResults.Add(instanceId);
                EditorGUIUtility.PingObject(instanceId);
            });
        }

        private static void SearchPropertyType(string searchTerm, bool caseSensitive, HashSet<int> searchResults)
        {
            HierarchyHelper.GetGameObjectsWithPropertyType(searchTerm, caseSensitive).ForEach(
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
