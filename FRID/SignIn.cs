#define Debug

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
    public partial class Form_SignIn : Form
    {
        DataSet ds = new DataSet();  //数据缓存
        string consqlserver = "Data Source=.;Initial Catalog=classManager;Integrated Security=True;";//数据库连接语句
        string sql = null; //sql语句
        SqlConnection con = null;  //sql连接对象
        //SqlDataAdapter da = null;//数据和DataSet的桥梁

        public Form_SignIn()
        {
            InitializeComponent();
        }

        private void Form_SignIn_Load(object sender, EventArgs e)
        {
#if Debug
            textBox_userName.Text = "111";
            textBox_password.Text = "123123";
#endif
            con = new SqlConnection(consqlserver);
            con.Open();
        }

        private void button_signIn_Click(object sender, EventArgs e)
        {
            string userName = textBox_userName.Text;
            string password = textBox_password.Text;

            //用户校验
            sql = "select * from users where unum='" + userName + "' and ukey='" + password  + "'";
            SqlCommand cmd = new SqlCommand(sql, con);
            SqlDataReader sread = cmd.ExecuteReader();
            try
            {
                if (sread.Read())
                {
                    MessageBox.Show("登录成功");
                    clear();
                    Form_Main fm = new Form_Main();
                    fm.Show();
                    Hide();
                }
                else
                {
                    MessageBox.Show("用户名不存在或密码错误或身份错误");
                    clear();
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

        private void button_signUp_Click(object sender, EventArgs e)
        {
            //转用户注册
            Form_SignUp fm = new Form_SignUp();
            fm.Show();
        }

        private void clear()
        {
            textBox_userName.Text = "";
            textBox_password.Text = "";
        }

        private void Form_SignIn_FormClosing(object sender, FormClosingEventArgs e)
        {
            con.Close();
        }
    }
}
