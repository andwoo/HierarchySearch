namespace HierarchySearch
{
    public interface IWindowTab
    {
        void OnEnable();
        void OnDisable();
        void OnDestroy();
        void OnGUI();
        void OnGUIEnd();
    }
}
