using System;
using UnityEditor;
using UnityEngine;

namespace HierarchySearch
{
    public class SearchWidget<TSearchType> where TSearchType : struct
    {
        private Texture2D m_SearchIcon;
        private Texture2D m_ClearIcon;
        private Action<TSearchType, string> m_OnSearch;
        private Action m_OnClear;

        public TSearchType SearchType { get; private set; }
        public string SearchTerm { get; private set; }
        public bool CaseSensitive { get; private set; }

        public SearchWidget(TSearchType defaultType, Action<TSearchType, string> onSearch, Action onClear)
        {
            SearchType = defaultType;
            SearchTerm = string.Empty;
            CaseSensitive = false;
            m_OnSearch = onSearch;
            m_OnClear = onClear;
            
            m_SearchIcon = Resources.Load<Texture2D>(string.Format("{0}/{1}", EditorStyles.ThemeFolder, EditorStyles.ICON_SEARCH));
            m_ClearIcon = Resources.Load<Texture2D>(string.Format("{0}/{1}", EditorStyles.ThemeFolder, EditorStyles.ICON_CLOSE));
        }

        public void OnDestroy()
        {
            m_SearchIcon = null;
            m_ClearIcon = null;
            m_OnSearch = null;
            m_OnClear = null;
        }

        public void OnGUI()
        {
            EditorGUILayout.BeginHorizontal();
            {
                SearchType = (TSearchType)(object)EditorGUILayout.EnumPopup((object)SearchType as Enum, GUILayout.Width(100f));
                SearchTerm = EditorGUILayout.TextField(SearchTerm);
                
                if (EditorStyles.GetIconButton(m_SearchIcon))
                {
                    if (m_OnSearch != null)
                    {
                        m_OnSearch(SearchType, SearchTerm);
                    }
                }
                else if (EditorStyles.GetIconButton(m_ClearIcon))
                {
                    SearchTerm = string.Empty;
                    GUI.FocusControl(null);
                    if(m_OnClear != null)
                    {
                        m_OnClear();
                    }

                    EditorApplication.RepaintHierarchyWindow();
                }
            }
            EditorGUILayout.EndHorizontal();

            CaseSensitive = EditorGUILayout.Toggle("Match case", CaseSensitive);
        }

        public void OnGUIEnd()
        {
            bool isReturnPressed = Event.current.type == EventType.KeyUp && Event.current.keyCode == KeyCode.Return;
            if (isReturnPressed && m_OnSearch != null)
            {
                m_OnSearch(SearchType, SearchTerm);
                GUI.FocusControl(null);
                return;
            }

            bool isEscapepressed = Event.current.type == EventType.KeyUp && Event.current.keyCode == KeyCode.Escape;
            if (isEscapepressed && m_OnClear != null)
            {
                SearchTerm = string.Empty;
                m_OnClear();
                GUI.FocusControl(null);
                return;
            }
        }
    }
}
