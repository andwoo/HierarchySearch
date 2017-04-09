using UnityEditor;
using UnityEditor.iOS;
using UnityEngine;

namespace HierarchySearch
{
    public static class EditorStyles
    {
        #region Layouts
        public static GUILayoutOption m_SmallButtonWidth;
        public static GUILayoutOption SmallButtonWidth
        {
            get
            {
                if (m_SmallButtonWidth == null)
                {
                    m_SmallButtonWidth = GUILayout.Width(20f);
                }
                return m_SmallButtonWidth;
            }
        }

        public static GUILayoutOption m_MediumButtonWidth;
        public static GUILayoutOption MediumButtonWidth
        {
            get
            {
                if (m_MediumButtonWidth == null)
                {
                    m_MediumButtonWidth = GUILayout.Width(50f);
                }
                return m_MediumButtonWidth;
            }
        }

        public static GUILayoutOption m_LargeButtonWidth;
        public static GUILayoutOption LargeButtonWidth
        {
            get
            {
                if (m_LargeButtonWidth == null)
                {
                    m_LargeButtonWidth = GUILayout.Width(75f);
                }
                return m_LargeButtonWidth;
            }
        }
        #endregion

        #region Styles
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
        #endregion
    }
}