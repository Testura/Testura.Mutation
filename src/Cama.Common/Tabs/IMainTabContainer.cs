using System.Windows.Controls;

namespace Cama.Common.Tabs
{
    public interface IMainTabContainer
    {
        void AddTab(TabItem userControl);

        void RemoveTab(string name);
    }
}
