using System.Linq;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace HierarchySearch
{
    delegate void SearchHandler(string searchTerm, bool caseSensitive, HashSet<int> searchResults);

    public class SearchWindow : EditorWindow
    {
        [MenuItem("GameObject/Hierarchy Search %#f", false, 200)]
        private static void ShowSearchWindow()
        {
            SearchWindow window = (SearchWindow)EditorWindow.GetWindow(typeof(SearchWindow));
            window.Show();
        }

        private Dictionary<string, IWindowTab> m_Tabs;
        private string[] m_TabNames;
        private int m_CurrentTab;

        public SearchWindow()
        {
            m_Tabs = new Dictionary<string, IWindowTab>();
            m_Tabs.Add("Hierarchy", new HierarchySearchTab());
            m_Tabs.Add("Prefab", new PrefabSearchTab());
            m_Tabs.Add("Settings", new SettingsTab());
            m_TabNames = m_Tabs.Keys.ToArray();
        }

        private void OnEnable()
        {
            EditorStyles.Initialize();

            Texture2D windowIcon = Resources.Load<Texture2D>(string.Format("{0}/{1}", EditorStyles.ThemeFolder, EditorStyles.ICON_LOGO));
            this.titleContent = new GUIContent("Search", windowIcon);

            foreach (var kvp in m_Tabs)
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
            EditorGUI.BeginChangeCheck();
            m_CurrentTab = GUILayout.Toolbar(m_CurrentTab, m_TabNames);
            if(EditorGUI.EndChangeCheck())
            {
                GUI.FocusControl(null);
            }
            m_Tabs[m_TabNames[m_CurrentTab]].OnGUI();
        }
    }
}