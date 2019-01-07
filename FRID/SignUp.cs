using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace FRID
{
    public partial class Form_SignUp : Form
    {
        public Form_SignUp()
        {
            InitializeComponent();
        }

        private void button_signUp_Click(object sender, EventArgs e)
        {
            string userName = textBox_userName.Text;
            string password = textBox_password.Text;
            string repassword = textBox_repassword.Text;
            if (password != repassword)
            {
                MessageBox.Show("两次密码不一致，请确认");
                textBox_password.Clear();
                textBox_repassword.Clear();
            }

            //写入数据库

            Hide();
        }
    }
}
