namespace KeyCodeReceiver
{
    partial class Form1
    {
        /// <summary>
        /// 必要なデザイナー変数です。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 使用中のリソースをすべてクリーンアップします。
        /// </summary>
        /// <param name="disposing">マネージ リソースを破棄する場合は true を指定し、その他の場合は false を指定します。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows フォーム デザイナーで生成されたコード

        /// <summary>
        /// デザイナー サポートに必要なメソッドです。このメソッドの内容を
        /// コード エディターで変更しないでください。
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
            this.portTextBox = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.pwTextBox = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.runBtn = new System.Windows.Forms.Button();
            this.logTextBox = new System.Windows.Forms.TextBox();
            this.notifyIcon1 = new System.Windows.Forms.NotifyIcon(this.components);
            this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.開くToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.閉じるToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.minimizeToTrayCheck = new System.Windows.Forms.CheckBox();
            this.autoRunCheck = new System.Windows.Forms.CheckBox();
            this.runOnTrayCheck = new System.Windows.Forms.CheckBox();
            this.contextMenuStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // portTextBox
            // 
            this.portTextBox.Location = new System.Drawing.Point(75, 19);
            this.portTextBox.MaxLength = 5;
            this.portTextBox.Name = "portTextBox";
            this.portTextBox.Size = new System.Drawing.Size(62, 19);
            this.portTextBox.TabIndex = 0;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 22);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(57, 12);
            this.label1.TabIndex = 1;
            this.label1.Text = "ポート番号";
            // 
            // pwTextBox
            // 
            this.pwTextBox.Location = new System.Drawing.Point(75, 51);
            this.pwTextBox.MaxLength = 15;
            this.pwTextBox.Name = "pwTextBox";
            this.pwTextBox.Size = new System.Drawing.Size(197, 19);
            this.pwTextBox.TabIndex = 2;
            this.pwTextBox.UseSystemPasswordChar = true;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(12, 54);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(52, 12);
            this.label2.TabIndex = 3;
            this.label2.Text = "パスワード";
            // 
            // runBtn
            // 
            this.runBtn.Location = new System.Drawing.Point(111, 162);
            this.runBtn.Name = "runBtn";
            this.runBtn.Size = new System.Drawing.Size(75, 23);
            this.runBtn.TabIndex = 4;
            this.runBtn.Text = "起動";
            this.runBtn.UseVisualStyleBackColor = true;
            this.runBtn.Click += new System.EventHandler(this.runBtn_Click);
            // 
            // logTextBox
            // 
            this.logTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.logTextBox.HideSelection = false;
            this.logTextBox.Location = new System.Drawing.Point(14, 191);
            this.logTextBox.Multiline = true;
            this.logTextBox.Name = "logTextBox";
            this.logTextBox.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.logTextBox.Size = new System.Drawing.Size(258, 132);
            this.logTextBox.TabIndex = 5;
            this.logTextBox.WordWrap = false;
            // 
            // notifyIcon1
            // 
            this.notifyIcon1.ContextMenuStrip = this.contextMenuStrip1;
            this.notifyIcon1.Icon = ((System.Drawing.Icon)(resources.GetObject("notifyIcon1.Icon")));
            this.notifyIcon1.Text = "プログラマブルキーボード";
            this.notifyIcon1.Visible = true;
            this.notifyIcon1.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.notifyIcon1_MouseDoubleClick);
            // 
            // contextMenuStrip1
            // 
            this.contextMenuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.開くToolStripMenuItem,
            this.閉じるToolStripMenuItem});
            this.contextMenuStrip1.Name = "contextMenuStrip1";
            this.contextMenuStrip1.Size = new System.Drawing.Size(105, 48);
            // 
            // 開くToolStripMenuItem
            // 
            this.開くToolStripMenuItem.Name = "開くToolStripMenuItem";
            this.開くToolStripMenuItem.Size = new System.Drawing.Size(104, 22);
            this.開くToolStripMenuItem.Text = "開く";
            this.開くToolStripMenuItem.Click += new System.EventHandler(this.開くToolStripMenuItem_Click);
            // 
            // 閉じるToolStripMenuItem
            // 
            this.閉じるToolStripMenuItem.Name = "閉じるToolStripMenuItem";
            this.閉じるToolStripMenuItem.Size = new System.Drawing.Size(104, 22);
            this.閉じるToolStripMenuItem.Text = "閉じる";
            this.閉じるToolStripMenuItem.Click += new System.EventHandler(this.閉じるToolStripMenuItem_Click);
            // 
            // minimizeToTrayCheck
            // 
            this.minimizeToTrayCheck.AutoSize = true;
            this.minimizeToTrayCheck.Location = new System.Drawing.Point(14, 90);
            this.minimizeToTrayCheck.Name = "minimizeToTrayCheck";
            this.minimizeToTrayCheck.Size = new System.Drawing.Size(148, 16);
            this.minimizeToTrayCheck.TabIndex = 7;
            this.minimizeToTrayCheck.Text = "最小化でトレイに移動する";
            this.minimizeToTrayCheck.UseVisualStyleBackColor = true;
            // 
            // autoRunCheck
            // 
            this.autoRunCheck.AutoSize = true;
            this.autoRunCheck.Location = new System.Drawing.Point(14, 112);
            this.autoRunCheck.Name = "autoRunCheck";
            this.autoRunCheck.Size = new System.Drawing.Size(172, 16);
            this.autoRunCheck.TabIndex = 8;
            this.autoRunCheck.Text = "PC起動時に自動的に起動する";
            this.autoRunCheck.UseVisualStyleBackColor = true;
            // 
            // runOnTrayCheck
            // 
            this.runOnTrayCheck.AutoSize = true;
            this.runOnTrayCheck.Location = new System.Drawing.Point(14, 134);
            this.runOnTrayCheck.Name = "runOnTrayCheck";
            this.runOnTrayCheck.Size = new System.Drawing.Size(102, 16);
            this.runOnTrayCheck.TabIndex = 9;
            this.runOnTrayCheck.Text = "トレイに起動する";
            this.runOnTrayCheck.UseVisualStyleBackColor = true;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(284, 335);
            this.Controls.Add(this.runOnTrayCheck);
            this.Controls.Add(this.autoRunCheck);
            this.Controls.Add(this.minimizeToTrayCheck);
            this.Controls.Add(this.logTextBox);
            this.Controls.Add(this.runBtn);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.pwTextBox);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.portTextBox);
            this.Name = "Form1";
            this.Text = "Form1";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Form1_FormClosing);
            this.Load += new System.EventHandler(this.Form1_Load);
            this.ClientSizeChanged += new System.EventHandler(this.Form1_ClientSizeChanged);
            this.contextMenuStrip1.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox portTextBox;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox pwTextBox;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button runBtn;
        private System.Windows.Forms.TextBox logTextBox;
        private System.Windows.Forms.NotifyIcon notifyIcon1;
        private System.Windows.Forms.ContextMenuStrip contextMenuStrip1;
        private System.Windows.Forms.ToolStripMenuItem 閉じるToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem 開くToolStripMenuItem;
        private System.Windows.Forms.CheckBox minimizeToTrayCheck;
        private System.Windows.Forms.CheckBox autoRunCheck;
        private System.Windows.Forms.CheckBox runOnTrayCheck;
    }
}

