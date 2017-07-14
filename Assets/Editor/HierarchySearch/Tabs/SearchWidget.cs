using System;
using UnityEditor;
using UnityEngine;

namespace HierarchySearch
{
    public class SearchWidget
    {
        private Texture2D m_SearchIcon;
        
        private Texture2D m_ClearIcon;
        private Action<SearchType, string> m_OnSearch;
        private Action m_OnClear;

        public SearchType Type { get; private set; }
        public string Term { get; private set; }
        public bool CaseSensitive { get; private set; }

        public SearchWidget(SearchType defaultType, Action<SearchType, string> onSearch, Action onClear)
        {
            Type = defaultType;
            Term = string.Empty;
            CaseSensitive = false;
            m_OnSearch = onSearch;
            m_OnClear = onClear;
            
            m_SearchIcon = Resources.Load<Texture2D>(string.Format("{0}/{1}", EditorStyles.ThemeFolder, SearchConstants.ICON_SEARCH));
            m_ClearIcon = Resources.Load<Texture2D>(string.Format("{0}/{1}", EditorStyles.ThemeFolder, SearchConstants.ICON_CLOSE));
            
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
                Type = (SearchType)EditorGUILayout.EnumPopup(Type, GUILayout.Width(100f));
                Term = EditorGUILayout.TextField(Term);

                bool isReturnPressed = Event.current.type == EventType.keyDown && Event.current.keyCode == KeyCode.Return;
                if (EditorStyles.IconButton(m_SearchIcon) || isReturnPressed)
                {
                    if (m_OnSearch != null)
                    {
                        m_OnSearch(Type, Term);
                    }
                }
                else if (EditorStyles.IconButton(m_ClearIcon))
                {
                    Term = string.Empty;
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
    }
}
