using System.Linq;
using System.Collections.Generic;
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

        private Dictionary<string, AbstractWindowTab> m_Tabs;
        private string[] m_TabNames;
        private int m_CurrentTab;

        public SearchWindow()
        {
            this.titleContent = new GUIContent("Search");
            m_Tabs = new Dictionary<string, AbstractWindowTab>();
            m_Tabs.Add("Search", new SearchTab());
            m_Tabs.Add("Settings", new SettingsTab());
            m_TabNames = m_Tabs.Keys.ToArray();
        }

        private void OnEnable()
        {
            foreach(var kvp in m_Tabs)
            {
                kvp.Value.OnEnable();
            }
        }

        private void OnDisable()
        {
            foreach (var kvp in m_Tabs)
            {
                kvp.Value.OnDisable();
            }
        }

        private void OnDestroy()
        {
            foreach (var kvp in m_Tabs)
            {
                kvp.Value.OnDestroy();
            }
            m_Tabs.Clear();
        }

        private void OnGUI()
        {
            m_CurrentTab = GUILayout.Toolbar(m_CurrentTab, m_TabNames);
            m_Tabs[m_TabNames[m_CurrentTab]].OnGUI();
        }
    }
}