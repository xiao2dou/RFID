//#define Debug

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
using System.Data.SqlClient;

namespace FRID
{
    public partial class Form_Main : Form
    {
        #region 读卡器运行配置
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

        #endregion

        string consqlserver = "Data Source=.;Initial Catalog=classManager;Integrated Security=True;";//数据库连接语句
        string sql = null; //sql语句
        SqlConnection con = null;  //sql连接对象

        bool confirm_message = false;
        int classState = 0;//1表示上课，2表示下课，默认上课模式
        string jxbNumber = "16032";//手动确定教学班号
        string time = "";

        int counter_needStudent = 0;
        int counter_cameStudent = 0;

        public Form_Main()
        {
            InitializeComponent();
        }

        //加载程序主页
        private void Form_Main_Load(object sender, EventArgs e)
        {

            //打开USB串口
            try
            {
                icdev = dc_init(100, 0);//USB对应端口号100，波特率无效，返回设备号
                int res = dc_beep(icdev, 100);
                //MessageBox.Show("读写器设备号："+Convert.ToString(icdev));
            }
            catch
            {
                MessageBox.Show("读卡器连接异常，请检查读卡器后重启软件");
            }

            con = new SqlConnection(consqlserver);
            con.Open();

            string classroomName = "";
            string teacherName = "";
            string courseName = "";

            //显示学生列表
            read_studentList();
            //显示课程列表
            read_classList();

            //查库，显示课名和教师名
            sql = "select jroom,jname,cname from course,JXB where course.cnum=JXB.cnum and jnum='" + jxbNumber + "'";
            SqlCommand cmd = new SqlCommand(sql, con);
            SqlDataReader sread = cmd.ExecuteReader();
            try
            {
                if (sread.Read())
                {
                    classroomName = sread["jroom"].ToString().TrimEnd();
                    teacherName = sread["jname"].ToString().TrimEnd();
                    courseName = sread["cname"].ToString().TrimEnd();
                }
                else
                {
                    MessageBox.Show("当前无课程！");
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

            string time = DateTime.Now.ToLongDateString().ToString() + " " + DateTime.Now.DayOfWeek.ToString();
            int week = 20;
            label_message.Text = "现在是" + time + "，第" + week + "周\n" + "您所在的教室为" + classroomName;

            textBox_courseName.Text = courseName;
            textBox_teacherName.Text = teacherName;

            main_button_overClass.Visible = false;

        }

        #region 上课主页
        //确认课程信息
        private void main_button_confirm_Click(object sender, EventArgs e)
        {
            confirm_message = true;
            MessageBox.Show("课程信息已确认无误，您现在可以开始上课！");
            main_button_confirm.Visible = false;

            //清空刷卡信息
            sql = "update student set cardtime=''";
            SqlCommand cmd = new SqlCommand(sql, con);
            SqlDataReader sread_stu = cmd.ExecuteReader();
            sread_stu.Close();

            //显示学生列表
            read_classStudentList();
        }

        private void read_classStudentList()
        {
            counter_needStudent = 0;
            counter_cameStudent = 0;
            listView_main_stu.Items.Clear();
            sql = "select stname,student.stnum,class,cardtime from student,JXB,sc where sc.jnum=JXB.jnum and sc.stnum=student.stnum and JXB.jnum='" + jxbNumber + "'";
            SqlCommand cmd = new SqlCommand(sql, con);
            SqlDataReader sread_stu = cmd.ExecuteReader();
            try
            {
                while (sread_stu.Read())
                {
                    ListViewItem it = new ListViewItem();
                    it.Text = sread_stu["stname"].ToString().TrimEnd();
                    it.SubItems.Add(sread_stu["stnum"].ToString().TrimEnd());
                    it.SubItems.Add(sread_stu["class"].ToString().TrimEnd());
                    it.SubItems.Add(sread_stu["cardtime"].ToString().TrimEnd());
                    listView_main_stu.Items.Add(it);
                    counter_needStudent++;
                    if (!sread_stu["cardtime"].ToString().TrimEnd().Equals(""))
                    {
                        counter_cameStudent++;
                    }

                }
                textBox_stu_number.Text = counter_needStudent.ToString();
                textBox_stu_cameNumber.Text = counter_cameStudent.ToString();
                sread_stu.Close();
            }
            catch (Exception msg)
            {
                throw new Exception(msg.ToString());
            }
            finally
            {
                sread_stu.Close();
            }
        }

        //开始上课
        private void main_button_beginClass_Click(object sender, EventArgs e)
        {
            if (!confirm_message)
            {
                MessageBox.Show("请先确认课程信息是否正确");
            }
            classState = 1;
            //开始接受刷卡——自动读卡
            timer1.Enabled = true;
            main_button_beginClass.Visible = false;
            main_button_overClass.Visible = true;
        }

        //开始下课
        private void main_button_overClass_Click(object sender, EventArgs e)
        {
            classState = 2;
            //开始接受刷卡
            timer1.Enabled = true;
        }

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
                if (classState == 1)//上课
                {
                    time = DateTime.Now.ToLongTimeString().ToString();
                    sql = "update student set cardtime='" + time + "' where cardid='" + str + "'";
                    SqlCommand cmd = new SqlCommand(sql, con);
                    SqlDataReader sread_stu = cmd.ExecuteReader();
                    sread_stu.Close();
                    read_classStudentList();

                    sql = "select stname,stnum from student where cardid='" + str + "'";
                    cmd = new SqlCommand(sql, con);
                    sread_stu = cmd.ExecuteReader();
                    try
                    {
                        while (sread_stu.Read())
                        {
                            label_message.Text = sread_stu["stname"].ToString().TrimEnd() + "(" + sread_stu["stnum"].ToString().TrimEnd() + ")上课打卡成功";
                        }
                        sread_stu.Close();
                    }
                    catch (Exception msg)
                    {
                        throw new Exception(msg.ToString());
                    }
                    finally
                    {
                        sread_stu.Close();
                    }
                }
                else if (classState == 2)//下课 
                {
                    time = DateTime.Now.ToLongTimeString().ToString();
                    sql = "update student set cardtime='" + time + "' where cardid='" + str + "'";
                    SqlCommand cmd = new SqlCommand(sql, con);
                    SqlDataReader sread_stu = cmd.ExecuteReader();
                    sread_stu.Close();
                    read_classStudentList();

                    sql = "select stname,stnum,cardtime from student where cardid='" + str + "'";
                    cmd = new SqlCommand(sql, con);
                    sread_stu = cmd.ExecuteReader();
                    try
                    {
                        while (sread_stu.Read())
                        {
                            label_message.Text = sread_stu["stname"].ToString().TrimEnd() + "(" + sread_stu["stnum"].ToString().TrimEnd() + ")下课打卡成功";
                        }
                    }
                    catch (Exception msg)
                    {
                        throw new Exception(msg.ToString());
                    }
                    finally
                    {
                        sread_stu.Close();
                    }
                }
                else
                {
                    MessageBox.Show("系统异常，请选择上课还是下课！");
                }
            }
        }
        #endregion

