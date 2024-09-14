using Microsoft.Web.WebView2.Core;
using Microsoft.Web.WebView2.WinForms;
using Seer.Utils;

namespace Seer
{
    sealed partial class Main
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
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
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Main));
            webView = new WebView2();
            menuStrip = new MenuStrip();
            MenuItem_Refresh = new ToolStripMenuItem();
            MenuItem_Mute = new ToolStripMenuItem();
            MenuItem_Login = new ToolStripMenuItem();
            LoginToolStripMenuItem = new ToolStripMenuItem();
            LogoutToolStripMenuItem = new ToolStripMenuItem();
            SGCToolStripMenuItem = new ToolStripMenuItem();
            SendEliteInfoItem = new ToolStripMenuItem();
            ShowpickToolStripMenuItem = new ToolStripMenuItem();
            WebSiteFunction = new ToolStripMenuItem();
            MenuItem_CreateFreeWar = new ToolStripMenuItem();
            BagStoreToolStripMenuItem = new ToolStripMenuItem();
            PeakLogger_ToolStripMenuItem = new ToolStripMenuItem();
            RivalMimiToolStripMenuItem = new ToolStripMenuItem();
            LoginerFunctuin = new ToolStripMenuItem();
            MenuItem_GenerateConventionalGame = new ToolStripMenuItem();
            MenuItem_JoinConventionalGame = new ToolStripMenuItem();
            MatchToolStripMenuItem = new ToolStripMenuItem();
            RemoveCacheItem = new ToolStripMenuItem();
            TestToolStripMenuItem = new ToolStripMenuItem();
            ((System.ComponentModel.ISupportInitialize)webView).BeginInit();
            menuStrip.SuspendLayout();
            SuspendLayout();
            // 
            // webView
            // 
            webView.AllowExternalDrop = true;
