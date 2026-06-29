namespace MM.ScriptableObjectCreator
{
    public class AddScriptableObjectMenuAttribute : System.Attribute
    {
        public string m_Menu;

        public AddScriptableObjectMenuAttribute(string i_Menu)
        {
            m_Menu = i_Menu;
        }
    }
}
