using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace GameFramework.UI
{
    public interface IScreenNavigator
    {
        void NavigateBack();
        void NavigateTo(IScreen screen, bool saveHistory);
        void ClearHistory();
    }

    public interface IScreen
    {
        void Show();
        void Hide();
        bool HideAfterShowAnotherPanel { get; }
    }

    public sealed class ScreenNavigator : MonoBehaviour, IScreenNavigator
    {
        public UnityEvent OnHistoryPanelsHasBeenEnded;

        private readonly Stack<IScreen> _history = new Stack<IScreen>(8);

        private IScreen _current;

        public IScreen Previous => _history.Count > 0 ? _history.Peek() : null;
        public IScreen Current => _current;

        public void NavigateToWithSaveHistory(Screen screen)
            => NavigateTo(screen, true);

        public void NavigateToWithoutSaveHistory(Screen screen)
            => NavigateTo(screen, false);

        public void NavigateTo(IScreen screen, bool saveToHistory)
        {
            if (_current != null && _current.HideAfterShowAnotherPanel)
                _current.Hide();

            if (saveToHistory)
                _history.Push(_current);

            _current = screen;
            _current.Show();
        }

        public void NavigateBack()
        {
            if (_history.Count == 0)
            {
                OnHistoryPanelsHasBeenEnded?.Invoke();
                return;
            }

            if (_history.Count <= 0)
                return;

            _current?.Hide();
            _current = _history.Pop();
            _current.Show();
        }

        public void ClearHistory()
            => _history.Clear();
    }
}
