using System.Windows.Controls;

namespace Unima.Helpers.Openers.Tabs
{
    public interface IMainTabContainer
    {
        void AddTab(TabItem userControl);

        void RemoveTab(string name);

        void RemoveAllTabs();
    }
}
