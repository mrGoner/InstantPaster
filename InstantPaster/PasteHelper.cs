using System;
using System.Runtime.InteropServices;
using System.Windows;

namespace InstantPaster
{
    internal class PasteHelper
    {
        private const uint WM_PASTE = 0x0302;

        [StructLayout(LayoutKind.Sequential)]
        internal struct Rect
        {
            public int Left;
            public int Top;
            public int Right;
            public int Bottom;
        }

        [StructLayout(LayoutKind.Sequential)]
        internal struct GuiThreadInfo
        {
            public int cbSize;
            public uint flags;
            public IntPtr hwndActive;
            public IntPtr hwndFocus;
            public IntPtr hwndCapture;
            public IntPtr hwndMenuOwner;
            public IntPtr hwndMoveSize;
            public IntPtr hwndCaret;
            public Rect rcCaret;
        }

        [DllImport("user32.dll")]
        private static extern int SendMessage(IntPtr _hWnd, uint _msg, IntPtr _wParam, IntPtr _lParam);

        [DllImport("user32.dll", SetLastError = true)]
        private static extern bool GetGUIThreadInfo(uint _idThread, ref GuiThreadInfo _threadInfo);

        private static IntPtr GetFocusedHandle()
        {
            var info = new GuiThreadInfo();
            info.cbSize = Marshal.SizeOf(info);

            if (!GetGUIThreadInfo(0, ref info))
                throw new InvalidOperationException("Failed to get thread info");

            return info.hwndFocus;
        }

        public static bool TryPaste(string _content)
        {
            try
            {
                Clipboard.SetText(_content);

                var currentHandler = GetFocusedHandle();

                SendMessage(currentHandler, WM_PASTE, IntPtr.Zero, IntPtr.Zero);

                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}
