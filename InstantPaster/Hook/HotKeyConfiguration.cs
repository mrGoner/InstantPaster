using System;

namespace InstantPaster.Hook
{
    public class HotKeyConfiguration
    {
        public string Combination { get; }

        public string Content { get; }
        public Action<string> HotKeyAction { get; }

        public HotKeyConfiguration(string _combination, Action<string> _hotKeyAction, string _content)
        {
            Combination = _combination;
            HotKeyAction = _hotKeyAction;
            Content = _content;
        }
    }
}
