using System;
using System.Collections.Generic;
using System.Text;
using System.IO.Ports;
using System.Threading;
using System.Diagnostics;

//using Nuctech.RDP.LogManager;

namespace Nuctech.RDP.Peripherals
{
    /// <summary>
    /// 短信模块的核心操作
    /// </summary>
    class SmsModuleKernel
    {
        // 短信模块所用到的串口
        private SerialPort serialPort = null;
        public SerialPort SerialPort
        {
            get { return serialPort; }
        }

        // 判断串口是否正常初始化 
        private bool serialPortIsInit = false;
        public bool SerialPortIsInit
        {
            get { return serialPortIsInit; }
        }
        // 串口名称，默认为“com1”
        private string portName = "com1";
        // 波特率，默认为115200
        private int baudRate = 115200;

        // 是否具备发送短信
        private bool canSendSMS = false;
        public bool CanSendSMS
        {
            get { return canSendSMS; }
            set { canSendSMS = value; }
        }

        // 发送短信的中心号码
        private string centerNumber = "";

        // 每次发送命令后需要停顿一段时间，否则从串口读的数据不完整，所以它的设定需要小心
        private int cmdSleepTime = 500;
        public int CmdSleepTime
        {
            get { return cmdSleepTime; }
        }
        // 每次发送信息后需要停顿一段时间，否则从串口读的数据不完整，所以它的设定需要小心（以后重构，应该消除这种影响）
        private int sendMsgSleepTime = 8000;
        // 保存每次发送命令的回复。发送成功，cmdReplay中有“OK”，失败则含有“ERROR”
        private string cmdReplay = "";

        string staticMsg = "";
        public string StaticMsg
        {
            get { return staticMsg; }
            set { staticMsg = value; }
        }

        private object locker = new object();       // 用于线程锁。因为线程heartMonitorThread和sendMessageThread都要用到同一个串口

        #region 构造函数

        public SmsModuleKernel(string portName, int baudRate)
        {
            Console.WriteLine("SmsModuleKernel:" + portName + "  " + baudRate);
            this.portName = portName;
            this.baudRate = baudRate;

            serialPort = new SerialPort(portName, baudRate);

            //serialPort.DataReceived += new SerialDataReceivedEventHandler(ProcessDataReceived);
        }

        ~SmsModuleKernel()
        {
            // 关闭串口，释放资源
            if (serialPort.IsOpen)
            {
                serialPort.Close();
            }
        }

        private void ProcessDataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            try
            {
                //cmdReplay = string.Empty;   // 读取串口数据之前清空

                SerialPort sp = (SerialPort)sender;
                cmdReplay += sp.ReadExisting();
            }
            catch (Exception ex)
            {
                Console.WriteLine("[SmsModuleKernel][ProcessDataReceived]" + ex.Message);
                canSendSMS = false;
            }
        }
        #endregion

        #region 串口相关的操作

        /// <summary>
        /// 初始化短信模块串口
        /// </summary>
        /// <returns>false表示初始化失败</returns>
        public bool InitSmsSerialPort()
        {
            try
            {
                // 清空串口serialPort的资源
                if (serialPort.IsOpen)
                {
                    serialPort.Close();
                }
                serialPort.Dispose();

                // 自动获取端口名称和波特率                

                // 设置串口serialPort的相关属性
                serialPort.PortName = this.portName;    // 端口号
                serialPort.BaudRate = this.baudRate;    //波特率
                serialPort.DtrEnable = true;
                serialPort.RtsEnable = true;
                serialPort.DataBits = 8;
                serialPort.Parity = Parity.None;   // 奇偶校验位
                serialPort.StopBits = StopBits.One;// 停止位
                serialPort.ReadTimeout = 500;     // 读超时时间
                serialPort.WriteTimeout = 500;    // 写超时时间
                serialPort.ReceivedBytesThreshold = 1;

                // 如果串口已经打开，则重新关闭再打开
                if (serialPort.IsOpen)
                {
                    serialPort.Close();
                    serialPort.Open();
                }
                else
                {
                    serialPort.Open();
                }

                // 初始化成功相应的标识
                serialPortIsInit = true;
                staticMsg = "串口初始化成功";

                return true;
            }
            catch (Exception ex)
            {
                // 初始化失败相应的标识
                serialPortIsInit = false;
                staticMsg = "串口初始化失败--" + ex.ToString();
                CloseSerialPort();
                return false;
            }
        }

        /// <summary>
        /// 关闭串口(本程序未用到)
        /// </summary>
        public void CloseSerialPort()
        {
            try
            {
                if (serialPort.IsOpen)
                {
                    serialPort.Close();
                }
            }
            catch
            {
                staticMsg = "串口关闭失败";
            }
        }

        public string SerialPortWrite(string cmd, int sleepTime)
        {
            byte[] buffer = Encoding.ASCII.GetBytes(cmd);
            return SerialPortWrite(buffer, 0, buffer.Length, sleepTime);
        }

