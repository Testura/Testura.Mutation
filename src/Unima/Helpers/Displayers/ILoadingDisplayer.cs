namespace Unima.Helpers.Displayers
{
    public interface ILoadingDisplayer
    {
        void ShowLoading(string message);

        void HideLoading();
    }
}