/*            webView.CreationProperties = new CoreWebView2CreationProperties
            {
                AdditionalBrowserArguments = "--disable-gpu"
            };*/
            webView.DefaultBackgroundColor = Color.White;
            webView.ImeMode = ImeMode.Off;
            webView.Location = new Point(0, 28);
            webView.Name = "webView";
            webView.Size = new Size(1064, 573);
            webView.Source = new Uri("https://seerh5.61.com/", UriKind.Absolute);
            webView.TabIndex = 0;
            webView.ZoomFactor = 1D;
            webView.CoreWebView2InitializationCompleted += WebView_CoreWebViewInitializationCompleted;
            webView.KeyDown += MainForm_KeyDown;
            // 
            // menuStrip
            // 
            menuStrip.Items.AddRange(new ToolStripItem[] { MenuItem_Refresh, MenuItem_Mute, MenuItem_Login, SGCToolStripMenuItem, WebSiteFunction, LoginerFunctuin, TestToolStripMenuItem });
            menuStrip.Location = new Point(0, 0);
            menuStrip.Name = "menuStrip";
            menuStrip.Size = new Size(1064, 25);
            menuStrip.TabIndex = 9;
            // 
            // MenuItem_Refresh
            // 
            MenuItem_Refresh.Name = "MenuItem_Refresh";
            MenuItem_Refresh.Size = new Size(44, 21);
            MenuItem_Refresh.Text = "刷新";
            MenuItem_Refresh.Click += Refresh_Click;
            // 
            // MenuItem_Mute
            // 
            MenuItem_Mute.Name = "MenuItem_Mute";
            MenuItem_Mute.Size = new Size(44, 21);
            MenuItem_Mute.Text = "静音";
            MenuItem_Mute.Click += MenuItem_Mute_Click;
            // 
            // MenuItem_Login
            // 
            MenuItem_Login.DropDownItems.AddRange(new ToolStripItem[] { LoginToolStripMenuItem, LogoutToolStripMenuItem });
            MenuItem_Login.Name = "MenuItem_Login";
            MenuItem_Login.Size = new Size(68, 21);
            MenuItem_Login.Text = "登录SGC";
            // 
            // LoginToolStripMenuItem
            // 
            LoginToolStripMenuItem.Name = "LoginToolStripMenuItem";
            LoginToolStripMenuItem.Size = new Size(180, 22);
            LoginToolStripMenuItem.Text = "登录";
            LoginToolStripMenuItem.Click += LoginDialogBtn_Click;
            // 
            // LogoutToolStripMenuItem
            // 
            LogoutToolStripMenuItem.Name = "LogoutToolStripMenuItem";
            LogoutToolStripMenuItem.Size = new Size(180, 22);
            LogoutToolStripMenuItem.Text = "登出";
            LogoutToolStripMenuItem.Click += LogoutToolStripMenuItem_Click;
            // 
            // SGCToolStripMenuItem
            // 
            SGCToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[] { SendEliteInfoItem, ShowpickToolStripMenuItem });
            SGCToolStripMenuItem.Name = "SGCToolStripMenuItem";
            SGCToolStripMenuItem.Size = new Size(108, 21);
            SGCToolStripMenuItem.Text = "SGCbp网站联动";
            // 
            // SendEliteInfoItem
            // 
            SendEliteInfoItem.Name = "SendEliteInfoItem";
            SendEliteInfoItem.Size = new Size(180, 22);
            SendEliteInfoItem.Text = "同步精英收藏";
            SendEliteInfoItem.Click += SendEliteInfoItem_Click;
            // 
            // ShowpickToolStripMenuItem
            // 
            ShowpickToolStripMenuItem.Name = "ShowpickToolStripMenuItem";
            ShowpickToolStripMenuItem.Size = new Size(180, 22);
            ShowpickToolStripMenuItem.Text = "展示pick";
            ShowpickToolStripMenuItem.Click += ShowpickToolStripMenuItem_Click;
            // 
            // WebSiteFunction
            // 
            WebSiteFunction.DropDownItems.AddRange(new ToolStripItem[] { MenuItem_CreateFreeWar, BagStoreToolStripMenuItem, PeakLogger_ToolStripMenuItem, RivalMimiToolStripMenuItem });
            WebSiteFunction.Name = "WebSiteFunction";
            WebSiteFunction.Size = new Size(68, 21);
            WebSiteFunction.Text = "常规功能";
            // 
            // MenuItem_CreateFreeWar
            // 
            MenuItem_CreateFreeWar.Name = "MenuItem_CreateFreeWar";
            MenuItem_CreateFreeWar.Size = new Size(184, 22);
            MenuItem_CreateFreeWar.Text = "快速创建自由战房间";
            MenuItem_CreateFreeWar.Click += CreateFreeWar;
            // 
            // BagStoreToolStripMenuItem
            // 
            BagStoreToolStripMenuItem.Name = "BagStoreToolStripMenuItem";
            BagStoreToolStripMenuItem.Size = new Size(184, 22);
            BagStoreToolStripMenuItem.Text = "背包记录";
            BagStoreToolStripMenuItem.Click += BagStoreToolStripMenuItem_Click;
            // 
            // PeakLogger_ToolStripMenuItem
            // 
            PeakLogger_ToolStripMenuItem.Name = "PeakLogger_ToolStripMenuItem";
            PeakLogger_ToolStripMenuItem.Size = new Size(184, 22);
            PeakLogger_ToolStripMenuItem.Text = "巅峰记牌";
            PeakLogger_ToolStripMenuItem.Click += PeakLogger_ToolStripMenuItem_Click;
            // 
            // RivalMimiToolStripMenuItem
            // 
            RivalMimiToolStripMenuItem.Name = "RivalMimiToolStripMenuItem";
            RivalMimiToolStripMenuItem.Size = new Size(184, 22);
            RivalMimiToolStripMenuItem.Text = "查看对方米米号";
            RivalMimiToolStripMenuItem.Click += RivalMimiToolStripMenuItem_Click;
            // 
            // LoginerFunctuin
            // 
            LoginerFunctuin.DropDownItems.AddRange(new ToolStripItem[] { MenuItem_GenerateConventionalGame, MenuItem_JoinConventionalGame, MatchToolStripMenuItem, RemoveCacheItem });
            LoginerFunctuin.Name = "LoginerFunctuin";
            LoginerFunctuin.Size = new Size(87, 21);
            LoginerFunctuin.Text = "12banN模式";
            // 
            // MenuItem_GenerateConventionalGame
            // 
            MenuItem_GenerateConventionalGame.Name = "MenuItem_GenerateConventionalGame";
            MenuItem_GenerateConventionalGame.Size = new Size(192, 22);
            MenuItem_GenerateConventionalGame.Text = "新建对战(需登录SGC)";
            MenuItem_GenerateConventionalGame.Click += GenerateGameButton_Click;
            // 
            // MenuItem_JoinConventionalGame
            // 
            MenuItem_JoinConventionalGame.Name = "MenuItem_JoinConventionalGame";
            MenuItem_JoinConventionalGame.Size = new Size(192, 22);
            MenuItem_JoinConventionalGame.Text = "加入对战(需登录SGC)";
            MenuItem_JoinConventionalGame.Click += JoinGameButton_Click;
            // 
            // MatchToolStripMenuItem
            // 
            MatchToolStripMenuItem.Name = "MatchToolStripMenuItem";
            MatchToolStripMenuItem.Size = new Size(192, 22);
            MatchToolStripMenuItem.Text = "匹配对局";
            MatchToolStripMenuItem.Click += MatchToolStripMenuItem_Click;
            // 
            // RemoveCacheItem
            // 
            RemoveCacheItem.Name = "RemoveCacheItem";
            RemoveCacheItem.Size = new Size(192, 22);
            RemoveCacheItem.Text = "清除匹配数据";
            RemoveCacheItem.Click += RemoveCacheItem_Click;
            // 
            // TestToolStripMenuItem
            // 
            TestToolStripMenuItem.Name = "TestToolStripMenuItem";
            TestToolStripMenuItem.Size = new Size(44, 21);
            TestToolStripMenuItem.Text = "测试";
            TestToolStripMenuItem.Click += TestToolStripMenuItem_Click;
            // 
            // Main
            // 
            AutoScaleDimensions = new SizeF(7F, 17F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1064, 601);
            Controls.Add(webView);
            Controls.Add(menuStrip);
            Icon = (Icon)resources.GetObject("$this.Icon");
            MainMenuStrip = menuStrip;
            Margin = new Padding(6, 5, 6, 5);
            Name = "Main";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "SGC比赛登录器";
            FormClosed += CloseAll;
            ((System.ComponentModel.ISupportInitialize)webView).EndInit();
            menuStrip.ResumeLayout(false);
            menuStrip.PerformLayout();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private WebView2 webView;
        private Dialog dialog;
        private Footer footer;
        private PeakLogger peakLogger;
        private JoinConventionalGame joinConventionalGame;
        private GenerateConventionalGame generateConventionalGame;
        private ConventionalGame conventionalGame;
        private Match match;
        private BagStoreForm bagStore;

        private MenuStrip menuStrip;
        private ToolStripMenuItem WebSiteFunction;
        private ToolStripMenuItem MenuItem_CreateFreeWar;
        private ToolStripMenuItem MenuItem_Mute;
        private ToolStripMenuItem MenuItem_Refresh;
        private ToolStripMenuItem LoginerFunctuin;
        private ToolStripMenuItem MenuItem_GenerateConventionalGame;
        private ToolStripMenuItem MenuItem_Login;
        private ToolStripMenuItem MenuItem_JoinConventionalGame;
        private ToolStripMenuItem PeakLogger_ToolStripMenuItem;
        private ToolStripMenuItem SGCToolStripMenuItem;
        private ToolStripMenuItem RivalMimiToolStripMenuItem;
        private ToolStripMenuItem MatchToolStripMenuItem;
        private ToolStripMenuItem LoginToolStripMenuItem;
        private ToolStripMenuItem LogoutToolStripMenuItem;
        private ToolStripMenuItem TestToolStripMenuItem;
        private ToolStripMenuItem SendEliteInfoItem;
        private ToolStripMenuItem RemoveCacheItem;
        private ToolStripMenuItem BagStoreToolStripMenuItem;
        private ToolStripMenuItem ShowpickToolStripMenuItem;
    }
}