using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace HierarchySearch
{
    public class SearchWindow : EditorWindow
    {
        [MenuItem("GameObject/HierarchySearch", false, 200)]
        private static void ShowSearchWindow()
        {
            SearchWindow window = (SearchWindow)EditorWindow.GetWindow(typeof(SearchWindow));
            window.Show();
        }

        private readonly string[] m_Tabs = new string[] { "Basic", "Advanced" };
        private int m_CurrentTab;
        private HashSet<int> m_SearchResults;

        //basic search
        private string m_DefaultComponentName;
        private string m_DefaultFieldName;
        public SearchWindow()
        {
            this.titleContent = new GUIContent("Search");
            m_SearchResults = new HashSet<int>();
        }

        private void OnEnable()
        {
            EditorApplication.hierarchyWindowItemOnGUI += HierarchyDrawItem;
        }

        private void OnDisable()
        {
            EditorApplication.hierarchyWindowItemOnGUI -= HierarchyDrawItem;
        }

        private void OnGUI()
        {
            m_CurrentTab = GUILayout.Toolbar(m_CurrentTab, m_Tabs);
            switch (m_CurrentTab)
            {
                default:
                    DrawDefaultTab();
                    break;
                case 1:
                    DrawAdvancedTab();
                    break;
            }
        }

        private void HierarchyDrawItem(int instanceId, Rect selectionRect)
        {
            if (m_SearchResults.Contains(instanceId))
            {
                EditorGUI.DrawRect(selectionRect, Color.green);
            }
        }

        private void DrawDefaultTab()
        {
            EditorGUILayout.BeginVertical(GUI.skin.GetStyle("box"));
            EditorGUILayout.LabelField("Component Name", EditorStyles.Header);
            EditorGUILayout.BeginHorizontal();
            m_DefaultComponentName = EditorGUILayout.TextField(m_DefaultComponentName);
            if (GUILayout.Button("X", EditorStyles.SmallButtonWidth))
            {
                m_SearchResults.Clear();
                m_DefaultComponentName = string.Empty;
                GUI.FocusControl(null);
                EditorApplication.RepaintHierarchyWindow();
            }
            else if (GUILayout.Button("Search", EditorStyles.MediumButtonWidth))
            {
                m_SearchResults.Clear();
                Type result = ReflectionHelper.GetTypeByName(m_DefaultComponentName, false);
                if (result != null)
                {
                    HierarchyHelper.GetGameObjectsWithType(result).ForEach(go => m_SearchResults.Add(go.GetInstanceID()));
                }
                EditorApplication.RepaintHierarchyWindow();
            }
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.EndVertical();
        }

        private void DrawAdvancedTab()
        {
            EditorGUILayout.LabelField("Advanced");
        }
    }
}