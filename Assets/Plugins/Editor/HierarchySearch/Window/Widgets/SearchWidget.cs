﻿using System;
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
        private Action m_OnStateChanged;

        public TSearchType SearchType { get; private set; }
        public string SearchTerm { get; private set; }
        private ButtonToggleWidget m_CaseSensitiveToggle;
        private ButtonToggleWidget m_MatchWholeWordToggle;
        private ButtonToggleWidget m_IncludeInactiveToggle;
        public bool CaseSensitive { get { return m_CaseSensitiveToggle.IsOn; } }
        public bool MatchWholeWord { get { return m_MatchWholeWordToggle.IsOn; } }
        public bool IncludeInactive { get { return m_IncludeInactiveToggle.IsOn; } }

        public SearchWidget(Action<TSearchType, string> onSearch, Action onClear, Action onStateChanged)
        {
            SearchType = default(TSearchType);
            SearchTerm = string.Empty;
            m_CaseSensitiveToggle = new ButtonToggleWidget(string.Format("{0}/{1}", EditorStyles.ThemeFolder, EditorStyles.ICON_MATCH_CASE), "Case sensitive");
            m_MatchWholeWordToggle = new ButtonToggleWidget(string.Format("{0}/{1}", EditorStyles.ThemeFolder, EditorStyles.ICON_MATCH_WHOLE_WORD), "Match whole word");
            m_IncludeInactiveToggle = new ButtonToggleWidget(string.Format("{0}/{1}", EditorStyles.ThemeFolder, EditorStyles.ICON_INACTIVE), "Include inactive GameObjects");
            m_CaseSensitiveToggle.OnStateChanged += OnStateUpdate;
            m_MatchWholeWordToggle.OnStateChanged += OnStateUpdate;
            m_IncludeInactiveToggle.OnStateChanged += OnStateUpdate;

            m_OnSearch = onSearch;
            m_OnClear = onClear;
            m_OnStateChanged = onStateChanged;

            m_SearchIcon = Resources.Load<Texture2D>(string.Format("{0}/{1}", EditorStyles.ThemeFolder, EditorStyles.ICON_SEARCH));
            m_ClearIcon = Resources.Load<Texture2D>(string.Format("{0}/{1}", EditorStyles.ThemeFolder, EditorStyles.ICON_CLOSE));
        }

        public void SetState(TSearchType searchType, string searchTerm, bool caseSensitive, bool matchWholeWord, bool includeInactive)
        {
            SearchType = searchType;
            SearchTerm = searchTerm;
            m_CaseSensitiveToggle.IsOn = caseSensitive;
            m_MatchWholeWordToggle.IsOn = matchWholeWord;
            m_IncludeInactiveToggle.IsOn = includeInactive;
        }

        private void OnStateUpdate()
        {
            if(m_OnStateChanged != null)
            {
                m_OnStateChanged();
            }
        }

        public void OnDestroy()
        {
            m_SearchIcon = null;
            m_ClearIcon = null;
            m_OnSearch = null;
            m_OnClear = null;
            m_OnStateChanged = null;
            m_CaseSensitiveToggle.OnDestroy();
            m_MatchWholeWordToggle.OnDestroy();
            m_IncludeInactiveToggle.OnDestroy();
        }

        public void OnGUI()
        {
            EditorGUILayout.BeginHorizontal();
            {
                EditorGUI.BeginChangeCheck();
                SearchType = (TSearchType)(object)EditorGUILayout.EnumPopup((object)SearchType as Enum, GUILayout.Width(90f));
                SearchTerm = EditorGUILayout.TextField(SearchTerm, GUILayout.ExpandWidth(true), GUILayout.MaxWidth(float.MaxValue));
                if(EditorGUI.EndChangeCheck())
                {
                    OnStateUpdate();
                }
                
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
                    OnStateUpdate();
                    if (m_OnClear != null)
                    {
                        m_OnClear();
                    }

                    EditorApplication.RepaintHierarchyWindow();
                }
            }
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.BeginHorizontal();
            {
                EditorGUILayout.LabelField("", GUILayout.MaxWidth(90f));
                m_CaseSensitiveToggle.OnGUI();
                m_MatchWholeWordToggle.OnGUI();
                m_IncludeInactiveToggle.OnGUI();
            }
            EditorGUILayout.EndHorizontal();
        }

        public void OnGUIEnd()
        {
            bool isReturnPressed = Event.current.type == EventType.KeyUp && Event.current.keyCode == KeyCode.Return;
            if (isReturnPressed && m_OnSearch != null)
            {
                if (m_OnSearch != null)
                {
                    m_OnSearch(SearchType, SearchTerm);
                }
                GUI.FocusControl(null);
                return;
            }

            bool isEscapepressed = Event.current.type == EventType.KeyUp && Event.current.keyCode == KeyCode.Escape;
            if (isEscapepressed && m_OnClear != null)
            {
                SearchTerm = string.Empty;
                OnStateUpdate();
                if (m_OnClear != null)
                {
                    m_OnClear();
                }
                GUI.FocusControl(null);
                return;
            }
        }
    }
}
