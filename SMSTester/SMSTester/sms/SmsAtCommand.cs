using System;
using System.Collections.Generic;
using System.Text;

namespace Nuctech.RDP.Peripherals
{
    /// <summary>
    /// 短信模块AT命令
    /// </summary>
    public class SmsAtCommand
    {
        // 普通命令发送成功
        static public string CmdReplayOk = "OK";

        // 普通命令发送失败
        static public string CmdReplayError = "ERROR";

        // 准备发送短信命令发送成功标识
        static public string SendMsgRelayOk = "\r\n> ";

        // 短信发送成功的回复标识
        static public string SmsReplayOk = "CMGS";

        // 取消回显
        static public string ClearScreen = "ATE0\r";

        // 测试命令
        static public string Test = "AT\r";

        // 信号强度
        static public string SignalIntensity = "AT+CSQ\r";

        // 短信中心号码
        static public string CenterNumber = "AT+CSCA?\r";

        // 设置PDU模式
        static public string PduMode = "AT+CMGF=0\r";

        // 准备发送短息的命令
        static public string SendSmsCmd = "AT+CMGS";
        static public string GetSendSmsCmd(string nLength)
        {
            return SmsAtCommand.SendSmsCmd + String.Format("={0}\r", nLength);
        }
       
        // 准备接收短信的命令
        static public string RecieveSmsCmd = "AT+CMGL\r";

        // 删除所有已读已发信息
        static public string DeleteSmsCmd = "AT+CMGD=1, 2\r";
    }
}
