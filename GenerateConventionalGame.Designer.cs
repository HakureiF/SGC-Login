namespace Seer
{
    partial class GenerateConventionalGame
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
            label = new Label();
            RaceGroupList = new ComboBox();
            generateGame = new Button();
            SuspendLayout();
            // 
            // label
            // 
            label.AutoSize = true;
            label.Font = new Font("Microsoft YaHei UI", 12F, FontStyle.Regular, GraphicsUnit.Point);
            label.Location = new Point(28, 77);
            label.Name = "label";
            label.Size = new Size(90, 21);
            label.TabIndex = 0;
            label.Text = "选择比赛组";
            // 
            // RaceGroupList
            // 
            RaceGroupList.Font = new Font("Microsoft YaHei UI", 12F, FontStyle.Regular, GraphicsUnit.Point);
            RaceGroupList.FormattingEnabled = true;
            RaceGroupList.Location = new Point(121, 74);
            RaceGroupList.Name = "RaceGroupList";
            RaceGroupList.Size = new Size(155, 29);
            RaceGroupList.TabIndex = 1;
            RaceGroupList.SelectedIndexChanged += RaceGroupList_SelectedIndexChanged;
            // 
            // generateGame
            // 
            generateGame.Font = new Font("Microsoft YaHei UI", 12F, FontStyle.Regular, GraphicsUnit.Point);
            generateGame.Location = new Point(111, 129);
            generateGame.Name = "generateGame";
            generateGame.Size = new Size(94, 28);
            generateGame.TabIndex = 2;
            generateGame.Text = "创建对局";
            generateGame.UseVisualStyleBackColor = true;
            generateGame.Click += generateGame_Click;
            // 
            // GenerateConventionalGame
            // 
            AutoScaleDimensions = new SizeF(7F, 17F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(322, 197);
            Controls.Add(generateGame);
            Controls.Add(RaceGroupList);
            Controls.Add(label);
            Name = "GenerateConventionalGame";
            Text = "新建对战";
            FormClosed += CloseGenerate;
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Label label;
        private ComboBox RaceGroupList;
        public Button generateGame;
    }
}