using System;
using System.Collections.Generic;
using System.Text;

namespace Nuctech.RDP.Peripherals
{
    /// <summary>
    /// 短信模块接收到的短信内容
    /// </summary>
    public class RecievedSMS
    {
        /// <summary>
        /// 短信发送者的号码
        /// </summary>
        private string originatorAddress;
        public string OriginatorAddress
        {
            get { return originatorAddress; }
            set { originatorAddress = value; }
        }

        /// <summary>
        /// 短信服务中心转发短信的时间，例如：20131220133541 (2013年12月20日13点35分41秒)
        /// </summary>
        private string serviceCenterTimeStamp;
        public string ServiceCenterTimeStamp
        {
            get { return serviceCenterTimeStamp; }
            set { serviceCenterTimeStamp = value; }
        }

        /// <summary>
        /// 所接收到的发送者发来的信息
        /// </summary>
        private string userData;
        public string UserData
        {
            get { return userData; }
            set { userData = value; }
        }

        /// <summary>
        /// 短信服务中心号码
        /// </summary>
        private string serviceCenterAddress;
        public string ServiceCenterAddress
        {
            get { return serviceCenterAddress; }
            set { serviceCenterAddress = value; }
        }
    }
}
