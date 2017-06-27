using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace KeyCodeReceiver
{
    public class Keyboarder
    {
        private const uint KEYEVENTF_KEYUP = 2;
        private Dictionary<string, Keys> toKeyCodeMap;
        private object lockObj = new object();
        private bool isSending;

        public Keyboarder()
        {
            InitKeyCodeMap();
        }

        public void InputKeys(string[] keyStrList)
        {
            Task.Run(() =>
            {
                lock (lockObj)
                {
                    if (isSending)
                    {
                        return;
                    }
                    isSending = true;
                }

                var keysList = new List<Keys>();
                foreach (string str in keyStrList)
                {
                    try
                    {
                        Console.WriteLine(str);
                        keysList.Add(toKeyCodeMap[str]);
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e.ToString());
                    }
                }

                keysList.Sort((a, b) =>
                {
                    //Winキーを一番小さくする
                    if (a == Keys.LWin)
                    {
                        if (b == Keys.LWin)
                        {
                            return 0;
                        }
                        return -1;
                    }
                    if (b == Keys.LWin)
                    {
                        return 1;
                    }

                    //Tabキーを二番目に小さくする
                    if (a == Keys.Tab)
                    {
                        if (b == Keys.Tab)
                        {
                            return 0;
                        }
                        return 1;
                    }
                    if (b == Keys.Tab)
                    {
                        return -1;
                    }

                    return b - a;
                });

                //キーを押す
                keysList.ForEach(keys => Win32api.keybd_event((byte)keys, 0, 0, IntPtr.Zero));
                //キーを離す
                keysList.ForEach(keys => Win32api.keybd_event((byte)keys, 0, KEYEVENTF_KEYUP, IntPtr.Zero));

                isSending = false;
            });
        }

        private void InitKeyCodeMap()
        {
            toKeyCodeMap = new Dictionary<string, Keys>();
            //アルファベット
            toKeyCodeMap.Add("A", Keys.A);
            toKeyCodeMap.Add("B", Keys.B);
            toKeyCodeMap.Add("C", Keys.C);
            toKeyCodeMap.Add("D", Keys.D);
            toKeyCodeMap.Add("E", Keys.E);
            toKeyCodeMap.Add("F", Keys.F);
            toKeyCodeMap.Add("G", Keys.G);
            toKeyCodeMap.Add("H", Keys.H);
            toKeyCodeMap.Add("I", Keys.I);
            toKeyCodeMap.Add("J", Keys.J);
            toKeyCodeMap.Add("K", Keys.K);
            toKeyCodeMap.Add("L", Keys.L);
            toKeyCodeMap.Add("M", Keys.M);
            toKeyCodeMap.Add("N", Keys.N);
            toKeyCodeMap.Add("O", Keys.O);
            toKeyCodeMap.Add("P", Keys.P);
            toKeyCodeMap.Add("Q", Keys.Q);
            toKeyCodeMap.Add("R", Keys.R);
            toKeyCodeMap.Add("S", Keys.S);
            toKeyCodeMap.Add("T", Keys.T);
            toKeyCodeMap.Add("U", Keys.U);
            toKeyCodeMap.Add("V", Keys.V);
            toKeyCodeMap.Add("W", Keys.W);
            toKeyCodeMap.Add("X", Keys.X);
            toKeyCodeMap.Add("Y", Keys.Y);
            toKeyCodeMap.Add("Z", Keys.Z);
            //数字
            Enumerable.Range(0, 9).ToList().ForEach(i => toKeyCodeMap.Add("D" + i, (Keys)Enum.Parse(typeof(Keys), "D" + i)));
            //ファンクションキー
            Enumerable.Range(1, 12).ToList().ForEach(i => toKeyCodeMap.Add("F" + i, (Keys)Enum.Parse(typeof(Keys), "F" + i, false)));
            //制御キー
            toKeyCodeMap.Add("Backspace", Keys.Back);
            toKeyCodeMap.Add("Enter", Keys.Enter);
            toKeyCodeMap.Add("Shift", Keys.LShiftKey);
            toKeyCodeMap.Add("Ctrl", Keys.LControlKey);
            toKeyCodeMap.Add("Alt", Keys.LMenu);
            toKeyCodeMap.Add("Pause", Keys.Pause);
            toKeyCodeMap.Add("Space", Keys.Space);
            toKeyCodeMap.Add("PageUp", Keys.PageUp);
            toKeyCodeMap.Add("PageDown", Keys.PageDown);
            toKeyCodeMap.Add("End", Keys.End);
            toKeyCodeMap.Add("Home", Keys.Home);
            toKeyCodeMap.Add("←", Keys.Left);
            toKeyCodeMap.Add("↑", Keys.Up);
            toKeyCodeMap.Add("→", Keys.Right);
            toKeyCodeMap.Add("↓", Keys.Down);
            toKeyCodeMap.Add("PrintScreen", Keys.PrintScreen);
            toKeyCodeMap.Add("Insert", Keys.Insert);
            toKeyCodeMap.Add("Delete", Keys.Delete);
            toKeyCodeMap.Add("Win", Keys.LWin);
            toKeyCodeMap.Add("NumLock", Keys.NumLock);
            toKeyCodeMap.Add("ScrollLock", Keys.Scroll);
            toKeyCodeMap.Add("Esc", Keys.Escape);
            toKeyCodeMap.Add("Tab", Keys.Tab);
            //特殊
            toKeyCodeMap.Add("VolUp", Keys.VolumeUp);
            toKeyCodeMap.Add("VolDown", Keys.VolumeDown);
            toKeyCodeMap.Add("VolMute", Keys.VolumeMute);
        }
    }
}
