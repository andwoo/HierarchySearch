using System;
using UnityEditor;
using UnityEngine;

namespace HierarchySearch
{
    public class SettingsTab : IWindowTab
    {
        private Texture2D m_Banner;
        private Color searchResultBackground = EditorStyles.Orange;
        private Color searchResultText = EditorStyles.Yellow;

        public void OnDestroy()
        {
        }

        public void OnDisable()
        {
        }

        public void OnEnable()
        {
            m_Banner = Resources.Load<Texture2D>(string.Format("{0}/{1}", EditorStyles.ThemeFolder, SearchConstants.BANNER_LOGO));
            searchResultBackground = HierarchySearchSettings.Instance.searchResultBackground;
            searchResultText = HierarchySearchSettings.Instance.searchResultText;
        }

        public void OnGUI()
        {
            searchResultBackground = EditorGUILayout.ColorField("Background Color", searchResultBackground);
            searchResultText = EditorGUILayout.ColorField("Text Color", searchResultText);

            if(GUILayout.Button("Save"))
            {
                Save();
            }

            GUILayout.Label(m_Banner);
        }

        private void Save()
        {
            HierarchySearchSettings.Instance.searchResultBackground = searchResultBackground;
            HierarchySearchSettings.Instance.searchResultText = searchResultText;
            
            HierarchySearchSettings.Instance.Save();
            EditorStyles.Reset();

            EditorApplication.RepaintHierarchyWindow();
        }
    }
}
