using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace HierarchySearch
{
    public class PrefabTab : AbstractWindowTab
    {
        private List<string> m_Prefabs;
        private Vector2 m_ScrollPosition;

        public override void OnEnable()
        {
            m_Prefabs = AssetDatabaseHelper.GetAllPrefabPaths();
        }

        public override void OnDisable()
        {

        }

        public override void OnDestroy()
        {

        }

        public override void OnGUI()
        {
            m_ScrollPosition = EditorGUILayout.BeginScrollView(m_ScrollPosition);
            foreach(string prefabPath in m_Prefabs)
            {
                EditorGUILayout.LabelField(prefabPath);
            }
            EditorGUILayout.EndScrollView();
        }
    }
}
