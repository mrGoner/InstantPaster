using System;
using System.Collections.Generic;
using System.Windows.Forms;
using Gma.System.MouseKeyHook;

namespace InstantPaster.Hook
{
    internal class HookEngine
    {
        private readonly IKeyboardMouseEvents m_hooks;

        public HookEngine()
        {
            m_hooks = Gma.System.MouseKeyHook.Hook.GlobalEvents();

            m_hooks.KeyDown += GlobalHookKeyPress;
            m_hooks.OnCombination(new List<KeyValuePair<Combination, Action>>
            {
                new KeyValuePair<Combination, Action>(Combination.FromString("Alt+C"), () => Console.WriteLine("Detected"))
            });
        }

        private void GlobalHookKeyPress(object sender, KeyEventArgs e)
        {
            Console.WriteLine($"Pressed + {e.KeyData.ToString()}");
        }
    }
}
