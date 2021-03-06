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
            m_SearchHandlers.Add(HierarchySearchType.Layer, SearchLayerName);
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
            m_SearchWidget = new SearchWidget<HierarchySearchType>(OnSearch, OnClear, Save);
            m_SearchWidget.SetState(
                EditorPrefsUtils.LoadEnum<HierarchySearchType>(EditorPrefKeys.KEY_SEARCH_TYPE, HierarchySearchType.Component),
                EditorPrefsUtils.LoadString(EditorPrefKeys.KEY_SEARCH_TERM, string.Empty),
                EditorPrefsUtils.LoadBool(EditorPrefKeys.KEY_CASE_SENSITIVE, false),
                EditorPrefsUtils.LoadBool(EditorPrefKeys.KEY_MATCH_WHOLE_WORD, false),
                EditorPrefsUtils.LoadBool(EditorPrefKeys.KEY_INCLUDE_INACTIVE, true));

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

            if (Event.current.type == EventType.KeyUp && Event.current.keyCode == KeyCode.Escape)
            {
                OnClear();
            }
        }

        public void OnGUIEnd()
        {
            m_SearchWidget.OnGUIEnd();
        }

        private void Save()
        {
            EditorPrefsUtils.SaveEnum<HierarchySearchType>(EditorPrefKeys.KEY_SEARCH_TYPE, m_SearchWidget.SearchType);
            EditorPrefsUtils.SaveString(EditorPrefKeys.KEY_SEARCH_TERM, m_SearchWidget.SearchTerm);
            EditorPrefsUtils.SaveBool(EditorPrefKeys.KEY_CASE_SENSITIVE, m_SearchWidget.CaseSensitive);
            EditorPrefsUtils.SaveBool(EditorPrefKeys.KEY_MATCH_WHOLE_WORD, m_SearchWidget.MatchWholeWord);
            EditorPrefsUtils.SaveBool(EditorPrefKeys.KEY_INCLUDE_INACTIVE, m_SearchWidget.IncludeInactive);
        }

        private void OnSearch(HierarchySearchType type, string term)
        {
            if (m_OnRepaintWindow != null)
            {
                m_OnRepaintWindow();
            }

            Save();
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
                if (type == HierarchySearchType.Layer)
                {
                    m_SearchPrompt.message = string.Format("Could not find match for \"{0}\". The layer name must be exact and is case sensitive.", term);
                }
                else
                {
                    m_SearchPrompt.message = string.Format("Could not find match for \"{0}\"", term);
                }
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

            Save();

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
                EditorGUI.DrawRect(selectionRect, EditorStyles.BackgroundColor);
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
        
        private static void SearchLayerName(string searchTerm, bool caseSensitive, bool includeInactive, bool matchWholeWord, HashSet<int> searchResults)
        {
            int targetLayer = LayerMask.NameToLayer(searchTerm);
            HierarchyHelper.GetGameObjectsWithLayer(targetLayer, includeInactive).ForEach(
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
