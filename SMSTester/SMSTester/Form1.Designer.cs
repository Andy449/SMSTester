namespace SMSTester
{
    partial class Form1
    {
        /// <summary>
        /// 必需的设计器变量。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 清理所有正在使用的资源。
        /// </summary>
        /// <param name="disposing">如果应释放托管资源，为 true；否则为 false。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows 窗体设计器生成的代码

        /// <summary>
        /// 设计器支持所需的方法 - 不要
        /// 使用代码编辑器修改此方法的内容。
        /// </summary>
        private void InitializeComponent()
        {
            this.textBoxBaudRate = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.labelbaudrate = new System.Windows.Forms.Label();
            this.buttonConnect = new System.Windows.Forms.Button();
            this.label2 = new System.Windows.Forms.Label();
            this.textBoxContext = new System.Windows.Forms.TextBox();
            this.buttonSend = new System.Windows.Forms.Button();
            this.comboBoxCOM = new System.Windows.Forms.ComboBox();
            this.richTextBoxMsg = new System.Windows.Forms.RichTextBox();
            this.buttonContacter = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // textBoxBaudRate
            // 
            this.textBoxBaudRate.Location = new System.Drawing.Point(112, 68);
            this.textBoxBaudRate.Name = "textBoxBaudRate";
            this.textBoxBaudRate.Size = new System.Drawing.Size(54, 21);
            this.textBoxBaudRate.TabIndex = 1;
            this.textBoxBaudRate.Text = "115200";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(27, 33);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(29, 12);
            this.label1.TabIndex = 2;
            this.label1.Text = "COM:";
            // 
            // labelbaudrate
            // 
            this.labelbaudrate.AutoSize = true;
            this.labelbaudrate.Location = new System.Drawing.Point(27, 71);
            this.labelbaudrate.Name = "labelbaudrate";
            this.labelbaudrate.Size = new System.Drawing.Size(59, 12);
            this.labelbaudrate.TabIndex = 3;
            this.labelbaudrate.Text = "BaudRate:";
            // 
            // buttonConnect
            // 
            this.buttonConnect.Location = new System.Drawing.Point(29, 105);
            this.buttonConnect.Name = "buttonConnect";
            this.buttonConnect.Size = new System.Drawing.Size(137, 33);
            this.buttonConnect.TabIndex = 4;
            this.buttonConnect.Text = "连接";
            this.buttonConnect.UseVisualStyleBackColor = true;
            this.buttonConnect.Click += new System.EventHandler(this.buttonConnect_Click);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(3, 170);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(41, 12);
            this.label2.TabIndex = 5;
            this.label2.Text = "内容：";
            // 
            // textBoxContext
            // 
            this.textBoxContext.Location = new System.Drawing.Point(40, 166);
            this.textBoxContext.Name = "textBoxContext";
            this.textBoxContext.Size = new System.Drawing.Size(352, 21);
            this.textBoxContext.TabIndex = 6;
            this.textBoxContext.Text = "2016-03-22 13:09:29,100K+200发生险情，请尽快到达现场处置";
            // 
            // buttonSend
            // 
            this.buttonSend.Location = new System.Drawing.Point(215, 105);
            this.buttonSend.Name = "buttonSend";
            this.buttonSend.Size = new System.Drawing.Size(137, 33);
            this.buttonSend.TabIndex = 7;
            this.buttonSend.Text = "发送";
            this.buttonSend.UseVisualStyleBackColor = true;
            this.buttonSend.Click += new System.EventHandler(this.buttonSend_Click);
            // 
            // comboBoxCOM
            // 
            this.comboBoxCOM.FormattingEnabled = true;
            this.comboBoxCOM.Items.AddRange(new object[] {
            "COM1",
            "COM2",
            "COM3",
            "COM4",
            "COM5",
            "COM6",
            "COM7",
            "COM8",
            "COM9",
            "COM10",
            "COM11",
            "COM12"});
            this.comboBoxCOM.Location = new System.Drawing.Point(62, 29);
            this.comboBoxCOM.Name = "comboBoxCOM";
            this.comboBoxCOM.Size = new System.Drawing.Size(104, 20);
            this.comboBoxCOM.TabIndex = 8;
            // 
            // richTextBoxMsg
            // 
            this.richTextBoxMsg.Location = new System.Drawing.Point(7, 193);
            this.richTextBoxMsg.Name = "richTextBoxMsg";
            this.richTextBoxMsg.Size = new System.Drawing.Size(385, 215);
            this.richTextBoxMsg.TabIndex = 9;
            this.richTextBoxMsg.Text = "";
            // 
            // buttonContacter
            // 
            this.buttonContacter.Location = new System.Drawing.Point(215, 29);
            this.buttonContacter.Name = "buttonContacter";
            this.buttonContacter.Size = new System.Drawing.Size(137, 33);
            this.buttonContacter.TabIndex = 11;
            this.buttonContacter.Text = "联系人";
            this.buttonContacter.UseVisualStyleBackColor = true;
            this.buttonContacter.Click += new System.EventHandler(this.buttonContacter_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(395, 416);
            this.Controls.Add(this.buttonContacter);
            this.Controls.Add(this.richTextBoxMsg);
            this.Controls.Add(this.comboBoxCOM);
            this.Controls.Add(this.buttonSend);
            this.Controls.Add(this.textBoxContext);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.buttonConnect);
            this.Controls.Add(this.labelbaudrate);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.textBoxBaudRate);
            this.Name = "Form1";
            this.Text = "Form1";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox textBoxBaudRate;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label labelbaudrate;
        private System.Windows.Forms.Button buttonConnect;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox textBoxContext;
        private System.Windows.Forms.Button buttonSend;
        private System.Windows.Forms.ComboBox comboBoxCOM;
        private System.Windows.Forms.RichTextBox richTextBoxMsg;
        private System.Windows.Forms.Button buttonContacter;
    }
}

