using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KeyCodeReceiver
{
    // Win32APIを呼び出すためのクラス
    class Win32api
    {
        [System.Runtime.InteropServices.DllImport("user32.dll")]
        public static extern uint keybd_event(byte bVk, byte bScan, uint dwFlags, IntPtr dwExtraInfo);
    }
}
