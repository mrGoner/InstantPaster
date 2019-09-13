using System;
using System.Collections.Generic;
using InstantPaster.Hook;
using InstantPaster.Settings;

namespace InstantPaster
{
    internal class HotKeyConfigurationFactory
    {
        private readonly Dictionary<ActionType, Action<string>> m_map;

        public HotKeyConfigurationFactory(Dictionary<ActionType, Action<string>> _map)
        {
            m_map = _map;
        }

        public HotKeyConfiguration Create(HotKeySettings _settings)
        {
            return new HotKeyConfiguration(_settings.Combination, m_map[_settings.ActionType], _settings.ActionContent);
        }

        public HotKeyConfiguration Create(string _combination, string _actionContent, ActionType _actionType)
        {
            return new HotKeyConfiguration(_combination, m_map[_actionType], _actionContent);
        }
    }
}
