using UnityEditor;
using UnityEngine;

namespace HierarchySearch
{
    public static class EditorStyles
    {
        #region Icons
        public const string ICON_SEARCH = "ic_search";
        public const string ICON_CLOSE = "ic_close";
        public const string ICON_NOTIFICATION = "ic_priority_high";
        public const string ICON_LOGO = "ic_logo";
        public const string ICON_PREFAB = "PrefabNormal Icon";
        public const string BANNER_LOGO = "hierachy-search-banner";
        #endregion

        #region Colours
        public static Color Red;
        public static Color Pink;
        public static Color Purple;
        public static Color DeepPurple;
        public static Color Indigo;
        public static Color Blue;
        public static Color LightBlue;
        public static Color Cyan;
        public static Color Teal;
        public static Color Green;
        public static Color LightGreen;
        public static Color Lime;
        public static Color Yellow;
        public static Color Amber;
        public static Color Orange;
        public static Color DeepOrange;
        public static Color Brown;
        public static Color Grey;
        public static Color BlueGrey;
        public static Color White;
        public static Color Black;
        #endregion

        private const string PRO_SKIN_RESOURCE_FOLDER = "ProTheme";
        private const string DEFAULT_SKIN_RESOURCE_FOLDER = "DefaultTheme";

        public static void Initialize()
        {
            ColorUtility.TryParseHtmlString("#f44336", out Red);
            ColorUtility.TryParseHtmlString("#e91e63", out Pink);
            ColorUtility.TryParseHtmlString("#9c27b0", out Purple);
            ColorUtility.TryParseHtmlString("#673ab7", out DeepPurple);
            ColorUtility.TryParseHtmlString("#3f51b5", out Indigo);
            ColorUtility.TryParseHtmlString("#2196f3", out Blue);
            ColorUtility.TryParseHtmlString("#03a9f4", out LightBlue);
            ColorUtility.TryParseHtmlString("#00bcd4", out Cyan);
            ColorUtility.TryParseHtmlString("#009688", out Teal);
            ColorUtility.TryParseHtmlString("#4caf50", out Green);
            ColorUtility.TryParseHtmlString("#8bc34a", out LightGreen);
            ColorUtility.TryParseHtmlString("#cddc39", out Lime);
            ColorUtility.TryParseHtmlString("#ffeb3b", out Yellow);
            ColorUtility.TryParseHtmlString("#ffc107", out Amber);
            ColorUtility.TryParseHtmlString("#ff9800", out Orange);
            ColorUtility.TryParseHtmlString("#ff5722", out DeepOrange);
            ColorUtility.TryParseHtmlString("#795548", out Brown);
            ColorUtility.TryParseHtmlString("#9e9e9e", out Grey);
            ColorUtility.TryParseHtmlString("#607d8b", out BlueGrey);
            ColorUtility.TryParseHtmlString("#ffffff", out White);
            ColorUtility.TryParseHtmlString("#000000", out Black);
        }

        public static void Reset()
        {
            m_SmallButtonWidth = null;
            m_MediumButtonWidth = null;
            m_Header = null;
            m_SearchResult = null;
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
            return GUILayout.Button(icon, GUILayout.Width(30f), GUILayout.Height(18f));
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
                    m_SearchResult.normal.textColor = HierarchySearchSettings.Instance.searchResultText;
                }
                return m_SearchResult;
            }
        }
        #endregion
    }
}