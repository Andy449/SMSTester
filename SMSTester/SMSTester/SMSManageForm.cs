using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using Nuctech.RDP.DomainModelLibrary;

namespace Nuctech.RMS_PC
{
    public partial class SMSManageForm : Form
    {
        public string SmsUser;
        public Dictionary<string, string> smsUser = new Dictionary<string, string>();

        public SMSManageForm()
        {
            InitializeComponent();
        }

        private void SMSManageForm_Load(object sender, EventArgs e)
        {
            //lvPhone.Items.Clear();
            //btnDelete.Enabled = false;     // ��ɾ����ť�û�
            //// ѡ�е�һ��
            //cbRole.SelectedIndex =1;
            //cbRole.Enabled = false;

            //CellphoneList cpl;
            //if (Program.frmMain.editMap == true)
            //{
            //    //cbRole.Items.Add("ϵͳ����Ա");
            //    cbRole.Enabled = true;
            //    // ��ȡ�ֻ��û���Ϣ�����ȴ����ݿ��ж�ȡ����ʧ�����ȡ��������
            //    cpl = SystemPara.GetCellphoneInfo();
            //}
            //else
            //{
            //    // ��ȡ�ֻ��û���Ϣ�����ȴ����ݿ��ж�ȡ����ʧ�����ȡ��������
            //    cpl = SystemPara.GetCellphoneInfo(SmsLevel.Leader);
            //}
           

            //if (cpl != null)
            //{
            //    foreach (Cellphone cellphone in cpl)
            //    {
            //        ListViewItem lvi = new ListViewItem(cellphone.MaterName);
            //        lvi.SubItems.Add(cellphone.PhoneNo);
            //        if (cellphone.Level == SmsLevel.Maintainer)
            //        {
            //            lvi.SubItems.Add("ϵͳ����Ա");
            //        }
            //        else
            //        {
            //            lvi.SubItems.Add("ϵͳ�û�");
            //        }
            //        lvi.SubItems.Add(cellphone.SiteId.ToString());
            //        lvPhone.Items.Add(lvi);
            //    }
            //}
            
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            if (!CheckCellphone())
            {
                return;
            }
            Cellphone cellphone = new Cellphone();
            cellphone.PhoneNo = tbCellphone.Text;
            cellphone.MaterName = tbName.Text;
            this.smsUser.Add(tbName.Text, tbCellphone.Text);
            if (cbRole.Text == "ϵͳ�û�")
            {
                cellphone.Level = SmsLevel.Leader;
            }
            else if (cbRole.Text == "ϵͳ����Ա")
            {
                cellphone.Level = SmsLevel.Maintainer;
            }
            cellphone.SiteId = 1;

                //if (SystemPara.InsertCellphoneInfo(cellphone) != 0)  // ���ɹ�
                //{
                    ListViewItem lvi = new ListViewItem(tbName.Text);
                    lvi.SubItems.Add(tbCellphone.Text);
                    lvi.SubItems.Add(cbRole.Text);

                    lvPhone.Items.Add(lvi);

                    tbName.Text = "";
                    tbCellphone.Text = "";

                //    Program.frmMain.publish.PublishSMSManage(1, cellphone);
                //}
                //else
                //{
                //    MessageBox.Show("�������ݿ�ʧ�ܣ�");
                //}          

        }

        // �������ĺϷ���
        private bool CheckCellphone()
        {
            tbName.Text = tbName.Text.Trim();
            if (tbName.Text.Length == 0)
            {
                MessageBox.Show("������������");
                return false;
            }

            if (tbCellphone.Text.Length != 11)
            {
                MessageBox.Show("�ֻ����볤�Ȳ���ȷ��");
                return false;
            }

            //if (SystemPara.GetCellphoneInfo(tbName.Text) != null)
            //{
            //    MessageBox.Show("���û��Ѿ����ڣ�");
            //    return false;
            //}
            //if (SystemPara.IsExist(tbCellphone.Text))
            //{
            //    MessageBox.Show("���ֻ������Ѿ����ڣ����������룡");
            //    return false;
            //}

            return true;
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            if (lvPhone.SelectedItems.Count > 0)
            {
                // ɾ�����ݿ����ֻ��û�����
                //int userId = this.FindSmsUserID(this.SmsUser);

                //int r = SystemPara.DeleteSmsUserInfo(userId);

                //if (r != 0)  // ���ݿ�ɾ���ɹ�
                //{
                    Cellphone cellphone = new Cellphone();
                    cellphone.PhoneNo = lvPhone.SelectedItems[0].SubItems[1].Text;
                    cellphone.MaterName = lvPhone.SelectedItems[0].SubItems[0].Text;

                    this.smsUser.Remove(lvPhone.SelectedItems[0].SubItems[0].Text);
                    lvPhone.Items.Remove(lvPhone.SelectedItems[0]);

                    tbName.Text = "";
                    tbCellphone.Text = "";

                //    Program.frmMain.publish.PublishSMSManage(2, cellphone);
                //}
                //else
                //{
                //    MessageBox.Show("����ɾ��ʧ�ܣ�");
                //}
            }
        }

        //private int FindSmsUserID(string userName)
        //{
        //    CellphoneList cpl = SystemPara.GetCellphoneInfo();

        //    foreach (Cellphone cellPhone in cpl)
        //    {
        //        if (cellPhone.MaterName == userName)
        //        {
        //            return cellPhone.CellphoneId;
        //        }
        //    }

        //    return 0;
        //}

        private void btnSave_Click(object sender, EventArgs e)
        {
        }

        private void lvPhone_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (lvPhone.SelectedItems.Count > 0)
            {
                btnDelete.Enabled = true;   // ��ɾ����ť�ÿ���

                tbName.Text = lvPhone.SelectedItems[0].Text;
                tbCellphone.Text = lvPhone.SelectedItems[0].SubItems[1].Text;
                if (lvPhone.SelectedItems[0].SubItems[2].Text.Equals("ϵͳ����Ա"))
                {
                    //ѡ�е�һ��
                    cbRole.SelectedIndex = 0;
                }
                else
                {   //ѡ�еڶ���
                    cbRole.SelectedIndex = 1;
                }

                this.SmsUser = lvPhone.SelectedItems[0].SubItems[0].Text;  // ѡ�����û���
            }
        }

        private void tbCellphone_KeyPress(object sender, KeyPressEventArgs e)
        {
            base.OnKeyPress(e);

            if ((!char.IsDigit(e.KeyChar)) && (e.KeyChar != '\b'))
            {
                e.Handled = true;
                return;
            }
        }

    }
}