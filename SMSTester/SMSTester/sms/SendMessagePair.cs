using System;
using System.Collections.Generic;
using System.Text;

namespace Nuctech.RDP.Peripherals
{
    /// <summary>
    /// 发送短信对
    /// </summary>
    class SendMessagePair
    {
        // 发送的短信最大为 65
        // 若被发送的信息的长度超过该长度，则需要在之前将信息分成大小合适的小段信息
        public static int MaxMessageLen = 65; 

        public string Phone;        // 接收信息的手机号码
        public string Message;      // 发送的信息（其长度应该 < MaxMessageLen）

        public SendMessagePair(string phone, string message)
        {
            this.Phone = phone;
            this.Message = message;
        }
    }
}
