using System;
using System.Collections.Generic;
using System.Text;

namespace Nuctech.RDP.Peripherals
{
    /*
     * 目前，发送短消息常用Text和PDU(Protocol Data Unit，协议数据单元)模式。使用Text模式收发短信代码简单，
     * 实现起来十分容易，但最大的缺点是不能收发中文短信；而PDU模式不仅支持中文短信，也能发送英文短信。PDU
     * 模式收发短信可以使用3种编码：7-bit、8-bit和UCS2编码。7-bit编码用于发送普通的ASCII字符，8-bit编码通
     * 常用于发送数据消息，UCS2编码用于发送Unicode字符。
     */
    
    /// <summary>
    /// PDU 编解码
    /// </summary>
    class PDUEncoderDecoder
    {
        /// <summary>
        /// SMS 的编码（此处为了方便，直接将短信内容编码成Unicode）
        /// </summary>
        /// <param name="strCenterNumber">短信中心号码</param>
        /// <param name="strNumber">目标号码</param>
        /// <param name="strSMScontent">发送内容</param>
        /// <returns>PDU编码</returns>
        static public string EncodePDU(string strCenterNumber, string strNumber, string strSMScontent)
        {
            string strEncoded = string.Empty;

            strEncoded = String.Format("{0}11000D91{1}000800{2}", EncodeServiceCenterAddress(strCenterNumber), EncodeReciverAddress(strNumber), EncodeMessagePDU(strSMScontent));
              
            return strEncoded;
        }

        /// <summary>
        /// 获取要发送内容的长度字符串
        /// 该长度两部分组成: 接收手机号加上要发送的内容
        /// </summary>
        /// <param name="strEncoded">经由函数SMSEncode编码后的结果</param>
        /// <param name="strCenterNumber">短信中心号码</param>
        /// <returns>发送内容的长度字符串</returns>
        static public string GetLengthStringOfSent(string strEncoded, string strCenterNumber)
        {
            return String.Format("{0:D2}", (strEncoded.Length - EncodeServiceCenterAddress(strCenterNumber).Length) / 2);
        }

        /// <summary>
        /// 短信中心号编码
        /// 1，将奇数位和偶数位交换。
        /// 2，短信中心号奇偶数交换后，看看长度是否为偶数，如果不是，最后添加F
        /// 3，加上短信中心号类型，91为国际化
        /// 4，计算编码后的短信中心号长度，并格化成二位的十六进制
        /// </summary>
        /// <param name="sca">短信中心号码</param>
        /// <returns></returns>
        static private string EncodeServiceCenterAddress(string sca)
        {
            string scaEncoded = string.Empty;

            if (!(sca.Substring(0, 2) == "86"))
            {
                sca = String.Format("86{0}", sca);      //检查当前接收手机号是否按标准格式书写，不是，就补上“86”
            }

            // 奇偶互换
            int nLength = sca.Length;
            for (int i = 1; i < nLength; i += 2)
            {
                scaEncoded += sca[i];
                scaEncoded += sca[i - 1];
            }

            // 是否为偶数，不是就加上F，并对最后一位与加上的F位互换
            if (!(nLength % 2 == 0))
            {
                scaEncoded += 'F';
                scaEncoded += sca[nLength - 1];
            }

            scaEncoded = String.Format("91{0}", scaEncoded);                      //加上91,代表短信中心类型为国际化
            scaEncoded = String.Format("{0:X2}{1}", scaEncoded.Length / 2, scaEncoded);    //编码后短信中心号长度，并格式化成二位十六制

            return scaEncoded;
        }

