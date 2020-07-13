using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace GameFramework.UI
{
    public sealed class WindowNavigator : MonoBehaviour
    {
        public UnityEvent OnHistoryPanelsHasBeenEnded;

        [SerializeField] private Window _current;

        private Stack<Window> _history = new Stack<Window>(8);

        public Window Previous => _history.Count > 0 ? _history.Peek() : null;
        public Window Current => _current;

        public void NavigateToWithSaveHistory(Window window) 
            => NavigateTo(window, true);

        public void NavigateToWithoutSaveHistory(Window window) 
            => NavigateTo(window, false);

        private void NavigateTo(Window window, bool saveToHistory)
        {
            if (_current && _current.HideAfterShowAnotherPanel)
                _current.Hide();

            if (saveToHistory)
                _history.Push(_current);

            _current = window;
            _current.Show();
        }

        public void NavigateBack()
        {
            if (_history.Count == 0)
            {
                OnHistoryPanelsHasBeenEnded?.Invoke();
                return;
            }

            if (_history.Count > 0)
            {
                if (_current)
                    _current.Hide();

                _current = _history.Pop();
                _current.Show();
            }
        }

        public void ClearHistory()
            => _history.Clear();
    }
}

