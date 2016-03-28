using System;
using System.Collections.Generic;
using System.Text;

namespace Nuctech.RDP.Peripherals
{
    public class SmsCommandEventArgs : EventArgs
    {
        /// <summary>
        /// 短信命令事件参数
        /// 当向短信模块发送命令时，传递此参数给系统
        /// </summary>
        private RecievedSMS recievedSms;
        public RecievedSMS RecievedSms
        {
            get { return recievedSms; }
            set { recievedSms = value; }
        }

        public SmsCommandEventArgs()
        {
        }

        public SmsCommandEventArgs(RecievedSMS sms)
        {
            this.recievedSms = sms;
        }
    }
}
