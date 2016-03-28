using System;
using System.Collections.Generic;
using System.Text;

namespace Nuctech.RDP.Peripherals
{
    /// <summary>
    /// ����ģ����յ��Ķ�������
    /// </summary>
    public class RecievedSMS
    {
        /// <summary>
        /// ���ŷ����ߵĺ���
        /// </summary>
        private string originatorAddress;
        public string OriginatorAddress
        {
            get { return originatorAddress; }
            set { originatorAddress = value; }
        }

        /// <summary>
        /// ���ŷ�������ת�����ŵ�ʱ�䣬���磺20131220133541 (2013��12��20��13��35��41��)
        /// </summary>
        private string serviceCenterTimeStamp;
        public string ServiceCenterTimeStamp
        {
            get { return serviceCenterTimeStamp; }
            set { serviceCenterTimeStamp = value; }
        }

        /// <summary>
        /// �����յ��ķ����߷�������Ϣ
        /// </summary>
        private string userData;
        public string UserData
        {
            get { return userData; }
            set { userData = value; }
        }

        /// <summary>
        /// ���ŷ������ĺ���
        /// </summary>
        private string serviceCenterAddress;
        public string ServiceCenterAddress
        {
            get { return serviceCenterAddress; }
            set { serviceCenterAddress = value; }
        }
    }
}
