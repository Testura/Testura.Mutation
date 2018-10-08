using System.Linq;
using System.Windows;

namespace Cama
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            if (e.Args.Any())
            {
                Properties["StartUpFile"] = e.Args[0];
            }

            var bootstrapper = new Bootstrapper();
            bootstrapper.Run();
        }
    }
}
