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
            button1.Text = I18N.GetString("Save");
            button2.Text = I18N.GetString("Cancel");
            textBox1.Text = _modifiedConfiguration.ApiEmail;
            textBox2.Text = _modifiedConfiguration.ApiPassword;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void button1_Click(object sender, EventArgs e)
        {

            _modifiedConfiguration.ApiEmail = textBox1.Text;
            _modifiedConfiguration.ApiPassword = textBox2.Text;

            if (_modifiedConfiguration.ApiUrl == "")
            {
                _modifiedConfiguration.ApiUrl = Configuration.GetDefaultUrl(0);
            }

            controller.SaveServersConfig(_modifiedConfiguration);

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
