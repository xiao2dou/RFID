#define Debug

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
    public partial class Form_Main : Form
    {
        bool confirm_message = false;
        public Form_Main()
        {
            InitializeComponent();
        }

        //加载程序主页
        private void Form_Main_Load(object sender, EventArgs e)
        {
            string time = DateTime.Now.ToLongDateString().ToString() + " " + DateTime.Now.DayOfWeek.ToString();
            int week= 20;
            string className = "C408";
            label_time_message.Text = "现在是" + time + "，第" + week + "周\n" + "您所在的教室为" + className;

            //查库，显示课名和教师名
        }

        //确认课程信息
        private void main_button_confirm_Click(object sender, EventArgs e)
        {
            confirm_message = true;
            MessageBox.Show("课程信息已确认无误，您现在可以开始上课！");
        }

        //开始上课
        private void main_button_beginClass_Click(object sender, EventArgs e)
        {
            if (confirm_message)
            {
                MessageBox.Show("请先确认课程信息是否正确");
            }

            //开始接受刷卡

        }

        //开始下课
        private void main_button_overClass_Click(object sender, EventArgs e)
        {
            //开始接受刷卡


        }

        //开卡程序
        private void button_OpenCard_Click(object sender, EventArgs e)
        {
            string stu_name = openCard_textBox_name.Text;
            string stu_class = openCard_textBox_class.Text;
            string stu_number = openCard_textBox_number.Text;

            //读卡

            //写数据库

            MessageBox.Show("开卡成功！");
        }

    }
}
