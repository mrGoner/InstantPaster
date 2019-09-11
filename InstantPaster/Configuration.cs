using System;
using System.Collections.Generic;

namespace InstantPaster
{
    internal class Configuration
    {
        public List<HotKeySettings> HotKeys { get; }

        public Configuration(List<HotKeySettings> _configurations)
        {
            HotKeys = _configurations ?? throw new ArgumentNullException(nameof(_configurations));
        }
    }
}