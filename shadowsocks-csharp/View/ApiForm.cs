using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using Shadowsocks.Controller;
using Shadowsocks.Model;
using Shadowsocks.Properties;

namespace Shadowsocks.View
{
    public partial class ApiForm : Form
    {
        private ShadowsocksController controller;
        // this is a copy of configuration that we are working on
        private Configuration _modifiedConfiguration;

        public ApiForm(ShadowsocksController controller)
        {
            this.Font = System.Drawing.SystemFonts.MessageBoxFont;
            InitializeComponent();
            this.Icon = Icon.FromHandle(Resources.ssw128.GetHicon());
            this.controller = controller;
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
            this.Text = I18N.GetString("ApiForm");
            label1.Text = I18N.GetString("Api_Email");
            label2.Text = I18N.GetString("Api_Password");
            label3.Text = I18N.GetString("Api_Url");
            button1.Text = I18N.GetString("Save");
            button2.Text = I18N.GetString("Cancel");
            checkBox1.Text = I18N.GetString("Auto Update");
            checkBox2.Text = I18N.GetString("UpdateWithProxy");
            checkBox3.Text = I18N.GetString("Api_SpiderMode");
            textBox1.Text = _modifiedConfiguration.ApiEmail;
            textBox2.Text = _modifiedConfiguration.ApiPassword;
            textBox3.Text = _modifiedConfiguration.ApiUrl;
            checkBox1.Checked = _modifiedConfiguration.ApiAutoUpdate;
            checkBox2.Checked = _modifiedConfiguration.ApiUpdateWithProxy;
            checkBox3.Checked = _modifiedConfiguration.ApiSpiderMode;
            checkBox4.Checked = _modifiedConfiguration.ApiHttps;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            string website = textBox3.Text;

            string pattern_end_1 = @"^http(s?)://.*?/$";
            string pattern_end_2 = @"/(auth|api|user|admin|404|405|500|code|tos|staff)(.*?)$";

            // 结尾不保留 /
            // 增加开头部分 https://
            if (checkBox4.Checked == true)
            {
                if (!Regex.IsMatch(website, @"^https://"))
                {
                    if (!Regex.IsMatch(website, @"^(.*?)//"))
                    {
                        website = Regex.Replace(website, @"^", "https://");
                    }
                    else
                    {
                        website = Regex.Replace(website, @"^(.*?)//", "https://");
                    }
                }
            }
            else
            {
                if (!Regex.IsMatch(website, @"^http://"))
                {
                    if (!Regex.IsMatch(website, @"^(.*?)//"))
                    {
                        website = Regex.Replace(website, @"^", "http://");
                    }
                    else
                    {
                        website = Regex.Replace(website, @"^(.*?)//", "http://");
                    }
                }
            }

            // 结尾去掉 /
            if (Regex.IsMatch(website, pattern_end_1))
            {
                website = Regex.Replace(website, @"/$", "");
            }
            // 去除各种后缀，去除结尾 /
            if (Regex.IsMatch(website, pattern_end_2))
            {
                website = Regex.Replace(website, @"/(auth|api|user|admin|404|405|500|code|tos|staff)(.*?)$", "");
            }

            _modifiedConfiguration.ApiEmail = textBox1.Text;
            _modifiedConfiguration.ApiPassword = textBox2.Text;
            _modifiedConfiguration.ApiUrl = website;
            _modifiedConfiguration.ApiAutoUpdate = checkBox1.Checked;
            _modifiedConfiguration.ApiUpdateWithProxy = checkBox2.Checked;
            _modifiedConfiguration.ApiSpiderMode = checkBox3.Checked;
            _modifiedConfiguration.ApiHttps = checkBox4.Checked;
            controller.SaveServersConfig(_modifiedConfiguration);
            //Configuration cfg = Configuration.Load();
            //Configuration.SetApiSettings(textBox1.Text, textBox2.Text, textBox3.Text);
            //Configuration.Save(cfg);
            Close();
        }

        private void checkBox5_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox5.Checked == true)
            {
                textBox2.PasswordChar = '\0';
            }
            else
            {
                textBox2.PasswordChar = '*';
            }
        }
    }
}
