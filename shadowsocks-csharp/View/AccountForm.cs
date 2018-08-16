using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Threading;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using System.Net;
using System.IO;
using Shadowsocks.Controller;
using Shadowsocks.Model;
using Shadowsocks.Properties;

namespace Shadowsocks.View
{
    public partial class AccountForm : Form
    {
        private ShadowsocksController controller;
        // this is a copy of configuration that we are working on
        private Configuration _modifiedConfiguration;
        public delegate void treeinvoke();

        public AccountForm(ShadowsocksController controller)
        {
            this.controller = controller;
            this.Font = System.Drawing.SystemFonts.MessageBoxFont;
            this.Icon = Icon.FromHandle(Resources.ssw128.GetHicon());
            InitializeComponent();
            LoadCurrentConfiguration();
            UpdateTexts();
        }

        private void LoadCurrentConfiguration()
        {
            _modifiedConfiguration = controller.GetConfiguration();
        }

        private void UpdateTexts()
        {
            //Configuration config = controller.GetCurrentConfiguration();
            this.Text = I18N.GetString("Account info");
            label1.Text = I18N.GetString("Api_Level");
            label3.Text = I18N.GetString("Api_Level_due_date");
            label5.Text = I18N.GetString("Api_Traffic(used/enable)");
            label7.Text = I18N.GetString("Api_Today_used");
            label9.Text = I18N.GetString("Api_Money");
            label11.Text = I18N.GetString("Account_due_date");
            button1.Text = I18N.GetString("Update");
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Thread t = new Thread(() =>
            {
                update_info();
            });
            t.Start();
        }
        private void update_info()
        {
            string email = _modifiedConfiguration.ApiEmail, passwd = _modifiedConfiguration.ApiPassword, website = _modifiedConfiguration.ApiUrl;

            if (email == "" || passwd == "" || website == "")
            {
                return;
            }
            CookieContainer cookies = new CookieContainer();

            string str = string.Empty, ssr_url_all = string.Empty;

        }

        private void get_info_from_web(string web_src)
        {
            string web = web_src;
            Configuration cfg = controller.GetCurrentConfiguration();

        }
    }
}
