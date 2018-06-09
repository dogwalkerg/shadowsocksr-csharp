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
            button1.Text = I18N.GetString("Update");
            checkBox1.Text = I18N.GetString("UpdateWithProxy");
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

            bool use_proxy = checkBox1.Checked;

            if (email == "" || passwd == "" || website == "")
            {
                return;
            }
            CookieContainer cookies = new CookieContainer();

            string str = string.Empty, ssr_url_all = string.Empty;

            try
            {
                Uri site = new Uri(website + "/auth/login");
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(site);
                request.Method = "POST";
                request.Proxy = null;
                string postData = "email=" + email + "&passwd=" + passwd;
                byte[] byteArray = Encoding.UTF8.GetBytes(postData);
                request.CookieContainer = cookies;
                request.ContentType = "application/x-www-form-urlencoded";
                request.ContentLength = byteArray.Length;
                Stream dataStream = request.GetRequestStream();
                dataStream.Write(byteArray, 0, byteArray.Length);
                dataStream.Close();
                HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                // 回复 ok
                // MessageBox.Show(((HttpWebResponse)response).StatusDescription);
                dataStream = response.GetResponseStream();
                StreamReader reader = new StreamReader(dataStream);
                string responseFromServer = reader.ReadToEnd();
                string cookie_str = cookies.GetCookieHeader(site);
                //MessageBox.Show(responseFromServer + cookie_str);
                reader.Close();
                dataStream.Close();
                response.Close();

                Uri site_ua = new Uri(website + "/user");
                HttpWebRequest request_ua = (HttpWebRequest)WebRequest.Create(site_ua);
                request_ua.Method = "GET";
                request_ua.CookieContainer = cookies;
                request_ua.Proxy = null;
                request_ua.ContentType = "application/x-www-form-urlencoded";
                HttpWebResponse response_ua = (HttpWebResponse)request_ua.GetResponse();
                //// 回复 ok
                //// MessageBox.Show(((HttpWebResponse)response).StatusDescription);
                StreamReader reader_ua = new StreamReader(response_ua.GetResponseStream());
                string responseFromServer_ua = reader_ua.ReadToEnd();
                // MessageBox.Show(responseFromServer_ua);
                reader_ua.Close();
                response_ua.Close();

                //Regex r = new Regex("(?<=>)Lv .(?=<)");
                //MatchCollection mc = r.Matches(responseFromServer_ua);
                ////for (int i = 0; i < mc.Count; i++)
                ////{
                ////    MessageBox.Show(i.ToString()+" : "+mc[i]);
                ////}
                //ssr_url_all = mc[0].ToString();
                //MessageBox.Show(ssr_url_all);
                ////updateFreeNodeChecker_NewFreeNodeFound_Api(ssr_url_all);


                get_info_from_web(responseFromServer_ua);
            }
            catch (Exception api_e)
            {
                MessageBox.Show(api_e.ToString());
            }
        }

        private void get_info_from_web(string web)
        {
            Regex lv_regex = new Regex("(?<=>)Lv .(?=<)");
            Regex lv_remain_regex = new Regex("(?<=等级过期：)(.*?)(?=<)");
            Regex traffic_regex = new Regex("(?<=\")(.*?)B\\s/\\s(.*?)B(?=\")");
            Regex money_regex = new Regex("(?<=35px\">)(.*?)(?=<)");
            Regex today_regex = new Regex("(?<=;\" >)(.*?)B(?=</div>今日流量)");

            try
            {
                MatchCollection mc0 = lv_regex.Matches(web);
                MessageBox.Show(mc0[0].ToString());

                MatchCollection mc1 = lv_remain_regex.Matches(web);
                MessageBox.Show(mc1[0].ToString());

                MatchCollection mc2 = traffic_regex.Matches(web);
                MessageBox.Show(mc2[0].ToString());
            }
            catch { }
            try
            {
                MatchCollection mc3 = money_regex.Matches(web);
                MessageBox.Show(mc3[0].ToString());
                MatchCollection mc4 = today_regex.Matches(web);
                MessageBox.Show(mc4[0].ToString());
            }
            catch { }

        }
    }
}
