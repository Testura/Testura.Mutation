using System.Windows;
using System.Windows.Controls;

namespace Cama.Sections.Shell
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class ShellWindow : Window
    {
        public ShellWindow()
        {
            InitializeComponent();
        }

        public void AddTab(TabItem userControl)
        {
            MyTabControl.Items.Add(userControl);
            MyTabControl.SelectedItem = userControl;
        }

        public void RemoveTab(string name)
        {
            for (int n = 0; n < MyTabControl.Items.Count; n++)
            {
                var tabItem = MyTabControl.Items[n] as TabItem;
                if (tabItem.Header.ToString() == name)
                {
                    MyTabControl.Items.Remove(tabItem);
                }
            }
        }

        public void RemoveAllTabs()
        {
            MyTabControl.Items.Clear();
        }
    }
}
