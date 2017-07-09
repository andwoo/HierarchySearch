using System;
using UnityEditor;
using UnityEngine;

namespace HierarchySearch
{
    public class SettingsTab : AbstractWindowTab
    {
        private Color searchResultBackground = EditorStyles.Orange;
        private Color searchResultText = EditorStyles.Yellow;

        public override void OnDestroy()
        {
        }

        public override void OnDisable()
        {
        }

        public override void OnEnable()
        {
            searchResultBackground = HierarchySearchSettings.Instance.searchResultBackground;
            searchResultText = HierarchySearchSettings.Instance.searchResultText;
        }

        public override void OnGUI()
        {
            searchResultBackground = EditorGUILayout.ColorField("Background Color", searchResultBackground);
            searchResultText = EditorGUILayout.ColorField("Text Color", searchResultText);

            if(GUILayout.Button("Save"))
            {
                Save();
            }
        }

        private void Save()
        {
            HierarchySearchSettings.Instance.Save();
            EditorStyles.Reset();
        }
    }
}
