using System;
using System.Windows.Input;
using Microsoft.Expression.Interactivity.Core;

namespace InstantPaster.ViewModels
{
    public class DetailViewModel
    {
        public string Content { get; set; }
        public ICommand CloseCommand { get; }

        public DetailViewModel(Action<string> _closeAction, string _content)
        {
            Content = _content;
            CloseCommand = new ActionCommand(() => _closeAction?.Invoke(Content));
        }
    }
}
