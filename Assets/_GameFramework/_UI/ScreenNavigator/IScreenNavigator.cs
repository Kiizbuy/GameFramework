namespace GameFramework.UI
{
    public interface IScreenNavigator
    {
        void NavigateBack();
        void NavigateTo(IScreen screen, bool saveHistory);
        void ClearHistory();
    }
}
