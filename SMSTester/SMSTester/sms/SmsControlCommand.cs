using System;
using System.Collections.Generic;
using System.Text;

namespace Nuctech.RDP.Peripherals
{
    public class SmsControlCommand
    {
        /// <summary>
        /// 报警复位命令，即停止外围报警设备的所有报警动作（包括声光报警灯、无线列调、铁路告警灯等）
        /// 可以以两种方式短信可以实现此命令：
        ///     （1）存在于短信号码列表中的手机号可以直接发送短信：“0” -- 适合于系统用户
        ///     （2）任何手机可发送：“0 username psw”，（即命令 + 用户名 + 密码） -- 适合于系统后台管理
        /// </summary>
        static public string StopAlarm = "0";

        /// <summary>
        /// 获取某现场的所有传感器状态命令
        /// 例如： “GSS 2” -- 表示获取 siteId 为 2 的所有现场的传感器状态（传感器是否在线）
        /// </summary>
        static public string GetSensorsStatus = "GSS";

        /// <summary>
        /// 获取某解调仪上某通道的中心波长值
        /// 例如：“GSW 1 2” -- 表示获取标识为 1 的解调仪的第 2 通道的所有在线传感器的中心波长值（精确到小数点后面一位）
        /// </summary>
        static public string GetSensorsWvl = "GSW";

        /// <summary>
        /// 重新启动算法（需要用户名和密码）
        /// 例如：“AR username psw”
        /// </summary>
        static public string RebootAlgorithm = "RA";

        /// <summary>
        /// 重新启动计算机（需要用户名和密码）
        /// 例如：“RC username psw”
        /// </summary>
        static public string RebootComputer = "RC";

        /// <summary>
        /// 从收到的短信内容中解析出命令值（包括随后的参数值），皆以字符串的形式
        /// </summary>
        /// <param name="recivedMessage">接收到的短信内容</param>
        /// <returns>命令值</returns>
        static public List<string> ParseCommandValues(string recivedMessage)
        {
            List<string> commandValues = new List<string>();

            string[] commandContents = recivedMessage.Split(new char[] { ' ', '\t', '\n', ',', ';', ':', '，', '：', '；' });

            for (int i = 0; i < commandContents.Length; i++)
            {
                if (commandContents[i] != string.Empty)
                {
                    commandValues.Add(commandContents[i]);
                }
            }

            return commandValues;
        }
    }
}
