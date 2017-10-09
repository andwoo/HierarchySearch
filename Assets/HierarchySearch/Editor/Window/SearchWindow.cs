//#define ENABLE_PREFAB_SEARCH //experimental feature

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
        private int m_CurrentTabId;

        public SearchWindow()
        {
        }

        private void OnEnable()
        {
            CreateWindowTabs();
            EditorStyles.Initialize();
            
            Texture2D windowIcon = Resources.Load<Texture2D>(string.Format("{0}/{1}", EditorStyles.ThemeFolder, EditorStyles.ICON_SEARCH));
            this.titleContent = new GUIContent("Search", windowIcon);

            foreach (var kvp in m_Tabs)
            {
                kvp.Value.OnEnable();
            }
        }

        private void CreateWindowTabs()
        {
            m_Tabs = new Dictionary<string, IWindowTab>();
            m_Tabs.Add("Hierarchy", new HierarchySearchTab(ForceWindowRepaint));
#if ENABLE_PREFAB_SEARCH
            m_Tabs.Add("Prefab", new PrefabSearchTab(ForceWindowRepaint));
#endif
            m_Tabs.Add("Settings", new SettingsTab());
            m_TabNames = m_Tabs.Keys.ToArray();
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
            m_CurrentTabId = GUILayout.Toolbar(m_CurrentTabId, m_TabNames);
            if(m_CurrentTabId >= m_Tabs.Count)
            {
                m_CurrentTabId = 0;
            }
            IWindowTab currentTab = m_Tabs[m_TabNames[m_CurrentTabId]];
            if (EditorGUI.EndChangeCheck())
            {
                GUI.FocusControl(null);
            }

            currentTab.OnGUI();
            currentTab.OnGUIEnd();
        }

        private void ForceWindowRepaint()
        {
            Repaint();
        }
    }
}