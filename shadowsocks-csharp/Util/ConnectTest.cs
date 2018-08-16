using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
//using System.U
using System.Runtime;
using System.Runtime.InteropServices;
using System.Windows.Forms;

using Shadowsocks.Model;
using Shadowsocks.Controller;


namespace Shadowsocks.Util
{
    class ConnectTest
    {
        // https://gist.github.com/richardchien/bc06a88d2fdf3033a9ce
        [DllImport("wininet.dll")]
        private extern static bool InternetGetConnectedState(int Description, int ReservedValue);

        // private ShadowsocksController controller;

        public static bool hasInternetAccess()
        {
            return InternetGetConnectedState(0, 0);
        }

        public static bool canUrlConnect(string testUrl, string toMatch, bool useProxy)
        {
            ShadowsocksController controller = new ShadowsocksController();
            Configuration config = controller.GetCurrentConfiguration();
            try
            {
                WebClient wclient = new WebClient();
                // wclient.BaseAddress = test_web;
                wclient.Encoding = Encoding.UTF8;
                wclient.Headers.Add("User-Agent", "Mozilla/5.0 (Windows NT 6.1; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/66.0.3359.117 Safari/537.36");
                wclient.Headers.Add("Content-Type", "application/x-www-form-urlencoded\r\n");

                if (useProxy)
                {
                    WebProxy proxy = new WebProxy(IPAddress.Loopback.ToString(), config.localPort);
                    if (!string.IsNullOrEmpty(config.authPass))
                    {
                        proxy.Credentials = new NetworkCredential(config.authUser, config.authPass);
                    }
                    wclient.Proxy = proxy;
                }

                string source = wclient.DownloadString(testUrl);

                return source.Contains(toMatch);
            }
            catch (Exception api_e)
            {
                MessageBox.Show(api_e.ToString());
                return false;
            }
        }

        public static string GetValidLoginUrl()
        {
            ShadowsocksController controller = new ShadowsocksController();
            Configuration config = controller.GetCurrentConfiguration();
            if (config.isDefaultConfig())
            {
                string testUrl = Configuration.GetDefaultUrl(0);
                if (ConnectTest.canUrlConnect(testUrl + "/mod_mu/func/ping", "ret", false))
                {
                    return testUrl;
                }
                else
                {
                    testUrl = Configuration.GetDefaultUrl(1);
                    if (ConnectTest.canUrlConnect(testUrl + "/mod_mu/func/ping", "ret", false))
                    {
                        return testUrl;
                    }
                }
            }
            else
            {
                string testUrl = config.ApiUrl;
                if (ConnectTest.canUrlConnect(testUrl + "/mod_mu/func/ping", "ret", false))
                {
                    return testUrl;
                }
                else if (ConnectTest.canUrlConnect(testUrl + "/mod_mu/func/ping", "ret", true))
                {
                    return testUrl;
                }
                else
                {
                    testUrl = Configuration.GetDefaultUrl(0);
                    if (ConnectTest.canUrlConnect(testUrl + "/mod_mu/func/ping", "ret", false))
                    {
                        return testUrl;
                    }
                    else if (ConnectTest.canUrlConnect(testUrl + "/mod_mu/func/ping", "ret", true))
                    {
                        return testUrl;
                    }
                    else
                    {
                        testUrl = Configuration.GetDefaultUrl(1);
                        if (ConnectTest.canUrlConnect(testUrl + "/mod_mu/func/ping", "ret", false))
                        {
                            return testUrl;
                        }
                        else if (ConnectTest.canUrlConnect(testUrl + "/mod_mu/func/ping", "ret", true))
                        {
                            return testUrl;
                        }
                    }
                }
            }
            return "";
        }

