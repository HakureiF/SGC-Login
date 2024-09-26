namespace Seer
{
    partial class Announce
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
            announcePanel = new Panel();
            announceContent = new RichTextBox();
            announcePanel.SuspendLayout();
            SuspendLayout();
            // 
            // announcePanel
            // 
            announcePanel.Controls.Add(announceContent);
            announcePanel.Location = new Point(1, 1);
            announcePanel.Name = "announcePanel";
            announcePanel.Size = new Size(300, 340);
            announcePanel.TabIndex = 0;
            // 
            // announceContent
            // 
            announceContent.Location = new Point(0, 0);
            announceContent.MaximumSize = new Size(295, 330);
            announceContent.Name = "announceContent";
            announceContent.ReadOnly = true;
            announceContent.Size = new Size(295, 330);
            announceContent.TabIndex = 1;
            announceContent.Multiline = true;
            announceContent.WordWrap = true;
            // 
            // Announce
            // 
            AutoScaleDimensions = new SizeF(7F, 17F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(304, 342);
            Controls.Add(announcePanel);
            Name = "Announce";
            Text = "公告";
            announcePanel.ResumeLayout(false);
            announcePanel.PerformLayout();
            ResumeLayout(false);
        }

        #endregion

        private Panel announcePanel;
        private RichTextBox announceContent;
    }
}