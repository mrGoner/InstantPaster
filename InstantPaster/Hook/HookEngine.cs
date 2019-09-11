using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using Gma.System.MouseKeyHook;
using Gma.System.MouseKeyHook.Implementation;

namespace InstantPaster.Hook
{
    internal class HookEngine
    {
        private IKeyboardMouseEvents m_hooks;
        private List<KeyValuePair<Combination, Action>> m_pairs;

        public void ApplyHotKeys(List<HotKeyConfiguration> _configurations)
        {
            if (_configurations == null)
                throw new ArgumentNullException(nameof(_configurations));

            try
            {
                m_hooks = Gma.System.MouseKeyHook.Hook.GlobalEvents();
                m_pairs = new List<KeyValuePair<Combination, Action>>();

                foreach (var configuration in _configurations)
                {
                    m_pairs.Add(new KeyValuePair<Combination, Action>(Combination.FromString(configuration.Combination),
                        () =>
                        {
                            Console.WriteLine("Detected");

                            configuration.HotKeyAction(configuration.Content);
                        }));
                }

                OnCombination(m_pairs);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        public void Change()
        {
            m_pairs.Add(new KeyValuePair<Combination, Action>(Combination.FromString("P"),
                () => { Console.WriteLine("Detected NEw"); }));
        }

        public void OnCombination(IEnumerable<KeyValuePair<Combination, Action>> _map)
        {
            var watchlists = _map.GroupBy(_k => _k.Key.TriggerKey)
                .ToDictionary(_g => _g.Key, _g => _g.ToArray());

            m_hooks.KeyDown += (_sender, _e) =>
            {
                if (watchlists.TryGetValue(_e.KeyCode, out var element))
                {
                    var state = KeyboardState.GetCurrent();
                    Action action = null;
                    var maxLength = 0;

                    foreach (var current in element)
                    {
                        var matches = current.Key.Chord.All(state.IsDown);

                        if (!matches)
                            continue;

                        if (maxLength > current.Key.ChordLength)
                            continue;

                        maxLength = current.Key.ChordLength;
                        action = current.Value;
                    }

                    action?.Invoke();
                }
            };
        }
    }
}
