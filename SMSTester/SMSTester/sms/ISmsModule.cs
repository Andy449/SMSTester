using System;
using System.Collections.Generic;
using System.Text;

namespace Nuctech.RDP.Peripherals
{
    public interface ISmsModule
    {
        /// <summary>
        /// 发送信息
        /// </summary>
        /// <param name="phoneNumber">手机号码</param>
        /// <param name="message">短信内容</param>
        /// <returns>false表示失败</returns>
        bool SendMessage(string phoneNumber, string message);

        /// <summary>
        /// 将message发送给多个手机号cellPhones
        /// </summary>
        /// <param name="cellPhones"></param>
        /// <param name="message"></param>
        /// <returns></returns>
        bool SendMessage(List<string> cellPhones, string message);

        /// <summary>
        /// 当短信模块的状态发送变化时
        /// </summary>
        event SmsModuleStatusEventHandler OnSmsModuleStatus;

        event SmsCommandOccurEventHandler OnSmsCommandOccured;

        event EventHandler<SMSReport> smsEventHandler;
    }
}
