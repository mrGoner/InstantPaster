using System;
using System.Linq;
using System.Windows.Interactivity;
using Gma.System.MouseKeyHook.Implementation;
using KeyEventArgs = System.Windows.Input.KeyEventArgs;
using TextBox = System.Windows.Controls.TextBox;

namespace InstantPaster
{
    internal class KeyListeningBehavior : Behavior<TextBox>
    {
        private Keys[] m_keys;
        private ModifierKeys[] m_modifierKeys;

        public KeyListeningBehavior()
        {
            m_keys = Enum.GetValues(typeof(Keys)).Cast<Keys>().ToArray();
            m_modifierKeys = Enum.GetValues(typeof(ModifierKeys)).Cast<ModifierKeys>().ToArray();
        }

        protected override void OnAttached()
        {
            AssociatedObject.KeyDown += AssociatedObjectOnKeyDown;
        }

        private void AssociatedObjectOnKeyDown(object _sender, KeyEventArgs _e)
        {
            var state = KeyboardState.GetCurrent();

            var pressedModifiers = m_modifierKeys.Where(_key => state.IsDown((System.Windows.Forms.Keys) _key))
                .Select(_x => _x.ToString());
            var pressedOthers = m_keys.Where(_key => state.IsDown((System.Windows.Forms.Keys) _key))
                .Select(_x => _x.ToString());

            var allPressed = pressedModifiers.Union(pressedOthers);

            _e.Handled = true;

            AssociatedObject.Text = string.Join("+", allPressed);
        }

        protected override void OnDetaching()
        {
            AssociatedObject.KeyUp -= AssociatedObjectOnKeyDown;
        }
    }
}
