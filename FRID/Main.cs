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

using System.Runtime.InteropServices;

namespace FRID
{
    public partial class Form_Main : Form
    {
        [DllImport("dcrf32.dll", EntryPoint = "dc_init", CharSet = CharSet.Auto)]
        public static extern int dc_init(int port, int baud);

        [DllImport("dcrf32.dll")]
        public static extern short dc_reset(int icdev, uint sec);

        [DllImport("dcrf32.dll")]
        public static extern short dc_card(int icdev, char _Mode, ref ulong Snr);

        [DllImport("dcrf32.dll")]
        public static extern short dc_authentication_passaddr(int icdev, int _Mode, int _Addr, byte[] nkey);


        [DllImport("dcrf32.dll")]
        public static extern short dc_read(int icdev, int adr, [Out] byte[] sdata);

        [DllImport("dcrf32.dll", EntryPoint = "dc_beep", CharSet = CharSet.Auto)]
        public static extern int dc_beep(int icdev, int _Msec);

        //[DllImport("dcrf32.dll", EntryPoint = "dc_read", CharSet = CharSet.Auto)]
        //public static extern int dc_read(int icdev, int Adr,byte[] Data);

        int icdev;
        ulong icCardNo = 0;
        char tt = (char)0;
        //核对密码

        byte[] hexkey = new byte[6] { 0xff, 0xff, 0xff, 0xff, 0xff, 0xff };
        byte[] hexkey2 = new byte[6] { 0, 0, 0, 0, 0, 0 };


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

            //打开USB串口
            try
            {
                icdev = dc_init(100, 0);//USB对应端口号100，波特率无效，返回设备号
                int res = dc_beep(icdev, 100);
                //MessageBox.Show("读写器设备号："+Convert.ToString(icdev));
            }
            catch
            {

            }

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

            //开始接受刷卡——自动读卡
            timer1.Enabled = true;

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
            byte[] bytesData = new byte[16];

            int intRes;
            int intSector;//扇区
            int intAdd;//区内块号

            //Stopwatch sw = new Stopwatch();
            //sw.Start();            

            intRes = dc_card(icdev, tt, ref icCardNo);
            if (icCardNo != 0)
            {
                ;
            }
            else
            {
                ;
                return;
            }

            //密码身份认证 参数（1.设备号、2.验证模式KeyA、3.读的块所在区的控制块、4.密码）
            //intSector = Convert.ToInt32(textBox1.Text);
            intSector = 0;
            intRes = dc_authentication_passaddr(icdev, 0, (intSector * 4 + 3), hexkey);

            //读块
            //intAdd = Convert.ToInt32(textBox2.Text);
            intAdd = 0;
            intRes = dc_read(icdev, (intSector * 4 + intAdd), bytesData);
            dc_beep(icdev, 10);
            if (intRes != 0)
            {
                MessageBox.Show("读卡失败");
            }
            //int res = dc_HL_read(icdev, 0, 5, 0, bytesData, intCardID);
            string str = "";
            for (int i = 0; i < 16; i++)
            {
                str += bytesData[i].ToString("X2");
            }

            MessageBox.Show(Convert.ToString(str));

            //textBox3.Text = str;

            //写数据库

            MessageBox.Show("开卡成功！");
        }
        #region 请假登记
        private void leave_button_search_Click(object sender, EventArgs e)
        {
            string stu_number = leave_textBox_stu_number.Text;

            //查学生姓名、学号

            //显示
        }

        private void leave_button_comfirm_leave_Click(object sender, EventArgs e)
        {
            //请假
        }

        #endregion

        private void timer1_Tick(object sender, EventArgs e)
        {
            byte[] bytesData = new byte[16];
            int[] intCardID = new int[1];
            int intRes;
            int intSector;//扇区
            int intAdd;//区内块号

            intRes = dc_card(icdev, tt, ref icCardNo);

            //密码身份认证 参数（1.设备号、2.验证模式KeyA、3.读的块所在区的控制块、4.密码）
            //intSector = Convert.ToInt32(textBox1.Text);
            intSector = 0;
            intRes = dc_authentication_passaddr(icdev, 0, (intSector * 4 + 3), hexkey);

            //读块
            //intAdd = Convert.ToInt32(textBox2.Text);
            intAdd = 0;
            intRes = dc_read(icdev, (intSector * 4 + intAdd), bytesData);
            if (intRes == 0)
            {
                dc_beep(icdev, 1);
                string str = "";
                for (int i = 0; i < 16; i++)
                {
                    str += bytesData[i].ToString("X2");
                }

                //textBox3.Text = str;
                //用卡号查数据库
            }
        }
    }
}
