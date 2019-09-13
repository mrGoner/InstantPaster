using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using Gma.System.MouseKeyHook;
using Gma.System.MouseKeyHook.Implementation;

namespace InstantPaster.Hook
{
    internal class HookEngine : IDisposable
    {
        private readonly IKeyboardMouseEvents m_hooks;
        private Dictionary<System.Windows.Forms.Keys, KeyValuePair<Combination, Action> []> m_watchList;
        private bool m_isNeedTracking;

        public HookEngine()
        {
            m_hooks = Gma.System.MouseKeyHook.Hook.GlobalEvents();
            m_hooks.KeyDown += HooksOnKeyDown;
        }

        private void HooksOnKeyDown(object _sender, KeyEventArgs _e)
        {
            if(m_watchList == null || !m_isNeedTracking)
                return;

            if (m_watchList.TryGetValue(_e.KeyCode, out var element))
            {
                var state = KeyboardState.GetCurrent();
                Action action = null;
                var maxLength = 0;
                var needHandle = false;

                foreach (var current in element)
                {
                    var matches = current.Key.Chord.All(state.IsDown);

                    if (!matches)
                        continue;

                    if (maxLength > current.Key.ChordLength)
                        continue;

                    maxLength = current.Key.ChordLength;
                    action = current.Value;
                    needHandle = true;
                }

                if (needHandle)
                {
                    _e.Handled = true;
                    Console.WriteLine("Handeled");
                }

                action?.Invoke();
            }
        }

        public void SetHotKeys(List<HotKeyConfiguration> _configurations)
        {
            if (_configurations == null)
                throw new ArgumentNullException(nameof(_configurations));

            m_isNeedTracking = false;

            try
            {
                var pairs = new List<KeyValuePair<Combination, Action>>();

                foreach (var configuration in _configurations)
                {
                    pairs.Add(new KeyValuePair<Combination, Action>(Combination.FromString(configuration.Combination),
                        () => configuration.HotKeyAction(configuration.Content)));
                }

                InitializeMap(pairs);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        public void StopTracking()
        {
            m_isNeedTracking = false;
        }

        public void StartTracking()
        {
            m_isNeedTracking = true;
        }

        private void InitializeMap(IEnumerable<KeyValuePair<Combination, Action>> _map)
        {
            m_watchList = _map.GroupBy(_k => _k.Key.TriggerKey)
                .ToDictionary(_g => _g.Key, _g => _g.ToArray());
        }

        public void Dispose()
        {
            m_hooks.KeyDown -= HooksOnKeyDown;
            m_hooks.Dispose();
        }
    }
}
