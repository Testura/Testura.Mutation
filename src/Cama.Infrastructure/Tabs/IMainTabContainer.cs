using System.Windows.Controls;

namespace Cama.Infrastructure.Tabs
{
    public interface IMainTabContainer
    {
        void AddTab(TabItem userControl);

        void RemoveTab(string name);
    }
}
