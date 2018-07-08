using Prism.Mvvm;

namespace Cama.Sections.Shell
{
    public class ShellViewModel : BindableBase
    {
        public ShellViewModel()
        {
            MyInterTabClient = new MyInterTabClient();
        }

        public MyInterTabClient MyInterTabClient { get; set; }
    }
}
