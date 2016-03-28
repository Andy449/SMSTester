using System;
using System.Collections.Generic;
using System.Text;

namespace Nuctech.RDP.Peripherals
{
    public class SmsCommandEventArgs : EventArgs
    {
        /// <summary>
        /// ���������¼�����
        /// �������ģ�鷢������ʱ�����ݴ˲�����ϵͳ
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
