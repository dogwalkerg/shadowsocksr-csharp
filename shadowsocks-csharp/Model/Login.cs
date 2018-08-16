using System;
using System.Collections.Generic;

using System.Text;
using System.Windows.Forms;
using Shadowsocks.Controller;
using Shadowsocks.Util;

// For Api and json parse
using System.Net;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Shadowsocks.Model
{
    class Login
    {
        // private ShadowsocksController controller;

        public static JObject GetLoginJObject()
        {
            if (!ConnectTest.hasInternetAccess())
            {
                return null;
            }

            ShadowsocksController controller = new ShadowsocksController();

            Configuration config = controller.GetCurrentConfiguration();
            string email = config.ApiEmail, passwd = config.ApiPassword, website=string.Empty ;

            if (email == "" || passwd == "")
            {
                return null;
            }

            if (config.ApiUrl == "")
            {
                config.ApiUrl = Configuration.GetDefaultUrl(0);
                controller.SaveServersConfig(config);
            }

            bool use_proxy = config.ApiUpdateWithProxy;

            // 默认服务器无法更新， 故设为false
            if (config.index == 0 && config.configs[0].server == Configuration.GetDefaultServer().server)
            {
                use_proxy = false;
            }
            // 检测当前服务器是否可用
            if ( ! ConnectTest.isValidLocalSocks5Proxy())
            {
                use_proxy = false;
                MessageBox.Show("! ConnectTest.isValidLocalSocks5Proxy()");
                website = ConnectTest.getValidLoginUrlByTcping(use_proxy);
            }
            else
            {
                website = ConnectTest.GetValidLoginUrlWithProxy(use_proxy);
            }


            if (website == "")
            {
                return null;
            }

            string str = string.Empty, result = string.Empty;
            website += "/api/login";
            //MessageBox.Show(website+email+passwd);
            try
            {
                WebClient wclient = new WebClient();
                wclient.BaseAddress = website;
                wclient.Encoding = Encoding.UTF8;
                wclient.Headers.Add("User-Agent", "Mozilla/5.0 (Windows NT 6.1; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/66.0.3359.117 Safari/537.36");
                wclient.Headers.Add("Content-Type", "application/x-www-form-urlencoded\r\n");
                if (use_proxy)
                {
                    WebProxy proxy = new WebProxy(IPAddress.Loopback.ToString(), config.localPort);
                    //MessageBox.Show(IPAddress.Loopback.ToString()+config.localPort.ToString());
                    if (!string.IsNullOrEmpty(config.authPass))
                    {
                        proxy.Credentials = new NetworkCredential(config.authUser, config.authPass);
                    }
                    wclient.Proxy = proxy;
                }
                else
                {
                    wclient.Proxy = null;
                }

                string postData = "email=" + email + "&passwd=" + passwd;
                byte[] sendData = Encoding.GetEncoding("utf-8").GetBytes(postData.ToString());

                result = wclient.UploadString(website, postData);

                JObject jo = (JObject)JsonConvert.DeserializeObject(result);

                return jo;

            }
            catch (Exception api_e)
            {
                MessageBox.Show(api_e.ToString());
                return null;
            }
        }



    }
}
