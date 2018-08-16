using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using System.Threading;
using Shadowsocks.Controller;
using Shadowsocks.Model;
using Shadowsocks.Util;
using Shadowsocks.Properties;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Shadowsocks.View
{
    public partial class NewMainForm : Form
    {
        private ShadowsocksController controller;
        // this is a copy of configuration that we are working on
        private Configuration _modifiedConfiguration;
        private ServerLogForm serverLogForm;
        private ConfigForm configForm;
        private AccountForm accountForm;
        private UpdateChecker updateChecker;
        public delegate void treeinvoke();

        public NewMainForm(ShadowsocksController controller)
        {
            this.Font = System.Drawing.SystemFonts.MessageBoxFont;
            InitializeComponent();
            this.Icon = Icon.FromHandle(Resources.ssw128.GetHicon());
            this.controller = controller;
            LoadCurrentConfiguration();
            UpdateTexts();
            LoadServer();
            updateChecker = new UpdateChecker();
            //Thread t = new Thread(new ThreadStart(show_ping_start));
            //t.Start();
        }

        private void LoadCurrentConfiguration()
        {
            _modifiedConfiguration = controller.GetConfiguration();
        }

        private void UpdateTexts()
        {
            //Configuration config = controller.GetCurrentConfiguration();
            this.Text = I18N.GetString("NewMainForm");
            label1.Text = I18N.GetString("Connect_type");
            button1.Text = I18N.GetString("Connect");
            button2.Text = I18N.GetString("Test");
            button3.Text = I18N.GetString("Statistics");
            button4.Text = I18N.GetString("Server_config");
            button5.Text = I18N.GetString("Account");
        }
        
        private void LoadServer()
        {
            JObject jo =  Login.GetLoginJObject();

            if (jo!=null)
            {
                string level = jo["data"]["class"].ToString();
                string traffic_remain = jo["data"]["unusedTraffic"].ToString();
                string due_date = jo["data"]["class_expire"].ToString();
                label7.Text = "Lv "+level;
                label5.Text = traffic_remain;
                label3.Text = due_date;
            }

            Configuration c = controller.GetConfiguration();
            for (int i=0; i < c.configs.Count; i++)
            {
                Server s = c.configs[i];
                string[] item = { s.FriendlyName(), "", "" };
                ListViewItem itm = new ListViewItem(item);
                //itm.BackColor = Color.Silver;
                //itm.ForeColor = Color.White;
                //itm.
                if (c.index == i)
                {
                    itm.BackColor = Color.FromArgb(10, 36, 106);
                    itm.ForeColor = Color.White;
                }
                listView1.Items.Add(itm);
            }
            switch (c.sysProxyMode)
            {
                case 1:
                    comboBox1.SelectedIndex = 0;
                    button1.Text = I18N.GetString("Click To Start");
                    break;
                case 2:
                    comboBox1.SelectedIndex = 1;
                    button1.Text = I18N.GetString("Click To Stop");
                    break;
                case 3:
                    comboBox1.SelectedIndex = 2;
                    button1.Text = I18N.GetString("Click To Stop");
                    break;
            }

        }

        private void DisconnectCurrent(object sender, EventArgs e)
        {
            Configuration config = controller.GetCurrentConfiguration();
            for (int id = 0; id < config.configs.Count; ++id)
            {
                Server server = config.configs[id];
                server.GetConnections().CloseAll();
            }
        }

        private void listView1_SelectedIndexChanged(object sender, EventArgs e)
        {
            //MessageBox.Show("hi");
            if (listView1.SelectedItems.Count == 0)
            {
                return;
            }
            else if (listView1.SelectedItems.Count == 1)
            {
                //MessageBox.Show(Convert.ToString(listView1.SelectedItems.Count));
                //_modifiedConfiguration.index = listView1.SelectedItems[0].Index;
                //_modifiedConfiguration
                
                //int this_index = listView1.SelectedItems[0].Index;
                
                //controller.SelectServerIndex(this_index);
                //listView1.Items[this_index].BackColor = Color.Silver;
                //DisconnectCurrent(this, new EventArgs());
            }
            
        }

        private void listView1_DoubleClick(object sender, EventArgs e)
        {
            if (listView1.SelectedItems.Count == 0)
            {
                return;
            }
            else if (listView1.SelectedItems.Count == 1)
            {
                //MessageBox.Show(Convert.ToString(listView1.SelectedItems.Count));
                //_modifiedConfiguration.index = listView1.SelectedItems[0].Index;
                //_modifiedConfiguration

                int last_index = controller.GetConfiguration().index;
                int this_index = listView1.SelectedItems[0].Index;

                listView1.Items[last_index].BackColor = Color.White;
                listView1.Items[last_index].ForeColor = Color.Black;
                controller.SelectServerIndex(this_index);
                listView1.Items[this_index].BackColor = Color.FromArgb(10, 36, 106);
                listView1.Items[this_index].ForeColor = Color.White;
                DisconnectCurrent(this, new EventArgs());
            }

        }
        private void button1_Click(object sender, EventArgs e)
        {
            Configuration c = controller.GetConfiguration();
            
            if (listView1.SelectedItems.Count>0)
            {
                int last_index = controller.GetConfiguration().index;
                int this_index = listView1.SelectedItems[0].Index;

                if (last_index != this_index)
                {
                    listView1.Items[last_index].BackColor = Color.White;
                    listView1.Items[last_index].ForeColor = Color.Black;
                    controller.SelectServerIndex(this_index);
                    listView1.Items[this_index].BackColor = Color.FromArgb(10, 36, 106);
                    listView1.Items[this_index].ForeColor = Color.White;
                    DisconnectCurrent(this, new EventArgs());
                }
            }

            if (comboBox1.SelectedIndex != c.sysProxyMode - 1)
            {
                if (comboBox1.SelectedIndex == 0)
                {
                    button1.Text = I18N.GetString("Click To Start");
                }
                else
                {
                    button1.Text = I18N.GetString("Click To Stop");
                }
                switch (comboBox1.SelectedIndex)
                {
                    case 0:
                        controller.ToggleMode(ProxyMode.Direct);
                        break;
                    case 1:
                        controller.ToggleMode(ProxyMode.Pac);
                        break;
                    case 2:
                        controller.ToggleMode(ProxyMode.Global);
                        break;
                }
            }
            else
            {
                // 同一模式，点击该按钮

                switch (comboBox1.SelectedIndex)
                {
                    case 0:
                        button1.Text = I18N.GetString("Click To Stop");
                        comboBox1.SelectedIndex = 1;
                        controller.ToggleMode(ProxyMode.Pac);
                        break;
                    case 1:
                        button1.Text = I18N.GetString("Click To Start");
                        comboBox1.SelectedIndex = 0;
                        controller.ToggleMode(ProxyMode.Direct);
                        break;
                    case 2:
                        button1.Text = I18N.GetString("Click To Start");
                        comboBox1.SelectedIndex = 0;
                        controller.ToggleMode(ProxyMode.Direct);
                        break;
                }
            }
        }

        private void show_ping_start()
        {
            Thread.Sleep(5000);
            Configuration c = controller.GetConfiguration();
            for (int i = 0; i < listView1.Items.Count; i++)
            {
                //Console.WriteLine("1: " + Convert.ToString(i));
                string ip = c.configs[i].server;
                int port = c.configs[i].server_port;

                my_param m = new my_param();
                m.i = i;
                m.ip = ip;
                m.method = 0;
                Thread t = new Thread(new ParameterizedThreadStart(inthread));
                t.Start(m);
                my_param mm = new my_param();
                mm.i = i;
                mm.ip = ip;
                mm.port = port;
                mm.method = 1;
                Thread tt = new Thread(new ParameterizedThreadStart(inthread));
                tt.Start(mm);
            }
        }

        private void show_ping()
        {
            
            Configuration c = controller.GetConfiguration();
            for (int i = 0; i < listView1.Items.Count; i++)
            {
                //Console.WriteLine("1: " + Convert.ToString(i));
                string ip = c.configs[i].server;
                int port = c.configs[i].server_port;

                my_param m = new my_param();
                m.i = i;
                m.ip = ip;
                m.method = 0;
                Thread t = new Thread(new ParameterizedThreadStart(inthread));
                t.Start(m);
                my_param mm = new my_param();
                mm.i = i;
                mm.ip = ip;
                mm.port = port;
                mm.method = 1;
                Thread tt = new Thread(new ParameterizedThreadStart(inthread));
                tt.Start(mm);
            }
        }
        private void inthread(object obj)
        {
            my_param o = (my_param)obj;
            if (o.method == 0)
            {
                string resp = Utils.ping_example_3(o.ip);
                if (this.IsHandleCreated)
                {
                    listView1.BeginInvoke(new treeinvoke(() =>
                    {
                        listView1.Items[o.i].SubItems[1].Text = resp;
                    }));
                }
                else
                {
                    return;
                }
                //listView1.BeginInvoke(new treeinvoke(() =>
                //{
                //    listView1.Items[o.i].SubItems[1].Text = resp;
                //}));

            }
            else if (o.method == 1)
            {
                string resp = Utils.tcping_example_3(o.ip, o.port);
                if (this.IsHandleCreated)
                {
                    listView1.BeginInvoke(new treeinvoke(() =>
                    {
                        listView1.Items[o.i].SubItems[2].Text = resp;
                    }));
                }
                else
                {
                    return;
                }
                //listView1.BeginInvoke(new treeinvoke(() =>
                //{
                //    listView1.Items[o.i].SubItems[2].Text = resp;
                //}));
            }
            //Console.WriteLine("inthread: " + Convert.ToString(o.i));

        }

        private void button2_Click(object sender, EventArgs e)
        {
            //Thread t = new Thread(new ThreadStart(show_ping));
            //t.Start();
            if (ConnectTest.hasInternetAccess())
            {
                show_ping();
            }
            
        }

        void serverLogForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            serverLogForm = null;
            Util.Utils.ReleaseMemory();
        }
        void configForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            configForm = null;
            Util.Utils.ReleaseMemory();
        }
        void accountForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            accountForm = null;
        }
        private void ShowServerLogForm()
        {
            if (serverLogForm != null)
            {
                serverLogForm.Activate();
                serverLogForm.Update();
                if (serverLogForm.WindowState == FormWindowState.Minimized)
                {
                    serverLogForm.WindowState = FormWindowState.Normal;
                }
            }
            else
            {
                serverLogForm = new ServerLogForm(controller);
                serverLogForm.Show();
                serverLogForm.Activate();
                serverLogForm.BringToFront();
                serverLogForm.FormClosed += serverLogForm_FormClosed;
            }
        }
        private void ShowConfigForm(bool addNode)
        {
            if (configForm != null)
            {
                configForm.Activate();
                // MessageBox.Show("1");
                if (addNode)
                {
                    Configuration cfg = controller.GetCurrentConfiguration();
                    configForm.SetServerListSelectedIndex(cfg.index + 1);
                }
            }
            else
            {
                //MessageBox.Show("2.0");
                configForm = new ConfigForm(controller, updateChecker, addNode ? -1 : -2);
                //MessageBox.Show("2.1");
                configForm.Show();
                //MessageBox.Show("2.2");
                configForm.Activate();
                configForm.BringToFront();
                configForm.FormClosed += configForm_FormClosed;
            }
        }
        private void ShowAccountForm()
        {
            if (accountForm != null)
            {
                accountForm.Activate();
                accountForm.Update();
                if (accountForm.WindowState == FormWindowState.Minimized)
                {
                    accountForm.WindowState = FormWindowState.Normal;
                }
            }
            else
            {
                accountForm = new AccountForm(controller);
                accountForm.Show();
                accountForm.Activate();
                accountForm.BringToFront();
                accountForm.FormClosed += accountForm_FormClosed;
            }
        }
        private void button3_Click(object sender, EventArgs e)
        {
            ShowServerLogForm();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            ShowConfigForm(false);
        }

        private void button5_Click(object sender, EventArgs e)
        {
            ShowAccountForm();
        }
    }

    class my_param
    {
        public string ip;
        public Int32 port;
        public int i;
        public int method;
    }
}