        public string SerialPortWrite(byte[] buffer, int offset, int count, int sleepTime)
        {
            try
            {
                lock (locker)
                {
                    // 如果串口没有打开
                    if (!serialPort.IsOpen)
                    {
                        serialPort.Open();
                    }

                    // 发送命令
                    serialPort.Write(buffer, offset, count);

                    // 等待sleepTime 毫秒
                    if (sleepTime > 0)
                    {
                        Thread.Sleep(sleepTime);
                    }

                    // 读串口的数据
                    string readContent = string.Empty;
                    readContent += serialPort.ReadExisting();

                    return readContent;
                }                
            }
            catch (Exception ex)
            {
                //LogHelper.WriteLog("[SmsModuleKernel][SerialPortWrite]" + ex.Message);
                canSendSMS = false;
                return string.Empty;
            }
        }

        #endregion

        #region GPRS Modem 相关的操作

        /// <summary>
        /// 初始化短信模块
        /// </summary>
        /// <returns>是否成功</returns>
        public bool InitGsm()
        {
            try
            {
                // 设置 PDU 模式
                if(!CheckCmdReply( SerialPortWrite(SmsAtCommand.PduMode, this.cmdSleepTime)))
                {
                    canSendSMS = false;
                    //LogHelper.WriteLog("[SMS]CheckCmdReply failed!");
                    return false;
                }           

                // 获取中心号码                
                if (centerNumber.Length < 13)
                {
                    centerNumber = GetCenterNumber();
                }
                if (centerNumber == string.Empty)
                {
                    canSendSMS = false;
                    //LogHelper.WriteLog("[SMS]GetCenterNumber failed!");
                    return false;
                }

                // 测试（查询sim卡信息）
                DeleteAllSms();                

                //至此具备了发送条件
                canSendSMS = true;
                staticMsg = "短信模块初始化成功";

                return true;
            }
            catch (Exception ex)
            {
                canSendSMS = false;
                staticMsg = "短信模块初始化失败--" + ex.ToString();
                //LogHelper.WriteLog("[SMS]"+staticMsg);
                return false;
            }
        }

        /// <summary>
        /// 获取中心号码
        /// </summary>
        /// <returns>短信中心号码</returns>
        public string GetCenterNumber()
        {
            string replay = SerialPortWrite(SmsAtCommand.CenterNumber, this.cmdSleepTime);
            return replay.Substring(replay.IndexOf("86"), 13);
        }

        private void DeleteAllSms()
        {
            string replay = SerialPortWrite("AT+CPMS?\r", this.cmdSleepTime);
            string content = replay.Substring(replay.IndexOf("+CPMS:"));
            string[] strArray = content.Split(new char[] { ',' });
            
            int count = Int32.Parse(strArray[1]);
                       
            for (int index = count; index > 0; index--)
            {
                SerialPortWrite("AT+CMGD=" + index.ToString() + "\r", this.cmdSleepTime);
            }

        }

        private bool CheckCmdReply(string readContent)
        {
            // 普通命令(注意：不是每个命令都一定会返回“OK”)
            return readContent.Contains(SmsAtCommand.CmdReplayOk);
        }

        private bool CheckSendMsgReplay(string replay)
        {
            // 发送短信成功后必然出现cmgs：
            return replay.Contains(SmsAtCommand.CmdReplayOk);
        }


        public bool SendCommand(string command)
        {
            return CheckSendMsgReplay(SerialPortWrite(command, this.cmdSleepTime));           
        }

        /// <summary>
        /// 向号码number发送短信
        /// </summary>
        /// <param name="number">目标号码</param>
        /// <param name="msg">短信内容</param>
        /// <returns>false表示发送失败</returns>
        public bool SendMsg(string number, string msg)
        {
            // 如果不具备发送短信的状态，则退出
            if (!this.canSendSMS)
            {
                if (!InitGsm())
                {
                    return false;
                }
            }

            try
            {
                // PDU编码
                string encodedSms = PDUEncoderDecoder.EncodePDU(centerNumber, number, msg);

                // 发送命令
                string sendMsgCmd = SmsAtCommand.GetSendSmsCmd(PDUEncoderDecoder.GetLengthStringOfSent(encodedSms, centerNumber));
                
                // 具备发生短信的条件
                if (SerialPortWrite(sendMsgCmd, this.cmdSleepTime).Contains(">"))
                {
                    byte[] sendBuf = Encoding.ASCII.GetBytes(String.Format("{0}\x01a", encodedSms));
                    if (CheckSendMsgReplay(SerialPortWrite(sendBuf, 0, sendBuf.Length, this.sendMsgSleepTime)))
                    {
                        return true;
                    }
                }
                    
                return false;
            }
            catch (Exception ex)
            {
                //LogHelper.WriteLog("[SMS][SendMsg] SerialPortWrite failed! " + ex.Message);
                this.canSendSMS = false;
                return false;
            }
        }

        #endregion
    }
}
