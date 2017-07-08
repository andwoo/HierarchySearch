using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace HierarchySearch
{
    internal class SearchTermVO
    {
        public string term;
        public bool enableCaseSensitive;
        public bool caseSensitive;

        public SearchTermVO(bool enableCaseSensitive)
        {
            this.enableCaseSensitive = enableCaseSensitive;
            term = string.Empty;
        }
    }

    public class SearchTab : AbstractWindowTab
    {
        private HashSet<int> m_SearchResults;
        //component searching
        private bool m_ComponentFoldOut;
        private SearchTermVO m_ComponentType;
        //field searching
        private bool m_FieldFoldOut;
        private SearchTermVO m_FieldName;
        private SearchTermVO m_FieldType;
        //property searching
        private bool m_PropertyFoldOut;
        private SearchTermVO m_PropertyName;
        private SearchTermVO m_PropertyType;

        private Vector2 m_ScrollPosition;

        private Texture2D m_SearchIcon;
        private Texture2D m_FoundIcon;
        private Texture2D m_ClearIcon;

        public SearchTab()
        {
            m_SearchResults = new HashSet<int>();
            m_ComponentType = new SearchTermVO(true);
            m_FieldName = new SearchTermVO(true);
            m_FieldType = new SearchTermVO(true);
            m_PropertyName = new SearchTermVO(true);
            m_PropertyType = new SearchTermVO(true);
        }

        public override void OnDestroy()
        {
            m_SearchIcon = null;
            m_FoundIcon = null;
            m_ClearIcon = null;
        }

        public override void OnEnable()
        {
            m_SearchIcon = Resources.Load<Texture2D>("ic_search");
            m_FoundIcon = Resources.Load<Texture2D>("ic_priority_high");
            m_ClearIcon = Resources.Load<Texture2D>("ic_close");
            EditorApplication.hierarchyWindowItemOnGUI += HierarchyDrawItem;
        }

        public override void OnDisable()
        {
            EditorApplication.hierarchyWindowItemOnGUI -= HierarchyDrawItem;
            m_SearchIcon = null;
            m_FoundIcon = null;
            m_ClearIcon = null;
        }

        public override void OnGUI()
        {
            m_ScrollPosition = EditorGUILayout.BeginScrollView(m_ScrollPosition);
            
            m_ComponentFoldOut = EditorGUILayout.Foldout(m_ComponentFoldOut, "Component Search");
            if(m_ComponentFoldOut)
            {
                DrawSearchField("Component Type", m_ComponentType, m_SearchResults, m_SearchIcon, m_ClearIcon, SearchComponentType);
            }
            
            m_FieldFoldOut = EditorGUILayout.Foldout(m_FieldFoldOut, "Field Search");
            if(m_FieldFoldOut)
            {
                DrawSearchField("Field Name", m_FieldName, m_SearchResults, m_SearchIcon, m_ClearIcon, SearchFieldName);
                DrawSearchField("Field Type", m_FieldType, m_SearchResults, m_SearchIcon, m_ClearIcon, SearchFieldType);
            }

            m_PropertyFoldOut = EditorGUILayout.Foldout(m_PropertyFoldOut, "Property Search");
            if (m_PropertyFoldOut)
            {
                DrawSearchField("Property Name", m_PropertyName, m_SearchResults, m_SearchIcon, m_ClearIcon, SearchPropertyName);
                DrawSearchField("Property Type", m_PropertyType, m_SearchResults, m_SearchIcon, m_ClearIcon, SearchPropertyType);
            }

            EditorGUILayout.EndScrollView();
        }

        private void HierarchyDrawItem(int instanceId, Rect selectionRect)
        {
            if (m_SearchResults.Contains(instanceId))
            {
                GameObject go = (GameObject)EditorUtility.InstanceIDToObject(instanceId);
                EditorGUI.DrawRect(selectionRect, EditorStyles.Orange);
                EditorGUI.LabelField(selectionRect, go.name, EditorStyles.SearchResult);

                Rect iconRect = selectionRect;
                iconRect.width = 18;
                iconRect.x = 0;
                GUI.Label(iconRect, m_FoundIcon);
            }
        }

        private static void SearchComponentType(SearchTermVO searchTerm, HashSet<int> searchResults)
        {
            Type result = ReflectionHelper.GetTypeByName(searchTerm.term, searchTerm.enableCaseSensitive ? searchTerm.caseSensitive : false);
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

        private static void SearchFieldName(SearchTermVO searchTerm, HashSet<int> searchResults)
        {
            HierarchyHelper.GetGameObjectsWithFieldName(searchTerm.term, searchTerm.enableCaseSensitive ? searchTerm.caseSensitive : false).ForEach(
            go =>
            {
                int instanceId = go.GetInstanceID();
                searchResults.Add(instanceId);
                EditorGUIUtility.PingObject(instanceId);
            });
        }

        private static void SearchFieldType(SearchTermVO searchTerm, HashSet<int> searchResults)
        {
            HierarchyHelper.GetGameObjectsWithFieldType(searchTerm.term, searchTerm.enableCaseSensitive ? searchTerm.caseSensitive : false).ForEach(
            go =>
            {
                int instanceId = go.GetInstanceID();
                searchResults.Add(instanceId);
                EditorGUIUtility.PingObject(instanceId);
            });
        }

        private static void SearchPropertyName(SearchTermVO searchTerm, HashSet<int> searchResults)
        {
            HierarchyHelper.GetGameObjectsWithPropertyName(searchTerm.term, searchTerm.enableCaseSensitive ? searchTerm.caseSensitive : false).ForEach(
            go =>
            {
                int instanceId = go.GetInstanceID();
                searchResults.Add(instanceId);
                EditorGUIUtility.PingObject(instanceId);
            });
        }

        private static void SearchPropertyType(SearchTermVO searchTerm, HashSet<int> searchResults)
        {
            HierarchyHelper.GetGameObjectsWithPropertyType(searchTerm.term, searchTerm.enableCaseSensitive ? searchTerm.caseSensitive : false).ForEach(
            go =>
            {
                int instanceId = go.GetInstanceID();
                searchResults.Add(instanceId);
                EditorGUIUtility.PingObject(instanceId);
            });
        }


        private static void DrawSearchField(string title, SearchTermVO searchTerm, HashSet<int> gameObjectResult, Texture2D searchIcon, Texture2D clearIcon, Action<SearchTermVO, HashSet<int>> OnSearch)
        {
            EditorGUILayout.BeginVertical(GUI.skin.GetStyle("box"));
            EditorGUILayout.LabelField(title /*, EditorStyles.Header*/);
            EditorGUILayout.BeginHorizontal();
            searchTerm.term = EditorGUILayout.TextField(searchTerm.term);

            if (EditorStyles.IconButton(searchIcon))
            {
                gameObjectResult.Clear();
                if(!string.IsNullOrEmpty(searchTerm.term))
                {
                    OnSearch(searchTerm, gameObjectResult);
                }
                EditorApplication.RepaintHierarchyWindow();
            }
            else if (EditorStyles.IconButton(clearIcon))
            {
                gameObjectResult.Clear();
                searchTerm.term = string.Empty;
                GUI.FocusControl(null);
                EditorApplication.RepaintHierarchyWindow();
            }
            
            EditorGUILayout.EndHorizontal();
            if (searchTerm.enableCaseSensitive)
            {
                searchTerm.caseSensitive = EditorGUILayout.Toggle("Match case", searchTerm.caseSensitive);
            }
            else
            {
                EditorGUILayout.Space();
            }

            EditorGUILayout.EndVertical();
        }
    }
}
