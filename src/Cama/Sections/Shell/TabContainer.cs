using System.Windows;
using System.Windows.Controls;
using Cama.Tabs;

namespace Cama.Sections.Shell
{
    public class TabContainer : IMainTabContainer
    {
        public void AddTab(TabItem userControl)
        {
            var shell = (ShellWindow)Application.Current.MainWindow;
            shell.AddTab(userControl);
        }

        public void RemoveTab(string name)
        {
            var shell = (ShellWindow)Application.Current.MainWindow;
            shell.RemoveTab(name);
        }

        public void RemoveAllTabs()
        {
            var shell = (ShellWindow)Application.Current.MainWindow;
            shell.RemoveAllTabs();
        }
    }
}