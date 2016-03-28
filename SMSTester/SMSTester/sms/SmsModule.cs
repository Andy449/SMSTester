using System;
using System.Collections.Generic;
using System.Text;
using System.IO.Ports;
using System.Threading;
using System.Diagnostics;
using System.Timers;

//using Nuctech.RDP.LogManager;
using Nuctech.RDP.DomainModelLibrary;

namespace Nuctech.RDP.Peripherals
{
    /// <summary>
    /// 短信模块的实现
    /// </summary>
    public class SmsModule : ISmsModule
    {
        // 短信模块的核心功能类
        private SmsModuleKernel smsModuleKernel = null;
        // 该队列保存待发的短信
        private Queue<SendMessagePair> sendMessagePairs = null;

        // 短信模块的心跳监控线程
        private System.Timers.Timer timerForHeart;
        private const int heartMonitorTimeSpan = 10000;    // 心跳监测的时间间隔(1分钟*3=3分钟)
        private const int heartMonitorTries = 3;          // 心跳监测尝试次数（若连续heartMonitorTries次心跳监测不成功，则认为短信模块有问题）
        private int heartMonitorFailedNum = 0;      // 心跳监测失败的次数     
        //private Thread heartMonitorThread = null;

        // 短信模块的发送短信的线程（所有短信的发生皆要通过线程完成，因为发送短信需要时间，不能因为发送短信而阻塞主线程）
        private Thread sendMessageThread = null;
        private AutoResetEvent sendMessageEvent = new AutoResetEvent(false);

        // 短信模块的接收短信线程
        //private Thread recieveMessageThread = null;
        //private int recieveMessageTimeSpan = 5000; 

        // 短信模块原始的状态
        private SmsModuleStatus originalStatus = SmsModuleStatus.None;
        // 短信模块当前的状态
        private SmsModuleStatus currentStatus = SmsModuleStatus.None;

        // private object locker = new object();       // 用于线程锁。因为线程heartMonitorThread和sendMessageThread都要用到同一个串口

        #region 构造函数

        /// <summary>
        /// 构造函数，必须明确其串口号和波特率
        /// </summary>
        /// <param name="portName">串口号，例如COM2等</param>
        /// <param name="baudRate">波特率</param>
        public SmsModule(string portName, int baudRate)
        {
            Console.WriteLine("SmsModule");
            //心跳定时器
            timerForHeart = new System.Timers.Timer();
            timerForHeart.Elapsed += timerForHeart_Elapsed;
            timerForHeart.Interval = heartMonitorTimeSpan;
            //初始化设备
            smsModuleKernel = new SmsModuleKernel(portName, baudRate);
            sendMessagePairs = new Queue<SendMessagePair>();

            Initialize();
            //ProcessSmsStatus();
            timerForHeart.Start();

            // 开启心跳控线程（用于监测短信模块的状态）
            //heartMonitorThread = new Thread(new ThreadStart(StartHeartMonitor));
            //heartMonitorThread.IsBackground = true;      // 这个很重要，否则程序关闭后，线程不会退出
            //heartMonitorThread.Start();

            // 开启发送短信的线程
            sendMessageThread = new Thread(new ThreadStart(StartSendMessage));
            sendMessageThread.IsBackground = true;
            sendMessageThread.Start();

            // 开启接收短信的线程//暂时不用
            //recieveMessageThread = new Thread(new ThreadStart(StartRecieveMessage));
            //recieveMessageThread.IsBackground = true;
            //recieveMessageThread.Start();
        }        

        ~SmsModule()
        {
            // 关闭串口
            smsModuleKernel.CloseSerialPort();
            // 关闭线程
            //if (null != heartMonitorThread)
            //{
            //    heartMonitorThread.Abort();
            //    heartMonitorThread = null;
            //}

            if (sendMessageThread != null)
            {
                sendMessageThread.Abort();
                sendMessageThread = null;
            }

            //if (recieveMessageThread != null)
            //{
            //    recieveMessageThread.Abort();
            //    recieveMessageThread = null;
            //}
        }

        #endregion


        /// <summary>
        /// 初始化（包括串口的初始化和GSM的初始化）
        /// </summary>
        public void Initialize()
        {
            // 串口初始化
            bool spStatus = smsModuleKernel.InitSmsSerialPort();
            if (spStatus)
                //LogHelper.WriteLog("sms串口初始化成功");
                Console.WriteLine("sms串口初始化成功");
            // GSM初始化
            bool smsStatus = smsModuleKernel.InitGsm();
            if (smsStatus)
                //LogHelper.WriteLog("GSM初始化成功");
                Console.WriteLine("GSM初始化成功");

            // 串口初始化和GSM初始化都成功，SmsModuleStatus.OK
            this.currentStatus = (spStatus && smsStatus) ? SmsModuleStatus.OK : SmsModuleStatus.Failed;
        }       

        #region 心跳部分
        /// <summary>
        /// 开始心跳监控线程
        /// 因为都要向串口写命令，所以此线程也发送短信的线程会冲突。解决办法：线程锁：this.locker
        /// </summary>
        //private void StartHeartMonitor()
        //{
        //    while (true)
        //    {
        //        Thread.Sleep(heartMonitorTimeSpan); // Sleep放置在前面可以延迟触发事件（可能事件还未订阅）

