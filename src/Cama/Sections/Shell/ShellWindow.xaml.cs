using System.Collections.Generic;
using System.Linq;
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

        private void OnDrop(object sender, DragEventArgs e)
        {
            var files = ((string[]) e.Data.GetData(DataFormats.FileDrop)).Where(p => p.EndsWith(".cama"));
            var viewModel = DataContext as ShellViewModel;
            viewModel?.OpenReport(files);
        }

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            if (Application.Current.Properties["StartUpFile"] != null)
            {
              var file = Application.Current.Properties["StartUpFile"].ToString();
              var viewModel = DataContext as ShellViewModel;
              viewModel?.OpenReport(new List<string> { file });
            }
        }
    }
}
