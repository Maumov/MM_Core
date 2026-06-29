using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace MM.EditorDrawer
{
    [System.Serializable]
    public class SearchListDrawer
    {
        public delegate void DisplayAction(string i_ItemValue, bool i_Selected);

        public delegate void EnterAction(string i_ItemValue, bool i_ControlPressed);

        private List<string> m_Items = new List<string>();
        private List<string> m_FilteredItems = new List<string>();

        private bool m_FirstFocus;
        private Vector2 m_ScrollVector;
        private string m_SearchText = "";
        private int m_Selected;

        public DisplayAction m_OnDisplayElement;
        public EnterAction m_OnEnterAction;
        private bool m_ForceFilter = true;

        public SearchListDrawer()
        {
            m_OnDisplayElement = (value, selected) =>
            {
                using (GUIScope.Color(selected ? Color.blue : GUI.color))
                {
                    EditorGUILayout.LabelField(value);
                }
            };
            m_OnEnterAction = (value, pressed) => { };
        }

        public void UpdateList(List<string> i_List)
        {
            m_Items = i_List;
            m_FilteredItems = new List<string>(m_Items.Count);
            m_ForceFilter = true;
        }

        public void Draw()
        {
            HandleInputs();
            DrawSearchBar();

            using (GUIScope.ScrollView(ref m_ScrollVector))
            {
                DisplaySearchList();
            }
        }

        private void DisplaySearchList()
        {
            for (var index = 0; index < m_FilteredItems.Count; index++)
            {
                string scene = m_FilteredItems[index];
                m_OnDisplayElement(scene, m_Selected == index);
            }
        }

        private void DrawSearchBar()
        {
            EditorGUI.BeginChangeCheck();
            using (GUIScope.Horizontal())
            {
                GUI.SetNextControlName("search");
                m_SearchText = EditorGUILayout.TextField(m_SearchText, new GUIStyle("SearchTextField"));
                GUILayout.Button(GUIContent.none, new GUIStyle("SearchCancelButtonEmpty"), GUILayout.Width(20f));
            }
            if (EditorGUI.EndChangeCheck() || m_ForceFilter)
            {
                FilterItems();
                m_ForceFilter = false;
            }

            if (m_Selected >= m_FilteredItems.Count)
            {
                m_Selected = m_FilteredItems.Count - 1;
            }
            if (m_Selected < 0)
            {
                m_Selected = 0;
            }

            if (!m_FirstFocus || !EditorGUIUtility.editingTextField)
            {
                m_FirstFocus = true;
                GUI.FocusControl("search");
                EditorGUIUtility.editingTextField = true;
            }
        }

        private void HandleInputs()
        {
            Event e = Event.current;
            switch (e.type)
            {
                case EventType.KeyDown:
                    if ((e.keyCode == KeyCode.UpArrow) && (m_Selected > 0))
                    {
                        m_Selected--;
                        e.Use();
                    }
                    else if ((e.keyCode == KeyCode.DownArrow) && (m_Selected < m_FilteredItems.Count - 1))
                    {
                        m_Selected++;
                        e.Use();
                    }
                    else if ((m_Selected < m_FilteredItems.Count) &&
                             ((e.keyCode == KeyCode.KeypadEnter) || (e.keyCode == KeyCode.Return)))
                    {
                        m_OnEnterAction(m_FilteredItems[m_Selected], e.control);
                        e.Use();
                    }
                    break;
            }
        }

        private void FilterItems()
        {
            if (string.IsNullOrEmpty(m_SearchText))
            {
                m_FilteredItems = new List<string>(m_Items);
            }
            else
            {
                m_FilteredItems = new List<string>(m_Items.Count);
                foreach (string s in m_Items)
                {
                    if (s.ToLower().Contains(m_SearchText.ToLower()))
                    {
                        m_FilteredItems.Add(s);
                    }
                }
            }
        }
    }
}