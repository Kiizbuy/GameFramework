namespace GameFramework.UI
{
    public interface IPresenter<T0>
    {
        void Present(T0 value);
    }

    public interface IPresenter<T0, T1>
    {
        void Present(T0 value, T1 value2);
    }

    public interface IPresenter<T0, T1, T2>
    {
        void Present(T0 value, T1 value2, T2 value3);
    }
}
