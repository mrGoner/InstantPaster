using System;
using System.Collections.Generic;

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
    }
}
