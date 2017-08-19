using UnityEngine;
using UnityEditor;

namespace HierarchySearch
{
    public class HierarchySearchSettings : ScriptableObject
    {
#region ScriptableObject Saving
        private const string SETTINGS_LOCATION = "Assets/Editor/HierarchySearch/ScriptableObjects/HierarchySearchSettings.asset";
        private static HierarchySearchSettings m_Instance;
        public static HierarchySearchSettings Instance
        {
            get
            {
                if(m_Instance == null)
                {
                    m_Instance = LoadResource();
                }
                return m_Instance;
            }
        }

        public void Save()
        {
            EditorUtility.SetDirty(this);
            AssetDatabase.SaveAssets();
        }

        private static HierarchySearchSettings LoadResource()
        {
            HierarchySearchSettings asset = AssetDatabase.LoadAssetAtPath<HierarchySearchSettings>(SETTINGS_LOCATION);
            if(asset == null)
            {
                asset = ScriptableObject.CreateInstance<HierarchySearchSettings>();
                AssetDatabase.CreateAsset(asset, SETTINGS_LOCATION);
            }
            return asset;
        }
        #endregion

        [SerializeField]
        public Color searchResultBackground = EditorStyles.Orange;
        [SerializeField]
        public Color searchResultText = EditorStyles.Yellow;
    }
}
