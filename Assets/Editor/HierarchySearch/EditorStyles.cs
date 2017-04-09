using UnityEngine;

namespace HierarchySearch
{
    public static class EditorStyles
    {
        private static GUIStyle m_Header;

        public static GUIStyle Header
        {
            get
            {
                if (m_Header == null)
                {
                    m_Header = GUI.skin.label;
                    m_Header.fontStyle = FontStyle.Bold;
                    m_Header.normal.textColor = Color.white;
                }
                return m_Header;
            }
        }
    }
}