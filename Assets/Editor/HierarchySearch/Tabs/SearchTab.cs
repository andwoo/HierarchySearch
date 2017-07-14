﻿using System;
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
    
    public class SearchTab : IWindowTab
    {
        private Dictionary<SearchType, SearchHandler> m_SearchHandlers;
        private HashSet<int> m_SearchResults;
        private SearchWidget m_SearchWidget;
        private SearchHelpBoxPrompt m_SearchPrompt;
        private Texture2D m_FoundIcon;
        private Vector2 m_ScrollPosition;

        public SearchTab()
        {
            m_SearchResults = new HashSet<int>();
            m_SearchPrompt = new SearchHelpBoxPrompt();

            m_SearchHandlers = new Dictionary<SearchType, SearchHandler>();
            m_SearchHandlers.Add(SearchType.Component, SearchComponentType);
            m_SearchHandlers.Add(SearchType.FieldName, SearchFieldName);
            m_SearchHandlers.Add(SearchType.FieldType, SearchFieldType);
            m_SearchHandlers.Add(SearchType.PropertyName, SearchPropertyName);
            m_SearchHandlers.Add(SearchType.PropertyType, SearchPropertyType);
        }

        public void OnDestroy()
        {
            m_FoundIcon = null;
            m_SearchWidget.OnDestroy();
            m_SearchWidget = null;
        }

        public void OnEnable()
        {
            m_FoundIcon = Resources.Load<Texture2D>(string.Format("{0}/{1}", EditorStyles.ThemeFolder, SearchConstants.ICON_NOTIFICATION));
            m_SearchWidget = new SearchWidget(SearchType.Component, OnSearch, OnClear);

            EditorApplication.hierarchyWindowItemOnGUI += HierarchyHighlightItem;
        }

        public void OnDisable()
        {
            EditorApplication.hierarchyWindowItemOnGUI -= HierarchyHighlightItem;
        }

        public void OnGUI()
        {
            m_ScrollPosition = EditorGUILayout.BeginScrollView(m_ScrollPosition);
            m_SearchWidget.OnGUI();
            EditorGUILayout.EndScrollView();

            if(!string.IsNullOrEmpty(m_SearchPrompt.message))
            {
                EditorGUILayout.HelpBox(m_SearchPrompt.message, m_SearchPrompt.type);
            }
        }

        private void OnSearch(SearchType type, string term)
        {
            m_SearchResults.Clear();
            if(string.IsNullOrEmpty(term))
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
