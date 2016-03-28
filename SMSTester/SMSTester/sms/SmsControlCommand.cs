using System;
using System.Collections.Generic;
using System.Text;

namespace Nuctech.RDP.Peripherals
{
    public class SmsControlCommand
    {
        /// <summary>
        /// ������λ�����ֹͣ��Χ�����豸�����б����������������ⱨ���ơ������е�����·�澯�Ƶȣ�
        /// ���������ַ�ʽ���ſ���ʵ�ִ����
        ///     ��1�������ڶ��ź����б��е��ֻ��ſ���ֱ�ӷ��Ͷ��ţ���0�� -- �ʺ���ϵͳ�û�
        ///     ��2���κ��ֻ��ɷ��ͣ���0 username psw������������ + �û��� + ���룩 -- �ʺ���ϵͳ��̨����
        /// </summary>
        static public string StopAlarm = "0";

        /// <summary>
        /// ��ȡĳ�ֳ������д�����״̬����
        /// ���磺 ��GSS 2�� -- ��ʾ��ȡ siteId Ϊ 2 �������ֳ��Ĵ�����״̬���������Ƿ����ߣ�
        /// </summary>
        static public string GetSensorsStatus = "GSS";

        /// <summary>
        /// ��ȡĳ�������ĳͨ�������Ĳ���ֵ
        /// ���磺��GSW 1 2�� -- ��ʾ��ȡ��ʶΪ 1 �Ľ���ǵĵ� 2 ͨ�����������ߴ����������Ĳ���ֵ����ȷ��С�������һλ��
        /// </summary>
        static public string GetSensorsWvl = "GSW";

        /// <summary>
        /// ���������㷨����Ҫ�û��������룩
        /// ���磺��AR username psw��
        /// </summary>
        static public string RebootAlgorithm = "RA";

        /// <summary>
        /// �����������������Ҫ�û��������룩
        /// ���磺��RC username psw��
        /// </summary>
        static public string RebootComputer = "RC";

        /// <summary>
        /// ���յ��Ķ��������н���������ֵ���������Ĳ���ֵ���������ַ�������ʽ
        /// </summary>
        /// <param name="recivedMessage">���յ��Ķ�������</param>
        /// <returns>����ֵ</returns>
        static public List<string> ParseCommandValues(string recivedMessage)
        {
            List<string> commandValues = new List<string>();

            string[] commandContents = recivedMessage.Split(new char[] { ' ', '\t', '\n', ',', ';', ':', '��', '��', '��' });

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
