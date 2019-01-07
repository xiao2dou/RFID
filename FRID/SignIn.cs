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
    public partial class Form_SignIn : Form
    {
        public Form_SignIn()
        {
            InitializeComponent();
        }

        private void button_signIn_Click(object sender, EventArgs e)
        {
            string userName = textBox_userName.Text;
            string password = textBox_password.Text;

            //用户校验

            Form_Main fm = new Form_Main();
            fm.Show();

            Hide();
        }

        private void button_signUp_Click(object sender, EventArgs e)
        {
            //转用户注册
            Form_SignUp fm = new Form_SignUp();
            fm.Show();
        }
    }
}