        #region 学生管理

        //显示学生列表
        private void read_studentList()
        {
            listView_allStudent.Items.Clear();
            sql = "select stname,student.stnum,class,phone,cardid from student";
            SqlCommand cmd = new SqlCommand(sql, con);
            SqlDataReader sread_stu = cmd.ExecuteReader();
            try
            {
                while (sread_stu.Read())
                {
                    ListViewItem it = new ListViewItem();
                    it.Text = sread_stu["stname"].ToString().TrimEnd();
                    it.SubItems.Add(sread_stu["stnum"].ToString().TrimEnd());
                    it.SubItems.Add(sread_stu["class"].ToString().TrimEnd());
                    it.SubItems.Add(sread_stu["phone"].ToString().TrimEnd());
                    it.SubItems.Add(sread_stu["cardid"].ToString().TrimEnd());
                    listView_allStudent.Items.Add(it);
                }
            }
            catch (Exception msg)
            {
                throw new Exception(msg.ToString());
            }
            finally
            {
                sread_stu.Close();
            }
        }

        private void button_refresh_Click(object sender, EventArgs e)
        {
            read_studentList();
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

            //写数据库
#if Debug
            str = "4D39B29F5908040001350207719B350F";
#endif

            sql = "insert into student(stnum,stname,class,cardid) values('" + stu_number + "','" + stu_name + "','" + stu_class + "','" + str + "');";
            try
            {
                SqlCommand cmd = new SqlCommand(sql, con);
                SqlDataReader sread_stu = cmd.ExecuteReader();
                sread_stu.Close();
            }
            catch (Exception msg)
            {
                throw new Exception(msg.ToString());
            }

            MessageBox.Show(stu_name + "开卡成功！\n卡号为：" + str);
        }

