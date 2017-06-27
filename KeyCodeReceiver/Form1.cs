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
            settings.Save();
        }

        private void WriteLog(string text)
        {
            logTextBox.AppendText(DateTime.Now + " " + text + "\r\n");
        }

        private void UpdateRunButtonText(string text)
        {
            runBtn.Text = text;
        }
    }
}