        /// <summary>
        /// 接收短信人的手机号码编码
        /// </summary>
        /// <param name="ra">接收者的号码</param>
        /// <returns>PDU编码</returns>
        static private string EncodeReciverAddress(string ra)
        {
            string raEncoded = string.Empty;

            // 检查当前接收手机号是否按标准格式书写，不是，就补上“86”
            if (!(ra.Substring(0, 2) == "86"))
            {
                ra = String.Format("86{0}", ra);     
            }

            // 将奇数位和偶数位交换
            int nLength = ra.Length;
            for (int i = 1; i < nLength; i += 2)                 
            {
                raEncoded += ra[i];
                raEncoded += ra[i - 1];
            }

            // 是否为偶数，不是就加上F，并对最后一位与加上的F位互换
            if (!(nLength % 2 == 0))
            {
                raEncoded += 'F';
                raEncoded += ra[nLength - 1];
            }

            return raEncoded;
        }

        /// <summary>
        /// 函数功能：短信内容编码
        /// 函数名称：smsPDUEncoded(string srvContent)
        /// 参    数：srvContent 要进行转换的短信内容,string类型
        /// 返 回 值：编码后的短信内容，string类型
        /// 函数说明：
        ///          1，采用Big-Endian 字节顺序的 Unicode 格式编码，也就说把高低位的互换在这里完成了
        ///          2，将转换后的短信内容存进字节数组
        ///          3，去掉在进行Unicode格式编码中，两个字节中的"-",例如：00-21，变成0021
        ///          4，将整条短信内容的长度除2，保留两位16进制数
        /// </summary>
        static private string EncodeMessagePDU(string srvContent)
        {
            Encoding encodingUTF = System.Text.Encoding.BigEndianUnicode;
            string contentEncoded = string.Empty;
            byte[] encodedBytes = encodingUTF.GetBytes(srvContent);
            for (int i = 0; i < encodedBytes.Length; i++)
            {
                contentEncoded += BitConverter.ToString(encodedBytes, i, 1);
            }
            contentEncoded = String.Format("{0:X2}{1}", contentEncoded.Length / 2, contentEncoded);

            return contentEncoded;
        }

        /// <summary>
        /// PDU 解码
        /// </summary>
        /// <param name="strPDU">PDU编码的输入数据（即从“0891”开始一直到最后）</param>
        /// <returns>接收到的短信息类</returns>
        static public RecievedSMS DecodePDU(string strPDU)
        {
            RecievedSMS recivedSms = new RecievedSMS();

            // 解析短信中心号码
            int lenSCA = Convert.ToInt32(strPDU.Substring(0, 2), 16) * 2 + 2;       // 短消息中心占长度13
            recivedSms.ServiceCenterAddress = ParseServiceCenterAddress(strPDU.Substring(0, lenSCA));

            // 解析发送者号码
            int lenOA = Convert.ToInt32(strPDU.Substring(lenSCA + 2, 2), 16);       // OA占用长度16         

            if (lenOA % 2 == 1)            
            {
                lenOA++;        // 奇数则加1 F位
            }

            lenOA += 4;         // 加号码编码的头部长度21:   
            recivedSms.OriginatorAddress = ParseOriginatorAddress(strPDU.Substring(lenSCA + 2, lenOA));

            string dataCodingScheme = strPDU.Substring(lenSCA + lenOA + 4, 2);             //DCS赋值，区分解码7bit还是USC2解码

            // 解析时间
            recivedSms.ServiceCenterTimeStamp = ParseServiceCenterTimeStamp(strPDU.Substring(lenSCA + lenOA + 6, 14));

            // 解析用户数据
            int lenUD = Convert.ToInt32(strPDU.Substring(lenSCA + lenOA + 20, 2), 16) * 2;
            string userPDUData = strPDU.Substring(lenSCA + lenOA + 22, lenUD);
            string userData = string.Empty;
            if (dataCodingScheme == "08" || dataCodingScheme == "18")
            {
                userData = PDUUCS2Decode(userPDUData);
            }
            else
            {
                userData = PDU7bitDecode(userPDUData.Trim());
            }

            recivedSms.UserData = userData;

            return recivedSms;
        }

