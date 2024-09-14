namespace Seer
{
    partial class Match
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Match));
            matchStateLabel = new Label();
            cancalButton = new Button();
            SuspendLayout();
            // 
            // matchStateLabel
            // 
            matchStateLabel.AutoSize = true;
            matchStateLabel.Location = new Point(42, 39);
            matchStateLabel.Margin = new Padding(2, 0, 2, 0);
            matchStateLabel.Name = "matchStateLabel";
            matchStateLabel.Size = new Size(0, 17);
            matchStateLabel.TabIndex = 0;
            // 
            // cancalButton
            // 
            cancalButton.Font = new Font("Microsoft YaHei UI", 12F, FontStyle.Regular, GraphicsUnit.Point);
            cancalButton.Location = new Point(47, 90);
            cancalButton.Margin = new Padding(2);
            cancalButton.Name = "cancalButton";
            cancalButton.Size = new Size(75, 28);
            cancalButton.TabIndex = 4;
            cancalButton.Text = "取消";
            cancalButton.UseVisualStyleBackColor = true;
            cancalButton.Click += cancalButton_Click;
            // 
            // Match
            // 
            AutoScaleDimensions = new SizeF(7F, 17F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(171, 135);
            Controls.Add(matchStateLabel);
            Controls.Add(cancalButton);
            Icon = (Icon)resources.GetObject("$this.Icon");
            Margin = new Padding(2);
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "Match";
            Text = "匹配";
            TopMost = true;
            FormClosed += CloseAll;
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion
        private Label matchStateLabel;
        private Button cancalButton;
    }
}