        #region 请假登记
        private void leave_button_search_Click(object sender, EventArgs e)
        {
            string stu_number = leave_textBox_stu_number.Text;

            //查学生姓名、学号
            sql = "select stname,class from student where stnum='" + stu_number + "'";
            SqlCommand cmd = new SqlCommand(sql, con);
            SqlDataReader sread = cmd.ExecuteReader();
            try
            {
                if (sread.Read())
                {
                    leave_textBox_stu_name.Text = sread["stname"].ToString().TrimEnd();
                    leave_textBox_stu_class.Text = sread["class"].ToString().TrimEnd();
                }
                else
                {
                    MessageBox.Show("用户不存在");
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

        private void leave_button_comfirm_leave_Click(object sender, EventArgs e)
        {
            //请假
            MessageBox.Show("假装请假成功！");
        }


        #endregion

        #endregion

        #region 课程管理
        //显示课程列表
        private void read_classList()
        {
            listView_allCourse.Items.Clear();
            sql = "select cnum,cname,clength from course";
            SqlCommand cmd = new SqlCommand(sql, con);
            SqlDataReader sread_stu = cmd.ExecuteReader();
            try
            {
                while (sread_stu.Read())
                {
                    ListViewItem it = new ListViewItem();
                    it.Text = sread_stu["cnum"].ToString().TrimEnd();
                    it.SubItems.Add(sread_stu["cname"].ToString().TrimEnd());
                    it.SubItems.Add(sread_stu["clength"].ToString().TrimEnd());
                    listView_allCourse.Items.Add(it);
                }
            }
            catch (Exception msg)
            {
                throw new Exception(msg.ToString());
            }
            finally
            {
                sread_stu.Close();
            }
        }

        private void button_class_refresh_List_Click(object sender, EventArgs e)
        {
            read_classList();
        }

        //查询课程
        private void class_button_search_Click(object sender, EventArgs e)
        {
            string classNumber = class_textBox_classNumber.Text;

            //查课程数据库
            sql = "select cname,clength from course where cnum='" + classNumber + "'";
            SqlCommand cmd = new SqlCommand(sql, con);
            SqlDataReader sread = cmd.ExecuteReader();
            try
            {
                if (sread.Read())
                {
                    class_textBox_className.Text = sread["cname"].ToString().TrimEnd();
                    class_textBox_classTime.Text = sread["clength"].ToString().TrimEnd();
                }
                else
                {
                    MessageBox.Show("课程不存在");
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

            class_button_edit.Visible = true;
            class_button_saveEdit.Visible = true;
        }

        //编辑课程
        private void class_button_edit_Click(object sender, EventArgs e)
        {
            class_textBox_classNumber.ReadOnly = true;
            class_textBox_className.ReadOnly = false;
            class_textBox_classTime.ReadOnly = false;
        }
        //保存课程修改
        private void class_button_saveEdit_Click(object sender, EventArgs e)
        {
            string courseName = class_textBox_className.Text;
            string courseTime = class_textBox_classTime.Text;
            //更新数据库
            sql = "update course set cname='" + courseName + "'and clength='" + courseTime + "' where cnum='" + class_textBox_classNumber.Text + "'";
            SqlCommand cmd = new SqlCommand(sql, con);
            SqlDataReader sread_stu = cmd.ExecuteReader();
            sread_stu.Close();
        }
        //添加课程
        private void class_button_addClass_Click(object sender, EventArgs e)
        {
            //写数据库
            class_button_search.Visible = false;
            class_button_edit.Visible = false;
            class_button_saveEdit.Visible = true;
            class_textBox_classNumber.ReadOnly = false;
            class_textBox_className.ReadOnly = false;
            class_textBox_classTime.ReadOnly = false;

            string classNumber = class_textBox_classNumber.Text;
            string courseName = class_textBox_className.Text;
            string courseTime = class_textBox_classTime.Text;

            sql = "insert into course(cnum,cname,clength) values('" + classNumber + "','" + courseName + "','" + courseTime + "');";
            try
            {
                SqlCommand cmd = new SqlCommand(sql, con);
                SqlDataReader sread_stu = cmd.ExecuteReader();
                sread_stu.Close();
                MessageBox.Show("添加课程成功");
            }
            catch (Exception msg)
            {
                throw new Exception(msg.ToString());
            }
        }


        #endregion

        #region 选课管理

        private void xk_button_classSearch_Click(object sender, EventArgs e)
        {
            string classNumber = xk_textBox_classNumber.Text;

            //查课程数据库
            sql = "select cname,clength from course where cnum='" + classNumber + "'";
            SqlCommand cmd = new SqlCommand(sql, con);
            SqlDataReader sread = cmd.ExecuteReader();
            try
            {
                if (sread.Read())
                {
                    xk_textBox_className.Text = sread["cname"].ToString().TrimEnd();
                    xk_textBox_classTime.Text = sread["clength"].ToString().TrimEnd();
                }
                else
                {
                    MessageBox.Show("课程不存在");
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

        private void xk_button_stuSearch_Click(object sender, EventArgs e)
        {
            string stu_number = xk_textBox_stuNumber.Text;

            //查学生姓名、学号
            sql = "select stname,class from student where stnum='" + stu_number + "'";
            SqlCommand cmd = new SqlCommand(sql, con);
            SqlDataReader sread = cmd.ExecuteReader();
            try
            {
                if (sread.Read())
                {
                    xk_textBox_stuName.Text = sread["stname"].ToString().TrimEnd();
                    xk_textBox_stuClass.Text = sread["class"].ToString().TrimEnd();
                }
                else
                {
                    MessageBox.Show("用户不存在");
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

        private void xk_button_confirm_Click(object sender, EventArgs e)
        {
            MessageBox.Show("假装选课成功（留一个bug给测试）");
        }
        #endregion

    }
}
