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
            //btnDelete.Enabled = false;     // 将删除按钮置灰
            //// 选中第一项
            //cbRole.SelectedIndex =1;
            //cbRole.Enabled = false;

            //CellphoneList cpl;
            //if (Program.frmMain.editMap == true)
            //{
            //    //cbRole.Items.Add("系统管理员");
            //    cbRole.Enabled = true;
            //    // 读取手机用户信息：首先从数据库中读取，若失败则读取本地数据
            //    cpl = SystemPara.GetCellphoneInfo();
            //}
            //else
            //{
            //    // 读取手机用户信息：首先从数据库中读取，若失败则读取本地数据
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
            //            lvi.SubItems.Add("系统管理员");
            //        }
            //        else
            //        {
            //            lvi.SubItems.Add("系统用户");
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
            if (cbRole.Text == "系统用户")
            {
                cellphone.Level = SmsLevel.Leader;
            }
            else if (cbRole.Text == "系统管理员")
            {
                cellphone.Level = SmsLevel.Maintainer;
            }
            cellphone.SiteId = 1;

                //if (SystemPara.InsertCellphoneInfo(cellphone) != 0)  // 入库成功
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
                //    MessageBox.Show("插入数据库失败！");
                //}          

        }

        // 检查输入的合法性
        private bool CheckCellphone()
        {
            tbName.Text = tbName.Text.Trim();
            if (tbName.Text.Length == 0)
            {
                MessageBox.Show("请输入姓名！");
                return false;
            }

            if (tbCellphone.Text.Length != 11)
            {
                MessageBox.Show("手机号码长度不正确！");
                return false;
            }

            //if (SystemPara.GetCellphoneInfo(tbName.Text) != null)
            //{
            //    MessageBox.Show("该用户已经存在！");
            //    return false;
            //}
            //if (SystemPara.IsExist(tbCellphone.Text))
            //{
            //    MessageBox.Show("该手机号码已经存在，请重新输入！");
            //    return false;
            //}

            return true;
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            if (lvPhone.SelectedItems.Count > 0)
            {
                // 删除数据库中手机用户数据
                //int userId = this.FindSmsUserID(this.SmsUser);

                //int r = SystemPara.DeleteSmsUserInfo(userId);

                //if (r != 0)  // 数据库删除成功
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
                //    MessageBox.Show("数据删除失败！");
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
                btnDelete.Enabled = true;   // 将删除按钮置可用

                tbName.Text = lvPhone.SelectedItems[0].Text;
                tbCellphone.Text = lvPhone.SelectedItems[0].SubItems[1].Text;
                if (lvPhone.SelectedItems[0].SubItems[2].Text.Equals("系统管理员"))
                {
                    //选中第一项
                    cbRole.SelectedIndex = 0;
                }
                else
                {   //选中第二项
                    cbRole.SelectedIndex = 1;
                }

                this.SmsUser = lvPhone.SelectedItems[0].SubItems[0].Text;  // 选中行用户名
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