using UnityEditor;
using UnityEngine;

namespace HierarchySearch
{
    public static class EditorStyles
    {
        #region Icons
        public const string ICON_WINDOW = "ic_window_icon";
        public const string ICON_SEARCH = "ic_search";
        public const string ICON_CLOSE = "ic_close";
        public const string ICON_NOTIFICATION = "ic_priority_high";
        public const string ICON_MATCH_CASE = "ic_match_case";
        public const string ICON_MATCH_WHOLE_WORD = "ic_whole_word";
        public const string ICON_INACTIVE = "ic_inactive";
        public const string ICON_LOGO = "ic_logo";
        public const string ICON_PREFAB = "PrefabNormal Icon";
        public const string BANNER_LOGO = "hierachy-search-banner";
        #endregion

        #region Colours

        public static Color BackgroundColor { get; private set; }
        public static Color TextColor { get; private set; }

        private static Color sz_ProDefaultBackgroundColor;
        private static Color sz_ProDefaultTextColor;
        private static Color sz_DefaultBackgroundColor;
        private static Color sz_DefaultTextColor;
        
        public static Color DefaultBackgroundColor
        {
            get { return EditorGUIUtility.isProSkin ? sz_ProDefaultBackgroundColor : sz_DefaultBackgroundColor; }
        }
        
        public static Color DefaultTextColor
        {
            get { return EditorGUIUtility.isProSkin ? sz_ProDefaultTextColor : sz_DefaultTextColor; }
        }
        #endregion

        private const string PRO_SKIN_RESOURCE_FOLDER = "ProTheme";
        private const string DEFAULT_SKIN_RESOURCE_FOLDER = "DefaultTheme";

        public static void Initialize()
        {
            ColorUtility.TryParseHtmlString("#3e5f96", out sz_ProDefaultBackgroundColor);
            ColorUtility.TryParseHtmlString("#e1e6ef", out sz_ProDefaultTextColor);
            ColorUtility.TryParseHtmlString("#3e7de7", out sz_DefaultBackgroundColor);
            ColorUtility.TryParseHtmlString("#e1ebfb", out sz_DefaultTextColor);

            Reset();
        }

        public static void Reset()
        {
            m_SmallButtonWidth = null;
            m_MediumButtonWidth = null;
            m_Header = null;
            m_SearchResult = null;

            BackgroundColor = EditorPrefsUtils.LoadColor(EditorPrefKeys.KEY_BACKGROUND_COLOR, DefaultBackgroundColor);
            TextColor = EditorPrefsUtils.LoadColor(EditorPrefKeys.KEY_TEXT_COLOR, DefaultTextColor);
        }

        public static string ThemeFolder
        {
            get
            {
                return EditorGUIUtility.isProSkin ? PRO_SKIN_RESOURCE_FOLDER : DEFAULT_SKIN_RESOURCE_FOLDER;
            }
        }

        public static bool GetIconButton(Texture2D icon)
        {
            return GUILayout.Button(icon, NormalButton, GUILayout.Width(25f), GUILayout.Height(18f));
        }

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
                    m_Header = new GUIStyle(GUI.skin.label);
                    m_Header.fontStyle = FontStyle.Bold;
                    m_Header.normal.textColor = EditorGUIUtility.isProSkin ? Color.white : Color.black;
                }
                return m_Header;
            }
        }

        private static GUIStyle m_SearchResult;
        public static GUIStyle SearchResult
        {
            get
            {
                if (m_SearchResult == null)
                {
                    m_SearchResult = new GUIStyle(GUI.skin.label);
                    m_SearchResult.fontStyle = FontStyle.Bold;
                    m_SearchResult.normal.textColor = TextColor;
                }
                return m_SearchResult;
            }
        }

        private static GUIStyle m_PrefabButton;
        public static GUIStyle PrefabButton
        {
            get
            {
                if (m_PrefabButton == null)
                {
                    m_PrefabButton = new GUIStyle(GUIStyle.none);
                    m_PrefabButton.fontStyle = FontStyle.Bold;
                    m_PrefabButton.normal.textColor = EditorGUIUtility.isProSkin ? Color.white : Color.black;
                }
                return m_PrefabButton;
            }
        }

        private static GUIStyle m_ActiveButton;
        public static GUIStyle ActiveButton
        {
            get
            {
                if (m_ActiveButton == null)
                {
                    m_ActiveButton = new GUIStyle(GUI.skin.button);
                    m_ActiveButton.normal = m_ActiveButton.active;
                }
                return m_ActiveButton;
            }
        }

        private static GUIStyle m_NormalButton;
        public static GUIStyle NormalButton
        {
            get
            {
                if (m_NormalButton == null)
                {
                    m_NormalButton = new GUIStyle(GUI.skin.button);
                }
                return m_NormalButton;
            }
        }
        #endregion
    }
}