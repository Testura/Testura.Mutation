using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;

namespace Testura.Mutation.Wpf.Sections.Shell
{
    /// <inheritdoc />
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
            var files = (string[])e.Data.GetData(DataFormats.FileDrop);
            var viewModel = DataContext as ShellViewModel;
            viewModel?.Open(files);
        }

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            if (System.Windows.Application.Current.Properties["StartUpFile"] != null)
            {
              var file = System.Windows.Application.Current.Properties["StartUpFile"].ToString();
              var viewModel = DataContext as ShellViewModel;
              viewModel?.Open(new List<string> { file });
            }
        }
    }
}
