using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace DailyQueue
{
    public partial class DailyQueueForm : Form
    {
        private DailyQueueConfiguration config;

        private NotifyIcon notifyIcon;
        private ContextMenu contextMenu;
        private HotKeys.GlobalHotkey ghk;

        public DailyQueueForm()
        {
            InitializeComponent();
            config = new DailyQueueConfiguration();
            ghk = new HotKeys.GlobalHotkey(HotKeys.Constants.CTRL + HotKeys.Constants.SHIFT, Keys.C, this);
            notifyIcon = new NotifyIcon();
            MenuItem[] menuItems = new MenuItem[2];
            MenuItem configureItem = new MenuItem();
            configureItem.Text = "configure";
            configureItem.Click += new System.EventHandler(this.configureItem_Click);
            menuItems[0] = configureItem;
            MenuItem closeItem = new MenuItem();
            closeItem.Text = "quit";
            closeItem.Click += new System.EventHandler(this.closeItem_Click);
            menuItems[1] = closeItem;
            contextMenu = new ContextMenu(menuItems);
            notifyIcon.Icon = new Icon("appicon.ico");
            notifyIcon.ContextMenu = contextMenu;
            notifyIcon.Text = "DailyQueue";
            notifyIcon.Visible = true;
        }

        private void handle()
        {
            ClipboardGrabber grabber = new ClipboardGrabber();
            WebServiceSender sender = new WebServiceSender();
            string link = grabber.grab();
            SendResult sendResult = sender.send(link, config);
            if (sendResult == SendResult.SUCCESS)
                notifySuccess();
            else if (sendResult == SendResult.FAILURE)
                notifyFailure();
            else if (sendResult == SendResult.NOT_CONFIGURED)
                notifyNotConfigured();
        }

        #region Overrides

        protected override void WndProc(ref Message m)
        {
            if (m.Msg == HotKeys.Constants.WM_HOTKEY_MSG_ID)
                handle();
            base.WndProc(ref m);
        }

        #endregion

        #region Private Event Handlers

        private void DailyQueueForm_Load(object sender, EventArgs e)
        {
            this.Hide();
            ghk.Register();
        }

        private void DailyQueueForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            ghk.Unregiser();
            notifyIcon.Visible = false;
            notifyIcon.Dispose();
            contextMenu.Dispose();
            this.Dispose();
        }

        private void configureItem_Click(object sender, EventArgs e)
        {
            PasswordForm form = new PasswordForm(config.User, config.Password);
            DialogResult dialogResult = form.ShowDialog();
            if (dialogResult == DialogResult.OK)
            {
                config.resetCookies();
                config.User = form.User;
                config.Password = form.Password;
            }
            form.Dispose();
        }

        private void closeItem_Click(object sender, EventArgs e)
        {
            ghk.Unregiser();
            notifyIcon.Visible = false;
            notifyIcon.Dispose();
            contextMenu.Dispose();
            this.Dispose();
        }

        #endregion

        #region Notify Icon Helper methods

        private void notifySuccess()
        {
            notifyIcon.BalloonTipText = Constants.SUCCESS_MSG;
            notifyIcon.ShowBalloonTip(Constants.BALLOON_TIME);
        }

        private void notifyFailure()
        {
            notifyIcon.BalloonTipText = Constants.FAILED_MSG;
            notifyIcon.ShowBalloonTip(Constants.BALLOON_TIME);
        }

        public void notifyNotConfigured()
        {
            notifyIcon.BalloonTipText = Constants.NOT_CONFIGURED_MSG;
            notifyIcon.ShowBalloonTip(Constants.BALLOON_TIME);
        }

        #endregion

    }
}
