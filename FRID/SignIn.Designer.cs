namespace FRID
{
    partial class SignIn
    {
        /// <summary>
        /// 必需的设计器变量。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 清理所有正在使用的资源。
        /// </summary>
        /// <param name="disposing">如果应释放托管资源，为 true；否则为 false。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows 窗体设计器生成的代码

        /// <summary>
        /// 设计器支持所需的方法 - 不要修改
        /// 使用代码编辑器修改此方法的内容。
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SignIn));
            this.textBox_userName = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.textBox_password = new System.Windows.Forms.TextBox();
            this.button_signUp = new System.Windows.Forms.Button();
            this.button_signIn = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // textBox_userName
            // 
            this.textBox_userName.Location = new System.Drawing.Point(268, 53);
            this.textBox_userName.Name = "textBox_userName";
            this.textBox_userName.Size = new System.Drawing.Size(100, 21);
            this.textBox_userName.TabIndex = 0;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(221, 56);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(41, 12);
            this.label1.TabIndex = 1;
            this.label1.Text = "用户名";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(221, 83);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(29, 12);
            this.label2.TabIndex = 2;
            this.label2.Text = "密码";
            // 
            // textBox_password
            // 
            this.textBox_password.Location = new System.Drawing.Point(268, 80);
            this.textBox_password.Name = "textBox_password";
            this.textBox_password.Size = new System.Drawing.Size(100, 21);
            this.textBox_password.TabIndex = 3;
            // 
            // button_signUp
            // 
            this.button_signUp.Location = new System.Drawing.Point(200, 126);
            this.button_signUp.Name = "button_signUp";
            this.button_signUp.Size = new System.Drawing.Size(75, 23);
            this.button_signUp.TabIndex = 4;
            this.button_signUp.Text = "注册";
            this.button_signUp.UseVisualStyleBackColor = true;
            // 
            // button_signIn
            // 
            this.button_signIn.Location = new System.Drawing.Point(317, 126);
            this.button_signIn.Name = "button_signIn";
            this.button_signIn.Size = new System.Drawing.Size(75, 23);
            this.button_signIn.TabIndex = 5;
            this.button_signIn.Text = "登陆";
            this.button_signIn.UseVisualStyleBackColor = true;
            this.button_signIn.Click += new System.EventHandler(this.button_signIn_Click);
            // 
            // SignIn
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("$this.BackgroundImage")));
            this.ClientSize = new System.Drawing.Size(583, 333);
            this.Controls.Add(this.button_signIn);
            this.Controls.Add(this.button_signUp);
            this.Controls.Add(this.textBox_password);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.textBox_userName);
            this.Name = "SignIn";
            this.Text = "考勤管理系统-欢迎使用";
            this.Load += new System.EventHandler(this.Form1_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox textBox_userName;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox textBox_password;
        private System.Windows.Forms.Button button_signUp;
        private System.Windows.Forms.Button button_signIn;
    }
}