        //        try
        //        {
        //            if (this.smsModuleKernel.SendCommand(SmsAtCommand.SignalIntensity))
        //            {
        //                // 能够正常通信
        //                heartMonitorFailedNum = 0;
        //                this.currentStatus = SmsModuleStatus.OK;
        //            }
        //            else
        //            {
        //                // 不能正常通信
        //                heartMonitorFailedNum++;
        //            }
        //        }
        //        catch
        //        {
        //            // 不能正常通信
        //            heartMonitorFailedNum++;
        //        }
        //        finally
        //        {
        //            if (heartMonitorFailedNum >= heartMonitorTries)
        //            {
        //                this.currentStatus = SmsModuleStatus.Failed;
        //            }

        //            ProcessSmsStatus();
        //        }
        //    }
        //}
        void timerForHeart_Elapsed(object sender, ElapsedEventArgs e)
        {
            try
            {
                if (this.smsModuleKernel.SendCommand(SmsAtCommand.SignalIntensity))
                {
                    // 能够正常通信
                    heartMonitorFailedNum = 0;
                    this.currentStatus = SmsModuleStatus.OK;
                }
                else
                {
                    // 不能正常通信
                    heartMonitorFailedNum++;
                }
            }
            catch
            {
                // 不能正常通信
                heartMonitorFailedNum++;
            }
            finally
            {
                if (heartMonitorFailedNum >= heartMonitorTries)
                {
                    this.currentStatus = SmsModuleStatus.Failed;
                }

                ProcessSmsStatus();
            }
        }
        #endregion

        #region 发送/接收线程部分
        /// <summary>
        /// 开始发送短信线程
        /// </summary>
        private void StartSendMessage()
        {
            while (true)
            {
                this.sendMessageEvent.WaitOne();
                
                while (this.sendMessagePairs.Count > 0)
                {
                    SendMessagePair messagePair = this.sendMessagePairs.Dequeue();
                    if (!smsModuleKernel.SendMsg(messagePair.Phone, messagePair.Message))
                    {
                        //LogHelper.WriteLog("SMS send failed! (" + messagePair.Phone + ": " + messagePair.Message + ")");
                        Console.WriteLine("SMS send failed! (" + messagePair.Phone + ": " + messagePair.Message + ")");
                    }

                    //for (int iCount = 0; iCount < 3; iCount++)
                    //{
                    //    if (!smsModuleKernel.SendMsg(messagePair.Phone, messagePair.Message))
                    //        break;
                    //    else
                    //        LogHelper.WriteLog("SMS send failed! (" + messagePair.Phone + ": " + messagePair.Message + ")");
                    //}
                    //this.sendMessagePairs.Enqueue(messagePair); 
                }
                //本次发送完毕，心跳重新开始
                timerForHeart.Start();
            }
        }

        /// <summary>
        /// 开始接收短信线程
        /// 因为都要向串口写命令，所以此线程与发送短信的线程会冲突。解决办法：线程锁：this.locker
        /// </summary>
        //private void StartRecieveMessage()
        //{
        //    while (true)
        //    {
        //        Thread.Sleep(recieveMessageTimeSpan); // Sleep放置在前面可以延迟触发事件（可能事件还未订阅）

        //        try
        //        {
        //            string cmdReplay = this.smsModuleKernel.SerialPortWrite(SmsAtCommand.RecieveSmsCmd, this.smsModuleKernel.CmdSleepTime);

        //            // 调试
        //            if (cmdReplay.Contains("+CMGL:"))
        //            {
        //                // 解析短信
        //                string pdustr = cmdReplay.Substring(cmdReplay.IndexOf("+CMGL:"));
        //                RecievedSMS recievedSms = PDUEncoderDecoder.DecodePDU(pdustr.Substring(pdustr.IndexOf("0891")));
                        
        //                // 保存日志
        //                LogHelper.WriteLog("Recieve a messge: " + recievedSms.UserData + " from " + recievedSms.OriginatorAddress);

        //                // 回复短信
        //                //int len = recievedSms.OriginatorAddress.Length;
        //                //SendMessage(recievedSms.OriginatorAddress.Substring(len - 11), recievedSms.UserData + " 已收到！");

        //                // 删除短信(防止短息积累，仅删除第一条即可)
        //                this.smsModuleKernel.SendCommand(SmsAtCommand.DeleteSmsCmd);

        //                // 短信命令的处理
        //                OnSmsCommandOccuredHandler(new object(), new SmsCommandEventArgs(recievedSms));                        
        //            }
        //        }
        //        catch(Exception ex)
        //        {
        //            // 不能正常通信
        //            LogHelper.WriteLog("[SmsModule][StartRecieveMessage]" + ex.Message);
        //        }
        //    }
        //}
        #endregion   

        #region 接口函数

