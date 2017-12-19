using System;
using UnityEditor;
using UnityEngine;

namespace HierarchySearch
{
    public class SettingsTab : IWindowTab
    {
        private const float BANNER_HEIGHT = 80f;
        private const string VERSION = "1.0.0";
        
        private Texture2D m_Banner;
        private Color searchResultBackground;
        private Color searchResultText;

        public void OnDestroy()
        {
        }

        public void OnDisable()
        {
        }

        public void OnEnable()
        {
            m_Banner = Resources.Load<Texture2D>(string.Format("{0}/{1}", EditorStyles.ThemeFolder, EditorStyles.BANNER_LOGO));
            
            searchResultBackground = EditorPrefsUtils.LoadColor(EditorPrefKeys.KEY_BACKGROUND_COLOR, EditorStyles.DefaultBackgroundColor);
            searchResultText = EditorPrefsUtils.LoadColor(EditorPrefKeys.KEY_TEXT_COLOR, EditorStyles.DefaultTextColor);
        }

        public void OnGUI()
        {
            searchResultBackground = EditorGUILayout.ColorField("Background Color", searchResultBackground);
            searchResultText = EditorGUILayout.ColorField("Text Color", searchResultText);

            if (GUILayout.Button("Save"))
            {
                Save();
            }

            if (GUILayout.Button("Reset"))
            {
                Reset();
            }

            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Version: " + VERSION);
            GUILayout.Label(m_Banner, GUILayout.Height(BANNER_HEIGHT));
        }

        public void OnGUIEnd()
        {
        }

        private void Save()
        {
            EditorPrefsUtils.SaveColor(EditorPrefKeys.KEY_BACKGROUND_COLOR, searchResultBackground);
            EditorPrefsUtils.SaveColor(EditorPrefKeys.KEY_TEXT_COLOR, searchResultText);

            EditorStyles.Reset();
            EditorApplication.RepaintHierarchyWindow();
        }

        private void Reset()
        {
            searchResultBackground = EditorStyles.DefaultBackgroundColor;
            searchResultText = EditorStyles.DefaultTextColor;

            Save();
        }
    }
}
