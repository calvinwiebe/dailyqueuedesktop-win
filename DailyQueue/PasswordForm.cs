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
    public partial class PasswordForm : Form
    {
        private string user = "";
        private string password = "";

        public PasswordForm(string aUser, string aPassword)
        {
            InitializeComponent();
            userTextBox.Text = aUser;
            passwordTextBox.Text = aPassword;
        }

        private void submitButton_Click(object sender, EventArgs e)
        {
            user = userTextBox.Text;
            password = passwordTextBox.Text;
            this.DialogResult = DialogResult.OK;
        }

        private void cancelButton_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
        }

        public string Password
        {
            get { return password; }
            set { password = value; }
        }

        public string User
        {
            get { return user; }
            set { user = value; }
        }

    }
}
