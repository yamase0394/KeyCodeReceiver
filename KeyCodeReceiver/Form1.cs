using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace KeyCodeReceiver
{
    public delegate void WriteLogDelegate(string text);
    public delegate void UpdateRunButtonTextDelegate(string text);

    public partial class Form1 : Form
    {
        private Settings settings;
        public WriteLogDelegate writeLogDelegate;
        public UpdateRunButtonTextDelegate updateRunButtonTextDelegate;
        private KeyReceiver keyReceiver;

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            //ログをTextBoxに出力するためのデリゲート
            writeLogDelegate = WriteLog;
            updateRunButtonTextDelegate = UpdateRunButtonText;
            keyReceiver = new KeyReceiver(this);

            settings = Settings.Instance;

            portTextBox.Text = settings.Port.ToString();

            if (String.IsNullOrWhiteSpace(settings.Pw))
            {
                pwTextBox.Text = "";
            }
            else
            {
                pwTextBox.Text = KeyContainerManager.Decrypt(settings.Pw, "KeyReceiver");
            }

            minimizeToTrayCheck.Checked = settings.MinimizeToTrayCheck;
            autoRunCheck.Checked = settings.AutoRunCheck;
            runOnTrayCheck.Checked = settings.RunOnTrayCheck;

            if (runOnTrayCheck.Checked)
            {
                this.WindowState = FormWindowState.Minimized;
                this.ShowInTaskbar = false;
                runBtn_Click(sender, e);
            }
            else
            {
                notifyIcon1.Visible = false;
            }

            //ログ出力用TextBoxをReadOnlyにしたときの背景色が灰色にならないようにする
            Color backColor = logTextBox.BackColor;
            logTextBox.ReadOnly = true;
            logTextBox.BackColor = backColor;
        }

        private void runBtn_Click(object sender, EventArgs e)
        {
            if (runBtn.Text == "停止")
            {
                keyReceiver.Stop();
                runBtn.Text = "起動";
                return;
            }

            keyReceiver.Run(int.Parse(portTextBox.Text), pwTextBox.Text);

            runBtn.Text = "停止";
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            settings.Port = int.Parse(portTextBox.Text);
            if (String.IsNullOrWhiteSpace(pwTextBox.Text))
            {
                settings.Pw = "";
            }
            else
            {
                settings.Pw = KeyContainerManager.Encrypt(pwTextBox.Text, KeyContainerManager.CreateKeys("KeyReceiver"));
            }
            settings.MinimizeToTrayCheck = minimizeToTrayCheck.Checked;
            settings.AutoRunCheck = autoRunCheck.Checked;
            if (settings.AutoRunCheck)
            {
                using (var regkey = Microsoft.Win32.Registry.CurrentUser.OpenSubKey(@"Software\Microsoft\Windows\CurrentVersion\Run", true))
                {
                    regkey.SetValue(Application.ProductName, Application.ExecutablePath);
                }
            }
            else
            {
                using (var regkey = Microsoft.Win32.Registry.CurrentUser.OpenSubKey(@"Software\Microsoft\Windows\CurrentVersion\Run", true))
                {
                    regkey.DeleteValue(Application.ProductName, false);
                }
            }
            settings.RunOnTrayCheck = runOnTrayCheck.Checked;
            settings.Save();
        }

        private void WriteLog(string text)
        {
            logTextBox.AppendText(DateTime.Now + " " + text + "\r\n");

            var maxLine = 1000;
            if (logTextBox.Lines.Length > maxLine)
            {
                var newLines = new string[maxLine];
                Array.Copy(logTextBox.Lines, 1, newLines, 0, maxLine);
                logTextBox.Lines = newLines;
            }

            logTextBox.SelectionStart = logTextBox.Text.Length;
            logTextBox.ScrollToCaret();
        }

        private void UpdateRunButtonText(string text)
        {
            runBtn.Text = text;
        }

        private void 閉じるToolStripMenuItem_Click(object sender, EventArgs e)
        {
            notifyIcon1.Visible = false;
            Application.Exit();
        }

        private void notifyIcon1_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            notifyIcon1.Visible = false;
            this.Visible = true;
            if (this.WindowState == FormWindowState.Minimized)
            {
                this.WindowState = FormWindowState.Normal;
                this.ShowInTaskbar = true;
            }
            this.Activate();
        }

        private void Form1_ClientSizeChanged(object sender, EventArgs e)
        {
            if (this.WindowState == FormWindowState.Minimized && minimizeToTrayCheck.Checked)
            {
                // フォームが最小化の状態であればフォームを非表示にする
                this.Hide();
                notifyIcon1.Visible = true;
            }
        }

        private void 開くToolStripMenuItem_Click(object sender, EventArgs e)
        {
            notifyIcon1.Visible = false;
            this.Visible = true;
            if (this.WindowState == FormWindowState.Minimized)
            {
                this.WindowState = FormWindowState.Normal;
                this.ShowInTaskbar = true;
            }
            this.Activate();
        }
    }
}
