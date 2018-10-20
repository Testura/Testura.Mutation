using System.Windows.Controls;

namespace Cama.Helpers.Openers.Tabs
{
    public interface IMainTabContainer
    {
        void AddTab(TabItem userControl);

        void RemoveTab(string name);

        void RemoveAllTabs();
    }
}
