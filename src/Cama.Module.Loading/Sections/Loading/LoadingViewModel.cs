using Cama.Infrastructure;
using Prism.Mvvm;

namespace Cama.Module.Loading.Sections.Loading
{
    public class LoadingViewModel : BindableBase, ILoadingDisplayer
    {
        public LoadingViewModel()
        {
            Message = "";
            IsVisible = false;
        }

        public string Message { get; set; }

        public bool IsVisible { get; set; }

        public void ShowLoading(string message)
        {
            IsVisible = true;
            Message = message;
        }

        public void HideLoading()
        {
            IsVisible = false;
            Message = string.Empty;
        }
    }
}