        /// <summary>
        /// 解析段信中心的号码
        /// </summary>
        /// <param name="scaPDU"></param>
        /// <returns></returns>
        static private string ParseServiceCenterAddress(string scaPDU)
        {
            int len = Convert.ToInt32(scaPDU.Substring(0, 2)) * 2 + 2;      //获取SCA长度(总共应该是18)
            string result = scaPDU.Substring(4, len - 4);                   //去掉起头部分"0891"

            result = ParityExchange(result);            //奇偶互换

            result = result.TrimEnd('F', 'f');          //去掉结尾F

            return result;
        }

        /// <summary>
        /// 奇偶互换
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        static private string ParityExchange(string str)
        {
            string result = string.Empty;

            if (str.Length % 2 != 0)
            {
                str += "F";
            }

            for (int i = 0; i < str.Length; i += 2)
            {
                result += str[i + 1];
                result += str[i];

            }

            return result;
        }

        /// <summary>
        /// 解析发送者的手机号码
        /// </summary>
        /// <param name="strPDU"></param>
        /// <returns></returns>
        static private string ParseOriginatorAddress(string strPDU)
        {
            int len = Convert.ToInt32(strPDU.Substring(0, 2), 16);    //十六进制字符串转为整形数据

            string result = string.Empty;
            if (len % 2 == 1)       
            {
                len++;      //号码长度是奇数，长度加1 编码时加了F 
            }

            result = strPDU.Substring(4, len);                  //去掉头部
            result = ParityExchange(result).TrimEnd('F', 'f');    //奇偶互换，并去掉结尾F

            return result;
        }

        /// <summary>
        /// 解析短信中心的时间戳
        /// </summary>
        /// <param name="serviceCenterTimeStamp"></param>
        /// <returns></returns>
        static private string ParseServiceCenterTimeStamp(string serviceCenterTimeStamp)
        {
            string result = ParityExchange(serviceCenterTimeStamp);     //奇偶互换
            result = "20" + result.Substring(0, 12);                    //年加开始的“20”

            return result;
        }

        static private string PDU7bitDecode(string userPDUData)
        {
            string result = string.Empty;
            byte[] b = new byte[100];   // 为什么是100
            string temp = string.Empty;

            for (int i = 0; i < userPDUData.Length; i += 2)
            {
                b[i / 2] = (byte)Convert.ToByte((userPDUData[i].ToString() + userPDUData[i + 1].ToString()), 16);
            }

            int j = 0;
            int tmp = 1;

            while (j < userPDUData.Length / 2 - 1)
            {
                string s = string.Empty;

                s = Convert.ToString(b[j], 2);

                while (s.Length < 8)
                {
                    s = "0" + s;
                }

                result += (char)Convert.ToInt32(s.Substring(tmp) + temp, 2);        //加入一个字符 结果集 tmp 上一位组剩余

                temp = s.Substring(0, tmp);             //前一位组多的部分

                if (tmp > 6)
                {
                    result += (char)Convert.ToInt32(temp, 2);
                    temp = string.Empty;
                    tmp = 0;
                }

                tmp++;
                j++;

                if (j == userPDUData.Length / 2 - 1)   // 最后一个字符
                {
                    result += (char)Convert.ToInt32(Convert.ToString(b[j], 2) + temp, 2);
                }
            }

            int le = result.Length;
            Console.WriteLine("[PDUEncoderDecoder][PDU7bitDecode]" + result);

            return result;
        }

        /// <summary>
        /// PDU UCS2 解码
        /// </summary>
        /// <param name="userData"></param>
        /// <returns></returns>
        static private string PDUUCS2Decode(string userPDUData)
        {
            string result = string.Empty;

            //四个一组，每组译为一个USC2字符 
            for (int i = 0; i < userPDUData.Length; i += 4)
            {
                string temp = userPDUData.Substring(i, 4);
                int byte1 = Convert.ToInt16(temp, 16);
                result += ((char)byte1).ToString();
            }

            return result;
        }

    }
}
