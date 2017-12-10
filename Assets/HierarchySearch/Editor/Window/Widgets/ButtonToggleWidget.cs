using System;
using UnityEngine;

namespace HierarchySearch
{
    public class ButtonToggleWidget
    {
        public Action OnStateChanged;

        private Texture2D m_Icon;
        private string m_Tooltip;

        public bool IsOn { get; set; }

        public ButtonToggleWidget(string iconPath, string tooltip)
        {
            m_Icon = Resources.Load<Texture2D>(iconPath);
            m_Tooltip = tooltip;
        }

        public void OnDestroy()
        {
            m_Icon = null;
            OnStateChanged = null;
        }

        public void OnGUI()
        {
            if(GUILayout.Button(new GUIContent(m_Icon, m_Tooltip), IsOn ? EditorStyles.ActiveButton : EditorStyles.NormalButton, GUILayout.Width(25f), GUILayout.Height(18f)))
            {
                IsOn = !IsOn;
                if(OnStateChanged != null)
                {
                    OnStateChanged();
                }
            }
        }
    }
}
