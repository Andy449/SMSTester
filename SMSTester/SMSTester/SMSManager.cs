using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using Nuctech.RDP.Peripherals;

namespace SMSTester
{
    public class SMSManager
    {
        private Thread threadForSMS;
        public string SMSCOM { get; set; }
        public int SMSBaudRate { get; set; }

        private ISmsModule smsModualManager = null;                    // 短信模块
        public ISmsModule SmsModualManager
        {
            get { return smsModualManager; }
        }

        public SMSManager()
        {
            //threadForSMS = new Thread(new ThreadStart(SMSProcess));
            //threadForSMS.IsBackground = true;
            //threadForSMS.Start();
        }

        public void SMSProcess()
        {
            ThreadPool.QueueUserWorkItem(new WaitCallback(Initialize), new object());
        }

        private void Initialize(Object ob)
        {
            smsModualManager = new SmsModule(this.SMSCOM, this.SMSBaudRate);
            smsModualManager.smsEventHandler += smsModualManager_smsEventHandler;
        }

        void smsModualManager_smsEventHandler(object sender, SMSReport e)
        {
            Console.WriteLine("收到短信设备事件");
            Console.WriteLine(e.Malfunction.Describe);
            OnReport(e);
        }

        public event EventHandler<SMSReport> smsHandler;
        private void OnReport(SMSReport report)
        {
            EventHandler<SMSReport> temp = Interlocked.CompareExchange(ref smsHandler, null, null);
            if(null!=temp)
            {
                temp(this, report);
            }
        }
    }
}
