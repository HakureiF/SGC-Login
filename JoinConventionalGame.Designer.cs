namespace Seer
{
    partial class JoinConventionalGame
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
            gameIdLabel = new Label();
            gameIdBox = new TextBox();
            joinButton = new Button();
            SuspendLayout();
            // 
            // gameIdLabel
            // 
            gameIdLabel.AutoSize = true;
            gameIdLabel.Font = new Font("Microsoft YaHei UI", 12F, FontStyle.Regular, GraphicsUnit.Point);
            gameIdLabel.Location = new Point(24, 34);
            gameIdLabel.Name = "gameIdLabel";
            gameIdLabel.Size = new Size(58, 21);
            gameIdLabel.TabIndex = 0;
            gameIdLabel.Text = "对战号";
            // 
            // gameIdBox
            // 
            gameIdBox.Font = new Font("Microsoft YaHei UI", 12F, FontStyle.Regular, GraphicsUnit.Point);
            gameIdBox.Location = new Point(100, 31);
            gameIdBox.Name = "gameIdBox";
            gameIdBox.Size = new Size(127, 28);
            gameIdBox.TabIndex = 1;
            // 
            // joinButton
            // 
            joinButton.Font = new Font("Microsoft YaHei UI", 12F, FontStyle.Regular, GraphicsUnit.Point);
            joinButton.Location = new Point(87, 81);
            joinButton.Name = "joinButton";
            joinButton.Size = new Size(75, 28);
            joinButton.TabIndex = 4;
            joinButton.Text = "加入";
            joinButton.UseVisualStyleBackColor = true;
            joinButton.Click += joinButton_Click;
            // 
            // JoinConventionalGame
            // 
            AutoScaleDimensions = new SizeF(7F, 17F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(252, 119);
            Controls.Add(joinButton);
            Controls.Add(gameIdBox);
            Controls.Add(gameIdLabel);
            Name = "JoinConventionalGame";
            Text = "加入对战";
            FormClosed += CloseJoin;
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Label gameIdLabel;
        private TextBox gameIdBox;
        private Button joinButton;
    }
}