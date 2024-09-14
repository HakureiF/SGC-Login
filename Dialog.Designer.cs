namespace Seer
{
    partial class Dialog
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Dialog));
            loginButton = new Button();
            idLabel = new Label();
            idTextBox = new TextBox();
            pwTextBox = new TextBox();
            pwLabel = new Label();
            rememberPwCheck = new CheckBox();
            SuspendLayout();
            // 
            // loginButton
            // 
            loginButton.Font = new Font("Microsoft YaHei UI", 12F, FontStyle.Regular, GraphicsUnit.Point);
            loginButton.Location = new Point(58, 135);
            loginButton.Margin = new Padding(2);
            loginButton.Name = "loginButton";
            loginButton.Size = new Size(75, 28);
            loginButton.TabIndex = 4;
            loginButton.Text = "登录";
            loginButton.UseVisualStyleBackColor = true;
            loginButton.Click += loginButton_Click;
            // 
            // idLabel
            // 
            idLabel.AutoSize = true;
            idLabel.Font = new Font("Microsoft YaHei UI", 12F, FontStyle.Regular, GraphicsUnit.Point);
            idLabel.Location = new Point(24, 13);
            idLabel.Margin = new Padding(2, 0, 2, 0);
            idLabel.Name = "idLabel";
            idLabel.Size = new Size(24, 21);
            idLabel.TabIndex = 0;
            idLabel.Text = "id";
            // 
            // idTextBox
            // 
            idTextBox.Font = new Font("Microsoft YaHei UI", 12F, FontStyle.Regular, GraphicsUnit.Point);
            idTextBox.Location = new Point(58, 11);
            idTextBox.Margin = new Padding(2);
            idTextBox.Name = "idTextBox";
            idTextBox.Size = new Size(115, 28);
            idTextBox.TabIndex = 1;
            idTextBox.TextChanged += idTextBox_TextChanged;
            // 
            // pwTextBox
            // 
            pwTextBox.Font = new Font("Microsoft YaHei UI", 12F, FontStyle.Regular, GraphicsUnit.Point);
            pwTextBox.Location = new Point(58, 53);
            pwTextBox.Margin = new Padding(2);
            pwTextBox.Name = "pwTextBox";
            pwTextBox.PasswordChar = '*';
            pwTextBox.Size = new Size(115, 28);
            pwTextBox.TabIndex = 2;
            pwTextBox.TextChanged += pwTextBox_TextChanged;
            pwTextBox.KeyDown += pwTextBox_KeyDown;
            // 
            // pwLabel
            // 
            pwLabel.AutoSize = true;
            pwLabel.Font = new Font("Microsoft YaHei UI", 12F, FontStyle.Regular, GraphicsUnit.Point);
            pwLabel.Location = new Point(11, 53);
            pwLabel.Margin = new Padding(2, 0, 2, 0);
            pwLabel.Name = "pwLabel";
            pwLabel.Size = new Size(42, 21);
            pwLabel.TabIndex = 3;
            pwLabel.Text = "密码";
            // 
            // rememberPwCheck
            // 
            rememberPwCheck.AutoSize = true;
            rememberPwCheck.Checked = true;
            rememberPwCheck.CheckState = CheckState.Checked;
            rememberPwCheck.Font = new Font("Microsoft YaHei UI", 10.5F, FontStyle.Regular, GraphicsUnit.Point);
            rememberPwCheck.Location = new Point(47, 94);
            rememberPwCheck.Name = "rememberPwCheck";
            rememberPwCheck.Size = new Size(84, 24);
            rememberPwCheck.TabIndex = 5;
            rememberPwCheck.Text = "记住密码";
            rememberPwCheck.UseVisualStyleBackColor = true;
            // 
            // Dialog
            // 
            AutoScaleDimensions = new SizeF(7F, 17F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(196, 185);
            Controls.Add(rememberPwCheck);
            Controls.Add(pwLabel);
            Controls.Add(pwTextBox);
            Controls.Add(idTextBox);
            Controls.Add(idLabel);
            Controls.Add(loginButton);
            Icon = (Icon)resources.GetObject("$this.Icon");
            Margin = new Padding(2);
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "Dialog";
            Text = "登录";
            TopMost = true;
            Activated += form_Activated;
            Load += Aside_Load;
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Button loginButton;
        private Label idLabel;
        private TextBox idTextBox;
        private TextBox pwTextBox;
        private Label pwLabel;
        private CheckBox rememberPwCheck;
    }
}