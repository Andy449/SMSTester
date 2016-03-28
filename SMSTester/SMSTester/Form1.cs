using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Threading;
using Nuctech.RMS_PC;

namespace SMSTester
{    
    public partial class Form1 : Form
    {
        SMSManager sms = null;
        private List<string> currentDestPhone = new List<string>();
        private bool isConnected = false;

        public Form1()
        {
            InitializeComponent();
            sms = new SMSManager();
            sms.smsHandler += sms_smsHandler;
        }

        void sms_smsHandler(object sender, Nuctech.RDP.Peripherals.SMSReport e)
        {
            Console.WriteLine("From smsManager");            
            this.SetText(e.Malfunction.Describe+"\n");
            if (0 == e.Malfunction.StateCode)
                this.isConnected = true;
            else
                this.isConnected = false;
        }


        private void buttonConnect_Click(object sender, EventArgs e)
        {
            //if(null==this.comboBoxCOM.SelectedItem)
            //{
            //    Console.WriteLine("null");
            //    return;
            //}
            if (string.Empty == this.comboBoxCOM.Text)
            {
                Console.WriteLine("null");
                this.SetText("COM口不能为空！" + "\n");
                return;
            }
            if(string.Empty==this.textBoxBaudRate.Text.Trim())
            {
                Console.WriteLine("null null");
                this.SetText("波特率不能为空！" + "\n");
                return;
            }
            string com = this.comboBoxCOM.Text;
            Console.WriteLine(com);
            string bd = this.textBoxBaudRate.Text;
            Console.WriteLine(bd);
            sms.SMSCOM = com;
            sms.SMSBaudRate = Convert.ToInt32(bd);
            sms.SMSProcess();
        }

        private void buttonSend_Click(object sender, EventArgs e)
        {
            if (this.isConnected == false)
            {
                this.SetText("设备还未就绪！" + "\n");
                return;
            }
            if(string.Empty==this.textBoxContext.Text.Trim())
            {
                Console.WriteLine("Context is null");
                this.SetText("短信内容不能为空！" + "\n");
            }
            this.richTextBoxMsg.AppendText(this.textBoxContext.Text + "\n");
            foreach (KeyValuePair<string,string> kvp in this.form.smsUser)
            {
                this.currentDestPhone.Add(kvp.Value);
            }
            this.sms.SmsModualManager.SendMessage(this.currentDestPhone,this.textBoxContext.Text);
        }

        private delegate void setTextBox(string text);
        private void SetText(string text)
        {
            if(this.richTextBoxMsg.InvokeRequired)
            {
                setTextBox d = new setTextBox(SetText);
                this.Invoke(d, new object[] { text });
            }
            else
            {
                this.richTextBoxMsg.Text = text;
            }
        }
        private SMSManageForm form = new SMSManageForm();
        private void buttonContacter_Click(object sender, EventArgs e)
        {            
            form.ShowDialog();
        }
    }
}
