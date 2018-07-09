namespace Cama.Infrastructure
{
    public interface ILoadingDisplayer
    {
        void ShowLoading(string message);

        void HideLoading();
    }
}