        /// <summary>
        /// 发送信息:短信设备接收到命令，获取中心波长等，通过这个接口发送
        /// </summary>
        /// <param name="phoneNumber">手机号码</param>
        /// <param name="message">短信内容</param>
        /// <returns>false表示失败</returns>
        public bool SendMessage(string phoneNumber, string message)
        {
            try
            {
                //LogHelper.WriteLog("[SmsModule][SendMessage-s]" + message);
                // 检查是否为手机号
                if (!IsCellPhoneNumber(phoneNumber))
                {
                    return false;
                }

                CreateSendMessagePairs(phoneNumber, message);

                this.sendMessageEvent.Set();

                return true;
            }
            catch (Exception ex)
            {
                Trace.WriteLine("[SmsModule][SendMessage]"+ex.Message);
                smsModuleKernel.CanSendSMS = false;
                return false;
            }
        }

        /// <summary>
        /// 根据message 的长度来分拆 message 发送
        /// 因为每条短信，最多一次大概可以发65个字符
        /// </summary>
        /// <param name="phoneNumber"></param>
        /// <param name="message"></param>
        private void CreateSendMessagePairs(string phoneNumber, string message)
        {
            string restMessage = message;
            string firstPart = "";
            do
            {
                int len = restMessage.Length > SendMessagePair.MaxMessageLen ? SendMessagePair.MaxMessageLen : restMessage.Length;
                firstPart = restMessage.Substring(0, len);
                this.sendMessagePairs.Enqueue(new SendMessagePair(phoneNumber, firstPart));             

                // 循环获取 leftMessage
                restMessage = restMessage.Substring(len);

            } while (restMessage.Length > 0);
        }

        /// <summary>
        /// 将message发送给多个手机号cellPhones
        /// </summary>
        /// <param name="cellPhones"></param>
        /// <param name="message"></param>
        /// <returns></returns>
        public bool SendMessage(List<string> cellPhones, string message)
        {
            //LogHelper.WriteLog("[SmsModule][SendMessage-List]" + message);
            try
            {
                timerForHeart.Stop();//报警期间停止心跳
                Thread.Sleep(500);//等待本次心跳结束
                
                foreach (string phoneNumber in cellPhones)
                {
                    // 检查是否为手机号
                    if (!IsCellPhoneNumber(phoneNumber))
                    {
                        continue;
                    }
                    CreateSendMessagePairs(phoneNumber, message);
                }
                this.sendMessageEvent.Set();

                return true;
            }
            catch (Exception ex)
            {
                //LogHelper.WriteLog("[SmsModule][SendMessage]" + ex.Message);
                smsModuleKernel.CanSendSMS = false;
                return false;
            }
        }

        /// <summary>
        /// 判断是不是手机号
        /// </summary>
        /// <param name="phoneNumber">手机号</param>
        /// <returns>是否是手机号</returns>
        public bool IsCellPhoneNumber(string phoneNumber)
        {
            return System.Text.RegularExpressions.Regex.IsMatch(phoneNumber, @"^[1]+[3,5,8]+\d{9}");
        }
        #endregion

        #region 状态处理

        private void ProcessSmsStatus()
        {
            // 如果状态发送变动，才触发事件
            if (this.currentStatus != this.originalStatus)
            {
                this.originalStatus = this.currentStatus;

                MalfunctionReport mr = new MalfunctionReport();
                mr.MalfunType = MalfunctionType.SMS;
                //mr.AttachementCode = PeripheralsManager.SMSAttachedCode;
                mr.TimeStamp = TimeConverter.DateTimeToUtc(DateTime.Now);
                if (SmsModuleStatus.OK == this.currentStatus)
                {
                    mr.Describe = "短信报警设备正常";
                    mr.StateCode = ReportState.Resume;
                }
                else
                {
                    mr.Describe = "短信报警设备异常";
                    mr.StateCode = ReportState.Create;
                }
                SMSReport report = new SMSReport();
                report.Malfunction = mr;
               
                OnReport(report);               
            }
        }

        public event EventHandler<SMSReport> smsEventHandler = null;
        private void OnReport(SMSReport report)
        {
            EventHandler<SMSReport> tempReport = Interlocked.CompareExchange(ref smsEventHandler, null, null);
            if(null!=tempReport)
            {
                tempReport(this,report);
            }
        }

        /// <summary>
        /// 短信模块的状态变化处理
        /// </summary>
        public event SmsModuleStatusEventHandler OnSmsModuleStatus = null;
        private void OnSmsModuleStatusHandler(object sender, SmsModuleStatus args)
        {
            if (OnSmsModuleStatus != null)
            {
                OnSmsModuleStatus(sender, args);
            }
        }

        /// <summary>
        /// 当向短信模块发送控制命令时，触发事件
        /// </summary>
        public event SmsCommandOccurEventHandler OnSmsCommandOccured = null;
        private void OnSmsCommandOccuredHandler(object sender, SmsCommandEventArgs args)
        {
            if (OnSmsCommandOccured != null)
            {
                OnSmsCommandOccured(sender, args);
            }
        }

        #endregion
    }

    public class SMSReport:EventArgs
    {
        private MalfunctionReport malfunction;
        public MalfunctionReport Malfunction
        {
            get { return malfunction; }
            set { malfunction = value; }
        }
    }
}
