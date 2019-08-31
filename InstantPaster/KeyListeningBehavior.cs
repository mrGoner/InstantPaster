using System;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Interactivity;

namespace InstantPaster
{
    internal class KeyListeningBehavior : Behavior<TextBox>
    {

        protected override void OnAttached()
        {
            AssociatedObject.KeyDown += AssociatedObjectOnKeyDown;
        }

        private void AssociatedObjectOnKeyDown(object sender, KeyEventArgs e)
        {
            AssociatedObject.Text = e.Key.ToString();
        }

        private void AssociatedObjectOnKeyUp(object sender, KeyEventArgs e)
        {
        }

        protected override void OnDetaching()
        {
            AssociatedObject.KeyUp -= AssociatedObjectOnKeyUp;
        }
    }
}