        public static string GetValidLoginUrlWithProxy(bool useProxy)
        {
            ShadowsocksController controller = new ShadowsocksController();
            Configuration config = controller.GetCurrentConfiguration();
            if (useProxy)
            {
                if (! config.isDefaultConfig())
                {
                    string testUrl = config.ApiUrl;
                    if (ConnectTest.canUrlConnect(testUrl + "/mod_mu/func/ping", "ret", true))
                    {
                        return testUrl;
                    }
                    else
                    {
                        testUrl = Configuration.GetDefaultUrl(0);
                        if (ConnectTest.canUrlConnect(testUrl + "/mod_mu/func/ping", "ret", true))
                        {
                            return testUrl;
                        }
                        else
                        {
                            testUrl = Configuration.GetDefaultUrl(1);
                            if (ConnectTest.canUrlConnect(testUrl + "/mod_mu/func/ping", "ret", true))
                            {
                                return testUrl;
                            }
                        }
                    }
                }
            }
            else
            {
                if (config.isDefaultConfig())
                {
                    string testUrl = Configuration.GetDefaultUrl(0);
                    if (ConnectTest.canUrlConnect(testUrl + "/mod_mu/func/ping", "ret", false))
                    {
                        return testUrl;
                    }
                    else
                    {
                        testUrl = Configuration.GetDefaultUrl(1);
                        if (ConnectTest.canUrlConnect(testUrl + "/mod_mu/func/ping", "ret", false))
                        {
                            return testUrl;
                        }
                    }
                }
                else
                {
                    string testUrl = config.ApiUrl;
                    if (ConnectTest.canUrlConnect(testUrl + "/mod_mu/func/ping", "ret", false))
                    {
                        return testUrl;
                    }
                    else
                    {
                        testUrl = Configuration.GetDefaultUrl(0);
                        if (ConnectTest.canUrlConnect(testUrl + "/mod_mu/func/ping", "ret", false))
                        {
                            return testUrl;
                        }
                        else
                        {
                            testUrl = Configuration.GetDefaultUrl(1);
                            if (ConnectTest.canUrlConnect(testUrl + "/mod_mu/func/ping", "ret", false))
                            {
                                return testUrl;
                            }
                        }
                    }
                }
            }
            return "";
        }

        public static string getValidLoginUrlByTcping(bool useProxy)
        {
            ShadowsocksController controller = new ShadowsocksController();
            Configuration config = controller.GetCurrentConfiguration();
            if (useProxy)
            {
                return "";
            }
            else
            {
                Uri testUrl = new Uri(config.ApiUrl);
                if (isValidServerPort(testUrl.Host, testUrl.Port))
                {
                    return config.ApiUrl;
                }
                else
                {
                    testUrl = new Uri(Configuration.GetDefaultUrl(0));
                    if (isValidServerPort(testUrl.Host, testUrl.Port))
                    {
                        return Configuration.GetDefaultUrl(0);
                    }
                    else
                    {
                        testUrl = new Uri(Configuration.GetDefaultUrl(1));
                        if (isValidServerPort(testUrl.Host, testUrl.Port))
                        {
                            return Configuration.GetDefaultUrl(1);
                        }
                    }
                }
            }

            return "";
        }

        public static bool isValidServerPort(string addr, int port)
        {
            if (Utils.tcping_example(addr, port) == 0)
            {
                if (Utils.tcping_example(addr, port) == 0)
                {
                    return false;
                }
            }
            return true;
        }

        public static bool isValidCurrentServer()
        {
            ShadowsocksController controller = new ShadowsocksController();
            Configuration config = controller.GetCurrentConfiguration();
            int i = config.index;

            return ConnectTest.isValidServerPort(config.configs[i].server, config.configs[i].server_port);
        }

        public static bool canLocalSocks5ProxyConnectBaidu()
        {
            if (!ConnectTest.isValidCurrentServer())
            {
                return false;
            }
            return ConnectTest.canUrlConnect("http://www.baidu.com", "html", true);
        }

        public static bool canLocalSocks5ProxyConnectGoogle()
        {
            if (!ConnectTest.isValidCurrentServer())
            {
                return false;
            }
            return ConnectTest.canUrlConnect("http://chrome.google.com", "html", true);
        }

        public static bool isValidLocalSocks5Proxy()
        {
            return ConnectTest.canLocalSocks5ProxyConnectBaidu();
        }
        
    }
}
