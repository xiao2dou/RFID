using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace FRID
{
    public partial class Form_SignUp : Form
    {
        string consqlserver = "Data Source=.;Initial Catalog=classManager;Integrated Security=True;";//数据库连接语句
        string sql = null; //sql语句
        SqlConnection con = null;  //sql连接对象

        public Form_SignUp()
        {
            InitializeComponent();
        }

        private void button_signUp_Click(object sender, EventArgs e)
        {
            string userName = textBox_userName.Text;
            string password = textBox_password.Text;
            string repassword = textBox_repassword.Text;

            //查、改数据库（用户注册）
            sql = "select * from users where unum = '" + userName + "'";
            SqlCommand cmd = new SqlCommand(sql, con);
            SqlDataReader sread = cmd.ExecuteReader();
            try
            {
                if (!password.Equals(repassword))
                {
                    MessageBox.Show("两次密码不一致，请确认");
                    textBox_password.Clear();
                    textBox_repassword.Clear();
                    return;
                }
                else if (sread.Read())
                {
                    MessageBox.Show("用户名已存在，请重新输入");
                    clear();
                    sread.Close();
                    return;
                }
                else
                {
                    sread.Close();
                    if (password.Length < 6)
                    {
                        MessageBox.Show("密码长度须不小于6位，请重新输入");
                        clear();
                        return;
                    }
                    else
                    {
                        sql = "insert into users values('" + userName + "','" + password + "');";
                        cmd = new SqlCommand(sql, con);
                        cmd.ExecuteNonQuery();

                        MessageBox.Show("注册成功，请返回登录");
                        clear();
                        Hide();
                    }
                }
            }
            catch (Exception msg)
            {
                throw new Exception(msg.ToString());
            }
            finally
            {
                sread.Close();
            }
        }

        private void Form_SignUp_Load(object sender, EventArgs e)
        {
            con = new SqlConnection(consqlserver);
            con.Open();
        }

        private void Form_SignUp_FormClosing(object sender, FormClosingEventArgs e)
        {
            con.Close();
        }

        private void clear()
        {
            textBox_userName.Text = "";
            textBox_password.Text = "";
            textBox_repassword.Text = "";
        }
    }
}
