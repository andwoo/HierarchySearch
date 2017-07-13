using System.Collections.Generic;
using System.IO;
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
                DrawPrefabUI(prefabPath);
            }
            EditorGUILayout.EndScrollView();
        }

        private void DrawPrefabUI(string prefabPath)
        {
            if (GUILayout.Button(Path.GetFileNameWithoutExtension(prefabPath)))
            {
                EditorGUIUtility.PingObject(AssetDatabase.LoadAssetAtPath<Object>(prefabPath));
            }
        }
    }
}
